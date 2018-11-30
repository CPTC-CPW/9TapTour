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
    public class MemberDb
    {
        public static void AddMember(Member temp)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(temp).State = db.Members.Any(m => m.Id == temp.Id) ?
                                            EntityState.Modified :
                                            EntityState.Added;
                    /********************************************************************************************
                    the if statement is so that you can update the handicap by changing the league average,
                        but it won't update if a member participated in a tournament
                    .value solves the problem where startAvg is nullable but the method is just int not int?
                    *********************************************************************************************/
                    if (temp.Average == 0)
                    {
                        temp.Handicap = Calculations.Calculations.CalculateHandicapPins((temp.StartAvg.Value));
                    }
                    /********************************************************************************************/
                    if (db.Entry(temp).State == EntityState.Modified)
                    {
                        temp.Handicap = Calculations.Calculations.CalculateHandicapPins((temp.StartAvg.Value));
                        //MessageBox.Show("Player Updated");
                    }
                    else
                    {
                        // MessageBox.Show("Player Saved Successfully");
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
        public static bool MemberExists(Member Temp)
        {


            using (var db = new NineTapDb())
            {

                if (db.Members.Any(m => m.Number == Temp.Number && m.NineTapRegionID == Temp.NineTapRegionID))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }
        public static List<Member> GetMemberList(int RegionID)
        {
            using (var db = new NineTapDb())
            {
                return (from m in db.Members
                        orderby m.Number
                        where m.NineTapRegionID == RegionID
                        select m).ToList();
            }
        }

        /// <summary>
        /// Get the number of Members in a particular region
        /// </summary>
        public static int GetMemberListCount(int regionId)
        {
            NineTapDb db = new NineTapDb();
            return db.Members.Where(member => member.NineTapRegionID == regionId).Count();
        }

        public static List<Member> GetALLMembersList()
        {
            using (var db = new NineTapDb())
            {
                return (from m in db.Members
                        orderby m.Number
                        select m).ToList();
            }
        }

        public static void DeleteMember(Member remove)
        {
            using (var db = new NineTapDb())
            {
                db.Entry(remove).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        public static Member GetMember(int memNumber, int RegionID)
        {
            Member currentMember = new Member();
            using (var db = new NineTapDb())
            {
                var temp = (from m in db.Members
                            where m.Number == memNumber && m.NineTapRegionID == RegionID
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
        /// Returns the ID of a member based on their Member Number
        /// </summary>
        public static int GetMemberIdByNumber(int memberNumber, int regionId, NineTapDb db)
        {
            return (from m in db.Members
                    where m.Number == memberNumber &&
                        m.NineTapRegionID == regionId
                    select m.Id).SingleOrDefault();
        }

        public static int GetMemberNumberbyID(int MemberID)
        {
            Member currentMember = new Member();
            using (var db = new NineTapDb())
            {
                var temp = (from m in db.Members
                            where m.Id == MemberID
                            select new
                            {
                                m.Number,
                            });
                foreach (var c in temp)
                {

                    currentMember.Number = c.Number;
                    ;

                }


                return currentMember.Number;


            }
        }

    }

}

