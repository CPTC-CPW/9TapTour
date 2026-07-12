namespace NineTapTour.Abstractions
{
    /// <summary>
    /// Administrative database operations (backup / restore / drop-and-recreate). SQL Server only.
    /// UI concerns (file dialogs, message boxes) stay in the forms; callers pass file paths in.
    /// Replaces the static <c>DatabaseManagement</c>.
    /// </summary>
    public interface IDatabaseAdminService
    {
        /// <summary>Suggested backup file name, e.g. <c>NineTapDb2025_dd-MM-yyyy-hmmss.bak</c>.</summary>
        string CreateBackupName();

        /// <summary>Backs up the database to the given .bak path.</summary>
        void BackupDatabase(string backupFilePath);

        /// <summary>Restores the database from the given .bak path. Caller should restart the app afterward.</summary>
        void RestoreDatabase(string backupFilePath);

        /// <summary>Deletes and recreates the database from migrations (all data is lost).</summary>
        void DropAndRecreate();
    }
}
