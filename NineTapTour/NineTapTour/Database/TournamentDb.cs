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
using NineTapTour.Models;


namespace NineTapTour.Database
{
    //remove public as soon as import project is merged with 9tap project
    public class TournamentDB
    {
        /// <summary>
        /// Adds the given Tournament into the database
        /// </summary>
        public static void AddTournament(Tournament tourn)
        {
            try
            {
                using (var db = new NineTapDB())
                {
                    //checks if tournament is new or already existing in db
                    db.Entry(tourn).State = db.Tournaments.Any(t => t.Id == tourn.Id) ?
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
        /// Updates the given Tournament in the database
        /// </summary>
        public static bool UpdateTournament(Tournament tourn)
        {
            try
            {
                using (var db = new NineTapDB())
                {
                    Tournament original = db.Tournaments.Find(tourn.Id);
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(tourn);
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
        /// Returns the list of Tournaments ordered by Date descending
        /// </summary>
        public static List<Tournament> GetTournamentList(int regionID)
        {
            using (NineTapDB db = new NineTapDB())
            {
                return (from t in db.Tournaments
                        orderby t.Date descending
                        where t.TourneyRegion == regionID
                        select t).ToList();
            }
        }

        /// <summary>
        /// Returns a list of Participants within the given Tournament
        /// </summary>
        public static List<Participant> GetTournamentMemberList(Tournament tourn)
        {
            using (NineTapDB db = new NineTapDB())
            {
                return (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        where p.Tournament.Id == tourn.Id
                        select p).Include(m =>m.Member).ToList();
            }
        }

        /// <summary>
        /// Returns a list of Participants within the given Tournament
        /// orders by MemberID
        /// </summary>
        public static List<Participant> GetTournamentMemberListInOrder(Tournament tourn)
        {
            using (NineTapDB db = new NineTapDB())
            {
                return (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        orderby p.Member.Id
                        where p.Tournament.Id == tourn.Id
                        select p).Include(m => m.Member).ToList();
            }
        }

        /// <summary>
        /// Get the total number of Participants in the given Tournament
        /// </summary>
        public static int GetTotalNumberParticipantsInTournament(Tournament tourn)
        {
            NineTapDB db = new NineTapDB();
            return db.Participants
                .Where(p => p.Tournament.Id == tourn.Id)
                .Count();
        }

        /// <summary>
        /// Returns a list of unique Members in the given Tournament
        /// </summary>
        public static List<Member> GetUniqueTourMembers(Tournament tourn)
        {
            using (NineTapDB db = new NineTapDB())
            {
                return (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        where p.Tournament.Id == tourn.Id
                        select m).Distinct().ToList();
            }
        }

        /// <summary>
        /// Returns a list of unique Members that were in a Tournament within the given dates inclusively
        /// </summary>
        public static List<Member> GetUniqueTourMembersByDate(DateTime start, DateTime end)
        {
            using (NineTapDB db = new NineTapDB())
            {
                return (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        where p.Tournament.Date >= start && p.Tournament.Date <= end
                        select m).Distinct().ToList();
            }
        }

        /// <summary>
        /// Adds the given Participant to the Tournament
        /// </summary>
        public static void AddMemberToTournament(Participant player)
        {
            try
            {
                using (var db = new NineTapDB())
                {
                    int check = (from p in db.Participants
                                 where player.Member.Id == p.Member.Id
                                 && player.Tournament.Id == p.Tournament.Id
                                 && player.Squad == p.Squad
                                 select p).Count();
                    if (check == 0)
                    {
                        db.Participants.Add(player);
                        /* Uses AddObject because you cannot have object graph where part of objects are connected to context and part of not.
                         Changed so that context knows that department already exists. */
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
                            result.IsComp = player.Game.IsComp;

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
                            string fullErrorMessage = string.Join("; ", errorMessages);

                            // Combine the original exception message with the new one.
                            string exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

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

        /// <summary>
        /// Returns a Tournament from the database with the same TournamentID given,
        /// returns null if one was not found
        /// </summary>
        public static Tournament GetTourneyByID(int tournID)
        {
            using (NineTapDB db = new NineTapDB())
            {
                Tournament tournament =
                    (from g in db.Tournaments
                     where g.Id == tournID
                     select g).SingleOrDefault();
                return tournament;
            }
        }

        public static List<Member> GetAllActiveMembers()
        {
            using (NineTapDB db = new NineTapDB())
            {
                List<Member> activeMembers =
                        (from active in db.Members
                         where active.IsActive == true
                         select active).ToList();
                return activeMembers;
            }
        }

        /// <summary>
        /// Deletes the given Tournament from the database
        /// </summary>
        public static void DeleteTournament(Tournament tourn)
        {
            using (var db = new NineTapDB())
            {
                db.Entry(tourn).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }
    }
}

