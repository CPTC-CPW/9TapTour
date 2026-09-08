using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NineTapTour.Core.Services;
using NineTapTour.Core.Startup;
using NineTapTour.Web.Components.Account;
using NineTapTour.Web.Data;
using NineTapTour.Web.Infrastructure;

namespace NineTapTour.Web.Startup;

/// <summary>
/// Registers everything the website needs: Core (repositories and services),
/// Identity on its own DbContext in the same database, authorization policies,
/// Blazor, and the web-only helpers. Mirrors the desktop's
/// ServiceConfiguration.AddNineTapTourServices so it can be validated with a
/// plain ServiceCollection in tests.
/// </summary>
public static class WebServiceConfiguration
{
    public const string AdminRole = "Admin";

    public const string AdminOnlyPolicy = "AdminOnly";

    /// <summary>Separate migrations history so the two contexts sharing one database never collide.</summary>
    public const string IdentityMigrationsHistoryTable = "__IdentityMigrationsHistory";

    public static IServiceCollection AddNineTapTourWeb(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        string connectionString = configuration.GetConnectionString("NineTapDb")
            ?? throw new InvalidOperationException("Connection string 'NineTapDb' is missing from appsettings.json.");

        services.AddNineTapTourCore(connectionString);

        // The desktop's cross-form session object has no meaning on the web; the
        // tournament id travels in the route. Kept resolvable (per circuit) only so
        // ValidateOnBuild never trips; no component may inject it.
        services.Replace(ServiceDescriptor.Scoped<ITournamentSession, TournamentSession>());

        services.AddRazorComponents()
            .AddInteractiveServerComponents(options => options.DetailedErrors = isDevelopment);

        // Excel uploads stream over the circuit in chunks; allow generous messages.
        services.AddSignalR(options => options.MaximumReceiveMessageSize = 1024 * 1024);

        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
        });

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.MigrationsHistoryTable(IdentityMigrationsHistoryTable)));
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddAuthorizationBuilder()
            .AddPolicy(AdminOnlyPolicy, policy => policy.RequireRole(AdminRole));

        services.Configure<IdentitySeedOptions>(configuration.GetSection(IdentitySeedOptions.SectionName));

        services.AddScoped<FlashMessageService>();
        services.AddScoped<ExcelDownload>();

        return services;
    }
}
