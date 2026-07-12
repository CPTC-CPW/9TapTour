using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Calculations;
using NineTapTour.Models;

namespace NineTapTourTests
{
    /// <summary>
    /// Scenario tests built from realistic tournament data. These lock in behavior for the scoring
    /// paths that were previously broken:
    ///  - duplicate-bowler removal crashing when a bowler had 3+ entries (multiple squads),
    ///  - 3-of-4 tournaments ranking by the 4-game total instead of the best 3,
    ///  - scratch series being summed with (missing) handicap/bonus, yielding zero/null totals.
    /// </summary>
    [TestClass]
    public class TournamentScenarioTests
    {
        // ------------------------------------------------------------------
        // Duplicate-bowler removal: a bowler entered in multiple squads must
        // collapse to their single highest entry, and must never throw when
        // three or more entries share a member number.
        // ------------------------------------------------------------------

        [TestMethod]
        public void CalculatePlaceStandings_MemberScores_ThreeEntriesSameBowler_KeepsHighestWithoutCrashing()
        {
            // Bowler 5 bowled three squads (100, 200, 150). Bowler 6 bowled once (175).
            List<MemberScores> members =
            [
                new() { MemberId = 5, FirstName = "Amy",  LastName = "Ace",  Score = 100 },
                new() { MemberId = 5, FirstName = "Amy",  LastName = "Ace",  Score = 200 },
                new() { MemberId = 5, FirstName = "Amy",  LastName = "Ace",  Score = 150 },
                new() { MemberId = 6, FirstName = "Bo",   LastName = "Bell", Score = 175 },
            ];

            List<MemberScores> result = TournamentCalculations.CalculatePlaceStandings(members);

            Assert.HasCount(2, result, "Each bowler should appear once after de-duplication.");

            MemberScores bowler5 = result.Single(m => m.MemberId == 5);
            Assert.AreEqual(200, bowler5.Score, "Only the bowler's highest entry should survive.");
            Assert.AreEqual(1, bowler5.placing, "Highest scorer places first.");
            Assert.AreEqual(2, result.Single(m => m.MemberId == 6).placing);
        }

        [TestMethod]
        public void CalculatePlaceStandings_ExcelMember_ThreeEntriesSameBowler_KeepsHighestWithoutCrashing()
        {
            List<ExcelMember> members =
            [
                new() { MemberNumber = 5, TotalScore = 100 },
                new() { MemberNumber = 5, TotalScore = 200 },
                new() { MemberNumber = 5, TotalScore = 150 },
                new() { MemberNumber = 6, TotalScore = 175 },
            ];

            List<ExcelMember> result = TournamentCalculations.CalculatePlaceStandings(members);

            Assert.HasCount(2, result);
            ExcelMember bowler5 = result.Single(m => m.MemberNumber == 5);
            Assert.AreEqual(200, bowler5.TotalScore);
            Assert.AreEqual(1, bowler5.PlaceStanding);
            Assert.AreEqual(2, result.Single(m => m.MemberNumber == 6).PlaceStanding);
        }

        [TestMethod]
        public void CalculatePlaceStandings_GameViewModel_ThreeEntriesSameBowler_KeepsHighestWithoutCrashing()
        {
            // Three entries for bowler 5 (ordered low, high, mid) reproduces the historic crash where
            // the losing first entry was added to the removal list twice.
            var low  = new GameViewModel { MemberNumber = 5, MemberId = 5, HandicapTotal = 100 };
            var high = new GameViewModel { MemberNumber = 5, MemberId = 5, HandicapTotal = 200 };
            var mid  = new GameViewModel { MemberNumber = 5, MemberId = 5, HandicapTotal = 150 };
            var other = new GameViewModel { MemberNumber = 6, MemberId = 6, HandicapTotal = 175 };

            var tournament = new Tournament { ThreeOutOf4 = false };

            Dictionary<GameViewModel, int> result =
                TournamentCalculations.CalculatePlaceStandings([low, high, mid, other], tournament);

            Assert.AreEqual(1, result[high], "Bowler 5's best entry places first.");
            Assert.AreEqual(2, result[other]);
            Assert.AreEqual(0, result[low],  "Removed duplicate entries are reported with placing 0.");
            Assert.AreEqual(0, result[mid]);
        }

        // ------------------------------------------------------------------
        // 3-of-4 tournaments: bowlers are ranked on their best three games,
        // not on the raw four-game total.
        // ------------------------------------------------------------------

        [TestMethod]
        public void CalculatePlaceStandings_GameViewModel_ThreeOfFour_RanksByBestThreeGames()
        {
            // Xavier: 250/250/250/100 -> 4-game 850, best-3 750.
            // Yolanda: 230/230/230/200 -> 4-game 890, best-3 690.
            // By four-game total Yolanda leads; by best-3 Xavier should win.
            var xavier = new GameViewModel
            {
                MemberNumber = 1, MemberId = 1, Handicap = 0, Bonus = 0,
                Game1 = 250, Game2 = 250, Game3 = 250, Game4 = 100, HandicapTotal = 850
            };
            var yolanda = new GameViewModel
            {
                MemberNumber = 2, MemberId = 2, Handicap = 0, Bonus = 0,
                Game1 = 230, Game2 = 230, Game3 = 230, Game4 = 200, HandicapTotal = 890
            };

            var tournament = new Tournament { ThreeOutOf4 = true };

            Dictionary<GameViewModel, int> result =
                TournamentCalculations.CalculatePlaceStandings([xavier, yolanda], tournament);

            Assert.AreEqual(1, result[xavier], "Best-3 total (750) should place first.");
            Assert.AreEqual(2, result[yolanda], "Best-3 total (690) should place second.");
        }

