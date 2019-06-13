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
using System.Windows.Forms;

namespace NineTapTour.Database
{
    public class PlayerHistoryDB
    {
        /// <summary>
        /// Adds the PlayerHistory given to the database
        /// </summary>
        public static void AddPlayerHistory(PlayerHistory playerHistory)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(playerHistory).State = db.PlayerHistory.Any(his => his.hisID == playerHistory.hisID) ?
                         EntityState.Modified :
                         EntityState.Added;
                    db.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                //throw new PlayerHistoryTableException("Error Number : " + ex.Number + " - " + ex.Message);

                //Display error to user so it can be fixed
                Member member = MemberDB.GetMember(playerHistory.MemberNumber, playerHistory.regionID);
                //For more info on "?." see null conditional docs https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-conditional-operators 
                MessageBox.Show(
                    $"There was a problem with Member Number: {playerHistory.MemberNumber}, {member?.FirstName} {member?.LastName}.\n" +
                    $"Please verify all tournament dates for that member\n" +
                    $"Error: {ex.Message}\n" +
                    $"PLEASE WRITE THIS DOWN OR TAKE A PICTURE AND FIX THE MEMBER EXCEL FILE.");
            }
        }

        /// <summary>
        /// Updates the PlayerHistory in the database if it exists. 
        /// If no playerHistory was found, adds a new PlayerHistory to the database
        /// </summary>
        public static void AddOrUpdatePlayerHistory(PlayerHistory playerHistory)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(playerHistory).State = db.PlayerHistory.Any(ph => ph.hisID == playerHistory.hisID) ?
                        EntityState.Modified : EntityState.Added;
                    #region Refactored Code
                    /*
                    if (!PlayerHistoryExists(playerHistory) && FinalizeTempDB.GameExists(playerHistory))
                    {
                        db.Entry(playerHistory).State = EntityState.Added;
                    }
                    else
                    {
                        playerHistory.hisID = getHisID(playerHistory);
                        db.Entry(playerHistory).State = EntityState.Modified;
                    }
                    */
                    #endregion
                    db.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                throw new PlayerHistoryTableException("Error Number : " + ex.Number + " - " + ex.Message);
            }
        }

        /// <summary>
        /// Adds all PlayerHistories in the list given to the database. 
        /// If any of the PlayerHistories were found in the database, they are updated instead
        /// </summary>
        public static void AddOrUpdatePlayerHistoryList(List<PlayerHistory> playerHistoryList)
        {
            using (var db = new NineTapDb())
            {
                foreach (PlayerHistory playerHistory in playerHistoryList)
                {
                    db.Entry(playerHistory).State = db.PlayerHistory.Any(ph => ph.hisID == playerHistory.hisID) ?
                        EntityState.Modified : EntityState.Added;
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Returns the top 30 PlayerHistories with the same MemberNumber as the one given
        /// </summary>
        public static List<PlayerHistory> GetTop30FromPlayerHistory(int memberNumber)
        {
            List<PlayerHistory> PlayerHistoryList = new List<PlayerHistory>();
            int howmany = 30;
            using (var db = new NineTapDb())
            {
                var temp = (from h in db.PlayerHistory
                            where h.MemberNumber == memberNumber
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
                            }).Take(howmany).ToList();
                foreach (var item in temp)
                {
                    PlayerHistory newHistory = new PlayerHistory();
                    newHistory.GamesPlayed = item.GamesPlayed;
                    newHistory.TournamentDate = item.TournamentDate;
                    newHistory.Game1 = item.Game1;
                    newHistory.Game2 = item.Game2;
                    newHistory.Game3 = item.Game3;
                    newHistory.Game4 = item.Game4;
                    newHistory.AverageForGame = item.AverageForGame;
                    newHistory.trueAVG = item.trueAVG;
                    newHistory.AVG = item.AVG;
                    newHistory.HandiCap = item.HandiCap;
                    newHistory.Bonus = item.Bonus;
                    newHistory.ProPot = item.ProPot;
                    newHistory.PPHG = item.PPHG;
                    newHistory.MoneyWon = item.MoneyWon;
                    newHistory.Notes = item.Notes;
                    PlayerHistoryList.Add(newHistory);
                }
            }
            return PlayerHistoryList;
        }

        /// <summary>
        /// Finds the hisID from the playerHistory given. If no hisID was found, returns 0
        /// </summary>
        public static int GetHisID(PlayerHistory playerHistory)
        {
            using (var db = new NineTapDb())
            {
                int? hisID = (from h in db.PlayerHistory
                            join g in db.Games on h.GameID equals g.Id
                            where h.GameID == playerHistory.GameID
                            select new
                            {
                                h.hisID
                            }).FirstOrDefault().hisID;
                // Returns 0 if hisID is null
                return hisID ?? 0;
            }
        }

        /// <summary>
        /// Returns a list of all PlayerHistories with the given memberNumber and regionID
        /// </summary>
        public static List<PlayerHistory> GetMemberPlayerHistory(int memberNum, int regionID)
        {
            using (var db = new NineTapDb())
            {
                List<PlayerHistory> PlayerHistoryList = 
                    (from h in db.PlayerHistory
                    where h.MemberNumber == memberNum && h.regionID == regionID
                    orderby h.TournamentDate descending
                    select new PlayerHistory
                    {
                        hisID = h.hisID,
                        GameID = h.GameID,
                        GamesPlayed = h.GamesPlayed,
                        TournamentDate = h.TournamentDate,
                        MemberNumber = h.MemberNumber,
                        Game1 = h.Game1,
                        Game2 = h.Game2,
                        Game3 = h.Game3,
                        Game4 = h.Game4,
                        AverageForGame = h.AverageForGame,
                        trueAVG = h.trueAVG,
                        AVG = h.AVG,
                        HandiCap = h.HandiCap,
                        Bonus = h.Bonus,
                        ProPot = h.ProPot,
                        MoneyWon = h.MoneyWon,
                        Notes = h.Notes,
                    }).ToList();
                return PlayerHistoryList;
            }
        }

        /// <summary>
        /// Returns a list of the last 30 PlayerHistories with the given memberNumber and regionID
        /// </summary>
        public static List<PlayerHistory> GetMemberPlayerHistoryCount(int memberNum, int regionID)
        {
            using (var db = new NineTapDb())
            {
                List<PlayerHistory> PlayerHistoryList = 
                    (from h in db.PlayerHistory
                    where h.MemberNumber == memberNum && h.regionID == regionID
                    orderby h.TournamentDate descending
                    //orderby h.MoneyWon descending
                    select new PlayerHistory
                    {
                        hisID = h.hisID,
                        GameID = h.GameID,
                        GamesPlayed = h.GamesPlayed,
                        TournamentDate = h.TournamentDate,
                        MemberNumber = h.MemberNumber,
                        Game1 = h.Game1,
                        Game2 = h.Game2,
                        Game3 = h.Game3,
                        Game4 = h.Game4,
                        AverageForGame = h.AverageForGame,
                        trueAVG = h.trueAVG,
                        AVG = h.AVG,
                        HandiCap = h.HandiCap,
                        Bonus = h.Bonus,
                        ProPot = h.ProPot,
                        MoneyWon = h.MoneyWon,
                        Notes = h.Notes,
                    }).Take(30).ToList();
                return PlayerHistoryList;
            }
        }


        /// <summary>
        /// Returns a list of all PlayerHistories with the given regionID
        /// </summary>
        public static List<PlayerHistory> GetAllPlayerHistory(int regionID)
        {
            using (var db = new NineTapDb())
            {
                List<PlayerHistory> PlayerHistoryList =
                    (from h in db.PlayerHistory
                    where h.regionID == regionID
                    select new PlayerHistory
                    {
                        hisID = h.hisID,
                        GameID = h.GameID,
                        GamesPlayed = h.GamesPlayed,
                        TournamentDate = h.TournamentDate,
                        MemberNumber = h.MemberNumber,
                        Game1 = h.Game1,
                        Game2 = h.Game2,
                        Game3 = h.Game3,
                        Game4 = h.Game4,
                        AverageForGame = h.AverageForGame,
                        trueAVG = h.trueAVG,
                        AVG = h.AVG,
                        HandiCap = h.HandiCap,
                        Bonus = h.Bonus,
                        ProPot = h.ProPot,
                        MoneyWon = h.MoneyWon,
                        Notes = h.Notes,
                    }).ToList();
                return PlayerHistoryList;
            }
        }

        /// <summary>
        /// Gets the last quantity of games selecting only the tournament date and bonus pins.
        /// Used to calculate bonus pins.
        /// </summary>
        /// <param name="memberNum"> member number of player</param>
        /// <param name="regionID">region the tournament takes place</param>
        /// <param name="howmany">quantity of games to pull from the database</param>
        /// <returns></returns>
        public static List<PlayerHistory> GetLastQtyGamesMoneyWon(int memberNum, int regionID, int howmany)
        {
            var queryResult = new NineTapDb().PlayerHistory
                .Where(ph => ph.MemberNumber == memberNum && ph.regionID == regionID)
                .OrderByDescending(ph => ph.TournamentDate)
                .Select(ph => new {ph.TournamentDate, ph.MoneyWon})
                .Take(howmany)
                .ToList();
            return queryResult.Select(qr => new PlayerHistory()
            {
                TournamentDate = qr.TournamentDate,
                MoneyWon = qr.MoneyWon
            })
            .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<PlayerHistory> GetLastFiveTournaments(int memberNum, int regionID)
        {
            using (var db = new NineTapDb())
            {
                /* will only grab the last 5 where the AVG was adjusted, 
                  that way the bonus pins cant be affected by bowling in more then one squad */
                List<PlayerHistory> PlayerHistoryList = 
                    (from h in db.PlayerHistory
                    where h.MemberNumber == memberNum && h.regionID == regionID && h.AVG > 0 
                    /* Only grabs tournaments where average was determined. 
                      that way it doest grab history from a diffrent sqaud */
                    orderby h.TournamentDate descending, h.hisID descending
                    select new PlayerHistory
                    {
                        GamesPlayed = h.GamesPlayed,
                        TournamentDate = h.TournamentDate,
                        Game1 = h.Game1,
                        Game2 = h.Game2,
                        Game3 = h.Game3,
                        Game4 = h.Game4,
                        AverageForGame = h.AverageForGame,
                        trueAVG = h.trueAVG,
                        AVG = h.AVG,
                        HandiCap = h.HandiCap,
                        Bonus = h.Bonus,
                        ProPot = h.ProPot,
                        MoneyWon = h.MoneyWon,
                        PPHG = h.PPHG,
                        Notes = h.Notes,
                    }).Take(5).ToList();
                    return PlayerHistoryList;
            }
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

        public static bool PlayerHistoryExists(PlayerHistory ph)
        {
            return PlayerHistoryExists(ph.GameID);
        }
    }
}
