using Microsoft.Extensions.Configuration;
using NineTapTour.Database;
using System;
using System.Windows.Forms;

namespace MemberImportTest
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

            // The appsettings.json is copied from the main NineTapTour project
            // through the project reference; fall back to the default connection
            // string when it is not present.
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            DbConfig.ConnectionString = configuration.GetConnectionString("NineTapDb") ?? DbConfig.DefaultConnectionString;

            Application.Run(new FrmMain());
        }
    }
}
