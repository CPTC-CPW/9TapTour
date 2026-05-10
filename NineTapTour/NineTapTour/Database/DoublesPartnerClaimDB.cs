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
}
