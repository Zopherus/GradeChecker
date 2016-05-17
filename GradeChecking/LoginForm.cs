using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GradeChecking
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string shows = GetSourceForMyShowsPage(textBoxUsername.Text, textBoxPassword.Text);
            if (shows.Contains("Not Authorized"))
            {
                MessageBox.Show("Incorrect username or password");
                return;
            }

            if (checkBoxRememberInfo.Checked)
            {
                using (StreamWriter streamWriter = new StreamWriter("info.txt"))
                {
                    string uname = Base64Encryption.Encode(textBoxUsername.Text);
                    string pword = Base64Encryption.Encode(textBoxPassword.Text);
                    streamWriter.Write(uname + "," + pword);
                }
            }
            Program.uname = textBoxUsername.Text;
            Program.pword = textBoxPassword.Text;
            this.Close();
        }

        static string GetSourceForMyShowsPage(string uname, string pword)
        {
            using (var client = new WebClientEx())
            {
                var values = new NameValueCollection
                {
                    { "uname", uname },
                    { "pword", pword },
                };
                client.UploadValues("https://grades.nsd.org//login.php", values);
                return client.DownloadString("https://grades.nsd.org/showreportcard.php");
            }
        }

    }
}
