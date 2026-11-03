using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NineTapTour.Database;
using NineTapTour.Core.Data;
using NineTapTour.Core.Startup;
using NineTapTour.Deployment;
using System;
using System.Windows.Forms;
using Velopack;

namespace MemberImportTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Must run first. The installer and updater start the app with special arguments to finish their work
            // (shortcuts, uninstalling), and Run() handles those and exits. It also installs an update downloaded last time.
            VelopackApp.Build().Run();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (ClickOnceMigration.MoveToVelopack(DeployedApp.MemberImport))
            {
                return;
            }
            ClickOnceMigration.RemoveClickOnceCopy(DeployedApp.MemberImport, args);

            // The appsettings.json is copied from the main NineTapTour project
            // through the project reference; fall back to the default connection
            // string when it is not present.
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            string connectionString = configuration.GetConnectionString("NineTapDb") ?? NineTapDbFactory.DesignTimeConnectionString;

            if (!LocalDbCheck.EnsureInstalled(connectionString, DeployedApp.MemberImport.ProductName))
            {
                return;
            }

            ServiceCollection services = new();
            services.AddNineTapTourCore(connectionString);
            services.AddSingleton<AppUpdater>();
            services.AddTransient<FrmMain>();

            using ServiceProvider provider = services.BuildServiceProvider();

            Application.Run(provider.GetRequiredService<FrmMain>());
        }
    }
}
