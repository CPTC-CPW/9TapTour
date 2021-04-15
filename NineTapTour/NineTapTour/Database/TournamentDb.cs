using NineTapTour.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineTapTour.Models;
using NineTapTour.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

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
                using (var db = new NineTapDb())
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
                using (var db = new NineTapDb())
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
            using (NineTapDb db = new NineTapDb())
            {
                return (from t in db.Tournaments
                        orderby t.Date descending
                        where t.TourneyRegion == regionID
                        select t).ToList();
            }
        }

        /// <summary>
        /// Returns a list of Participants within the given Tournament. Participants
        /// are returned in the order they were entered in the tournament (by participant id)
        /// </summary>
        public static List<Participant> GetTournamentMemberList(Tournament tourn)
        {
            using (NineTapDb db = new NineTapDb())
            {
                return (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        orderby p.Id
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
            using (NineTapDb db = new NineTapDb())
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
            NineTapDb db = new NineTapDb();
            return db.Participants
                .Where(p => p.Tournament.Id == tourn.Id)
                .Count();
        }

        /// <summary>
        /// Returns a list of unique Members in the given Tournament
        /// </summary>
        public static List<Member> GetUniqueTourMembers(Tournament tourn)
        {
            using (NineTapDb db = new NineTapDb())
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
            using (NineTapDb db = new NineTapDb())
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
                using (var db = new NineTapDb())
                {
                    int check = (from p in db.Participants
                                 where player.Member.Id == p.Member.Id
                                 && player.Tournament.Id == p.Tournament.Id
                                 && player.Squad == p.Squad
                                 select p).Count();
                    if (check == 0)
                    {
                        db.Participants.Add(player);
                        // We only want to add a the current person. Tournament and Member data is not changed here.
                        db.Entry(player.Tournament).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                        db.Entry(player.Member).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                        db.SaveChanges();
                    }
                    else
                    {
                        try
                        {
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
                        catch (DbUpdateException)
                        {
                            throw;
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
            using (NineTapDb db = new NineTapDb())
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
            using (NineTapDb db = new NineTapDb())
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
            using (var db = new NineTapDb())
            {
                db.Entry(tourn).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        public static List<WinnerListMemberViewModel> GetWinnerListMemberData(int tournamentId)
        {
            using (var db = new NineTapDb())
            {
                // Get participant/member/game info to populate DataTable
                return (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        join g in db.Games on p.Game.Id equals g.Id
                        join t in db.Tournaments on p.Tournament.Id equals t.Id
                        let memberNumber = m.Number
                        let name = m.FirstName + " " + m.LastName
                        where tournamentId == p.Tournament.Id
                        select new WinnerListMemberViewModel
                        {
                            PlaceStanding = g.PlaceStanding,
                            MemberNumber = memberNumber,
                            BowlerName = name,
                            Handicap = g.Handicap,
                            Bonus = g.Bonus,
                            MoneyWon = g.MoneyWon,
                            SidePot = g.SidePot,
                            GameId = g.Id,
                            Game1 = g.Game1,
                            Game2 = g.Game2,
                            Game3 = g.Game3,
                            Game4 = g.Game4,
                            IsComp = g.IsComp
                        }).ToList();
            }
        }
    }
}

