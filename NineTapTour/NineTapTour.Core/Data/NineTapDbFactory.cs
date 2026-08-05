using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NineTapTour.Core.Data;

/// <summary>
/// Design-time factory so dotnet ef commands (migrations add, database update)
/// keep working after OnConfiguring is removed from NineTapDb.
/// </summary>
public class NineTapDbFactory : IDesignTimeDbContextFactory<NineTapDb>
{
    public NineTapDb CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<NineTapDb> optionsBuilder = new();
        optionsBuilder.UseSqlServer(DbConfig.ConnectionString);
        return new NineTapDb(optionsBuilder.Options);
    }
}
