using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Models;
using NineTapTour.Core.Services;
using System;
using System.Collections.Generic;

namespace NineTapTourTests.Services
{
    /// <summary>
    /// Characterization tests for the pure member statistics logic extracted from
    /// FrmStats (M7.5). Expectations were hand-computed from the original form code
    /// (populateStats, tableview and FrmStats_Load) and must not change.
    /// </summary>
    [TestClass]
    public class StatsServiceTests
    {
        // Entry A: 150/200/180/170, handicap 10, bonus 2
        //   ScratchTotal = 700, GameTotal = 700 + 4 * (10 + 2) = 748, AvgPerGame = 700 / 4 = 175
        // Entry B: 210/-/190/-, handicap 20, bonus 0
        //   ScratchTotal = 400, GameTotal = 400 + 4 * (20 + 0) = 480 (handicap + bonus is
        //   added for all 4 game slots, played or not), AvgPerGame = 400 / 2 = 200
        private static List<StatGameEntry> MakeStatEntries()
        {
            return
            [
                new StatGameEntry(150, 200, 180, 170, 10, 2),
                new StatGameEntry(210, null, 190, null, 20, 0)
            ];
        }

        [TestMethod]
        public void ComputeMemberStatAverages_TwoEntries_AveragesEveryFieldOverAllEntries()
        {
            MemberStatAverages result = StatsService.ComputeMemberStatAverages(MakeStatEntries());

            // Null games count as 0 but the entry still counts toward the divisor
            Assert.AreEqual(180.0, result.Game1Average);   // (150 + 210) / 2
            Assert.AreEqual(100.0, result.Game2Average);   // (200 + 0) / 2
            Assert.AreEqual(185.0, result.Game3Average);   // (180 + 190) / 2
            Assert.AreEqual(85.0, result.Game4Average);    // (170 + 0) / 2
            Assert.AreEqual(550.0, result.ScratchTotalAverage); // (700 + 400) / 2
            Assert.AreEqual(614.0, result.GameTotalAverage);    // (748 + 480) / 2
            Assert.AreEqual(187.5, result.AveragePerGame);      // (175 + 200) / 2
            Assert.AreEqual(15.0, result.HandicapAverage);      // (10 + 20) / 2
            Assert.AreEqual(1.0, result.BonusAverage);          // (2 + 0) / 2
        }

        [TestMethod]
        public void ComputeMemberStatAverages_NullHandicap_NullsOutGameTotalForThatEntry()
        {
            // Null handicap propagates through (Handicap + Bonus), making GameTotal null,
            // which the averaging loop converts to 0 — original form behavior.
            List<StatGameEntry> entries = [new StatGameEntry(100, 100, 100, 100, null, 5)];

            MemberStatAverages result = StatsService.ComputeMemberStatAverages(entries);

            Assert.AreEqual(400.0, result.ScratchTotalAverage);
            Assert.AreEqual(0.0, result.GameTotalAverage);
            Assert.AreEqual(100.0, result.AveragePerGame);
            Assert.AreEqual(0.0, result.HandicapAverage);
            Assert.AreEqual(5.0, result.BonusAverage);
        }

        [TestMethod]
        public void ComputeMemberStatAverages_NoEntries_ReturnsNaNAverages()
        {
            // 0 / 0 double division — the original form displayed "NaN" for members
            // with no tournament entries
            MemberStatAverages result = StatsService.ComputeMemberStatAverages([]);

            Assert.IsTrue(double.IsNaN(result.Game1Average));
            Assert.IsTrue(double.IsNaN(result.ScratchTotalAverage));
            Assert.IsTrue(double.IsNaN(result.GameTotalAverage));
            Assert.IsTrue(double.IsNaN(result.AveragePerGame));
            Assert.IsTrue(double.IsNaN(result.BonusAverage));
        }