        [TestMethod]
        public void CalculatePlaceStandings_GameViewModel_ThreeOfFour_RestoresFullHandicapTotal()
        {
            // The ranking pass temporarily drops each bowler's lowest game; afterward the full
            // four-game HandicapTotal must be intact for downstream reporting.
            var bowler = new GameViewModel
            {
                MemberNumber = 1, MemberId = 1, Handicap = 0, Bonus = 0,
                Game1 = 250, Game2 = 250, Game3 = 250, Game4 = 100, HandicapTotal = 850
            };

            var tournament = new Tournament { ThreeOutOf4 = true };

            TournamentCalculations.CalculatePlaceStandings([bowler], tournament);

            Assert.AreEqual(850, bowler.HandicapTotal, "The dropped game must be added back after ranking.");
        }

        // ------------------------------------------------------------------
        // Series-score computation (extracted from the standings queries).
        // Scratch = games only; handicap = games + (hdcp + bonus) per game;
        // 3-of-4 drops the lowest game (and its handicap/bonus when included).
        // ------------------------------------------------------------------

        [TestMethod]
        public void ComputeSeriesScore_Scratch_SumsGamesOnly()
        {
            int score = TournamentCalculations.ComputeSeriesScore(
                [200, 180, 190, 210], handicap: 30, bonus: 5,
                includeHandicap: false, isThreeOfFourTournament: false);

            Assert.AreEqual(780, score, "Scratch totals exclude handicap and bonus.");
        }

        [TestMethod]
        public void ComputeSeriesScore_Scratch_ThreeOfFour_DropsLowestGame()
        {
            int score = TournamentCalculations.ComputeSeriesScore(
                [200, 180, 190, 100], handicap: 30, bonus: 5,
                includeHandicap: false, isThreeOfFourTournament: true);

            Assert.AreEqual(570, score, "Lowest game (100) is dropped; no handicap/bonus added.");
        }

        [TestMethod]
        public void ComputeSeriesScore_Handicap_AddsHandicapAndBonusPerGame()
        {
            int score = TournamentCalculations.ComputeSeriesScore(
                [200, 180, 190, 210], handicap: 30, bonus: 5,
                includeHandicap: true, isThreeOfFourTournament: false);

            // 780 + 4 * (30 + 5)
            Assert.AreEqual(920, score);
        }

        [TestMethod]
        public void ComputeSeriesScore_Handicap_ThreeOfFour_DropsLowestGameAndItsHandicapBonus()
        {
            int score = TournamentCalculations.ComputeSeriesScore(
                [200, 180, 190, 100], handicap: 30, bonus: 5,
                includeHandicap: true, isThreeOfFourTournament: true);

            // best-3 (570) + 3 * (30 + 5) = 675
            Assert.AreEqual(675, score);
        }

        [TestMethod]
        public void ComputeSeriesScore_IgnoresUnbowledGamesWithoutCrashing()
        {
            int score = TournamentCalculations.ComputeSeriesScore(
                [200, null, 190, null], handicap: 0, bonus: 0,
                includeHandicap: false, isThreeOfFourTournament: false);

            Assert.AreEqual(390, score, "Null (unbowled) games are ignored, not treated as zero-and-crash.");
        }

        [TestMethod]
        public void ComputeSeriesScore_ThreeOfFour_WithOnlyThreeGamesBowled_DoesNotDropAGame()
        {
            int score = TournamentCalculations.ComputeSeriesScore(
                [200, 180, 190, null], handicap: 0, bonus: 0,
                includeHandicap: false, isThreeOfFourTournament: true);

            Assert.AreEqual(570, score, "Nothing is dropped unless all four games were bowled.");
        }

        // ------------------------------------------------------------------
        // Bonus-pin deduction ladder for bowlers who cashed.
        // ------------------------------------------------------------------

        [TestMethod]
        [DataRow(1, 5, 0)]    // 1st place removes all bonus pins
        [DataRow(2, 5, 2)]    // 2nd-5th removes 3
        [DataRow(5, 5, 2)]
        [DataRow(6, 5, 3)]    // 6th-10th removes 2
        [DataRow(10, 5, 3)]
        [DataRow(11, 5, 4)]   // 11th+ removes 1
        [DataRow(2, 2, 0)]    // never goes below zero
        public void DeductFromBonusPins_ReturnsExpectedForPlacement(int placement, int currentBonus, int expected)
        {
            Assert.AreEqual(expected, TournamentCalculations.DeductFromBonusPins(placement, currentBonus));
        }
    }
}
