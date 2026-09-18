using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTourTests.Services
{
    /// <summary>
    /// Characterization of the client-report composition logic moved out of
    /// FrmReports: period labels, titles, and the auto-generated column set.
    /// </summary>
    [TestClass]
    public class ReportsServiceTests
    {
        [TestMethod]
        public void NormalizePeriod_Career_WhenBothYearsNull()
        {
            (int? start, int? end, string label) = ReportsService.NormalizePeriod(null, null);

            Assert.IsNull(start);
            Assert.IsNull(end);
            Assert.AreEqual("Career", label);
        }

        [TestMethod]
        public void NormalizePeriod_SingleYear()
        {
            (int? start, int? end, string label) = ReportsService.NormalizePeriod(2025, 2025);

            Assert.AreEqual(2025, start);
            Assert.AreEqual(2025, end);
            Assert.AreEqual("2025", label);
        }

        [TestMethod]
        public void NormalizePeriod_SwapsReversedRange()
        {
            (int? start, int? end, string label) = ReportsService.NormalizePeriod(2025, 2023);

            Assert.AreEqual(2023, start);
            Assert.AreEqual(2025, end);
            Assert.AreEqual("2023 to 2025", label);
        }

        [TestMethod]
        public void BuildTitle_TourWide_WithSuffixes()
        {
            string title = ReportsService.BuildTitle(null, "Earnings", "Career", includeSidePots: true, includeImported: false);

            Assert.AreEqual("Tour-Wide — Earnings — Career (incl. side pots) (excl. imported history)", title);
        }

        [TestMethod]
        public void BuildTitle_Individual_UsesMemberToString()
        {
            Member member = new() { FirstName = "Alice", LastName = "Anderson", Number = 101 };

            string title = ReportsService.BuildTitle(member, "Summary", "2025", includeSidePots: false, includeImported: true);

            Assert.AreEqual("Anderson, Alice 101 — Summary — 2025", title);
        }

        [TestMethod]
        public void BuildTable_HighSeriesRows_ProducesFriendlyHeadersAndKinds()
        {
            List<HighSeriesRow> rows =
            [
                new() { Rank = 1, Member = 101, Name = "Anderson, Alice", Date = new DateTime(2026, 1, 10), Location = "Lanes", Series = 660, SeriesWithHdcp = 920 },
            ];

            ReportTable table = ReportsService.BuildTable("t", rows);

            CollectionAssert.AreEqual(
                new[] { "Rank", "Member #", "Name", "Date", "Location", "Series", "Series w/HDCP" },
                table.Columns.Select(c => c.Header).ToArray());
            CollectionAssert.AreEqual(
                new[]
                {
                    ReportColumnKind.Integer, ReportColumnKind.Integer, ReportColumnKind.Text, ReportColumnKind.Date,
                    ReportColumnKind.Text, ReportColumnKind.Integer, ReportColumnKind.Integer,
                },
                table.Columns.Select(c => c.Kind).ToArray());
            Assert.AreEqual(1, table.Rows.Count);
            Assert.AreEqual(920, table.Rows[0][6]);
            Assert.IsFalse(table.IsEmpty);
        }

        [TestMethod]
        public void BuildTable_MemberSummary_CurrencyAndNumberKinds()
        {
            List<MemberReportSummary> rows = [new() { Member = 5, Earnings = 12.5m, Average = 180.25 }];

            ReportTable table = ReportsService.BuildTable("t", rows);

            ReportColumn earnings = table.Columns.Single(c => c.Name == "Earnings");
            ReportColumn average = table.Columns.Single(c => c.Name == "Average");
            Assert.AreEqual(ReportColumnKind.Currency, earnings.Kind);
            Assert.AreEqual(ReportColumnKind.Number, average.Kind);
            Assert.AreEqual("1st Place", table.Columns.Single(c => c.Name == "FirstPlace").Header);
            Assert.AreEqual("Top 10", table.Columns.Single(c => c.Name == "Top10").Header);
        }

        [TestMethod]
        public void BuildTable_EmptyTypedList_StillYieldsColumns()
        {
            ReportTable table = ReportsService.BuildTable("t", new List<ReportStatistic>());

            Assert.IsTrue(table.IsEmpty);
            CollectionAssert.AreEqual(new[] { "Statistic", "Value" }, table.Columns.Select(c => c.Header).ToArray());
        }
    }
}
