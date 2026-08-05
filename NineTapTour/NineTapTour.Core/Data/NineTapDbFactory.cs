using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NineTapTour.Core.Data;

/// <summary>
/// Design-time factory so dotnet ef commands (migrations add, database update)
/// keep working after OnConfiguring is removed from NineTapDb.
/// </summary>
public class NineTapDbFactory : IDesignTimeDbContextFactory<NineTapDb>
{
    public const string DesignTimeConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NineTapDb2025;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

    public NineTapDb CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<NineTapDb> optionsBuilder = new();
        optionsBuilder.UseSqlServer(DesignTimeConnectionString);
        return new NineTapDb(optionsBuilder.Options);
    }
}
