using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Services;
using NineTapTour.Web.Startup;

namespace NineTapTour.Web.Tests;

/// <summary>
/// Structural rules for the Razor components, enforced by reflection so a new
/// page cannot quietly bypass them.
/// </summary>
[TestClass]
public class ComponentArchitectureTests
{
    private static readonly Assembly WebAssembly = typeof(Program).Assembly;

    private static IEnumerable<Type> Components => WebAssembly.GetTypes()
        .Where(t => typeof(IComponent).IsAssignableFrom(t) && !t.IsAbstract);

    private static IEnumerable<Type> RoutablePages => Components
        .Where(t => t.GetCustomAttributes<RouteAttribute>().Any());

    [TestMethod]
    public void NoComponentInjectsTheDesktopTournamentSession()
    {
        var offenders = Components
            .Where(t => t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(p => p.GetCustomAttribute<InjectAttribute>() != null && p.PropertyType == typeof(ITournamentSession)))
            .Select(t => t.FullName)
            .ToList();

        Assert.AreEqual(0, offenders.Count, "Components must take the tournament id from the route, not ITournamentSession: " + string.Join(", ", offenders));
    }

    [TestMethod]
    public void OnlyAccountErrorAndNotFoundPagesAllowAnonymous()
    {
        string[] allowed = ["NineTapTour.Web.Components.Pages.Error", "NineTapTour.Web.Components.Pages.NotFound"];

        var offenders = RoutablePages
            .Where(t => t.GetCustomAttribute<AllowAnonymousAttribute>() != null)
            .Where(t => !t.Namespace!.StartsWith("NineTapTour.Web.Components.Account", StringComparison.Ordinal))
            .Where(t => !allowed.Contains(t.FullName))
            .Select(t => t.FullName)
            .ToList();

        Assert.AreEqual(0, offenders.Count, "Unexpected anonymous pages: " + string.Join(", ", offenders));
    }

    [TestMethod]
    public void EveryPageOutsideAccountRequiresAuthorization()
    {
        var offenders = RoutablePages
            .Where(t => !t.Namespace!.StartsWith("NineTapTour.Web.Components.Account", StringComparison.Ordinal))
            .Where(t => t.GetCustomAttribute<AllowAnonymousAttribute>() == null)
            .Where(t => t.GetCustomAttributes<AuthorizeAttribute>().Any() == false)
            .Select(t => t.FullName)
            .ToList();

        Assert.AreEqual(0, offenders.Count, "Pages missing [Authorize]: " + string.Join(", ", offenders));
    }

    [TestMethod]
    public void AdminPagesRequireTheAdminPolicy()
    {
        var adminPages = RoutablePages
            .Where(t => t.Namespace!.StartsWith("NineTapTour.Web.Components.Pages.Admin", StringComparison.Ordinal))
            .ToList();
        Assert.IsTrue(adminPages.Count >= 4, "expected the Regions and Users pages");

        var offenders = adminPages
            .Where(t => !t.GetCustomAttributes<AuthorizeAttribute>().Any(a => a.Policy == WebServiceConfiguration.AdminOnlyPolicy))
            .Select(t => t.FullName)
            .ToList();

        Assert.AreEqual(0, offenders.Count, "Admin pages without the AdminOnly policy: " + string.Join(", ", offenders));
    }

    [TestMethod]
    public void PrintPagesAreStaticallyRendered()
    {
        var printPages = RoutablePages
            .Where(t => t.GetCustomAttributes<RouteAttribute>().Any(r => r.Template.StartsWith("/print/", StringComparison.Ordinal)))
            .ToList();
        Assert.IsTrue(printPages.Count >= 4, "expected recap, labels, and standings print pages");

        var offenders = printPages
            .Where(t => t.GetCustomAttribute<ExcludeFromInteractiveRoutingAttribute>() == null)
            .Select(t => t.FullName)
            .ToList();

        Assert.AreEqual(0, offenders.Count, "Print pages must be [ExcludeFromInteractiveRouting]: " + string.Join(", ", offenders));
    }
}
