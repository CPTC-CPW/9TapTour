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
            return "NineTapDb2025_" + DateTime.Now.ToString("dd-MM-yyyy-hmmss") + ".bak";
        }

        /// <summary>
        /// Backs up the current database with DateTime attached to a backup name
        /// </summary>
        public static void BackupDatabase()
        {
            // Raw SQL with EF Core
            // https://www.learnentityframeworkcore.com/raw-sql
            const string dbName = "NineTapDb2025";
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
        /// <returns>Returns true if database is restored and app must be restarted</returns>
        public static bool RestoreDatabase()
        {
            using NineTapDb context = new();

            OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Backup file |*.bak",
                DefaultExt = ".bak"
            };

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                using var backUpCmd = context.Database.GetDbConnection().CreateCommand();
                context.Database.ExecuteSqlRaw($"USE master;DROP DATABASE [NineTapdb2025];"
                    + $"RESTORE DATABASE [NineTapDb2025] FILE = 'NineTapDb2025' FROM DISK = '{openFileDialog.FileName}' WITH FILE = 1,"
                    + $"MOVE 'NineTapDb2025' TO '{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\NineTapDb2025.mdf'," +
                    $"MOVE 'NineTapDb2025_log' TO '{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\NineTap2025.ldf', NOUNLOAD");
                MessageBox.Show("Restore successful");
                return true;
            }

            return false;
        }
    }
}
