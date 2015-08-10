using NineTapTour.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Database
{
    class TournamentDb
    {
        public static void AddTournament(Tournament New)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(New).State = db.Tournaments.Any(t => t.Id == New.Id) ?
                        EntityState.Modified :
                        EntityState.Added;
                    db.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                throw new TournamentTableException("Error Number : " + ex.Number + " - " + ex.Message);
            }
        }

        public static List<Tournament> GetTournamentList()
        {
            using (var db = new NineTapDb())
            {
                return (from t in db.Tournaments
                        orderby t.Date
                        select t).ToList();
            }
        }
    }
}
