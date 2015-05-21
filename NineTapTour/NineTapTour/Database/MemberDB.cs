using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                db.Entry(temp).State = db.Members.Any(m => m.Id == temp.Id) ?
                                        EntityState.Modified :
                                        EntityState.Added;
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
    }
}
