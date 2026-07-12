using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NineTapTour.Database;

namespace NineTapTour.Data
{
    /// <summary>
    /// Supplies a configured <see cref="NineTapDb"/> to the <c>dotnet ef</c> design-time tooling
    /// (Add-Migration / Update-Database) now that the context no longer self-configures via
    /// <c>OnConfiguring</c>. Runtime configuration comes from DI (see <see cref="ServiceCollectionExtensions"/>).
    /// </summary>
    public sealed class NineTapDbDesignTimeFactory : IDesignTimeDbContextFactory<NineTapDb>
    {
        public NineTapDb CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<NineTapDb>()
                .UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NineTapDb2025;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False")
                .Options;

            return new NineTapDb(options);
        }
    }
}
