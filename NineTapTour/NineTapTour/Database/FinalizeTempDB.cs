using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Database
{
    class FinalizeTempDB
    {
        public static void AddFinalizeTemp(FinalizeTemp temp)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    //checks if tournament is new or already existing in db
                    if (!db.FinalizeTemp.Any(f => f.GameId == temp.GameId))
                    {
                        db.Entry(temp).State = EntityState.Added;
                        db.SaveChanges();
                    }

                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error Number : " + ex.Number + " - " + ex.Message);
            }
        }

        /// <summary>
        /// This method take all Current data from FinalizeTemp table and uses these values 
        /// to update the Game Table.
        /// </summary>
        /// <param name="TournamentId">Active tournament to finalize</param>
        public static void finalizeGame(int TournamentId)
        {
            using (var db = new NineTapDb())
            {
                var Tournament = (from t in db.FinalizeTemp
                                  where t.TournamentID == TournamentId
                                  select t).ToList();

                foreach (var t in Tournament)
                {
                    var game = db.Games.SingleOrDefault(g => g.Id == t.GameId);
                    if(game != null)
                    {
                        game.UseGame1 = t.UseGame1;
                        game.UseGame2 = t.UseGame2;
                        game.UseGame3 = t.UseGame3;
                        game.UseGame4 = t.UseGame4;
                        game.Notes = t.Notes;
                        game.Handicap = t.Handicap;
                        game.Bonus = t.Bonus;
                        //game.MoneyWon = t.MoneyWon; //Add MoneyWon field in Finalize
                        //game.PlaceStanding = t.PlaceStanding; //Possibly update PlaceStanding
                        db.SaveChanges();
                    }

                }
                
            }
            
        }

        /// <summary>
        /// This method take all Current data from FinalizeTemp table and uses these values 
        /// to update the Game Table.
        /// </summary>
        /// <param name="TournamentId">Active tournament to finalize</param>
        public static void FinalizeGame(int TournamentId)
        {
            using (var db = new NineTapDb())
            {
                var Tournament = (from t in db.FinalizeTemp
                                  where t.TournamentID == TournamentId
                                  select t).ToList();

                foreach (var t in Tournament)
                {
                    var game = db.Games.SingleOrDefault(g => g.Id == t.GameId);
                    if (game != null)
                    {
                        game.UseGame1 = t.UseGame1;
                        game.UseGame2 = t.UseGame2;
                        game.UseGame3 = t.UseGame3;
                        game.UseGame4 = t.UseGame4;
                        game.Notes = t.Notes;
                        game.Handicap = t.Handicap;
                        game.Bonus = t.Bonus;
                        //game.MoneyWon = t.MoneyWon; //Add MoneyWon field in Finalize
                        //game.PlaceStanding = t.PlaceStanding; //Possibly update PlaceStanding
                        db.SaveChanges();
                    }

                }

            }

        }
}
