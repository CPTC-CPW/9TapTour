using NineTapTour.Core.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Startup;
using System.Collections.Generic;

namespace NineTapTourTests.Startup
{
    [TestClass]
    public class ServiceConfigurationTests
    {
        /// <summary>
        /// Builds the real service collection and validates every registration
        /// can be constructed, so a missing dependency fails in CI instead of
        /// at runtime when a menu item is clicked.
        /// </summary>
        [TestMethod]
        public void AddNineTapTourServices_AllRegistrationsAreResolvable()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionStrings:NineTapDb"] = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NineTapDbValidationOnly;Integrated Security=True",
                })
                .Build();

            ServiceCollection services = new();
            services.AddNineTapTourServices(configuration);

            using ServiceProvider provider = services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true,
            });
        }

        [TestMethod]
        public void AddNineTapTourServices_MissingConnectionString_Throws()
        {
            IConfiguration configuration = new ConfigurationBuilder().Build();
            ServiceCollection services = new();

            Assert.ThrowsExactly<System.InvalidOperationException>(
                () => services.AddNineTapTourServices(configuration));
        }
    }
}
