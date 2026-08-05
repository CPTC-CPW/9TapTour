using ClosedXML.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Import;
using NineTapTour.Core.Models;
using System;

namespace NineTapTourTests.Import
{
    /// <summary>
    /// Pins down the two row/header parsing conventions moved into
    /// ExcelWorkbookReader from FrmMemberData and the member import tool.
    /// Workbooks are built in memory with ClosedXML to mimic the legacy
    /// weekly-book sheet layout; no binary fixtures.
    /// </summary>
    [TestClass]
    public class ExcelWorkbookReaderTests
    {
        [TestMethod]
        public void ReadMemberDataHeader_ParsesLastNameAverageAndNumber()
        {
            using XLWorkbook workbook = new();
            IXLWorksheet ws = workbook.AddWorksheet("Sheet1");
            ws.Cell(1, 2).Value = "Anderson, Alice B";
            ws.Cell(1, 10).Value = 150;
            ws.Cell(1, 14).Value = "Mem # 903";

            ExcelPlayerHeader header = ExcelWorkbookReader.ReadMemberDataHeader(ws);

            Assert.AreEqual("Anderson", header.LastName);
            Assert.AreEqual(150, header.OriginalAverage);
            Assert.AreEqual(903, header.PlayerNumber);
            // Quirk preserved from the form: the first/middle split result was
            // never copied into the returned values, so both stay empty
            Assert.AreEqual("", header.FirstName);
            Assert.AreEqual("", header.MiddleName);
        }

        [TestMethod]
        public void ReadMemberDataHeader_BlankAverage_UsesMinusOneSentinel()
        {
            using XLWorkbook workbook = new();
            IXLWorksheet ws = workbook.AddWorksheet("Sheet1");
            ws.Cell(1, 2).Value = "Smith, John";
            ws.Cell(1, 14).Value = "904";

            ExcelPlayerHeader header = ExcelWorkbookReader.ReadMemberDataHeader(ws);

            Assert.AreEqual(-1, header.OriginalAverage);
            Assert.AreEqual(904, header.PlayerNumber);
        }

        [TestMethod]
        public void ReadMemberDataRow_MapsColumnsWithNullCoalescedSentinels()
        {
            using XLWorkbook workbook = new();
            IXLWorksheet ws = workbook.AddWorksheet("Sheet1");
            ws.Cell(3, 1).Value = 12;                          // GameTotal
            ws.Cell(3, 2).Value = new DateTime(2001, 5, 5);    // Date
            ws.Cell(3, 3).Value = 180;                         // Game1
            ws.Cell(3, 4).Value = 190;                         // Game2
            // Game3/Game4 left blank
            ws.Cell(3, 7).Value = 370;                         // Total
            ws.Cell(3, 8).Value = 185.5;                       // AverageOfRow
            ws.Cell(3, 9).Value = 182.25;                      // TrueAverage
            ws.Cell(3, 10).Value = 184;                        // AVG
            // Handicap (column 11) left blank
            ws.Cell(3, 12).Value = 2;                          // Bonus
            ws.Cell(3, 14).Value = "1st";                      // FinPPHG
            ws.Cell(3, 15).Value = 25.5;                       // Cash
            ws.Cell(3, 16).Value = "note";                     // Notes

            ExcelPlayerHeader player = new()
            {
                LastName = "Anderson",
                OriginalAverage = 150,
                PlayerNumber = 903
            };

            ExcelRow row = ExcelWorkbookReader.ReadMemberDataRow(ws, 3, player);

            Assert.AreEqual("Anderson", row.PlayerLastName);
            Assert.AreEqual(150, row.PlayerOrginalAVG);
            Assert.AreEqual(903, row.PlayerNumber);
            Assert.AreEqual(12, row.GameTotal);
            Assert.AreEqual(new DateTime(2001, 5, 5), row.Date);
            Assert.AreEqual(180, row.Game1);
            Assert.AreEqual(190, row.Game2);
            Assert.AreEqual(-1, row.Game3);
            Assert.AreEqual(-1, row.Game4);
            Assert.AreEqual(370, row.Total);
            Assert.AreEqual(185.5, row.AverageOfRow);
            Assert.AreEqual(182.25, row.TrueAverage);
            Assert.AreEqual(184, row.AVG);
            Assert.AreEqual(-1000, row.HandyCap);
            Assert.AreEqual(2, row.Bonus);
            Assert.AreEqual("1st", row.FinPPHG);
            Assert.AreEqual(25.5, row.Cash);
            Assert.AreEqual("note", row.Notes);
        }

