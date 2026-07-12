using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Data.Repositories;   // DoublesRepository
using NineTapTour.Models;

namespace NineTapTour.IntegrationTests
{
    [TestClass]
    [TestCategory("Integration")]
    public class DoublesRepositoryTests
    {
        [TestMethod]
        public void AddTeam_ThenTeamExists_ReturnsTrue()
        {
            var factory = IntegrationEnvironment.Require();
            int tournamentId, member1Id, member2Id;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db, doubles: true, squads: 2);
                var m1 = TestSeed.AddMember(db, first: "A");
                var m2 = TestSeed.AddMember(db, first: "B");
                db.SaveChanges();
                tournamentId = t.Id; member1Id = m1.Id; member2Id = m2.Id;
            }
            var repo = new DoublesRepository(factory);

            bool added = repo.AddTeam(tournamentId, member1Id, member2Id, 1);

            Assert.IsTrue(added, "AddTeam should return true when the team is newly created.");
            Assert.IsTrue(repo.TeamExists(tournamentId, member1Id, member2Id, 1),
                "TeamExists should return true for the pairing just added.");
        }

        [TestMethod]
        public void GetTeamsByTournament_ReturnsAddedTeams()
        {
            var factory = IntegrationEnvironment.Require();
            int tournamentId, member1Id, member2Id;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db, doubles: true, squads: 2);
                var m1 = TestSeed.AddMember(db, first: "A");
                var m2 = TestSeed.AddMember(db, first: "B");
                db.SaveChanges();
                tournamentId = t.Id; member1Id = m1.Id; member2Id = m2.Id;
            }
            var repo = new DoublesRepository(factory);
            repo.AddTeam(tournamentId, member1Id, member2Id, 1);

            List<DoublesTeam> teams = repo.GetTeamsByTournament(tournamentId);

            Assert.AreEqual(1, teams.Count, "Expected exactly one team for the tournament.");
            var team = teams.Single();
            var memberIds = new[] { team.Member1.Id, team.Member2.Id };
            CollectionAssert.AreEquivalent(new[] { member1Id, member2Id }, memberIds,
                "The team's members should match the two seeded members by Id.");
        }

        [TestMethod]
        public void RemoveTeam_DeletesTheTeam()
        {
            var factory = IntegrationEnvironment.Require();
            int tournamentId, member1Id, member2Id;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db, doubles: true, squads: 2);
                var m1 = TestSeed.AddMember(db, first: "A");
                var m2 = TestSeed.AddMember(db, first: "B");
                db.SaveChanges();
                tournamentId = t.Id; member1Id = m1.Id; member2Id = m2.Id;
            }
            var repo = new DoublesRepository(factory);
            repo.AddTeam(tournamentId, member1Id, member2Id, 1);
            int teamId = repo.GetTeamsByTournament(tournamentId).Single().Id;

            repo.RemoveTeam(teamId);

            Assert.AreEqual(0, repo.GetTeamsByTournament(tournamentId).Count,
                "After RemoveTeam the tournament should have no teams.");
        }

        [TestMethod]
        public void UpsertPlan_InsertThenUpdate()
        {
            var factory = IntegrationEnvironment.Require();
            int tournamentId, member1Id;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db, doubles: true, squads: 2);
                var m1 = TestSeed.AddMember(db, first: "A");
                db.SaveChanges();
                tournamentId = t.Id; member1Id = m1.Id;
            }
            var repo = new DoublesRepository(factory);

            repo.UpsertPlan(tournamentId, member1Id, 1, 2);
            Assert.AreEqual(2, repo.GetExpectedPartnerCount(tournamentId, member1Id, 1),
                "Expected count should be 2 after the initial insert.");

            repo.UpsertPlan(tournamentId, member1Id, 1, 3);
            Assert.AreEqual(3, repo.GetExpectedPartnerCount(tournamentId, member1Id, 1),
                "Expected count should be 3 after the update.");

            Assert.AreEqual(1, repo.GetPlansByTournament(tournamentId).Count,
                "Upserting the same member/squad should update in place, not create a duplicate plan.");
        }

        [TestMethod]
        public void GetExpectedPartnerCount_NoPlan_ReturnsZero()
        {
            var factory = IntegrationEnvironment.Require();
            int tournamentId, member1Id;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db, doubles: true, squads: 2);
                var m1 = TestSeed.AddMember(db, first: "A");
                db.SaveChanges();
                tournamentId = t.Id; member1Id = m1.Id;
            }
            var repo = new DoublesRepository(factory);

            Assert.AreEqual(0, repo.GetExpectedPartnerCount(tournamentId, member1Id, 1),
                "GetExpectedPartnerCount should return 0 when no plan exists.");
        }

        [TestMethod]
        public void AddClaim_ThenClaimExists()
        {
            var factory = IntegrationEnvironment.Require();
            int tournamentId, member1Id, member2Id;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db, doubles: true, squads: 2);
                var m1 = TestSeed.AddMember(db, first: "A");
                var m2 = TestSeed.AddMember(db, first: "B");
                db.SaveChanges();
                tournamentId = t.Id; member1Id = m1.Id; member2Id = m2.Id;
            }
            var repo = new DoublesRepository(factory);

            bool added = repo.AddClaim(tournamentId, member1Id, member2Id, 1);

            Assert.IsTrue(added, "AddClaim should return true when the claim is newly created.");
            Assert.IsTrue(repo.ClaimExists(tournamentId, member1Id, member2Id, 1),
                "ClaimExists should return true for the claim just added.");
            Assert.AreEqual(1, repo.GetClaimsByTournament(tournamentId).Count,
                "Expected exactly one claim for the tournament.");
        }

        [TestMethod]
        public void RemoveClaimsForPair_RemovesBothDirections()
        {
            var factory = IntegrationEnvironment.Require();
            int tournamentId, member1Id, member2Id;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db, doubles: true, squads: 2);
                var m1 = TestSeed.AddMember(db, first: "A");
                var m2 = TestSeed.AddMember(db, first: "B");
                db.SaveChanges();
                tournamentId = t.Id; member1Id = m1.Id; member2Id = m2.Id;
            }
            var repo = new DoublesRepository(factory);
            repo.AddClaim(tournamentId, member1Id, member2Id, 1);
            repo.AddClaim(tournamentId, member2Id, member1Id, 1);
            Assert.AreEqual(2, repo.GetClaimsByTournament(tournamentId).Count,
                "Both directional claims should exist before removal.");

            // RemoveClaimsForPair deletes both member1->member2 and member2->member1 rows for the squad.
            repo.RemoveClaimsForPair(tournamentId, member1Id, member2Id, 1);

            List<DoublesPartnerClaim> remaining = repo.GetClaimsByTournament(tournamentId);
            int involvingPair = remaining.Count(c =>
                c.Squad == 1 &&
                ((c.SourceMember.Id == member1Id && c.PartnerMember.Id == member2Id) ||
                 (c.SourceMember.Id == member2Id && c.PartnerMember.Id == member1Id)));
            Assert.AreEqual(0, involvingPair,
                "RemoveClaimsForPair should remove both directional claims for the pair in that squad.");
        }
    }
}
