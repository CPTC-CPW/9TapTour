using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Database;

namespace NineTapTour.IntegrationTests
{
    /// <summary>
    /// Owns a single migrated LocalDB database for the whole test run (created once, dropped once).
    /// Individual tests seed their own rows (with unique member numbers / their own tournaments) so
    /// they stay independent. Tests call <see cref="Require"/> to get the factory or self-skip when
    /// LocalDB is unavailable.
    /// </summary>
    [TestClass]
    public static class IntegrationEnvironment
    {
        private static LocalDbFixture _fixture;

        /// <summary>True when SQL Server LocalDB was reachable and the shared database was created.</summary>
        public static bool Available { get; private set; }

        /// <summary>The shared context factory (null when LocalDB is unavailable).</summary>
        public static IDbContextFactory<NineTapDb> Factory => _fixture?.Factory;

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext _)
        {
            if (LocalDbFixture.IsLocalDbAvailable())
            {
                _fixture = new LocalDbFixture();
                Available = true;
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            _fixture?.Dispose();
        }

        /// <summary>Returns the shared context factory, or marks the test inconclusive when LocalDB is absent.</summary>
        public static IDbContextFactory<NineTapDb> Require()
        {
            if (!Available)
            {
                Assert.Inconclusive("SQL Server LocalDB is not available; skipping integration test.");
            }
            return _fixture.Factory;
        }
    }
}