        [TestMethod]
        public void ReadMemberDataRow_BlankCash_IsZeroEvenWithoutFinPPHG()
        {
            using XLWorkbook workbook = new();
            IXLWorksheet ws = workbook.AddWorksheet("Sheet1");
            ws.Cell(3, 1).Value = 3;
            ws.Cell(3, 2).Value = new DateTime(2001, 6, 6);

            ExcelRow row = ExcelWorkbookReader.ReadMemberDataRow(ws, 3, new ExcelPlayerHeader());

            Assert.AreEqual(0, row.Cash);
            Assert.AreEqual("", row.FinPPHG);
        }

        [TestMethod]
        public void ExtractHistoryPlayerInfo_CommaName_PopulatesAllParts()
        {
            using XLWorkbook workbook = new();
            IXLWorksheet ws = workbook.AddWorksheet("Sheet1");
            ws.Cell(1, 2).Value = "Anderson, Alice B";
            ws.Cell(1, 10).Value = 150;
            ws.Cell(1, 14).Value = "Mem # 903";

            string[] firstAndMiddle = ["", ""];
            string lastName = "";
            int orgAvg = -1;
            int number = 0;

            ExcelWorkbookReader.ExtractHistoryPlayerInfo(ws, ref firstAndMiddle, ref lastName,
                ref orgAvg, ref number, ['/', '-']);

            Assert.AreEqual("Anderson", lastName);
            Assert.AreEqual("Alice", firstAndMiddle[0]);
            Assert.AreEqual("B", firstAndMiddle[1]);
            Assert.AreEqual(150, orgAvg);
            Assert.AreEqual(903, number);
        }

        [TestMethod]
        public void ExtractHistoryPlayerInfo_AverageString_FallsBackToParsedPrefix()
        {
            using XLWorkbook workbook = new();
            IXLWorksheet ws = workbook.AddWorksheet("Sheet1");
            ws.Cell(1, 2).Value = "Smith, John";
            ws.Cell(1, 10).Value = "150-L";
            ws.Cell(1, 14).Value = "904";

            string[] firstAndMiddle = ["", ""];
            string lastName = "";
            int orgAvg = -1;
            int number = 0;

            ExcelWorkbookReader.ExtractHistoryPlayerInfo(ws, ref firstAndMiddle, ref lastName,
                ref orgAvg, ref number, ['/', '-']);

            Assert.AreEqual(150, orgAvg);
            Assert.AreEqual(904, number);
        }

        [TestMethod]
        public void ExtractHistoryPlayerInfo_NoDigitsInNumberCell_LeavesZero()
        {
            using XLWorkbook workbook = new();
            IXLWorksheet ws = workbook.AddWorksheet("Sheet1");
            ws.Cell(1, 2).Value = "Smith, John";
            ws.Cell(1, 10).Value = 150;
            ws.Cell(1, 14).Value = "none";

            string[] firstAndMiddle = ["", ""];
            string lastName = "";
            int orgAvg = -1;
            int number = 0;

            ExcelWorkbookReader.ExtractHistoryPlayerInfo(ws, ref firstAndMiddle, ref lastName,
                ref orgAvg, ref number, ['/', '-']);

            Assert.AreEqual(0, number);
        }

