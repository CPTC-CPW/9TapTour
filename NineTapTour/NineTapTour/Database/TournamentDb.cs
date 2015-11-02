using NineTapTour.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
                    //checks if tournament is new or already existing in db
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

        public static void AddMemberToTournament(Participant player)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    //Adds player inside NineTapDb
                    db.Participants.Add(player);
                    //Uses AddObject because you cannot have object graph where part of objects are connected to context and part of not.
                    //Changed so that context knows that department already exists.
                    var manager = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager;
                    manager.ChangeObjectState(player.Game.Member,
                                                EntityState.Unchanged);
                    manager.ChangeObjectState(player.Member, EntityState.Unchanged);
                    db.SaveChanges();
                    
                }
            }
            catch (SqlException ex)
            {
                
                throw new MemberTableException("Error number : " + ex.Number + " - " + ex.Message);
            }
        }
    }
}
