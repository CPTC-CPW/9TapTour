using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Models;
using NineTapTour.Core.Services;
using System.Collections.Generic;

namespace NineTapTourTests.Services
{
    /// <summary>
    /// Characterization tests for the finalize-tournament calculation logic extracted
    /// from FrmFinalizeTournament (M7.1). Expected values were computed by hand from
    /// the original form code before extraction — do not change expectations.
    /// </summary>
    [TestClass]
    public class FinalizeCalculationServiceTests
    {
        private readonly FinalizeCalculationService service = new();

        // --- RecalculateRow (formerly the arithmetic in RecalculateTournamentRow) ---

        [TestMethod]
        public void RecalculateRow_AllFourGamesChecked_SumsAllScores()
        {
            // scratch 780, games 4, hdcpTotal 780 + 4*(20+3) = 872, entryAvg 780/4 = 195
            FinalizeRowResult result = service.RecalculateRow(new FinalizeRowInput(
                200, 180, 190, 210, true, true, true, true,
                Handicap: 20, AdjustedAvg: 195, BaseBonus: 3));

            Assert.AreEqual(780, result.ScratchTotal);
            Assert.AreEqual(4, result.CheckedGames);
            Assert.AreEqual(872, result.HdcpTotal);
            Assert.AreEqual(195, result.EntryAvg);
            Assert.AreEqual(20, result.ResolvedHandicap);
            Assert.IsFalse(result.HandicapWasDerived);
        }

        [TestMethod]
        public void RecalculateRow_UncheckedGameExcluded_ThreeOfFour()
        {
            // Game 2 unchecked: scratch 600, games 3, hdcpTotal 600 + 3*23 = 669, entryAvg 200
            FinalizeRowResult result = service.RecalculateRow(new FinalizeRowInput(
                200, 180, 190, 210, true, false, true, true,
                Handicap: 20, AdjustedAvg: 195, BaseBonus: 3));

            Assert.AreEqual(600, result.ScratchTotal);
            Assert.AreEqual(3, result.CheckedGames);
            Assert.AreEqual(669, result.HdcpTotal);
            Assert.AreEqual(200, result.EntryAvg);
        }

        [TestMethod]
        public void RecalculateRow_MissingHandicap_DerivedFromAdjustedAvg()
        {
            // hdcp 0, adjAvg 180 -> (220-180)*90/100 = 36; scratch 310, games 2,
            // hdcpTotal 310 + 2*(36+5) = 392, entryAvg 155
            FinalizeRowResult result = service.RecalculateRow(new FinalizeRowInput(
                150, 160, null, null, true, true, false, false,
                Handicap: 0, AdjustedAvg: 180, BaseBonus: 5));

            Assert.AreEqual(36, result.ResolvedHandicap);
            Assert.IsTrue(result.HandicapWasDerived);
            Assert.AreEqual(310, result.ScratchTotal);
            Assert.AreEqual(2, result.CheckedGames);
            Assert.AreEqual(392, result.HdcpTotal);
            Assert.AreEqual(155, result.EntryAvg);
        }

        [TestMethod]
        public void RecalculateRow_MissingHandicapAndAdjAvg_HandicapStaysZero()
        {
            // Checked game with a null score counts toward games but adds 0 pins:
            // scratch 170, games 2, entryAvg 85, hdcpTotal 170 + 2*(0+0) = 170
            FinalizeRowResult result = service.RecalculateRow(new FinalizeRowInput(
                null, 170, null, null, true, true, false, false,
                Handicap: 0, AdjustedAvg: 0, BaseBonus: 0));

            Assert.AreEqual(0, result.ResolvedHandicap);
            Assert.IsFalse(result.HandicapWasDerived);
            Assert.AreEqual(170, result.ScratchTotal);
            Assert.AreEqual(2, result.CheckedGames);
            Assert.AreEqual(170, result.HdcpTotal);
            Assert.AreEqual(85, result.EntryAvg);
        }

