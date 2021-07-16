using Microsoft.EntityFrameworkCore;
using NineTapTour.Calculations;
using NineTapTour.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Database
{
    public class ParticipantsDB
    {
        /// <summary>
        /// Returns a list of all participants in a tournament
        /// </summary>
        public static List<Participant> GetParticipants(int TournamentID)
        {
            using (var db = new NineTapDb())
            {
               return db.Participants.Include("Member").Include("Game").Include("Tournament")
                    .Where(p => p.Tournament.Id == TournamentID)
                    .OrderBy(p => p.Member.Id)
                    .ToList();
            }
        }

        /// <summary>
        /// Returns a list of MemberScores with the same given TournamentID
        /// </summary>
        public static List<MemberScores> GetGameMemberScores(int TournamentID)
        {
            using (NineTapDb db = new NineTapDb())
            {
                return (from g in (db.Participants.Include(b => b.Member)
                            .Include(b => b.Game)
                            .Where(b => b.Tournament.Id == TournamentID))
                        select new MemberScores {
                            MemberId = g.Member.Number,
                            FirstName = g.Member.FirstName,
                            LastName = g.Member.LastName,
                            Score = g.Game.Game1.Value,
                            LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                            Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                                (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                        }).Concat(
                            (from g in (db.Participants.Include(b => b.Member)
                                .Include(b => b.Game)
                                .Where(b => b.Tournament.Id == TournamentID))

                 select new MemberScores {
                     MemberId = g.Member.Number,
                     FirstName = g.Member.FirstName,
                     LastName = g.Member.LastName,
                     Score = g.Game.Game2.Value,
                     LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                     Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                        (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                 })).Concat(
                     (from g in (db.Participants.Include(b => b.Member)
                          .Include(b => b.Game)
                          .Where(b => b.Tournament.Id == TournamentID))

                 select new MemberScores {
                     MemberId = g.Member.Number,
                     FirstName = g.Member.FirstName,
                     LastName = g.Member.LastName,
                     Score = g.Game.Game3.Value,
                     LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                     Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                        (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                 })).Concat(
                     (from g in (db.Participants.Include(b => b.Member)
                          .Include(b => b.Game)
                          .Where(b => b.Tournament.Id == TournamentID))

                 select new MemberScores {
                     MemberId = g.Member.Number,
                     FirstName = g.Member.FirstName,
                     LastName = g.Member.LastName,
                     Score = g.Game.Game4.Value,
                     LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                     Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                        (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                 })).ToList();
            }
        }

        /// <summary>
        /// Gets a list of Senior scores for the Senior Report
        /// </summary>
        public static List<MemberScores> GetSeniorMemberScores(int selectedTourneyId)
        {
            using (NineTapDb db = new NineTapDb())
            {
                List<MemberScores> temp = 
                    (from g in db.Participants.Include(b => b.Member)
                    .Include(b => b.Game)
                    .Where(b => b.Tournament.Id == selectedTourneyId)
                    .Where(b => b.Member.IsSenior)

                select new MemberScores {
                    MemberId = g.Member.Number,
                    FirstName = g.Member.FirstName,
                    LastName = g.Member.LastName,
                    Score = g.Game.Game1.Value,
                    LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                    Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                        (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                }).Concat(
                    (from g in (db.Participants.Include(nameof(Participant.Member))
                        .Include(nameof(Participant.Game))
                        .Where(b => b.Tournament.Id == selectedTourneyId)
                        .Where(b => b.Member.IsSenior))

                select new MemberScores {
                    MemberId = g.Member.Number,
                    FirstName = g.Member.FirstName,
                    LastName = g.Member.LastName,
                    Score = g.Game.Game2.Value,
                    LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                    Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                        (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                })).Concat(
                    (from g in (db.Participants.Include(nameof(Participant.Member))
                        .Include(nameof(Participant.Game))
                        .Where(b => b.Tournament.Id == selectedTourneyId)
                        .Where(b => b.Member.IsSenior))

                select new MemberScores {
                    MemberId = g.Member.Number,
                    FirstName = g.Member.FirstName,
                    LastName = g.Member.LastName,
                    Score = g.Game.Game3.Value,
                    LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                    Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                        (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                })).Concat(
                    (from g in (db.Participants.Include(nameof(Participant.Member))
                        .Include(nameof(Participant.Game))
                        .Where(b => b.Tournament.Id == selectedTourneyId)
                        .Where(b => b.Member.IsSenior))

                select new MemberScores {
                    MemberId = g.Member.Number,
                    FirstName = g.Member.FirstName,
                    LastName = g.Member.LastName,
                    Score = g.Game.Game4.Value,
                    LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                    Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null &&
                        (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                })).ToList();
                temp.Sort(new MemberScoresComparer());
                return temp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<MemberScores> GetStandingsForThreeOutOf4ByHandicap(int selectedTournament)
        {
            // Remove the lists and find the lowest game score
            // Use Raw SQL?
            using (NineTapDb db = new NineTapDb())
            {
                string con = db.Database.GetDbConnection().ConnectionString;
                string query = @"SELECT MemberId
    , FirstName
    , LastName
    , (Game1 + Game2 + Game3 + Game4 + Games.Handicap * 3 + Games.Bonus * 3)
    - (
        CASE 
            WHEN Game1 < Game2 AND Game1 < Game3 AND Game1 < Game4 THEN Game1
            WHEN Game2 < Game1 AND Game2 < Game3 AND Game2 < Game4 THEN Game2
            WHEN Game3 < Game1 AND Game3 < Game2 AND Game3 < Game4 THEN Game4
            ELSE Game4
        END) AS Score
    , CASE
        WHEN IsLifetimeMember = 1 THEN 'life'
        ELSE YEAR(LastPayment)
    END AS LastPaymentYear
    , CASE
        WHEN IsLifetimeMember = 1 THEN 'true'
        WHEN LastPayment IS NOT NULL AND YEAR(LastPayment) <= @tourneyYear THEN 'true'
        ELSE 'false'
    END AS Paid
FROM Members
    JOIN Participants ON Members.Id = Participants.MemberId
    JOIN Games ON Participants.GameId = Games.Id
WHERE TournamentId = @tourneyId
ORDER BY Score DESC";
                using SqlCommand queryCmd = new SqlCommand(query, new SqlConnection(con));
                queryCmd.Parameters.AddWithValue("@tourneyYear", DateTime.Today.Year - 1);
                queryCmd.Parameters.AddWithValue("@tourneyId", selectedTournament);
                queryCmd.Connection.Open();
                SqlDataReader rdr = queryCmd.ExecuteReader();

                List<MemberScores> memberScores = new();
                while (rdr.Read())
                {
                    memberScores.Add(
                        new MemberScores()
                        {
                            FirstName = rdr["FirstName"].ToString(),
                            LastName = rdr["LastName"].ToString(),
                            MemberId = Convert.ToInt32(rdr["MemberId"]),
                            Score = Convert.ToInt32(rdr["Score"]),
                            LastPaymentYear = rdr["LastPaymentYear"].ToString(),
                            Paid = Convert.ToBoolean(rdr["Paid"])
                        }
                    );
                }
                
                return memberScores;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<MemberScores> GetStandingsForThreeOf4ByScratch(int selectedTournament)
        {
            using (NineTapDb db = new NineTapDb())
            {
                return (from g in (db.Participants.Include(b => b.Member)
                            .Include(b => b.Game)
                            .Where(b => b.Tournament.Id == selectedTournament))
                        orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - 
                            (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }
                            .Min())) descending
                        select new MemberScores {
                            MemberId = g.Member.Number,
                            FirstName = g.Member.FirstName,
                            LastName = g.Member.LastName,
                            Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - 
                                (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()),
                            LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                            Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null &&
                                (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                        }).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<MemberScores> GetStandingsForThreeOf4BySquadNumberByHandicap(int qualifyBySquadNumber, int selectedTournament)
        {
            using (NineTapDb db = new NineTapDb())
            {
                return (from g in (db.Participants.Include(b => b.Member)
                            .Include(b => b.Game)
                            .Where(b => b.Tournament.Id == selectedTournament).Where(b => b.Squad == qualifyBySquadNumber))
                        orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3 + g.Game.Bonus * 3) - 
                            (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }
                            .Min())) descending
                        select new MemberScores {
                            MemberId = g.Member.Number,
                            FirstName = g.Member.FirstName,
                            LastName = g.Member.LastName,
                            Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3) + (g.Game.Bonus * 3) - 
                                (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()),
                            LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                            Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                                (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                        }).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<MemberScores> GetStandingsThreeOfFourBySquadScratch(int qualifyBySquadNumber, int selectedTournament)
        {
            using (NineTapDb db = new NineTapDb())
            {
                return (from g in (db.Participants.Include(b => b.Member)
                           .Include(b => b.Game)
                           .Where(b => b.Tournament.Id == selectedTournament).Where(b => b.Squad == qualifyBySquadNumber))
                        orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - 
                            (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }
                            .Min())) descending
                        select new MemberScores {
                            MemberId = g.Member.Number,
                            FirstName = g.Member.FirstName,
                            LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - 
                                (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()),
                            LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                            Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                                (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                        }).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<MemberScores> GetStandingsForTournamentByHandicap(int selectedTournament)
        {
            using (NineTapDb db = new NineTapDb())
            {
                return (from g in (db.Participants.Include(b => b.Member)
                            .Include(b => b.Game)
                            .Where(b => b.Tournament.Id == selectedTournament))
                        orderby ((g.Game.Game1 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game2 + g.Game.Bonus + g.Game.Handicap) + 
                            (g.Game.Game3 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game4 + g.Game.Bonus + g.Game.Handicap)) descending
                        select new MemberScores {
                            MemberId = g.Member.Number,
                            FirstName = g.Member.FirstName,
                            LastName = g.Member.LastName,
                            Score = (g.Game.Game1 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game2 + g.Game.Bonus + g.Game.Handicap) + 
                                (g.Game.Game3 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game4 + g.Game.Bonus + g.Game.Handicap),
                            LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                            Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                                (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                        }).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<MemberScores> GetStandingsForTournamentBySquadByHandicap(int qualifyBySquadNumber, int selectedTournament)
        {
            using (NineTapDb db = new NineTapDb())
            {
                return (from g in (db.Participants.Include(b => b.Member)
                            .Include(b => b.Game)
                            .Where(b => b.Tournament.Id == selectedTournament).Where(b => b.Squad == qualifyBySquadNumber))
                        orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + 
                            (g.Game.Handicap * 4 + g.Game.Bonus * 4)) descending
                        select new MemberScores {
                            MemberId = g.Member.Number,
                            FirstName = g.Member.FirstName,
                            LastName = g.Member.LastName,
                            Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 4) +  (g.Game.Bonus * 4),
                            LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                            Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                                (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                        }).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<MemberScores> GetStandingsForTournamentBySquadScratch(int qualifyBySquadNumber, int selectedTournament)
        {
            using (NineTapDb db = new NineTapDb())
            {
                return (from g in (db.Participants.Include(b => b.Member)
                            .Include(b => b.Game)
                            .Where(b => b.Tournament.Id == selectedTournament).Where(b => b.Squad == qualifyBySquadNumber))
                        orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4) descending
                        select new MemberScores {
                            MemberId = g.Member.Number,
                            FirstName = g.Member.FirstName,
                            LastName = g.Member.LastName,
                            Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4,
                            LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                            Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                                (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                        }).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<MemberScores> GetStandingsForTournamentByScratch(int selectedTournament)
        {
            using (NineTapDb db = new NineTapDb())
            {
                return (from g in (db.Participants.Include(b => b.Member)
                            .Include(b => b.Game)
                            .Where(b => b.Tournament.Id == selectedTournament))
                        orderby ((g.Game.Game1) + (g.Game.Game2) + (g.Game.Game3) + (g.Game.Game4)) descending
                        select new MemberScores {
                            MemberId = g.Member.Number,
                            FirstName = g.Member.FirstName,
                            LastName = g.Member.LastName,
                            Score = (g.Game.Game1) + (g.Game.Game2) + (g.Game.Game3) + (g.Game.Game4),
                            LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                            Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                                (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                        }).ToList();
            }
        }

        //Note:When printing, these methods get the desired squads in squadList instead of the qualifyBySquad radio btns

        /// <summary>
        /// These GetStandings calls will return multiple squads by getting all the members of a squad then appending that list to the returned value.
        /// Once all the squads in squadList have been appended to the returnedList it is returned.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="squadList">A list of all the selected squads</param>
        /// <param name="selectedTournament"></param>
        /// <returns></returns>
        public static List<MemberScores> GetStandingsForThreeOutOf4ByFilterSeriesByHandicap(List<int> squadList, int selectedTournament)
        {
            using (NineTapDb db = new NineTapDb())
            {
                List<MemberScores> returnedList = new List<MemberScores>();
                foreach (int squad in squadList)
                {
                    returnedList.AddRange(
                        (from g in (db.Participants.Include(b => b.Member)
                             .Include(b => b.Game)
                             .Where(b => b.Tournament.Id == selectedTournament).Where(b => b.Squad == squad))
                         orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3 + g.Game.Bonus * 3) - 
                             (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }
                             .Min())) descending
                         select new MemberScores {
                             MemberId = g.Member.Number,
                             FirstName = g.Member.FirstName,
                             LastName = g.Member.LastName,
                             Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3) + (g.Game.Bonus * 3) - 
                                 (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()),
                             LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                             Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                                 (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                         }).ToList());
                }
                return returnedList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<MemberScores> GetStandingsForTournamentByFilterSeriesByHandicap(List<int> squadList, int selectedTournament)
        {
            using (NineTapDb db = new NineTapDb())
            {
                List<MemberScores> returnedList = new List<MemberScores>();
                foreach (int squad in squadList)
                {
                    returnedList.AddRange(
                        (from g in (db.Participants.Include(b => b.Member)
                             .Include(b => b.Game)
                             .Where(b => b.Tournament.Id == selectedTournament).Where(b => b.Squad == squad))
                         orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + 
                             (g.Game.Handicap * 4 + g.Game.Bonus * 4)) descending
                         select new MemberScores {
                             MemberId = g.Member.Number,
                             FirstName = g.Member.FirstName,
                             LastName = g.Member.LastName,
                             Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 4) + (g.Game.Bonus * 4),
                             LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                             Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                                (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                         }).ToList());

                }
                return returnedList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<MemberScores> GetStandingsForThreeOf4ByFilterSeriesByScratch(List<int> squadList, int selectedTournament)
        {
            using (NineTapDb db = new NineTapDb())
            {
                List<MemberScores> returnedList = new List<MemberScores>();
                foreach (int squad in squadList)
                {
                    returnedList.AddRange(
                        (from g in (db.Participants.Include(b => b.Member)
                             .Include(b => b.Game)
                             .Where(b => b.Tournament.Id == selectedTournament).Where(b => b.Squad == squad))
                         orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - 
                             (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }
                             .Min())) descending
                         select new MemberScores {
                             MemberId = g.Member.Number,
                             FirstName = g.Member.FirstName,
                             LastName = g.Member.LastName,
                             Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - 
                                (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()),
                             LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                             Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                                (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                         }).ToList());
                }
                return returnedList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<MemberScores> GetStandingsForTournamentByFilterSeriesByScratch(List<int> squadList, int selectedTournament)
        {
            using (NineTapDb db = new NineTapDb())
            {
                List<MemberScores> returnedList = new List<MemberScores>();
                foreach (int squad in squadList)
                {
                    returnedList.AddRange(
                        (from g in (db.Participants.Include(b => b.Member)
                             .Include(b => b.Game)
                             .Where(b => b.Tournament.Id == selectedTournament).Where(b => b.Squad == squad))
                         orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4) descending
                         select new MemberScores {
                             MemberId = g.Member.Number,
                             FirstName = g.Member.FirstName,
                             LastName = g.Member.LastName,
                             Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4,
                             LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                             Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                             (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                         }).ToList());
                }
                return returnedList;
            }
        }

        public static int GetParticipantID(NineTapDb db, int memberId, int tournyId, int squad)
        {
            return (from p in db.Participants
                    where p.Member.Id == memberId
                        && p.Tournament.Id == tournyId
                        && p.Squad == squad
                    select p.Id).FirstOrDefault();
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