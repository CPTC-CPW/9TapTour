using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Data.Repositories;   // TournamentRepository
using NineTapTour.Models;
using NineTapTour.Models.ViewModels;   // WinnerListMemberViewModel

namespace NineTapTour.IntegrationTests
{
    /// <summary>
    /// Full-database scenario tests for <see cref="TournamentRepository"/> against seeded SQL Server
    /// LocalDB data. Each test seeds its own rows (with process-unique member numbers / its own
    /// tournaments) so the tests stay independent while sharing the run-wide database.
    /// </summary>
    [TestClass]
    [TestCategory("Integration")]
    public class TournamentRepositoryTests
    {
        [TestMethod]
        public void AddTournament_InsertsNewTournament()
        {
            var factory = IntegrationEnvironment.Require();
            var repo = new TournamentRepository(factory);

            string location = "AddTournament_" + Guid.NewGuid().ToString("N");
            var t = new Tournament
            {
                Date = new DateTime(2026, 3, 15),
                Location = location,
                Event = "Test",
                Squads = 4,
            };

            repo.AddTournament(t);

            // EF populates the generated key on the passed-in object.
            Assert.IsTrue(t.Id > 0, "Expected the tournament Id to be populated after insert.");

            Tournament reloaded = repo.GetTourneyByID(t.Id);
            Assert.IsNotNull(reloaded);
            Assert.AreEqual(location, reloaded.Location);
        }

        [TestMethod]
        public void GetTournamentList_OrdersByDateDescending()
        {
            var factory = IntegrationEnvironment.Require();
            var repo = new TournamentRepository(factory);

            string prefix = "OrderTest_" + Guid.NewGuid().ToString("N");
            int earlierId, laterId;
            using (var db = factory.CreateDbContext())
            {
                var earlier = TestSeed.AddTournament(db, date: new DateTime(2026, 1, 1), location: prefix + "_early");
                var later = TestSeed.AddTournament(db, date: new DateTime(2026, 6, 1), location: prefix + "_late");
                db.SaveChanges();
                earlierId = earlier.Id;
                laterId = later.Id;
            }

            // Filter to just the two tournaments we seeded so other tests' data cannot interfere.
            List<Tournament> mine = repo.GetTournamentList()
                .Where(t => t.Id == earlierId || t.Id == laterId)
                .ToList();

            Assert.AreEqual(2, mine.Count);
            Assert.AreEqual(laterId, mine[0].Id, "The later-dated tournament should come first (descending).");
            Assert.AreEqual(earlierId, mine[1].Id);
        }

        [TestMethod]
        public void UpdateTournament_ChangesFields()
        {
            var factory = IntegrationEnvironment.Require();
            var repo = new TournamentRepository(factory);

            int tournamentId;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db, location: "Before_" + Guid.NewGuid().ToString("N"));
                db.SaveChanges();
                tournamentId = t.Id;
            }

            string newLocation = "After_" + Guid.NewGuid().ToString("N");
            Tournament detached = repo.GetTourneyByID(tournamentId); // AsNoTracking -> detached
            detached.Location = newLocation;

            bool result = repo.UpdateTournament(detached);
            Assert.IsTrue(result);

            Tournament reloaded = repo.GetTourneyByID(tournamentId);
            Assert.AreEqual(newLocation, reloaded.Location);
        }

        [TestMethod]
        public void UpdateTournament_UnknownId_Throws()
        {
            var factory = IntegrationEnvironment.Require();
            var repo = new TournamentRepository(factory);

            Assert.ThrowsExactly<System.ArgumentException>(() =>
                repo.UpdateTournament(new Tournament { Id = -999, Location = "x", Date = DateTime.Now }));
        }

        [TestMethod]
        public void GetUniqueTourMembers_ReturnsDistinctMembers()
        {
            var factory = IntegrationEnvironment.Require();
            var repo = new TournamentRepository(factory);

            int tournamentId;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db, location: "Unique_" + Guid.NewGuid().ToString("N"));
                // Two entries for two distinct members.
                TestSeed.AddEntry(db, t, 1, 200, 180, 190, 210, memberNumber: TestSeed.NextNumber());
                TestSeed.AddEntry(db, t, 1, 150, 160, 170, 140, memberNumber: TestSeed.NextNumber());
                db.SaveChanges();
                tournamentId = t.Id;
            }

            Tournament tourn = repo.GetTourneyByID(tournamentId);
            List<Member> members = repo.GetUniqueTourMembers(tourn);

            Assert.AreEqual(2, members.Count);
        }

        [TestMethod]
        public void AddMemberToTournament_InsertsParticipant()
        {
            var factory = IntegrationEnvironment.Require();
            var repo = new TournamentRepository(factory);

            int tournamentId, memberId;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db, location: "AddMember_" + Guid.NewGuid().ToString("N"));
                var member = TestSeed.AddMember(db, TestSeed.NextNumber());
                db.SaveChanges();
                tournamentId = t.Id;
                memberId = member.Id;
            }

            // Load the existing tournament + member fresh (detached), and pair with a brand-new game.
            Tournament tournament;
            Member existingMember;
            using (var db = factory.CreateDbContext())
            {
                tournament = db.Tournaments.AsNoTracking().Single(t => t.Id == tournamentId);
                existingMember = db.Members.AsNoTracking().Single(m => m.Id == memberId);
            }

            var participant = new Participant
            {
                Member = existingMember,
                Game = new Game
                {
                    Game1 = 200,
                    Game2 = 180,
                    Game3 = 190,
                    Game4 = 210,
                    UseGame1 = true,
                    UseGame2 = true,
                    UseGame3 = true,
                    UseGame4 = true,
                },
                Tournament = tournament,
                Squad = 1,
            };

            repo.AddMemberToTournament(participant);

            Assert.AreEqual(1, repo.GetTotalNumberParticipantsInTournament(tournament));

            // And the member now surfaces in the winner-list projection.
            List<WinnerListMemberViewModel> winners = repo.GetWinnerListMemberData(tournamentId);
            Assert.AreEqual(1, winners.Count);
            Assert.AreEqual(memberId, winners[0].MemberId);
        }

        [TestMethod]
        public void DeleteTournament_RemovesTournamentParticipantsAndGames()
        {
            var factory = IntegrationEnvironment.Require();
            var repo = new TournamentRepository(factory);

            int tournamentId, gameId, participantId;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db, location: "Delete_" + Guid.NewGuid().ToString("N"));
                var p = TestSeed.AddEntry(db, t, 1, 200, 180, 190, 210, memberNumber: TestSeed.NextNumber());
                db.SaveChanges();
                tournamentId = t.Id;
                gameId = p.Game.Id;
                participantId = p.Id;
            }

            Tournament tourn = repo.GetTourneyByID(tournamentId);
            Assert.IsNotNull(tourn);

            repo.DeleteTournament(tourn);

            using (var db = factory.CreateDbContext())
            {
                Assert.IsNull(db.Tournaments.Find(tournamentId), "Tournament should be deleted.");
                Assert.IsNull(db.Participants.Find(participantId), "Participants should be deleted.");
                Assert.IsNull(db.Games.Find(gameId), "Games should be deleted.");
            }
        }
    }
}
