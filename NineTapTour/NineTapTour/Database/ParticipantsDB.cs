using NineTapTour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Database
{
    public class ParticipantsDB
    {
        public static List<Participant> GetParticipants(int tournID)
        {
            using (var db = new NineTapDb())
            {
               return db.Participants.Include("Member").Include("Game").Include("Tournament")
                    .Where(p => p.Tournament.Id == tournID).OrderBy(p => p.Member.Id).ToList();
            }
        }

    }
}

//@"SELECT Participants.Member_Id, Members.FirstName, Members.LastName, Game1, Game2, Game3, Game4, Games.Id,  Members.Handicap, Members.Bonus, SUM(Game1 + Game2 + Game3 + Game4) AS Total
//                            FROM Tournaments JOIN Participants ON Tournaments.Id = Participants.Tournament_Id
//                            JOIN Games ON Games.Id = Participants.Game_Id
//                            JOIN Members ON Members.Id = Participants.Member_Id 
//                            WHERE Tournaments.Id = @TID
//                            GROUP BY Game1, Game2, Game3, Game4, Participants.Member_Id, Tournaments.Location, Participants.SquadNumber, Members.FirstName, Members.LastName, Members.Handicap, Members.Bonus,  Games.Id
//                            ORDER BY Participants.Member_Id";