using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Data.Repositories;
using NineTapTour.Data.Services;
using NineTapTour.Models;

namespace NineTapTour.IntegrationTests
{
    /// <summary>
    /// Full-database tests for the winners-list computation extracted from FrmTournamentResults.
    /// </summary>
    [TestClass]
    [TestCategory("Integration")]
    public class TournamentResultsServiceTests
    {
        private static TournamentResultsService NewService(Microsoft.EntityFrameworkCore.IDbContextFactory<NineTapTour.Database.NineTapDb> factory)
            => new TournamentResultsService(new TournamentRepository(factory), new DoublesRepository(factory), factory);

        [TestMethod]
        public void BuildWinnersList_Singles_ComputesHandicappedTotal()
        {
            var factory = IntegrationEnvironment.Require();
            int tournamentId, num = TestSeed.NextNumber();
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);
                var m = TestSeed.AddMember(db, num, bonus: 2);
                var g = TestSeed.AddGame(db, 200, 180, 190, 210, handicap: 20);
                TestSeed.AddParticipant(db, t, m, g, 1);
                db.SaveChanges();
                tournamentId = t.Id;
            }

            WinnersListResult result = NewService(factory).BuildWinnersList(tournamentId, isDoubles: false, isThreeOfFour: false);

            Assert.AreEqual(1, result.TotalEntries);
            Assert.AreEqual(0, result.CompEntries);
            // 780 + 4*(handicap 20 + member bonus 2) = 868
            Assert.AreEqual(868, result.Bowlers.Single(b => b.MemberNumber == num).TotalScore);
        }

        [TestMethod]
        public void BuildWinnersList_ThreeOfFour_DropsLowestGame()
        {
            var factory = IntegrationEnvironment.Require();
            int tournamentId, num = TestSeed.NextNumber();
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db, threeOfFour: true);
                var m = TestSeed.AddMember(db, num, bonus: 0);
                var g = TestSeed.AddGame(db, 250, 250, 250, 100, handicap: 0);
                TestSeed.AddParticipant(db, t, m, g, 1);
                db.SaveChanges();
                tournamentId = t.Id;
            }

            WinnersListResult result = NewService(factory).BuildWinnersList(tournamentId, isDoubles: false, isThreeOfFour: true);

            // Drops the 100; 250*3 + 3*(0+0) = 750
            Assert.AreEqual(750, result.Bowlers.Single(b => b.MemberNumber == num).TotalScore);
        }

        [TestMethod]
        public void BuildWinnersList_CountsCompEntries()
        {
            var factory = IntegrationEnvironment.Require();
            int tournamentId, compNum = TestSeed.NextNumber(), payNum = TestSeed.NextNumber();
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);
                var comp = TestSeed.AddMember(db, compNum);
                var compGame = TestSeed.AddGame(db, 200, 200, 200, 200);
                compGame.IsComp = true;
                TestSeed.AddParticipant(db, t, comp, compGame, 1);

                var pay = TestSeed.AddMember(db, payNum);
                TestSeed.AddParticipant(db, t, pay, TestSeed.AddGame(db, 150, 150, 150, 150), 1);

                db.SaveChanges();
                tournamentId = t.Id;
            }

            WinnersListResult result = NewService(factory).BuildWinnersList(tournamentId, isDoubles: false, isThreeOfFour: false);

            Assert.AreEqual(2, result.TotalEntries);
            Assert.AreEqual(1, result.CompEntries, "Only the comp entry should be counted.");
        }

        [TestMethod]
        public void BuildWinnersList_Doubles_ProducesTwoTiedEntriesPerTeam()
        {
            var factory = IntegrationEnvironment.Require();
            int tournamentId, num1 = TestSeed.NextNumber(), num2 = TestSeed.NextNumber();
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db, doubles: true);
                var m1 = TestSeed.AddMember(db, num1, bonus: 0);
                var m2 = TestSeed.AddMember(db, num2, bonus: 0);
                TestSeed.AddParticipant(db, t, m1, TestSeed.AddGame(db, 200, 180, null, null, handicap: 10), 1);
                TestSeed.AddParticipant(db, t, m2, TestSeed.AddGame(db, 190, 170, null, null, handicap: 20), 1);
                db.DoublesTeams.Add(new DoublesTeam { Tournament = t, Member1 = m1, Member2 = m2, Squad = 1 });
                db.SaveChanges();
                tournamentId = t.Id;
            }

            WinnersListResult result = NewService(factory).BuildWinnersList(tournamentId, isDoubles: true, isThreeOfFour: false);

            Assert.HasCount(2, result.Bowlers, "A doubles team produces two entries.");
            // Combined = (200+180)+(190+170) + 2*(10+0) + 2*(20+0) = 740 + 20 + 40 = 800
            Assert.IsTrue(result.Bowlers.All(b => b.TotalScore == 800), "Both members share the combined team total.");
            Assert.IsTrue(result.Bowlers.All(b => b.PlaceStanding == 1), "Both members share the team's place.");
        }
    }
}
