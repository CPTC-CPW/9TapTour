using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Data;

namespace NineTapTour.Web.Tests;

/// <summary>
/// HTTP-level checks through the real host: authentication gates, the SSR
/// print pages, and the prerendered HTML of every interactive page.
/// </summary>
[TestClass]
public class PageSmokeTests
{
    [TestMethod]
    public void Startup_MigratedBothContextsAndSeededAdmin()
    {
        using IServiceScope scope = WebTestHost.Factory.Services.CreateScope();
        using NineTapDb tourDb = scope.ServiceProvider.GetRequiredService<IDbContextFactory<NineTapDb>>().CreateDbContext();
        Assert.AreEqual(1, tourDb.Regions.Count(), "AddRegions seeds exactly one region");
        Assert.AreEqual("Default", tourDb.Regions.Single().Name);

        var identityDb = scope.ServiceProvider.GetRequiredService<NineTapTour.Web.Data.ApplicationDbContext>();
        Assert.AreEqual(1, identityDb.Users.Count(), "the seed admin was created");
        Assert.IsTrue(identityDb.Roles.Any(r => r.Name == "Admin"));
    }

    [TestMethod]
    public async Task AnonymousRequest_IsRedirectedToLogin()
    {
        using HttpClient client = WebTestHost.AnonymousClient();

        HttpResponseMessage response = await client.GetAsync("/members");

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        StringAssert.Contains(response.Headers.Location!.ToString(), "Account/Login");
    }

    [TestMethod]
    public async Task LoginPage_IsAnonymous()
    {
        using HttpClient client = WebTestHost.AnonymousClient();

        HttpResponseMessage response = await client.GetAsync("/Account/Login");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        string html = await response.Content.ReadAsStringAsync();
        StringAssert.Contains(html, "Log in");
    }

    [TestMethod]
    public async Task NonAdmin_CannotOpenAdminPages()
    {
        using HttpClient client = WebTestHost.ClientAs("user");

        HttpResponseMessage response = await client.GetAsync("/admin/regions");

        Assert.AreNotEqual(HttpStatusCode.OK, response.StatusCode, "a plain user must not see the Regions page");
    }

    [DataTestMethod]
    [DataRow("/")]
    [DataRow("/about")]
    [DataRow("/members")]
    [DataRow("/members/edit")]
    [DataRow("/members/deactivate")]
    [DataRow("/members/labels")]
    [DataRow("/tournaments")]
    [DataRow("/tournaments/edit")]
    [DataRow("/reports")]
    [DataRow("/admin/regions")]
    [DataRow("/admin/regions/edit")]
    [DataRow("/admin/users")]
    [DataRow("/admin/users/edit")]
    public async Task Admin_CanOpenPage(string path)
    {
        using HttpClient client = WebTestHost.ClientAs("admin");

        HttpResponseMessage response = await client.GetAsync(path);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, path);
    }

    [TestMethod]
    public async Task Admin_CanOpenEveryTournamentPage()
    {
        using HttpClient client = WebTestHost.ClientAs("admin");
        int id = WebTestHost.SeededTournamentId;
        string[] paths =
        [
            $"/tournaments/{id}", $"/tournaments/edit/{id}", $"/tournaments/{id}/stats", $"/scores/{id}",
            $"/standings/{id}", $"/results/{id}", $"/finalize/{id}", $"/doubles/{id}", $"/doubles/{id}/discrepancies",
        ];

        foreach (string path in paths)
        {
            HttpResponseMessage response = await client.GetAsync(path);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, path);
        }
    }

    [TestMethod]
    public async Task Admin_CanOpenMemberPages()
    {
        using HttpClient client = WebTestHost.ClientAs("admin");
        int number = WebTestHost.SeededMemberNumber;

        foreach (string path in new[] { $"/members/edit/{number}", $"/members/{number}/stats" })
        {
            HttpResponseMessage response = await client.GetAsync(path);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, path);
        }
    }

    [TestMethod]
    public async Task PrintPages_RenderStatically()
    {
        using HttpClient client = WebTestHost.ClientAs("user");
        int number = WebTestHost.SeededMemberNumber;
        int id = WebTestHost.SeededTournamentId;

        HttpResponseMessage recap = await client.GetAsync($"/print/members/{number}/recap");
        Assert.AreEqual(HttpStatusCode.OK, recap.StatusCode);
        StringAssert.Contains(await recap.Content.ReadAsStringAsync(), "Tester");

        HttpResponseMessage labels = await client.GetAsync($"/print/members/labels?ids=&start=1");
        Assert.AreEqual(HttpStatusCode.OK, labels.StatusCode);

        HttpResponseMessage active = await client.GetAsync("/print/members/active-recaps");
        Assert.AreEqual(HttpStatusCode.OK, active.StatusCode);

        HttpResponseMessage recaps = await client.GetAsync($"/print/tournaments/{id}/recaps");
        Assert.AreEqual(HttpStatusCode.OK, recaps.StatusCode);

        HttpResponseMessage standings = await client.GetAsync($"/print/standings/{id}?type=series&handicap=true&qualify=0&count=5&dues=false");
        Assert.AreEqual(HttpStatusCode.OK, standings.StatusCode);
    }

    [TestMethod]
    public async Task MemberEdit_HidesSsnFromNonAdmins()
    {
        int number = WebTestHost.SeededMemberNumber;

        using HttpClient user = WebTestHost.ClientAs("user");
        string userHtml = await (await user.GetAsync($"/members/edit/{number}")).Content.ReadAsStringAsync();
        Assert.IsFalse(userHtml.Contains("id=\"m-ssn\""), "non-admins must not see the SSN field");

        using HttpClient admin = WebTestHost.ClientAs("admin");
        string adminHtml = await (await admin.GetAsync($"/members/edit/{number}")).Content.ReadAsStringAsync();
        Assert.IsTrue(adminHtml.Contains("id=\"m-ssn\""), "admins see the SSN field");
    }
}
