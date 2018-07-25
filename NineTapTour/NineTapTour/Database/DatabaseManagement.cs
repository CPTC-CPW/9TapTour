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

        private static string CreateBackupName()
        {
            return "NineTapDBBackup_" + DateTime.Now.ToString("d-MMM-yyyy-hmmss") + ".bak";
        }

        public static bool BackupDatabase(string backupPath)
        {
            NineTapDb db = new NineTapDb();
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

        public static bool RestoreDatabase(string restorePath)
        {
            NineTapDb db = new NineTapDb();
            string query = "USE master " +
                           "ALTER DATABASE [NineTapTour.NineTapDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE " +
                           "RESTORE DATABASE @dbName FROM DISK = @restorePath WITH REPLACE";

            try
            {
                db.Database.ExecuteSqlCommand(
                    TransactionalBehavior.DoNotEnsureTransaction,
                    query,
                    new SqlParameter("@dbName", db.Database.Connection.Database),
                    new SqlParameter("@restorePath", restorePath)
                    );
            }
            catch (Exception)
            {

                throw;
            }

            return true;
        }
    }
}
