using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NineTapTour.Database;
using NineTapTour.Core.Data;
using NineTapTour.Forms;
using NineTapTour.Startup;
using System;
using System.IO;
using System.Threading;
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

            SetUpGlobalExceptionHandling();

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            ServiceCollection services = new();
            services.AddNineTapTourServices(configuration);

            using ServiceProvider provider = services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true,
            });

            MigrateDatabase(provider);

            Application.Run(provider.GetRequiredService<FrmMain>());
        }

        /// <summary>
        /// Applies any pending EF Core migrations before any form opens so a
        /// schema problem fails fast instead of surfacing mid workflow.
        /// </summary>
        private static void MigrateDatabase(IServiceProvider provider)
        {
            var dbFactory = provider.GetRequiredService<IDbContextFactory<NineTapDb>>();
            using NineTapDb db = dbFactory.CreateDbContext();
            db.Database.Migrate();
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

        private static void RecordExceptionToFile(Exception errorData)
        {
            string exceptionData = GetExceptionData(errorData);

            const string errorFolder = "9-Tap Errors";
            string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string errorLogPath = Path.Combine(desktopFolder, errorFolder);

            Directory.CreateDirectory(errorLogPath);

            //Create unique filename
            string fileTitle = $"Error-{DateTime.Today:MMMMM dd yyyy}-{Guid.NewGuid().ToString()}.txt";
            string errorFilePath = Path.Combine(errorLogPath, fileTitle);
            try
            {
                File.WriteAllText(errorFilePath, exceptionData);
                string errorMsg = "An error was encountered and the program crashed. A file has been created in" +
                    $" {errorLogPath}. Please email the files to the developer team. You may delete them after you" +
                    $" send your email.";
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                string msg = $"An error was encountered but the program failed to create a text file to save error information. Error: {ex.Message}";
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GetExceptionData(Exception errorData)
        {
            string newLine = Environment.NewLine;
            string exceptionData = $"{errorData.GetType().ToString()}{newLine}" +
                $"{errorData.Message}{newLine}{newLine}" +
                $"Stacktrace: {newLine}{errorData.StackTrace}{newLine}{newLine}{newLine}" +
                $"Full Information:{errorData.ToString()}";
            return exceptionData;
        }
    }
}
