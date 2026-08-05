namespace NineTapTour.Core.Services;

/// <summary>
/// Database backup and restore. Callers pick the file paths (via dialogs in
/// the desktop app); this service only talks to SQL Server.
/// </summary>
public interface IDatabaseMaintenanceService
{
    /// <summary>Suggested file name for a new backup, stamped with the current time.</summary>
    string CreateBackupName();

    void BackupTo(string backupFilePath);

    void RestoreFrom(string backupFilePath);
}
