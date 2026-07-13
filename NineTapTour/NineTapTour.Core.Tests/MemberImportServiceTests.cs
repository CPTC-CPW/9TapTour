using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Models;
using NineTapTour.Services;

namespace NineTapTour.Core.Tests
{
    [TestClass]
    public class MemberImportServiceTests
    {
        private readonly MemberImportService _svc = new MemberImportService();

        [TestMethod]
        public void SplitName_CommaSeparated_SplitsLastFromFirstMiddle()
        {
            var (last, firstMiddle) = _svc.SplitName("Smith, John A");
            Assert.AreEqual("Smith", last);
            Assert.AreEqual("John A", firstMiddle);
        }

        [TestMethod]
        public void SplitName_AccidentalPeriod_IsTreatedLikeComma()
        {
            var (last, firstMiddle) = _svc.SplitName("Jones. Mary B");
            Assert.AreEqual("Jones", last);
            Assert.AreEqual("Mary B", firstMiddle);
        }

        [TestMethod]
        public void SplitName_NoSeparator_ReturnsEmpties()
        {
            var (last, firstMiddle) = _svc.SplitName("Cher");
            Assert.AreEqual("", last);
            Assert.AreEqual("", firstMiddle);
        }

        [TestMethod]
        public void ParseMemberNumber_StripsNonNumericCharacters()
        {
            Assert.AreEqual(12345, _svc.ParseMemberNumber("12345"));
            Assert.AreEqual(123, _svc.ParseMemberNumber("#00123"));
            Assert.AreEqual(0, _svc.ParseMemberNumber("0"));
        }

        [TestMethod]
        public void BuildGameFromRow_MapsSentinelsToNullAndSetsUseFlags()
        {
            var row = new ExcelRow
            {
                Game1 = 200,
                Game2 = -1,   // absent
                Game3 = 180,
                Game4 = -1,   // absent
                HandyCap = 20,
                Bonus = -1,   // absent
                Cash = 50.0,
                Notes = "legacy",
                AVG = 190,
                TrueAverage = 185.5
            };

            var game = _svc.BuildGameFromRow(row);

            Assert.AreEqual(200, game.Game1);
            Assert.IsNull(game.Game2);
            Assert.AreEqual(180, game.Game3);
            Assert.IsNull(game.Game4);
            Assert.IsTrue(game.UseGame1);
            Assert.IsFalse(game.UseGame2);
            Assert.IsTrue(game.UseGame3);
            Assert.IsFalse(game.UseGame4);
            Assert.AreEqual(20, game.Handicap);
            Assert.IsNull(game.Bonus);
            Assert.AreEqual(50m, game.MoneyWon);
            Assert.IsTrue(game.IsFinalized);
            Assert.AreEqual(190, game.AdjustedAvg);
            Assert.AreEqual(185.5, game.LeagueAverage);
        }

        [TestMethod]
        public void BuildGameFromRow_ZeroCash_LeavesMoneyWonNull()
        {
            var game = _svc.BuildGameFromRow(new ExcelRow { Game1 = 150, Cash = 0 });
            Assert.IsNull(game.MoneyWon);
        }
    }
}
