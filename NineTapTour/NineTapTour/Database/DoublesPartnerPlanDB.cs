using NineTapTour.Core.Data;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Database;

public static class DoublesPartnerPlanDB
{
    public static List<DoublesPartnerPlan> GetPlansByTournament(int tournamentId)
    {
        using var db = new NineTapDb();
        return [.. db.DoublesPartnerPlans
            .Include(p => p.Member)
            .Include(p => p.Tournament)
            .Where(p => p.Tournament.Id == tournamentId)
            .OrderBy(p => p.Squad)
            .ThenBy(p => p.Member.Number)];
    }

    public static int GetExpectedPartnerCount(int tournamentId, int memberId, int squad)
    {
        using var db = new NineTapDb();
        return db.DoublesPartnerPlans
            .Where(p => p.Tournament.Id == tournamentId && p.Member.Id == memberId && p.Squad == squad)
            .Select(p => (int?)p.ExpectedPartnerCount)
            .FirstOrDefault() ?? 0;
    }

    public static void UpsertPlan(int tournamentId, int memberId, int squad, int expectedPartnerCount)
    {
        using var db = new NineTapDb();
        UpsertPlan(db, tournamentId, memberId, squad, expectedPartnerCount);
        db.SaveChanges();
    }

    public static void UpsertPlan(NineTapDb db, int tournamentId, int memberId, int squad, int expectedPartnerCount)
    {
        var existing = db.DoublesPartnerPlans
            .Include(p => p.Member)
            .Include(p => p.Tournament)
            .FirstOrDefault(p => p.Tournament.Id == tournamentId && p.Member.Id == memberId && p.Squad == squad);

        if (existing == null)
        {
            var tournament = db.Tournaments.Find(tournamentId);
            var member = db.Members.Find(memberId);
            if (tournament == null || member == null)
                return;

            db.DoublesPartnerPlans.Add(new DoublesPartnerPlan
            {
                Tournament = tournament,
                Member = member,
                Squad = squad,
                ExpectedPartnerCount = expectedPartnerCount,
                UpdatedAtUtc = DateTime.UtcNow
            });
            return;
        }

        existing.ExpectedPartnerCount = expectedPartnerCount;
        existing.UpdatedAtUtc = DateTime.UtcNow;
    }
}