        [TestMethod]
        public void ComputeLast30Averages_TwoEntries_AveragesGamesAndSumsScratch()
        {
            List<PlayerHistoryViewModel> last30 =
            [
                new PlayerHistoryViewModel { Game1 = 150, Game2 = 200, Game3 = 180, Game4 = 170, HandiCap = 10, Bonus = 2 },
                new PlayerHistoryViewModel { Game1 = 210, Game2 = null, Game3 = 190, Game4 = null, HandiCap = 20, Bonus = 5 }
            ];

            Last30Averages result = StatsService.ComputeLast30Averages(last30);

            Assert.AreEqual(2, result.EntryCount);
            Assert.AreEqual(180, result.Game1Average);  // (150 + 210) / 2
            Assert.AreEqual(100, result.Game2Average);  // (200 + 0) / 2
            Assert.AreEqual(185, result.Game3Average);  // (180 + 190) / 2
            Assert.AreEqual(85, result.Game4Average);   // (170 + 0) / 2
            Assert.AreEqual(1100, result.ScratchTotal); // 700 + 400

            // Characterizes two preserved quirks of the original FrmStats_Load code:
            // gameTotal is overwritten each iteration (only the last entry survives), and
            // the ?? precedence means handicap/bonus never contribute for played games —
            // so this is 400, not 450 (400 + 2 * (20 + 5)) and not a running total.
            Assert.AreEqual(400, result.GameTotal);
        }

        [TestMethod]
        public void ComputeLast30Averages_NoEntries_ReturnsZeroCountWithoutDividing()
        {
            Last30Averages result = StatsService.ComputeLast30Averages([]);

            Assert.AreEqual(0, result.EntryCount);
            Assert.AreEqual(0, result.Game1Average);
            Assert.AreEqual(0, result.ScratchTotal);
            Assert.AreEqual(0, result.GameTotal);
        }

        [TestMethod]
        public void ShapeMemberStatsRows_ZeroScoresAndZeroAdjustedAvg_BecomeNullCells()
        {
            DateTime date = new(2026, 3, 14);
            List<FinalizedGameEntry> games =
            [
                new FinalizedGameEntry(7, 2, date, 180, 0, 200, 0, 380, 410, 175.5, 0, 12, 3, 25m, null, "note")
            ];

            List<MemberStatsRow> rows = StatsService.ShapeMemberStatsRows(games);

            Assert.HasCount(1, rows);
            MemberStatsRow row = rows[0];
            Assert.AreEqual(180, row.Game1);
            Assert.IsNull(row.Game2);
            Assert.AreEqual(200, row.Game3);
            Assert.IsNull(row.Game4);
            Assert.IsNull(row.AdjustedAvg);   // 0 displays as an empty cell
            Assert.AreEqual("", row.Place);   // Null place standing displays as empty text
            Assert.AreEqual(380, row.ScratchTotal);
            Assert.AreEqual(410, row.HandicapTotal);
            Assert.AreEqual(175.5, row.LeagueAverage);
            Assert.AreEqual(25m, row.MoneyWon);
            Assert.AreEqual("note", row.Notes);
            Assert.AreEqual(7, row.GameId);
        }

        [TestMethod]
        public void ShapeMemberStatsRows_EntryWithNoGamesPlayed_IsDropped()
        {
            DateTime date = new(2026, 3, 14);
            List<FinalizedGameEntry> games =
            [
                new FinalizedGameEntry(1, 0, date, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0m, null, null),
                new FinalizedGameEntry(2, 4, date, 150, 200, 180, 170, 700, 748, 172.3, 170, 12, 3, 0m, 3, "")
            ];

            List<MemberStatsRow> rows = StatsService.ShapeMemberStatsRows(games);

            Assert.HasCount(1, rows);
            Assert.AreEqual(2, rows[0].GameId);
            Assert.AreEqual(170, rows[0].AdjustedAvg);
            Assert.AreEqual("3", rows[0].Place); // Numeric place standing displays as text
        }
    }
}
