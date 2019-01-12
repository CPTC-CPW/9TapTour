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
    public class PlayerHistoryDB
    {


        public static void AddPlayerHistory(PlayerHistory temp)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(temp).State = db.PlayerHistory.Any(his => his.hisID == temp.hisID) ?
                         EntityState.Modified :
                         EntityState.Added;
                    db.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                throw new PlayerHistoryTableException("Error Number : " + ex.Number + " - " + ex.Message);
            }
        }
        public static void AddPlayerHistory2(PlayerHistory temp)
        {
            try
            {
                using (var db = new NineTapDb())
                {

                    if (GameExists(temp) == false && FinalizeTempDB.GameExists(temp) == true)
                    {
                        db.Entry(temp).State = EntityState.Added;
                        db.SaveChanges();
                    }
                    else if (GameExists(temp) == true && FinalizeTempDB.GameExists(temp) == true)
                    {
                        int ID = getHisID(temp);
                        temp.hisID = ID;
                        db.Entry(temp).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        int ID = getHisID(temp);
                        temp.hisID = ID;
                        db.Entry(temp).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                }
            }
            catch (SqlException ex)
            {
                throw new PlayerHistoryTableException("Error Number : " + ex.Number + " - " + ex.Message);
            }
        }

        public static List<PlayerHistory> getTop30FromPlayerHistory(int id)
        {
            List<PlayerHistory> Return = new List<PlayerHistory>();
            using (var db = new NineTapDb())
            {
                var temp = (from h in db.PlayerHistory
                            where h.MemberNumber == id
                            orderby h.TournamentDate descending
                            select new
                            {
                                h.GamesPlayed,
                                h.TournamentDate,
                                h.Game1,
                                h.Game2,
                                h.Game3,
                                h.Game4,
                                h.AverageForGame,
                                h.trueAVG,
                                h.AVG,
                                h.HandiCap,
                                h.Bonus,
                                h.ProPot,
                                h.MoneyWon,
                                h.PPHG,
                                h.Notes,
                            }).Take(30).ToList();
                foreach (var item in temp)
                {
                    PlayerHistory newHistory = new PlayerHistory();


                    newHistory.GamesPlayed = item.GamesPlayed;
                    newHistory.TournamentDate = item.TournamentDate;
                    newHistory.Game1 = item.Game1;
                    newHistory.Game2 = item.Game2;
                    newHistory.Game3 = item.Game3;
                    newHistory.Game4 = item.Game4;
                    newHistory.TotalScore = (item.Game1 ?? 0) + (item.Game2 ?? 0) + (item.Game3 ?? 0) + (item.Game4 ?? 0);
                    newHistory.AverageForGame = item.AverageForGame;
                    newHistory.trueAVG = item.trueAVG;
                    newHistory.AVG = item.AVG;
                    newHistory.HandiCap = item.HandiCap;
                    newHistory.Bonus = item.Bonus;
                    newHistory.ProPot = item.ProPot;
                    newHistory.PPHG = item.PPHG;
                    newHistory.MoneyWon = item.MoneyWon;
                    newHistory.Notes = item.Notes;
                    Return.Add(newHistory);
                }


            }

            return Return;
        }
        public static bool GameExists(PlayerHistory Temp)
        {

            using (var db = new NineTapDb())
            {

                if (db.PlayerHistory.Any(m => m.GameID == Temp.GameID))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }

        public static int getHisID(PlayerHistory t)
        {
            int hisID = 0;
            int v = t.GameID;
            using (var db = new NineTapDb())
            {
                var temp = (from h in db.PlayerHistory
                            join g in db.Games on h.GameID equals g.Id
                            where h.GameID == v
                            select new
                            {
                                h.hisID
                            });

                foreach (var i in temp)
                {
                    hisID = i.hisID;
                }

                return hisID;


            }


        }

        public static List<PlayerHistory> getMemberPlayerHistory(int memnum, int RegionId)
        {
            List<PlayerHistory> Return = new List<PlayerHistory>();
            using (var db = new NineTapDb())
            {
                var temp = (from h in db.PlayerHistory
                            where h.MemberNumber == memnum && h.regionID == RegionId
                            orderby h.TournamentDate descending
                            select new
                            {
                                h.hisID,
                                h.GameID,
                                h.GamesPlayed,
                                h.TournamentDate,
                                h.MemberNumber,
                                h.Game1,
                                h.Game2,
                                h.Game3,
                                h.Game4,
                                h.AverageForGame,
                                h.trueAVG,
                                h.AVG,
                                h.HandiCap,
                                h.Bonus,
                                h.ProPot,
                                h.MoneyWon,
                                h.Notes,
                            }).ToList();
                foreach (var item in temp)
                {
                    PlayerHistory newHistory = new PlayerHistory();
                    newHistory.MemberNumber = item.MemberNumber;
                    newHistory.hisID = item.hisID;
                    newHistory.GameID = item.GameID;
                    newHistory.GamesPlayed = item.GamesPlayed;
                    newHistory.TournamentDate = item.TournamentDate;
                    newHistory.Game1 = item.Game1;
                    newHistory.Game2 = item.Game2;
                    newHistory.Game3 = item.Game3;
                    newHistory.Game4 = item.Game4;
                    newHistory.TotalScore = (item.Game1 ?? 0) + (item.Game2 ?? 0) + (item.Game3 ?? 0) + (item.Game4 ?? 0); ;
                    newHistory.AverageForGame = item.AverageForGame;
                    newHistory.trueAVG = item.trueAVG;
                    newHistory.AVG = item.AVG;
                    newHistory.HandiCap = item.HandiCap;
                    newHistory.Bonus = item.Bonus;
                    newHistory.ProPot = item.ProPot;
                    newHistory.MoneyWon = item.MoneyWon;
                    newHistory.Notes = item.Notes;
                    Return.Add(newHistory);
                }


            }

            return Return;
        }

        public static List<PlayerHistory> getMemberPlayerHistoryCount(int memnum, int RegionId)
        {
            List<PlayerHistory> Return = new List<PlayerHistory>();
            using (var db = new NineTapDb())
            {
                var temp = (from h in db.PlayerHistory
                            where h.MemberNumber == memnum && h.regionID == RegionId
                            orderby h.TournamentDate descending
                            select new
                            {
                                h.hisID,
                                h.GameID,
                                h.GamesPlayed,
                                h.TournamentDate,
                                h.MemberNumber,
                                h.Game1,
                                h.Game2,
                                h.Game3,
                                h.Game4,
                                h.AverageForGame,
                                h.trueAVG,
                                h.AVG,
                                h.HandiCap,
                                h.Bonus,
                                h.ProPot,
                                h.MoneyWon,
                                h.Notes,
                            }).Take(30).ToList();
                foreach (var item in temp)
                {
                    PlayerHistory newHistory = new PlayerHistory();
                    newHistory.MemberNumber = item.MemberNumber;
                    newHistory.hisID = item.hisID;
                    newHistory.GameID = item.GameID;
                    newHistory.GamesPlayed = item.GamesPlayed;
                    newHistory.TournamentDate = item.TournamentDate;
                    newHistory.Game1 = item.Game1;
                    newHistory.Game2 = item.Game2;
                    newHistory.Game3 = item.Game3;
                    newHistory.Game4 = item.Game4;
                    newHistory.TotalScore = (item.Game1 ?? 0) + (item.Game2 ?? 0) + (item.Game3 ?? 0) + (item.Game4 ?? 0); ;
                    newHistory.AverageForGame = item.AverageForGame;
                    newHistory.trueAVG = item.trueAVG;
                    newHistory.AVG = item.AVG;
                    newHistory.HandiCap = item.HandiCap;
                    newHistory.Bonus = item.Bonus;
                    newHistory.ProPot = item.ProPot;
                    newHistory.MoneyWon = item.MoneyWon;
                    newHistory.Notes = item.Notes;
                    Return.Add(newHistory);
                }


            }

            return Return;
        }



        public static List<PlayerHistory> getAllPlayerHistory(int RegionID)
        {
            List<PlayerHistory> Return = new List<PlayerHistory>();
            using (var db = new NineTapDb())
            {
                var temp = (from h in db.PlayerHistory
                            where h.regionID == RegionID
                            select new
                            {
                                h.GamesPlayed,
                                h.TournamentDate,
                                h.Game1,
                                h.Game2,
                                h.Game3,
                                h.Game4,
                                h.AverageForGame,
                                h.trueAVG,
                                h.AVG,
                                h.HandiCap,
                                h.Bonus,
                                h.ProPot,
                                h.MoneyWon,
                                h.Notes,
                                h.hisID,
                                h.regionID
                            }).ToList();
                foreach (var item in temp)
                {
                    PlayerHistory newHistory = new PlayerHistory();


                    newHistory.GamesPlayed = item.GamesPlayed;
                    newHistory.TournamentDate = item.TournamentDate;
                    newHistory.Game1 = item.Game1;
                    newHistory.Game2 = item.Game2;
                    newHistory.Game3 = item.Game3;
                    newHistory.Game4 = item.Game4;
                    newHistory.TotalScore = (item.Game1 ?? 0) + (item.Game2 ?? 0) + (item.Game3 ?? 0) + (item.Game4 ?? 0); ;
                    newHistory.AverageForGame = item.AverageForGame;
                    newHistory.trueAVG = item.trueAVG;
                    newHistory.AVG = item.AVG;
                    newHistory.HandiCap = item.HandiCap;
                    newHistory.Bonus = item.Bonus;
                    newHistory.ProPot = item.ProPot;
                    newHistory.MoneyWon = item.MoneyWon;
                    newHistory.Notes = item.Notes;
                    newHistory.regionID = item.regionID;
                    newHistory.hisID = item.hisID;
                    Return.Add(newHistory);
                }


            }

            return Return;
        }

        /// <summary>
        /// Gets the last eight tournaments selecting only the tournament date and bonus pins.
        /// Used to calculate bonus pins.
        /// </summary>
        /// <param name="memNum"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public static List<PlayerHistory> GetLastFifteenTournaments(int memNum, int regionId)
        {
            var queryResult = new NineTapDb().PlayerHistory
                                    .Where(ph => ph.MemberNumber == memNum && ph.regionID == regionId)
                                    .OrderByDescending(ph => ph.TournamentDate)
                                    .Select(ph => new {ph.TournamentDate, ph.MoneyWon, ph.Bonus})
                                    .Take(15)
                                    .ToList();

            return queryResult.Select(qr => new PlayerHistory()
                            {
                                TournamentDate = qr.TournamentDate,
                                MoneyWon = qr.MoneyWon,
                                Bonus = qr.Bonus
                            })
                            .ToList();
        }

        public static List<PlayerHistory> GetLastFiveTournaments(int memNum, int regionId)
        {
            List<PlayerHistory> lastFiveTournaments = new List<PlayerHistory>();
            using (var db = new NineTapDb())
            {
                //will only grab the last 5 where the AVG was adjusted, that way the bonus pins cant be affected by bowling in more then one squad
                var temp = (from h in db.PlayerHistory
                            where h.MemberNumber == memNum && h.regionID == regionId && h.AVG > 0 //only grabs tournaments where average was determined. that way it doest grab history from a diffrent sqaud
                            orderby h.TournamentDate descending, h.hisID descending
                            select new
                            {
                                h.GamesPlayed,
                                h.TournamentDate,
                                h.Game1,
                                h.Game2,
                                h.Game3,
                                h.Game4,
                                h.AverageForGame,
                                h.trueAVG,
                                h.AVG,
                                h.HandiCap,
                                h.Bonus,
                                h.ProPot,
                                h.MoneyWon,
                                h.PPHG,
                                h.Notes,
                            }).Take(5).ToList();
                foreach (var item in temp)
                {
                    PlayerHistory newHistory = new PlayerHistory
                    {
                        GamesPlayed = item.GamesPlayed,
                        TournamentDate = item.TournamentDate,
                        Game1 = item.Game1,
                        Game2 = item.Game2,
                        Game3 = item.Game3,
                        Game4 = item.Game4,
                        TotalScore = (item.Game1 ?? 0) + (item.Game2 ?? 0) + (item.Game3 ?? 0) + (item.Game4 ?? 0),
                        AverageForGame = item.AverageForGame,
                        trueAVG = item.trueAVG,
                        AVG = item.AVG,
                        HandiCap = item.HandiCap,
                        Bonus = item.Bonus,
                        ProPot = item.ProPot,
                        PPHG = item.PPHG,
                        MoneyWon = item.MoneyWon,
                        Notes = item.Notes
                    };
                    lastFiveTournaments.Add(newHistory);
                }


            }

            return lastFiveTournaments;
        }


        public static void DeleteGame(Game game)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(game).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch
            {

            }




        }
        public static void DeletePlayerHistory(PlayerHistory playerhist)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(playerhist).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch
            {

            }



        }

        public static int getNumberOfAllGames()
        {
            
            List<Game> gamesList = new List<Game>();
            Game current = new Game();
            using (var db = new NineTapDb())
            {
                var temp = (from g in db.Games
                            select new
                            {
                                g.Id
                            });
                foreach (var v in temp)
                {
                    current.Id = v.Id;
                    gamesList.Add(current);
                    
                }
                return gamesList.Count;
            }
        }
        public static List<PlayerHistory> getMemberPlayerHistoryByTotal(int memnum, int rID)
        {
            List<PlayerHistory> Return = new List<PlayerHistory>();
            using (var db = new NineTapDb())
            {
                var temp = (from h in db.PlayerHistory
                            where h.MemberNumber == memnum && h.regionID == rID
                            orderby h.TotalScore descending
                            select new
                            {
                                h.hisID,
                                h.GameID,
                                h.GamesPlayed,
                                h.TournamentDate,
                                h.MemberNumber,
                                h.Game1,
                                h.Game2,
                                h.Game3,
                                h.Game4,
                                h.AverageForGame,
                                h.trueAVG,
                                h.AVG,
                                h.HandiCap,
                                h.Bonus,
                                h.ProPot,
                                h.MoneyWon,
                                h.Notes,
                                h.TotalScore,
                            }).ToList().OrderByDescending(a => (a.TotalScore));
                foreach (var item in temp)
                {
                    PlayerHistory newHistory = new PlayerHistory();
                    newHistory.MemberNumber = item.MemberNumber;
                    newHistory.hisID = item.hisID;
                    newHistory.GameID = item.GameID;
                    newHistory.GamesPlayed = item.GamesPlayed;
                    newHistory.TournamentDate = item.TournamentDate;
                    newHistory.Game1 = item.Game1;
                    newHistory.Game2 = item.Game2;
                    newHistory.Game3 = item.Game3;
                    newHistory.Game4 = item.Game4;
                    newHistory.TotalScore = (item.Game1 ?? 0) + (item.Game2 ?? 0) + (item.Game3 ?? 0) + (item.Game4 ?? 0); ;
                    newHistory.AverageForGame = item.AverageForGame;
                    newHistory.trueAVG = item.trueAVG;
                    newHistory.AVG = item.AVG;
                    newHistory.HandiCap = item.HandiCap;
                    newHistory.Bonus = item.Bonus;
                    newHistory.ProPot = item.ProPot;
                    newHistory.MoneyWon = item.MoneyWon;
                    newHistory.Notes = item.Notes;
                    Return.Add(newHistory);
                }


            }

            return Return;
        }

        public static PlayerHistory getPlayerHistoryByGameID (int GameID)
        {
            PlayerHistory p = new PlayerHistory();
            using (var db = new NineTapDb())
            {
                var temp = (from h in db.PlayerHistory
                            where h.GameID == GameID

                            select new
                            {
                                h.GameID,
                                h.GamesPlayed,
                                h.TournamentDate,
                                h.Game1,
                                h.Game2,
                                h.Game3,
                                h.Game4,
                                h.AverageForGame,
                                h.trueAVG,
                                h.AVG,
                                h.HandiCap,
                                h.Bonus,
                                h.ProPot,
                                h.MoneyWon,
                                h.PPHG,
                                h.Notes,
                                h.hisID,
                                h.MemberNumber,
                                h.TotalScore
                            });

                foreach(var item in temp)
                {
                    p.AverageForGame = item.AverageForGame;
                    p.AVG = item.AVG;
                    p.Bonus = item.Bonus;
                    p.Game1 = item.Game1;
                    p.Game2 = item.Game2;
                    p.Game3 = item.Game3;
                    p.Game4 = item.Game4;
                    p.GameID = item.GameID;
                    p.GamesPlayed = item.GamesPlayed;
                    p.HandiCap = item.HandiCap;
                    p.hisID = item.hisID;
                    p.MemberNumber = item.MemberNumber;
                    p.MoneyWon = item.MoneyWon;
                    p.Notes = item.Notes;
                    p.PPHG = item.PPHG;
                    p.ProPot = item.ProPot;
                    p.TotalScore = item.TotalScore;
                    p.TournamentDate = item.TournamentDate;
                    p.trueAVG = item.trueAVG;
                }
                return p;
            }
        }

        /// <summary>
        /// Returns the total money won throughout a player's history
        /// </summary>
        /// <param name="memberNumber">the memberNumber property of the member</param>
        /// <param name="regionId">the RegionId property indicating where the member is from </param>
        /// <returns></returns>
        public static decimal GetTotalMoneyWon(int memberNumber, int regionId)
        {
            //return the sum of all money won or 0 if no entries are present for this bowler in the database
            var db = new NineTapDb();
            return db.PlayerHistory
                    .Where(p => p.MemberNumber == memberNumber && p.regionID == regionId)
                    .Select(p => (decimal?)p.MoneyWon)
                    .Sum() ?? 0; 
        }

        /// <summary>
        /// Returns true if a PlayerHistory with the same GameId exist in the database
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public static bool PlayerHistoryExists(int gameId)
        {
            return new NineTapDb().PlayerHistory.Any(ph => ph.GameID == gameId);
        }
    }
}