        [TestMethod]
        public void RecalculateRow_NoGamesChecked_AllTotalsZero()
        {
            FinalizeRowResult result = service.RecalculateRow(new FinalizeRowInput(
                200, 180, 190, 210, false, false, false, false,
                Handicap: 20, AdjustedAvg: 195, BaseBonus: 3));

            Assert.AreEqual(0, result.ScratchTotal);
            Assert.AreEqual(0, result.CheckedGames);
            Assert.AreEqual(0, result.HdcpTotal);
            Assert.AreEqual(0, result.EntryAvg);
        }

        [TestMethod]
        public void RecalculateRow_EntryAvgUsesIntegerDivision()
        {
            // scratch 605, games 3, entryAvg 605/3 = 201 (integer division),
            // hdcpTotal 605 + 3*(10+0) = 635
            FinalizeRowResult result = service.RecalculateRow(new FinalizeRowInput(
                200, 200, 205, null, true, true, true, false,
                Handicap: 10, AdjustedAvg: 0, BaseBonus: 0));

            Assert.AreEqual(605, result.ScratchTotal);
            Assert.AreEqual(201, result.EntryAvg);
            Assert.AreEqual(635, result.HdcpTotal);
        }

        [TestMethod]
        public void RecalculateRow_StoredHandicap_NeverOverwrittenByAdjAvg()
        {
            // hdcp 15 stays 15 even though adjAvg 100 would derive 70
            FinalizeRowResult result = service.RecalculateRow(new FinalizeRowInput(
                180, null, null, null, true, false, false, false,
                Handicap: 15, AdjustedAvg: 100, BaseBonus: 2));

            Assert.AreEqual(15, result.ResolvedHandicap);
            Assert.IsFalse(result.HandicapWasDerived);
            Assert.AreEqual(180 + 1 * (15 + 2), result.HdcpTotal);
        }

        // --- Doubles combined total (formerly the doubles branch of RecalculateTournamentRow) ---

        [TestMethod]
        public void ComputeCombinedHdcpTotal_SumsBothMembersWithPerGamePins()
        {
            // 310 + 350 + 2*(36+5) + 2*(20+1) = 660 + 82 + 42 = 784
            int combined = service.ComputeCombinedHdcpTotal(310, 2, 36, 5, 350, 2, 20, 1);
            Assert.AreEqual(784, combined);
        }

        // --- ComputeNewHdcpPreview (formerly UpdateNewHdcpPreview) ---

        [TestMethod]
        public void ComputeNewHdcpPreview_PositiveAvg_ReturnsHandicap()
        {
            // (220-200)*90/100 = 18
            Assert.AreEqual(18, service.ComputeNewHdcpPreview(200));
        }

        [TestMethod]
        public void ComputeNewHdcpPreview_ZeroAvg_ReturnsNull()
        {
            Assert.IsNull(service.ComputeNewHdcpPreview(0));
        }

        [TestMethod]
        public void ComputeNewHdcpPreview_LowAvg_CappedAtSeventy()
        {
            // (220-130)*90/100 = 81, capped at 70
            Assert.AreEqual(70, service.ComputeNewHdcpPreview(130));
        }

        // --- Compute30EntryAverage (formerly the math in UpdateAll30AvgForMember) ---

        [TestMethod]
        public void Compute30EntryAverage_CombinesHistoryAndCurrent()
        {
            // (5400+600) / (27+3) = 200.0
            Assert.AreEqual(200.0, service.Compute30EntryAverage(5400, 27, 600, 3));
        }

        [TestMethod]
        public void Compute30EntryAverage_RoundsToOneDecimal()
        {
            // 605/3 = 201.666... -> 201.7
            Assert.AreEqual(201.7, service.Compute30EntryAverage(0, 0, 605, 3));
        }

        [TestMethod]
        public void Compute30EntryAverage_NoGames_ReturnsZero()
        {
            Assert.AreEqual(0, service.Compute30EntryAverage(0, 0, 0, 0));
        }

