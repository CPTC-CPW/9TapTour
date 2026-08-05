#nullable disable
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Core.Repositories;

public class GameRepository : IGameRepository
{
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public GameRepository(IDbContextFactory<NineTapDb> dbFactory)
    {
        this.dbFactory = dbFactory;
    }

    /// <summary>
    /// Adds a Game that doesn't have an Id in the database. Updates a Game that has an id
    /// that exist in the database
    /// </summary>
    public void AddOrUpdateGame(Game game)
    {
        var db = dbFactory.CreateDbContext();
        db.Entry(game).State = db.Games.Any(g => g.Id == game.Id) ?
                EntityState.Modified : EntityState.Added;

        db.SaveChanges();
    }

    /// <summary>
    /// Adds a Game that doesn't have an Id in the database. Updates a Game that has an id
    /// that exist in the database using an existing DbContext.
    /// NOTE: Does NOT call SaveChanges - caller controls when to save for batch operations.
    /// </summary>
    public void AddOrUpdateGame(Game game, NineTapDb db)
    {
        db.Entry(game).State = db.Games.Any(g => g.Id == game.Id) ?
                EntityState.Modified : EntityState.Added;
    }

    /// <summary>
    /// Returns a game from the Games table by id. Returns null if not found
    /// </summary>
    /// <param name="gameID"></param>
    public Game GetGame(int gameID)
    {
        var db = dbFactory.CreateDbContext();
        return (from g in db.Games
                where g.Id == gameID
                select g).SingleOrDefault();
    }

    /// <summary>
    /// Adds Games that don't have Ids in the database. Updates Games that have ids
    /// that exist in the database
    /// </summary>
    /// <param name="games">The Games to add or update</param>
    public void AddOrUpdateSomeGames(List<Game> games)
    {
        using (var db = dbFactory.CreateDbContext())
        {
            foreach (var currGame in games)
            {
                db.Entry(currGame).State = db.Games.Any(g => g.Id == currGame.Id) ?
                        EntityState.Modified : EntityState.Added;
            }
            db.SaveChanges();
        }
    }

    public Game GetGameInTournament(int memberID, int tournamentID, int squad)
    {
        using (var db = dbFactory.CreateDbContext())
        {
            return (from t in db.Tournaments
                    join p in db.Participants on t.Id equals p.Tournament.Id
                    where t.Id == p.Tournament.Id
                    && memberID == p.Member.Id
                    && t.Id == tournamentID
                    && p.Squad == squad
                    select p.Game).SingleOrDefault();
        }
    }

    public int GetGameID(NineTapDb db, int memberId, int tournyId, int squad)
    {
        return (from p in db.Participants
                where p.Member.Id == memberId
                    && p.Tournament.Id == tournyId
                    && p.Squad == squad
                select p.Game.Id).FirstOrDefault();
    }

    /// <summary>
    /// Gets all finalized games for a tournament (Phase 4: uses Participant.Tournament).
    /// </summary>
    /// <param name="tournamentId">The tournament ID</param>
    /// <returns>List of finalized games</returns>
    public List<Game> GetFinalizedGamesByTournament(int tournamentId)
    {
        using (var db = dbFactory.CreateDbContext())
        {
            // Phase 4: Query via Participant instead of Game.TournamentID
            return (from p in db.Participants
                    join g in db.Games on p.Game.Id equals g.Id
                    where p.Tournament.Id == tournamentId && g.IsFinalized
                    select g).ToList();
        }
    }

    /// <summary>
    /// Gets all finalized games for a member in a specific region (Phase 5: uses Member.NineTapRegionID).
    /// </summary>
    /// <param name="memberNumber">The member number</param>
    /// <param name="regionId">The region ID</param>
    /// <returns>List of finalized games</returns>
    public List<Game> GetFinalizedGamesByMember(int memberNumber, int regionId)
    {
        using (var db = dbFactory.CreateDbContext())
        {
            // Phase 5: Use Member.NineTapRegionID instead of Participant.ParticipantRegionID
            return (from p in db.Participants
                    join m in db.Members on p.Member.Id equals m.Id
                    join g in db.Games on p.Game.Id equals g.Id
                    where m.Number == memberNumber && g.IsFinalized
                    select g).ToList();
        }
    }

    /// <summary>
    /// Checks if a game is finalized (Phase 2 refactoring).
    /// </summary>
    /// <param name="gameId">The game ID</param>
    /// <returns>True if game is finalized, false otherwise</returns>
    public bool IsGameFinalized(int gameId)
    {
        using (var db = dbFactory.CreateDbContext())
        {
            return db.Games.Any(g => g.Id == gameId && g.IsFinalized);
        }
    }
}
