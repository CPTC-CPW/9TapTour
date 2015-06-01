using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineTapTour.Exceptions;
using System.Data.SqlClient;

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
                db.SaveChanges();
            }

            }
            catch(SqlException ex)
            {
                throw new MemberTableException("Error Number : " + ex.Number + " - " + ex.Message);
            }
           
       
        }

        public static List<Member> GetMemberList()
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
            using(var db = new NineTapDb())
            {
                db.Entry(remove).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }
    }
}
