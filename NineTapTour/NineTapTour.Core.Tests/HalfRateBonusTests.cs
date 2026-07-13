using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Calculations;

namespace NineTapTour.Core.Tests
{
    [TestClass]
    public class HalfRateBonusTests
    {
        [TestMethod]
        public void NonCashing_KeepsBaseBonus()
        {
            Assert.AreEqual(4, TournamentCalculations.ComputeHalfRateBonus(4, 0, isCashing: false));
            Assert.AreEqual(4, TournamentCalculations.ComputeHalfRateBonus(4, 1, isCashing: false));
        }

        [TestMethod]
        public void Cashing_LosesHalfTheNormalDeduction_RoundedUp()
        {
            // 1st place normally removes ALL pins (4 -> 0, delta -4); half (rounded up) is -2 -> 2.
            Assert.AreEqual(2, TournamentCalculations.ComputeHalfRateBonus(4, 1, isCashing: true));
            // base 5, 1st place: 5 -> 0, delta -5; half rounded up is -3 -> 2.
            Assert.AreEqual(2, TournamentCalculations.ComputeHalfRateBonus(5, 1, isCashing: true));
            // 2nd-5th place normally removes 3 (4 -> 1, delta -3); half rounded up is -2 -> 2.
            Assert.AreEqual(2, TournamentCalculations.ComputeHalfRateBonus(4, 2, isCashing: true));
        }

        [TestMethod]
        public void Cashing_WithZeroBaseBonus_StaysZero()
        {
            Assert.AreEqual(0, TournamentCalculations.ComputeHalfRateBonus(0, 1, isCashing: true));
        }
    }
}
