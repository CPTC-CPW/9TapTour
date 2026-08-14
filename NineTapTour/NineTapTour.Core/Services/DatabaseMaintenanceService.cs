#nullable disable
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Data;
using System;
using System.IO;

namespace NineTapTour.Core.Services;

public class DatabaseMaintenanceService : IDatabaseMaintenanceService
{
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public DatabaseMaintenanceService(IDbContextFactory<NineTapDb> dbFactory)
    {
        this.dbFactory = dbFactory;
    }

    /// <summary>
    /// Returns the string {database}_ + the current date/time + .bak
    /// </summary>
    public string CreateBackupName()
    {
        using NineTapDb context = dbFactory.CreateDbContext();
        string databaseName = context.Database.GetDbConnection().Database;
        return databaseName + "_" + DateTime.Now.ToString("dd-MM-yyyy-hmmss") + ".bak";
    }

    /// <summary>
    /// Backs up the configured database to the given .bak file. The file path
    /// is passed as a SQL parameter; the database name comes from the
    /// connection string (never from user input) and is bracket-escaped.
    /// </summary>
    public void BackupTo(string backupFilePath)
    {
        using NineTapDb context = dbFactory.CreateDbContext();
        string escapedName = EscapedIdentifier(context.Database.GetDbConnection().Database);
        context.Database.ExecuteSqlRaw(
            "USE master; BACKUP DATABASE " + escapedName + " TO DISK = {0}", backupFilePath);
    }

    /// <summary>
    /// Drops the configured database and restores it from the given backup.
    /// Data files are placed in the user profile folder, matching the
    /// long-standing behavior of the old DatabaseManagement class.
    /// </summary>
    public void RestoreFrom(string backupFilePath)
    {
        using NineTapDb context = dbFactory.CreateDbContext();
        string databaseName = context.Database.GetDbConnection().Database;
        string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string dataFilePath = Path.Combine(userProfile, databaseName + ".mdf");
        string logFilePath = Path.Combine(userProfile, databaseName + "_log.ldf");
        string escapedName = EscapedIdentifier(databaseName);

        // Drop and restore must run as one batch on one connection: after the
        // drop, a fresh connection could no longer open against this catalog.
        context.Database.ExecuteSqlRaw(
            "USE master; ALTER DATABASE " + escapedName + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE " + escapedName + ";" +
            "RESTORE DATABASE " + escapedName + " FROM DISK = {0} WITH FILE = 1, MOVE {1} TO {2}, MOVE {3} TO {4}, NOUNLOAD",
            backupFilePath, databaseName, dataFilePath, databaseName + "_log", logFilePath);
    }

    private static string EscapedIdentifier(string databaseName)
    {
        return "[" + databaseName.Replace("]", "]]") + "]";
    }
}
