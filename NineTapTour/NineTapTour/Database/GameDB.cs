using Microsoft.EntityFrameworkCore;
using NineTapTour.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Database
{
    public static class GameDB
    {
        /// <summary>
        /// Adds a Game that doesn't have an Id in the database. Updates a Game that has an id 
        /// that exist in the database
        /// </summary>
        public static void AddOrUpdateGame(Game game)
        {
            var db = new NineTapDb();
            db.Entry(game).State = db.Games.Any(g => g.Id == game.Id) ?
                    EntityState.Modified : EntityState.Added;

            db.SaveChanges();
        }

        /// <summary>
        /// Returns a game from the Games table by id. Returns null if not found
        /// </summary>
        /// <param name="gameID"></param>
        public static Game GetGame(int gameID)
        {
            var db = new NineTapDb();
            return (from g in db.Games
                    where g.Id == gameID
                    select g).SingleOrDefault();
        }

        /// <summary>
        /// Adds Games that don't have Ids in the database. Updates Games that have ids 
        /// that exist in the database
        /// </summary>
        /// <param name="games">The Games to add or update</param>
        public static void AddOrUpdateSomeGames(List<Game> games)
        {
            using (var db = new NineTapDb())
            {
                foreach (var currGame in games)
                {
                    db.Entry(currGame).State = db.Games.Any(g => g.Id == currGame.Id) ?
                            EntityState.Modified : EntityState.Added;
                }
                db.SaveChanges();
            }
        }

        public static Game GetGameInTournament(int memberID, int tournamentID, int squad)
        {
            using (NineTapDb db = new NineTapDb())
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

        public static int GetGameID(NineTapDb db, int memberId, int tournyId, int squad)
        {
            return (from p in db.Participants
                    where p.Member.Id == memberId
                        && p.Tournament.Id == tournyId
                        && p.Squad == squad
                    select p.Game.Id).FirstOrDefault();
        }
    }
}
