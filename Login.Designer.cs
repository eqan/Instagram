﻿namespace Instagram
{
    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.statusSymbolPassword = new System.Windows.Forms.PictureBox();
            this.statusSymbolID = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.instagramLogo = new System.Windows.Forms.PictureBox();
            this.password_Box = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.userName_Box = new System.Windows.Forms.TextBox();
            this.userNameLabel = new System.Windows.Forms.Label();
            this.statusPassword = new System.Windows.Forms.Label();
            this.statusID = new System.Windows.Forms.Label();
            this.loginBtn = new Instagram.CustomButton();
            this.signUpBtn = new Instagram.CustomButton();
            ((System.ComponentModel.ISupportInitialize)(this.statusSymbolPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusSymbolID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.instagramLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // statusSymbolPassword
            // 
            this.statusSymbolPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.statusSymbolPassword.Location = new System.Drawing.Point(582, 248);
            this.statusSymbolPassword.Name = "statusSymbolPassword";
            this.statusSymbolPassword.Size = new System.Drawing.Size(39, 42);
            this.statusSymbolPassword.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.statusSymbolPassword.TabIndex = 79;
            this.statusSymbolPassword.TabStop = false;
            // 
            // statusSymbolID
            // 
            this.statusSymbolID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.statusSymbolID.Location = new System.Drawing.Point(582, 196);
            this.statusSymbolID.Name = "statusSymbolID";
            this.statusSymbolID.Size = new System.Drawing.Size(39, 42);
            this.statusSymbolID.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.statusSymbolID.TabIndex = 78;
            this.statusSymbolID.TabStop = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(290, 276);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(275, 3);
            this.label2.TabIndex = 75;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(290, 217);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(275, 3);
            this.label1.TabIndex = 74;
            // 
            // instagramLogo
            // 
            this.instagramLogo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.instagramLogo.Image = ((System.Drawing.Image)(resources.GetObject("instagramLogo.Image")));
            this.instagramLogo.Location = new System.Drawing.Point(280, 77);
            this.instagramLogo.Name = "instagramLogo";
            this.instagramLogo.Size = new System.Drawing.Size(275, 55);
            this.instagramLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.instagramLogo.TabIndex = 73;
            this.instagramLogo.TabStop = false;
            // 
            // password_Box
            // 
            this.password_Box.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.password_Box.BackColor = System.Drawing.Color.Black;
            this.password_Box.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.password_Box.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.password_Box.ForeColor = System.Drawing.Color.White;
            this.password_Box.Location = new System.Drawing.Point(290, 259);
            this.password_Box.Name = "password_Box";
            this.password_Box.Size = new System.Drawing.Size(275, 15);
            this.password_Box.TabIndex = 68;
            this.password_Box.UseSystemPasswordChar = true;
            this.password_Box.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.passwordBox_KeyPress);
            // 
            // passwordLabel
            // 
            this.passwordLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordLabel.ForeColor = System.Drawing.Color.White;
            this.passwordLabel.Location = new System.Drawing.Point(178, 259);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(102, 28);
            this.passwordLabel.TabIndex = 67;
            this.passwordLabel.Text = "Password:";
            // 
            // userName_Box
            // 
            this.userName_Box.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.userName_Box.BackColor = System.Drawing.Color.Black;
            this.userName_Box.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.userName_Box.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userName_Box.ForeColor = System.Drawing.Color.White;
            this.userName_Box.Location = new System.Drawing.Point(290, 199);
            this.userName_Box.Name = "userName_Box";
            this.userName_Box.Size = new System.Drawing.Size(275, 15);
            this.userName_Box.TabIndex = 66;
            // 
            // userNameLabel
            // 
            this.userNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.userNameLabel.AutoSize = true;
            this.userNameLabel.BackColor = System.Drawing.Color.Black;
            this.userNameLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userNameLabel.ForeColor = System.Drawing.Color.White;
            this.userNameLabel.Location = new System.Drawing.Point(162, 199);
            this.userNameLabel.Name = "userNameLabel";
            this.userNameLabel.Size = new System.Drawing.Size(118, 28);
            this.userNameLabel.TabIndex = 65;
            this.userNameLabel.Text = "User Name:";
            // 
            // statusPassword
            // 
            this.statusPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.statusPassword.AutoSize = true;
            this.statusPassword.Font = new System.Drawing.Font("CarnacW03-ExtraBold", 8.249999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusPassword.ForeColor = System.Drawing.Color.White;
            this.statusPassword.Location = new System.Drawing.Point(630, 264);
            this.statusPassword.Name = "statusPassword";
            this.statusPassword.Size = new System.Drawing.Size(0, 14);
            this.statusPassword.TabIndex = 86;
            // 
            // statusID
            // 
            this.statusID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.statusID.AutoSize = true;
            this.statusID.Font = new System.Drawing.Font("CarnacW03-ExtraBold", 8.249999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusID.ForeColor = System.Drawing.Color.White;
            this.statusID.Location = new System.Drawing.Point(630, 217);
            this.statusID.Name = "statusID";
            this.statusID.Size = new System.Drawing.Size(0, 14);
            this.statusID.TabIndex = 85;
            // 
            // loginBtn
            // 
            this.loginBtn.BackColor = System.Drawing.Color.DodgerBlue;
            this.loginBtn.BackgroundColor = System.Drawing.Color.DodgerBlue;
            this.loginBtn.BorderColor = System.Drawing.Color.DodgerBlue;
            this.loginBtn.BorderRadius = 10;
            this.loginBtn.BorderSize = 0;
            this.loginBtn.FlatAppearance.BorderSize = 0;
            this.loginBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loginBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginBtn.ForeColor = System.Drawing.Color.White;
            this.loginBtn.Location = new System.Drawing.Point(303, 305);
            this.loginBtn.Name = "loginBtn";
            this.loginBtn.Size = new System.Drawing.Size(118, 48);
            this.loginBtn.TabIndex = 87;
            this.loginBtn.Text = "Login";
            this.loginBtn.TextColor = System.Drawing.Color.White;
            this.loginBtn.UseVisualStyleBackColor = false;
            this.loginBtn.Click += new System.EventHandler(this.login_Click);
            // 
            // signUpBtn
            // 
            this.signUpBtn.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.signUpBtn.BackgroundColor = System.Drawing.Color.DeepSkyBlue;
            this.signUpBtn.BorderColor = System.Drawing.Color.DodgerBlue;
            this.signUpBtn.BorderRadius = 10;
            this.signUpBtn.BorderSize = 0;
            this.signUpBtn.FlatAppearance.BorderSize = 0;
            this.signUpBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.signUpBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.signUpBtn.ForeColor = System.Drawing.Color.White;
            this.signUpBtn.Location = new System.Drawing.Point(427, 305);
            this.signUpBtn.Name = "signUpBtn";
            this.signUpBtn.Size = new System.Drawing.Size(118, 48);
            this.signUpBtn.TabIndex = 88;
            this.signUpBtn.Text = "Sign Up";
            this.signUpBtn.TextColor = System.Drawing.Color.White;
            this.signUpBtn.UseVisualStyleBackColor = false;
            this.signUpBtn.Click += new System.EventHandler(this.signUp_Click);
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(786, 457);
            this.Controls.Add(this.signUpBtn);
            this.Controls.Add(this.loginBtn);
            this.Controls.Add(this.statusPassword);
            this.Controls.Add(this.statusID);
            this.Controls.Add(this.statusSymbolPassword);
            this.Controls.Add(this.statusSymbolID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.instagramLogo);
            this.Controls.Add(this.password_Box);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.userName_Box);
            this.Controls.Add(this.userNameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Login";
            this.Text = "Login";
            ((System.ComponentModel.ISupportInitialize)(this.statusSymbolPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusSymbolID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.instagramLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox statusSymbolPassword;
        private System.Windows.Forms.PictureBox statusSymbolID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox instagramLogo;
        private System.Windows.Forms.TextBox password_Box;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox userName_Box;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.Label statusPassword;
        private System.Windows.Forms.Label statusID;
        private CustomButton loginBtn;
        private CustomButton signUpBtn;
    }
}