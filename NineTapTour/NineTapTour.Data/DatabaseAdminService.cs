using System;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Abstractions;
using NineTapTour.Database;

namespace NineTapTour.Data
{
    /// <summary>EF Core / SQL Server implementation of <see cref="IDatabaseAdminService"/> (formerly <c>DatabaseManagement</c>).</summary>
    public sealed class DatabaseAdminService : IDatabaseAdminService
    {
        private const string DbName = "NineTapDb2025";

        private readonly IDbContextFactory<NineTapDb> _factory;

        public DatabaseAdminService(IDbContextFactory<NineTapDb> factory) => _factory = factory;

        public string CreateBackupName()
        {
            return "NineTapDb2025_" + DateTime.Now.ToString("dd-MM-yyyy-hmmss") + ".bak";
        }

        public void BackupDatabase(string backupFilePath)
        {
            using var context = _factory.CreateDbContext();
            // NOTE: behavior preserved verbatim from DatabaseManagement (identifier interpolation quirk left as-is).
            context.Database.ExecuteSqlInterpolated($"USE master; BACKUP DATABASE {DbName} TO DISK = {backupFilePath}");
        }

        public void RestoreDatabase(string backupFilePath)
        {
            using var context = _factory.CreateDbContext();
            context.Database.ExecuteSqlRaw($"USE master;DROP DATABASE [NineTapdb2025];"
                + $"RESTORE DATABASE [NineTapDb2025] FILE = 'NineTapDb2025' FROM DISK = '{backupFilePath}' WITH FILE = 1,"
                + $"MOVE 'NineTapDb2025' TO '{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\NineTapDb2025.mdf'," +
                $"MOVE 'NineTapDb2025_log' TO '{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\NineTap2025.ldf', NOUNLOAD");
        }

        public void DropAndRecreate()
        {
            using var context = _factory.CreateDbContext();
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }
    }
}
