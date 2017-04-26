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
                        MessageBox.Show("Player Updated");
                    }
                    else
                    {
                        MessageBox.Show("Player Saved Successfully");
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
                // throw new MemberTableException("Error Number : " + ex.Number + " - " + ex.Message);
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
