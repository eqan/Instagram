﻿using System;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Instagram
{
    class DBHandlingUtilities
    {
        public SqlConnection dbConnection;
        public SqlCommand cmd;
        public SqlDataAdapter adapt;
        public string fileDirectory = null;

        public DBHandlingUtilities()
        {
            Initialize_Information();
        }

        public void Initialize_Information()
        {
            dbConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB; AttachDbFilename= E:\Instagram\Instagram\Instagram\InstagramDB.mdf; Integrated Security=True; Connect Timeout=30");
        }

        public void Get_Picture()
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
                    openFileDialog.Filter = "BMP|*.bmp|GIF|*.gif|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff";
                    openFileDialog.RestoreDirectory = true;
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        fileDirectory = openFileDialog.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public int[] Check_UserName_And_Password(string userName, string password)
        {
            int[] results = { 0, 0 };
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    // Check User
                    dbConnection.Open();
                    cmd = new SqlCommand(@"SELECT [dbo].[Check_UserName](@UserName);", dbConnection);
                    SqlParameter param1 = new SqlParameter();
                    param1 = new SqlParameter("@UserName", SqlDbType.VarChar);
                    param1.Value = userName;
                    cmd.Parameters.Add(param1);
                    results[0] = (Int32)cmd.ExecuteScalar();
                    Console.WriteLine("Result User: {0}", results[0].ToString());
                    dbConnection.Close();
                    if (results[0] == 1)
                    {
                        // Check Password
                        cmd = new SqlCommand(@"SELECT [dbo].[Check_Password](@UserName, @Password);", dbConnection);
                        SqlParameter param2 = new SqlParameter();
                        param2 = new SqlParameter("@UserName", SqlDbType.VarChar);
                        param2.Value = userName;
                        cmd.Parameters.Add(param2);
                        SqlParameter param3 = new SqlParameter();
                        param3 = new SqlParameter("@Password", SqlDbType.VarChar);
                        param3.Value = password;
                        cmd.Parameters.Add(param3);
                        dbConnection.Open();
                        results[1] = (Int32)cmd.ExecuteScalar();
                        Console.WriteLine("Result Password: {0}", results[1].ToString());
                        dbConnection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            return results;
        }

        public void Add_Post(string userID, string userName, string postDescription, string location)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("INSERT INTO " + userName + "_" + userID + "_PostTable (PostDesc, Location, Image) VALUES (@PostDesc, @Location, @Image)", dbConnection);
                    cmd.Parameters.AddWithValue("@PostDesc", postDescription);
                    cmd.Parameters.AddWithValue("@Location", location);
                    cmd.Parameters.AddWithValue("@Image", Get_Binary_Of_File());
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    Console.WriteLine("Post Created for {0}", userName);
                    Create_Post_Likes_Log(userID, userName);
                    Create_Post_BookMarks_Log(userID, userName);
                    Create_Trigger_For_Post_And_BookMarks(userID, userName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }

        }
        public void Add_Story(string userID, string userName)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("INSERT INTO " + userName + "_" + userID + "_StoryTable(Image, DueDate) VALUES(@Image, @DueDate)", dbConnection);
                    cmd.Parameters.AddWithValue("@DueDate", DateTime.Now.AddDays(1));
                    cmd.Parameters.AddWithValue("@Image", Get_Binary_Of_File());
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    Console.WriteLine("Story Added for {0}", userName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
        }

        public void Create_Valid_Story_View(string userID, string userName)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    string tableName = userName + "_" + userID + "_StoryTable";
                    string viewName = userName + "_" + userID + "_StoryViewTable";
                    string dropCmd = "IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo." + viewName + "') ) BEGIN DROP VIEW " + viewName + " END";
                    string addCmd = "CREATE VIEW " + viewName + " AS SELECT * FROM " + tableName + " WHERE DueDate >= CURRENT_TIMESTAMP ";
                    dbConnection.Open();
                    cmd = new SqlCommand(dropCmd, dbConnection);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(addCmd, dbConnection);
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    Console.WriteLine("Story View Created for {0}", userName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
        }



        public void Create_User_Entities(string userID, string userName)
        {
            string tableName = "Build_User_Followers_Table";
            Create_Entity(userID, userName, tableName);
            tableName = "Build_User_Stories_Table";
            Create_Entity(userID, userName, tableName);
            tableName = "Build_User_Followers_Table";
            Create_Entity(userID, userName, tableName);
            tableName = "Build_User_Posts_Table";
            Create_Entity(userID, userName, tableName);
            tableName = "Build_User_Activity_Table";
            Create_Entity(userID, userName, tableName);
        }

        private int Retrieve_Latest_PostID(string userID, string userName)
        {
            string sqlCmd = "SELECT PostID FROM " + userName + "_" + userID + "_PostTable";
            cmd = new SqlCommand(sqlCmd, dbConnection);
            adapt = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapt.Fill(ds);
            return (Int32)ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["PostID"];
        }

        public void Create_Post_Likes_Log(string userID, string userName)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    int postID = Retrieve_Latest_PostID(userID, userName);
                    cmd = new SqlCommand("CREATE TABLE " + userName + "_" + userID + "_" + postID.ToString() + "_LikesRecord ( ID INT IDENTITY(1,1) PRIMARY KEY,UserID INT UNIQUE, UserName VARCHAR(MAX))", dbConnection);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Like Record created for post {0} User {1}", postID, userName);
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (dbConnection.State == ConnectionState.Open)
                dbConnection.Close();
        }

        public void Create_Post_BookMarks_Log(string userID, string userName)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    int postID = Retrieve_Latest_PostID(userID, userName);
                    cmd = new SqlCommand("CREATE TABLE " + userName + "_" + userID + "_" + postID.ToString() + "_BookMarksRecord ( ID INT IDENTITY(1,1) PRIMARY KEY,UserID INT UNIQUE, UserName VARCHAR(MAX))", dbConnection);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("BookMarks Record created for post {0} User {1}", postID, userName);
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (dbConnection.State == ConnectionState.Open)
                dbConnection.Close();
        }

        public void Create_Entity(string userID, string userName, string procedureName)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand(procedureName, dbConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine(procedureName + " Table Created for {0}", userName);
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (dbConnection.State == ConnectionState.Open)
                dbConnection.Close();
        }

        public void Truncate_Temporary_Post_Table()
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("TRUNCATE TABLE TemporaryTable_For_All_Posts", dbConnection);
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (dbConnection.State == ConnectionState.Open)
                dbConnection.Close();
        }


        public void Create_View_For_Following_Posts(string userID, string userName)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    string tableName = "TemporaryTable_For_All_Posts";
                    cmd = new SqlCommand("SELECT ( CASE WHEN EXISTS(SELECT NULL FROM " + tableName + " )THEN 1 ELSE 0 END ) AS isEmpty", dbConnection);
                    int result = (Int32)cmd.ExecuteScalar();
                    if (result == 1)
                        cmd = new SqlCommand("SELECT " + userID + "AS UserID, '" + userName + "' AS UserName, PostID, PostDesc, Location, Image, Video, TimeLine  INTO " + tableName + "  FROM " + userName + "_" + userID + "_PostTable", dbConnection);
                    else
                        cmd = new SqlCommand("INSERT INTO " + tableName + " SELECT " + userID + "AS UserID, '" + userName + "' AS UserName, PostID, PostDesc, Location, Image, Video, TimeLine FROM " + userName + "_" + userID + "_PostTable", dbConnection);
                    Console.WriteLine(result);
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (dbConnection.State == ConnectionState.Open)
                dbConnection.Close();
        }

        public void Add_Follower(string userID, string userName, string followerID, string followerName)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("INSERT INTO " + userName + "_" + userID + "_FollowersTable (UserID, UserName) VALUES(@FollowerID, @FollowerName)", dbConnection);
                    cmd.Parameters.AddWithValue("@FollowerID", Int32.Parse(followerID));
                    cmd.Parameters.AddWithValue("@FollowerName", followerName);
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    Console.WriteLine("{0} As Following Added!", userName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
        }

        public void Add_Following(string userID, string userName, string followingID, string followingName)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("INSERT INTO " + userName + "_" + userID + "_FollowingTable (UserID, UserName) VALUES(@FollowingID, @FollowingName)", dbConnection);
                    cmd.Parameters.AddWithValue("@FollowingID", Int32.Parse(followingID));
                    cmd.Parameters.AddWithValue("@FollowingName", followingName);
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    Console.WriteLine("{0} As Follower Added!", userName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
        }

        public List<string[]> Import_Data_Using_SQL(string userID, string userName, string tableName)
        {
            List<string[]> data = new List<string[]>();
            string sqlCommand = "SELECT * FROM " + userName + "_" + userID + "_" + tableName;
            DataTable dataTable = Fetch_Records_In_DataTable(sqlCommand);
            foreach (DataRow row in dataTable.AsEnumerable().ToArray())
            {
                object[] dr1 = row.ItemArray;
                data.Add(Array.ConvertAll(dr1, (p => (p).ToString())));
            }
            return data;
        }

        public bool Check_If_Story_Exists(string userID, string userName)
        {
            bool decision = false;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("SELECT ( CASE WHEN NOT EXISTS(SELECT NULL FROM " + userName + "_" + userID + "_StoryViewTable" + " )THEN 1 ELSE 0 END ) AS isEmpty", dbConnection);
                    int result = (Int32)cmd.ExecuteScalar();
                    Console.WriteLine(result);
                    if (result == 0)
                        decision = true;
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            return decision;
        }


        public Image Convert_Bytes_To_Image(byte[] imgBytes)
        {
            Image img;
            using (MemoryStream ms = new MemoryStream(imgBytes))
            {
                img = Image.FromStream(ms);
            }
            return img;
        }

        public byte[] Get_Binary_Of_File()
        {
            FileStream fs;
            BinaryReader br;
            byte[] ImageData;
            fs = new FileStream(fileDirectory, FileMode.Open, FileAccess.Read);
            br = new BinaryReader(fs);
            ImageData = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close();
            return ImageData;
        }

        public Image Retrieve_Profile_Picture_Using_SQL(int userID)
        {
            Image image = null;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();
                string sqlCmd = "SELECT ProfilePicture FROM USERS WHERE UserID = @UserID";
                cmd = new SqlCommand(sqlCmd, dbConnection);
                cmd.Parameters.AddWithValue("@UserID", userID);
                adapt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapt.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    image = Convert_Bytes_To_Image((byte[])ds.Tables[0].Rows[0]["ProfilePicture"]);
                }
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return image;
        }

        public Image[] Retrieve_All_Pictures(string tableName)
        {
            Image[] image = null;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();
                string sqlCmd = "SELECT Image FROM " + tableName;
                cmd = new SqlCommand(sqlCmd, dbConnection);
                adapt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapt.Fill(ds);
                image = new Image[ds.Tables[0].Rows.Count];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    image[i] = Convert_Bytes_To_Image((byte[])ds.Tables[0].Rows[i]["Image"]);
                }
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return image;
        }

        public void Create_Trigger_For_Post_And_BookMarks(string userID, string userName)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    string postID = Retrieve_Latest_PostID(userID, userName).ToString();
                    string tableName = userName + "_" + userID;
                    string sqlCmd = "CREATE TRIGGER TR_Add_Post_Like_Activity_For_" + tableName + "_" + postID + " ON " + tableName + "_" + postID + "_LikesRecord FOR INSERT AS DECLARE @UserID INT, @UserName VARCHAR(MAX); SELECT @UserID = ins.UserID FROM INSERTED ins; SELECT @UserName = ins.UserName FROM INSERTED ins; INSERT INTO " + tableName + "_Activity ( UserID, UserName, PostID, ActivityType ) VALUES ( @UserID, @UserName, " + postID + ",'Like')";
                    cmd = new SqlCommand(sqlCmd, dbConnection);
                    cmd.ExecuteNonQuery();
                    sqlCmd = "CREATE TRIGGER TR_Add_Post_BookMark_Activity_For_" + tableName + "_" + postID + " ON " + tableName + "_" + postID + "_BookMarksRecord FOR INSERT AS DECLARE @UserID INT, @UserName VARCHAR(MAX); SELECT @UserID = ins.UserID FROM INSERTED ins; SELECT @UserName = ins.UserName FROM INSERTED ins; INSERT INTO " + tableName + "_Activity ( UserID, UserName, PostID, ActivityType ) VALUES ( @UserID, @UserName, " + postID + ",'BookMark')";
                    cmd = new SqlCommand(sqlCmd, dbConnection);
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    Console.WriteLine("Triggers for Posts and Bookmarks, Successfully created for User {0}", userName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public bool Add_User(string userName, string realUserName, string userPassword, string tagLine = null)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("Add_User", dbConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (fileDirectory != null)
                    {
                        cmd.Parameters.AddWithValue("@Picture", Get_Binary_Of_File());
                    }
                    else
                        cmd.Parameters.AddWithValue("@Picture", null);
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@RealUserName", realUserName);
                    cmd.Parameters.AddWithValue("@UserPassword", userPassword);
                    cmd.Parameters.AddWithValue("@Tagline", tagLine);
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    Console.WriteLine("{0} Added!", userName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            return true;
        }


        public void Add_Like(string userID, string userName, string followingID, string followingName, string postID)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("INSERT INTO " + followingName + "_" + followingID + "_" + postID + "_LikesRecord (UserID, UserName) VALUES ( " + userID + ", '" + userName + "' )", dbConnection);
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    Console.WriteLine("Like Added! for {0} to {1}", userName, followingName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
        }

        public void Add_BookMark(string userID, string userName, string followingID, string followingName, string postID)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("INSERT INTO " + followingName + "_" + followingID + "_" + postID + "_BookMarksRecord (UserID, UserName) VALUES ( " + userID + ", '" + userName + "' )", dbConnection);
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    Console.WriteLine("BookMark Added! for {0} to {1}", userName, followingName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
        }

        public Image Retrieve_Image_From_Post(string userID,string userName, string postID)
        {
            Image image = null;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();
                string sqlCmd = "SELECT Image FROM "+ userName + "_" + userID + "_PostTable" + " WHERE PostID = @PostID";
                cmd = new SqlCommand(sqlCmd, dbConnection);
                cmd.Parameters.AddWithValue("@PostID", postID);
                adapt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapt.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    image = Convert_Bytes_To_Image((byte[])ds.Tables[0].Rows[0]["Image"]);
                }
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return image;
        }

        public ActivityRow[] Generate_Activity(string userID, string userName, bool lightModeOn)
        {
            ActivityRow[] activityList = null;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();
                string sqlCmd = "SELECT * FROM " + userName + "_" + userID + "_Activity" + " ORDER BY TimeLine DESC";
                cmd = new SqlCommand(sqlCmd, dbConnection);
                adapt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapt.Fill(ds);
                activityList = new ActivityRow[ds.Tables[0].Rows.Count];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    activityList[i] = new ActivityRow(lightModeOn);
                    activityList[i].profilePictureBox.Bitmap = new Bitmap(Retrieve_Profile_Picture_Using_SQL((((Int32)ds.Tables[0].Rows[i]["UserID"]))));
                    activityList[i].userNameLabel.Text = (string)ds.Tables[0].Rows[i]["UserName"];
                    activityList[i].postPicture.Image = Retrieve_Image_From_Post(((Int32)ds.Tables[0].Rows[i]["UserID"]).ToString(),((string)ds.Tables[0].Rows[i]["UserName"]).ToString(),((Int32)ds.Tables[0].Rows[i]["PostID"]).ToString() );
                    activityList[i].activityLabel.Text = "has " + (string)ds.Tables[0].Rows[i]["ActivityType"] + "ed your Post";
                    DateTime totalTime = ((DateTime)ds.Tables[0].Rows[i]["TimeLine"]);
                    TimeSpan t = DateTime.Now.Date.Subtract(totalTime.Date);
                    if (t.TotalDays != 0)
                        activityList[i].timeLabel.Text = t.TotalDays.ToString() + " Days Ago";
                    else if (t.TotalHours != 0)
                        activityList[i].timeLabel.Text = t.TotalHours.ToString() + " Hours Ago";
                    else if (t.Minutes != 0)
                        activityList[i].timeLabel.Text = t.TotalMinutes.ToString() + " Minutes Ago";
                    else
                        activityList[i].timeLabel.Text = t.Seconds.ToString() + " Seconds Ago";
                }
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return activityList;
        }

        public Post[] Generate_Posts(string userID, string userName, bool lightModeOn)
        {
            Post[] postList = null;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();
                string sqlCmd = "SELECT * FROM TemporaryTable_For_All_Posts ORDER BY TimeLine DESC";
                cmd = new SqlCommand(sqlCmd, dbConnection);
                adapt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapt.Fill(ds);
                postList = new Post[ds.Tables[0].Rows.Count];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    postList[i] = new Post(userID, userName, lightModeOn);
                    Bitmap profilePicture = new Bitmap(Retrieve_Profile_Picture_Using_SQL((((Int32)ds.Tables[0].Rows[i]["UserID"]))));
                    postList[i].profilePictureBox.Bitmap = profilePicture;
                    postList[i].userNameLabel.Text = ((string)ds.Tables[0].Rows[i]["UserName"]);
                    postList[i].followingID = (((Int32)ds.Tables[0].Rows[i]["UserID"]).ToString());
                    postList[i].postID = (((Int32)ds.Tables[0].Rows[i]["PostID"]).ToString());
                    postList[i].likeLabel.Text = Return_Total_Likes(postList[i].followingID, postList[i].userNameLabel.Text, postList[i].postID).ToString() + " Likes";
                    postList[i].postPictureBox.Image = Convert_Bytes_To_Image((byte[])ds.Tables[0].Rows[i]["Image"]);
                    postList[i].postDescriptionBox.Text = ((string)ds.Tables[0].Rows[i]["PostDesc"]);
                    postList[i].locationLabel.Text = ((string)ds.Tables[0].Rows[i]["Location"]);
                    DateTime totalTime = ((DateTime)ds.Tables[0].Rows[i]["TimeLine"]);
                    TimeSpan t = DateTime.Now.Date.Subtract(totalTime.Date);
                    if (t.TotalDays != 0)
                        postList[i].timeLabel.Text = t.TotalDays.ToString() + " Days Ago";
                    else if (t.TotalHours != 0)
                        postList[i].timeLabel.Text = t.TotalHours.ToString() + " Hours Ago";
                    else if (t.Minutes != 0)
                        postList[i].timeLabel.Text = t.TotalMinutes.ToString() + " Minutes Ago";
                    else
                        postList[i].timeLabel.Text = t.Seconds.ToString() + " Seconds Ago";
                    if (Is_Post_Liked(postList[i].followingID, postList[i].userNameLabel.Text, userName, postList[i].postID))
                    {
                        postList[i].likeBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Selected Mode\heart-selected.png");
                        //postList[i].MouseHover += new EventHandler((o, a) => postList[i].likeBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Selected Mode\heart-selected.png"));
                        //postList[i].MouseLeave += new EventHandler((o, a) => postList[i].likeBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Selected Mode\heart-selected.png"));
                    }
                    else
                    {
                        if (lightModeOn)
                            postList[i].likeBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Light Mode\UI Icons\heart.png");
                        else
                            postList[i].likeBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Dark Mode\UI Icons\heart.png");
                    }
                    if (Is_Post_BookMarked(postList[i].followingID, postList[i].userNameLabel.Text, userName, postList[i].postID))
                        postList[i].bookMarkedBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Selected Mode\bookmark.png");
                    else
                    {
                        if (lightModeOn)
                            postList[i].bookMarkedBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Light Mode\UI Icons\bookmark.png");
                        else
                            postList[i].bookMarkedBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Dark Mode\UI Icons\bookmark.png");
                    }
                }
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return postList;
        }

        public bool Is_Post_Liked(string followingID, string followingName, string userName, string postID)
        {
            bool decision = false;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("SELECT ( CASE WHEN NOT EXISTS(SELECT NULL FROM " + followingName + "_" + followingID + "_" + postID + "_LikesRecord WHERE UserName = '" + userName + "' )THEN 1 ELSE 0 END ) AS isEmpty", dbConnection);
                    int result = (Int32)cmd.ExecuteScalar();
                    if (result == 0)
                    {
                        decision = true;
                        Console.WriteLine("Post {0} is Liked", postID);
                    }
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            return decision;
        }

        public bool Is_Post_BookMarked(string followingID, string followingName, string userName, string postID)
        {
            bool decision = false;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("SELECT ( CASE WHEN NOT EXISTS(SELECT NULL FROM " + followingName + "_" + followingID + "_" + postID + "_BookMarksRecord WHERE UserName = '" + userName + "' )THEN 1 ELSE 0 END ) AS isEmpty", dbConnection);
                    int result = (Int32)cmd.ExecuteScalar();
                    if (result == 0)
                    {
                        decision = true;
                        Console.WriteLine("Post {0} is bookmarked", postID);
                    }
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            return decision;
        }

        public ProfilePreview[] Return_Search_Results(string input, bool lightModeOn)
        {
            ProfilePreview[] searchList = null;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    string sqlCommand = "SELECT * FROM USERS WHERE UserName LIKE '" + input + "%' OR UserName = '" + input + "' OR RealUserName LIKE '" + input + "%' OR RealUserName = '" + input + "'";
                    cmd = new SqlCommand(sqlCommand, dbConnection);
                    adapt = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapt.Fill(ds);
                    searchList = new ProfilePreview[ds.Tables[0].Rows.Count];
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        searchList[i] = new ProfilePreview(lightModeOn);
                        searchList[i].profilePictureBox.Bitmap = new Bitmap(Retrieve_Profile_Picture_Using_SQL((Int32)ds.Tables[0].Rows[i]["UserID"]));
                        searchList[i].userNameLabel.Text = ((string)ds.Tables[0].Rows[i]["UserName"]);
                        searchList[i].realUserNameLabel.Text = ((string)ds.Tables[0].Rows[i]["RealUserName"]);
                    }
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            return searchList;
        }

        public int Return_Total_Likes(string followingID, string followingName, string postID)
        {
            int totalLikes = 0;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("Select COUNT(*) FROM " + followingName + "_" + followingID + "_" + postID + "_LikesRecord", dbConnection);
                    totalLikes = (Int32)cmd.ExecuteScalar();
                    dbConnection.Close();
                    Console.WriteLine("Total Likes {0}", totalLikes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            return totalLikes;
        }

        public DataTable Fetch_Records_In_DataTable(string command)
        {
            DataTable table = new DataTable();
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand(command, dbConnection);
                    adapt = new SqlDataAdapter(cmd);
                    adapt.Fill(table);
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            return table;
        }
    }
}