        // --- Compute30EntryHistory (formerly the accumulation loops in LoadTournamentGrid) ---

        private static HistoryGameEntry FourGames(int score) =>
            new(score, score, score, score, null, null, null, null);

        [TestMethod]
        public void Compute30EntryHistory_TakesEntriesUpToWindowLimit()
        {
            var entries = new List<HistoryGameEntry> { FourGames(200), FourGames(200) };
            // currentEntryCount 28 -> limit 2 -> both taken: (1600, 8)
            Assert.AreEqual((1600, 8), service.Compute30EntryHistory(entries, 28));
            // currentEntryCount 29 -> limit 1 -> first only: (800, 4)
            Assert.AreEqual((800, 4), service.Compute30EntryHistory(entries, 29));
            // currentEntryCount 30 -> limit 0 -> nothing taken
            Assert.AreEqual((0, 0), service.Compute30EntryHistory(entries, 30));
        }

        [TestMethod]
        public void Compute30EntryHistory_UnusedGameExcluded()
        {
            // UseGame1 = false excludes game 1 from both scratch and game count
            var entries = new List<HistoryGameEntry>
            {
                new(200, 180, 190, 210, false, null, null, null)
            };
            Assert.AreEqual((580, 3), service.Compute30EntryHistory(entries, 0));
        }

        [TestMethod]
        public void Compute30EntryHistory_EmptyEntryDoesNotConsumeWindow()
        {
            var entries = new List<HistoryGameEntry>
            {
                new(null, null, null, null, null, null, null, null),
                FourGames(200)
            };
            // limit 1: the empty entry is skipped without consuming the window
            Assert.AreEqual((800, 4), service.Compute30EntryHistory(entries, 29));
        }

        // --- IsSandbaggingScore (formerly the predicate in ApplySandbaggingHighlight) ---

        [DataTestMethod]
        [DataRow(200.0, 160, true)]   // exactly 40 below average triggers
        [DataRow(200.0, 161, false)]  // 39 below does not
        [DataRow(200.0, 0, false)]    // zero score never triggers
        [DataRow(0.0, 100, false)]    // no league average -> never triggers
        [DataRow(200.5, 160, true)]   // fractional average: 40.5 below triggers
        public void IsSandbaggingScore_MatchesFormPredicate(double leagueAverage, int score, bool expected)
        {
            Assert.AreEqual(expected, service.IsSandbaggingScore(leagueAverage, score));
        }

        // --- IsRowValid (formerly ValidateRow) ---

        [DataTestMethod]
        [DataRow(true, 180, true)]
        [DataRow(false, 180, false)]
        [DataRow(true, 0, false)]
        [DataRow(false, 0, false)]
        public void IsRowValid_RequiresDirectorCheckAndNonZeroAdjAvg(bool directorChecked, int adjAvg, bool expected)
        {
            Assert.AreEqual(expected, service.IsRowValid(directorChecked, adjAvg));
        }

        // --- DetermineUseGameDefaults (formerly the checkbox defaulting in LoadTournamentGrid) ---

        [TestMethod]
        public void DetermineUseGameDefaults_ThreeOutOf4_UnchecksLowestOfFour()
        {
            UseGameFlags flags = service.DetermineUseGameDefaults(
                180, 170, 190, 200, null, null, null, null, threeOutOf4: true);
            Assert.AreEqual(new UseGameFlags(true, false, true, true), flags);
        }

        [TestMethod]
        public void DetermineUseGameDefaults_ThreeOutOf4_TieUnchecksFirstLowest()
        {
            UseGameFlags flags = service.DetermineUseGameDefaults(
                170, 170, 190, 200, null, null, null, null, threeOutOf4: true);
            Assert.AreEqual(new UseGameFlags(false, true, true, true), flags);
        }

