using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using System;
using System.Diagnostics;

namespace NineTapTour.IntegrationTests
{
    /// <summary>
    /// Creates a unique LocalDB catalog for the test run, migrates it, seeds
    /// the canonical dataset, and exposes a DbFactory pointed at it for
    /// constructing repositories under test. The catalog is dropped when the
    /// assembly finishes.
    /// </summary>
    [TestClass]
    public static class TestDatabase
    {
        private const string ServerConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=60;Encrypt=False";

        public static string CatalogName { get; private set; }

        /// <summary>
        /// Factory pointed at the test catalog; repositories under test are
        /// constructed with this.
        /// </summary>
        public static IDbContextFactory<NineTapDb> DbFactory { get; private set; }

        // Seeded tournament ids, captured after SaveChanges
        public static int ThreeOf4TournamentId { get; private set; }
        public static int RegularTournamentId { get; private set; }

        /// <summary>
        /// Maps member Number (101..107) to the database identity Id. The raw
        /// SQL standings return Members.Id while the EF paths return
        /// Member.Number — tests must assert against the right one.
        /// </summary>
        public static System.Collections.Generic.Dictionary<int, int> DbIdByNumber { get; } = [];

        [AssemblyInitialize]
        public static void CreateAndSeed(TestContext context)
        {
            CatalogName = $"NineTapDb_Test_{DateTime.Now:yyyyMMddHHmmss}_{Environment.ProcessId}";
            string connectionString = $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={CatalogName};Integrated Security=True;Connect Timeout=60;Encrypt=False";

            DbContextOptionsBuilder<NineTapDb> optionsBuilder = new();
            optionsBuilder.UseSqlServer(connectionString);
            DbFactory = new TestDbFactory(optionsBuilder.Options);

            using (NineTapDb db = DbFactory.CreateDbContext())
            {
                db.Database.Migrate();
            }

            Seed();
        }

        private sealed class TestDbFactory : IDbContextFactory<NineTapDb>
        {
            private readonly DbContextOptions<NineTapDb> options;

            public TestDbFactory(DbContextOptions<NineTapDb> options)
            {
                this.options = options;
            }

            public NineTapDb CreateDbContext()
            {
                return new NineTapDb(options);
            }
        }