        [TestMethod]
        public void ReadHistoryRow_NoGameTotal_ReturnsNull()
        {
            using XLWorkbook workbook = new();
            IXLWorksheet ws = workbook.AddWorksheet("Sheet1");
            // Row 3 left completely blank

            ExcelRow row = ExcelWorkbookReader.ReadHistoryRow(ws, 3, ["", ""], "Anderson", 150, 903);

            Assert.IsNull(row);
        }

        [TestMethod]
        public void ReadHistoryRow_MapsColumnsWithTryCatchSentinels()
        {
            using XLWorkbook workbook = new();
            IXLWorksheet ws = workbook.AddWorksheet("Sheet1");
            ws.Cell(3, 1).Value = 12;                          // GameTotal
            ws.Cell(3, 2).Value = new DateTime(2001, 5, 5);    // Date
            ws.Cell(3, 3).Value = 180;                         // Game1
            // Game2-Game4 blank
            ws.Cell(3, 7).Value = 180;                         // Total
            ws.Cell(3, 9).Value = 182.25;                      // TrueAverage
            ws.Cell(3, 10).Value = 184;                        // AVG
            // Handicap blank: this variant uses -1, not -1000
            ws.Cell(3, 12).Value = 2;                          // Bonus
            ws.Cell(3, 14).Value = "1st";                      // FinPPHG
            ws.Cell(3, 15).Value = 25.5;                       // Cash
            ws.Cell(3, 16).Value = "note";                     // Notes

            ExcelRow row = ExcelWorkbookReader.ReadHistoryRow(ws, 3, ["Alice", "B"], "Anderson", 150, 903);

            Assert.IsNotNull(row);
            Assert.AreEqual("Alice", row.PlayerFirstName);
            Assert.AreEqual("B", row.PlayerMiddleName);
            Assert.AreEqual(12, row.GameTotal);
            Assert.AreEqual(new DateTime(2001, 5, 5), row.Date);
            Assert.AreEqual(180, row.Game1);
            Assert.AreEqual(-1, row.Game2);
            Assert.AreEqual(-1, row.Game3);
            Assert.AreEqual(-1, row.Game4);
            Assert.AreEqual(180, row.Total);
            Assert.AreEqual(-1, row.AverageOfRow);
            Assert.AreEqual(182.25, row.TrueAverage);
            Assert.AreEqual(184, row.AVG);
            Assert.AreEqual(-1, row.HandyCap);
            Assert.AreEqual(2, row.Bonus);
            Assert.AreEqual("1st", row.FinPPHG);
            Assert.AreEqual(25.5, row.Cash);
            Assert.AreEqual("note", row.Notes);
        }

        [TestMethod]
        public void ReadHistoryRow_CashOnlyReadWhenFinPPHGPresent()
        {
            using XLWorkbook workbook = new();
            IXLWorksheet ws = workbook.AddWorksheet("Sheet1");
            ws.Cell(3, 1).Value = 12;
            ws.Cell(3, 2).Value = new DateTime(2001, 5, 5);
            // FinPPHG blank, but cash has a value that must be ignored
            ws.Cell(3, 15).Value = 25.5;

            ExcelRow row = ExcelWorkbookReader.ReadHistoryRow(ws, 3, ["", ""], "Anderson", 150, 903);

            Assert.IsNotNull(row);
            Assert.AreEqual(0, row.Cash);
        }

        [TestMethod]
        public void ReadHistoryRow_BlankDate_UsesDefaultDateSentinel()
        {
            using XLWorkbook workbook = new();
            IXLWorksheet ws = workbook.AddWorksheet("Sheet1");
            ws.Cell(3, 1).Value = 12;
            // Date left blank

            ExcelRow row = ExcelWorkbookReader.ReadHistoryRow(ws, 3, ["", ""], "Anderson", 150, 903);

            Assert.IsNotNull(row);
            Assert.AreEqual(default, row.Date);
        }
    }
}
