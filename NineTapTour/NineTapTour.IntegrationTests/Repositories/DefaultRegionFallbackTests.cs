using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Repositories;
using System;
using System.Linq;

namespace NineTapTour.IntegrationTests.Repositories
{
    /// <summary>
    /// The desktop app and the legacy importer never set RegionId. The
    /// repositories must apply the default region on insert and keep the
    /// stored region on update so a region chosen on the website survives a
    /// desktop edit.
    /// </summary>
    [TestClass]
    public class DefaultRegionFallbackTests
    {
        [TestMethod]
        public void AddOrUpdateMember_InsertWithoutRegion_UsesDefaultRegion()
        {
            MemberRepository repository = new(TestDatabase.DbFactory);
            Member member = new() { Number = 9201, FirstName = "No", LastName = "Region", IsActive = true };

            repository.AddOrUpdateMember(member);
            try
            {
                Assert.AreEqual(TestDatabase.DefaultRegionId, member.RegionId);
                Assert.AreEqual(TestDatabase.DefaultRegionId, repository.GetMember(9201).RegionId);
            }
            finally
            {
                RemoveMember(member.Id);
            }
        }

        [TestMethod]
        public void AddOrUpdateMember_UpdateWithoutRegion_PreservesStoredRegion()
        {
            RegionRepository regions = new(TestDatabase.DbFactory);
            MemberRepository repository = new(TestDatabase.DbFactory);
            Region other = new() { Name = $"Other {Guid.NewGuid():N}" };
            regions.Add(other);

            Member member = new() { Number = 9202, FirstName = "Web", LastName = "Chosen", IsActive = true, RegionId = other.Id };
            repository.AddOrUpdateMember(member);
            try
            {
                // Simulate FrmMemberData.SaveMemberData: a fresh object with the
                // same Id and RegionId left at 0.
                Member desktopEdit = new() { Id = member.Id, Number = 9202, FirstName = "Web", LastName = "Edited", IsActive = true };
                repository.AddOrUpdateMember(desktopEdit);

                Member stored = repository.GetMember(9202);
                Assert.AreEqual("Edited", stored.LastName);
                Assert.AreEqual(other.Id, stored.RegionId, "a desktop edit must not wipe the region chosen on the web");
            }
            finally
            {
                RemoveMember(member.Id);
                regions.Delete(other.Id);
            }
        }

        [TestMethod]
        public void AddOrUpdateMember_UpdateByNumberWithoutRegion_PreservesStoredRegion()
        {
            RegionRepository regions = new(TestDatabase.DbFactory);
            MemberRepository repository = new(TestDatabase.DbFactory);
            Region other = new() { Name = $"Import {Guid.NewGuid():N}" };
            regions.Add(other);

            Member member = new() { Number = 9203, FirstName = "Legacy", LastName = "Import", IsActive = true, RegionId = other.Id };
            repository.AddOrUpdateMember(member);
            try
            {
                // Imports resolve identity by Number (Id 0).
                Member imported = new() { Number = 9203, FirstName = "Legacy", LastName = "Reimported", IsActive = true };
                repository.AddOrUpdateMember(imported);

                Assert.AreEqual(other.Id, repository.GetMember(9203).RegionId);
            }
            finally
            {
                RemoveMember(member.Id);
                regions.Delete(other.Id);
            }
        }

        [TestMethod]
        public void AddTournament_InsertWithoutRegion_UsesDefaultRegion()
        {
            TournamentRepository repository = new(TestDatabase.DbFactory);
            Tournament tournament = new() { Date = new DateTime(2026, 9, 1), Location = "Fallback Lanes", Squads = 1 };

            repository.AddTournament(tournament);
            try
            {
                Assert.AreEqual(TestDatabase.DefaultRegionId, tournament.RegionId);
                Assert.AreEqual(TestDatabase.DefaultRegionId, repository.GetTourneyByID(tournament.Id).RegionId);
            }
            finally
            {
                repository.DeleteTournament(tournament);
            }
        }

        [TestMethod]
        public void AddTournament_SharedContextOverload_UsesDefaultRegion()
        {
            TournamentRepository repository = new(TestDatabase.DbFactory);
            Tournament tournament = new() { Date = new DateTime(2026, 9, 2), Location = "Shared Context Lanes", Squads = 1 };

            using (NineTapDb db = TestDatabase.DbFactory.CreateDbContext())
            {
                repository.AddTournament(tournament, db);
                db.SaveChanges();
            }

            try
            {
                Assert.AreEqual(TestDatabase.DefaultRegionId, repository.GetTourneyByID(tournament.Id).RegionId);
            }
            finally
            {
                repository.DeleteTournament(tournament);
            }
        }

        [TestMethod]
        public void AddTournament_UpdateWithoutRegion_PreservesStoredRegion()
        {
            RegionRepository regions = new(TestDatabase.DbFactory);
            TournamentRepository repository = new(TestDatabase.DbFactory);
            Region other = new() { Name = $"Tourney {Guid.NewGuid():N}" };
            regions.Add(other);

            Tournament tournament = new() { Date = new DateTime(2026, 9, 3), Location = "Web Lanes", Squads = 1, RegionId = other.Id };
            repository.AddTournament(tournament);
            try
            {
                Tournament desktopEdit = new() { Id = tournament.Id, Date = tournament.Date, Location = "Web Lanes Edited", Squads = 2 };
                repository.AddTournament(desktopEdit);
                Assert.AreEqual(other.Id, repository.GetTourneyByID(tournament.Id).RegionId, "AddTournament update path");

                Tournament secondEdit = new() { Id = tournament.Id, Date = tournament.Date, Location = "Web Lanes Edited Twice", Squads = 3 };
                repository.UpdateTournament(secondEdit);
                Tournament stored = repository.GetTourneyByID(tournament.Id);
                Assert.AreEqual("Web Lanes Edited Twice", stored.Location);
                Assert.AreEqual(other.Id, stored.RegionId, "UpdateTournament path");
            }
            finally
            {
                repository.DeleteTournament(tournament);
                regions.Delete(other.Id);
            }
        }

        private static void RemoveMember(int memberId)
        {
            using NineTapDb db = TestDatabase.DbFactory.CreateDbContext();
            Member member = db.Members.Single(m => m.Id == memberId);
            db.Members.Remove(member);
            db.SaveChanges();
        }
    }
}
