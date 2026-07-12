using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Data.Repositories;
using NineTapTour.Models;

namespace NineTapTour.IntegrationTests
{
    /// <summary>
    /// Full-database scenario tests for the standings queries: real repositories against seeded
    /// SQL Server LocalDB data. Proves the scratch/handicap/3-of-4 standings end-to-end.
    /// </summary>
    [TestClass]
    [TestCategory("Integration")]
    public class StandingsIntegrationTests
    {
        private static int _scratchTournamentId;
        private static int _threeOfFourTournamentId;

        [ClassInitialize]
        public static void ClassInit(TestContext _)
        {
            if (!IntegrationEnvironment.Available)
            {
                return; // tests self-skip via IntegrationEnvironment.Require()
            }

            using var db = IntegrationEnvironment.Factory.CreateDbContext();

            // Scratch tournament (not 3-of-4).
            var scratch = TestSeed.AddTournament(db, location: "Scratch Cup", squads: 2);
            TestSeed.AddEntry(db, scratch, squad: 1, 200, 180, 190, 210, handicap: 20, bonus: 2, first: "Ann", last: "Ace", memberNumber: 101);
            TestSeed.AddEntry(db, scratch, squad: 1, 150, 160, 170, 140, handicap: 30, bonus: 0, first: "Bob", last: "Bell", memberNumber: 102);
            TestSeed.AddEntry(db, scratch, squad: 2, 220, 210, 200, null, handicap: 10, bonus: 1, first: "Cal", last: "Cash", memberNumber: 103);

            // 3-of-4 tournament.
            var threeOfFour = TestSeed.AddTournament(db, threeOfFour: true, squads: 1, location: "3-of-4 Open");
            TestSeed.AddEntry(db, threeOfFour, squad: 1, 250, 250, 250, 100, handicap: 0, bonus: 0, first: "Dan", last: "Drop", memberNumber: 201);

            db.SaveChanges();
            _scratchTournamentId = scratch.Id;
            _threeOfFourTournamentId = threeOfFour.Id;
        }

        [TestMethod]
        public void GetStandingsForTournamentByScratch_ReturnsGameTotals_NotZero()
        {
            var repo = new StandingsRepository(IntegrationEnvironment.Require());

            List<MemberScores> standings = repo.GetStandingsForTournamentByScratch(_scratchTournamentId);

            Assert.HasCount(3, standings);
            Assert.AreEqual(780, standings.Single(m => m.MemberId == 101).Score, "Scratch total should be the sum of the four games.");
            Assert.AreEqual(620, standings.Single(m => m.MemberId == 102).Score);
            Assert.AreEqual(630, standings.Single(m => m.MemberId == 103).Score, "A missing 4th game must not null/zero the scratch total.");
        }

        [TestMethod]
        public void GetStandingsForTournamentByHandicap_AddsHandicapAndBonusPerGame()
        {
            var repo = new StandingsRepository(IntegrationEnvironment.Require());

            List<MemberScores> standings = repo.GetStandingsForTournamentByHandicap(_scratchTournamentId);

            // Ann: 780 + 4*(20+2) = 868 ; Cal (3 games): 630 + 3*(10+1) = 663
            Assert.AreEqual(868, standings.Single(m => m.MemberId == 101).Score);
            Assert.AreEqual(663, standings.Single(m => m.MemberId == 103).Score);
        }

        [TestMethod]
        public void GetStandingsForTournamentByScratch_ThreeOfFour_DropsLowestGame()
        {
            var repo = new StandingsRepository(IntegrationEnvironment.Require());

            List<MemberScores> standings = repo.GetStandingsForTournamentByScratch(_threeOfFourTournamentId, isThreeOfFourTournament: true);

            Assert.AreEqual(750, standings.Single(m => m.MemberId == 201).Score, "The lowest game (100) should be dropped.");
        }

        [TestMethod]
        public void GetStandingsForTournamentByFilterSeriesByScratch_FiltersToSelectedSquads()
        {
            var repo = new StandingsRepository(IntegrationEnvironment.Require());

            // Only squad 1 (Ann + Bob); Cal is squad 2 and must be excluded.
            List<MemberScores> squad1 = repo.GetStandingsForTournamentByFilterSeriesByScratch(new List<int> { 1 }, _scratchTournamentId);

            Assert.HasCount(2, squad1);
            Assert.IsTrue(squad1.All(m => m.MemberId == 101 || m.MemberId == 102));
            Assert.AreEqual(780, squad1.Single(m => m.MemberId == 101).Score);
        }
    }
}
