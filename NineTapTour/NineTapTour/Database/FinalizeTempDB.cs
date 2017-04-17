using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Database
{
    class FinalizeTempDB
    {
        public static void AddFinalizeTemp(FinalizeTemp temp)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    /***********************************************************************************
                    the code that should change the handicap of the members
                    it is not updating because I believe there is a problem in the database where finalize is not connected to members
                    ************************************************************************************/
                    //temp.Handicap = Calculations.Calculations.CalculateHandicapPins(temp.AdjustedAvg);
                    /***********************************************************************************/
                    //checks if tournament is new or already existing in db
                    if (!db.FinalizeTemp.Any(f => f.GameId == temp.GameId))
                    {
                        db.Entry(temp).State = EntityState.Added;
                        db.SaveChanges();
                    }

                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error Number : " + ex.Number + " - " + ex.Message);
            }
        }
    }
}
