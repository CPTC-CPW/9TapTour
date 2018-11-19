using NineTapTour.Forms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineTapTour
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if !DEBUG //if app is set to release mode
            SetConnectionString(@".\SQLExpress");
#elif DEBUG //set connection to dev database
            SetConnectionString(@"(localdb)\MSSQLLocalDB");
#endif

            SetUpGlobalExceptionHandling();

            Application.Run(new FrmMain());
        }

        private static void SetUpGlobalExceptionHandling()
        {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(LogThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(LogUnhandledException);
        }

        private static void LogThreadException(object sender, ThreadExceptionEventArgs e)
        {
            RecordExceptionToFile(e.Exception);
        }

        private static void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            RecordExceptionToFile(e.ExceptionObject as Exception);
        }

        private static void RecordExceptionToFile(Exception e)
        {
            string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string errorLogPath = System.IO.Path.Combine(desktopFolder, "9-TapErrors");
            string fileTitle = $"Error {Guid.NewGuid()}";
            File.WriteAllText(errorLogPath, e.ToString());
            ShowExceptionDialog("Error", "exception was thrown", e);
        }

        private static DialogResult ShowExceptionDialog(string title, string message, Exception e)
        {
            return MessageBox.Show("Program terminated because of " + e.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void SetConnectionString(string dataSource)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings["NineTapDbConnection"].ConnectionString = String.Format("data source={0};initial catalog=NineTapTour.NineTapDb;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework", dataSource);
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("connectionStrings");
        }
    }
}
