using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.IntegrationTests
{
    [TestClass]
    public class InactiveMembersRepositoryTests
    {
        [TestMethod]
        public void GetLastEntryDates_ReturnsMostRecentTournamentPerMember()
        {
            ReportsRepository repository = new(TestDatabase.DbFactory);

            Dictionary<int, DateTime> lastEntryDates = repository.GetLastEntryDates();

            // Seeded 3-of-4 tournament is 1/10/2026 and the regular one is 2/14/2026
            Assert.HasCount(7, lastEntryDates);
            Assert.AreEqual(new DateTime(2026, 2, 14), lastEntryDates[TestDatabase.DbIdByNumber[101]], "101 bowled both");
            Assert.AreEqual(new DateTime(2026, 1, 10), lastEntryDates[TestDatabase.DbIdByNumber[102]], "102 bowled only the 3-of-4");
            Assert.AreEqual(new DateTime(2026, 2, 14), lastEntryDates[TestDatabase.DbIdByNumber[107]], "107 bowled only the regular");
        }

        [TestMethod]
        public void SetInactive_FlagsOnlyTheGivenMembers()
        {
            MemberRepository repository = new(TestDatabase.DbFactory);
            Member first = NewUnseededMember(901);
            Member second = NewUnseededMember(902);

            using (NineTapDb db = TestDatabase.DbFactory.CreateDbContext())
            {
                db.Members.AddRange(first, second);
                db.SaveChanges();
            }

            try
            {
                int updated = repository.SetInactive([first.Id]);
                int updatedAgain = repository.SetInactive([first.Id]);

                using NineTapDb db = TestDatabase.DbFactory.CreateDbContext();
                Assert.AreEqual(1, updated);
                Assert.AreEqual(0, updatedAgain, "already inactive members are not counted");
                Assert.IsFalse(db.Members.Single(m => m.Id == first.Id).IsActive);
                Assert.IsTrue(db.Members.Single(m => m.Id == second.Id).IsActive);
            }
            finally
            {
                using NineTapDb db = TestDatabase.DbFactory.CreateDbContext();
                db.Members.RemoveRange(db.Members.Where(m => m.Id == first.Id || m.Id == second.Id));
                db.SaveChanges();
            }
        }

        private static Member NewUnseededMember(int number)
        {
            return new Member
            {
                Number = number,
                IsActive = true,
                FirstName = "Temp",
                LastName = "Member" + number,
                MiddleInitial = "",
                Street = "1 Test St",
                City = "Testville",
                State = "WA",
                PostalCode = "98000",
                PrimaryPhone = "555-0100",
            };
        }
    }
}
