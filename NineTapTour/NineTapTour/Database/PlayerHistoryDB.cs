using System;
using System.Collections.Generic;
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
        /// Returns the top 30 player histories with the same MemberNumber as the one given.
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static List<PlayerHistoryViewModel> GetTop30FromPlayerHistory(int memberNum)
        {
            const int howmany = 30;
            using (var db = new NineTapDb())
            {
                var games = db.Games
                    .Include(g => g.Participant)
                        .ThenInclude(p => p.Member)
                    .Include(g => g.Participant.Tournament)
                    .Where(g => g.Participant.Member.Number == memberNum 
                             && g.IsFinalized)
                    .OrderByDescending(g => g.Participant.Tournament.Date)
                    .Take(howmany)
                    .ToList();

                return games.Select(g => new PlayerHistoryViewModel(
                    g,
                    memberNum,
                    g.Participant.Tournament.Date
                )).ToList();
            }
        }

        /// <summary>
        /// Returns player histories from Games table (single source of truth).
        /// Gets the specified number of most recent finalized games for a member.
        /// </summary>
        public static List<PlayerHistoryViewModel> GetPlayerHistories(int memberNum, int numEntries)
        {
            using NineTapDb db = new();
            
            // Query from Games table instead of PlayerHistory
            var games = db.Games
                .Include(g => g.Participant)
                    .ThenInclude(p => p.Member)
                .Include(g => g.Participant.Tournament)
                .Where(g => g.Participant.Member.Number == memberNum 
                         && g.IsFinalized)
                .OrderByDescending(g => g.Participant.Tournament.Date)
                .ThenByDescending(g => g.ScratchTotal)
                .Take(numEntries)
                .ToList();

            // Convert Game entities to PlayerHistoryViewModel
            return games.Select(g => new PlayerHistoryViewModel(
                g,
                memberNum,
                g.Participant.Tournament.Date
            )).ToList();
        }

        /// <summary>
        /// Finds the hisID from the player history view model given.
        /// Returns GameID as hisID (they are the same in the refactored model).
        /// </summary>
        public static int GetHisID(PlayerHistoryViewModel playerHistory)
        {
            // In the refactored model, hisID = GameID
            return playerHistory.GameID;
        }

        /// <summary>
        /// Returns a list of all player histories with the given memberNumber.
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static List<PlayerHistoryViewModel> GetMemberPlayerHistory(int memberNum)
        {
            using (var db = new NineTapDb())
            {
                // Query from Games table instead of PlayerHistory
                var games = db.Games
                    .Include(g => g.Participant)
                        .ThenInclude(p => p.Member)
                    .Include(g => g.Participant.Tournament)
                    .Where(g => g.Participant.Member.Number == memberNum 
                             && g.IsFinalized)
                    .OrderByDescending(g => g.Participant.Tournament.Date)
                    .ToList();

                // Convert Game entities to PlayerHistoryViewModel
                return games.Select(g => new PlayerHistoryViewModel(
                    g,
                    memberNum,
                    g.Participant.Tournament.Date
                )).ToList();
            }
        }

        /// <summary>
        /// Returns a list of the last 30 player histories with the given memberNumber
        /// </summary>
        public static List<PlayerHistoryViewModel> GetMemberPlayerHistoryCount(int memberNum)
        {
            using (var db = new NineTapDb())
            {
                var games = db.Games
                    .Include(g => g.Participant)
                        .ThenInclude(p => p.Member)
                    .Include(g => g.Participant.Tournament)
                    .Where(g => g.Participant.Member.Number == memberNum 
                             && g.IsFinalized) // Only finalized games
                    .OrderByDescending(g => g.Participant.Tournament.Date)
                    .ThenByDescending(g => g.MoneyWon)
                    .Take(30)
                    .ToList();

                return games.Select(g => new PlayerHistoryViewModel(
                    g,
                    memberNum,
                    g.Participant.Tournament.Date
                )).ToList();
            }
        }

        /// <summary>
        /// Returns a list of all player histories.
        /// </summary>
        public static List<PlayerHistoryViewModel> GetAllPlayerHistory()
        {
            using (var db = new NineTapDb())
            {
                var games = db.Games
                    .Include(g => g.Participant)
                        .ThenInclude(p => p.Member)
                    .Include(g => g.Participant.Tournament)
                    .Where(g => g.IsFinalized)
                    .OrderByDescending(g => g.Participant.Tournament.Date)
                    .ToList();

                return games.Select(g => new PlayerHistoryViewModel(
                    g,
                    g.Participant.Member.Number,
                    g.Participant.Tournament.Date
                )).ToList();
            }
        }

        /// <summary>
        /// Gets the last quantity of games selecting only the tournament date and money won.
        /// Used to calculate bonus pins.
        /// </summary>
        public static List<PlayerHistoryViewModel> GetLastQtyGamesMoneyWon(int memberNum, int howmany)
        {
            using(var db = new NineTapDb())
            {
                var games = db.Games
                    .Include(g => g.Participant)
                        .ThenInclude(p => p.Member)
                    .Include(g => g.Participant.Tournament)
                    .Where(g => g.Participant.Member.Number == memberNum
                             && g.IsFinalized)
                    .OrderByDescending(g => g.Participant.Tournament.Date)
                    .Take(howmany)
                    .ToList();

                return games.Select(g => new PlayerHistoryViewModel
                {
                    TournamentDate = g.Participant.Tournament.Date,
                    MoneyWon = g.MoneyWon ?? 0,
                    GameID = g.Id,
                    MemberNumber = memberNum,
                    regionID = regionID
                }).ToList();
            }
        }

        /// <summary>
        /// Returns a list of the last 5 finalized games where AVG was adjusted.
        /// Only grabs games where AVG was adjusted so bonus pins aren't affected by bowling in multiple squads.
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static List<PlayerHistoryViewModel> GetLastFiveTournaments(int memberNum)
        {
            const int HOW_MANY = 5;
            using (var db = new NineTapDb())
            {
                var games = db.Games
                    .Include(g => g.Participant)
                        .ThenInclude(p => p.Member)
                    .Include(g => g.Participant.Tournament)
                    .Where(g => g.Participant.Member.Number == memberNum 
                             && g.Participant.Member.NineTapRegionID == regionID 
                             && g.IsFinalized
                             && g.AdjustedAvg > 0) // Only games where AVG was adjusted
                    .OrderByDescending(g => g.Participant.Tournament.Date)
                    .ThenByDescending(g => g.Id)
                    .Take(HOW_MANY)
                    .ToList();

                return games.Select(g => new PlayerHistoryViewModel(
                    g,
                    memberNum,
                    g.Participant.Tournament.Date,
                    regionID
                )).ToList();
            }
        }

        /// <summary>
        /// Returns the most recent finalized game where AVG was adjusted.
        /// Only grabs games where AVG was adjusted so bonus pins aren't affected by bowling in multiple squads.
        /// Returns null if no recent player history is found.
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static PlayerHistoryViewModel GetMostRecentTournament(int memberNum, int regionID)
        {
            using var db = new NineTapDb();
            
            var game = db.Games
                .Include(g => g.Participant)
                    .ThenInclude(p => p.Member)
                .Include(g => g.Participant.Tournament)
                .Where(g => g.Participant.Member.Number == memberNum 
                         && g.Participant.Member.NineTapRegionID == regionID 
                         && g.IsFinalized
                         && g.AdjustedAvg > 0) // Only games where AVG was adjusted
                .OrderByDescending(g => g.Participant.Tournament.Date)
                .ThenByDescending(g => g.Id)
                .FirstOrDefault();

            return game == null ? null : new PlayerHistoryViewModel(
                game,
                memberNum,
                game.Participant.Tournament.Date,
                regionID
            );
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
        /// Returns a list of player histories ordered by TotalScore descending.
        /// </summary>
        public static List<PlayerHistoryViewModel> GetMemberPlayerHistoryByTotal(int memberNum)
        {
            using (var db = new NineTapDb())
            {
                // Materialize the query first with ToList() since ScratchTotal is a calculated property
                var games = db.Games
                    .Include(g => g.Participant)
                        .ThenInclude(p => p.Member)
                    .Include(g => g.Participant.Tournament)
                    .Where(g => g.Participant.Member.Number == memberNum
                             && g.IsFinalized)
                    .ToList();

                // Now order by ScratchTotal (calculated property) in memory
                var orderedGames = games.OrderByDescending(g => g.ScratchTotal).ToList();

                return orderedGames.Select(g => new PlayerHistoryViewModel(
                    g,
                    memberNum,
                    g.Participant.Tournament.Date,
                    regionID
                )).ToList();
            }
        }

        /// <summary>
        /// Returns a player history view model with the same GameID given.
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static PlayerHistoryViewModel GetPlayerHistoryByGameID(int gameID)
        {
            using (var db = new NineTapDb())
            {
                var game = db.Games
                    .Include(g => g.Participant)
                        .ThenInclude(p => p.Member)
                    .Include(g => g.Participant.Tournament)
                    .Where(g => g.Id == gameID && g.IsFinalized)
                    .FirstOrDefault();

                if (game == null)
                    return null;

                return new PlayerHistoryViewModel(
                    game,
                    game.Participant.Member.Number,
                    game.Participant.Tournament.Date,
                    game.Participant.Member.NineTapRegionID
                );
            }
        }

        /// <summary>
        /// Returns the total money won by a member.
        /// </summary>
        public static decimal GetTotalMoneyWon(int memberNum)
        {
            using (var db = new NineTapDb())
            {
                // Query from Games table instead of PlayerHistory
                return db.Games
                    .Include(g => g.Participant)
                        .ThenInclude(p => p.Member)
                    .Where(g => g.Participant.Member.Number == memberNum 
                             && g.IsFinalized) // Only finalized games
                    .Select(g => (decimal?)(g.MoneyWon ?? 0))
                    .Sum() ?? 0;
            }
        }

        /// <summary>
        /// Returns total games played by a member in a region.
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static int GetTotalGamesPlayed(int memberNum, int regionID)
        {
            using (var db = new NineTapDb())
            {
                return db.Games
                    .Include(g => g.Participant)
                        .ThenInclude(p => p.Member)
                    .Where(g => g.Participant.Member.Number == memberNum 
                             && g.Participant.Member.NineTapRegionID == regionID
                             && g.IsFinalized)
                    .Select(g => g.GamesPlayed)
                    .Sum();
            }
        }

        /// <summary>
        /// Gets total games played from history (last N entries).
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static int GetTotalGamesPlayedFromHistory(int memberNum, int regionID, int numberOfEntriesToTake)
        {
            using NineTapDb db = new();
            
            // Query from Games table instead of PlayerHistory
            return db.Games
                .Include(g => g.Participant)
                    .ThenInclude(p => p.Member)
                .Include(g => g.Participant.Tournament)
                .Where(g => g.Participant.Member.Number == memberNum 
                         && g.Participant.Member.NineTapRegionID == regionID
                         && g.IsFinalized) // Only finalized games
                .OrderByDescending(g => g.Participant.Tournament.Date)
                .ThenByDescending(g => g.ScratchTotal)
                .Take(numberOfEntriesToTake)
                .Select(g => g.GamesPlayed)
                .Sum();
        }

        /// <summary>
        /// Returns the sum of game 1 total played.
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static int GetTotalGame1Played(int memberNum, int regionID)
        {
            using (var db = new NineTapDb())
            {
                return db.Games
                    .Include(g => g.Participant)
                        .ThenInclude(p => p.Member)
                    .Where(g => g.Participant.Member.Number == memberNum 
                             && g.Participant.Member.NineTapRegionID == regionID
                             && g.IsFinalized)
                    .Select(g => g.Game1)
                    .Sum() ?? 0;
            }
        }

        /// <summary>
        /// Returns the sum of game 2 total played.
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static int GetTotalGame2Played(int memberNum, int regionID)
        {
            using (var db = new NineTapDb())
            {
                return db.Games
                    .Include(g => g.Participant)
                        .ThenInclude(p => p.Member)
                    .Where(g => g.Participant.Member.Number == memberNum 
                             && g.Participant.Member.NineTapRegionID == regionID
                             && g.IsFinalized)
                    .Select(g => g.Game2)
                    .Sum() ?? 0;
            }
        }

        /// <summary>
        /// Returns the sum of game 3 total played.
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static int GetTotalGame3Played(int memberNum, int regionID)
        {
            using (var db = new NineTapDb())
            {
                return db.Games
                    .Include(g => g.Participant)
                        .ThenInclude(p => p.Member)
                    .Where(g => g.Participant.Member.Number == memberNum 
                             && g.Participant.Member.NineTapRegionID == regionID
                             && g.IsFinalized)
                    .Select(g => g.Game3)
                    .Sum() ?? 0;
            }
        }

        /// <summary>
        /// Returns the sum of game 4 total played.
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static int GetTotalGame4Played(int memberNum, int regionID)
        {
            using (var db = new NineTapDb())
            {
                return db.Games
                    .Include(g => g.Participant)
                        .ThenInclude(p => p.Member)
                    .Where(g => g.Participant.Member.Number == memberNum 
                             && g.Participant.Member.NineTapRegionID == regionID
                             && g.IsFinalized)
                    .Select(g => g.Game4)
                    .Sum() ?? 0;
            }
        }

        /// <summary>
        /// Returns the total sum of Game1 scores from history.
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static int GetGame1TotalFromHistory(int memberNum, int regionID, int numberOfEntriesToTake)
        {
            using NineTapDb db = new();
            
            return db.Games
                .Include(g => g.Participant)
                    .ThenInclude(p => p.Member)
                .Include(g => g.Participant.Tournament)
                .Where(g => g.Participant.Member.Number == memberNum 
                         && g.Participant.Member.NineTapRegionID == regionID
                         && g.IsFinalized)
                .OrderByDescending(g => g.Participant.Tournament.Date)
                .ThenByDescending(g => g.ScratchTotal)
                .Take(numberOfEntriesToTake)
                .Select(g => g.Game1)
                .Sum() ?? 0;
        }

        /// <summary>
        /// Returns the total sum of Game2 scores from history.
        /// </summary>
        public static int GetGame2TotalFromHistory(int memberNum, int regionID, int numberOfEntriesToTake)
        {
            using NineTapDb db = new();
            
            return db.Games
                .Include(g => g.Participant)
                    .ThenInclude(p => p.Member)
                .Include(g => g.Participant.Tournament)
                .Where(g => g.Participant.Member.Number == memberNum 
                         && g.Participant.Member.NineTapRegionID == regionID
                         && g.IsFinalized)
                .OrderByDescending(g => g.Participant.Tournament.Date)
                .ThenByDescending(g => g.ScratchTotal)
                .Take(numberOfEntriesToTake)
                .Select(g => g.Game2)
                .Sum() ?? 0;
        }

        /// <summary>
        /// Returns the total sum of Game3 scores from history.
        /// </summary>
        public static int GetGame3TotalFromHistory(int memberNum, int regionID, int numberOfEntriesToTake)
        {
            using NineTapDb db = new();
            
            return db.Games
                .Include(g => g.Participant)
                    .ThenInclude(p => p.Member)
                .Include(g => g.Participant.Tournament)
                .Where(g => g.Participant.Member.Number == memberNum 
                         && g.Participant.Member.NineTapRegionID == regionID
                         && g.IsFinalized)
                .OrderByDescending(g => g.Participant.Tournament.Date)
                .ThenByDescending(g => g.ScratchTotal)
                .Take(numberOfEntriesToTake)
                .Select(g => g.Game3)
                .Sum() ?? 0;
        }

        /// <summary>
        /// Returns the total sum of Game4 scores from history.
        /// </summary>
        public static int GetGame4TotalFromHistory(int memberNum, int regionID, int numberOfEntriesToTake)
        {
            using NineTapDb db = new();
            
            return db.Games
                .Include(g => g.Participant)
                    .ThenInclude(p => p.Member)
                .Include(g => g.Participant.Tournament)
                .Where(g => g.Participant.Member.Number == memberNum 
                         && g.Participant.Member.NineTapRegionID == regionID
                         && g.IsFinalized)
                .OrderByDescending(g => g.Participant.Tournament.Date)
                .ThenByDescending(g => g.ScratchTotal)
                .Take(numberOfEntriesToTake)
                .Select(g => g.Game4)
                .Sum() ?? 0;
        }

        /// <summary>
        /// Returns the sum of scratch totals from history.
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static int GetScratchTotalFromHistory(int memberNum, int regionID, int numberOfGamesToTake)
        {
            using NineTapDb db = new();
            
            return db.Games
                .Include(g => g.Participant)
                    .ThenInclude(p => p.Member)
                .Include(g => g.Participant.Tournament)
                .Where(g => g.Participant.Member.Number == memberNum 
                         && g.Participant.Member.NineTapRegionID == regionID
                         && g.IsFinalized)
                .OrderByDescending(g => g.Participant.Tournament.Date)
                .ThenByDescending(g => g.ScratchTotal)
                .Take(numberOfGamesToTake)
                .Select(g => g.ScratchTotal)
                .Sum();
        }

        /// <summary>
        /// Returns the sum of entry averages (calculated value for display purposes).
        /// </summary>
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

        /// <summary>
        /// Returns the sum of game averages from history.
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static int GetGameAvgFromHistory(int memberNum, int regionID, int numberOfEntriesToTake)
        {
            using NineTapDb db = new();
            
            return db.Games
                .Include(g => g.Participant)
                    .ThenInclude(p => p.Member)
                .Include(g => g.Participant.Tournament)
                .Where(g => g.Participant.Member.Number == memberNum 
                         && g.Participant.Member.NineTapRegionID == regionID
                         && g.IsFinalized)
                .OrderByDescending(g => g.Participant.Tournament.Date)
                .ThenBy(g => g.ScratchTotal)
                .Take(numberOfEntriesToTake)
                .Select(g => g.ScratchTotal)
                .Sum();
        }

        /// <summary>
        /// Returns true if a Game with the given GameID exists and is finalized.
        /// Queries from Games table (single source of truth).
        /// </summary>
        public static bool GameExists(int gameID)
        {
            using (var db = new NineTapDb())
            {
                // Check Games table
                return db.Games.Any(g => g.Id == gameID && g.IsFinalized);
            }
        }

        /// <summary>
        /// Returns true if a player history view model with the same GameID exists in the database
        /// </summary>
        public static bool GameExists(PlayerHistoryViewModel ph)
        {
            return GameExists(ph.GameID);
        }

        /// <summary>
        /// Returns the total number of finalized game entries for a member.
        /// </summary>
        internal static int GetTotalNumberOfEntries(int memberNumber, int regionID)
        {
            using NineTapDb db = new();
            
            return db.Games
                .Include(g => g.Participant)
                    .ThenInclude(p => p.Member)
                .Where(g => g.Participant.Member.Number == memberNumber 
                         && g.Participant.Member.NineTapRegionID == regionID
                         && g.IsFinalized)
                .Count();
        }

        /// <summary>
        /// Returns the total number of games played from history entries.
        /// </summary>
        internal static int GetNumberOfGamesFromHistory(int memberNumber, int regionID, int numberOfEntries)
        {
            using NineTapDb db = new();
            
            return db.Games
                .Include(g => g.Participant)
                    .ThenInclude(p => p.Member)
                .Include(g => g.Participant.Tournament)
                .Where(g => g.Participant.Member.Number == memberNumber 
                         && g.Participant.Member.NineTapRegionID == regionID
                         && g.IsFinalized)
                .OrderByDescending(g => g.Participant.Tournament.Date)
                .ThenBy(g => g.ScratchTotal)
                .Take(numberOfEntries)
                .Select(g => g.GamesPlayed)
                .Sum();
        }
    }
}
