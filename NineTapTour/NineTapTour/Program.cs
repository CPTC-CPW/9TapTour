using NineTapTour.Forms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
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

#if DEBUG //if app is set to release mode
            SetConnectionString(@".\SQLExpress");
#elif !DEBUG //set connection to dev database
            SetConnectionString(@"(localdb)\MSSQLLocalDB");
#endif

            Application.Run(new FrmMain());
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
