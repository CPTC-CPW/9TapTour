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

        /// <summary>
        /// The standard test entries plus a third entry for member 2 that won
        /// no prize money, only a side pot.
        /// </summary>
        private static List<ReportGameEntry> GetCashoutTestEntries()
        {
            List<ReportGameEntry> entries = GetTestEntries();
            entries.Add(new ReportGameEntry
            {
                MemberNumber = 2, FirstName = "Bob", LastName = "Jones",
                TournamentId = 101, TournamentDate = new DateTime(2025, 4, 1), Location = "Lanes B",
                GameScores = [150, 150, 150], ScratchSeries = 450, HandicapSeries = 470,
                GamesPlayed = 3, MoneyWon = 0m, SidePot = 10m, PlaceStanding = 12
            });
            return entries;
        }

        [TestMethod]
        public void GetCashoutReport_CountsEntriesAndCashesPerMember()
        {
            List<CashoutRow> rows = ReportCalculations.GetCashoutReport(GetCashoutTestEntries());

            // Member 1 cashed in both entries; member 2 cashed in one of two
            Assert.HasCount(2, rows);
            Assert.AreEqual(1, rows[0].Member);
            Assert.AreEqual(1, rows[0].Rank);
            Assert.AreEqual(2, rows[0].Entries);
            Assert.AreEqual(2, rows[0].Cashes);
            Assert.AreEqual(100.0, rows[0].CashPercent);
            Assert.AreEqual(125m, rows[0].Earnings);

            Assert.AreEqual(2, rows[1].Member);
            Assert.AreEqual(2, rows[1].Rank);
            Assert.AreEqual(2, rows[1].Entries);
            Assert.AreEqual(1, rows[1].Cashes);
            Assert.AreEqual(50.0, rows[1].CashPercent);
            Assert.AreEqual(50m, rows[1].Earnings);
        }

        [TestMethod]
        public void GetCashoutReport_SidePotsCountAsCashesWhenIncluded()
        {
            List<CashoutRow> rows = ReportCalculations.GetCashoutReport(GetCashoutTestEntries(), includeSidePots: true);

            CashoutRow bob = rows.Single(r => r.Member == 2);
            Assert.AreEqual(2, bob.Cashes);
            Assert.AreEqual(100.0, bob.CashPercent);
            Assert.AreEqual(60m, bob.Earnings);

            // Both members now cash 100%, so they share the top rank
            Assert.IsTrue(rows.All(r => r.Rank == 1));
        }

        [TestMethod]
        public void GetCashoutReport_LimitsToTopN()
        {
            List<CashoutRow> rows = ReportCalculations.GetCashoutReport(GetCashoutTestEntries(), 1);

            Assert.HasCount(1, rows);
            Assert.AreEqual(1, rows[0].Member);
        }

        [TestMethod]
        public void GetInactiveCutoff_IsSixMonthsBeforeToday()
        {
            DateTime cutoff = ReportCalculations.GetInactiveCutoff(new DateTime(2026, 10, 23, 14, 30, 0));

            Assert.AreEqual(new DateTime(2026, 4, 23), cutoff);
        }

        private static Member NewInactiveTestMember(int id, string lastName, bool isActive = true,
            DateTime? joinDate = null, DateTime? rejoinDate = null)
        {
            return new Member
            {
                Id = id, Number = id + 100, FirstName = "Test", LastName = lastName, IsActive = isActive,
                JoinDate = joinDate, RejoinDate = rejoinDate, PrimaryPhone = "555-0100", Email = "test@example.com"
            };
        }

        [TestMethod]
        public void GetInactiveMembers_ListsActiveMembersNotBowledSinceCutoff()
        {
            DateTime cutoff = new(2026, 4, 23);
            List<Member> members =
            [
                NewInactiveTestMember(1, "Recent"),
                NewInactiveTestMember(2, "OnCutoff"),
                NewInactiveTestMember(3, "Stale"),
                NewInactiveTestMember(4, "Never"),
                NewInactiveTestMember(5, "AlreadyInactive", isActive: false),
                NewInactiveTestMember(6, "NewMember", joinDate: new DateTime(2026, 9, 1)),
                NewInactiveTestMember(7, "Rejoined", rejoinDate: new DateTime(2026, 6, 1)),
                NewInactiveTestMember(8, "OldJoin", joinDate: new DateTime(2020, 1, 1)),
            ];
            Dictionary<int, DateTime> lastEntryDates = new()
            {
                { 1, new DateTime(2026, 10, 1) },
                { 2, cutoff },
                { 3, new DateTime(2025, 12, 6) },
                { 5, new DateTime(2024, 1, 1) },
                { 7, new DateTime(2025, 1, 1) },
            };

            List<InactiveMemberRow> rows = ReportCalculations.GetInactiveMembers(members, lastEntryDates, cutoff);

            // Never-bowled members first (by name), then oldest last bowled
            CollectionAssert.AreEqual(new[] { "Never", "OldJoin", "Stale" },
                rows.Select(r => r.Name.Split(',')[0]).ToArray());
            Assert.IsNull(rows[0].LastBowled);
            Assert.AreEqual(new DateTime(2025, 12, 6), rows[2].LastBowled);
            Assert.AreEqual(3, rows[2].MemberId);
            Assert.AreEqual(103, rows[2].Member);
            Assert.AreEqual("555-0100", rows[2].Phone);
        }
    }
}
