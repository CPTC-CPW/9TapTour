using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Models.ViewModels
{
    public class TournamentHighScoresViewModel
    {
        public int memberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }
        public int GameId { get; set; }
        public int Handicap { get; set; }
        public int Bonus { get; set; }

        //total of all 4 games
        public int? Total()
        {
            List<int?> test  = [Game1, Game2, Game3, Game4];
            int score = 0;
            for (int i = 0; i < test.Count; i++)
            {
                // skips games that were not finished
                if (test[i] != null)
                {
                    score += test[i].Value;
                }
            }
            return score;
        }


        //@"SELECT Participants.Member_Id, Members.FirstName, Members.LastName, Game1, Game2, Game3, Game4, Games.Id,  Members.Handicap, Members.Bonus, SUM(Game1 + Game2 + Game3 + Game4) AS Total
        //                            FROM Tournaments JOIN Participants ON Tournaments.Id = Participants.Tournament_Id
        //                            JOIN Games ON Games.Id = Participants.Game_Id
        //                            JOIN Members ON Members.Id = Participants.Member_Id 
        //                            WHERE Tournaments.Id = @TID
        //                            GROUP BY Game1, Game2, Game3, Game4, Participants.Member_Id, Tournaments.Location, Participants.SquadNumber, Members.FirstName, Members.LastName, Members.Handicap, Members.Bonus,  Games.Id
        //                            ORDER BY Participants.Member_Id";
    }
}
