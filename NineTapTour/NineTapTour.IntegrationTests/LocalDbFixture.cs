using System;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Database;

namespace NineTapTour.IntegrationTests
{
    /// <summary>Minimal <see cref="IDbContextFactory{TContext}"/> bound to a fixed set of options.</summary>
    internal sealed class TestDbContextFactory : IDbContextFactory<NineTapDb>
    {
        private readonly DbContextOptions<NineTapDb> _options;
        public TestDbContextFactory(DbContextOptions<NineTapDb> options) => _options = options;
        public NineTapDb CreateDbContext() => new NineTapDb(_options);
    }

    /// <summary>
    /// Creates a uniquely-named SQL Server LocalDB database, applies EF Core migrations, and drops it on
    /// dispose. Exposes an <see cref="IDbContextFactory{TContext}"/> bound to that database so the real
    /// production repositories can run against a live, seeded schema.
    /// </summary>
    internal sealed class LocalDbFixture : IDisposable
    {
        private const string LocalDbInstance = "(localdb)\\MSSQLLocalDB";

        public IDbContextFactory<NineTapDb> Factory { get; }

        public LocalDbFixture()
        {
            string dbName = "NineTapTest_" + Guid.NewGuid().ToString("N");
            string connectionString =
                $"Data Source={LocalDbInstance};Initial Catalog={dbName};Integrated Security=True;" +
                "Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";

            var options = new DbContextOptionsBuilder<NineTapDb>()
                .UseSqlServer(connectionString)
                .Options;

            Factory = new TestDbContextFactory(options);

            using var db = Factory.CreateDbContext();
            db.Database.Migrate();
        }

        public void Dispose()
        {
            using var db = Factory.CreateDbContext();
            db.Database.EnsureDeleted();
        }

        /// <summary>
        /// True when SQL Server LocalDB is reachable. Integration tests call this and mark themselves
        /// inconclusive (rather than failing) when LocalDB is not installed — e.g. on CI runners.
        /// </summary>
        public static bool IsLocalDbAvailable()
        {
            try
            {
                var options = new DbContextOptionsBuilder<NineTapDb>()
                    .UseSqlServer($"Data Source={LocalDbInstance};Initial Catalog=master;Integrated Security=True;" +
                                  "Connect Timeout=5;Encrypt=False;Trust Server Certificate=False")
                    .Options;
                using var db = new NineTapDb(options);
                return db.Database.CanConnect();
            }
            catch
            {
                return false;
            }
        }
    }
}
