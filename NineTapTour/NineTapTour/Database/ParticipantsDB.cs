using NineTapTour.Calculations;
using NineTapTour.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        /// <summary>
        /// Gets a list of Senior scores for the Senior Report
        /// </summary>
        /// <param name="db"></param>
        /// <param name="selectedTourneyId"></param>
        /// <returns></returns>
        public static List<MemberScores> GetSeniorMemberScores(NineTapDb db, int selectedTourneyId)
        {
            var temp = (from g in (db.Participants.Include(nameof(Participant.Member))
                                                       .Include(nameof(Participant.Game))
                                                       .Where(b => b.Tournament.Id == selectedTourneyId)
                                                       .Where(b => b.Member.IsSenior))

                        select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) }).Concat(
                                   (from g in (db.Participants.Include(nameof(Participant.Member))
                                                       .Include(nameof(Participant.Game))
                                                       .Where(b => b.Tournament.Id == selectedTourneyId)
                                                       .Where(b => b.Member.IsSenior))
                                    select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game2.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) })).Concat(
                                   (from g in (db.Participants.Include(nameof(Participant.Member))
                                                       .Include(nameof(Participant.Game))
                                                       .Where(b => b.Tournament.Id == selectedTourneyId)
                                                       .Where(b => b.Member.IsSenior))
                                    select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game3.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) })).Concat(
                                   (from g in (db.Participants.Include(nameof(Participant.Member))
                                                       .Include(nameof(Participant.Game))
                                                       .Where(b => b.Tournament.Id == selectedTourneyId)
                                                       .Where(b => b.Member.IsSenior))
                                    select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game4.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) })).ToList();
            temp.Sort(new MemberScoresComparer());
            temp.Reverse();
            return temp;
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