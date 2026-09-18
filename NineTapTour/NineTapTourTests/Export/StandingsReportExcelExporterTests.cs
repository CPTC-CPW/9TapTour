using ClosedXML.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Export;
using NineTapTour.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using static NineTapTour.Core.Calculations.ReportHelper;

namespace NineTapTourTests.Export
{
    /// <summary>
    /// Pins the cell layout FrmMemberScoresReports.ExportToExcel wrote into the
    /// series report template: header cells on rows 3-4, one bowler per row from
    /// row 5, doubles teams as "a & b", and the optional dues column.
    /// </summary>
    [TestClass]
    public class StandingsReportExcelExporterTests
    {
        private string templatePath;

        [TestInitialize]
        public void CreateBlankTemplate()
        {
            templatePath = Path.Combine(Path.GetTempPath(), $"standings-template-{Guid.NewGuid():N}.xlsx");
            using XLWorkbook workbook = new();
            workbook.Worksheets.Add("Sheet1");
            workbook.SaveAs(templatePath);
        }

        [TestCleanup]
        public void DeleteTemplate()
        {
            if (File.Exists(templatePath))
            {
                File.Delete(templatePath);
            }
        }

        private static StandingsReportExportRequest Request(ReportType type, bool printDues, List<MemberScores> rows)
        {
            return new StandingsReportExportRequest("Golden Master Lanes", "Spring Classic", new DateTime(2026, 3, 7), type, printDues, rows);
        }

        [TestMethod]
        public void Export_Singles_WithDues_WritesHeaderAndRows()
        {
            List<MemberScores> rows =
            [
                new() { placing = 1, Score = 920, MemberId = 101, FirstName = "Alice", LastName = "Anderson", LastPaymentYear = "2025" },
                new() { placing = 2, Score = 732, MemberId = 106, FirstName = "Frank", LastName = "Fox", LastPaymentYear = "" },
                new() { placing = 3, Score = 700, MemberId = 107, FirstName = "Grace", LastName = "Gill", LastPaymentYear = "life " },
            ];
            StandingsReportExcelExporter exporter = new();
            using MemoryStream output = new();

            exporter.Export(templatePath, output, Request(ReportType.HighSeriesScratch, printDues: true, rows));

            output.Position = 0;
            using XLWorkbook workbook = new(output);
            IXLWorksheet ws = workbook.Worksheet(1);
            Assert.AreEqual("Golden Master Lanes", ws.Cell(3, 1).GetString());
            Assert.AreEqual("Spring Classic", ws.Cell(3, 4).GetString());
            Assert.AreEqual(new DateTime(2026, 3, 7), ws.Cell(3, 5).GetDateTime());
            Assert.AreEqual("Series", ws.Cell(4, 2).GetString());
            Assert.AreEqual("Membership Paid To", ws.Cell(4, 5).GetString());

            Assert.AreEqual(1, ws.Cell(5, 1).GetValue<int>());
            Assert.AreEqual(920, ws.Cell(5, 2).GetValue<int>());
            Assert.AreEqual(101, ws.Cell(5, 3).GetValue<int>());
            Assert.AreEqual("Anderson, Alice", ws.Cell(5, 4).GetString());
            Assert.AreEqual("2026", ws.Cell(5, 5).GetString(), "dues year is last payment year + 1");
            Assert.AreEqual("N/A", ws.Cell(6, 5).GetString(), "blank payment year prints N/A");
            Assert.AreEqual("life ", ws.Cell(7, 5).GetString(), "lifetime marker passes through");
        }

        [TestMethod]
        public void Export_Doubles_NoDues_CombinesPartners()
        {
            List<MemberScores> rows =
            [
                new TeamMemberScores
                {
                    placing = 1, Score = 1500,
                    Partner1MemberId = 101, Partner1FirstName = "Alice", Partner1LastName = "Anderson",
                    Partner2MemberId = 102, Partner2FirstName = "Bob", Partner2LastName = "Baker",
                },
            ];
            StandingsReportExcelExporter exporter = new();
            using MemoryStream output = new();

            exporter.Export(templatePath, output, Request(ReportType.HighGame, printDues: false, rows));

            output.Position = 0;
            using XLWorkbook workbook = new(output);
            IXLWorksheet ws = workbook.Worksheet(1);
            Assert.AreEqual("Game", ws.Cell(4, 2).GetString());
            Assert.IsTrue(ws.Cell(4, 5).IsEmpty(), "no dues header when PrintDues is false");
            Assert.AreEqual("101 & 102", ws.Cell(5, 3).GetString());
            Assert.AreEqual("Alice Anderson & Bob Baker", ws.Cell(5, 4).GetString());
            Assert.IsTrue(ws.Cell(5, 5).IsEmpty());
        }

        [DataTestMethod]
        [DataRow(ReportType.HighSeriesScratch, "SeriesGolden Master Lanes Spring Classic 03-07-2026.xlsx")]
        [DataRow(ReportType.HighGameHandicapGameSenior, "SeniorGolden Master Lanes Spring Classic 03-07-2026.xlsx")]
        [DataRow(ReportType.HighGame, "FinalGameGolden Master Lanes Spring Classic 03-07-2026.xlsx")]
        [DataRow(ReportType.HighSeriesHandicap, "FinalGameGolden Master Lanes Spring Classic 03-07-2026.xlsx")]
        public void BuildFileName_MatchesDesktopSuggestion(ReportType type, string expected)
        {
            StandingsReportExcelExporter exporter = new();

            Assert.AreEqual(expected, exporter.BuildFileName(Request(type, false, [])));
        }
    }
}
