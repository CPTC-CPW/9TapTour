using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Data;
using NineTapTour.Web.Data;

namespace NineTapTour.Web.Startup;

/// <summary>
/// Startup database work: migrate the tour database (shared with the desktop
/// app), migrate the Identity tables, and create the first admin user.
/// </summary>
public static class DatabaseStartup
{
    public static async Task MigrateAndSeedAsync(IServiceProvider services, ILogger logger)
    {
        using IServiceScope scope = services.CreateScope();

        IDbContextFactory<NineTapDb> tourFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<NineTapDb>>();
        await using (NineTapDb tourDb = await tourFactory.CreateDbContextAsync())
        {
            await tourDb.Database.MigrateAsync();
        }

        ApplicationDbContext identityDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await identityDb.Database.MigrateAsync();

        IdentitySeeder seeder = ActivatorUtilities.CreateInstance<IdentitySeeder>(scope.ServiceProvider);
        await seeder.SeedAsync();
        logger.LogInformation("Database migrations applied and Identity seeded.");
    }
}
