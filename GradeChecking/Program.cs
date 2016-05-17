using System;
using System.Timers;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.Collections.Specialized;
using System.IO;

namespace GradeChecking
{
    static class Program
    {

        private static System.Timers.Timer timer;
        public static string uname;
        public static string pword;
        private static string result;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            StreamReader streamReader = new StreamReader("info.txt");
			string line = streamReader.ReadLine();
            streamReader.Close();
			if (line != null && line != "")
			{
				string[] properties = line.Split(',');
				uname = properties[0];
				pword = properties[1];
			}
            else
            {
                Application.Run(new LoginForm());
            }
            StartTimer();
            while (true)
            {

            }
        }


        private static void StartTimer()
        {
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timerElapsed);
            timer.Enabled = true;
        }

        static void timerElapsed(object sender, ElapsedEventArgs e)
        {
            string shows = GetSourceForMyShowsPage(uname,pword);
            string date = ToTwoDigits(DateTime.Now.Month.ToString()) + "-" + ToTwoDigits(DateTime.Now.Day.ToString())  + "-" + ToTwoDigits(DateTime.Now.Year.ToString());
            
            if (shows.Contains(date))
            {
                result = "";
                List<int> updated = AllIndexesOf(shows, date);
                foreach(int value in updated)
                {
                    string substring = shows.Substring(0, value);
                    int endPosition = substring.LastIndexOf("</a>");
                    int startPosition = substring.LastIndexOf("nsd.org");
                    result += substring.Substring(startPosition + 9, endPosition - startPosition - 9);
                }
                timer.Enabled = false;
                Application.Run(new ResultsForm(result));
                
            }

            
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
    }

    

    public class WebClientEx : WebClient
    {
        public CookieContainer CookieContainer { get; private set; }

        public WebClientEx()
        {
            CookieContainer = new CookieContainer();
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = CookieContainer;
            }
            return request;
        }
    }
}