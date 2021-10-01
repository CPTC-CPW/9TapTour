using NineTapTour.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineTapTour.Models;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

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
            using(var db = new NineTapDb())
            {
                var queryResult = db.PlayerHistory
                    .Where(ph => ph.MemberNumber == memberNum && ph.regionID == regionID)
                    .OrderByDescending(ph => ph.TournamentDate)
                    .Select(ph => new {ph.TournamentDate, ph.MoneyWon})
                    .Take(howmany)
                    .ToList();

                return queryResult.Select(qr => new PlayerHistory()
                {
                    TournamentDate = qr.TournamentDate,
                    MoneyWon = qr.MoneyWon
                }).ToList();
            }
        }

        /// <summary>
        /// Returns a list of the last 5 PlayerHistories with the given MemberNumber and RegionID
        /// </summary>
        public static List<PlayerHistory> GetLastFiveTournaments(int memberNum, int regionID)
        {
            const int HOW_MANY = 5;
            using (var db = new NineTapDb())
            {
                // Will only grab the last 5 PlayerHistories where the AVG was adjusted, 
                // that way the bonus pins can't be affected by bowling in more than one squad
                List<PlayerHistory> PlayerHistoryList = 
                    (from h in db.PlayerHistory
                    where h.MemberNumber == memberNum && h.regionID == regionID && h.AVG > 0 
                    orderby h.TournamentDate descending, h.hisID descending
                    select h).Take(HOW_MANY).ToList();
                return PlayerHistoryList;
            }
        }

        /// <summary>
        /// Will only grab the most recent PlayerHistory where the AVG was adjusted, 
        /// that way the bonus pins can't be affected by bowling in more than one squad.
        /// Returns null if no recent player history is found
        /// </summary>
        /// <param name="memberNum">The bowlers MemberNumber</param>
        /// <param name="regionID">Region of the Tournament</param>
        /// <returns></returns>
        public static PlayerHistory GetMostRecentTournament(int memberNum, int regionID)
        {
            using var db = new NineTapDb();
            PlayerHistory mostRecentTournament =
                (from h in db.PlayerHistory
                 where h.MemberNumber == memberNum && h.regionID == regionID && h.AVG > 0
                 orderby h.TournamentDate descending, h.hisID descending
                 select h).Take(1).SingleOrDefault();
            return mostRecentTournament;
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
            using (var db = new NineTapDb())
            {
                return db.PlayerHistory
                        .Where(p => p.MemberNumber == memberNum && p.regionID == regionID)
                        .Select(p => (decimal?)p.MoneyWon)
                        .Sum() ?? 0;
            }
        }

        public static int GetTotalGamesPlayed(int memberNum, int regionID)
        {
            using (var db = new NineTapDb())
            {
                return db.PlayerHistory
                    .Where(p => p.MemberNumber == memberNum && p.regionID == regionID)
                        .Select(p => (int?)p.GamesPlayed)
                        .Sum() ?? 0;
            }
        }

        public static int GetTotalGamesPlayedFromHistory(int memberNum, int regionID, int numberOfEntriesToTake)
        {
            using NineTapDb db = new();
            return db.PlayerHistory
                .Where(p => p.MemberNumber == memberNum && p.regionID == regionID)
                .OrderByDescending(p => p.TournamentDate)
                .ThenByDescending(p => p.TotalScore)
                .Select(p => p.GamesPlayed)
                .Take(numberOfEntriesToTake)
                .Sum();
        }

        //return the sum of game 1 total played or 0 if no entries are present for this bowler in the database
        public static int GetTotalGame1Played(int memberNum, int regionID)
        {
            using (var db = new NineTapDb())
            {
                return db.PlayerHistory
                    .Where(p => p.MemberNumber == memberNum && p.regionID == regionID)
                        .Select(p => (int?)p.Game1)
                        .Sum() ?? 0;
            }
        }

        //return the sum of game 2 total played or 0 if no entries are present for this bowler in the database
        public static int GetTotalGame2Played(int memberNum, int regionID)
        {
            using (var db = new NineTapDb())
            {
                return db.PlayerHistory
                    .Where(p => p.MemberNumber == memberNum && p.regionID == regionID)
                        .Select(p => (int?)p.Game2)
                        .Sum() ?? 0;
            }
        }

        //return the sum of game 3 total played or 0 if no entries are present for this bowler in the database
        public static int GetTotalGame3Played(int memberNum, int regionID)
        {
            using (var db = new NineTapDb())
            {
                return db.PlayerHistory
                    .Where(p => p.MemberNumber == memberNum && p.regionID == regionID)
                        .Select(p => (int?)p.Game3)
                        .Sum() ?? 0;
            }
        }

        //return the sum of game 4 total played or 0 if no entries are present for this bowler in the database
        public static int GetTotalGame4Played(int memberNum, int regionID)
        {
            using (var db = new NineTapDb())
            {
                return db.PlayerHistory
                    .Where(p => p.MemberNumber == memberNum && p.regionID == regionID)
                        .Select(p => (int?)p.Game4)
                        .Sum() ?? 0;
            }
        }

        /// <summary>
        /// Returns the total sum of a game score (1,2,3, or 4) for a specific member.
        /// To return a 30 game total, take 30 games - the number of entries in the current tournament
        /// </summary>
        /// <param name="memberNum"></param>
        /// <param name="regionID"></param>
        /// <param name="numberOfEntriesToTake">The number of entries from playerhistory to take</param>
        /// <returns></returns>
        public static int GetGame1TotalFromHistory(int memberNum, int regionID, int numberOfEntriesToTake)
        {
            using NineTapDb db = new();
            return db.PlayerHistory
                .Where(p => p.MemberNumber == memberNum && p.regionID == regionID)
                .OrderByDescending(p => p.TournamentDate)
                .ThenByDescending(p => p.TotalScore)
                .Select(p => p.Game1)
                .Take(numberOfEntriesToTake)
                .Sum() ?? 0;
        }

        public static int GetGame2TotalFromHistory(int memberNum, int regionID, int numberOfEntriesToTake)
        {
            using NineTapDb db = new();
            return db.PlayerHistory
                .Where(p => p.MemberNumber == memberNum && p.regionID == regionID)
                .OrderByDescending(p => p.TournamentDate)
                .ThenByDescending(p => p.TotalScore)
                .Select(p => p.Game2)
                .Take(numberOfEntriesToTake)
                .Sum() ?? 0;
        }

        public static int GetGame3TotalFromHistory(int memberNum, int regionID, int numberOfEntriesToTake)
        {
            using NineTapDb db = new();
            return db.PlayerHistory
                .Where(p => p.MemberNumber == memberNum && p.regionID == regionID)
                .OrderByDescending(p => p.TournamentDate)
                .ThenByDescending(p => p.TotalScore)
                .Select(p => p.Game3)
                .Take(numberOfEntriesToTake)
                .Sum() ?? 0;
        }

        public static int GetGame4TotalFromHistory(int memberNum, int regionID, int numberOfEntriesToTake)
        {
            using NineTapDb db = new();
            return db.PlayerHistory
                .Where(p => p.MemberNumber == memberNum && p.regionID == regionID)
                .OrderByDescending(p => p.TournamentDate)
                .ThenByDescending(p => p.TotalScore)
                .Select(p => p.Game4)
                .Take(numberOfEntriesToTake)
                .Sum() ?? 0;
        }

        /// <summary>
        /// Return the sum of scratch total played from desired number of games.
        /// </summary>
        /// <param name="memberNum"></param>
        /// <param name="regionID"></param>
        /// <param name="numberOfGamesToTake">Number of games to take from history. If 30 total is needed, 
        /// subtract the number of entries from the current tournament</param>
        /// <returns></returns>
        public static int GetScratchTotalFromHistory(int memberNum, int regionID, int numberOfGamesToTake)
        {
            using NineTapDb db = new();
            return db.PlayerHistory
                .Where(p => p.MemberNumber == memberNum && p.regionID == regionID)
                .OrderByDescending(p => p.TournamentDate)
                .ThenByDescending(p => p.TotalScore)
                .Select(p => p.TotalScore)
                .Take(numberOfGamesToTake)
                .Sum();
        }

        //return the sum of handiCap total played or 0 if no entries are present for this bowler in the database
        public static int GetEntryAvgTotal(int memberNum, int regionID, int game1, int game2, int game3, int game4, int games)
        {
            using (var db = new NineTapDb())
            {
                int game1sum = GetTotalGame1Played(memberNum, regionID);
                int game2sum = GetTotalGame2Played(memberNum, regionID);
                int game3sum = GetTotalGame3Played(memberNum, regionID);
                int game4sum = GetTotalGame4Played(memberNum, regionID);
                int gametotalsum = GetTotalGamesPlayed(memberNum, regionID);
                return (game1sum + game2sum + game3sum + game4sum + game1 + game2 + game3 + game4) / (gametotalsum + games);

            }
        }

        public static int GetGameAvgFromHistory(int memberNum, int regionID, int numberOfEntriesToTake)
        {
            using NineTapDb db = new();
            return db.PlayerHistory
                .Where(p => p.MemberNumber == memberNum && p.regionID == regionID)
                .OrderByDescending(p => p.TournamentDate)
                .ThenBy(p => p.TotalScore)
                .Select(p => p.TotalScore)
                .Take(numberOfEntriesToTake)
                .Sum();
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

        internal static int GetTotalNumberOfEntries(int memberNumber, int regionID)
        {
            using NineTapDb db = new();
            return db.PlayerHistory.Where(p => p.MemberNumber == memberNumber && p.regionID == regionID).Count();
        }

        internal static int GetNumberOfGamesFromHistory(int memberNumber, int regionID, int numberOfEntries)
        {
            using NineTapDb db = new();
            return db.PlayerHistory
                .Where(p => p.MemberNumber == memberNumber && p.regionID == regionID)
                .OrderByDescending(p => p.TournamentDate)
                .ThenBy(p => p.TotalScore)
                .Select(p => p.GamesPlayed)
                .Take(numberOfEntries)
                .Sum();
        }
    }
}