        [TestMethod]
        public void DetermineUseGameDefaults_ThreeOutOf4_OnlyThreeGames_NoAutoUncheck()
        {
            UseGameFlags flags = service.DetermineUseGameDefaults(
                180, 170, 190, null, null, null, null, null, threeOutOf4: true);
            Assert.AreEqual(new UseGameFlags(true, true, true, false), flags);
        }

        [TestMethod]
        public void DetermineUseGameDefaults_SavedFlagDisablesAutoLogicAndOverrides()
        {
            // Any saved flag means "never saved" is false, so 3-of-4 auto-uncheck is skipped
            UseGameFlags flags = service.DetermineUseGameDefaults(
                180, 170, 190, 200, false, null, null, null, threeOutOf4: true);
            Assert.AreEqual(new UseGameFlags(false, true, true, true), flags);
        }

        [TestMethod]
        public void DetermineUseGameDefaults_StandardFormat_ChecksGamesWithScores()
        {
            UseGameFlags flags = service.DetermineUseGameDefaults(
                180, null, 190, 200, null, null, null, null, threeOutOf4: false);
            Assert.AreEqual(new UseGameFlags(true, false, true, true), flags);
        }

        // --- ComputePreviousHandicapAndBonus (carry-forward from prior tournament) ---

        [TestMethod]
        public void ComputePreviousHandicapAndBonus_NoCash_TakesMaxBonus()
        {
            var entries = new List<PreviousEntrySnapshot>
            {
                new(190, 2, 0),
                new(0, 4, 0)
            };
            // hdcp from first entry with avg > 0: (220-190)*90/100 = 27; no cash -> max bonus 4
            Assert.AreEqual((27, 4), service.ComputePreviousHandicapAndBonus(entries));
        }

        [TestMethod]
        public void ComputePreviousHandicapAndBonus_Cashed_TakesMinBonus()
        {
            var entries = new List<PreviousEntrySnapshot>
            {
                new(200, 3, 50),
                new(0, 1, 0)
            };
            // hdcp (220-200)*90/100 = 18; cashed -> min bonus 1
            Assert.AreEqual((18, 1), service.ComputePreviousHandicapAndBonus(entries));
        }

        [TestMethod]
        public void ComputePreviousHandicapAndBonus_NoAdjustedAvg_HandicapZero()
        {
            var entries = new List<PreviousEntrySnapshot> { new(0, 2, 0) };
            Assert.AreEqual((0, 2), service.ComputePreviousHandicapAndBonus(entries));
        }

        // --- ComputeBonusPreview (cash-line deduction + third-entry bonus preview) ---

        [DataTestMethod]
        // Cashing 1st place: all bonus pins removed
        [DataRow(5, 1, 3, false, 5, 1, 0, true, false)]
        // Cashing 3rd place (2nd-5th): 3 pins removed
        [DataRow(5, 3, 3, false, 0, 1, 2, true, false)]
        // Placed below the cash line: no deduction
        [DataRow(5, 4, 3, false, 0, 1, 5, false, false)]
        // Not placed at all: no deduction
        [DataRow(5, 0, 3, false, 0, 1, 5, false, false)]
        // Third total entry: +1 bonus pin
        [DataRow(4, 0, 3, false, 2, 1, 5, false, true)]
        // Third entry bump capped at 5 pins
        [DataRow(5, 0, 3, false, 2, 1, 5, false, true)]
        // 2-day championship: third-entry bump suppressed
        [DataRow(4, 0, 0, true, 2, 1, 4, false, false)]
        // Cashing suppresses the third-entry bump
        [DataRow(4, 1, 3, false, 2, 1, 0, true, false)]
        public void ComputeBonusPreview_MatchesFormBehavior(
            int baseBonus, int placing, int cashLine, bool isTwoDay, int histCount, int currCount,
            int expectedBonus, bool expectedCashing, bool expectedThirdEntry)
        {
            BonusPreviewResult result = service.ComputeBonusPreview(
                baseBonus, placing, cashLine, isTwoDay, histCount, currCount);

            Assert.AreEqual(expectedBonus, result.DisplayBonus);
            Assert.AreEqual(expectedCashing, result.IsCashing);
            Assert.AreEqual(expectedThirdEntry, result.AwardedThirdEntryBonus);
        }

