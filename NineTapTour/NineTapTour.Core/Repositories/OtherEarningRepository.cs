#nullable disable
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Core.Repositories;

public class OtherEarningRepository : IOtherEarningRepository
{
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public OtherEarningRepository(IDbContextFactory<NineTapDb> dbFactory)
    {
        this.dbFactory = dbFactory;
    }

    /// <inheritdoc />
    public List<OtherEarning> GetForMember(int memberNumber)
    {
        using var db = dbFactory.CreateDbContext();
        return db.OtherEarnings
            .AsNoTracking()
            .Where(o => o.Member.Number == memberNumber)
            .OrderBy(o => o.Date == null)
            .ThenBy(o => o.Date)
            .ThenBy(o => o.Id)
            .ToList();
    }

    /// <inheritdoc />
    public void SaveForMember(int memberNumber, List<OtherEarning> rows)
    {
        using var db = dbFactory.CreateDbContext();

        Member member = db.Members.FirstOrDefault(m => m.Number == memberNumber)
            ?? throw new ArgumentException($"Member #{memberNumber} was not found.", nameof(memberNumber));

        List<OtherEarning> existing = db.OtherEarnings
            .Where(o => o.Member.Id == member.Id)
            .ToList();

        HashSet<int> keptIds = [.. rows.Where(r => r.Id != 0).Select(r => r.Id)];
        db.OtherEarnings.RemoveRange(existing.Where(o => !keptIds.Contains(o.Id)));

        foreach (OtherEarning row in rows)
        {
            OtherEarning target = row.Id == 0 ? null : existing.FirstOrDefault(o => o.Id == row.Id);
            if (target == null)
            {
                db.OtherEarnings.Add(new OtherEarning
                {
                    Member = member,
                    Amount = row.Amount,
                    Description = row.Description,
                    Date = row.Date,
                    IsImported = false,
                });
                continue;
            }

            target.Amount = row.Amount;
            target.Description = row.Description;
            target.Date = row.Date;
        }

        db.SaveChanges();
    }
}
