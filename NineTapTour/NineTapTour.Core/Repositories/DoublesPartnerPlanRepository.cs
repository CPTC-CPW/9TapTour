#nullable disable
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Core.Repositories;

public class DoublesPartnerPlanRepository : IDoublesPartnerPlanRepository
{
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public DoublesPartnerPlanRepository(IDbContextFactory<NineTapDb> dbFactory)
    {
        this.dbFactory = dbFactory;
    }

    public List<DoublesPartnerPlan> GetPlansByTournament(int tournamentId)
    {
        using var db = dbFactory.CreateDbContext();
        return [.. db.DoublesPartnerPlans
            .Include(p => p.Member)
            .Include(p => p.Tournament)
            .Where(p => p.Tournament.Id == tournamentId)
            .OrderBy(p => p.Squad)
            .ThenBy(p => p.Member.Number)];
    }

    public int GetExpectedPartnerCount(int tournamentId, int memberId, int squad)
    {
        using var db = dbFactory.CreateDbContext();
        return db.DoublesPartnerPlans
            .Where(p => p.Tournament.Id == tournamentId && p.Member.Id == memberId && p.Squad == squad)
            .Select(p => (int?)p.ExpectedPartnerCount)
            .FirstOrDefault() ?? 0;
    }

    public void UpsertPlan(int tournamentId, int memberId, int squad, int expectedPartnerCount)
    {
        using var db = dbFactory.CreateDbContext();
        UpsertPlan(db, tournamentId, memberId, squad, expectedPartnerCount);
        db.SaveChanges();
    }

    public void UpsertPlan(NineTapDb db, int tournamentId, int memberId, int squad, int expectedPartnerCount)
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
