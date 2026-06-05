using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Models;

namespace NineTapTour.Database
{
    public class FinalizeTempDB
    {
        /// <summary>
        /// Calculates league average for given member based off last 30 games 
        /// or total games played if less than 30.
        /// </summary>
        public static double Get30GameAverage(Member mem)
        {
            // Represents the number of past games for the player
            int howManyGames = 30;
            using(var db = new NineTapDb())
            {
                var getGames = (from p in db.Participants
                           join m in db.Members on p.Member.Id equals m.Id
                           join g in db.Games on p.Game.Id equals g.Id
                           join t in db.Tournaments on p.Tournament.Id equals t.Id
                           where mem.Id == m.Id
                           orderby t.Date descending
                           select new { g.Game1, g.Game2, g.Game3, g.Game4, g.UseGame1, g.UseGame2, g.UseGame3, g.UseGame4 })
                              .Take(howManyGames).ToList();

                List<double> allAverages = [];
                foreach (var avg in getGames)
                {
                    allAverages.Add(Convert.ToDouble(avg.Game1 + avg.Game2 + avg.Game3 + avg.Game4) / 
                        LeagueAverageHelper(avg.UseGame1, avg.UseGame2, avg.UseGame3, avg.UseGame4));
                }

                return allAverages.Sum() / allAverages.Count;
            }
        }

        /// <summary>
        /// This is a helper method for <see cref="Get30GameAverage(Member)"/>
        /// that takes the useGames from the database to see if they are true or null.
        /// We are saying that null is true, because they are optional booleans in the database.
        /// (Assuming that during the import process these are not being updated)
        /// </summary>
        /// <param name="g1">UseGame1 From GameDB</param>
        /// <param name="g2">UseGame2 From GameDB</param>
        /// <param name="g3">UseGame3 From GameDB</param>
        /// <param name="g4">UseGame4 From GameDB</param>
        public static int LeagueAverageHelper(bool? g1, bool? g2, 
            bool? g3, bool? g4)
        {
            // Total games played (Max: 4)
            int totalGamesPlayed = 0;
            if (g1 == null || g1.Value)
            {
                totalGamesPlayed++;  
            }

            if (g2 == null || g2.Value)
            {
                totalGamesPlayed++;
            }

            if (g3 == null || g3.Value)
            {
                totalGamesPlayed++;
            }

            if (g4 == null || g4.Value)
            {
                totalGamesPlayed++;
            }
            return totalGamesPlayed;
        }

        #region Useless Method
        /*
        public static double LeagueAverage(int memID)
        {
            double sum = 0;
            double average = 0;
            var db = new NineTapDb();
            var temp = (
                        from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        join g in db.Games on p.Game.Id equals g.Id
                        join t in db.Tournaments on p.Tournament.Id equals t.Id
                        where memID == m.Id
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
        */
        #endregion

        /// <summary>
        /// Get's the leauge average based off the formula
        /// (Total of Scratch Score / Total of games played)
        /// from the last 30 entries
        /// </summary>
        /// <param name="memberNumber">The member number NOT id</param>
        /// <param name="regionId">The region id</param>
        /// <param name="tournamentId">The id of the tournament</param>
        public static double Get30GameAverage(int memberNumber, int tournamentId)
        {
            int allGamesPlayed = 0;
            int totalScratchTotal = 0;
            using (var db = new NineTapDb())
            {
                /*
                    Going through the database to get all of a player's ScratchTotals and the 4 UseGames
                    to help with calculating the league Average
                 */
                List<CurrentHistory> currentHistory = GetCurrentHistory(memberNumber, tournamentId);
                /*
                    Going through the database to get all of a player's GamePlayed and ScratchTotals Excluding
                    those from the current game.
                */
                List<PreviousHistory> previousHistory = GetPreviousHistory(memberNumber, currentHistory);

                // Looping through the current player history adding up there scratch totals & games played
                foreach (var c in currentHistory)
                {
                    allGamesPlayed += LeagueAverageHelper(c.UseGame1, c.UseGame2, c.UseGame3, c.UseGame4);
                    totalScratchTotal += c.ScratchTotal;
                }

                int counter = 0;
                //Looping through the previous player history adding up there scratch totals & games played
                foreach (var ph in previousHistory)
                {
                    allGamesPlayed += ph.GamesPlayed;
                    totalScratchTotal += ph.TotalScore;
                    counter++;
                }

                // Truncate to one decimal place
                return Math.Truncate(((double)totalScratchTotal / allGamesPlayed) * 10) / 10;
            }
        }

        public static List<CurrentHistory> GetCurrentHistory(int memberNumber, int tournamentId)
        {
            using (var db = new NineTapDb())
            {
                // Phase 6: Use Tournament.TourneyRegion.NineTapRegionID for proper FK relationship
                var curHistory = (from p in db.Participants
                                  join m in db.Members on p.Member.Id equals m.Id
                                  join g in db.Games on p.Game.Id equals g.Id
                                  join t in db.Tournaments on p.Tournament.Id equals t.Id
                                  where m.Number == memberNumber &&
                                        t.Id == tournamentId
                                  select new CurrentHistory
                                  {
                                      ScratchTotal = (g.Game1 ?? 0) + (g.Game2 ?? 0) + (g.Game3 ?? 0) + (g.Game4 ?? 0),
                                      UseGame1 = g.UseGame1 ?? true,
                                      UseGame2 = g.UseGame2 ?? true,
                                      UseGame3 = g.UseGame3 ?? true,
                                      UseGame4 = g.UseGame4 ?? true
                                  }).ToList();

                return curHistory;
            }
        }

