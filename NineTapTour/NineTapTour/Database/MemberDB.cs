using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using NineTapTour.Models;
using Microsoft.EntityFrameworkCore;

namespace NineTapTour.Database;

public class MemberDB
{
    /// <summary>
    /// If the Member given is found in the database, updates that Memeber. 
    /// Otherwise, adds the new Memeber to the database
    /// </summary>
    /// <exception cref="DbUpdateException"></exception>
    public static void AddOrUpdateMember(Member temp)
    {
        try
        {
            using (var db = new NineTapDb())
            {
                bool doesMemberExist = db.Members.Any(m => m.Id == temp.Id);

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
                    temp.Handicap = Calculations.Calculations.CalculateHandicapPins(temp.Average.Value);
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
    public static bool MemberExists(Member Temp)
    {
        using (var db = new NineTapDb())
        {
            return db.Members.Any(m => m.Number == Temp.Number);
        }
    }

    /// <summary>
    /// Returns a list of all of the members
    /// </summary>
    public static List<Member> GetMemberList()
    {
        using (var db = new NineTapDb())
        {
            return [.. (from m in db.Members
                    orderby  m.Number
                    select m)];
        }
    }

    /// <summary>
    /// Returns a member with the given memberNumber
    /// </summary>
    public static Member GetMember(int memberNumber)
    {
        using (var db = new NineTapDb())
        {
            return (from m in db.Members
                    where m.Number == memberNumber
                    select m).SingleOrDefault() ?? new Member();
        }
    }

    /// <summary>
    /// Returns a member with the same memberNumber
    /// </summary>
    public static Member GetMember(int memberNumber, NineTapDb db)
    {
        return (from m in db.Members
                where m.Number == memberNumber
                select m).SingleOrDefault() ?? new Member();
    }

    /// <summary>
    /// Returns a member with the same gameID given
    /// </summary>
    public static Member GetMemberByGameId(int gameID)
    {
        using (NineTapDb db = new())
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
    public static int GetMemberIdByNumber(int memberNumber)
    {
        using (NineTapDb db = new())
        {
            return (from m in db.Members
                    where m.Number == memberNumber
                    select m.Id).SingleOrDefault();
        }

    }

    /// <summary>
    /// Returns a Member with the same memberID as the one given
    /// </summary>
    public static int GetMemberNumberbyID(int memberID)
    {
        Member currentMember = new();
        using (var db = new NineTapDb())
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
    /// <param name="regionID"></param>
    /// <returns></returns>
    public static int GetLastMemberNumber()
    {
        using var db = new NineTapDb();
        return db.Members
            .Max(m => (int?)m.Number) ?? 0;
    }
}
