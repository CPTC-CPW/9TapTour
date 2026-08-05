using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Export;
using NineTapTour.Core.Import;
using NineTapTour.Core.Services;

namespace NineTapTourTests.Characterization
{
    /// <summary>
    /// Golden-master tests freezing the current behavior of private form logic
    /// before it is extracted into Core services (M7). When a method moves,
    /// re-point the test at the Core service — do not rewrite expectations.
    /// </summary>
    [TestClass]
    public class ComputeHalfRateBonusCharacterizationTests
    {
        [DataTestMethod]
        // Not cashing: bonus passes through untouched regardless of place
        [DataRow(4, 1, false, 4)]
        [DataRow(0, 11, false, 0)]
        // Cashing: half of the normal deduction, rounded so the bowler loses
        // slightly more than a pure half
        [DataRow(4, 1, true, 2)]   // normal 0, delta -4, half -2
        [DataRow(5, 1, true, 2)]   // normal 0, delta -5, half -3 (round up magnitude)
        [DataRow(3, 2, true, 1)]   // normal 0, delta -3, half -2
        [DataRow(5, 3, true, 3)]   // normal 2, delta -3, half -2
        [DataRow(2, 6, true, 1)]   // normal 0, delta -2, half -1
        [DataRow(5, 7, true, 4)]   // normal 3, delta -2, half -1
        [DataRow(1, 11, true, 0)]  // normal 0, delta -1, half -1
        [DataRow(5, 12, true, 4)]  // normal 4, delta -1, half -1
        [DataRow(0, 1, true, 0)]   // nothing to deduct
        public void ComputeHalfRateBonus_MatchesCurrentBehavior(int baseBonus, int place, bool isCashing, int expected)
        {
            Assert.AreEqual(expected, FinalizeCalculationService.ComputeHalfRateBonus(baseBonus, place, isCashing));
        }
    }

    [TestClass]
    public class ApplyRowRemapCharacterizationTests
    {
        [DataTestMethod]
        // Plain column reference moves to the new row
        [DataRow("=Results!A10", 10, 20, false, "=Results!A20")]
        // Column I expands to include the progressive-pot row below when present
        [DataRow("=Results!I10", 10, 20, true, "=Results!I20+Results!I21")]
        // An existing pot combo collapses to a single reference when the new row has no pot
        [DataRow("=Results!I10+Results!I11", 10, 20, false, "=Results!I20")]
        // Multiple columns in one formula all remap; I keeps its special handling
        [DataRow("=Results!A10+Results!B10+Results!I10", 10, 20, false, "=Results!A20+Results!B20+Results!I20")]
        // The (?!\d) guard prevents partial row-number matches
        [DataRow("=Results!A100", 10, 20, false, "=Results!A100")]
        // Case-insensitive match; sheet prefix is normalized, column letter case is preserved
        [DataRow("=results!a10", 10, 20, false, "=Results!a20")]
        public void ApplyRowRemap_MatchesCurrentBehavior(string formula, int oldRow, int newRow, bool newHasPot, string expected)
        {
            Assert.AreEqual(expected, SeriesReportExcelExporter.ApplyRowRemap(formula, oldRow, newRow, newHasPot));
        }
    }

    [TestClass]
    public class SplitNameCharacterizationTests
    {
        [DataTestMethod]
        [DataRow("Smith, John", "Smith", "John")]
        [DataRow("Smith. John", "Smith", "John")]
        [DataRow("Smith-Jones, Mary Ann", "Smith-Jones", "Mary Ann")]
        // Quirk: the parser assumes a space after the separator, so a missing
        // space eats the first character of the first name
        [DataRow("Smith,John", "Smith", "ohn")]
        public void SplitName_MatchesCurrentBehavior(string fullName, string expectedLast, string expectedFirstMiddle)
        {
            string last = "unchanged-last";
            string firstMiddle = "unchanged-first";
            NameParser.SplitName(ref last, ref firstMiddle, fullName);
            Assert.AreEqual(expectedLast, last);
            Assert.AreEqual(expectedFirstMiddle, firstMiddle);
        }

        [TestMethod]
        public void SplitName_NoSeparator_LeavesInputsUntouched()
        {
            string last = "unchanged-last";
            string firstMiddle = "unchanged-first";
            NameParser.SplitName(ref last, ref firstMiddle, "Madonna");
            Assert.AreEqual("unchanged-last", last);
            Assert.AreEqual("unchanged-first", firstMiddle);
        }
    }
}
