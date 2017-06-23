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
namespace NineTapTour.Database
{
    public class PlayerHistoryDB
    {
        public static void AddPlayerHistory(PlayerHistory temp)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(temp).State = db.PlayerHistory.Any(his => his.hisID == temp.hisID) ?
                         EntityState.Modified :
                         EntityState.Added;
                    db.SaveChanges();
                }
            }
            catch(SqlException ex)
            {
                throw new PlayerHistoryTableException ("Error Number : " + ex.Number + " - " + ex.Message);
            }
        }

        public static List<PlayerHistory> getTop30FromPlayerHistory(int id)
        {
            List<PlayerHistory> Return = new List<PlayerHistory>();
            using (var db = new NineTapDb())
            {
                var temp = (from h in db.PlayerHistory
                            where h.MemberNumber == id
                            orderby h.TournamentDate descending
                            select new
                            {
                                h.GamesPlayed,
                                h.TournamentDate,
                                h.Game.Game1,
                                h.Game.Game2,
                                h.Game.Game3,
                                h.Game.Game4,
                                h.AverageForGame,
                                h.trueAVG,
                                h.AVG,
                                h.Game.Handicap,
                                h.Game.Bonus,
                                h.ProPot,
                                h.Game.MoneyWon,
                                h.Game.Notes,
                            }).Take(30).ToList();
                foreach(var item in temp)
                {
                    PlayerHistory newHistory = new PlayerHistory();
                    Game gameHistory = new Game();

                    newHistory.GamesPlayed = item.GamesPlayed;
                    newHistory.TournamentDate = item.TournamentDate;
                    gameHistory.Game1 = item.Game1;
                    gameHistory.Game2 = item.Game2;
                    gameHistory.Game3 = item.Game3;
                    gameHistory.Game4 = item.Game4;
                    gameHistory.TotalScore = (item.Game1 + item.Game2 + item.Game3 + item.Game4);
                    newHistory.AverageForGame = item.AverageForGame;
                    newHistory.trueAVG = item.trueAVG;
                    newHistory.AVG = item.AVG;
                    gameHistory.Handicap = item.Handicap;
                    gameHistory.Bonus = item.Bonus;
                    newHistory.ProPot = item.ProPot;
                    gameHistory.MoneyWon = item.MoneyWon;
                    gameHistory.Notes = item.Notes;
                    newHistory.Game = gameHistory;
                    Return.Add(newHistory);
                }
            
                            
            }

            return Return;
        }
    }
}
