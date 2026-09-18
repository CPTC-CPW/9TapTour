using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Repositories;
using System;
using System.Linq;

namespace NineTapTour.IntegrationTests.Repositories
{
    [TestClass]
    public class RegionRepositoryTests
    {
        [TestMethod]
        public void GetDefault_ReturnsRegionSeededByMigration()
        {
            RegionRepository repository = new(TestDatabase.DbFactory);

            Region region = repository.GetDefault();

            Assert.AreEqual(TestDatabase.DefaultRegionId, region.Id);
            Assert.AreEqual("Default", region.Name);
        }

        [TestMethod]
        public void Add_Update_Delete_RoundTrip()
        {
            RegionRepository repository = new(TestDatabase.DbFactory);
            Region region = new() { Name = $"  North {Guid.NewGuid():N}  " };

            repository.Add(region);
            try
            {
                Assert.IsTrue(region.Id > 0, "Add should assign an identity");
                Assert.IsFalse(region.Name.StartsWith(' '), "Add should trim the name");
                Assert.IsTrue(repository.NameExists(region.Name));
                Assert.IsFalse(repository.NameExists(region.Name, excludeId: region.Id), "excludeId should ignore the region itself");

                region.Name = "Renamed " + region.Name;
                repository.Update(region);
                Assert.AreEqual(region.Name, repository.GetById(region.Id).Name);

                Assert.IsTrue(repository.GetAll().Select(r => r.Id).Contains(region.Id));
            }
            finally
            {
                RegionDeleteResult result = repository.Delete(region.Id);
                Assert.IsTrue(result.Deleted, result.Reason);
                Assert.IsNull(repository.GetById(region.Id));
            }
        }

        [TestMethod]
        public void GetAll_IsOrderedByName()
        {
            RegionRepository repository = new(TestDatabase.DbFactory);
            Region aaa = new() { Name = $"AAA {Guid.NewGuid():N}" };
            Region zzz = new() { Name = $"ZZZ {Guid.NewGuid():N}" };
            repository.Add(zzz);
            repository.Add(aaa);
            try
            {
                var names = repository.GetAll().Select(r => r.Name).ToList();
                CollectionAssert.AreEqual(names.OrderBy(n => n, StringComparer.Ordinal).ToList(), names);
            }
            finally
            {
                repository.Delete(aaa.Id);
                repository.Delete(zzz.Id);
            }
        }

        [TestMethod]
        public void Delete_RefusesWhenMembersOrTournamentsReferenceRegion()
        {
            RegionRepository repository = new(TestDatabase.DbFactory);
            Region region = new() { Name = $"InUse {Guid.NewGuid():N}" };
            repository.Add(region);

            using NineTapDb db = TestDatabase.DbFactory.CreateDbContext();
            Member member = new() { Number = 9101, FirstName = "Region", LastName = "User", RegionId = region.Id };
            Tournament tournament = new() { Date = DateTime.Today, Location = "Region Lanes", RegionId = region.Id };
            db.Members.Add(member);
            db.Tournaments.Add(tournament);
            db.SaveChanges();

            try
            {
                RegionDeleteResult result = repository.Delete(region.Id);

                Assert.IsFalse(result.Deleted);
                Assert.AreEqual(1, result.MemberCount);
                Assert.AreEqual(1, result.TournamentCount);
                StringAssert.Contains(result.Reason, "1 member(s)");
                StringAssert.Contains(result.Reason, "1 tournament(s)");
                Assert.IsNotNull(repository.GetById(region.Id), "region must survive a refused delete");
            }
            finally
            {
                db.Members.Remove(member);
                db.Tournaments.Remove(tournament);
                db.SaveChanges();
                repository.Delete(region.Id);
            }
        }

        [TestMethod]
        public void Delete_RefusesDefaultRegionWhileInUse()
        {
            RegionRepository repository = new(TestDatabase.DbFactory);

            // Every seeded member and tournament references the default region.
            RegionDeleteResult result = repository.Delete(TestDatabase.DefaultRegionId);

            Assert.IsFalse(result.Deleted);
            Assert.IsNotNull(result.Reason);
            Assert.IsTrue(result.MemberCount >= 7);
        }

        [TestMethod]
        public void Delete_UnknownRegion_ReportsNotFound()
        {
            RegionRepository repository = new(TestDatabase.DbFactory);

            RegionDeleteResult result = repository.Delete(int.MaxValue);

            Assert.IsFalse(result.Deleted);
            StringAssert.Contains(result.Reason, "not found");
        }
    }
}
