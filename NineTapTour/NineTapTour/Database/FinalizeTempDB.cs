using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineTapTour.Models;

namespace NineTapTour.Database
{
    class FinalizeTempDB
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

                List<double> allAverages = new List<double>();
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
        public static int GetLeagueAverage(int memberNumber, int regionId, int tournamentId)
        {
            int allGamesPlayed = 0;
            int totalScratchTotal = 0;
            using (var db = new NineTapDb())
            {
                /*
                    Going through the database to get all of a player's ScratchTotals and the 4 UseGames
                    to help with calculating the leauge Average
                 */
                var currentHistory = GetCurrentHistory(memberNumber,regionId,tournamentId);
                /*
                    Going through the database to get all of a player's GamePlayed and ScratchTotals Excluding
                    those from the current game.
                */
                var previousHistory = GetPreviousHistory(memberNumber, regionId, currentHistory);

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

                // Cast to a double to avoid integer division and then rounding to the nearest whole number
                return Convert.ToInt32(Math.Round((double)totalScratchTotal / allGamesPlayed, MidpointRounding.AwayFromZero));
            }
        }

        /// <summary>
        /// Updates the FinalizeTemp with the same ID if it already exists in the database,
        /// otherwise adds the FinalizeTemp to the database
        /// </summary>
        public static void AddFinalizeTemp(FinalizeTemp temp)
        {
            using(var db = new NineTapDb())
            {
                try
                {
                    // Checks if tournament is new or already existing in the database
                    if (!db.FinalizeTemp.Any(f => f.GameId == temp.GameId))
                    {
                        db.Entry(temp).State = EntityState.Added;

                        /* There is a problem in the database's member's average, so it was not 
                            used, but I believe it should be. The problem might be when a tournament 
                            record is added, it is not updating the member's average in the database. */
                        // Updates the handicap of a member that participated in the tournament in the database
                        db.Members.First(x => x.Id == temp.MemberId).Handicap = 
                            Calculations.Calculations.CalculateHandicapPins(
                                Convert.ToInt16(LeagueAverage(db.Members.First(x => x.Id == temp.MemberId)))
                            );
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(temp).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error Number : " + ex.Number + " - " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Finds and returns the FinalizeTemp with the same ID as the GameID given,
        /// returns an empty FinalizeTemp if none were found
        /// </summary>
        public static FinalizeTemp GetFinalizeID(Game currentG)
        {
            using (var db = new NineTapDb())
            {
                var finalizeTemp = (from par in db.FinalizeTemp
                                    where par.GameId == currentG.Id
                                    select par).SingleOrDefault();
                //return empty finalize object if none are found to retain original behavior
                return finalizeTemp is null ? new FinalizeTemp() : finalizeTemp;
            }
        }

        /// <summary>
        /// Gets and returns a list of all participant objects for the tournament given
        /// </summary>
        public static List<FinalizeTemp> GetAllInitialParticipantGameList(Tournament tourn)
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
                                t.TourneyRegion
                            }).ToList();

                List<FinalizeTemp> ParticipantList = new List<FinalizeTemp>();
                // Populates ParticipantList with the data pulled from the database
                foreach (var item in temp)
                {
                    FinalizeTemp NewParticipant = new FinalizeTemp();

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

                    // Increases GamesPlayed by 1 for each game1-4 with a value
                    int GamesPlayed = 0;
                    if (item.Game1.HasValue)
                        GamesPlayed++;
                    if (item.Game2.HasValue)
                        GamesPlayed++;
                    if (item.Game3.HasValue)
                        GamesPlayed++;
                    if (item.Game4.HasValue)
                        GamesPlayed++;

                    // Used GamesPlayed to calculate the GameAvg
                    NewParticipant.GameAvg = ((item.Game1 ?? 0) +
                        (item.Game2 ?? 0) + (item.Game3 ?? 0) + (item.Game4 ?? 0)) / GamesPlayed;

                    // Popualtes the handicaps
                    NewParticipant.Handicap = item.Handicap ?? 0;
                    NewParticipant.Bonus = item.Bonus ?? 0;

                    // Calcualates the HandicapTotal
                    int HandicapTotal = (item.Game1 != null) ? ((item.Game1 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0)) : 0;
                    HandicapTotal += (item.Game2 != null) ? ((item.Game2 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0)) : 0;
                    HandicapTotal += (item.Game3 != null) ? ((item.Game3 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0)) : 0;
                    HandicapTotal += (item.Game4 != null) ? ((item.Game4 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0)) : 0;
                    NewParticipant.HandicapTotal = HandicapTotal;

                    NewParticipant.FinalizeRegionID = item.TourneyRegion;

                    ParticipantList.Add(NewParticipant);
                }
                return ParticipantList;
            }
        }

