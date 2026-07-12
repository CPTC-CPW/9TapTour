using System;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NineTapTour.Data;

namespace MemberImportTest
{
    static class Program
    {
        /// <summary>
        /// Composition root for the member-import tool. Builds the service provider (shared data layer)
        /// and launches the main form resolved from DI.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var services = new ServiceCollection();
            services.AddNineTapData(configuration.GetConnectionString("NineTapDb"));
            services.AddTransient<FrmMain>();

            var provider = services.BuildServiceProvider();

            Application.Run(provider.GetRequiredService<FrmMain>());
        }
    }
}
