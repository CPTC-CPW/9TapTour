using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineTapTour.Exceptions;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
using System.Windows.Forms;
using NineTapTour.Models;


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
                        MessageBox.Show("Player Updated");
#endif
                    }
                    else
                    {
#if DEBUG
                        MessageBox.Show("Player Saved Successfully");
#endif
                    }
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                Exception raise = ex;
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }

            }
            catch (SystemException ex)
            {
                Console.WriteLine("Error Number : " + ex.Message);
                // throw new MemberTableException("Error Number : " + ex.Number + " - " + ex.Message);
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
            NineTapDb db = new NineTapDb();
            return db.Members.Where(member => member.NineTapRegionID == regionId).Count();
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
            Member currentMember = new Member();
            using (var db = new NineTapDb())
            {
                var temp = (from m in db.Members
                            where m.Number == memberNumber && m.NineTapRegionID == regionID
                            select new
                            {
                                m.Average,
                                m.Bonus,
                                m.City,
                                m.DateOfBirth,
                                m.Email,
                                m.FirstName,
                                m.Gender,
                                m.Handicap,
                                m.Id,
                                m.IsActive,
                                m.IsLifetimeMember,
                                m.IsSenior,
                                m.JoinDate,
                                m.LastBowled,
                                m.LastName,
                                m.LastPayment,
                                m.MiddleInitial,
                                m.MoneyEarned,
                                m.Notes,
                                m.Number,
                                m.PostalCode,
                                m.PrimaryPhone,
                                m.Referrals,
                                m.RejoinDate,
                                m.SecondaryPhone,
                                m.SSN,
                                m.StartAvg,
                                m.State,
                                m.Street,
                                m.NineTapRegionID
                            });
                foreach (var c in temp)
                {
                    currentMember.Average = c.Average;
                    currentMember.Bonus = c.Bonus;
                    currentMember.City = c.City;
                    currentMember.DateOfBirth = c.DateOfBirth;
                    currentMember.Email = c.Email;
                    currentMember.FirstName = c.FirstName;
                    currentMember.Gender = c.Gender;
                    currentMember.Handicap = c.Handicap;
                    currentMember.Id = c.Id;
                    currentMember.IsActive = c.IsActive;
                    currentMember.IsLifetimeMember = c.IsLifetimeMember;
                    currentMember.IsSenior = c.IsSenior;
                    currentMember.JoinDate = c.JoinDate;
                    currentMember.LastBowled = c.LastBowled;
                    currentMember.LastName = c.LastName;
                    currentMember.LastPayment = c.LastPayment;
                    currentMember.MiddleInitial = c.MiddleInitial;
                    currentMember.MoneyEarned = c.MoneyEarned;
                    currentMember.Notes = c.Notes;
                    currentMember.Number = c.Number;
                    currentMember.PostalCode = c.PostalCode;
                    currentMember.PrimaryPhone = c.PrimaryPhone;
                    currentMember.Referrals = c.Referrals;
                    currentMember.RejoinDate = c.RejoinDate;
                    currentMember.SecondaryPhone = c.SecondaryPhone;
                    currentMember.SSN = c.SSN;
                    currentMember.StartAvg = c.StartAvg;
                    currentMember.State = c.State;
                    currentMember.Street = c.Street;
                    currentMember.NineTapRegionID = c.NineTapRegionID;
                }
                return currentMember;
            }
        }

        /// <summary>
        /// Returns a member with the same gameID given
        /// </summary>
        public static Member GetMemberByGameId(int gameID)
        {
            return new NineTapDb().Participants
                                  .Include(b => b.Game)
                                  .Include(b => b.Member)
                                  .First(p => p.Game.Id == gameID)
                                  .Member;
        }
            
        /// <summary>
        /// Returns the ID of a member based on their Member Number
        /// </summary>
        public static int GetMemberIdByNumber(int memberNumber, int regionId, NineTapDb db)
        {
            return (from m in db.Members
                    where m.Number == memberNumber &&
                        m.NineTapRegionID == regionId
                    select m.Id).SingleOrDefault();
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