        /// <summary>
        /// Makes and returns a list from the FinalizeTemp Table to be used in dataview source
        /// </summary>
        public static List<FinalizeTemp> GetListFromTable(Tournament tourn)
        {
            using (var db = new NineTapDb())
            {
                // Returns a list of participants with the same TournamentID
                return db.FinalizeTemp
                    .Where(p => p.TournamentID == tourn.Id)
                    .OrderBy(p => p.FirstName)
                    .ThenBy(p => p.Squad)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets and returns a list of FinalizeTemp with the same RegionID as the ID given
        /// </summary>
        public static List<FinalizeTemp> GetFinalizeListByRegionID(int RegionID)
        {
            using (var db = new NineTapDb())
            {
                return (from f in db.FinalizeTemp
                        where f.FinalizeRegionID == RegionID
                        select f).ToList();
            }
        }

        /// <summary>
        /// Deletes the FinalizeTemp given from the database
        /// </summary>
        public static void DeleteFinalizeTemp(FinalizeTemp ft)
        {
            using (var db = new NineTapDb())
            {
                db.Entry(ft).State = EntityState.Deleted;
                db.SaveChanges();
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
        /// Returns a list of Participants with a RegionID the same as the ID given
        /// </summary>
        public static List<Participant> GetParticipantListByRegionID(int RegionID)
        {
            using (var db = new NineTapDb())
            {
                return (from p in db.Participants
                        where p.ParticipantRegionID == RegionID
                        select p).Include(nameof(Participant.Member)).ToList();
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
                return (from g in db.Games
                where g.gameRegionID == RegionID
                select g).ToList();
            }
        }

        /// <summary>
        /// Returns the total amount of comp entries for the tournament
        /// </summary>
        /// <param name="tourneyId">The id property for the tournament</param>
        /// <returns>Qty of comp entries</returns>
        public static int GetCompEntryQtyByTourneyID(int tourneyId)
        {
            using (var db = new NineTapDb())
            {
                return db.FinalizeTemp
                        .Join(db.Games, ft => ft.GameId, g => g.Id,
                                   (ft, g) => new { g.IsComp, ft.TournamentID })
                        .Where(ftg => ftg.TournamentID == tourneyId && ftg.IsComp)
                        .Count();
            }
        }

        /// <summary>
        /// Gets the entry quantity for a member in a tournament
        /// </summary>
        /// <returns>the amount of enties in tournament by a member</returns>
        public static int GetMembersGameEntryCount(int tourneyId, int memberNum)
        {
            using (var db = new NineTapDb())
            {
                return db.FinalizeTemp
                        .Where(ft => ft.TournamentID == tourneyId && ft.MemberNumber == memberNum)
                        .Count();
            }
        }

        public static List<CurrentHistory> GetCurrentHistory(int memberNumber, int regionId, int tournamentId) 
        {
            using(var db = new NineTapDb()) 
            {
                var curHistory = (from f in db.FinalizeTemp
                        where f.FinalizeRegionID == regionId &&
                              f.MemberNumber == memberNumber &&
                              f.TournamentID == tournamentId
                        select new CurrentHistory
                        {
                            ScratchTotal = f.ScratchTotal,
                            UseGame1 = f.UseGame1,
                            UseGame2 = f.UseGame2,
                            UseGame3 = f.UseGame3,
                            UseGame4 = f.UseGame4
                        }).ToList();

                return curHistory;
            }
           
        }

        public static List<PreviousHistory> GetPreviousHistory(int memberNumber, int regionId, List<CurrentHistory> curHistory) 
        {
            using(var db = new NineTapDb()) 
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
        /// Returns true if a Game has the same GameID in the Database 
        /// as the GameID in the PlayerHistory given, returns false otherwise
        /// </summary>
        public static bool GameExists(PlayerHistory Temp)
        {
            using (var db = new NineTapDb())
            {
                if (db.FinalizeTemp.Any(m => m.GameId == Temp.GameID))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}

