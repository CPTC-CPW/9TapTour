namespace NineTapTour.Database;

/// <summary>
/// Interim holder for the connection string used by NineTapDb.OnConfiguring.
/// Set once at startup from appsettings.json. This class is deleted once every
/// new NineTapDb() call site is replaced with an injected IDbContextFactory.
/// </summary>
public static class DbConfig
{
    public const string DefaultConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NineTapDb2025;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

    public static string ConnectionString { get; set; } = DefaultConnectionString;
}
