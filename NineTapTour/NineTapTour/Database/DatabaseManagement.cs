using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public static bool BackupDatabase(string backupPath)
        {
            NineTapDB db = new NineTapDB();
            string query = "BACKUP DATABASE @dbName TO DISK = @backupPath";

            try
            {
                db.Database.ExecuteSqlCommand(
                        TransactionalBehavior.DoNotEnsureTransaction,
                        query,
                        new SqlParameter("@dbName", db.Database.Connection.Database),
                        new SqlParameter("@backupPath", backupPath + "\\" + CreateBackupName())
                        );
            }
            catch (SqlException sqle)
            {
                if (sqle.Message.Contains("Operating system error 5(Access is denied.)"))
                {
                    MessageBox.Show("SQL Server does not have permission to access the folder selected.");
                    return false;
                }

                throw;
            }

            return true;
        }

        /// <summary>
        /// Tryes to restore the NineTap Database with the same path given, 
        /// returns true if restore was successful
        /// </summary>
        public static bool RestoreDatabase(string restorePath)
        {
            NineTapDB db = new NineTapDB();
            string query = "USE master " +
                           "ALTER DATABASE [NineTapTour.NineTapDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE " +
                           "RESTORE DATABASE @dbName FROM DISK = @restorePath WITH REPLACE";
            db.Database.ExecuteSqlCommand(
                TransactionalBehavior.DoNotEnsureTransaction,
                query,
                new SqlParameter("@dbName", db.Database.Connection.Database),
                new SqlParameter("@restorePath", restorePath)
                );
            return true;
        }
    }
}
