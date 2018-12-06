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

        public static List<MemberScores> GetGameMemberScores(NineTapDb db, int selectedTourney)
        {
            return (from g in (db.Participants.Include(b => b.Member)
                                     .Include(b => b.Game)
                                     .Where(b => b.Tournament.Id == selectedTourney))

                    select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) }).Concat(
                (from g in (db.Participants.Include(b => b.Member)
                                    .Include(b => b.Game)
                                    .Where(b => b.Tournament.Id == selectedTourney))
                 select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game2.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) })).Concat(
                (from g in (db.Participants.Include(b => b.Member)
                                    .Include(b => b.Game)
                                    .Where(b => b.Tournament.Id == selectedTourney))
                 select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game3.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) })).Concat(
                (from g in (db.Participants.Include(b => b.Member)
                                    .Include(b => b.Game)
                                    .Where(b => b.Tournament.Id == selectedTourney))
                 select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game4.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) })).ToList();
        }

        /// <summary>
        /// Gets a list of Senior scores for the Senior Report
        /// </summary>
        /// <param name="db"></param>
        /// <param name="selectedTourneyId"></param>
        /// <returns></returns>
        public static List<MemberScores> GetSeniorMemberScores(NineTapDb db, int selectedTourneyId)
        {
            var temp = (from g in db.Participants.Include(b => b.Member)
                                                       .Include(b => b.Game)
                                                       .Where(b => b.Tournament.Id == selectedTourneyId)
                                                       .Where(b => b.Member.IsSenior)

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


        public static List<MemberScores> GetStandingsForThreeOutOf4ByHandicap(NineTapDb db, int selectedTourney)
        {
            return (from g in (db.Participants.Include(b => b.Member)
                                     .Include(b => b.Game)
                                     .Where(b => b.Tournament.Id == selectedTourney))
                    orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3 + g.Game.Bonus * 3) - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min())) descending
                    select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3) + (g.Game.Bonus * 3) - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
        }

        public static List<MemberScores> GetStandingsForThreeOf4ByScratch(NineTapDb db, int selectedTourney)
        {
            return (from g in (db.Participants.Include(b => b.Member)
                                     .Include(b => b.Game)
                                     .Where(b => b.Tournament.Id == selectedTourney))
                    orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min())) descending
                    select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
        }

        public static List<MemberScores> GetStandingsForThreeOf4BySquadNumberByHandicap(NineTapDb db, int qualifyBySquadNumber, int selectedTourney)
        {
            return (from g in (db.Participants.Include(b => b.Member)
                                                       .Include(b => b.Game)
                                                       .Where(b => b.Tournament.Id == selectedTourney).Where(b => b.Squad == qualifyBySquadNumber))
                    orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3 + g.Game.Bonus * 3) - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min())) descending
                    select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3) + (g.Game.Bonus * 3) - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
        }

        public static List<MemberScores> GetStandingsThreeOfFourBySquadScratch(NineTapDb db, int qualifyBySquadNumber, int selectedTourney)
        {
            return (from g in (db.Participants.Include(b => b.Member)
                           .Include(b => b.Game)
                           .Where(b => b.Tournament.Id == selectedTourney).Where(b => b.Squad == qualifyBySquadNumber))
                    orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min())) descending
                    select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) }).ToList();

        }

        public static List<MemberScores> GetStandingsForTournamentByHandicap(NineTapDb db, int selectedTourney)
        {
            return (from g in (db.Participants.Include(b => b.Member)
                                                                 .Include(b => b.Game)
                                                                 .Where(b => b.Tournament.Id == selectedTourney))
                    orderby ((g.Game.Game1 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game2 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game3 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game4 + g.Game.Bonus + g.Game.Handicap)) descending
                    select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = (g.Game.Game1 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game2 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game3 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game4 + g.Game.Bonus + g.Game.Handicap), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
        }

        public static List<MemberScores> GetStandingsForTournamentBySquadByHandicap(NineTapDb db, int qualifyBySquadNumber, int selectedTourney)
        {
            return (from g in (db.Participants.Include(b => b.Member)
                                     .Include(b => b.Game)
                                     .Where(b => b.Tournament.Id == selectedTourney).Where(b => b.Squad == qualifyBySquadNumber))
                    orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 4 + g.Game.Bonus * 4)) descending
                    select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 4) + (g.Game.Bonus * 4), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
        }

        public static List<MemberScores> GetStandingsForTournamentBySquadScratch(NineTapDb db, int qualifyBySquadNumber, int selectedTourney)
        {
            return (from g in (db.Participants.Include(b => b.Member)
                                     .Include(b => b.Game)
                                     .Where(b => b.Tournament.Id == selectedTourney).Where(b => b.Squad == qualifyBySquadNumber))
                    orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4) descending
                    select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
        }

        public static List<MemberScores> GetStandingsForTournamentByScratch(NineTapDb db, int selectedTourney)
        {
            return (from g in (db.Participants.Include(b => b.Member)
                                     .Include(b => b.Game)
                                     .Where(b => b.Tournament.Id == selectedTourney))
                    orderby ((g.Game.Game1) + (g.Game.Game2) + (g.Game.Game3) + (g.Game.Game4)) descending
                    select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = (g.Game.Game1) + (g.Game.Game2) + (g.Game.Game3) + (g.Game.Game4), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
        }

        //Note:When printing, these methods get the desired squads in squadList instead of the qualifyBySquad radio btns

        /// <summary>
        /// These GetStandings calls will return multiple squads by getting all the members of a squad then appending that list to the returned value.
        /// Once all the squads in squadList have been appended to the returnedList it is returned.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="squadList">A list of all the selected squads</param>
        /// <param name="selectedTourney"></param>
        /// <returns></returns>
        public static List<MemberScores> GetStandingsForThreeOutOf4ByFilterSeriesByHandicap(NineTapDb db, List<int> squadList, int selectedTourney)
        {
            List<MemberScores> returnedList = new List<MemberScores>();
            foreach (int squad in squadList)
            {
                returnedList.AddRange((from g in (db.Participants.Include(b => b.Member)
                                                       .Include(b => b.Game)
                                                       .Where(b => b.Tournament.Id == selectedTourney).Where(b => b.Squad == squad))
                                       orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3 + g.Game.Bonus * 3) - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min())) descending
                                       select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3) + (g.Game.Bonus * 3) - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) }).ToList());
            }
            return returnedList;
        }

        public static List<MemberScores> GetStandingsForTournamentByFilterSeriesByHandicap(NineTapDb db, List<int> squadList, int selectedTourney)
        {
            List<MemberScores> returnedList = new List<MemberScores>();
            foreach (int squad in squadList)
            {
                returnedList.AddRange((from g in (db.Participants.Include(b => b.Member)
                                     .Include(b => b.Game)
                                     .Where(b => b.Tournament.Id == selectedTourney).Where(b => b.Squad == squad))
                                 orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 4 + g.Game.Bonus * 4)) descending
                                 select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 4) + (g.Game.Bonus * 4), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) }).ToList());

            }
            return returnedList;
        }

        public static List<MemberScores> GetStandingsForThreeOf4ByFilterSeriesByScratch(NineTapDb db, List<int> squadList, int selectedTourney)
        {
            List<MemberScores> returnedList = new List<MemberScores>();
            foreach (int squad in squadList)
            {
                returnedList.AddRange((from g in (db.Participants.Include(b => b.Member)
                           .Include(b => b.Game)
                           .Where(b => b.Tournament.Id == selectedTourney).Where(b => b.Squad == squad))
                                       orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min())) descending
                                       select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) }).ToList());
            }
            return returnedList;
        }

        public static List<MemberScores> GetStandingsForTournamentByFilterSeriesByScratch(NineTapDb db, List<int> squadList, int selectedTourney)
        {
            List<MemberScores> returnedList = new List<MemberScores>();
            foreach (int squad in squadList)
            {
                returnedList.AddRange((from g in (db.Participants.Include(b => b.Member)
                                     .Include(b => b.Game)
                                     .Where(b => b.Tournament.Id == selectedTourney).Where(b => b.Squad == squad))
                                       orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4) descending
                                       select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= DbFunctions.AddYears(DateTime.Now, -1)))) }).ToList());
            }
            return returnedList;
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