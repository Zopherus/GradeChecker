using IWshRuntimeLibrary;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;
using System.Threading;

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
            PhantomJSDriverService driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            WebDriver = new PhantomJSDriver(driverService);



            date = ToTwoDigits(DateTime.Now.Month.ToString()) + "-" + ToTwoDigits(DateTime.Now.Day.ToString()) + "-" + ToTwoDigits(DateTime.Now.Year.ToString());
            string line = "";
            using (StreamReader streamReader = new StreamReader("Text Files/teachers.txt"))
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

            using (StreamReader streamReader = new StreamReader("Text Files/info.txt"))
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
            date = ToTwoDigits(DateTime.Now.Month.ToString()) + "/" + ToTwoDigits(DateTime.Now.Day.ToString())  + "/" + ToTwoDigits(DateTime.Now.Year.ToString());
            WebDriver.Navigate().GoToUrl(WEBSITE_URL);
            //try
            //{
            WebDriver.FindElement(By.Name("username")).SendKeys(username);
            WebDriver.FindElement(By.Name("password")).SendKeys(password);
            WebDriver.FindElement(By.Name("Submit1")).Click();
            WebDriver.Navigate().GoToUrl(GRADEBOOK_URL);


            IEnumerable<IWebElement> OddClasses = WebDriver.FindElements(By.ClassName("altrow1"));
            IEnumerable<IWebElement> EvenClasses = WebDriver.FindElements(By.ClassName("altrow2"));
            IEnumerable<IWebElement> Classes = OddClasses
                                                .SelectMany((x, idx) => new[] { x, EvenClasses.ElementAt(idx) })
                                                .Concat(EvenClasses.Skip(OddClasses.Count()));
            if (!Directory.EnumerateFileSystemEntries("Text Files/Classes").Any())
            {
                for (int i = 0; i < Classes.Count(); i++)
                {
                    System.IO.File.CreateText("Text Files/Classes/" + i.ToString() + ".txt");
                }
            }
            List<string> ClassLinks = new List<string>();
            foreach (IWebElement element in Classes)
            {
                ClassLinks.Add(element.FindElement(By.XPath("./td/a[1]")).GetAttribute("href"));
            }
            
            for (int i = 0; i < ClassLinks.Count; i++)
            {
                WebDriver.Navigate().GoToUrl(ClassLinks[i]);

                IEnumerable<IWebElement> OddDates = WebDriver.FindElements(By.ClassName("altrow1"));
                IEnumerable<IWebElement> EvenDates = WebDriver.FindElements(By.ClassName("altrow2"));
                IEnumerable<IWebElement> DateLinks = OddDates.Concat(EvenDates);
                List<string> Dates = new List<string>();
                foreach (IWebElement element in DateLinks)
                {
                    Dates.Add(element.FindElement(By.XPath("./td")).Text);
                }

                // Find all assignments that were uploaded today by teachers
                // Grades are only updated if the points are also included in the grade
                // If an assignment was uploaded but no point values were given, save it and continually check that assignment for points
                if (Dates.Any(s => s == date))
                {

                }
                WebDriver.TakeScreenshot().SaveAsFile("screenshot" + i.ToString() + ".png", ImageFormat.Png);
            }
            WebDriver.TakeScreenshot().SaveAsFile("screenshot.png", ImageFormat.Png);
            connectedToInternet = true;
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

        public static string GetSourceForMyShowsPage(string username, string password)
        {
            WebDriver.Navigate().GoToUrl(WEBSITE_URL);
            //try
            //{
            WebDriver.FindElement(By.Name("username")).SendKeys(username);
            WebDriver.FindElement(By.Name("password")).SendKeys(password);
            WebDriver.FindElement(By.Name("Submit1")).Click();
            WebDriver.Navigate().GoToUrl(GRADEBOOK_URL);


            IEnumerable<IWebElement> OddClasses = WebDriver.FindElements(By.ClassName("altrow1"));
            IEnumerable<IWebElement> EvenClasses = WebDriver.FindElements(By.ClassName("altrow2"));
            IEnumerable<IWebElement> Classes = OddClasses.Concat(EvenClasses);
            if (!Directory.EnumerateFileSystemEntries("Text Files/Classes").Any())
            {
                for (int i = 0; i < Classes.Count(); i++)
                {
                    System.IO.File.CreateText("Text Files/Classes/" + i.ToString() + ".txt");
                }
            }
            List<IWebElement> ClassLinks = new List<IWebElement>();
            foreach (IWebElement element in Classes)
            {
                ClassLinks.Add(element.FindElement(By.XPath("./td/a[1]")));
            }
            ClassLinks[0].Click();
            IEnumerable<IWebElement> OddDates = WebDriver.FindElements(By.ClassName("altrow1"));
            IEnumerable<IWebElement> EvenDates = WebDriver.FindElements(By.ClassName("altrow2"));
            IEnumerable<IWebElement> DateLinks = OddDates.Concat(EvenDates);
            

            List<string> Dates = new List<string>(); 
            foreach (IWebElement element in DateLinks)
            {
                Console.WriteLine(element.Text);
                Dates.Add(element.FindElement(By.XPath("./td")).Text);
            }

            // Find all assignments that were uploaded today by teachers
            // Grades are only updated if the points are also included in the grade
            // If an assignment was uploaded but no point values were given, save it and continually check that assignment for points


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