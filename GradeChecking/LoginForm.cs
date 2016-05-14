using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;

namespace GradeChecking
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
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


        static string ToTwoDigits(string date)
        {
            if (date.Length == 1)
                date = "0" + date;
            if (date.Length > 2)
                date = date.Substring(date.Length - 2);
            return date;
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            string shows = GetSourceForMyShowsPage(textBoxUsername.Text,textBoxPassword.Text);
            string date = ToTwoDigits(DateTime.Now.Month.ToString()) + "-" + ToTwoDigits(DateTime.Now.Day.ToString())  + "-" + ToTwoDigits(DateTime.Now.Year.ToString());
            if (shows.Contains(date))
                labelPassword.Text = "Grade has been updated";

            List<int> updated = AllIndexesOf(shows, date);
            foreach(int value in updated)
            {
                string substring = shows.Substring(0, value);
                int endPosition = substring.LastIndexOf("</a>");
                int startPosition = substring.LastIndexOf("nsd.org");
                labelPassword.Text += substring.Substring(startPosition + 9, endPosition - startPosition - 9);
            }

			if (checkBoxRememberInfo.Checked)
			{
				using (StreamWriter streamWriter = new StreamWriter("info.txt"))
				{
					streamWriter.Write(textBoxUsername.Text + "," + textBoxPassword.Text);
				}
			}
        }
        
        public static List<int> AllIndexesOf(string str, string value) 
        {
            List<int> indexes = new List<int>();
            for (int index = 0;; index += value.Length) 
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

		private void GradeChecker_Load(object sender, EventArgs e)
		{
			StreamReader streamReader = new StreamReader("info.txt");
			string line = streamReader.ReadLine();
			if (line != null && line != "")
			{
				string[] properties = line.Split(',');
				textBoxUsername.Text = properties[0];
				textBoxPassword.Text = properties[1];
			}
			streamReader.Close();
		}
    }
}
