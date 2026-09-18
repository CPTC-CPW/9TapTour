using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Repositories;
using NineTapTour.Web.Startup;

namespace NineTapTour.Web.Tests;

/// <summary>
/// Hosts the real website against a throwaway LocalDB catalog for the whole
/// test assembly. Startup applies both sets of migrations (tour + Identity)
/// and seeds the admin, which exercises the AddRegions and InitialIdentity
/// migrations together. A header-driven test authentication scheme lets tests
/// act as an admin, a plain user, or anonymously.
/// </summary>
[TestClass]
public static class WebTestHost
{
    private const string ServerConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=60;Encrypt=False";

    public const string RoleHeader = "X-Test-Role";

    public static string CatalogName { get; private set; } = string.Empty;

    public static WebApplicationFactory<Program> Factory { get; private set; } = default!;

    public static int SeededMemberNumber { get; private set; }

    public static int SeededTournamentId { get; private set; }

    [AssemblyInitialize]
    public static void Start(TestContext context)
    {
        CatalogName = $"NineTapDb_WebTest_{DateTime.Now:yyyyMMddHHmmss}_{Environment.ProcessId}";
        string connectionString = $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={CatalogName};Integrated Security=True;Connect Timeout=60;Encrypt=False";

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:NineTapDb", connectionString);
            builder.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
            builder.ConfigureTestServices(services =>
            {
                // Authenticate from the test header, but leave challenges and
                // forbids to the Identity cookie so redirects match production.
                services.AddAuthentication(options => options.DefaultAuthenticateScheme = TestAuthHandler.Scheme)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });
            });
        });

        // Force the host to start (runs migrations and seeding) and seed test data.
        using (Factory.CreateClient()) { }

        using IServiceScope scope = Factory.Services.CreateScope();
        IMemberRepository members = scope.ServiceProvider.GetRequiredService<IMemberRepository>();
        ITournamentRepository tournaments = scope.ServiceProvider.GetRequiredService<ITournamentRepository>();

        Member member = new() { Number = 501, FirstName = "Web", LastName = "Tester", IsActive = true, Average = 150, Bonus = 2, JoinDate = new DateTime(2020, 1, 1) };
        members.AddOrUpdateMember(member);
        SeededMemberNumber = member.Number;

        Tournament tournament = new() { Date = new DateTime(2026, 9, 5), Location = "Web Test Lanes", Event = "Smoke", Squads = 2 };
        tournaments.AddTournament(tournament);
        SeededTournamentId = tournament.Id;
    }

    [AssemblyCleanup]
    public static void Stop()
    {
        Factory?.Dispose();
        if (string.IsNullOrEmpty(CatalogName))
        {
            return;
        }

        using SqlConnection connection = new(ServerConnectionString);
        connection.Open();
        using SqlCommand command = new(
            $"IF DB_ID('{CatalogName}') IS NOT NULL BEGIN " +
            $"ALTER DATABASE [{CatalogName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
            $"DROP DATABASE [{CatalogName}]; END", connection);
        command.ExecuteNonQuery();
    }

    /// <summary>Client that sends no role header, so requests are anonymous.</summary>
    public static HttpClient AnonymousClient()
    {
        return Factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    public static HttpClient ClientAs(string role)
    {
        HttpClient client = Factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add(RoleHeader, role);
        return client;
    }

    /// <summary>
    /// Authenticates from the X-Test-Role header: "admin" gets the Admin role,
    /// "user" is a plain signed-in user, no header means anonymous.
    /// </summary>
    public sealed class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        public const string Scheme = "Test";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(RoleHeader, out var values))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            string role = values.ToString();
            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, $"test-{role}"),
                new Claim(ClaimTypes.Name, $"test-{role}"),
            ];
            if (role == "admin")
            {
                claims.Add(new Claim(ClaimTypes.Role, WebServiceConfiguration.AdminRole));
            }

            ClaimsIdentity identity = new(claims, Scheme);
            AuthenticationTicket ticket = new(new ClaimsPrincipal(identity), Scheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
