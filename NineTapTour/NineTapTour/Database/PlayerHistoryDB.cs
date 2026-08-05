using NineTapTour.Core.Data;
using System.Collections.Generic;
using System.Linq;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace NineTapTour.Database;

public class PlayerHistoryDB
{
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
                .ThenByDescending(g => g.MoneyWon) // Ensure entries are ordered by place standing within the same tournament date (legacy tournaments do not have places stored)
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
                MemberNumber = memberNum
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
                         && g.IsFinalized
                         && g.AdjustedAvg > 0) // Only games where AVG was adjusted
                .OrderByDescending(g => g.Participant.Tournament.Date)
                .ThenByDescending(g => g.Id)
                .Take(HOW_MANY)
                .ToList();

            return games.Select(g => new PlayerHistoryViewModel(
                g,
                memberNum,
                g.Participant.Tournament.Date
            )).ToList();
        }
    }

    /// <summary>
    /// Returns the most recent finalized game where AVG was adjusted.
    /// Only grabs games where AVG was adjusted so bonus pins aren't affected by bowling in multiple squads.
    /// Returns null if no recent player history is found.
    /// </summary>
    public static PlayerHistoryViewModel GetMostRecentTournament(int memberNum)
    {
        using var db = new NineTapDb();
        
        var game = db.Games
            .Include(g => g.Participant)
                .ThenInclude(p => p.Member)
            .Include(g => g.Participant.Tournament)
            .Where(g => g.Participant.Member.Number == memberNum
                     && g.IsFinalized
                     && g.AdjustedAvg > 0) // Only games where AVG was adjusted
            .OrderByDescending(g => g.Participant.Tournament.Date)
            .ThenByDescending(g => g.Id)
            .FirstOrDefault();

        return game == null ? null : new PlayerHistoryViewModel(
            game,
            memberNum,
            game.Participant.Tournament.Date
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

    public static int? GetMostRecentAverage(int memberNum)
    {
        using var db = new NineTapDb();
        var game = db.Games
            .Include(g => g.Participant)
                .ThenInclude(p => p.Member)
            .Include(g => g.Participant.Tournament)
            .Where(g => g.Participant.Member.Number == memberNum
                     && g.IsFinalized
                     && g.AdjustedAvg > 0) // Only games where AVG was adjusted
            .OrderByDescending(g => g.Participant.Tournament.Date)
            .ThenByDescending(g => g.Id)
            .FirstOrDefault();
        return game?.AdjustedAvg ?? null;
    }
}