        [AssemblyCleanup]
        public static void Drop()
        {
            if (CatalogName == null)
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

        /// <summary>
        /// Canonical dataset used by the golden-master tests. Payment dates are
        /// relative to today so paid/unpaid classifications are stable year round.
        /// Scores are chosen so every standings ordering is tie free.
        /// </summary>
        private static void Seed()
        {
            using NineTapDb db = DbFactory.CreateDbContext();

            Member m101 = NewMember(101, "Alice", "Anderson", average: 150, handicap: 63, bonus: 2, isSenior: false, lastPayment: DateTime.Today, isLifetime: false);
            Member m102 = NewMember(102, "Bob", "Baker", average: 180, handicap: 36, bonus: 0, isSenior: true, lastPayment: DateTime.Today.AddYears(-2), isLifetime: false);
            Member m103 = NewMember(103, "Carol", "Chen", average: 200, handicap: 18, bonus: 5, isSenior: false, lastPayment: DateTime.Today, isLifetime: false);
            Member m104 = NewMember(104, "Dave", "Diaz", average: 220, handicap: 0, bonus: 1, isSenior: false, lastPayment: DateTime.Today, isLifetime: false);
            Member m105 = NewMember(105, "Eve", "Evans", average: 140, handicap: 70, bonus: 3, isSenior: true, lastPayment: DateTime.Today, isLifetime: false);
            Member m106 = NewMember(106, "Frank", "Fox", average: 160, handicap: 54, bonus: 0, isSenior: false, lastPayment: null, isLifetime: false);
            // Lifetime member participates ONLY in the regular tournament: the raw
            // 3-of-4 SQL crashes when a lifetime member is present (its CASE mixes
            // 'life' varchar with YEAR() int) — that behavior gets its own test.
            Member m107 = NewMember(107, "Grace", "Gill", average: 200, handicap: 18, bonus: 5, isSenior: false, lastPayment: null, isLifetime: true);
            db.Members.AddRange(m101, m102, m103, m104, m105, m106, m107);

            Tournament threeOf4 = new()
            {
                Date = new DateTime(2026, 1, 10),
                Location = "Golden Master Lanes",
                Event = "3of4 Golden Master",
                Sponsors = "",
                Notes = "",
                Squads = 2,
                ThreeOutOf4 = true,
            };
            Tournament regular = new()
            {
                Date = new DateTime(2026, 2, 14),
                Location = "Golden Master Lanes",
                Event = "Regular Golden Master",
                Sponsors = "",
                Notes = "",
                Squads = 1,
            };
            db.Tournaments.AddRange(threeOf4, regular);

            // Three-of-four tournament: every participant has all 4 games so the
            // drop-lowest SQL never sees nulls (matching real usage of that path).
            AddParticipant(db, threeOf4, m101, squad: 1, 150, 160, 170, 180, handicap: 63, bonus: 2);
            AddParticipant(db, threeOf4, m102, squad: 1, 200, 190, 180, 170, handicap: 36, bonus: 0);
            AddParticipant(db, threeOf4, m103, squad: 1, 210, 211, 212, 214, handicap: 18, bonus: 5);
            AddParticipant(db, threeOf4, m104, squad: 2, 220, 110, 230, 240, handicap: 0, bonus: 1);
            AddParticipant(db, threeOf4, m105, squad: 2, 100, 105, 110, 115, handicap: 70, bonus: 3);
            AddParticipant(db, threeOf4, m106, squad: 2, 121, 122, 123, 124, handicap: 54, bonus: 0);

            // Regular tournament: includes participants with missing games to
            // pin down the null-game handling of the EF standings queries.
            AddParticipant(db, regular, m101, squad: 1, 150, 160, 170, 180, handicap: 63, bonus: 2);
            AddParticipant(db, regular, m105, squad: 1, 190, 200, null, null, handicap: 70, bonus: 3);
            AddParticipant(db, regular, m106, squad: 1, 180, 190, 200, null, handicap: 54, bonus: 0);
            AddParticipant(db, regular, m107, squad: 1, 210, null, null, null, handicap: 18, bonus: 5);

            db.SaveChanges();

            ThreeOf4TournamentId = threeOf4.Id;
            RegularTournamentId = regular.Id;
            foreach (Member member in new[] { m101, m102, m103, m104, m105, m106, m107 })
            {
                DbIdByNumber[member.Number] = member.Id;
            }
        }

        private static Member NewMember(int number, string firstName, string lastName, int average, int handicap,
            int bonus, bool isSenior, DateTime? lastPayment, bool isLifetime)
        {
            return new Member
            {
                Number = number,
                IsActive = true,
                FirstName = firstName,
                LastName = lastName,
                MiddleInitial = "",
                Gender = MemberGenders.Female,
                Street = "1 Test St",
                City = "Testville",
                State = "WA",
                PostalCode = "98000",
                PrimaryPhone = "555-0100",
                Average = average,
                Handicap = handicap,
                Bonus = bonus,
                IsSenior = isSenior,
                LastPayment = lastPayment,
                IsLifetimeMember = isLifetime,
            };
        }

        private static void AddParticipant(NineTapDb db, Tournament tournament, Member member, int squad,
            int? game1, int? game2, int? game3, int? game4, int handicap, int bonus)
        {
            Game game = new()
            {
                Game1 = game1,
                Game2 = game2,
                Game3 = game3,
                Game4 = game4,
                Handicap = handicap,
                Bonus = bonus,
                MoneyWon = 0,
                IsComp = false,
            };
            db.Games.Add(game);
            db.Participants.Add(new Participant
            {
                Tournament = tournament,
                Member = member,
                Squad = squad,
                Game = game,
            });
        }
    }
}
