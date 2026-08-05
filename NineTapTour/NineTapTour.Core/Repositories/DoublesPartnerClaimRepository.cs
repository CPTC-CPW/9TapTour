#nullable disable
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Core.Repositories;

public class DoublesPartnerClaimRepository : IDoublesPartnerClaimRepository
{
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public DoublesPartnerClaimRepository(IDbContextFactory<NineTapDb> dbFactory)
    {
        this.dbFactory = dbFactory;
    }

    public List<DoublesPartnerClaim> GetClaimsByTournament(int tournamentId)
    {
        using var db = dbFactory.CreateDbContext();
        return [.. db.DoublesPartnerClaims
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
        using var db = dbFactory.CreateDbContext();
        return db.DoublesPartnerClaims.Any(c =>
            c.Tournament.Id == tournamentId &&
            c.Squad == squad &&
            c.SourceMember.Id == sourceMemberId &&
            c.PartnerMember.Id == partnerMemberId);
    }

    public bool AddClaim(int tournamentId, int sourceMemberId, int partnerMemberId, int squad)
    {
        if (sourceMemberId == partnerMemberId)
            return false;

        using var db = dbFactory.CreateDbContext();

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
        using var db = dbFactory.CreateDbContext();
        var claimsToRemove = db.DoublesPartnerClaims.Where(c =>
            c.Tournament.Id == tournamentId &&
            c.Squad == squad &&
            ((c.SourceMember.Id == memberId1 && c.PartnerMember.Id == memberId2) ||
             (c.SourceMember.Id == memberId2 && c.PartnerMember.Id == memberId1)));
        db.DoublesPartnerClaims.RemoveRange(claimsToRemove);
        db.SaveChanges();
    }
}
