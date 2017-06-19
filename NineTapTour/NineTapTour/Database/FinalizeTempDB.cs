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
                    //checks if tournament is new or already existing in db
                    if (!db.FinalizeTemp.Any(f => f.GameId == temp.GameId))
                    {
                        db.Entry(temp).State = EntityState.Added;
                        /*************************************************************************
                        updates the handicap of a member that participated in the tournament in the database 
                        ***There is a problem in the database's member's average, so it was not 
                           used, but I believe it should be
                           -The problem might be when a tournament record is added, it is not 
                           updating the member's average in the database.
                        *************************************************************************/
                        db.Members.First(x => x.Id == temp.MemberId).Handicap = Calculations.Calculations.CalculateHandicapPins(Convert.ToInt16(LeagueAverage(db.Members.First(x => x.Id == temp.MemberId))));
                        /************************************************************************/
                        db.SaveChanges();
                    }

                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error Number : " + ex.Number + " - " + ex.Message);
            }
        }
        /***************************************************************
        calculates the average
        ***note I saw this method twice now and this is the third one
        ****************************************************************/
        public static double LeagueAverage(Member mem)
        {
            double sum = 0;
            double average = 0;
            var db = new NineTapDb();
            var temp = (

                        from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        join g in db.Games on p.Game.Id equals g.Id
                        join t in db.Tournaments on p.Tournament.Id equals t.Id
                        where mem.Id == m.Id
                        orderby t.Date descending
                        select new
                        {
                            t.Date,
                            g.Game1,
                            g.Game2,
                            g.Game3,
                            g.Game4,
                            Average = (g.Game1 + g.Game2 + g.Game3 + g.Game4) / 4

                        }).Take(30).ToList();
            if (temp.Count > 0)
            {
                foreach (var item in temp)
                {
                    sum += Convert.ToDouble(item.Average);
                }
                return (average = sum / temp.Count());
            }
            return 0;
        }
        /*****************/
    }
}
