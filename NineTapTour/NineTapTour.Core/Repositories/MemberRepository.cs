#nullable disable
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Core.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public MemberRepository(IDbContextFactory<NineTapDb> dbFactory)
    {
        this.dbFactory = dbFactory;
    }

    /// <summary>
    /// If the Member given is found in the database, updates that Memeber.
    /// Otherwise, adds the new Memeber to the database
    /// </summary>
    /// <exception cref="DbUpdateException"></exception>
    public void AddOrUpdateMember(Member temp)
    {
        try
        {
            using (var db = dbFactory.CreateDbContext())
            {
                // Members built from imports have Id 0 even when they already exist
                // in the database, so resolve identity by Number before deciding
                // between insert and update.
                if (temp.Id == 0)
                {
                    temp.Id = db.Members
                        .Where(m => m.Number == temp.Number)
                        .OrderBy(m => m.Id)
                        .Select(m => m.Id)
                        .FirstOrDefault();
                }

                bool doesMemberExist = temp.Id != 0 && db.Members.Any(m => m.Id == temp.Id);

                // Callers without a region UI (desktop app, imports) leave
                // RegionId at 0: keep the stored region on update so a region
                // chosen on the website is not wiped, and use the default on insert.
                if (doesMemberExist)
                {
                    if (temp.RegionId == 0)
                    {
                        temp.RegionId = db.Members
                            .Where(m => m.Id == temp.Id)
                            .Select(m => m.RegionId)
                            .First();
                    }
                    db.Entry(temp).State = EntityState.Modified;
                }
                else
                {
                    temp.Id = 0;
                    if (temp.RegionId == 0)
                    {
                        temp.RegionId = DefaultRegionResolver.ResolveDefaultRegionId(db);
                    }
                    db.Entry(temp).State = EntityState.Added;
                }

                if (temp.Average != null)
                {
                    temp.Handicap = Calculations.TournamentCalculations.CalculateHandicapPins(temp.Average.Value);
                }
                db.SaveChanges();
            }
        }
        catch (DbUpdateException)
        {
            throw;
        }
    }

    /// <summary>
    /// Returns true if the Member is found in the database by comparing Member Number
    /// </summary>
    public bool MemberExists(Member Temp)
    {
        using (var db = dbFactory.CreateDbContext())
        {
            return db.Members.Any(m => m.Number == Temp.Number);
        }
    }

    /// <summary>
    /// Returns a list of all of the members
    /// </summary>
    public List<Member> GetMemberList()
    {
        using (var db = dbFactory.CreateDbContext())
        {
            return [.. (from m in db.Members
                    orderby  m.Number
                    select m)];
        }
    }

    /// <summary>
    /// Returns a member with the given memberNumber
    /// </summary>
    public Member GetMember(int memberNumber)
    {
        using (var db = dbFactory.CreateDbContext())
        {
            return (from m in db.Members
                    where m.Number == memberNumber
                    select m).SingleOrDefault() ?? new Member();
        }
    }

    /// <summary>
    /// Returns a member with the same memberNumber, using the caller's context
    /// so multi-operation workflows can share one unit of work.
    /// </summary>
    public Member GetMember(int memberNumber, NineTapDb db)
    {
        return (from m in db.Members
                where m.Number == memberNumber
                select m).SingleOrDefault() ?? new Member();
    }

    /// <summary>
    /// Returns a member with the same gameID given
    /// </summary>
    public Member GetMemberByGameId(int gameID)
    {
        using (var db = dbFactory.CreateDbContext())
        {
            return db.Participants
                    .Include(b => b.Game)
                    .Include(b => b.Member)
                    .First(p => p.Game.Id == gameID)
                    .Member;
        }
    }

    /// <summary>
    /// Returns the ID of a member based on their Member Number
    /// </summary>
    public int GetMemberIdByNumber(int memberNumber)
    {
        using (var db = dbFactory.CreateDbContext())
        {
            return (from m in db.Members
                    where m.Number == memberNumber
                    select m.Id).SingleOrDefault();
        }
    }

    /// <summary>
    /// Returns a Member with the same memberID as the one given
    /// </summary>
    public int GetMemberNumberbyID(int memberID)
    {
        Member currentMember = new();
        using (var db = dbFactory.CreateDbContext())
        {
            var temp = (from m in db.Members
                        where m.Id == memberID
                        select new
                        {
                            m.Number,
                        });
            foreach (var c in temp)
            {
                currentMember.Number = c.Number;
            }
            return currentMember.Number;
        }
    }

    /// <summary>
    /// Returns the highest Member Number, or 0 if there are no members
    /// </summary>
    public int GetLastMemberNumber()
    {
        using var db = dbFactory.CreateDbContext();
        return db.Members
            .Max(m => (int?)m.Number) ?? 0;
    }

    /// <summary>
    /// Member search moved verbatim from FrmSearch.btnSearch_Click.
    /// </summary>
    public List<Member> Search(Models.MemberSearchCriteria criteria)
    {
        using var db = dbFactory.CreateDbContext();
        IQueryable<Member> query = db.Members.Include(m => m.Region).AsNoTracking();

        if (criteria.Number.HasValue)
        {
            int number = criteria.Number.Value;
            query = query.Where(m => m.Number == number);
        }

        if (!string.IsNullOrWhiteSpace(criteria.FirstName))
        {
            string first = criteria.FirstName.ToLower().Trim();
            query = query.Where(m => m.FirstName.ToLower().Contains(first));
        }

        if (!string.IsNullOrWhiteSpace(criteria.LastName))
        {
            string last = criteria.LastName.ToLower().Trim();
            query = query.Where(m => m.LastName.ToLower().Contains(last));
        }

        if (criteria.IsActive.HasValue)
        {
            bool isActive = criteria.IsActive.Value;
            query = query.Where(m => m.IsActive == isActive);
        }

        if (criteria.Average.HasValue)
        {
            int average = criteria.Average.Value;
            query = query.Where(m => m.Average == average);
        }

        if (criteria.Handicap.HasValue)
        {
            int handicap = criteria.Handicap.Value;
            query = query.Where(m => m.Handicap == handicap);
        }

        if (criteria.Bonus.HasValue)
        {
            int bonus = criteria.Bonus.Value;
            query = query.Where(m => m.Bonus == bonus);
        }

        return [.. query.OrderBy(m => m.Number)];
    }

    /// <summary>
    /// Deactivation candidates moved from FrmUpdateActiveMem.UpdateList.
    /// </summary>
    public List<Member> GetInactiveCandidates(System.DateTime lastBowledOnOrBefore)
    {
        using var db = dbFactory.CreateDbContext();
        return [.. db.Members
            .AsNoTracking()
            .Where(m => m.IsActive && (m.LastBowled == null || m.LastBowled <= lastBowledOnOrBefore))
            .OrderBy(m => m.Number)];
    }

    public int SetInactive(IEnumerable<int> memberIds)
    {
        List<int> ids = [.. memberIds];
        using var db = dbFactory.CreateDbContext();
        List<Member> members = [.. db.Members.Where(m => ids.Contains(m.Id))];
        foreach (Member member in members)
        {
            member.IsActive = false;
        }
        db.SaveChanges();
        return members.Count;
    }
}
