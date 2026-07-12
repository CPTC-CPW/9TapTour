using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Data.Repositories;   // FinalizeRepository
using NineTapTour.Models;

namespace NineTapTour.IntegrationTests
{
    /// <summary>
    /// Full-database scenario tests for <see cref="FinalizeRepository"/> against seeded SQL Server
    /// LocalDB data. Each test seeds its own rows with process-unique member numbers so the tests
    /// stay independent while sharing the run-wide database.
    /// </summary>
    [TestClass]
    [TestCategory("Integration")]
    public class FinalizeRepositoryTests
    {
        [TestMethod]
        public void Get30GameAverage_NoHistory_ReturnsZero()
        {
            var factory = IntegrationEnvironment.Require();

            int memberNumber = TestSeed.NextNumber();
            int emptyTournamentId;
            using (var db = factory.CreateDbContext())
            {
                // A member with no games and an empty tournament (no participants).
                TestSeed.AddMember(db, memberNumber);
                var t = TestSeed.AddTournament(db);
                db.SaveChanges();
                emptyTournamentId = t.Id;
            }

            var repo = new FinalizeRepository(factory);

            double result = repo.Get30GameAverage(memberNumber, emptyTournamentId);

            Assert.AreEqual(0d, result);
        }

        [TestMethod]
        public void GetMembersGameEntryCount_CountsEntries()
        {
            var factory = IntegrationEnvironment.Require();

            int memberNumber = TestSeed.NextNumber();
            int tournamentId;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);
                // One member with two game entries (two games + two participants).
                var member = TestSeed.AddMember(db, memberNumber);

                var game1 = TestSeed.AddGame(db, 200, 180, 190, 210, finalized: true);
                TestSeed.AddParticipant(db, t, member, game1, squad: 1);

                var game2 = TestSeed.AddGame(db, 150, 160, 170, 140, finalized: true);
                TestSeed.AddParticipant(db, t, member, game2, squad: 2);

                db.SaveChanges();
                tournamentId = t.Id;
            }

            var repo = new FinalizeRepository(factory);

            int count = repo.GetMembersGameEntryCount(tournamentId, memberNumber);

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void GetParticipantByGameId_ReturnsParticipantWithMember()
        {
            var factory = IntegrationEnvironment.Require();

            int memberNumber = TestSeed.NextNumber();
            int gameId;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);
                var p = TestSeed.AddEntry(db, t, 1, 200, 180, 190, 210, finalized: true, memberNumber: memberNumber);
                db.SaveChanges();
                gameId = p.Game.Id;
            }

            var repo = new FinalizeRepository(factory);

            Participant result = repo.GetParticipantByGameId(gameId);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Member);
            Assert.AreEqual(memberNumber, result.Member.Number);
        }

        [TestMethod]
        public void GetParticipantByGameId_UnknownGame_ReturnsNull()
        {
            var factory = IntegrationEnvironment.Require();

            var repo = new FinalizeRepository(factory);

            Participant result = repo.GetParticipantByGameId(-999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void AddFinalizeTemp_PersistsFinalizationFieldsOntoGame()
        {
            var factory = IntegrationEnvironment.Require();

            int memberNumber = TestSeed.NextNumber();
            int gameId, memberId;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);
                // Average non-null so the member handicap can be computed during finalization.
                var member = TestSeed.AddMember(db, memberNumber, average: 180);
                var game = TestSeed.AddGame(db, 200, 180, 190, 210, finalized: true);
                TestSeed.AddParticipant(db, t, member, game, squad: 1);
                db.SaveChanges();
                gameId = game.Id;
                memberId = member.Id;
            }

            var repo = new FinalizeRepository(factory);

            var vm = new GameViewModel
            {
                GameId = gameId,
                MemberId = memberId,
                LeagueAverage = 195.5,
                AdjustedAvg = 190,
                KeepAdjustedAvg = true,
                HandicapTotal = 210,
            };

            repo.AddFinalizeTemp(vm);

            using (var db = factory.CreateDbContext())
            {
                var game = db.Games.Single(g => g.Id == gameId);
                Assert.AreEqual(195.5, game.LeagueAverage);
                Assert.AreEqual(190, game.AdjustedAvg);
                Assert.IsTrue(game.KeepAdjustedAvg);
                Assert.AreEqual(210, game.HandicapTotal);
            }
        }

        [TestMethod]
        public void DeleteParticipant_RemovesRow()
        {
            var factory = IntegrationEnvironment.Require();

            int memberNumber = TestSeed.NextNumber();
            int gameId;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);
                var p = TestSeed.AddEntry(db, t, 1, 200, 180, 190, 210, finalized: true, memberNumber: memberNumber);
                db.SaveChanges();
                gameId = p.Game.Id;
            }

            var repo = new FinalizeRepository(factory);

            Participant participant = repo.GetParticipantByGameId(gameId);
            Assert.IsNotNull(participant);

            repo.DeleteParticipant(participant);

            Assert.IsNull(repo.GetParticipantByGameId(gameId));
        }
    }
}
