using ClosedXML.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Export;
using NineTapTour.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace NineTapTourTests.Export
{
    /// <summary>
    /// Regression tests for the series report Excel exporter extracted from
    /// FrmTournamentResults.ExportToExcel (M7.2). Each test builds a minimal
    /// in-memory template mimicking the cells the exporter touches, runs the
    /// exporter, and reopens the output to assert cell values and formulas.
    /// </summary>
    [TestClass]
    public class SeriesReportExcelExporterTests
    {
        private string _tempDir;

        [TestInitialize]
        public void SetUp()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "SeriesReportExporterTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        [TestCleanup]
        public void TearDown()
        {
            try { Directory.Delete(_tempDir, recursive: true); } catch { }
        }

        private string TempFile(string name) => Path.Combine(_tempDir, name);

        /// <summary>
        /// Creates a minimal results template: a "Results" sheet with header rows and
        /// optional "Progressive Pot" marker rows in column F.
        /// </summary>
        private string CreateTemplate(string name, Action<XLWorkbook> customize = null)
        {
            string path = TempFile(name);
            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Results");
            ws.Cell(3, 1).Value = "Place";
            ws.Cell(3, 2).Value = "Name";
            ws.Cell(3, 7).Value = "Total Score";
            customize?.Invoke(wb);
            wb.SaveAs(path);
            return path;
        }

        private static SeriesReportRow Row(string place, string name, string memberNumber,
            string earnings = "0", string groupLabel = null)
        {
            return new SeriesReportRow(place, groupLabel, name, "10 + 1", "250", memberNumber, earnings);
        }

        private static SeriesReportExportRequest Request(
            IReadOnlyList<SeriesReportRow> rows,
            bool isTwoDay = false,
            bool applyDoublesConsolidation = false,
            IReadOnlyDictionary<int, bool> membership = null)
        {
            return new SeriesReportExportRequest(
                "Test Lanes", " Open", new DateTime(2026, 8, 1), isTwoDay,
                applyDoublesConsolidation, rows, membership ?? new Dictionary<int, bool>());
        }

        [TestMethod]
        public void Export_WritesHeaderAndBowlerRows_WithOrdinalsAndTies()
        {
            string template = CreateTemplate("template.xlsx");
            string output = TempFile("output.xlsx");
            var rows = new List<SeriesReportRow>
            {
                Row("1", "Alice A", "101"),
                Row("2T", "Bob B", "102"),
                Row("2T", "Carol C", "103")
            };

            new SeriesReportExcelExporter().Export(template, output, Request(rows));

            using var wb = new XLWorkbook(output);
            var ws = wb.Worksheet(1);
            Assert.AreEqual("Test Lanes Open", ws.Cell(1, 1).GetString());
            Assert.AreEqual(new DateTime(2026, 8, 1), ws.Cell(2, 1).GetDateTime());
            Assert.AreEqual("Total Score", ws.Cell(3, 7).GetString());
            // Row 4: place 1, no tie
            Assert.AreEqual("1st", ws.Cell(4, 1).GetString());
            Assert.AreEqual("Alice A", ws.Cell(4, 2).GetString());
            Assert.AreEqual("10 + 1", ws.Cell(4, 6).GetString());
            Assert.AreEqual("250", ws.Cell(4, 7).GetString());
            Assert.AreEqual(1, ws.Cell(4, 11).GetValue<int>());
            Assert.AreEqual("101", ws.Cell(4, 12).GetString());
            // Rows 5-6: tied pair renders as ordinal + T
            Assert.AreEqual("2ndT", ws.Cell(5, 1).GetString());
            Assert.AreEqual("2ndT", ws.Cell(6, 1).GetString());
            Assert.AreEqual(2, ws.Cell(5, 11).GetValue<int>());
        }

        [TestMethod]
        public void Export_SkipsProgressivePotRows_WhenTemplateHasThem()
        {
            // Pot row after the first bowler row (row 5)
            string template = CreateTemplate("template.xlsx", wb =>
            {
                var ws = wb.Worksheet("Results");
                ws.Cell(5, 6).Value = "Progressive Pot";
            });
            string output = TempFile("output.xlsx");
            var rows = new List<SeriesReportRow>
            {
                Row("1", "Alice A", "101"),
                Row("2", "Bob B", "102")
            };

            new SeriesReportExcelExporter().Export(template, output, Request(rows));

            using var wb = new XLWorkbook(output);
            var ws = wb.Worksheet(1);
            Assert.AreEqual("1st", ws.Cell(4, 1).GetString());
            // Row 5 is the pot row, so the second bowler lands on row 6
            Assert.AreEqual("Progressive Pot", ws.Cell(5, 6).GetString());
            Assert.AreEqual("2nd", ws.Cell(6, 1).GetString());
            Assert.AreEqual("Bob B", ws.Cell(6, 2).GetString());
        }

        [TestMethod]
        public void Export_MarksNonCurrentMembership_Orange()
        {
            string template = CreateTemplate("template.xlsx");
            string output = TempFile("output.xlsx");
            var rows = new List<SeriesReportRow>
            {
                Row("1", "Alice A", "101"),
                Row("2", "Bob B", "102")
            };
            var membership = new Dictionary<int, bool> { [101] = true, [102] = false };

            new SeriesReportExcelExporter().Export(template, output, Request(rows, membership: membership));

            using var wb = new XLWorkbook(output);
            var ws = wb.Worksheet(1);
            Assert.AreNotEqual(XLColor.Orange, ws.Cell(4, 13).Style.Fill.BackgroundColor);
            Assert.AreEqual(XLColor.Orange, ws.Cell(5, 13).Style.Fill.BackgroundColor);
        }

        [TestMethod]
        public void Export_TwoDay_WritesGroupLabelsAndQualifyingHeader()
        {
            string template = CreateTemplate("template.xlsx");
            string output = TempFile("output.xlsx");
            var rows = new List<SeriesReportRow>
            {
                Row("46th - 59th", "Alice A", "101", groupLabel: "46th - 59th"),
                Row("46th - 59th", "Bob B", "102", groupLabel: "46th - 59th")
            };

            new SeriesReportExcelExporter().Export(template, output, Request(rows, isTwoDay: true));

            using var wb = new XLWorkbook(output);
            var ws = wb.Worksheet(1);
            Assert.AreEqual("Qualifying Score", ws.Cell(3, 7).GetString());
            Assert.AreEqual("46th - 59th", ws.Cell(4, 1).GetString());
            Assert.AreEqual("46th - 59th", ws.Cell(5, 1).GetString());
            // K column holds the numeric start place parsed from the group label
            Assert.AreEqual(46, ws.Cell(4, 11).GetValue<int>());
            Assert.AreEqual(46, ws.Cell(5, 11).GetValue<int>());
        }

        [TestMethod]
        public void Export_DoublesConsolidation_RemapsCheckFormulasAndWritesMemo()
        {
            // Member 7 places twice (rows 0 and 1 → excel rows 4 and 5); member 8 once (row 6).
            // The check sheet references all three bowler rows; the secondary row (5) must be
            // remapped to the next unique row (6), and the primary check gets the memo "1st, 2nd".
            string template = CreateTemplate("template.xlsx", wb =>
            {
                var checks = wb.AddWorksheet("Checks");
                checks.Cell(3, 3).FormulaA1 = "Results!A4";   // primary check place → memo
                checks.Cell(4, 3).FormulaA1 = "Results!B4";
                checks.Cell(20, 3).FormulaA1 = "Results!A5";  // secondary check → remap to row 6
                checks.Cell(21, 3).FormulaA1 = "Results!I5";
                checks.Cell(37, 3).FormulaA1 = "Results!A6";
            });
            string output = TempFile("output.xlsx");
            var rows = new List<SeriesReportRow>
            {
                Row("1", "Dana D", "7", earnings: "100"),
                Row("2", "Dana D", "7", earnings: "50"),
                Row("3", "Evan E", "8", earnings: "25")
            };

            new SeriesReportExcelExporter().Export(template, output,
                Request(rows, applyDoublesConsolidation: true));

            using var wb = new XLWorkbook(output);
            var checks = wb.Worksheet("Checks");
            // Primary check place cell is overwritten with the combined memo text
            Assert.AreEqual("1st, 2nd", checks.Cell(3, 3).GetString());
            // Non-place primary references are untouched
            Assert.AreEqual("Results!B4", checks.Cell(4, 3).FormulaA1);
            // Secondary check references remap from row 5 to the next unique row (6)
            Assert.AreEqual("Results!A6", checks.Cell(20, 3).FormulaA1);
            Assert.AreEqual("Results!I6", checks.Cell(21, 3).FormulaA1);
            // The last unique row keeps its own reference
            Assert.AreEqual("Results!A6", checks.Cell(37, 3).FormulaA1);
        }

        [TestMethod]
        public void ReadEarningsAndPots_ReadsNumbersStringsAndPotRows()
        {
            string template = CreateTemplate("template.xlsx", wb =>
            {
                var ws = wb.Worksheet("Results");
                ws.Cell(4, 9).Value = 100;               // bowler 1 earnings (number)
                ws.Cell(5, 6).Value = "Progressive Pot"; // pot row after bowler 1
                ws.Cell(5, 9).Value = 25;
                ws.Cell(6, 9).Value = "$1,100";          // bowler 2 earnings (currency string)
            });

            List<TemplateEarningsRow> result =
                new SeriesReportExcelExporter().ReadEarningsAndPots(template, rowCount: 2);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(100m, result[0].Earnings);
            Assert.AreEqual(25m, result[0].ProgressivePot);
            Assert.AreEqual(1100m, result[1].Earnings);
            Assert.AreEqual(0m, result[1].ProgressivePot);
        }

        [DataTestMethod]
        // Plain column reference moves to the new row
        [DataRow("=Results!A10", 10, 20, false, "=Results!A20")]
        // Column I expands to include the progressive-pot row below when present
        [DataRow("=Results!I10", 10, 20, true, "=Results!I20+Results!I21")]
        // An existing pot combo collapses to a single reference when the new row has no pot
        [DataRow("=Results!I10+Results!I11", 10, 20, false, "=Results!I20")]
        public void ApplyRowRemap_SmokeCases(string formula, int oldRow, int newRow, bool newHasPot, string expected)
        {
            Assert.AreEqual(expected, SeriesReportExcelExporter.ApplyRowRemap(formula, oldRow, newRow, newHasPot));
        }
    }
}
