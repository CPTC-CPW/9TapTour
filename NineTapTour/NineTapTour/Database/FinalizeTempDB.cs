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
        public static double LeagueAverage(Member mem)
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
        /// This is a helper method for <see cref="LeagueAverage(Member)"/>
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
        public static double GetLeagueAverage(int memberNumber, int regionId, int tournamentId)
        {
            int allGamesPlayed = 0;
            int totalScratchTotal = 0;
            using (var db = new NineTapDb())
            {
                /*
                    Going through the database to get all of a player's ScratchTotals and the 4 UseGames
                    to help with calculating the leauge Average
                 */
                List<CurrentHistory> currentHistory = GetCurrentHistory(memberNumber, regionId, tournamentId);
                /*
                    Going through the database to get all of a player's GamePlayed and ScratchTotals Excluding
                    those from the current game.
                */
                List<PreviousHistory> previousHistory = GetPreviousHistory(memberNumber, regionId, currentHistory);

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

        public static List<CurrentHistory> GetCurrentHistory(int memberNumber, int regionId, int tournamentId)
        {
            using (var db = new NineTapDb())
            {
                // Phase 3: Read directly from Games table (no FinalizeTemp fallback)
                var curHistory = (from p in db.Participants
                                  join m in db.Members on p.Member.Id equals m.Id
                                  join g in db.Games on p.Game.Id equals g.Id
                                  join t in db.Tournaments on p.Tournament.Id equals t.Id
                                  where m.Number == memberNumber &&
                                        t.TourneyRegion == regionId &&
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

        public static List<PreviousHistory> GetPreviousHistory(int memberNumber, int regionId, List<CurrentHistory> curHistory)
        {
            using (var db = new NineTapDb())
            {
                var prevHistory = (from p in db.PlayerHistory
                                   where p.MemberNumber == memberNumber && p.regionID == regionId
                                   orderby p.TournamentDate descending
                                   select new PreviousHistory
                                   {
                                       TournamentDate = p.TournamentDate,
                                       GamesPlayed = p.GamesPlayed,
                                       TotalScore = p.TotalScore
                                   }).Take(30 - curHistory.Count).ToList();
                return prevHistory;
            }
        }

        /// <summary>
        /// [Phase 3 REFACTORED] Updates Game entity with finalization properties.
        /// FinalizeTemp writes are deprecated - only Games table is updated.
        /// </summary>
        public static void AddFinalizeTemp(GameViewModel temp)
        {
            using(var db = new NineTapDb())
            {
                try
                {
                    // Phase 3: Only update Games table, skip FinalizeTemp entirely
                    Game game = db.Games.FirstOrDefault(g => g.Id == temp.GameId);
                    if (game != null)
                    {
                        // Update finalization properties on Game entity
                        game.TournamentID = temp.TournamentID;
                        game.LeagueAverage = temp.LeagueAverage;
                        game.AdjustedAvg = temp.AdjustedAvg;
                        game.KeepAdjustedAvg = temp.KeepAdjustedAvg;
                        game.GameAvg = temp.GameAvg;
                        game.HandicapTotal = temp.HandicapTotal;
                        
                        // Update member handicap
                        var member = db.Members.FirstOrDefault(x => x.Id == temp.MemberId);
                        if (member != null)
                        {
                            member.Handicap = Calculations.Calculations.CalculateHandicapPins(
                                Convert.ToInt16(LeagueAverage(member))
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
        /// </summary>
        public static List<GameViewModel> GetAllInitialParticipantGameList(Tournament tourn)
        {
            using (var db = new NineTapDb())
            {
                var temp = (from p in db.Participants
                            join m in db.Members on p.Member.Id equals m.Id
                            join g in db.Games on p.Game.Id equals g.Id
                            join t in db.Tournaments on p.Tournament.Id equals t.Id
                            where tourn.Id == p.Tournament.Id
                            orderby m.FirstName descending
                            select new
                            {
                                g.Id,
                                m.FirstName,
                                m.LastName,
                                MemberId = m.Id,
                                p.Squad,
                                g.Game1,
                                g.Game2,
                                g.Game3,
                                g.Game4,
                                g.UseGame1,
                                g.UseGame2,
                                g.UseGame3,
                                g.UseGame4,
                                g.Notes,
                                g.Handicap,
                                g.Bonus,
                                m.Number,
                                t.TourneyRegion,
                                g.LeagueAverage,
                                g.AdjustedAvg,
                                g.KeepAdjustedAvg,
                                g.GameAvg,
                                g.HandicapTotal
                            }).ToList();

                List<GameViewModel> ParticipantList = [];
                // Populates ParticipantList with the data pulled from the database
                foreach (var item in temp)
                {
                    GameViewModel NewParticipant = new();

                    // Populates the names and ID's
                    NewParticipant.GameId = item.Id;
                    NewParticipant.MemberId = item.MemberId;
                    NewParticipant.MemberNumber = item.Number;
                    NewParticipant.FirstName = item.FirstName;
                    NewParticipant.LastName = item.LastName;
                    NewParticipant.Squad = item.Squad;

                    // Populates the Games
                    NewParticipant.Game1 = item.Game1;
                    NewParticipant.Game2 = item.Game2;
                    NewParticipant.Game3 = item.Game3;
                    NewParticipant.Game4 = item.Game4;
                    NewParticipant.UseGame1 = item.UseGame1 ?? item.Game1.HasValue;
                    NewParticipant.UseGame2 = item.UseGame2 ?? item.Game2.HasValue;
                    NewParticipant.UseGame3 = item.UseGame3 ?? item.Game3.HasValue;
                    NewParticipant.UseGame4 = item.UseGame4 ?? item.Game4.HasValue;
                    NewParticipant.Notes = item.Notes;
                    NewParticipant.ScratchTotal = (item.Game1 ?? 0) + (item.Game2 ?? 0) + (item.Game3 ?? 0) + (item.Game4 ?? 0);

                    // Phase 3: Use values from Game if available
                    NewParticipant.LeagueAverage = item.LeagueAverage;
                    NewParticipant.AdjustedAvg = item.AdjustedAvg;
                    NewParticipant.KeepAdjustedAvg = item.KeepAdjustedAvg;
                    NewParticipant.GameAvg = item.GameAvg > 0 ? item.GameAvg : CalculateGameAvg(item);
                    NewParticipant.HandicapTotal = item.HandicapTotal > 0 ? item.HandicapTotal : CalculateHandicapTotal(item);

                    // Populates the handicaps
                    NewParticipant.Handicap = item.Handicap ?? 0;
                    NewParticipant.Bonus = item.Bonus ?? 0;

                    NewParticipant.FinalizeRegionID = item.TourneyRegion;

                    ParticipantList.Add(NewParticipant);
                }
                return ParticipantList;
            }
        }

        private static int CalculateGameAvg(dynamic item)
        {
            int gamesPlayed = 0;
            if (item.Game1.HasValue) gamesPlayed++;
            if (item.Game2.HasValue) gamesPlayed++;
            if (item.Game3.HasValue) gamesPlayed++;
            if (item.Game4.HasValue) gamesPlayed++;
            
            if (gamesPlayed == 0) return 0;
            
            return ((item.Game1 ?? 0) + (item.Game2 ?? 0) + (item.Game3 ?? 0) + (item.Game4 ?? 0)) / gamesPlayed;
        }

        private static int CalculateHandicapTotal(dynamic item)
        {
            int handicapTotal = 0;
            if (item.Game1 != null) handicapTotal += (item.Game1 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0);
            if (item.Game2 != null) handicapTotal += (item.Game2 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0);
            if (item.Game3 != null) handicapTotal += (item.Game3 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0);
            if (item.Game4 != null) handicapTotal += (item.Game4 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0);
            return handicapTotal;
        }

        /// <summary>
        /// Gets finalization data for a tournament from Games table (Phase 3: FinalizeTemp deprecated).
        /// Returns data in FinalizeTemp format for UI compatibility.
        /// </summary>
        public static List<GameViewModel> GetListFromTable(Tournament tourn)
        {
            using (var db = new NineTapDb())
            {
                // Phase 3: Always read from Games table
                return [.. (from p in db.Participants
                           join m in db.Members on p.Member.Id equals m.Id
                           join g in db.Games on p.Game.Id equals g.Id
                           where p.Tournament.Id == tourn.Id
                           orderby m.FirstName, p.Squad
                           select new GameViewModel
                           {
                               FinalizeID = g.Id, // Use Game ID as FinalizeID
                               TournamentID = g.TournamentID ?? tourn.Id,
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
                               GameAvg = g.GameAvg,
                               HandicapTotal = g.HandicapTotal,
                               ScratchTotal = (g.Game1 ?? 0) + (g.Game2 ?? 0) + (g.Game3 ?? 0) + (g.Game4 ?? 0),
                               Handicap = g.Handicap ?? 0,
                               Bonus = g.Bonus ?? 0,
                               Notes = g.Notes,
                               FinalizeRegionID = g.gameRegionID
                           })];
            }
        }

        /// <summary>
        /// [DEPRECATED - Phase 3] Deletes the FinalizeTemp given from the database.
        /// FinalizeTemp is deprecated - data is now in Games table.
        /// </summary>
        [Obsolete("This method is deprecated. FinalizeTemp table is being phased out.")]
        public static void DeleteFinalizeTemp(GameViewModel ft)
        {
            // Phase 3: No-op - FinalizeTemp deletion no longer needed
            // Data is in Games table which should not be deleted this way
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
        /// Returns a list of Participants with a RegionID the same as the ID given
        /// </summary>
        public static List<Participant> GetParticipantListByRegionID(int RegionID)
        {
            using (var db = new NineTapDb())
            {
                return [.. (from p in db.Participants
                        where p.ParticipantRegionID == RegionID
                        select p).Include(nameof(Participant.Member))];
            }
                
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
        /// Returns a list of the Games with the same RegionID as the ID given
        /// </summary>
        public static List<Game> GetGameListByRegionID(int RegionID)
        {
            using (var db = new NineTapDb())
            {
                return [.. (from g in db.Games
                where g.gameRegionID == RegionID
                select g)];
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
        /// Returns true if a Game exists in the database (Phase 3: checks Games table only).
        /// </summary>
        public static bool GameExists(PlayerHistory Temp)
        {
            using (var db = new NineTapDb())
            {
                // Phase 3: Check Games table instead of FinalizeTemp
                return db.Games.Any(g => g.Id == Temp.GameID);
            }
        }
    }
}

