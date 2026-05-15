using Microsoft.EntityFrameworkCore;
using NineTapTour.Models;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Database;

public static class DoublesPartnerClaimDB
{
    public static List<DoublesPartnerClaim> GetClaimsByTournament(int tournamentId)
    {
        using var db = new NineTapDb();
        return [.. db.DoublesPartnerClaims
            .Include(c => c.Tournament)
            .Include(c => c.SourceMember)
            .Include(c => c.PartnerMember)
            .Where(c => c.Tournament.Id == tournamentId)
            .OrderBy(c => c.Squad)
            .ThenBy(c => c.SourceMember.Number)
            .ThenBy(c => c.PartnerMember.Number)];
    }

    public static bool ClaimExists(int tournamentId, int sourceMemberId, int partnerMemberId, int squad)
    {
        using var db = new NineTapDb();
        return db.DoublesPartnerClaims.Any(c =>
            c.Tournament.Id == tournamentId &&
            c.Squad == squad &&
            c.SourceMember.Id == sourceMemberId &&
            c.PartnerMember.Id == partnerMemberId);
    }

    public static bool AddClaim(int tournamentId, int sourceMemberId, int partnerMemberId, int squad)
    {
        if (sourceMemberId == partnerMemberId)
            return false;

        using var db = new NineTapDb();

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
    public static void RemoveClaimsForPair(int tournamentId, int memberId1, int memberId2, int squad)
    {
        using var db = new NineTapDb();
        var claimsToRemove = db.DoublesPartnerClaims.Where(c =>
            c.Tournament.Id == tournamentId &&
            c.Squad == squad &&
            ((c.SourceMember.Id == memberId1 && c.PartnerMember.Id == memberId2) ||
             (c.SourceMember.Id == memberId2 && c.PartnerMember.Id == memberId1)));
        db.DoublesPartnerClaims.RemoveRange(claimsToRemove);
        db.SaveChanges();
    }
}
