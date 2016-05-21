using System;
using System.Timers;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;

namespace GradeChecking
{
    static class Program
    {

        private static System.Timers.Timer timer;
        public static string uname;
        public static string pword;
        private static string result;
        private static string date;
        private static List<string> teachersShown = new List<string>();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            Console.WriteLine(path);

            if (!File.Exists(path + @"\GradeChecking.lnk"))
            {
                using (StreamWriter writer = new StreamWriter(path + "\\" + "Gradechecking" + ".lnk"))
                {
                    string app = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine("URL=file:///" + app);
                    writer.WriteLine("IconIndex=0");
                    string icon = app.Replace('\\', '/');
                    writer.WriteLine("IconFile=" + icon);
                    writer.Flush();
                }
            }



            // Check if there already is a shortcut in this path and if not, create one




            date = ToTwoDigits(DateTime.Now.Month.ToString()) + "-" + ToTwoDigits(DateTime.Now.Day.ToString()) + "-" + ToTwoDigits(DateTime.Now.Year.ToString());
            string line = "";
            using (StreamReader streamReader = new StreamReader("teachers.txt"))
            { 
                line = streamReader.ReadLine();
                while (line != null)
                {
                    string[] properties = line.Split(',');
                    if (properties[1] == date)
                    {
                        teachersShown.Add(properties[0]);
                    }
                    line = streamReader.ReadLine();
                }
            }

            using (StreamReader streamReader = new StreamReader("info.txt"))
            {
                line = streamReader.ReadLine();
            }
			if (line != null && line != "")
			{
				string[] properties = line.Split(',');
				uname = Base64Encryption.Decode(properties[0]);
				pword = Base64Encryption.Decode(properties[1]);
			}
            else
            {
                Application.Run(new LoginForm());
            }
            if (uname == null || pword == null)
                return;
            StartTimer();
            while (true)
            {
            }
        }


        private static void StartTimer()
        {
            timer = new System.Timers.Timer(10000);
            timer.Elapsed += new ElapsedEventHandler(timerElapsed);
            timer.Enabled = true;
        }

        static void timerElapsed(object sender, ElapsedEventArgs e)
        {
            string shows = GetSourceForMyShowsPage(uname,pword);
            if (shows == null)
                return;
            date = ToTwoDigits(DateTime.Now.Month.ToString()) + "-" + ToTwoDigits(DateTime.Now.Day.ToString())  + "-" + ToTwoDigits(DateTime.Now.Year.ToString());
            
            if (shows.Contains(date))
            {
                result = "";
                List<int> updated = AllIndexesOf(shows, date);

                bool showResults = false;
                foreach(int value in updated)
                {
                    string substring = shows.Substring(0, value);
                    int endPosition = substring.LastIndexOf("</a>");
                    int startPosition = substring.LastIndexOf("nsd.org");
                    string teacherName = substring.Substring(startPosition + 9, endPosition - startPosition - 9);
                    if (!teachersShown.Contains(teacherName))
                    {
                        teachersShown.Add(teacherName);
                        using (StreamWriter streamWriter = new StreamWriter("teachers.txt"))
                        {
                            streamWriter.WriteLine(teacherName + "," + date);
                        }


                        // Create a more detailed result with what the teacher updated 
                        result += teacherName + "\n";
                        showResults = true;
                    }
                };

                if (showResults)
                    Application.Run(new ResultsForm(result));
            }
        }

        public static string GetSourceForMyShowsPage(string uname, string pword)
        {
            using (WebClientEx client = new WebClientEx())
            {
                NameValueCollection values = new NameValueCollection
                {
                    { "uname", uname },
                    { "pword", pword },
                };
                try
                {
                    client.UploadValues("https://grades.nsd.org//login.php", values);
                    return client.DownloadString("https://grades.nsd.org/showreportcard.php");
                }
                catch
                {
                    MessageBox.Show("Please connect to the internet.");
                }
            }
            return null;
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
}