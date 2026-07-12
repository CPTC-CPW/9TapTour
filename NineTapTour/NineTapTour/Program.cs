using NineTapTour.Forms;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NineTapTour.Data;
using NineTapTour.Database;
using NineTapTour.State;

namespace NineTapTour
{
    static class Program
    {
        /// <summary>
        /// The composition root: builds the service provider, applies pending migrations, and
        /// launches the main form resolved from dependency injection.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SetUpGlobalExceptionHandling();

            IServiceProvider services = BuildServiceProvider();

            // Bridge legacy static session-state access to the DI singleton until every form is
            // fully converted to injected ITournamentSessionState.
            NineTapTour.Models.FrmMemberScoresHelpers.Session =
                services.GetRequiredService<ITournamentSessionState>();

            // Apply any pending EF Core migrations once, at startup.
            var factory = services.GetRequiredService<IDbContextFactory<NineTapDb>>();
            using (var db = factory.CreateDbContext())
            {
                db.Database.Migrate();
            }

            Application.Run(services.GetRequiredService<FrmMain>());
        }

        /// <summary>
        /// Configures dependency injection: configuration, the data layer (context factory +
        /// repositories), shared session state, and the application forms.
        /// </summary>
        private static IServiceProvider BuildServiceProvider()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var services = new ServiceCollection();

            services.AddNineTapData(configuration.GetConnectionString("NineTapDb"));
            services.AddSingleton<ITournamentSessionState, TournamentSessionState>();

            NineTapTour.Forms.FormRegistration.AddForms(services);

            return services.BuildServiceProvider();
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
