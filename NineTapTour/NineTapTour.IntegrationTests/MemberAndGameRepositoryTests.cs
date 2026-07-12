using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Data.Repositories;   // MemberRepository, GameRepository
using NineTapTour.Models;

namespace NineTapTour.IntegrationTests
{
    /// <summary>
    /// Full-database scenario tests for <see cref="MemberRepository"/> and <see cref="GameRepository"/>
    /// against seeded SQL Server LocalDB data. Each test seeds its own rows with process-unique member
    /// numbers so the tests stay independent while sharing the run-wide database.
    /// </summary>
    [TestClass]
    [TestCategory("Integration")]
    public class MemberAndGameRepositoryTests
    {
        [TestMethod]
        public void AddOrUpdateMember_Insert_ComputesHandicapFromAverage()
        {
            var factory = IntegrationEnvironment.Require();

            int number = TestSeed.NextNumber();
            var repo = new MemberRepository(factory);

            repo.AddOrUpdateMember(new Member
            {
                Number = number,
                FirstName = "H",
                LastName = "Cap",
                Average = 180,
                IsActive = true,
            });

            // (220 - 180) * 90 / 100 = 3600 / 100 = 36
            Member reloaded = repo.GetMember(number);
            Assert.AreEqual(36, reloaded.Handicap);
        }

        [TestMethod]
        public void AddOrUpdateMember_HighAverageCapsHandicapAt70()
        {
            var factory = IntegrationEnvironment.Require();

            int number = TestSeed.NextNumber();
            var repo = new MemberRepository(factory);

            repo.AddOrUpdateMember(new Member
            {
                Number = number,
                FirstName = "H",
                LastName = "Cap",
                Average = 120,
                IsActive = true,
            });

            // (220 - 120) * 90 / 100 = 90 -> capped at 70
            Member reloaded = repo.GetMember(number);
            Assert.AreEqual(70, reloaded.Handicap);
        }

        [TestMethod]
        public void AddOrUpdateMember_Update_ChangesFields()
        {
            var factory = IntegrationEnvironment.Require();

            int number = TestSeed.NextNumber();
            int memberId;
            using (var db = factory.CreateDbContext())
            {
                var m = TestSeed.AddMember(db, number, first: "Orig", last: "Name");
                db.SaveChanges();
                memberId = m.Id;
            }

            var repo = new MemberRepository(factory);

            // A detached copy carrying the same Id so AddOrUpdateMember takes the update path.
            repo.AddOrUpdateMember(new Member
            {
                Id = memberId,
                Number = number,
                FirstName = "Orig",
                LastName = "Changed",
                IsActive = true,
            });

            Member reloaded = repo.GetMember(number);
            Assert.AreEqual("Changed", reloaded.LastName);
        }

        [TestMethod]
        public void MemberExists_TrueForSeeded_FalseForUnknown()
        {
            var factory = IntegrationEnvironment.Require();

            int number = TestSeed.NextNumber();
            using (var db = factory.CreateDbContext())
            {
                TestSeed.AddMember(db, number);
                db.SaveChanges();
            }

            var repo = new MemberRepository(factory);

            Assert.IsTrue(repo.MemberExists(new Member { Number = number }));
            Assert.IsFalse(repo.MemberExists(new Member { Number = -999 }));
        }

        [TestMethod]
        public void GetMember_UnknownNumber_ReturnsEmptyMember()
        {
            var factory = IntegrationEnvironment.Require();

            var repo = new MemberRepository(factory);

            Member result = repo.GetMember(-999);

            Assert.AreEqual(0, result.Id);
        }

        [TestMethod]
        public void GetMemberByGameId_ReturnsOwningMember()
        {
            var factory = IntegrationEnvironment.Require();

            int memberNumber = TestSeed.NextNumber();
            int gameId;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);
                var member = TestSeed.AddMember(db, memberNumber);
                var game = TestSeed.AddGame(db, 200, 180, 190, 210);
                TestSeed.AddParticipant(db, t, member, game, squad: 1);
                db.SaveChanges();
                gameId = game.Id;
            }

            var repo = new MemberRepository(factory);

            Member result = repo.GetMemberByGameId(gameId);

            Assert.IsNotNull(result);
            Assert.AreEqual(memberNumber, result.Number);
        }

        [TestMethod]
        public void GetGame_ReturnsSeededGame_And_Unknown_ReturnsNull()
        {
            var factory = IntegrationEnvironment.Require();

            int gameId;
            using (var db = factory.CreateDbContext())
            {
                var game = TestSeed.AddGame(db, 200, 180, 190, 210);
                db.SaveChanges();
                gameId = game.Id;
            }

            var repo = new GameRepository(factory);

            Game seeded = repo.GetGame(gameId);
            Assert.IsNotNull(seeded);
            Assert.AreEqual(200, seeded.Game1);

            Assert.IsNull(repo.GetGame(-999));
        }

        [TestMethod]
        public void GetFinalizedGamesByTournament_ReturnsOnlyFinalized()
        {
            var factory = IntegrationEnvironment.Require();

            int tournamentId, finalizedGameId, unfinalizedGameId;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);

                var finalized = TestSeed.AddEntry(db, t, 1, 200, 180, 190, 210,
                    finalized: true, memberNumber: TestSeed.NextNumber());
                var notFinalized = TestSeed.AddEntry(db, t, 2, 150, 160, 170, 140,
                    finalized: false, memberNumber: TestSeed.NextNumber());

                db.SaveChanges();
                tournamentId = t.Id;
                finalizedGameId = finalized.Game.Id;
                unfinalizedGameId = notFinalized.Game.Id;
            }

            var repo = new GameRepository(factory);

            var finalizedGames = repo.GetFinalizedGamesByTournament(tournamentId);
            Assert.AreEqual(1, finalizedGames.Count);
            Assert.IsTrue(finalizedGames.Single().IsFinalized);
            Assert.AreEqual(finalizedGameId, finalizedGames.Single().Id);

            Assert.IsTrue(repo.IsGameFinalized(finalizedGameId));
            Assert.IsFalse(repo.IsGameFinalized(unfinalizedGameId));
        }
    }
}
