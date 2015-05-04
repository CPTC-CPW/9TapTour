using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Database
{
    class MemberDB
    {
        public static Boolean addMember(Member temp)
        {
            using (NineTapDb db = new NineTapDb())
            {
                Member newMember = new Member();
                #region Personal Info
                newMember.Number = temp.Number; 
                newMember.LastName = temp.LastName; 
                newMember.FirstName = temp.FirstName;
                newMember.MiddleInitial = temp.MiddleInitial;
                newMember.DateOfBirth = temp.DateOfBirth;
                newMember.SSN = temp.SSN;
                #endregion

                #region Postal Address 
                newMember.Street = temp.Street;
                newMember.City = temp.City;
                newMember.State = temp.State;
                newMember.PostalCode = temp.PostalCode;
                #endregion

                #region Contact Info
                newMember.Email = temp.Email;
                newMember.PrimaryPhone = temp.PrimaryPhone;
                newMember.SecondaryPhone = temp.SecondaryPhone;
                #endregion

                #region Score Info
                newMember.Average = temp.Average;
                newMember.Handicap = temp.Handicap;
                newMember.Bonus = temp.Bonus;
                #endregion

                #region Misc. Info
                newMember.JoinDate = temp.JoinDate;
                newMember.RejoinDate = temp.RejoinDate;
                newMember.LastBowled = temp.LastBowled;
                newMember.MoneyEarned = temp.MoneyEarned;
                newMember.Notes = temp.Notes;
                newMember.Referrals = temp.Referrals;
                newMember.IsSenior = temp.IsSenior; 
                newMember.IsActive = temp.IsActive; 
                newMember.Gender = temp.Gender;
                #endregion
                db.Members.Add(newMember);
                db.SaveChanges();
                return true;
            }
            
        }

        public static List<Member> getMember()
        {
            using (NineTapDb db = new NineTapDb())
            {
                List<Member> Members = (from m in db.Members
                                        select m).ToList();
                return Members;
            }
        }



        //public static Member getMember(int ID)
        //{
        //    using (NineTapDb db = new NineTapDb())
        //    {
        //        var currentMem = from m in db.Members
        //                         where ID == m.Number
        //                         select m;

        //        if (!currentMem.Any())
        //        {
        //            Member newMember = new Member();
        //            return newMember;
        //        }
        //        return (Member)currentMem.First();
        //    }
        //}



    }
}
