using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Calculations;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTourTests
{
    [TestClass]
    public class ReportCalculationsTests
    {
        /// <summary>
        /// Builds a set of entries for two members across two tournaments in
        /// different years, with known series, games, earnings, and placings.
        /// </summary>
        private static List<ReportGameEntry> GetTestEntries()
        {
            return
            [
                // Member 1: two entries
                new ReportGameEntry
                {
                    MemberNumber = 1, FirstName = "Jane", LastName = "Smith",
                    TournamentId = 100, TournamentDate = new DateTime(2024, 3, 1), Location = "Lanes A",
                    GameScores = [200, 210, 190], ScratchSeries = 600, HandicapSeries = 660,
                    GamesPlayed = 3, MoneyWon = 100m, SidePot = 20m, PlaceStanding = 1
                },
                new ReportGameEntry
                {
                    MemberNumber = 1, FirstName = "Jane", LastName = "Smith",
                    TournamentId = 101, TournamentDate = new DateTime(2025, 4, 1), Location = "Lanes B",
                    GameScores = [150, 160, 170], ScratchSeries = 480, HandicapSeries = 540,
                    GamesPlayed = 3, MoneyWon = 25m, SidePot = 5m, PlaceStanding = 7
                },
                // Member 2: one entry
                new ReportGameEntry
                {
                    MemberNumber = 2, FirstName = "Bob", LastName = "Jones",
                    TournamentId = 100, TournamentDate = new DateTime(2024, 3, 1), Location = "Lanes A",
                    GameScores = [220, 180, 200], ScratchSeries = 600, HandicapSeries = 620,
                    GamesPlayed = 3, MoneyWon = 50m, PlaceStanding = 2
                },
            ];
        }

        [TestMethod]
        public void GetHighSeries_RanksTiesWithSameRank()
        {
            List<HighSeriesRow> rows = ReportCalculations.GetHighSeries(GetTestEntries());

            // Members 1 and 2 both shot a 600 series; the third entry is 480
            Assert.HasCount(3, rows);
            Assert.AreEqual(1, rows[0].Rank);
            Assert.AreEqual(1, rows[1].Rank);
            Assert.AreEqual(600, rows[0].Series);
            Assert.AreEqual(600, rows[1].Series);
            Assert.AreEqual(3, rows[2].Rank);
            Assert.AreEqual(480, rows[2].Series);
        }

        [TestMethod]
        public void GetHighSeries_LimitsToTopN()
        {
            List<HighSeriesRow> rows = ReportCalculations.GetHighSeries(GetTestEntries(), 2);

            Assert.HasCount(2, rows);
        }

        [TestMethod]
        public void GetHighGames_UsesEveryGameScore()
        {
            List<HighGameRow> rows = ReportCalculations.GetHighGames(GetTestEntries());

            // 3 entries x 3 games each = 9 candidate games
            Assert.HasCount(9, rows);
            Assert.AreEqual(220, rows[0].Game);
            Assert.AreEqual(2, rows[0].Member);
            Assert.AreEqual(1, rows[0].Rank);
        }

        [TestMethod]
        public void BuildMemberSummaries_AggregatesPerMember()
        {
            List<MemberReportSummary> summaries = ReportCalculations.BuildMemberSummaries(GetTestEntries());

            Assert.HasCount(2, summaries);

            MemberReportSummary member1 = summaries.Single(s => s.Member == 1);
            Assert.AreEqual(2, member1.Entries);
            Assert.AreEqual(2, member1.Tournaments);
            Assert.AreEqual(125m, member1.Earnings);
            Assert.AreEqual(600, member1.HighSeries);
            Assert.AreEqual(210, member1.HighGame);
            // (600 + 480) pins over 6 games = 180 average
            Assert.AreEqual(180, member1.Average);
            Assert.AreEqual(1, member1.FirstPlace);
            Assert.AreEqual(0, member1.SecondPlace);
            Assert.AreEqual(1, member1.Top5);
            // Places 1 and 7 both count for Top 10
            Assert.AreEqual(2, member1.Top10);

            MemberReportSummary member2 = summaries.Single(s => s.Member == 2);
            Assert.AreEqual(1, member2.Entries);
            Assert.AreEqual(0, member2.FirstPlace);
            Assert.AreEqual(1, member2.SecondPlace);
            Assert.AreEqual(1, member2.Top5);
        }

        [TestMethod]
        public void BuildMemberSummaries_ExcludesSidePotsByDefault()
        {
            List<MemberReportSummary> summaries = ReportCalculations.BuildMemberSummaries(GetTestEntries());

            // Member 1 won $125 plus $25 in side pots; side pots are excluded by default
            Assert.AreEqual(125m, summaries.Single(s => s.Member == 1).Earnings);
        }

        [TestMethod]
        public void BuildMemberSummaries_AddsSidePotsWhenRequested()
        {
            List<MemberReportSummary> summaries = ReportCalculations.BuildMemberSummaries(GetTestEntries(), includeSidePots: true);

            Assert.AreEqual(150m, summaries.Single(s => s.Member == 1).Earnings);
            // Member 2 has no side pot winnings, so earnings are unchanged
            Assert.AreEqual(50m, summaries.Single(s => s.Member == 2).Earnings);
        }

        [TestMethod]
        public void BuildIndividualSummary_LabelsEarningsWithSidePots()
        {
            List<ReportGameEntry> memberEntries = GetTestEntries().Where(e => e.MemberNumber == 1).ToList();

            List<ReportStatistic> stats = ReportCalculations.BuildIndividualSummary(memberEntries, includeSidePots: true);

            ReportStatistic earnings = stats.Single(s => s.Statistic == "Total Earnings (incl. Side Pots)");
            Assert.AreEqual(150m.ToString("C2"), earnings.Value);
        }

        [TestMethod]
        public void BuildMemberSummaries_IgnoresNullPlaceStandingsForPlacementCounts()
        {
            List<ReportGameEntry> entries =
            [
                new ReportGameEntry
                {
                    MemberNumber = 3, FirstName = "Pat", LastName = "Lee",
                    TournamentId = 100, TournamentDate = new DateTime(2024, 3, 1), Location = "Lanes A",
                    GameScores = [100], ScratchSeries = 100, HandicapSeries = 100,
                    GamesPlayed = 1, MoneyWon = 0m, PlaceStanding = null
                },
            ];

            MemberReportSummary summary = ReportCalculations.BuildMemberSummaries(entries).Single();

            Assert.AreEqual(0, summary.FirstPlace);
            Assert.AreEqual(0, summary.Top5);
            Assert.AreEqual(0, summary.Top10);
        }

        [TestMethod]
        public void GetTourReport_SortsByCategoryValue()
        {
            List<MemberReportSummary> byEarnings = ReportCalculations.GetTourReport(GetTestEntries(), TourReportCategory.Earnings);

            Assert.AreEqual(1, byEarnings[0].Member); // $125 beats $50
            Assert.AreEqual(1, byEarnings[0].Rank);
            Assert.AreEqual(2, byEarnings[1].Member);
            Assert.AreEqual(2, byEarnings[1].Rank);

            List<MemberReportSummary> byEntries = ReportCalculations.GetTourReport(GetTestEntries(), TourReportCategory.TotalEntries);

            Assert.AreEqual(1, byEntries[0].Member); // 2 entries beats 1
            Assert.AreEqual(2, byEntries[0].Entries);

            List<MemberReportSummary> byAverage = ReportCalculations.GetTourReport(GetTestEntries(), TourReportCategory.HighAverages);

            Assert.AreEqual(2, byAverage[0].Member); // 200.00 beats 180.00
            Assert.AreEqual(200, byAverage[0].Average);
        }

        [TestMethod]
        public void GetTourReport_FirstPlaceFinishes()
        {
            List<MemberReportSummary> rows = ReportCalculations.GetTourReport(GetTestEntries(), TourReportCategory.FirstPlaceFinishes);

            Assert.AreEqual(1, rows[0].Member);
            Assert.AreEqual(1, rows[0].FirstPlace);
        }

        [TestMethod]
        public void BuildIndividualSummary_ReturnsStatsForMember()
        {
            List<ReportGameEntry> memberEntries = GetTestEntries().Where(e => e.MemberNumber == 1).ToList();

            List<ReportStatistic> stats = ReportCalculations.BuildIndividualSummary(memberEntries);

            Assert.AreNotEqual(0, stats.Count);
            Assert.AreEqual("2", stats.Single(s => s.Statistic == "Total Entries").Value);
            Assert.AreEqual("1", stats.Single(s => s.Statistic == "1st Place Finishes").Value);
            StringAssert.StartsWith(stats.Single(s => s.Statistic == "High Series (Scratch)").Value, "600");
            StringAssert.StartsWith(stats.Single(s => s.Statistic == "High Game (Scratch)").Value, "210");
        }

        [TestMethod]
        public void BuildIndividualSummary_EmptyEntriesReturnsEmptyList()
        {
            List<ReportStatistic> stats = ReportCalculations.BuildIndividualSummary([]);

            Assert.IsEmpty(stats);
        }
    }
}