        public static List<PreviousHistory> GetPreviousHistory(int memberNumber, List<CurrentHistory> curHistory)
        {
            using (var db = new NineTapDb())
            {
                var prevHistory = (from p in db.Participants
                                  join m in db.Members on p.Member.Id equals m.Id
                                  join g in db.Games on p.Game.Id equals g.Id
                                  join t in db.Tournaments on p.Tournament.Id equals t.Id
                                  where m.Number == memberNumber 
                                     && g.IsFinalized
                                  orderby t.Date descending
                                  orderby g.MoneyWon descending
                                   select new PreviousHistory
                                  {
                                      TournamentDate = t.Date,
                                      GamesPlayed = g.GamesPlayed,
                                      TotalScore = g.ScratchTotal
                                  }).Take(30 - curHistory.Count).ToList();
                return prevHistory;
            }
        }

        /// <summary>
        /// [Phase 4 REFACTORED] Updates Game entity with finalization properties.
        /// FinalizeTemp writes are deprecated - only Games table is updated.
        /// </summary>
        public static void AddFinalizeTemp(GameViewModel temp)
        {
            using(var db = new NineTapDb())
            {
                try
                {
                    // Phase 4: Only update Games table, skip FinalizeTemp entirely
                    Game game = db.Games.FirstOrDefault(g => g.Id == temp.GameId);
                    if (game != null)
                    {
                        // Phase 4: Removed game.TournamentID (redundant - stored in Participant)
                        // Update finalization properties on Game entity
                        game.LeagueAverage = temp.LeagueAverage;
                        game.AdjustedAvg = temp.AdjustedAvg;
                        game.KeepAdjustedAvg = temp.KeepAdjustedAvg;
                        game.HandicapTotal = temp.HandicapTotal;
                        
                        // Update member handicap
                        var member = db.Members.FirstOrDefault(x => x.Id == temp.MemberId);
                        if (member != null)
                        {
                            member.Handicap = Calculations.TournamentCalculations.CalculateHandicapPins(
                                Convert.ToInt16(Get30GameAverage(member))
                            );
                        }
                        
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error updating game finalization data: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// [DEPRECATED - Phase 3] Finds and returns the FinalizeTemp with the same ID as the GameID given.
        /// Use GameDB.GetGame() instead.
        /// </summary>
        [Obsolete("This method is deprecated. Use GameDB.GetGame() to get game data directly.")]
        public static GameViewModel GetFinalizeID(Game currentG)
        {
            using (var db = new NineTapDb())
            {
                // Return empty object for backward compatibility during transition
                return new GameViewModel { GameId = currentG.Id };
            }
        }

        /// <summary>
        /// Gets and returns a list of all participant objects for the tournament given
        /// (Phase 5: Optimized with direct projection and uses Member.NineTapRegionID)
        /// </summary>
        public static List<GameViewModel> GetAllInitialParticipantGameList(Tournament tourn)
        {
            using (var db = new NineTapDb())
            {
                return [.. (from p in db.Participants
                           join m in db.Members on p.Member.Id equals m.Id
                           join g in db.Games on p.Game.Id equals g.Id
                           join t in db.Tournaments on p.Tournament.Id equals t.Id
                           where tourn.Id == p.Tournament.Id
                           orderby m.FirstName descending
                           select new GameViewModel
                           {
                               // IDs
                               GameId = g.Id,
                               MemberId = m.Id,
                               MemberNumber = m.Number,
                               
                               // Names
                               FirstName = m.FirstName,
                               LastName = m.LastName,
                               
                               // Squad
                               Squad = p.Squad,
                               
                               // Game Scores
                               Game1 = g.Game1,
                               Game2 = g.Game2,
                               Game3 = g.Game3,
                               Game4 = g.Game4,
                               
                               // Use Game flags (null-coalesce with HasValue)
                               UseGame1 = g.UseGame1 ?? g.Game1.HasValue,
                               UseGame2 = g.UseGame2 ?? g.Game2.HasValue,
                               UseGame3 = g.UseGame3 ?? g.Game3.HasValue,
                               UseGame4 = g.UseGame4 ?? g.Game4.HasValue,
                               
                               // Notes
                               Notes = g.Notes,
                               
                               // Calculated totals
                               ScratchTotal = (g.Game1 ?? 0) + (g.Game2 ?? 0) + (g.Game3 ?? 0) + (g.Game4 ?? 0),
                               
                               // Finalization data from Game
                               LeagueAverage = g.LeagueAverage,
                               AdjustedAvg = g.AdjustedAvg,
                               KeepAdjustedAvg = g.KeepAdjustedAvg,
                               HandicapTotal = g.HandicapTotal > 0 ? g.HandicapTotal :
                                   ((g.Game1.HasValue ? (g.Game1 ?? 0) + (g.Handicap ?? 0) + (g.Bonus ?? 0) : 0) +
                                    (g.Game2.HasValue ? (g.Game2 ?? 0) + (g.Handicap ?? 0) + (g.Bonus ?? 0) : 0) +
                                    (g.Game3.HasValue ? (g.Game3 ?? 0) + (g.Handicap ?? 0) + (g.Bonus ?? 0) : 0) +
                                    (g.Game4.HasValue ? (g.Game4 ?? 0) + (g.Handicap ?? 0) + (g.Bonus ?? 0) : 0)),
                               
                               // Handicap and Bonus
                               Handicap = g.Handicap ?? 0,
                               Bonus = g.Bonus ?? 0,
                           }).ToList()];
            }
        }

        /// <summary>
        /// Gets finalization data for a tournament from Games table (Phase 5: Uses Member relationships).
        /// Returns data in FinalizeTemp format for UI compatibility.
        /// </summary>
        public static List<GameViewModel> GetListFromTable(Tournament tourn)
        {
            using (var db = new NineTapDb())
            {
                // Phase 5: Use Member.NineTapRegionID instead of Participant.ParticipantRegionID
                return [.. (from p in db.Participants
                           join m in db.Members on p.Member.Id equals m.Id
                           join g in db.Games on p.Game.Id equals g.Id
                           where p.Tournament.Id == tourn.Id
                           orderby m.FirstName, p.Squad
                           select new GameViewModel
                           {
                               TournamentID = tourn.Id, // Use Tournament from Participant
                               GameId = g.Id,
                               MemberId = m.Id,
                               MemberNumber = m.Number,
                               FirstName = m.FirstName,
                               LastName = m.LastName,
                               Squad = p.Squad,
                               Game1 = g.Game1,
                               Game2 = g.Game2,
                               Game3 = g.Game3,
                               Game4 = g.Game4,
                               UseGame1 = g.UseGame1 ?? true,
                               UseGame2 = g.UseGame2 ?? true,
                               UseGame3 = g.UseGame3 ?? true,
                               UseGame4 = g.UseGame4 ?? true,
                               LeagueAverage = g.LeagueAverage,
                               AdjustedAvg = g.AdjustedAvg,
                               KeepAdjustedAvg = g.KeepAdjustedAvg,
                               HandicapTotal = g.HandicapTotal,
                               ScratchTotal = (g.Game1 ?? 0) + (g.Game2 ?? 0) + (g.Game3 ?? 0) + (g.Game4 ?? 0),
                               Handicap = g.Handicap ?? 0,
                               Bonus = g.Bonus ?? 0,
                               Notes = g.Notes
                           })];
            }
        }

        /// <summary>
        /// Retrieves a single participant from a tournament based on given gameID.
        /// Return null if no participant is found
        /// </summary>
        public static Participant GetParticipantByGameId (int gameID)
        {
            var db = new NineTapDb();
            return (from par in db.Participants
                   where par.Game.Id == gameID
                   // No tracking prevents EF from monitoring changes. This means
                   // that we will have to manually update/delete entities
                   select par).Include(nameof(Member)).AsNoTracking().SingleOrDefault();
        }

        /// <summary>
        /// Deletes the Participant given from the database
        /// </summary>
        public static void DeleteParticipant(Participant p)
        {
            using (var db = new NineTapDb())
            {
                db.Entry(p).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Returns the total amount of comp entries for the tournament (Phase 3: reads from Games only).
        /// </summary>
        /// <param name="tourneyId">The id property for the tournament</param>
        /// <returns>Qty of comp entries</returns>
        public static int GetCompEntryQtyByTourneyID(int tourneyId)
        {
            using (var db = new NineTapDb())
            {
                // Phase 3: Read from Games via Participants join
                return (from p in db.Participants
                       join g in db.Games on p.Game.Id equals g.Id
                       where p.Tournament.Id == tourneyId && g.IsComp
                       select g).Count();
            }
        }

        /// <summary>
        /// Gets the entry quantity for a member in a tournament (Phase 3: reads from Games only).
        /// </summary>
        /// <returns>the amount of enties in tournament by a member</returns>
        public static int GetMembersGameEntryCount(int tourneyId, int memberNum)
        {
            using (var db = new NineTapDb())
            {
                // Phase 3: Read from Games via Participants join
                return (from p in db.Participants
                       join m in db.Members on p.Member.Id equals m.Id
                       join g in db.Games on p.Game.Id equals g.Id
                       where p.Tournament.Id == tourneyId && m.Number == memberNum
                       select g).Count();
            }
        }

        /// <summary>
        /// Returns true if a Game exists in the database (checks Games table only).
        /// </summary>
        public static bool GameExists(PlayerHistoryViewModel Temp)
        {
            using (var db = new NineTapDb())
            {
                // Check Games table instead of FinalizeTemp
                return db.Games.Any(g => g.Id == Temp.GameID);
            }
        }
    }
}

