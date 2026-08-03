using System;
using System.Collections.Generic;
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
            using var db = new NineTapDb();
            //checks if tournament is new or already existing in db
            db.Entry(tourn).State = db.Tournaments.Any(t => t.Id == tourn.Id) ?
                EntityState.Modified :
                EntityState.Added;
            db.SaveChanges();
        }

        /// <summary>
        /// Adds the given Tournament into the database using an existing DbContext.
        /// NOTE: Does NOT call SaveChanges - caller controls when to save for batch operations.
        /// </summary>
        public static void AddTournament(Tournament tourn, NineTapDb db)
        {
            //checks if tournament is new or already existing in db
            db.Entry(tourn).State = db.Tournaments.Any(t => t.Id == tourn.Id) ?
                EntityState.Modified :
                EntityState.Added;
        }

        /// <summary>
        /// Updates the given Tournament in the database
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if tournament argument not found in database</exception>
        public static bool UpdateTournament(Tournament tourn)
        {
            using var db = new NineTapDb();
            Tournament original = db.Tournaments.Find(tourn.Id);
            if (original != null)
            {
                db.Entry(original).CurrentValues.SetValues(tourn);
                db.SaveChanges();
            }
            else
            {
                throw new ArgumentException("The original data could not be found.");
            }
            return true;
        }

        /// <summary>
        /// Returns the list of Tournaments ordered by Date descending
        /// </summary>
        public static List<Tournament> GetTournamentList()
        {
            using (NineTapDb db = new())
            {
                return [.. (from t in db.Tournaments
                        orderby t.Date descending
                        select t)];
            }
        }

        /// <summary>
        /// Returns the list of Tournaments ordered by Date descending using an existing DbContext (Phase 6: uses TourneyRegion FK)
        /// </summary>
        public static List<Tournament> GetTournamentList(NineTapDb db)
        {
            // Phase 6: Use Tournament.TourneyRegion.NineTapRegionID for proper FK relationship
            return [.. (from t in db.Tournaments
                    orderby t.Date descending
                    select t)];
        }

        /// <summary>
        /// Returns a list of Participants within the given Tournament. Participants
        /// are returned in the order they were entered in the tournament (by participant id)
        /// </summary>
        public static List<Participant> GetTournamentMemberList(Tournament tourn)
        {
            using (NineTapDb db = new())
            {
                return [.. (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        orderby p.Id
                        where p.Tournament.Id == tourn.Id
                        select p).Include(m =>m.Member)];
            }
        }

        /// <summary>
        /// Returns a list of Participants within the given Tournament
        /// orders by MemberID
        /// </summary>
        public static List<Participant> GetTournamentMemberListInOrder(Tournament tourn)
        {
            using (NineTapDb db = new())
            {
                return [.. (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        orderby p.Member.Id
                        where p.Tournament.Id == tourn.Id
                        select p).Include(m => m.Member)];
            }
        }

        /// <summary>
        /// Get the total number of Participants in the given Tournament
        /// </summary>
        public static int GetTotalNumberParticipantsInTournament(Tournament tourn)
        {
            NineTapDb db = new();
            return db.Participants
                .Where(p => p.Tournament.Id == tourn.Id)
                .Count();
        }

        /// <summary>
        /// Returns a list of unique Members in the given Tournament
        /// </summary>
        public static List<Member> GetUniqueTourMembers(Tournament tourn)
        {
            using (NineTapDb db = new())
            {
                return [.. (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        where p.Tournament.Id == tourn.Id
                        select m).Distinct()];
            }
        }

        /// <summary>
        /// Returns a list of unique Members that were in a Tournament within the given dates inclusively
        /// </summary>
        public static List<Member> GetUniqueTourMembersByDate(DateTime start, DateTime end)
        {
            using (NineTapDb db = new())
            {
                return [.. (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        where p.Tournament.Date >= start && p.Tournament.Date <= end
                        select m).Distinct()];
            }
        }

        /// <summary>
        /// Adds the given Participant to the Tournament
        /// </summary>
        public static void AddMemberToTournament(Participant player)
        {
            using var db = new NineTapDb();
            
            // Use AsNoTracking to avoid tracking entities in the duplicate check query
            bool isMemberInTournament = db.Participants
                .AsNoTracking()
                .Any(p => p.Member.Id == player.Member.Id
                       && p.Tournament.Id == player.Tournament.Id
                       && p.Squad == player.Squad);
            
            if (!isMemberInTournament)
            {
                player.Id = 0; // New participants will get an auto generated id
                
                // Attach related entities to this context before adding the participant
                db.Attach(player.Member);
                db.Attach(player.Tournament);
                db.Attach(player.Game);

                db.Participants.Add(player);
                
                // Set states to Unchanged
                // db.Entry(player.Game).State = EntityState.Unchanged; // If the member is new, a game would be too
                db.Entry(player.Tournament).State = EntityState.Unchanged;
                db.Entry(player.Member).State = EntityState.Unchanged;
                
                db.SaveChanges();
            }
            else
            {
                UpdateExistingParticipantScores(player, db);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Adds the given Participant to the Tournament using an existing DbContext.
        /// NOTE: Does NOT call SaveChanges - caller controls when to save for batch operations.
        /// </summary>
        public static void AddMemberToTournament(Participant player, NineTapDb db)
        {
            // Use AsNoTracking to avoid tracking entities in the duplicate check query
            bool isMemberInTournament = db.Participants
                .AsNoTracking()
                .Any(p => p.Member.Id == player.Member.Id
                       && p.Tournament.Id == player.Tournament.Id
                       && p.Squad == player.Squad);
            
            if (!isMemberInTournament)
            {
                player.Id = 0; // New participants will get an auto generated id
                
                // Ensure related entities are tracked by this context
                var memberEntry = db.Entry(player.Member);
                if (memberEntry.State == EntityState.Detached)
                {
                    db.Attach(player.Member);
                }
                
                var tournamentEntry = db.Entry(player.Tournament);
                if (tournamentEntry.State == EntityState.Detached)
                {
                    db.Attach(player.Tournament);
                }
                
                var gameEntry = db.Entry(player.Game);
                if (gameEntry.State == EntityState.Detached)
                {
                    db.Attach(player.Game);
                }
                
                db.Participants.Add(player);
            }
            else
            {
                UpdateExistingParticipantScores(player, db);
            }
        }

        /// <summary>
        /// Updates the scores on the Game of the Participant already in the tournament
        /// that matches the given player's member, tournament, and squad. The lookup is
        /// by member/tournament/squad because callers may pass a new, unsaved Game (Id 0).
        /// NOTE: Does NOT call SaveChanges - caller controls when to save.
        /// </summary>
        private static void UpdateExistingParticipantScores(Participant player, NineTapDb db)
        {
            Participant existing = db.Participants
                .Include(p => p.Game)
                .First(p => p.Member.Id == player.Member.Id
                         && p.Tournament.Id == player.Tournament.Id
                         && p.Squad == player.Squad);

            existing.Game.Game1 = player.Game.Game1;
            existing.Game.Game2 = player.Game.Game2;
            existing.Game.Game3 = player.Game.Game3;
            existing.Game.Game4 = player.Game.Game4;
            existing.Game.MoneyWon = player.Game.MoneyWon;
            existing.Game.IsComp = player.Game.IsComp;
        }

        /// <summary>
        /// Returns a Tournament from the database with the same TournamentID given,
        /// returns null if one was not found
        /// </summary>
        public static Tournament GetTourneyByID(int tournID)
        {
            using (NineTapDb db = new())
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
            using (NineTapDb db = new())
            {
                List<Member> activeMembers =
                        [.. (from active in db.Members
                         where active.IsActive == true
                         select active)];
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
                // Delete games
                var gamesToDelete = db.Games.Where(g => g.Participant.Tournament.Id == tourn.Id).ToList();
                db.Games.RemoveRange(gamesToDelete);

                // Delete participants
                var participantsToDelete = db.Participants.Where(p => p.Tournament.Id == tourn.Id).ToList();
                db.Participants.RemoveRange(participantsToDelete);
               
                db.Entry(tourn).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        public static List<WinnerListMemberViewModel> GetWinnerListMemberData(int tournamentId)
        {
            using (var db = new NineTapDb())
            {
                // Get participant/member/game info to populate DataTable
                return [.. (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        join g in db.Games on p.Game.Id equals g.Id
                        join t in db.Tournaments on p.Tournament.Id equals t.Id
                        let memberNumber = m.Number
                        let name = m.FirstName + " " + m.LastName
                        where tournamentId == p.Tournament.Id
                        select new WinnerListMemberViewModel
                        {
                            PlaceStanding = g.PlaceStanding,
                            PlaceStandingLabel = g.PlaceStandingLabel,
                            MemberId = m.Id,
                            MemberNumber = memberNumber,
                            BowlerName = name,
                            Handicap = g.Handicap,
                            Bonus = g.Bonus,
                            MemberBonus = m.Bonus,
                            MoneyWon = g.MoneyWon,
                            SidePot = g.SidePot,
                            GameId = g.Id,
                            Game1 = g.Game1,
                            Game2 = g.Game2,
                            Game3 = g.Game3,
                            Game4 = g.Game4,
                            IsComp = g.IsComp,
                            LeagueAverage = (double)(m.Average ?? 0),
                            AdjustedAvg = g.AdjustedAvg,
                            UseGame1 = g.UseGame1,
                            UseGame2 = g.UseGame2,
                            UseGame3 = g.UseGame3,
                            UseGame4 = g.UseGame4,
                            KeepAdjustedAvg = g.KeepAdjustedAvg,
                            Squad = p.Squad
                        })];
            }
        }
    }
}

