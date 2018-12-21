using NineTapTour.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
        /// <param name="game"></param>
        public static void AddOrUpdateGame(Game game)
        {
            var db = new NineTapDb();
            db.Entry(game).State = db.Games.Any(g => g.Id == game.Id) ?
                    EntityState.Modified :
                    EntityState.Added;

            db.SaveChanges();
        }


        /// <summary>
        /// Returns a game from the Games table by id. Returns null if not found
        /// </summary>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public static Game GetGame(int gameID)
        {
            Game currentGame = new Game();

            var db = new NineTapDb();
            var temp = (

                from g in db.Games
                where g.Id == gameID
                select new
                {
                    g.Bonus,
                    g.Game1,
                    g.Game2,
                    g.Game3,
                    g.Game4,
                    g.Handicap,
                    g.InputtedAvg,
                    g.Id,
                    g.MoneyWon,
                    g.SidePot,
                    g.Notes,
                    g.PlaceStanding,
                    g.UseGame1,
                    g.UseGame2,
                    g.UseGame3,
                    g.UseGame4

                });
            foreach (var g in temp)
            {
                currentGame.Bonus = g.Bonus;
                currentGame.Game1 = g.Game1;
                currentGame.Game2 = g.Game2;
                currentGame.Game3 = g.Game3;
                currentGame.Game4 = g.Game4;
                currentGame.Handicap = g.Handicap;
                currentGame.InputtedAvg = g.InputtedAvg;
                currentGame.Id = g.Id;
                currentGame.MoneyWon = g.MoneyWon;
                currentGame.SidePot = g.SidePot;
                currentGame.Notes = g.Notes;
                currentGame.PlaceStanding = g.PlaceStanding;
                currentGame.UseGame1 = g.UseGame1;
                currentGame.UseGame2 = g.UseGame2;
                currentGame.UseGame3 = g.UseGame3;
                currentGame.UseGame4 = g.UseGame4;

            }
            return currentGame;
        }


        /// <summary>
        /// Adds Games that don't have Ids in the database. Updates Games that have ids 
        /// that exist in the database
        /// </summary>
        /// <param name="games">The Games to add or update</param>
        public static void AddOrUpdateSomeGames(List<Game> games)
        {
            var db = new NineTapDb();
            foreach (var currGame in games)
            {
                db.Entry(currGame).State = db.Games.Any(g => g.Id == currGame.Id) ?
                        EntityState.Modified :
                        EntityState.Added;
            }
            db.SaveChanges();
        }

    }
}
