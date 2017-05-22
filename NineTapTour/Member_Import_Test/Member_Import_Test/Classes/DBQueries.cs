using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineTapTour.Database;

namespace Member_Import_Test.Classes
{
    class DBQueries
    {
        public static void AddMember(Member temp)
        {

            using (var db = new MembersDB())
            {
                db.Entry(temp).State = db.Members.Any(m => m.Id == temp.Id) ?
                                        EntityState.Modified :
                                        EntityState.Added;
                db.SaveChanges();
            }
        }

        //method to check if the user aleady exists in the database when trying to add from member file
        public static bool MemberExists(Member Temp)
        {
            using (var db = new MembersDB())
            {
                if(db.Members.Any(m => m.Number == Temp.Number))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
           
    }
}
