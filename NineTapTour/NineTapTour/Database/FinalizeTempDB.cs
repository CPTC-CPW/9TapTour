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
            int howmany = 30;
            var db = new NineTapDb();
            double avg = (double)(from p in db.Participants
                          join m in db.Members on p.Member.Id equals m.Id
                          join g in db.Games on p.Game.Id equals g.Id
                          join t in db.Tournaments on p.Tournament.Id equals t.Id
                          where mem.Id == m.Id
                          orderby t.Date descending
                          select (g.Game1 + g.Game2 + g.Game3 + g.Game4) / 
                              ((g.UseGame1??false?1:0) + (g.UseGame2??false?1:0) + (g.UseGame3??false?1:0) + (g.UseGame4??false?1:0))
                          ).Take(howmany).Average();
            return avg;
            #region Refactored Code
            /*
            double sum = 0;
            double average = 0;
            var temp = (from p in db.Participants
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
            */
            #endregion
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
        /// Returns a sum of the games from a MemberNumber given
        /// <paramref name="howmany">The number of games taken</paramref>
        /// </summary>
        public static double LeagueAvgFromPlayerHistory(int memberNumber, int howmany, int regionid)
        {
            var db = new NineTapDb();
            double sum = 0;
            // Calculates the Sum as the query instead of grabing all the data
            double avg = (from p in db.PlayerHistory
                          where p.MemberNumber == memberNumber && p.regionID == regionid
                          orderby p.TournamentDate descending
                          select p.AverageForGame).Take(howmany).Average();
            return avg;
            /*
            var temp = (from p in db.PlayerHistory
                        where p.MemberNumber == memberNumber && p.regionID == regionid
                        orderby p.TournamentDate descending
                        select new
                        {
                            p.TournamentDate,
                            p.Game1,
                            p.Game2,
                            p.Game3,
                            p.Game4,
                            p.trueAVG,
                            p.AverageForGame
                        }).Take(howmany).ToList();
            if (temp.Count > 0)
            {

                foreach (var item in temp)
                {
                    sum += Convert.ToDouble(item.AverageForGame);
                }
                return sum;
            }
            return 0;
            */
        }

        /// <summary>
        /// Updates the FinalizeTemp with the same ID if it already exists in the database,
        /// otherwise adds the FinalizeTemp to the database
        /// </summary>
        public static void AddFinalizeTemp(FinalizeTemp temp)
        {
            try
            {
                var db = new NineTapDb();
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

        /// <summary>
        /// Finds and returns the FinalizeTemp with the same ID as the GameID given,
        /// returns an empty FinalizeTemp if none were found
        /// </summary>
        public static FinalizeTemp GetFinalizeID(Game currentG)
        {
            var db = new NineTapDb();
            var finalizeTemp = (from par in db.FinalizeTemp
                                where par.GameId == currentG.Id
                                select par).SingleOrDefault();
            //return empty finalize object if none are found to retain original behavior
            return finalizeTemp is null ? new FinalizeTemp() : finalizeTemp;
        }

        /// <summary>
        /// Gets and returns a list of all participant objects for the tournament given
        /// </summary>
        public static List<FinalizeTemp> GetAllInitialParticipantGameList(Tournament tourn)
        {
            var db = new NineTapDb();
            List<FinalizeTemp> FinalizeTempList = 
                (from p in db.Participants
                join m in db.Members on p.Member.Id equals m.Id
                join g in db.Games on p.Game.Id equals g.Id
                join t in db.Tournaments on p.Tournament.Id equals t.Id
                where tourn.Id == p.Tournament.Id
                orderby m.FirstName descending
                select new FinalizeTemp
                {
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
                    UseGame1 = g.UseGame1 ?? g.Game1.HasValue,
                    UseGame2 = g.UseGame2 ?? g.Game2.HasValue,
                    UseGame3 = g.UseGame3 ?? g.Game3.HasValue,
                    UseGame4 = g.UseGame4 ?? g.Game4.HasValue,
                    Notes = g.Notes,
                    ScratchTotal = (g.Game1 ?? 0) + (g.Game2 ?? 0) + (g.Game3 ?? 0) + (g.Game4 ?? 0),

                    GameAvg = ((g.Game1 ?? 0) + (g.Game2 ?? 0) + (g.Game3 ?? 0) + (g.Game4 ?? 0)) /
                        ((g.UseGame1 ?? false ? 1 : 0) + (g.UseGame2 ?? false ? 1 : 0) + (g.UseGame3 ?? false ? 1 : 0) + (g.UseGame4 ?? false ? 1 : 0)),
                    HandicapTotal =
                        ((g.Game1 != null) ? ((g.Game1 ?? 0) + (g.Handicap ?? 0) + (g.Bonus ?? 0)) : 0) +
                        ((g.Game2 != null) ? ((g.Game2 ?? 0) + (g.Handicap ?? 0) + (g.Bonus ?? 0)) : 0) +
                        ((g.Game3 != null) ? ((g.Game3 ?? 0) + (g.Handicap ?? 0) + (g.Bonus ?? 0)) : 0) +
                        ((g.Game4 != null) ? ((g.Game4 ?? 0) + (g.Handicap ?? 0) + (g.Bonus ?? 0)) : 0),
                    Handicap = g.Handicap ?? 0,
                    Bonus = g.Bonus ?? 0,
                    FinalizeRegionID = t.TourneyRegion
                }).ToList();
            return FinalizeTempList;
        }

        /// <summary>
        /// Makes and returns a list from the FinalizeTemp Table to be used in dataview source
        /// </summary>
        public static List<FinalizeTemp> GetListFromTable(Tournament tourn)
        {
            var db = new NineTapDb();
            // Returns a list of participants with the same TournamentID
            return db.FinalizeTemp
                .Where(p => p.TournamentID == tourn.Id)
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.Squad)
                .ToList();
        }

        /// <summary>
        /// Gets and returns a list of FinalizeTemp with the same RegionID as the ID given
        /// </summary>
        public static List<FinalizeTemp> GetFinalizeListByRegionID(int RegionID)
        {
            var db = new NineTapDb();
            return (from f in db.FinalizeTemp
                    where f.FinalizeRegionID == RegionID
                    select f).ToList();
        }

        /// <summary>
        /// Deletes the FinalizeTemp given from the database
        /// </summary>
        public static void DeleteFinalizeTemp(FinalizeTemp ft)
        {
            var db = new NineTapDb();
            db.Entry(ft).State = EntityState.Deleted;
            db.SaveChanges();
        }

        /// <summary>
        /// Returns a list of Participants with a TournamentID equal to the ID given
        /// </summary>
        public static List<Participant> GetGameParticipantList(int id)
        {
            List<Participant> par = new List<Participant>();
            var db = new NineTapDb();
            var temp = (from p in db.Participants
                        where p.Tournament.Id == id
                        select new
                        {
                            p.Id,
                            p.Game,
                            p.Member,
                            p.Squad,
                            p.Tournament
                        }).ToList();
            // par is never populated, so this always returns an empty list
            return par;
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
                   select par).Include(nameof(Member)).SingleOrDefault();
        }

        /// <summary>
        /// Returns a list of Participants with a RegionID the same as the ID given
        /// </summary>
        public static List<Participant> GetParticipantListByRegionID(int RegionID)
        {
            var db = new NineTapDb();
            return (from p in db.Participants
                    where p.ParticipantRegionID == RegionID
                    select p).Include(nameof(Participant.Member)).ToList();
        }

        /// <summary>
        /// Deletes the Participant given from the database
        /// </summary>
        public static void DeleteParticipant(Participant p)
        {
            var db = new NineTapDb();
            db.Entry(p).State = EntityState.Deleted;
            db.SaveChanges();
        }

        /// <summary>
        /// Returns a list of the Games with the same RegionID as the ID given
        /// </summary>
        public static List<Game> GetGameListByRegionID(int RegionID)
        {
            var db = new NineTapDb();
            return (from g in db.Games
                    where g.gameRegionID == RegionID
                    select g).ToList();
        }

        /// <summary>
        /// Returns the total amount of comp entries for the tournament
        /// </summary>
        /// <param name="tourneyId">The id property for the tournament</param>
        /// <returns>Qty of comp entries</returns>
        public static int GetCompEntryQtyByTourneyID(int tourneyId)
        {
            var db = new NineTapDb();
            return db.FinalizeTemp
                    .Join(db.Games,
                        ft => ft.GameId,
                        g => g.Id,
                        (ft, g) => new { g.IsComp, ft.TournamentID })
                    .Where(ftg => ftg.TournamentID == tourneyId && ftg.IsComp)
                    .Count();
        }

        /// <summary>
        /// Gets the entry quantity for a member in a tournament
        /// </summary>
        /// <returns>the amount of enties in tournament by a member</returns>
        public static int GetMembersGameEntryCount(int tourneyId, int memberNum)
        {
            var db = new NineTapDb();
            return db.FinalizeTemp
                    .Where(ft => ft.TournamentID == tourneyId && ft.MemberNumber == memberNum)
                    .Count();
        }

        /// <summary>
        /// Returns true if a Game has the same GameID in the Database 
        /// as the GameID in the PlayerHistory given, returns false otherwise
        /// </summary>
        public static bool GameExists(PlayerHistory Temp)
        {
            var db = new NineTapDb();
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

