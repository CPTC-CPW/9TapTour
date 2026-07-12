using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Calculations;

namespace NineTapTour.Core.Tests
{
    [TestClass]
    public class TournamentStatsCalculatorTests
    {
        [TestMethod]
        public void GetTop3OutOf4_FourScores_DropsLowestAndReturnsThreeDescending()
        {
            List<int> top3 = TournamentStatsCalculator.GetTop3OutOf4(200, 100, 150, 175);

            Assert.HasCount(3, top3);
            Assert.IsFalse(top3.Contains(100), "Lowest game must be dropped.");
            CollectionAssert.AreEqual(new List<int> { 200, 175, 150 }, top3, "Scores should be descending.");
        }

        [TestMethod]
        public void GetTop3OutOf4_ThreeOrFewerScores_DropsNothing()
        {
            Assert.HasCount(3, TournamentStatsCalculator.GetTop3OutOf4(100, 120, 110, null));
            Assert.HasCount(2, TournamentStatsCalculator.GetTop3OutOf4(100, 120, null, null));
        }
    }
}