        // --- ResolveDisplayHandicap (fallback chain in LoadTournamentGrid) ---

        [DataTestMethod]
        [DataRow(25, 30, 180, 25)]    // previous tournament handicap wins
        [DataRow(null, 30, 180, 30)]  // no previous -> stored game handicap
        [DataRow(0, 30, 180, 30)]     // previous exists but zero -> stored game handicap
        [DataRow(null, 0, 180, 36)]   // nothing stored -> derived from adjAvg
        [DataRow(null, 0, 0, 0)]      // nothing available -> zero
        public void ResolveDisplayHandicap_UsesFallbackChain(int? previous, int stored, int adjAvg, int expected)
        {
            Assert.AreEqual(expected, service.ResolveDisplayHandicap(previous, stored, adjAvg));
        }

        // --- ComputeEntryTotalScore (formerly BuildExcelMemberList arithmetic) ---

        [TestMethod]
        public void ComputeEntryTotalScore_ThreeOutOf4_DropsLowestOfFour()
        {
            // Drop 170: 180+190+200 = 570, + 3*(20+3) = 639
            Assert.AreEqual(639, service.ComputeEntryTotalScore(180, 170, 190, 200, 20, 3, threeOutOf4: true));
        }

        [TestMethod]
        public void ComputeEntryTotalScore_ThreeOutOf4_ThreeGames_NothingDropped()
        {
            // 150+190+200 = 540, + 3*(20+3) = 609
            Assert.AreEqual(609, service.ComputeEntryTotalScore(150, null, 190, 200, 20, 3, threeOutOf4: true));
        }

        [TestMethod]
        public void ComputeEntryTotalScore_StandardFormat_UsesAllGames()
        {
            // 740 + 4*(20+3) = 832
            Assert.AreEqual(832, service.ComputeEntryTotalScore(180, 170, 190, 200, 20, 3, threeOutOf4: false));
        }

        [TestMethod]
        public void ComputeEntryTotalScore_StandardFormat_SingleGame()
        {
            // 150 + 1*(20+3) = 173
            Assert.AreEqual(173, service.ComputeEntryTotalScore(150, null, null, null, 20, 3, threeOutOf4: false));
        }

        // --- AssignTeamPlaces (doubles tie detection) ---

        [TestMethod]
        public void AssignTeamPlaces_TiedTotalsSharePlace()
        {
            CollectionAssert.AreEqual(
                new[] { 1, 1, 3, 3, 5 },
                service.AssignTeamPlaces(new[] { 900, 900, 850, 850, 800 }));
        }

        [TestMethod]
        public void AssignTeamPlaces_SingleTeam()
        {
            CollectionAssert.AreEqual(new[] { 1 }, service.AssignTeamPlaces(new[] { 700 }));
        }

        [TestMethod]
        public void AssignTeamPlaces_Empty()
        {
            CollectionAssert.AreEqual(new int[0], service.AssignTeamPlaces(new int[0]));
        }

        // --- ResolvePersistedBonus (pure part of PersistRowToDatabase / FinalizeAllGames) ---

        [DataTestMethod]
        [DataRow(true, 4, 2, 4)]      // preserve: original wins
        [DataRow(false, 4, 2, 2)]     // not preserving: edited cell value wins
        public void ResolvePersistedBonus_PreservesOriginalForCashingRows(
            bool preserve, int original, int edited, int expected)
        {
            Assert.AreEqual(expected, service.ResolvePersistedBonus(preserve, original, edited));
        }

        [TestMethod]
        public void ResolvePersistedBonus_PreserveWithUnknownOriginal_FallsBackToEdited()
        {
            Assert.AreEqual(2, service.ResolvePersistedBonus(true, null, 2));
        }
    }
}
