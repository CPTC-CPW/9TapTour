#nullable disable
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Core.Repositories;

public class PlayerHistoryRepository : IPlayerHistoryRepository
{
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public PlayerHistoryRepository(IDbContextFactory<NineTapDb> dbFactory)
    {
        this.dbFactory = dbFactory;
    }

    /// <summary>
    /// Returns a list of all player histories with the given memberNumber.
    /// Queries from Games table (single source of truth).
    /// </summary>
    public List<PlayerHistoryViewModel> GetMemberPlayerHistory(int memberNum)
    {
        using (var db = dbFactory.CreateDbContext())
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
    public List<PlayerHistoryViewModel> GetLastQtyGamesMoneyWon(int memberNum, int howmany)
    {
        using(var db = dbFactory.CreateDbContext())
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
    public List<PlayerHistoryViewModel> GetLastFiveTournaments(int memberNum)
    {
        const int HOW_MANY = 5;
        using (var db = dbFactory.CreateDbContext())
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
    public PlayerHistoryViewModel GetMostRecentTournament(int memberNum)
    {
        using var db = dbFactory.CreateDbContext();

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
    public void DeleteGame(Game game)
    {
        using (var db = dbFactory.CreateDbContext())
        {
            db.Entry(game).State = EntityState.Deleted;
            db.SaveChanges();
        }
    }

    /// <summary>
    /// Returns the total money won by a member.
    /// </summary>
    public decimal GetTotalMoneyWon(int memberNum)
    {
        using (var db = dbFactory.CreateDbContext())
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

    public int? GetMostRecentAverage(int memberNum)
    {
        using var db = dbFactory.CreateDbContext();
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
