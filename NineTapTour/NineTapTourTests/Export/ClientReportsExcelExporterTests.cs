using ClosedXML.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Export;
using NineTapTour.Core.Models;
using System;
using System.IO;

namespace NineTapTourTests.Export
{
    /// <summary>
    /// Pins the workbook layout FrmReports.BtnExport_Click produced: title rows,
    /// bold headers on row 4, data from row 5, currency and date formats.
    /// </summary>
    [TestClass]
    public class ClientReportsExcelExporterTests
    {
        private static ReportTable SampleTable()
        {
            return new ReportTable
            {
                Title = "Tour-Wide — Earnings — Career",
                Columns =
                [
                    new ReportColumn("Rank", "Rank", ReportColumnKind.Integer),
                    new ReportColumn("Name", "Name", ReportColumnKind.Text),
                    new ReportColumn("Earnings", "Earnings", ReportColumnKind.Currency),
                    new ReportColumn("Date", "Date", ReportColumnKind.Date),
                ],
                Rows =
                [
                    [1, "Anderson, Alice", 125.5m, new DateTime(2026, 1, 10)],
                    [2, "Baker, Bob", 40m, new DateTime(2026, 2, 14)],
                ],
            };
        }

        [TestMethod]
        public void Export_WritesTitleHeadersRowsAndFormats()
        {
            ClientReportsExcelExporter exporter = new();
            using MemoryStream output = new();

            exporter.Export(SampleTable(), output);

            output.Position = 0;
            using XLWorkbook workbook = new(output);
            IXLWorksheet ws = workbook.Worksheet("Report");

            Assert.AreEqual("9 Tap Tour Report", ws.Cell(1, 1).GetString());
            Assert.IsTrue(ws.Cell(1, 1).Style.Font.Bold);
            Assert.AreEqual("Tour-Wide — Earnings — Career", ws.Cell(2, 1).GetString());

            Assert.AreEqual("Rank", ws.Cell(4, 1).GetString());
            Assert.AreEqual("Earnings", ws.Cell(4, 3).GetString());
            Assert.IsTrue(ws.Cell(4, 3).Style.Font.Bold);

            Assert.AreEqual(1, ws.Cell(5, 1).GetValue<int>());
            Assert.AreEqual("Anderson, Alice", ws.Cell(5, 2).GetString());
            Assert.AreEqual(125.5, ws.Cell(5, 3).GetValue<double>(), 0.0001);
            Assert.AreEqual("$#,##0.00", ws.Cell(5, 3).Style.NumberFormat.Format);
            Assert.AreEqual(new DateTime(2026, 1, 10), ws.Cell(5, 4).GetDateTime());
            Assert.AreEqual("m/d/yyyy", ws.Cell(5, 4).Style.DateFormat.Format);
            Assert.AreEqual("Baker, Bob", ws.Cell(6, 2).GetString());
        }

        [TestMethod]
        public void BuildFileName_ReplacesInvalidCharacters()
        {
            ClientReportsExcelExporter exporter = new();
            ReportTable table = new() { Title = "Anderson, Alice 101 — High Series — 2024 to 2025 (incl. side pots)" };

            string name = exporter.BuildFileName(table);

            StringAssert.EndsWith(name, ".xlsx");
            foreach (char invalid in Path.GetInvalidFileNameChars())
            {
                Assert.IsFalse(name.Contains(invalid), $"file name contains '{invalid}'");
            }
        }
    }
}
