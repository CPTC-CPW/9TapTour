using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Printing;
using System;
using System.Collections.Generic;
using System.Linq;
using static NineTapTour.Core.Calculations.ReportHelper;

namespace NineTapTourTests.Printing
{
    /// <summary>
    /// Characterization tests for the print content logic extracted from
    /// NineTapTour.Database.Print (M8). Expectations were hand-computed from
    /// the original GDI+ print code and must not change.
    /// </summary>
    [TestClass]
    public class PrintContentBuilderPagingTests
    {
        [TestMethod]
        public void BuildMemberReport_ExactlyOnePageOfBowlers_ProducesSinglePage()
        {
            MemberReportContent content = BuildReport(MakeMembers(40));

            Assert.AreEqual(1, content.Pages.Count);
            Assert.AreEqual(40, content.Pages[0].Rows.Count);
        }

        [TestMethod]
        public void BuildMemberReport_OneOverPageSize_ProducesSecondPageWithOneRow()
        {
            MemberReportContent content = BuildReport(MakeMembers(41));

            Assert.AreEqual(2, content.Pages.Count);
            Assert.AreEqual(40, content.Pages[0].Rows.Count);
            Assert.AreEqual(1, content.Pages[1].Rows.Count);
            // The 41st member lands as the first row of the second page
            Assert.AreEqual("41", content.Pages[1].Rows[0].Placing);
        }

        [TestMethod]
        public void BuildMemberReport_EmptyList_ProducesNoPages()
        {
            MemberReportContent content = BuildReport([]);

            Assert.AreEqual(0, content.Pages.Count);
        }

        [TestMethod]
        public void BuildMemberReport_RowsKeepListOrderAcrossPages()
        {
            MemberReportContent content = BuildReport(MakeMembers(90));

            Assert.AreEqual(3, content.Pages.Count);
            List<string> placings = [.. content.Pages.SelectMany(p => p.Rows).Select(r => r.Placing)];
            CollectionAssert.AreEqual(Enumerable.Range(1, 90).Select(n => n.ToString()).ToList(), placings);
        }

        internal static MemberReportContent BuildReport(List<MemberScores> members, ReportType reportType = ReportType.HighGame, int currentSquad = 0, List<int> squadList = null, bool printDues = false, int? manualCutoff = null)
        {
            Tournament tournament = new()
            {
                Location = "Test Lanes",
                Date = new DateTime(2019, 5, 13),
                ThreeOutOf4 = false
            };
            return PrintContentBuilder.BuildMemberReport(members, tournament, reportType, currentSquad, squadList ?? [0], printDues, manualCutoff);
        }

        internal static List<MemberScores> MakeMembers(int count)
        {
            List<MemberScores> members = [];
            for (int i = 1; i <= count; i++)
            {
                members.Add(new MemberScores
                {
                    placing = i,
                    Score = 700 + i,
                    MemberId = 1000 + i,
                    FirstName = "First" + i,
                    LastName = "Last" + i,
                    LastPaymentYear = "2023"
                });
            }
            return members;
        }
    }

    [TestClass]
    public class PrintContentBuilderCutoffTests
    {
        [TestMethod]
        public void BuildMemberReport_FewerThanFiveMembers_NoCutoffLine()
        {
            // winningPlaces defaults to 5 for short lists; a 3-row report never
            // reaches row index 4, so no line is drawn
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport(PrintContentBuilderPagingTests.MakeMembers(3));

            Assert.IsNull(content.Pages[0].CutoffAfterRowIndex);
        }

        [TestMethod]
        public void BuildMemberReport_FiveMembers_CutoffAfterFirstRow()
        {
            // winningPlaces = 5 / 5 = 1, so the line follows row index 0
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport(PrintContentBuilderPagingTests.MakeMembers(5));

            Assert.AreEqual(0, content.Pages[0].CutoffAfterRowIndex);
        }

        [TestMethod]
        public void BuildMemberReport_OneHundredMembers_CutoffAfterRow19OnFirstPage()
        {
            // winningPlaces = 100 / 5 = 20, so the line follows row index 19
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport(PrintContentBuilderPagingTests.MakeMembers(100));

            Assert.AreEqual(19, content.Pages[0].CutoffAfterRowIndex);
            Assert.IsNull(content.Pages[1].CutoffAfterRowIndex);
            Assert.IsNull(content.Pages[2].CutoffAfterRowIndex);
        }

        [TestMethod]
        public void BuildMemberReport_WinningPlacesOnSecondPage_CutoffOnSecondPageOnly()
        {
            // winningPlaces = 210 / 5 = 42, which is row index 1 of page 2
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport(PrintContentBuilderPagingTests.MakeMembers(210));

            Assert.IsNull(content.Pages[0].CutoffAfterRowIndex);
            Assert.AreEqual(1, content.Pages[1].CutoffAfterRowIndex);
        }

