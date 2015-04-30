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
                newMember.Number = temp.Number; 
                newMember.LastName = temp.LastName; 
                newMember.FirstName = temp.FirstName;
                newMember.MiddleInitial = temp.MiddleInitial;
                newMember.IsActive = temp.IsActive; 
                newMember.IsSenior = temp.IsSenior; 
                newMember.Gender = temp.Gender; 
                newMember.Notes = temp.Notes;
                newMember.Street = temp.Street;
                newMember.Email = temp.Email;
                newMember.City = temp.City;
                newMember.PostalCode = temp.PostalCode;
                newMember.JoinDate = temp.JoinDate;
                newMember.Referrals = temp.Referrals;
                newMember.PrimaryPhone = temp.PrimaryPhone;
                newMember.SecondaryPhone = temp.SecondaryPhone;

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
