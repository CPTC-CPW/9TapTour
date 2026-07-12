using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Abstractions;
using NineTapTour.Database;   // NineTapDb lives here (in the Data assembly)
using NineTapTour.Models;

namespace NineTapTour.Data.Repositories
{
    public sealed class DoublesRepository : IDoublesRepository
    {
        private readonly IDbContextFactory<NineTapDb> _factory;
        public DoublesRepository(IDbContextFactory<NineTapDb> factory) => _factory = factory;

        // ---- DoublesTeam ----

        /// <summary>
        /// Returns all DoublesTeam records for the given tournament,
        /// with Member1 and Member2 navigation properties loaded.
        /// </summary>
        public List<DoublesTeam> GetTeamsByTournament(int tournamentId)
        {
            using var db = _factory.CreateDbContext();
            return [.. db.DoublesTeams
                .AsNoTracking()
                .Include(dt => dt.Member1)
                .Include(dt => dt.Member2)
                .Include(dt => dt.Tournament)
                .Where(dt => dt.Tournament.Id == tournamentId)
                .OrderBy(dt => dt.Id)];
        }

        /// <summary>
        /// Returns true if a pairing between memberId1 and memberId2 already exists in
        /// the tournament for the given squad (order-independent: (A,B) == (B,A)).
        /// </summary>
        public bool TeamExists(int tournamentId, int memberId1, int memberId2, int squad)
        {
            using var db = _factory.CreateDbContext();
            return db.DoublesTeams.AsNoTracking().Any(dt =>
                dt.Tournament.Id == tournamentId &&
                dt.Squad == squad &&
                ((dt.Member1.Id == memberId1 && dt.Member2.Id == memberId2) ||
                 (dt.Member1.Id == memberId2 && dt.Member2.Id == memberId1)));
        }