        [TestMethod]
        public void BuildMemberReport_ManualCutoff_OverridesWinningPlaces()
        {
            // winningPlaces would be 10, but the manual cutoff of 45 wins and
            // lands on row index 4 of the second page
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport(PrintContentBuilderPagingTests.MakeMembers(50), manualCutoff: 45);

            Assert.IsNull(content.Pages[0].CutoffAfterRowIndex);
            Assert.AreEqual(4, content.Pages[1].CutoffAfterRowIndex);
        }

        [TestMethod]
        public void BuildMemberReport_ManualCutoffOnPageBoundary_LineAfterLastRowOfFirstPage()
        {
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport(PrintContentBuilderPagingTests.MakeMembers(41), manualCutoff: 40);

            Assert.AreEqual(39, content.Pages[0].CutoffAfterRowIndex);
            Assert.IsNull(content.Pages[1].CutoffAfterRowIndex);
        }

        [TestMethod]
        public void BuildMemberReport_ManualCutoffBeyondList_NoCutoffLine()
        {
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport(PrintContentBuilderPagingTests.MakeMembers(10), manualCutoff: 25);

            Assert.IsNull(content.Pages[0].CutoffAfterRowIndex);
        }
    }

    [TestClass]
    public class PrintContentBuilderDuesTests
    {
        [DataTestMethod]
        [DataRow(null, "N/A")]
        [DataRow("", "N/A")]
        [DataRow("   ", "N/A")]
        [DataRow("life ", "life ")]
        [DataRow("2023", "2024")]
        [DataRow("1999", "2000")]
        [DataRow("unknown", "unknown")]
        public void FormatDuesYear_MatchesCurrentBehavior(string lastPaymentYear, string expected)
        {
            Assert.AreEqual(expected, PrintContentBuilder.FormatDuesYear(lastPaymentYear));
        }

        [TestMethod]
        public void BuildMemberReport_PrintDuesOff_DuesTextIsEmpty()
        {
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport(PrintContentBuilderPagingTests.MakeMembers(1), printDues: false);

            Assert.AreEqual(string.Empty, content.Pages[0].Rows[0].DuesText);
        }

        [TestMethod]
        public void BuildMemberReport_PrintDuesOn_DuesTextIsPaidThroughYear()
        {
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport(PrintContentBuilderPagingTests.MakeMembers(1), printDues: true);

            Assert.AreEqual("2024", content.Pages[0].Rows[0].DuesText);
        }

        [TestMethod]
        public void BuildMemberReport_TeamEntryWithDues_CombinesBothPartners()
        {
            TeamMemberScores team = new()
            {
                placing = 1,
                Score = 1500,
                Partner1MemberId = 11,
                Partner2MemberId = 22,
                Partner1FirstName = "Ann",
                Partner1LastName = "Ames",
                Partner2FirstName = "Bob",
                Partner2LastName = "Barnes",
                LastPaymentYear = "2023",
                Partner2LastPaymentYear = "life "
            };
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport([team], printDues: true);

            ReportRowContent row = content.Pages[0].Rows[0];
            Assert.AreEqual("2024 & life ", row.DuesText);
            Assert.AreEqual("11 & 22", row.MemberNumber);
            Assert.AreEqual("Ann Ames & Bob Barnes", row.Name);
        }
    }

    [TestClass]
    public class PrintContentBuilderHeaderTests
    {
        [TestMethod]
        public void BuildMemberReport_HighGameAllSquads_FinalStandingsTitle()
        {
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport(PrintContentBuilderPagingTests.MakeMembers(1), ReportType.HighGame, currentSquad: 0);

            Assert.AreEqual("Test Lanes 5-13-2019", content.TournamentLine);
            Assert.AreEqual("9 Tap Tour High - Game Final Standings", content.Title);
            Assert.IsNull(content.SeriesSubtitle);
            Assert.AreEqual("       Game     Mem No            Name", content.ColumnHeaderLine);
        }

        [TestMethod]
        public void BuildMemberReport_SeniorReport_ColumnHeaderFallsBackToGame()
        {
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport(PrintContentBuilderPagingTests.MakeMembers(1), ReportType.HighGameHandicapGameSenior, printDues: true);

            Assert.AreEqual("9 Tap Tour High - Game Senior Final Standings", content.Title);
            Assert.AreEqual("       Game     Mem No            Name                                           Membership Paid To", content.ColumnHeaderLine);
        }

        [TestMethod]
        public void BuildMemberReport_SquadSpecificReport_TitleNamesTheSquad()
        {
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport(PrintContentBuilderPagingTests.MakeMembers(1), ReportType.HighGame, currentSquad: 2);

            Assert.AreEqual("9 Tap Tour High - Game     Squad 2 Standings ", content.Title);
        }

