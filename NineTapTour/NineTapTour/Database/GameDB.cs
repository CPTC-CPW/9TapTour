using NineTapTour.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Database
{
    public static class GameDB
    {
        public static void AddGame(Game temp)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(temp).State = db.Participants.Any(GAME => GAME.Id == temp.Id) ?
                        EntityState.Modified :
                        EntityState.Added;
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                //throw new GameTableException("Error Number : " + " - " + ex.Message);
            }
        }

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
    }
}
