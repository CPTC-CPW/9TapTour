using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Calculations;
using System;

namespace NineTapTourTests
{
    [TestClass]
    public class MembershipDuesTests
    {
        [DataTestMethod]
        [DataRow("2026-03-03", 2026)] // client example: paid 3/3/26 is valid until 12/31/26
        [DataRow("2027-01-01", 2027)]
        [DataRow("2027-09-30", 2027)]
        [DataRow("2027-10-01", 2028)]
        [DataRow("2027-12-31", 2028)]
        public void PaidThroughYear_CutoffIsOctoberFirst(string payment, int expected)
        {
            Assert.AreEqual(expected, MembershipDues.PaidThroughYear(DateTime.Parse(payment)));
        }

        [TestMethod]
        public void PaidThroughYear_IgnoresTimeOfDay()
        {
            Assert.AreEqual(2027, MembershipDues.PaidThroughYear(new DateTime(2027, 9, 30, 23, 59, 59)));
        }

        [TestMethod]
        public void FormatPaidThroughYear_LifetimeNoPaymentAndPayment()
        {
            Assert.AreEqual("life ", MembershipDues.FormatPaidThroughYear(true, new DateTime(9999, 12, 31)));
            Assert.AreEqual(string.Empty, MembershipDues.FormatPaidThroughYear(false, null));
            Assert.AreEqual("2028", MembershipDues.FormatPaidThroughYear(false, new DateTime(2027, 10, 1)));
        }

        [DataTestMethod]
        [DataRow("2026-03-03", "2026-12-31", true)]
        [DataRow("2026-03-03", "2027-01-01", false)]
        [DataRow("2026-10-01", "2027-12-31", true)]
        [DataRow("2025-09-30", "2026-01-01", false)]
        public void IsCurrent_ValidThroughDecember31OfPaidThroughYear(string payment, string today, bool expected)
        {
            Assert.AreEqual(expected, MembershipDues.IsCurrent(false, DateTime.Parse(payment), DateTime.Parse(today)));
        }

        [TestMethod]
        public void IsCurrent_LifetimeAndNoPayment()
        {
            Assert.IsTrue(MembershipDues.IsCurrent(true, null, new DateTime(2026, 10, 23)));
            Assert.IsFalse(MembershipDues.IsCurrent(false, null, new DateTime(2026, 10, 23)));
        }

        [TestMethod]
        public void DefaultLastPayment_NewBowlerWithNoPayment_UsesJoinDate()
        {
            DateTime joined = new(2026, 3, 3, 14, 30, 0);

            Assert.AreEqual(new DateTime(2026, 3, 3),
                MembershipDues.DefaultLastPayment(null, joined, isLifetimeMember: false, hasGameHistory: false));
        }

        [TestMethod]
        public void DefaultLastPayment_KeepsEnteredPaymentLifetimeAndBowlersWithHistory()
        {
            DateTime joined = new(2026, 3, 3);
            DateTime paid = new(2026, 10, 5);

            Assert.AreEqual(paid, MembershipDues.DefaultLastPayment(paid, joined, false, false));
            Assert.IsNull(MembershipDues.DefaultLastPayment(null, joined, isLifetimeMember: true, hasGameHistory: false));
            Assert.IsNull(MembershipDues.DefaultLastPayment(null, joined, isLifetimeMember: false, hasGameHistory: true));
            Assert.IsNull(MembershipDues.DefaultLastPayment(null, null, false, false));
        }

        [TestMethod]
        public void ImportedPaidThroughYear_ReadsBackAsThatYear()
        {
            // MemberImportTest stores "paid thru YYYY" as 12/31 of YYYY-1.
            Assert.AreEqual(2026, MembershipDues.PaidThroughYear(new DateTime(2025, 12, 31)));
        }
    }
}