        /// <summary>
        /// Creates a new doubles pairing for the tournament and squad.
        /// Returns false (without saving) if the pairing already exists in that squad or if
        /// both member IDs are the same.
        /// </summary>
        public bool AddTeam(int tournamentId, int memberId1, int memberId2, int squad)
        {
            if (memberId1 == memberId2)
                return false;

            if (TeamExists(tournamentId, memberId1, memberId2, squad))
                return false;

            using var db = _factory.CreateDbContext();

            var tournament = db.Tournaments.Find(tournamentId);
            var member1 = db.Members.Find(memberId1);
            var member2 = db.Members.Find(memberId2);

            if (tournament == null || member1 == null || member2 == null)
                return false;

            var team = new DoublesTeam
            {
                Tournament = tournament,
                Member1 = member1,
                Member2 = member2,
                Squad = squad
            };

            db.DoublesTeams.Add(team);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// Removes the DoublesTeam record with the given ID.
        /// </summary>
        public void RemoveTeam(int teamId)
        {
            using var db = _factory.CreateDbContext();
            var team = db.DoublesTeams.Find(teamId);
            if (team != null)
            {
                db.DoublesTeams.Remove(team);
                db.SaveChanges();
            }
        }

        // ---- DoublesPartnerPlan ----

        public List<DoublesPartnerPlan> GetPlansByTournament(int tournamentId)
        {
            using var db = _factory.CreateDbContext();
            return [.. db.DoublesPartnerPlans
                .AsNoTracking()
                .Include(p => p.Member)
                .Include(p => p.Tournament)
                .Where(p => p.Tournament.Id == tournamentId)
                .OrderBy(p => p.Squad)
                .ThenBy(p => p.Member.Number)];
        }

        public int GetExpectedPartnerCount(int tournamentId, int memberId, int squad)
        {
            using var db = _factory.CreateDbContext();
            return db.DoublesPartnerPlans
                .AsNoTracking()
                .Where(p => p.Tournament.Id == tournamentId && p.Member.Id == memberId && p.Squad == squad)
                .Select(p => (int?)p.ExpectedPartnerCount)
                .FirstOrDefault() ?? 0;
        }

        public void UpsertPlan(int tournamentId, int memberId, int squad, int expectedPartnerCount)
        {
            using var db = _factory.CreateDbContext();

            var existing = db.DoublesPartnerPlans
                .Include(p => p.Member)
                .Include(p => p.Tournament)
                .FirstOrDefault(p => p.Tournament.Id == tournamentId && p.Member.Id == memberId && p.Squad == squad);

            if (existing == null)
            {
                var tournament = db.Tournaments.Find(tournamentId);
                var member = db.Members.Find(memberId);
                if (tournament == null || member == null)
                {
                    db.SaveChanges();
                    return;
                }

                db.DoublesPartnerPlans.Add(new DoublesPartnerPlan
                {
                    Tournament = tournament,
                    Member = member,
                    Squad = squad,
                    ExpectedPartnerCount = expectedPartnerCount,
                    UpdatedAtUtc = DateTime.UtcNow
                });
                db.SaveChanges();
                return;
            }

            existing.ExpectedPartnerCount = expectedPartnerCount;
            existing.UpdatedAtUtc = DateTime.UtcNow;
            db.SaveChanges();
        }

        // ---- DoublesPartnerClaim ----

        public List<DoublesPartnerClaim> GetClaimsByTournament(int tournamentId)
        {
            using var db = _factory.CreateDbContext();
            return [.. db.DoublesPartnerClaims
                .AsNoTracking()
                .Include(c => c.Tournament)
                .Include(c => c.SourceMember)
                .Include(c => c.PartnerMember)
                .Where(c => c.Tournament.Id == tournamentId)
                .OrderBy(c => c.Squad)
                .ThenBy(c => c.SourceMember.Number)
                .ThenBy(c => c.PartnerMember.Number)];
        }

        public bool ClaimExists(int tournamentId, int sourceMemberId, int partnerMemberId, int squad)
        {
            using var db = _factory.CreateDbContext();
            return db.DoublesPartnerClaims.AsNoTracking().Any(c =>
                c.Tournament.Id == tournamentId &&
                c.Squad == squad &&
                c.SourceMember.Id == sourceMemberId &&
                c.PartnerMember.Id == partnerMemberId);
        }

        public bool AddClaim(int tournamentId, int sourceMemberId, int partnerMemberId, int squad)
        {
            if (sourceMemberId == partnerMemberId)
                return false;

            using var db = _factory.CreateDbContext();

            bool exists = db.DoublesPartnerClaims.Any(c =>
                c.Tournament.Id == tournamentId &&
                c.Squad == squad &&
                c.SourceMember.Id == sourceMemberId &&
                c.PartnerMember.Id == partnerMemberId);
            if (exists)
                return false;

            var tournament = db.Tournaments.Find(tournamentId);
            var source = db.Members.Find(sourceMemberId);
            var partner = db.Members.Find(partnerMemberId);
            if (tournament == null || source == null || partner == null)
                return false;

            db.DoublesPartnerClaims.Add(new DoublesPartnerClaim
            {
                Tournament = tournament,
                SourceMember = source,
                PartnerMember = partner,
                Squad = squad
            });
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// Removes the directional claims for both member1→member2 and member2→member1
        /// in the given squad, validating each exists before attempting deletion.
        /// </summary>
        public void RemoveClaimsForPair(int tournamentId, int memberId1, int memberId2, int squad)
        {
            using var db = _factory.CreateDbContext();
            var claimsToRemove = db.DoublesPartnerClaims.Where(c =>
                c.Tournament.Id == tournamentId &&
                c.Squad == squad &&
                ((c.SourceMember.Id == memberId1 && c.PartnerMember.Id == memberId2) ||
                 (c.SourceMember.Id == memberId2 && c.PartnerMember.Id == memberId1)));
            db.DoublesPartnerClaims.RemoveRange(claimsToRemove);
            db.SaveChanges();
        }
    }
}
