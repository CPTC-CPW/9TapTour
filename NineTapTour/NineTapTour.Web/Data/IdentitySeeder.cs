using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NineTapTour.Web.Infrastructure;
using NineTapTour.Web.Startup;

namespace NineTapTour.Web.Data;

/// <summary>
/// Ensures the Admin role exists and, when the user table is empty, creates the
/// first admin from configuration. A missing password is logged, not fatal, so
/// a misconfigured deployment still starts and shows a hint on the login page.
/// </summary>
public sealed class IdentitySeeder(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentitySeedOptions> options,
    ILogger<IdentitySeeder> logger)
{
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

        IdentitySeedOptions seed = options.Value;
        if (string.IsNullOrWhiteSpace(seed.Password))
        {
            logger.LogError("No users exist and Identity:Admin:Password is not configured; nobody can log in until it is set.");
            return;
        }

        ApplicationUser admin = new() { UserName = seed.UserName, Email = null, EmailConfirmed = true };
        IdentityResult created = await userManager.CreateAsync(admin, seed.Password);
        if (!created.Succeeded)
        {
            logger.LogError("Could not create the seed admin user: {Errors}", string.Join("; ", created.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(admin, WebServiceConfiguration.AdminRole);
        logger.LogInformation("Seeded admin user '{UserName}'.", seed.UserName);
    }
}
