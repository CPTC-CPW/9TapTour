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

                if (doesMemberExist)
                {
                    db.Entry(temp).State = EntityState.Modified;
                }
                else
                {
                    temp.Id = 0;
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
}
