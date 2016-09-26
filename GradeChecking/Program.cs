using System;
using System.Timers;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.Drawing.Imaging;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using IWshRuntimeLibrary;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.Extensions;

namespace GradeChecking
{
    static class Program
    {
        private const string WEBSITE_URL = "https://siswebpv.nsd.org/Login_Student_PXP.aspx";
        private const string GRADEBOOK_URL = "https://siswebpv.nsd.org/PXP_Gradebook.aspx?AGU=0";
        private static System.Timers.Timer timer;
        public static string username;
        public static string password;
        private static string result;
        private static string date;
        private static PhantomJSDriver WebDriver;
        private static bool connectedToInternet = true;
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


            if (!System.IO.File.Exists(path + @"\GradeChecker.lnk"))
            {
                string shortcutLocation = System.IO.Path.Combine(path, "Gradechecker.lnk");
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                shortcut.Description = "Northshore School District Grade Checking Updater";   // The description of the shortcut
                shortcut.TargetPath = Assembly.GetExecutingAssembly().Location;                 // The path of the file that will launch when the shortcut is run
                shortcut.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                shortcut.Save();
            }

            WebDriver = new PhantomJSDriver();



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
				username = Base64Encryption.Decode(properties[0]);
				password = Base64Encryption.Decode(properties[1]);
			}
            else
            {
                Application.Run(new LoginForm());
            }
            if (username == null || password == null)
                Application.Run(new LoginForm());
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
            string shows = GetSourceForMyShowsPage(username,password);
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
                            streamWriter.Close();
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

        public static string GetSourceForMyShowsPage(string username, string password)
        {
            WebDriver.Navigate().GoToUrl(WEBSITE_URL);
            //try
            //{
                WebDriver.FindElement(By.Name("username")).SendKeys(username);
                WebDriver.FindElement(By.Name("password")).SendKeys(password);
                WebDriver.FindElement(By.Name("Submit1")).Click();
                WebDriver.Navigate().GoToUrl(GRADEBOOK_URL);
                WebDriver.TakeScreenshot().SaveAsFile("screenshot.png", ImageFormat.Png);
                connectedToInternet = true;
                return WebDriver.PageSource;
            /*}
            catch
            {
                if (connectedToInternet)
                {
                    MessageBox.Show(@"Gradechecker cannot run as your device is not currently connected to the internet.
                                    When internet connection is gained, Gradechecker will start running again.");
                    connectedToInternet = false;
                }
            }
            return null;*/
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