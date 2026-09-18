using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.Services;
using NineTapTour.Web.Startup;

namespace NineTapTour.Web.Tests;

/// <summary>
/// Mirrors the desktop's ServiceConfigurationTests: the whole web container
/// must be constructible so a missing dependency fails in CI, not on a page.
/// </summary>
[TestClass]
public class WebServiceConfigurationTests
{
    [TestMethod]
    public void AddNineTapTourWeb_AllRegistrationsAreResolvable()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:NineTapDb"] = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NineTapDbValidationOnly;Integrated Security=True",
        });
        builder.Host.UseDefaultServiceProvider(options =>
        {
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
        });

        builder.Services.AddNineTapTourWeb(builder.Configuration, isDevelopment: false);

        using WebApplication app = builder.Build();
        using IServiceScope scope = app.Services.CreateScope();
        Assert.IsNotNull(scope.ServiceProvider.GetRequiredService<IRegionRepository>());
        Assert.IsNotNull(scope.ServiceProvider.GetRequiredService<IFinalizeGridService>());
        Assert.IsNotNull(scope.ServiceProvider.GetRequiredService<IResultsService>());
        Assert.IsNotNull(scope.ServiceProvider.GetRequiredService<IReportsService>());
        Assert.IsNotNull(scope.ServiceProvider.GetRequiredService<IMemberEditorService>());
    }

    [TestMethod]
    public void AddNineTapTourWeb_MissingConnectionString_Throws()
    {
        ServiceCollection services = new();
        IConfiguration configuration = new ConfigurationBuilder().Build();

        Assert.ThrowsExactly<InvalidOperationException>(
            () => services.AddNineTapTourWeb(configuration, isDevelopment: false));
    }
}
