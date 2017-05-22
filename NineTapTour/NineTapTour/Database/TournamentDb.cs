using NineTapTour.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;


namespace NineTapTour.Database
{
    //remove public as soon as import project is merged with 9tap project
    public class TournamentDb
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

        /// <summary>
        /// Updates an existing tournament
        /// </summary>
        /// <param name="New"></param> 
        
        public static bool UpdateTournament(Tournament newTour)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    var original = db.Tournaments.Find(newTour.Id);
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(newTour);
                        db.SaveChanges();
                    } else
                    {
                        throw new TournamentTableException("The original data could not be found.");
                    }
                }
                return true;
            }
            catch (SqlException ex)
            {
                throw new TournamentTableException("Error Number : " + ex.Number + " - " + ex.Message);
            }
        }

        /// <summary>
        /// returns the list of tournaments in descending order by date
        /// </summary>
        /// <returns></returns>
        public static List<Tournament> GetTournamentList()
        {
            using (NineTapDb db = new NineTapDb())
            {
                return (from t in db.Tournaments
                        orderby t.Date descending
                        select t).ToList();
            }
        }

        public static List<Participant> GetTournamentMemberList(Tournament currTourney)
        {
            
            using (NineTapDb db = new NineTapDb())
            {
                return (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        where p.Tournament.Id == currTourney.Id
                        select p).Include(m =>m.Member).ToList();
            }
        }

        public static List<Member> GetUniqueTourMembers(Tournament currTourney)
        {

            using (NineTapDb db = new NineTapDb())
            {
                return (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        where p.Tournament.Id == currTourney.Id
                        select m).Distinct().ToList();
            }
        }

        public static List<Member> GetUniqueTourMembersByDate(DateTime start, DateTime end)
        {

            using (NineTapDb db = new NineTapDb())
            {
                return (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        where p.Tournament.Date >= start && p.Tournament.Date <= end
                        select m).Distinct().ToList();
            }
        }

        public static void AddMemberToTournament(Participant player)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    var check = (from p in db.Participants
                                 where player.Member.Id == p.Member.Id
                                 && player.Tournament.Id == p.Tournament.Id
                                 && player.Squad == p.Squad
                                 select p).Count();
                    if (check == 0)
                    {
                        
                        
                        db.Participants.Add(player);
                        //Uses AddObject because you cannot have object graph where part of objects are connected to context and part of not.
                        //Changed so that context knows that department already exists.
                        var manager = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager;
                        manager.ChangeObjectState(player.Tournament, EntityState.Unchanged);
                        manager.ChangeObjectState(player.Member, EntityState.Unchanged);
                        db.SaveChanges();
                    }
                    else
                    {
                        try
                        {
                            System.Data.Entity.Core.Objects.ObjectStateManager manager = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager;
                            Game result = db.Games.SingleOrDefault(g => g.Id == player.Game.Id);
                            Participant squadResult = db.Participants.SingleOrDefault(p => p.Id == player.Id);
                            Participant memberQuery = db.Participants.Include(m => m.Member)
                                .Where(m => m.Member.Id == player.Member.Id).FirstOrDefault();
                            result.Game1 = player.Game.Game1;
                            result.Game2 = player.Game.Game2;
                            result.Game3 = player.Game.Game3;
                            result.Game4 = player.Game.Game4;
                            result.MoneyWon = player.Game.MoneyWon;

                            if (squadResult == null)
                            {
                                squadResult = new Participant();
                                Console.WriteLine("No squad");
                            }
                            squadResult.Squad = player.Squad;
                            squadResult.Member = memberQuery.Member;
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException ex)
                        {
                            // Retrieve the error messages as a list of strings.
                            var errorMessages = ex.EntityValidationErrors
                                    .SelectMany(x => x.ValidationErrors)
                                    .Select(x => x.ErrorMessage);

                            // Join the list to a single string.
                            var fullErrorMessage = string.Join("; ", errorMessages);

                            // Combine the original exception message with the new one.
                            var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                            // Throw a new DbEntityValidationException with the improved exception message.
                            throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                        }
                        
                    }
                    //Adds player inside NineTapDb
                    
                }
            }
            catch (SqlException ex)
            {
                
                throw new MemberTableException("Error number : " + ex.Number + " - " + ex.Message);
            }
        }
    }
}
