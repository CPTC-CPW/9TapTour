using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineTapTour.Exceptions;
using System.Data.SqlClient;
using System.Windows.Forms;
using NineTapTour.Models;
using Microsoft.EntityFrameworkCore;

namespace NineTapTour.Database
{
    public class MemberDB
    {
        /// <summary>
        /// If the Member given is found in the database, updates that Memeber. 
        /// Otherwise, adds the new Memeber to the database
        /// </summary>
        public static void AddOrUpdateMember(Member temp)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(temp).State = db.Members.Any(m => m.Id == temp.Id) ?
                                            EntityState.Modified :
                                            EntityState.Added;
                    /* The if statement is so that you can update the handicap by changing the league average,
                     but it won't update if a member participated in a tournament, .Value solves the problem 
                     where startAvg is nullable but the method is just int not int? */
                    if (temp.Average == 0)
                    {
                        temp.Handicap = Calculations.Calculations.CalculateHandicapPins((temp.StartAvg.Value));
                    }
                    if (db.Entry(temp).State == EntityState.Modified)
                    {
                        temp.Handicap = Calculations.Calculations.CalculateHandicapPins((temp.StartAvg.Value));
#if DEBUG
                        // For debugging purposes will send PLAYER UPDATED to the Console
                        // DO NOT USE Messagebox as it bogs down the program
                        Console.WriteLine("Player Updated");
#endif
                    }
                    else
                    {
#if DEBUG
                        // For debugging purposes will send PLAYER SAVED SUCCESSFULLY to the Console
                        // DO NOT USE Messagebox as it bogs down the program
                        Console.WriteLine("Player Saved Successfully");
#endif
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
        /// Returns true if the Member is found in the database by comparing Member Number and RegionID
        /// </summary>
        public static bool MemberExists(Member Temp)
        {
            using (var db = new NineTapDb())
            {
                return db.Members.Any(m => m.Number == Temp.Number && m.NineTapRegionID == Temp.NineTapRegionID);
            }
        }

        /// <summary>
        /// Returns a list of all of the members with the same regionID as the one given
        /// </summary>
        public static List<Member> GetMemberList(int regionID)
        {
            using (var db = new NineTapDb())
            {
                return (from m in db.Members
                        orderby  m.Number
                        where m.NineTapRegionID == regionID
                        select m).ToList();
            }
        }

        /// <summary>
        /// Get the number of Members in the same region as the regionID given
        /// </summary>
        public static int GetMemberListCount(int regionId)
        {
            using (NineTapDb db = new NineTapDb())
            {
                return db.Members.Where(member => member.NineTapRegionID == regionId).Count();
            }
        }

        /// <summary>
        /// Returns a list of all Members
        /// </summary>
        public static List<Member> GetAllMembersList()
        {
            using (var db = new NineTapDb())
            {
                return (from m in db.Members
                        orderby m.Number
                        select m).ToList();
            }
        }

        /// <summary>
        /// Deletes the Member given from the database
        /// </summary>
        public static void DeleteMember(Member mem)
        {
            using (var db = new NineTapDb())
            {
                db.Entry(mem).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Returns a member with the same memberNumber and regionID given
        /// </summary>
        public static Member GetMember(int memberNumber, int regionID)
        {
            using (var db = new NineTapDb())
            {
                return (from m in db.Members
                            where m.Number == memberNumber && m.NineTapRegionID == regionID
                            select m).SingleOrDefault() ?? new Member();
            }
        }

        /// <summary>
        /// Returns a member with the same gameID given
        /// </summary>
        public static Member GetMemberByGameId(int gameID)
        {
            using(NineTapDb db = new NineTapDb())
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
        public static int GetMemberIdByNumber(int memberNumber, int regionId)
        {
            using(NineTapDb db = new NineTapDb())
            {
                 return (from m in db.Members
                        where m.Number == memberNumber &&
                            m.NineTapRegionID == regionId
                        select m.Id).SingleOrDefault();
            }
           
        }

        /// <summary>
        /// Returns a Member with the same memberID as the one given
        /// </summary>
        public static int GetMemberNumberbyID(int memberID)
        {
            Member currentMember = new Member();
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
    }
}

