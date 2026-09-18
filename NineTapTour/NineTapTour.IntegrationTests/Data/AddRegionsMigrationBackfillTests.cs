using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Data;
using System;
using System.Linq;

namespace NineTapTour.IntegrationTests.Data
{
    /// <summary>
    /// Rehearses the AddRegions migration against a database that already
    /// holds members and tournaments (the production situation). Uses its own
    /// throwaway catalog because the shared test catalog is migrated to the
    /// latest version before any test runs.
    /// </summary>
    [TestClass]
    public class AddRegionsMigrationBackfillTests
    {
        private const string ServerConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=60;Encrypt=False";
        private const string LastMigrationBeforeRegions = "20260814204547_AddTournamentIsImported";

        [TestMethod]
        public void AddRegions_SeedsDefaultRegionAndBackfillsExistingRows()
        {
            string catalog = $"NineTapDb_Backfill_{DateTime.Now:yyyyMMddHHmmss}_{Environment.ProcessId}";
            string connectionString = $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={catalog};Integrated Security=True;Connect Timeout=60;Encrypt=False";
            DbContextOptionsBuilder<NineTapDb> optionsBuilder = new();
            optionsBuilder.UseSqlServer(connectionString);

            try
            {
                using NineTapDb db = new(optionsBuilder.Options);

                // Schema as deployed before this change, then rows that predate regions.
                db.GetService<IMigrator>().Migrate(LastMigrationBeforeRegions);
                db.Database.ExecuteSqlRaw(
                    "INSERT INTO Members (Number, IsActive, Gender, Bonus, IsLifetimeMember, IsSenior, MoneyEarned, FirstName, LastName) " +
                    "VALUES (1, 1, 0, 0, 0, 0, 0, 'Old', 'MemberOne'), (2, 1, 1, 0, 0, 1, 0, 'Old', 'MemberTwo')");
                db.Database.ExecuteSqlRaw(
                    "INSERT INTO Tournaments (Date, Location, Squads, Doubles, ThreeOutOf4, IsOnlyThreeGames, IsTournamentFinalized, IsTwoDay, IsImported) " +
                    "VALUES ('2025-01-01', 'Old Lanes', 1, 0, 0, 0, 1, 0, 0), ('2025-02-01', 'Old Lanes', 2, 1, 0, 0, 0, 0, 0)");

                db.Database.Migrate();

                Assert.AreEqual(1, db.Regions.Count(), "exactly one region is seeded");
                int defaultId = db.Regions.Single().Id;
                Assert.AreEqual("Default", db.Regions.Single().Name);

                Assert.AreEqual(2, db.Members.Count());
                Assert.IsTrue(db.Members.All(m => m.RegionId == defaultId), "every existing member is backfilled");
                Assert.AreEqual(2, db.Tournaments.Count());
                Assert.IsTrue(db.Tournaments.All(t => t.RegionId == defaultId), "every existing tournament is backfilled");

                Assert.AreEqual("NO", GetIsNullable(connectionString, "Members"), "Members.RegionId must end up NOT NULL");
                Assert.AreEqual("NO", GetIsNullable(connectionString, "Tournaments"), "Tournaments.RegionId must end up NOT NULL");
            }
            finally
            {
                Drop(catalog);
            }
        }

        private static string GetIsNullable(string connectionString, string table)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();
            using SqlCommand command = new(
                "SELECT IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table AND COLUMN_NAME = 'RegionId'", connection);
            command.Parameters.AddWithValue("@table", table);
            return (string)command.ExecuteScalar();
        }

        private static void Drop(string catalog)
        {
            using SqlConnection connection = new(ServerConnectionString);
            connection.Open();
            using SqlCommand command = new(
                $"IF DB_ID('{catalog}') IS NOT NULL BEGIN " +
                $"ALTER DATABASE [{catalog}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                $"DROP DATABASE [{catalog}]; END", connection);
            command.ExecuteNonQuery();
        }
    }
}
