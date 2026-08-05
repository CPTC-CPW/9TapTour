using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NineTapTour.Database;
using NineTapTour.Core.Data;
using NineTapTour.Core.Startup;
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

            string connectionString = configuration.GetConnectionString("NineTapDb") ?? NineTapDbFactory.DesignTimeConnectionString;

            ServiceCollection services = new();
            services.AddNineTapTourCore(connectionString);
            services.AddTransient<FrmMain>();

            using ServiceProvider provider = services.BuildServiceProvider();

            Application.Run(provider.GetRequiredService<FrmMain>());
        }
    }
}
