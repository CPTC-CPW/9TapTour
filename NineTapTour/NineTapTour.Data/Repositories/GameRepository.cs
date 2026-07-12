using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Abstractions;
using NineTapTour.Database;
using NineTapTour.Models;

namespace NineTapTour.Data.Repositories
{
    /// <summary>EF Core implementation of <see cref="IGameRepository"/> (formerly <c>GameDB</c>).</summary>
    public sealed class GameRepository : IGameRepository
    {
        private readonly IDbContextFactory<NineTapDb> _factory;

        public GameRepository(IDbContextFactory<NineTapDb> factory) => _factory = factory;

        /// <summary>
        /// Adds a Game that doesn't have an Id in the database. Updates a Game that has an id
        /// that exist in the database
        /// </summary>
        public void AddOrUpdateGame(Game game)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(game).State = db.Games.Any(g => g.Id == game.Id) ?
                    EntityState.Modified : EntityState.Added;

            db.SaveChanges();
        }

        /// <summary>
        /// Returns a game from the Games table by id. Returns null if not found
        /// </summary>
        /// <param name="gameID"></param>
        public Game GetGame(int gameID)
        {
            using var db = _factory.CreateDbContext();
            return (from g in db.Games.AsNoTracking()
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
            using (var db = _factory.CreateDbContext())
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
            using (var db = _factory.CreateDbContext())
            {
                return (from t in db.Tournaments.AsNoTracking()
                        join p in db.Participants on t.Id equals p.Tournament.Id
                        where t.Id == p.Tournament.Id
                        && memberID == p.Member.Id
                        && t.Id == tournamentID
                        && p.Squad == squad
                        select p.Game).SingleOrDefault();
            }
        }

        public int GetGameID(int memberId, int tournyId, int squad)
        {
            using var db = _factory.CreateDbContext();
            return (from p in db.Participants.AsNoTracking()
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
            using (var db = _factory.CreateDbContext())
            {
                // Phase 4: Query via Participant instead of Game.TournamentID
                return (from p in db.Participants.AsNoTracking()
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
            using (var db = _factory.CreateDbContext())
            {
                // Phase 5: Use Member.NineTapRegionID instead of Participant.ParticipantRegionID
                return (from p in db.Participants.AsNoTracking()
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
            using (var db = _factory.CreateDbContext())
            {
                return db.Games.AsNoTracking().Any(g => g.Id == gameId && g.IsFinalized);
            }
        }
    }
}
