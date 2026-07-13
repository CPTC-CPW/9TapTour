using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Models;
using NineTapTour.Services;

namespace NineTapTour.Core.Tests
{
    [TestClass]
    public class DoublesDiscrepancyServiceTests
    {
        private static Member Mem(int id, int number) =>
            new Member { Id = id, Number = number, FirstName = "F" + number, LastName = "L" + number };

        private static DoublesPartnerClaim Claim(Member source, Member partner, int squad = 1) =>
            new DoublesPartnerClaim { SourceMember = source, PartnerMember = partner, Squad = squad };

        private static DoublesPartnerPlan Plan(Member member, int expected, int squad = 1) =>
            new DoublesPartnerPlan { Member = member, ExpectedPartnerCount = expected, Squad = squad };

        private readonly DoublesDiscrepancyService _svc = new DoublesDiscrepancyService();

        [TestMethod]
        public void ReciprocalClaims_WithMatchingPlans_ProduceNoDiscrepancies()
        {
            var a = Mem(1, 101);
            var b = Mem(2, 102);
            var claims = new List<DoublesPartnerClaim> { Claim(a, b), Claim(b, a) };
            var plans = new List<DoublesPartnerPlan> { Plan(a, 1), Plan(b, 1) };

            var result = _svc.FindDiscrepancies(plans, claims);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void OneWayClaim_FlagsMissingReciprocal()
        {
            var a = Mem(1, 101);
            var b = Mem(2, 102);
            var claims = new List<DoublesPartnerClaim> { Claim(a, b) }; // b never claims a back
            var plans = new List<DoublesPartnerPlan> { Plan(a, 1) };

            var result = _svc.FindDiscrepancies(plans, claims);

            var missing = result.Single(d => d.Type == DoublesDiscrepancyType.MissingReciprocal);
            Assert.AreEqual(a.Id, missing.SourceMemberId);
            Assert.AreEqual(b.Id, missing.PartnerMemberId);
        }

        [TestMethod]
        public void ReciprocalIsSquadScoped_CrossSquadDoesNotCount()
        {
            var a = Mem(1, 101);
            var b = Mem(2, 102);
            // a claims b in squad 1, b claims a in squad 2 — not reciprocal within a squad.
            var claims = new List<DoublesPartnerClaim> { Claim(a, b, squad: 1), Claim(b, a, squad: 2) };

            var result = _svc.FindDiscrepancies(new List<DoublesPartnerPlan>(), claims);

            Assert.AreEqual(2, result.Count(d => d.Type == DoublesDiscrepancyType.MissingReciprocal));
        }

        [TestMethod]
        public void PlannedCountDifferentFromClaims_FlagsCountMismatch()
        {
            var a = Mem(1, 101);
            var b = Mem(2, 102);
            // a plans 2 partners but only claims 1.
            var claims = new List<DoublesPartnerClaim> { Claim(a, b), Claim(b, a) };
            var plans = new List<DoublesPartnerPlan> { Plan(a, 2), Plan(b, 1) };

            var result = _svc.FindDiscrepancies(plans, claims);

            var mismatch = result.Single(d => d.Type == DoublesDiscrepancyType.CountMismatch);
            Assert.AreEqual(a.Id, mismatch.SourceMemberId);
            Assert.AreEqual(2, mismatch.PlannedCount);
            Assert.AreEqual(1, mismatch.ActualCount);
        }
    }
}
