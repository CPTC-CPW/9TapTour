using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Models;
using NineTapTour.Models.ViewModels;
using NineTapTour.Services;

namespace NineTapTour.Core.Tests
{
    [TestClass]
    public class StandingsReportServiceTests
    {
        private static Participant Entry(int number, int? g1, int? g2, int? g3, int? g4,
            int memberHandicap = 20, int memberBonus = 2, int gameHandicap = 20, int gameBonus = 2, int squad = 1)
        {
            return new Participant
            {
                Squad = squad,
                Member = new Member { Number = number, FirstName = "F" + number, LastName = "L" + number, Handicap = memberHandicap, Bonus = memberBonus },
                Game = new Game { Id = number, Game1 = g1, Game2 = g2, Game3 = g3, Game4 = g4, Handicap = gameHandicap, Bonus = gameBonus },
            };
        }

        private static readonly StandingsReportService Service = new();

        [TestMethod]
        public void BuildReport_HighGame_OrdersGameScoresByHighestGameDescending()
        {
            var participants = new List<Participant>
            {
                Entry(2, 150, 160, 170, 140), // high game 170
                Entry(1, 200, 180, 190, 210), // high game 210
            };

            var (gameScores, _) = Service.BuildReport(participants, isThreeOfFour: false, ReportType.HighGame);

            Assert.HasCount(2, gameScores);
            Assert.AreEqual(1, gameScores[0].MemberNo, "Highest single game (210) should be first.");
            Assert.AreEqual(210, gameScores[0].HighScore);
        }

        [TestMethod]
        public void BuildReport_SeriesScores_ComputeScratchAndTop3()
        {
            var participants = new List<Participant> { Entry(1, 200, 180, 190, 210) };

            var (_, series) = Service.BuildReport(participants, isThreeOfFour: false, ReportType.HighSeriesScratch);

            TopParticipantGameViewModel vm = series.Single(s => s.MemberNo == 1);
            Assert.AreEqual(780, vm.ScratchTotal, "Full scratch series is the sum of all four games.");
            // Top-3 of {200,180,190,210} drops 180 -> 600.
            Assert.AreEqual(600, vm.Top3ScratchScore);
        }

        [TestMethod]
        public void BuildReport_HighSeriesScratch_NotThreeOfFour_OrdersByScratchTotal()
        {
            var participants = new List<Participant>
            {
                Entry(1, 100, 100, 100, 100), // 400
                Entry(2, 200, 200, 200, 200), // 800
            };

            var (_, series) = Service.BuildReport(participants, isThreeOfFour: false, ReportType.HighSeriesScratch);

            Assert.AreEqual(2, series[0].MemberNo, "Higher scratch total (800) should be first.");
        }

        [TestMethod]
        public void BuildReport_HighSeriesScratch_ThreeOfFour_OrdersByTop3()
        {
            // Bowler 1 has a high 4-game total but a low best-3; bowler 2 the reverse.
            var participants = new List<Participant>
            {
                Entry(1, 250, 250, 250, 10),  // top-3 = 750
                Entry(2, 240, 240, 240, 230),  // top-3 = 720
            };

            var (_, series) = Service.BuildReport(participants, isThreeOfFour: true, ReportType.HighSeriesScratch);

            Assert.AreEqual(1, series[0].MemberNo, "Best-3 total should drive the 3-of-4 ordering.");
            Assert.AreEqual(750, series[0].Top3ScratchScore);
        }
    }
}
