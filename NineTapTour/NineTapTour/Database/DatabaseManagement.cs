using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace NineTapTour.Database
{
    public static class DatabaseManagement
    {
        /// <summary>
        /// Returns the string NineTapDBBackup_ + the current date/time + .bak 
        /// </summary>
        private static string CreateBackupName()
        {
            return "NineTapDBBackup_" + DateTime.Now.ToString("dd-MM-yyyy-hmmss") + ".bak";
        }

        /// <summary>
        /// Tryes to backup the NineTap Database with the same path as the one given by CreateBackupName(), 
        /// returns true if backup was successful
        /// </summary>
        public static bool BackupDatabase()
        {
            // Raw SQL with EF Core
            // https://www.learnentityframeworkcore.com/raw-sql
            const string dbName = "NineTapTour.NineTapDb";
            using (NineTapDb context = new())
            {
                using var backUpCmd = context.Database.GetDbConnection().CreateCommand();
                context.Database.ExecuteSqlRaw($"BACKUP DATABASE {dbName} TO DISK = {backUpCmd + "\\" + CreateBackupName()}");
            }
            return true;
        }

        /// <summary>
        /// Tryes to restore the NineTap Database with the same path given, 
        /// returns true if restore was successful
        /// </summary>
        public static bool RestoreDatabase()
        {
            using (NineTapDb context = new())
            {
                using var backUpCmd = context.Database.GetDbConnection().CreateCommand();
                context.Database.ExecuteSqlRaw("USE master;"
                    + "ALTER DATABASE [NineTapTour.NineTapDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;"
                    + "RESTORE DATABASE @dbName FROM DISK = @restorePath WITH REPLACE");
            }
            return true;
        }
    }
}
