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
            return "NineTapDb2021_" + DateTime.Now.ToString("dd-MM-yyyy-hmmss") + ".bak";
        }

        /// <summary>
        /// Backs up the current database with DateTime attached to a backup name
        /// </summary>
        public static void BackupDatabase()
        {
            // Raw SQL with EF Core
            // https://www.learnentityframeworkcore.com/raw-sql
            const string dbName = "NineTapDb2021";
            using NineTapDb context = new();

            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Backup file |*.bak",
                DefaultExt = ".bak",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = CreateBackupName()
            };
            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string backupName = saveFileDialog.FileName;
                context.Database.ExecuteSqlInterpolated($"USE master; BACKUP DATABASE {dbName} TO DISK = {backupName}");
                MessageBox.Show("Backup successful");
            }

            
        }

        /// <summary>
        /// Restores NineTap database from backup
        /// </summary>
        public static bool RestoreDatabase()
        {
            using (NineTapDb context = new())
            {
                using var backUpCmd = context.Database.GetDbConnection().CreateCommand();
                context.Database.ExecuteSqlRaw("USE master;"
                    + "ALTER DATABASE [NineTapDb2021] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;"
                    + "RESTORE DATABASE @dbName FROM DISK = @restorePath WITH REPLACE");
            }
            return true;
        }
    }
}