        [TestMethod]
        public void BuildMemberReport_SeriesAllSquads_FinalSubtitleAndSeriesTitle()
        {
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport(PrintContentBuilderPagingTests.MakeMembers(1), ReportType.HighSeriesScratch, currentSquad: 0, squadList: [0]);

            Assert.AreEqual("Final", content.SeriesSubtitle);
            Assert.AreEqual("9 Tap Tour High - Series Standings", content.Title);
        }

        [DataTestMethod]
        [DataRow(new int[] { 1 }, "Through Squad 1")]
        [DataRow(new int[] { 2 }, "Squad 2")]
        [DataRow(new int[] { 1, 2 }, "Through Squad 2")]
        [DataRow(new int[] { 2, 3 }, "Squads 2 Through 3")]
        [DataRow(new int[] { 2, 4 }, "Squad 2 and 4")]
        [DataRow(new int[] { 1, 2, 3 }, "Through squad3")]
        [DataRow(new int[] { 2, 3, 4 }, "Squads 2 Through 4")]
        [DataRow(new int[] { 1, 3, 5 }, "Squads 1,3,5")]
        public void BuildMemberReport_SeriesSquadFilters_MatchCurrentSubtitles(int[] squads, string expected)
        {
            MemberReportContent content = PrintContentBuilderPagingTests.BuildReport(PrintContentBuilderPagingTests.MakeMembers(1), ReportType.HighSeriesScratch, squadList: [.. squads]);

            Assert.AreEqual(expected, content.SeriesSubtitle);
        }

        [TestMethod]
        public void BuildMemberReport_ThreeOutOf4Tournament_TournamentLineIncludes3of4()
        {
            Tournament tournament = new()
            {
                Location = "Test Lanes",
                Date = new DateTime(2019, 5, 13),
                ThreeOutOf4 = true
            };
            MemberReportContent content = PrintContentBuilder.BuildMemberReport(PrintContentBuilderPagingTests.MakeMembers(1), tournament, ReportType.HighGame, 0, [0], false, null);

            Assert.AreEqual("Test Lanes 3of4 5-13-2019", content.TournamentLine);
        }
    }

    [TestClass]
    public class PrintContentBuilderRecapCardTests
    {
        [TestMethod]
        public void BuildRecapCard_FromValues_TotalHandicapIsFourTimesHandicap()
        {
            RecapCardContent card = PrintContentBuilder.BuildRecapCard(25, 123, "Springfield", "Jane", "Smith", "180", 5);

            Assert.AreEqual("180", card.AverageText);
            Assert.AreEqual("25", card.HandicapText);
            Assert.AreEqual("5", card.BonusText);
            Assert.AreEqual("100", card.TotalHandicapText);
            Assert.AreEqual("Smith, Jane", card.NameLine);
            Assert.AreEqual("Springfield", card.CityLine);
            Assert.AreEqual("123", card.MemberNumberText);
        }

        [TestMethod]
        public void BuildRecapCard_FromMemberWithNulls_UsesZeroHandicapAndEmptyAverage()
        {
            Member mem = new()
            {
                Number = 77,
                City = "Shelbyville",
                FirstName = "John",
                LastName = "Doe",
                Handicap = null,
                Average = null,
                Bonus = 0
            };
            RecapCardContent card = PrintContentBuilder.BuildRecapCard(mem);

            Assert.AreEqual("", card.AverageText);
            Assert.AreEqual("0", card.HandicapText);
            Assert.AreEqual("0", card.TotalHandicapText);
            Assert.AreEqual("Doe, John", card.NameLine);
            Assert.AreEqual("77", card.MemberNumberText);
        }
    }

    [TestClass]
    public class PrintContentBuilderRecapOrderingTests
    {
        [TestMethod]
        public void OrderMembersForRecaps_SortsByLastNameThenFirstName()
        {
            List<Member> members =
            [
                new Member { FirstName = "Zoe", LastName = "Baker" },
                new Member { FirstName = "Amy", LastName = "Cook" },
                new Member { FirstName = "Amy", LastName = "Baker" }
            ];

            List<Member> ordered = PrintContentBuilder.OrderMembersForRecaps(members);

            Assert.AreEqual("Amy Baker", ordered[0].FirstName + " " + ordered[0].LastName);
            Assert.AreEqual("Zoe Baker", ordered[1].FirstName + " " + ordered[1].LastName);
            Assert.AreEqual("Amy Cook", ordered[2].FirstName + " " + ordered[2].LastName);
        }
    }

    [TestClass]
    public class PrintContentBuilderLabelTests
    {
        [TestMethod]
        public void BuildLabelLines_MatchesCurrentLabelFormat()
        {
            Member mem = new()
            {
                FirstName = "Jane",
                LastName = "Smith",
                Street = "12 Main St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701"
            };

            LabelContent label = PrintContentBuilder.BuildLabelLines(mem);

            Assert.AreEqual("Jane Smith", label.NameLine);
            Assert.AreEqual("12 Main St", label.StreetLine);
            Assert.AreEqual("Springfield, IL 62701", label.CityStateZipLine);
        }
    }
}
