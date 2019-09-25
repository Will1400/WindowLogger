using System;
using WindowLogger.DataAccess;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WindowLogger.Logger
{
    class Program
    {
        public static Repository repository;
        public static List<string> windowIgnoreNames = new List<string>() { "Reload", "Selected Tab" };

        static async Task Main(string[] args)
        {
            IntPtr handle = GetConsoleWindow();
#if DEBUG // Show console window only in debug mode
            ShowWindow(handle, SW_SHOW);
#else
            ShowWindow(handle, SW_HIDE);
#endif
            repository = new Repository();
            await Listen();
        }


        static async Task Listen()
        {
            string lastWindow = "";
            DateTime detectedDate = default;

            while (true)
            {
                await Task.Delay(200);
                string window = GetActiveWindowTitle();
                if (window != null &&  lastWindow != window)
                {
                    if (CheckIfWindowIsIgnored(window) || string.IsNullOrEmpty(window.Split(',').Last()))
                        continue;

                    LogWindow(lastWindow, detectedDate, DateTime.Now);
                    lastWindow = window;
                    detectedDate = DateTime.Now;

#if DEBUG // Logs to console if in Debug mode
                    Console.WriteLine(lastWindow);
#endif
                }
            }
        }


        static async void LogWindow(string windowTitle, DateTime gainedFocus, DateTime lostFocus)
        {
            await repository.LogAsync($"{gainedFocus},{lostFocus},{windowTitle}");
        }

        /// <summary>
        /// Checks a window title to see if it's ignored
        /// </summary>
        /// <param name="windowTitle">Window title to be checked</param>
        /// <returns>True if window is ignored, false if not</returns>
        public static bool CheckIfWindowIsIgnored(string windowTitle)
        {
            if (windowIgnoreNames.Contains(windowTitle))
                return true;

            return false;
        }

        #region Console Hiding

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        #endregion

        #region window listening

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
                return Buff.ToString();

            return null;
        }
        #endregion
    }
}