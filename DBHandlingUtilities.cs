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
            string databaseFileName = "InstagramDB.mdf";
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB; AttachDbFilename={path}{databaseFileName}; Integrated Security=True; Connect Timeout=10000";

            dbConnection = new SqlConnection(connectionString);
        }

        public void Get_Picture()
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
                    openFileDialog.Filter = "Image Files (*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.tif;*.tiff)|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.tif;*.tiff";
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

        public int Return_User_ID(string userName)
        {
            int userID = -1;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    Console.WriteLine(userName);
                    dbConnection.Open();
                    cmd = new SqlCommand("SELECT UserID FROM USERS WHERE UserName = '" + userName + "'", dbConnection);
                    userID = (int)cmd.ExecuteScalar();
                    Console.WriteLine("User ID: {0}", userID);
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
            return userID;
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
                    results[0] = (int)cmd.ExecuteScalar();
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
        public void Remove_Post(string userID, string userName, string postId)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("DELETE FROM  " + userName + "_" + userID + "_PostTable WHERE PostID = @postId", dbConnection);
                    cmd.Parameters.AddWithValue("@postId", postId);
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    Console.WriteLine("Post Removed", userName);
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
            tableName = "Build_User_Following_Table";
            Create_Entity(userID, userName, tableName);
            tableName = "Build_User_Posts_Table";
            Create_Entity(userID, userName, tableName);
            tableName = "Build_User_Activity_Table";
            Create_Entity(userID, userName, tableName);
            Create_Uniqueness_Constraint(userID, userName, "Activity", "UserID, PostID, ActivityType");
            Create_Relationship_With_Self(userID, userName);

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
                    cmd = new SqlCommand("CREATE TABLE " + userName + "_" + userID + "_" + postID.ToString() + "_LikesRecord ( ID INT IDENTITY(1,1) PRIMARY KEY, UserID INT UNIQUE NOT NULL, UserName VARCHAR(MAX), FOREIGN KEY (UserID) REFERENCES Users(UserID))", dbConnection);
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
                    cmd = new SqlCommand("CREATE TABLE " + userName + "_" + userID + "_" + postID.ToString() + "_BookMarksRecord ( ID INT IDENTITY(1,1) PRIMARY KEY,UserID INT UNIQUE NOT NULL, UserName VARCHAR(MAX))", dbConnection);
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

        public void Create_Relationship_With_Self(string userID, string userName)
        {
            Add_Follower(userID, userName, userID, userName);
            Add_Following(userID, userName, userID, userName);
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
            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }
            try
            {
                cmd = new SqlCommand("TRUNCATE TABLE TemporaryTable_For_All_Posts", dbConnection);
                cmd.ExecuteNonQuery();
                dbConnection.Close();
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
            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }
            try
            {
                string tableName = "TemporaryTable_For_All_Posts";
                cmd = new SqlCommand("INSERT INTO " + tableName + " SELECT " + userID + "AS UserID, '" + userName + "' AS UserName, PostID, PostDesc, Location, Image, Video, TimeLine FROM " + userName + "_" + userID + "_PostTable", dbConnection);
                cmd.ExecuteNonQuery();
                dbConnection.Close();
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
            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();
            try
            {
                cmd = new SqlCommand("INSERT INTO " + userName + "_" + userID + "_FollowingTable (UserID, UserName) VALUES(@FollowingID, @FollowingName)", dbConnection);
                Console.WriteLine("User Id" + userID + userName + followingID + followingName);
                cmd.Parameters.AddWithValue("@FollowingID", Int32.Parse(followingID));
                cmd.Parameters.AddWithValue("@FollowingName", followingName);
                cmd.ExecuteNonQuery();
                dbConnection.Close();
                Console.WriteLine("{0} As Follower Added!", userName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (dbConnection.State == ConnectionState.Open)
                dbConnection.Close();
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
            Console.WriteLine("I'm here" + ImageData.ToString());
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

        public Image[] Retrieve_All_Pictures(string tableName, string columnNameForIndex = null, string orderBy = null)
        {
            Image[] image = null;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();
                string sqlCmd;
                if (columnNameForIndex != null)
                    sqlCmd = "SELECT Image, " + columnNameForIndex + " FROM " + tableName + " " + orderBy;
                else
                    sqlCmd = "SELECT Image FROM " + tableName;
                cmd = new SqlCommand(sqlCmd, dbConnection);
                adapt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapt.Fill(ds);
                image = new Image[ds.Tables[0].Rows.Count];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    image[i] = Convert_Bytes_To_Image((byte[])ds.Tables[0].Rows[i]["Image"]);
                    if (columnNameForIndex != null)
                        image[i].Tag = ((int)ds.Tables[0].Rows[i][columnNameForIndex]).ToString();
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
                    string sqlCmd = "CREATE TRIGGER TR_Add_Post_Like_Activity_For_" + tableName + "_" + postID + " ON " + tableName + "_" + postID + "_LikesRecord FOR INSERT AS DECLARE @UserID INT, @UserName VARCHAR(MAX); SELECT @UserID = ins.UserID FROM INSERTED ins; SELECT @UserName = ins.UserName FROM INSERTED ins; INSERT INTO " + tableName + "_Activity ( UserID, UserName, PostID, ActivityType ) VALUES ( @UserID, @UserName, " + postID + ",'Liked')";
                    cmd = new SqlCommand(sqlCmd, dbConnection);
                    cmd.ExecuteNonQuery();
                    sqlCmd = "CREATE TRIGGER TR_Add_Post_BookMark_Activity_For_" + tableName + "_" + postID + " ON " + tableName + "_" + postID + "_BookMarksRecord FOR INSERT AS DECLARE @UserID INT, @UserName VARCHAR(MAX); SELECT @UserID = ins.UserID FROM INSERTED ins; SELECT @UserName = ins.UserName FROM INSERTED ins; INSERT INTO " + tableName + "_Activity ( UserID, UserName, PostID, ActivityType ) VALUES ( @UserID, @UserName, " + postID + ",'BookMarked')";
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


        public bool Add_User(string userName, string realUserName, string userPassword, string imageLocation = null, string tagLine = null)
        {
            bool decision = false;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("Add_User", dbConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (imageLocation == null)
                    {
                        imageLocation = Environment.CurrentDirectory + @"\Assets\Avatar.png";
                    }
                    cmd.Parameters.AddWithValue("@Picture", Get_Binary_Of_File());
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@RealUserName", realUserName);
                    cmd.Parameters.AddWithValue("@UserPassword", userPassword);
                    cmd.Parameters.AddWithValue("@Tagline", tagLine);
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    Console.WriteLine("{0} Added!", userName);
                    decision = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                decision = false;
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            return decision;
        }

        public bool Update_User(string userID, string userName, string realUserName, string userPassword, string imageLocation = null, string tagLine = null)
        {
            bool decision = false;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("UPDATE USERS SET UserName = @UserName, UserPassword = @UserPassword, RealUserName = @RealUserName, ProfilePicture =  @ProfilePicture, TagLine = @TagLine where UserID = @UserID", dbConnection);
                    if (imageLocation == null)
                    {
                        imageLocation = Environment.CurrentDirectory + @"\Assets\Avatar.png";
                    }
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@RealUserName", realUserName);
                    cmd.Parameters.AddWithValue("@UserPassword", userPassword);
                    cmd.Parameters.AddWithValue("@Tagline", tagLine);
                    cmd.Parameters.AddWithValue("@ProfilePicture", Get_Binary_Of_File());
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    Console.WriteLine("{0} Added!", userName);
                    decision = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                decision = false;
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            return decision;
        }

        public bool Add_Like(string userID, string userName, string followingID, string followingName, string postID)
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
            return false;
        }

        public bool Add_BookMark(string userID, string userName, string followingID, string followingName, string postID)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("INSERT INTO " + followingName + "_" + followingID + "_" + postID + "_BookMarksRecord (UserID, UserName) VALUES ( " + userID + ", '" + userName + "' )", dbConnection);
                    int done = cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    Console.WriteLine("BookMark Added! for {0} to {1}", userName, followingName);
                    return done > 0;
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
            return false;
        }

        public void Create_Uniqueness_Constraint(string userID, string userName, string tableName, string args)
        {
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    cmd = new SqlCommand("CREATE UNIQUE INDEX UQ_" + userName + "_" + userID + "_" + tableName + " ON " + userName + "_" + userID + "_" + tableName + "( " + args + ");", dbConnection);
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    Console.WriteLine("Add Uniqueness Constraint! {0} {1}", userName, tableName);
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

        public Image Retrieve_Image_From_Post(string userID, string userName, string postID)
        {
            Image image = null;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();
                string sqlCmd = "SELECT Image FROM " + userName + "_" + userID + "_PostTable" + " WHERE PostID = @PostID";
                cmd = new SqlCommand(sqlCmd, dbConnection);
                cmd.Parameters.AddWithValue("@PostID", postID);
                adapt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapt.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                    image = Convert_Bytes_To_Image((byte[])ds.Tables[0].Rows[0]["Image"]);
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return image;
        }

        public Post Return_Post(string userID, string userName, bool lightModeOn, string postID, Main main)
        {
            Post post = null;
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();
                string sqlCmd = "SELECT PostID,PostDesc, Location, Image, TimeLine FROM " + userName + "_" + userID + "_PostTable" + " WHERE PostID = @PostID";
                cmd = new SqlCommand(sqlCmd, dbConnection);
                cmd.Parameters.AddWithValue("@PostID", postID);
                adapt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapt.Fill(ds);
                post = new Post(userID, userName, lightModeOn);
                post.TopLevel = false;
                post.TopMost = true;
                post.followingID = userID;
                post.userNameLabel.Text = userName;
                Bitmap profilePicture = new Bitmap(Retrieve_Profile_Picture_Using_SQL(Int32.Parse(userID)));
                post.profilePictureBox.Bitmap = profilePicture;
                post.postID = postID;
                post.likeLabel.Text = Return_Total_Likes(userID, userName, postID).ToString() + " Likes";
                post.postPictureBox.Image = Convert_Bytes_To_Image((byte[])ds.Tables[0].Rows[0]["Image"]);
                post.postDescriptionBox.Text = ((string)ds.Tables[0].Rows[0]["PostDesc"]);
                post.locationLabel.Text = ((string)ds.Tables[0].Rows[0]["Location"]);
                post.main = main;
                if (userID != main.userID)
                {
                    post.menuBtn.Dispose();
                }
                DateTime totalTime = ((DateTime)ds.Tables[0].Rows[0]["TimeLine"]);
                TimeSpan t = DateTime.Now.Date.Subtract(totalTime.Date);
                if (t.TotalDays != 0)
                    post.timeLabel.Text = t.TotalDays.ToString() + " Days Ago";
                else if (t.TotalHours != 0)
                    post.timeLabel.Text = t.TotalHours.ToString() + " Hours Ago";
                else if (t.TotalMinutes != 0)
                    post.timeLabel.Text = t.TotalMinutes.ToString() + " Minutes Ago";
                else
                    post.timeLabel.Text = t.Seconds.ToString() + " Seconds Ago";
                if (Is_Post_Liked(post.followingID, post.userNameLabel.Text, userName, post.postID))
                {
                    post.likeBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Selected Mode\heart-selected.png");
                    //postList[i].MouseHover += new EventHandler((o, a) => postList[i].likeBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Selected Mode\heart-selected.png"));
                    //postList[i].MouseLeave += new EventHandler((o, a) => postList[i].likeBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Selected Mode\heart-selected.png"));
                }
                else
                {
                    if (lightModeOn)
                        post.likeBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Light Mode\UI Icons\heart.png");
                    else
                        post.likeBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Dark Mode\UI Icons\heart.png");
                }
                if (Is_Post_BookMarked(post.followingID, post.userNameLabel.Text, userName, post.postID))
                    post.bookMarkedBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Selected Mode\bookmark.png");
                else
                {
                    if (lightModeOn)
                        post.bookMarkedBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Light Mode\UI Icons\bookmark.png");
                    else
                        post.bookMarkedBtn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Dark Mode\UI Icons\bookmark.png");
                }
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return post;
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
                    activityList[i].postPicture.Image = Retrieve_Image_From_Post(userID, userName, ((Int32)ds.Tables[0].Rows[i]["PostID"]).ToString());
                    activityList[i].activityLabel.Text = "has " + (string)ds.Tables[0].Rows[i]["ActivityType"] + " your Post";
                    DateTime totalTime = ((DateTime)ds.Tables[0].Rows[i]["TimeLine"]);
                    TimeSpan t = DateTime.Now.Date.Subtract(totalTime.Date);
                    if (t.TotalDays > 0)
                        activityList[i].timeLabel.Text = t.TotalDays.ToString() + " Days Ago";
                    else if (t.TotalHours > 0)
                        activityList[i].timeLabel.Text = t.TotalHours.ToString() + " Hours Ago";
                    else if (t.TotalMinutes > 0)
                        activityList[i].timeLabel.Text = t.TotalMinutes.ToString() + " Minutes Ago";
                    else
                        activityList[i].timeLabel.Text = t.TotalSeconds.ToString() + " Seconds Ago";
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

        private Post[] Return_Posts_From_DataSet(string userID, string userName, bool lightModeOn, Main main, DataSet ds)
        {
            Post[] postList = null;
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
                postList[i].main = main;
                //if (userID != main.userID || userName != main.userName)
                {
                    postList[i].menuBtn.Visible = false;
                    postList[i].menuBtn.Dispose();
                    Console.WriteLine("I'm here");
                }
                DateTime totalTime = ((DateTime)ds.Tables[0].Rows[i]["TimeLine"]);
                TimeSpan t = DateTime.Now.Date.Subtract(totalTime.Date);
                if (t.TotalDays != 0)
                    postList[i].timeLabel.Text = t.TotalDays.ToString() + " Days Ago";
                else if (t.TotalHours != 0)
                    postList[i].timeLabel.Text = t.TotalHours.ToString() + " Hours Ago";
                else if (t.TotalMinutes != 0)
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
            return postList;
        }

        public Post[] Generate_Posts(string userID, string userName, bool lightModeOn, Main main)
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
                postList = Return_Posts_From_DataSet(userID, userName, lightModeOn, main, ds);
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

        public ProfilePreview[] Return_Search_Results(string input, Main main)
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
                        searchList[i] = new ProfilePreview(main);
                        searchList[i].profilePictureBox.Bitmap = new Bitmap(Retrieve_Profile_Picture_Using_SQL((Int32)ds.Tables[0].Rows[i]["UserID"]));
                        searchList[i].userNameLabel.Text = ((string)ds.Tables[0].Rows[i]["UserName"]);
                        searchList[i].realUserNameLabel.Text = ((string)ds.Tables[0].Rows[i]["RealUserName"]);
                        searchList[i].userId = ((int)ds.Tables[0].Rows[i]["UserID"]).ToString();
                        searchList[i].userName = ((string)ds.Tables[0].Rows[i]["UserName"]);
                        searchList[i].realUserName = ((string)ds.Tables[0].Rows[i]["RealUserName"]);
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

        public bool Check_If_User_Followed_Or_Not(string userID, string userName, string followingId)
        {
            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();
            try
            {
                string tableName = userName + "_" + userID + "_";
                string sqlCommand = "SELECT COUNT(*) FROM " + tableName + "FollowingTable where UserID = " + followingId;
                cmd = new SqlCommand(sqlCommand, dbConnection);
                int count = ((Int32)cmd.ExecuteScalar());
                Console.WriteLine("This is the data" + count);
                dbConnection.Close();
                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (dbConnection.State == ConnectionState.Open)
                dbConnection.Close();
            return false;
        }


        public bool Return_True_If_Added_To_Following_Otherwise_False_If_Removed(string userID, string userName, string followingId, string followingName = null)
        {
            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();
            try
            {
                string tableName = userName + "_" + userID + "_";
                bool checkOperation = Check_If_User_Followed_Or_Not(userID, userName, followingId);
                Console.WriteLine("This is the state:" + checkOperation);
                if (checkOperation)
                {
                    dbConnection.Open();
                    string sqlCommand = "DELETE FROM " + tableName + "FollowingTable where UserID = " + followingId;
                    cmd = new SqlCommand(sqlCommand, dbConnection);
                    cmd.ExecuteNonQuery();
                    dbConnection.Close();
                    return false;
                }
                else
                {
                    this.Add_Following(userID, userName, followingId, followingName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (dbConnection.State == ConnectionState.Open)
                dbConnection.Close();
            return false;
        }
        public List<string> Return_Profile_Information(string userID, string userName)
        {
            List<string> information = new List<string>();
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                    string tableName = userName + "_" + userID + "_";
                    string sqlCommand = "SELECT COUNT(*) FROM " + tableName + "FollowingTable";
                    cmd = new SqlCommand(sqlCommand, dbConnection);
                    information.Add(((Int32)cmd.ExecuteScalar()).ToString());
                    sqlCommand = "SELECT COUNT(*) FROM " + tableName + "FollowersTable";
                    cmd = new SqlCommand(sqlCommand, dbConnection);
                    information.Add(((Int32)cmd.ExecuteScalar()).ToString());
                    sqlCommand = "SELECT COUNT(*) FROM " + tableName + "PostTable";
                    cmd = new SqlCommand(sqlCommand, dbConnection);
                    information.Add(((Int32)cmd.ExecuteScalar()).ToString());
                    sqlCommand = "SELECT RealUserName, TagLine FROM USERS WHERE UserID = " + userID;
                    cmd = new SqlCommand(sqlCommand, dbConnection);
                    adapt = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapt.Fill(ds);
                    information.Add((string)ds.Tables[0].Rows[0]["RealUserName"]);
                    information.Add((string)ds.Tables[0].Rows[0]["TagLine"]);
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
            return information;
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
