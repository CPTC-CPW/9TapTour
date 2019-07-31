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
        public static List<PlayerHistory> GetTop30FromPlayerHistory(int memberNum)
        {
            int howmany = 30;
            using (var db = new NineTapDb())
            {
                List<PlayerHistory> PlayerHistoryList =
                    (from h in db.PlayerHistory
                     where h.MemberNumber == memberNum
                     orderby h.TournamentDate descending
                     select h).Take(howmany).ToList();
                return PlayerHistoryList;
            }
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
                            }).FirstOrDefault()?.hisID; //assign null (default) or actual historyID if there is a value
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
                    select h).ToList();
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
                    orderby h.TournamentDate descending, h.MoneyWon descending
                    select h).Take(30).ToList();
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
                    select h).ToList();
                return PlayerHistoryList;
            }
        }

        /// <summary>
        /// Gets the last quantity of games selecting only the tournament date and bonus pins.
        /// Used to calculate bonus pins.
        /// </summary>
        /// <param name="howmany">number of games to pull from the database</param>
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
        /// Returns a list of the last 5 PlayerHistories with the given MemberNumber and RegionID
        /// </summary>
        public static List<PlayerHistory> GetLastFiveTournaments(int memberNum, int regionID)
        {
            int howmany = 5;
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
                    select h).Take(howmany).ToList();
                return PlayerHistoryList;
            }
        }

        /// <summary>
        /// Deletes the given Game from the database
        /// </summary>
        public static void DeleteGame(Game game)
        {
            using (var db = new NineTapDb())
            {
                db.Entry(game).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes the given PlayerHistory from the database
        /// </summary>
        public static void DeletePlayerHistory(PlayerHistory playerHistory)
        {
            using (var db = new NineTapDb())
            {
                db.Entry(playerHistory).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Returns the number of games in the database
        /// </summary>
        public static int GetNumberOfAllGames()
        {
            using (var db = new NineTapDb())
            {
                int gameCount = 
                    (from g in db.Games
                    select g).Count();
                return gameCount;
            }
        }

        /// <summary>
        /// Returns a list of PlayerHistories ordered by there TotalScore descending
        /// </summary>
        public static List<PlayerHistory> GetMemberPlayerHistoryByTotal(int memberNum, int regionID)
        {
            using (var db = new NineTapDb())
            {
                List<PlayerHistory> PlayerHistoryList = 
                    (from h in db.PlayerHistory
                    where h.MemberNumber == memberNum && h.regionID == regionID
                    orderby h.TotalScore descending
                    select h).ToList();
                return PlayerHistoryList;
            }
        }

        /// <summary>
        /// Returns a PlayerHistory with the same GameID given
        /// </summary>
        public static PlayerHistory GetPlayerHistoryByGameID (int gameID)
        {
            using (var db = new NineTapDb())
            {
                PlayerHistory playerHistory = 
                    (from h in db.PlayerHistory
                    where h.GameID == gameID
                    select h).SingleOrDefault();
                return playerHistory;
            }
        }

        /// <summary>
        /// Returns the total money won in a PlayerHistory with the same MemberNumber and RegionID
        /// </summary>
        public static decimal GetTotalMoneyWon(int memberNum, int regionID)
        {
            //return the sum of all money won or 0 if no entries are present for this bowler in the database
            var db = new NineTapDb();
            return db.PlayerHistory
                    .Where(p => p.MemberNumber == memberNum && p.regionID == regionID)
                    .Select(p => (decimal?)p.MoneyWon)
                    .Sum() ?? 0; 
        }

        /// <summary>
        /// Returns true if a PlayerHistory with the same GameID given exist in the database
        /// </summary>
        public static bool PlayerHistoryExists(int gameID)
        {
            using (var db = new NineTapDb())
            {
                return db.PlayerHistory
                    .Any(ph => ph.GameID == gameID);
            }
        }

        /// <summary>
        /// Returns true if a PlayerHistory with the same GameID as the PlayerHistory given exist in the database
        /// </summary>
        public static bool PlayerHistoryExists(PlayerHistory ph)
        {
            return PlayerHistoryExists(ph.GameID);
        }
    }
}
