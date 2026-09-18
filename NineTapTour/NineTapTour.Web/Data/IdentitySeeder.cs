using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Web.Startup;

namespace NineTapTour.Web.Data;

/// <summary>
/// Ensures the Admin role exists and, when no users exist at all, creates a
/// first admin. The default account is only created by Debug builds so a fresh
/// local database is usable without any setup; Release builds log an error and
/// the login page shows a hint instead.
/// </summary>
public sealed class IdentitySeeder(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<IdentitySeeder> logger)
{
    public const string DefaultAdminUserName = "admin";

    public const string DefaultAdminPassword = "Admin12345!";

    public async Task SeedAsync()
    {
        if (!await roleManager.RoleExistsAsync(WebServiceConfiguration.AdminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(WebServiceConfiguration.AdminRole));
        }

        if (await userManager.Users.AnyAsync())
        {
            return;
        }

#if DEBUG
        ApplicationUser admin = new() { UserName = DefaultAdminUserName, Email = null, EmailConfirmed = true };
        IdentityResult created = await userManager.CreateAsync(admin, DefaultAdminPassword);
        if (!created.Succeeded)
        {
            logger.LogError("Could not create the default admin user: {Errors}", string.Join("; ", created.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(admin, WebServiceConfiguration.AdminRole);
        logger.LogInformation("No users existed; created default admin user '{UserName}' (Debug build only).", DefaultAdminUserName);
#else
        logger.LogError("No users exist; nobody can log in until an admin account is created in the database.");
#endif
    }
}
