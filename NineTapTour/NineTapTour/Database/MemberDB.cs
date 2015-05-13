using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Database
{
    class MemberDb
    {
        public static bool AddMember(Member temp)
        {
            using (var db = new NineTapDb())
            {
                db.Members.Add(temp);
                db.SaveChanges();
                return true;
            }
            
        }

        public static List<Member> GetMemberList()
        {
            using (var db = new NineTapDb())
            {
                return (from m in db.Members
                        select m).ToList();
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
