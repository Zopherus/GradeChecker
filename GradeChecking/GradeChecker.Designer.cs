namespace GradeChecking
{
    partial class GradeChecker
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
			this.textBoxUsername = new System.Windows.Forms.TextBox();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.labelUsername = new System.Windows.Forms.Label();
			this.labelPassword = new System.Windows.Forms.Label();
			this.buttonCheck = new System.Windows.Forms.Button();
			this.checkBoxRememberInfo = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// textBoxUsername
			// 
			this.textBoxUsername.Location = new System.Drawing.Point(121, 37);
			this.textBoxUsername.Name = "textBoxUsername";
			this.textBoxUsername.Size = new System.Drawing.Size(100, 20);
			this.textBoxUsername.TabIndex = 0;
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Location = new System.Drawing.Point(121, 86);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.PasswordChar = '*';
			this.textBoxPassword.Size = new System.Drawing.Size(100, 20);
			this.textBoxPassword.TabIndex = 1;
			// 
			// labelUsername
			// 
			this.labelUsername.AutoSize = true;
			this.labelUsername.Location = new System.Drawing.Point(35, 44);
			this.labelUsername.Name = "labelUsername";
			this.labelUsername.Size = new System.Drawing.Size(61, 13);
			this.labelUsername.TabIndex = 2;
			this.labelUsername.Text = "Student ID:";
			// 
			// labelPassword
			// 
			this.labelPassword.AutoSize = true;
			this.labelPassword.Location = new System.Drawing.Point(38, 92);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(56, 13);
			this.labelPassword.TabIndex = 3;
			this.labelPassword.Text = "Password:";
			// 
			// buttonCheck
			// 
			this.buttonCheck.Location = new System.Drawing.Point(96, 157);
			this.buttonCheck.Name = "buttonCheck";
			this.buttonCheck.Size = new System.Drawing.Size(75, 23);
			this.buttonCheck.TabIndex = 4;
			this.buttonCheck.Text = "Check";
			this.buttonCheck.UseVisualStyleBackColor = true;
			this.buttonCheck.Click += new System.EventHandler(this.buttonCheck_Click);
			// 
			// checkBoxRememberInfo
			// 
			this.checkBoxRememberInfo.AutoSize = true;
			this.checkBoxRememberInfo.Location = new System.Drawing.Point(56, 123);
			this.checkBoxRememberInfo.Name = "checkBoxRememberInfo";
			this.checkBoxRememberInfo.Size = new System.Drawing.Size(95, 17);
			this.checkBoxRememberInfo.TabIndex = 5;
			this.checkBoxRememberInfo.Text = "Stay Signed In";
			this.checkBoxRememberInfo.UseVisualStyleBackColor = true;
			// 
			// GradeChecker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.checkBoxRememberInfo);
			this.Controls.Add(this.buttonCheck);
			this.Controls.Add(this.labelPassword);
			this.Controls.Add(this.labelUsername);
			this.Controls.Add(this.textBoxPassword);
			this.Controls.Add(this.textBoxUsername);
			this.Name = "GradeChecker";
			this.Text = "Grade Checker";
			this.Load += new System.EventHandler(this.GradeChecker_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Button buttonCheck;
		private System.Windows.Forms.CheckBox checkBoxRememberInfo;

    }
}

