using Microsoft.EntityFrameworkCore;
using NineTapTour.Calculations;
using NineTapTour.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Database;

public class ParticipantsDB
{
    /// <summary>
    /// Ensures a participant row (and empty game row) exists for the member in a tournament squad.
    /// Returns true when the participant already existed or was created successfully.
    /// </summary>
    public static bool EnsureParticipantExists(int tournamentId, int memberId, int squad)
    {
        using var db = new NineTapDb();

        bool exists = db.Participants.Any(p =>
            p.Tournament.Id == tournamentId &&
            p.Member.Id == memberId &&
            p.Squad == squad);
        if (exists)
            return true;

        var tournament = db.Tournaments.Find(tournamentId);
        var member = db.Members.Find(memberId);
        if (tournament == null || member == null)
            return false;

        var game = new Game
        {
            Bonus = member.Bonus,
            Handicap = member.Handicap,
            IsComp = false,
            MoneyWon = 0
        };

        db.Games.Add(game);

        var participant = new Participant
        {
            Tournament = tournament,
            Member = member,
            Squad = squad,
            Game = game
        };

        db.Participants.Add(participant);
        db.SaveChanges();
        return true;
    }

    /// <summary>
    /// Returns a list of all participants in a tournament
    /// </summary>
    public static List<Participant> GetParticipants(int TournamentID)
    {
        using (var db = new NineTapDb())
        {
           return [.. db.Participants.Include("Member").Include("Game").Include("Tournament")
                .Where(p => p.Tournament.Id == TournamentID)
                .OrderBy(p => p.Member.Id)];
        }
    }

    /// <summary>
    /// Returns a list of MemberScores with the same given TournamentID
    /// </summary>
    public static List<MemberScores> GetGameMemberScores(int TournamentID)
    {
        using (NineTapDb db = new())
        {
            return
            [
                .. (from g in (db.Participants.Include(b => b.Member)
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
                                        }),
                .. (from g in (db.Participants.Include(b => b.Member)
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
                }),
                .. (from g in (db.Participants.Include(b => b.Member)
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
                }),
                .. (from g in (db.Participants.Include(b => b.Member)
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
                }),
            ];
        }
    }

    /// <summary>
    /// Gets a list of Senior scores for the Senior Report
    /// </summary>
    public static List<MemberScores> GetSeniorMemberScores(int selectedTourneyId)
    {
        using (NineTapDb db = new())
        {
            List<MemberScores> temp = 
                [
                    .. (from g in db.Participants.Include(b => b.Member)
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
                    }),
                    .. (from g in (db.Participants.Include(nameof(Participant.Member))
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
                    }),
                    .. (from g in (db.Participants.Include(nameof(Participant.Member))
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
                    }),
                    .. (from g in (db.Participants.Include(nameof(Participant.Member))
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
                    }),
                ];
            temp.Sort(new MemberScoresComparer());
            return temp;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static List<MemberScores> GetStandingsForThreeOf4ByScratch(int selectedTournament)
    {
        using (NineTapDb db = new())
        {
            string con = db.Database.GetDbConnection().ConnectionString;
            string query = @"SELECT MemberId
    , FirstName
    , LastName
    , (Game1 + Game2 + Game3 + Game4)
    - (
        CASE 
            WHEN Game1 < Game2 AND Game1 < Game3 AND Game1 < Game4 THEN Game1
            WHEN Game2 < Game1 AND Game2 < Game3 AND Game2 < Game4 THEN Game2
            WHEN Game3 < Game1 AND Game3 < Game2 AND Game3 < Game4 THEN Game3
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
            using SqlCommand queryCmd = new(query, new SqlConnection(con));
            queryCmd.Parameters.AddWithValue("@tourneyYear", DateTime.Today.Year - 1);
            queryCmd.Parameters.AddWithValue("@tourneyId", selectedTournament);
            queryCmd.Connection.Open();
            SqlDataReader rdr = queryCmd.ExecuteReader();

            List<MemberScores> memberScores = [];
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
    public static List<MemberScores> GetStandingsForTournamentByHandicap(int selectedTournament, bool isThreeOfFourTournament = false)
    {
        using (NineTapDb db = new())
        {
            List<MemberScoresInterim> memberInterimScores = [.. (from g in (db.Participants.Include(b => b.Member)
                        .Include(b => b.Game)
                        .Where(b => b.Tournament.Id == selectedTournament))
                    orderby ((g.Game.Game1 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game2 + g.Game.Bonus + g.Game.Handicap) + 
                        (g.Game.Game3 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game4 + g.Game.Bonus + g.Game.Handicap)) descending
                    select new MemberScoresInterim {
                        MemberId = g.Member.Number,
                        FirstName = g.Member.FirstName,
                        LastName = g.Member.LastName,
                        Game1Score = g.Game.Game1,
                        Game2Score = g.Game.Game2,
                        Game3Score = g.Game.Game3,
                        Game4Score = g.Game.Game4,
                        HandicapValue = g.Game.Handicap,
                        BonusPinValue = g.Game.Bonus,
                        Score = 0,
                        LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                        Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                            (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                    })];

            List<MemberScores> memberScores = [];
         
            // Use member interim to manually add up the game scores to avoid trying to add null on the database end
            // and causing the players score to be null
            foreach (var memberInterim in memberInterimScores)
            {
                int?[] scores = [memberInterim.Game1Score, memberInterim.Game2Score, memberInterim.Game3Score, memberInterim.Game4Score];
                int totalUsableGames = 0;
                foreach (int? score in scores)
                {
                    if(score != null)
                    {
                        memberInterim.Score += score + memberInterim.HandicapValue + memberInterim.BonusPinValue;
                        totalUsableGames++;
                    }
                }

                if (isThreeOfFourTournament && totalUsableGames == 4)
                {
                    // Find lowest score in scores
                    int lowestScore = scores.Min(s => s.Value);
                    memberInterim.Score -= lowestScore + memberInterim.HandicapValue + memberInterim.BonusPinValue;
                }

                memberScores.Add(memberInterim);
            }

            return memberScores;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static List<MemberScores> GetStandingsForTournamentByScratch(int selectedTournament, bool isThreeOfFourTournament = false)
    {
        using (NineTapDb db = new())
        {
            List<MemberScoresInterim> interimScores = [.. (from g in (db.Participants.Include(b => b.Member)
                        .Include(b => b.Game)
                        .Where(b => b.Tournament.Id == selectedTournament))
                    orderby ((g.Game.Game1) + (g.Game.Game2) + (g.Game.Game3) + (g.Game.Game4)) descending
                    select new MemberScoresInterim {
                        MemberId = g.Member.Number,
                        FirstName = g.Member.FirstName,
                        LastName = g.Member.LastName,
                        Score = 0,
                        LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                        Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                            (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                    })];

            List<MemberScores> memberScores = [];
            // Use member interim to manually add up the game scores to avoid trying to add null on the database end
            // and causing the players score to be null
            foreach (var memberInterim in interimScores)
            {
                int?[] scores = [memberInterim.Game1Score, memberInterim.Game2Score, memberInterim.Game3Score, memberInterim.Game4Score];
                int totalUsableGames = 0;
                foreach (int? score in scores)
                {
                    if (score != null)
                    {
                        memberInterim.Score += score + memberInterim.HandicapValue + memberInterim.BonusPinValue;
                        totalUsableGames++;
                    }
                }

                if (isThreeOfFourTournament && totalUsableGames == 4)
                {
                    // Find lowest score in scores
                    int lowestScore = scores.Min(s => s.Value);
                    memberInterim.Score -= lowestScore + memberInterim.HandicapValue + memberInterim.BonusPinValue;
                }

                memberScores.Add(memberInterim);
            }

            return memberScores;
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
        using (NineTapDb db = new())
        {
            string con = db.Database.GetDbConnection().ConnectionString;
            string query = $@"SELECT MemberId
    , FirstName
    , LastName
    , (Game1 + Game2 + Game3 + Game4 + Games.Handicap * 3 + Games.Bonus * 3)
    - (
        CASE 
            WHEN Game1 < Game2 AND Game1 < Game3 AND Game1 < Game4 THEN Game1
            WHEN Game2 < Game1 AND Game2 < Game3 AND Game2 < Game4 THEN Game2
            WHEN Game3 < Game1 AND Game3 < Game2 AND Game3 < Game4 THEN Game3
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
WHERE TournamentId = @tourneyId AND SquadNumber IN ({string.Join(",", squadList)})
ORDER BY Score DESC";
            using SqlCommand queryCmd = new(query, new SqlConnection(con));
            queryCmd.Parameters.AddWithValue("@tourneyYear", DateTime.Today.Year - 1);
            queryCmd.Parameters.AddWithValue("@tourneyId", selectedTournament);
            queryCmd.Connection.Open();
            SqlDataReader rdr = queryCmd.ExecuteReader();

            List<MemberScores> memberScores = [];
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
    public static List<MemberScores> GetStandingsForTournamentByFilterSeriesByHandicap(List<int> squadList, int selectedTournament, bool isThreeOfFourTournament = false)
    {
        using (NineTapDb db = new())
        {
            List<MemberScoresInterim> returnedList = [];
            foreach (int squad in squadList)
            {
                returnedList.AddRange(
                    (from g in (db.Participants.Include(b => b.Member)
                         .Include(b => b.Game)
                         .Where(b => b.Tournament.Id == selectedTournament).Where(b => b.Squad == squad))
                     orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + 
                         (g.Game.Handicap * 4 + g.Game.Bonus * 4)) descending
                     select new MemberScoresInterim {
                         MemberId = g.Member.Number,
                         FirstName = g.Member.FirstName,
                         LastName = g.Member.LastName,
                         Game1Score = g.Game.Game1,
                         Game2Score = g.Game.Game2,
                         Game3Score = g.Game.Game3,
                         Game4Score = g.Game.Game4,
                         HandicapValue = g.Game.Handicap,
                         BonusPinValue = g.Game.Bonus,
                         Score = 0,
                         LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                         Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                            (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                     }).ToList());

            }

            List<MemberScores> memberScores = [];
            // Use member interim to manually add up the game scores to avoid trying to add null on the database end
            // and causing the players score to be null
            foreach (var memberInterim in returnedList)
            {
                int?[] scores = [memberInterim.Game1Score, memberInterim.Game2Score, memberInterim.Game3Score, memberInterim.Game4Score];
                int totalUsableGames = 0;
                foreach (int? score in scores)
                {
                    if (score != null)
                    {
                        memberInterim.Score += score + memberInterim.HandicapValue + memberInterim.BonusPinValue;
                        totalUsableGames++;
                    }
                }

                if (isThreeOfFourTournament && totalUsableGames == 4)
                {
                    // Find lowest score in scores
                    int lowestScore = scores.Min(s => s.Value);
                    memberInterim.Score -= lowestScore + memberInterim.HandicapValue + memberInterim.BonusPinValue;
                }

                memberScores.Add(memberInterim);
            }

            return memberScores;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static List<MemberScores> GetStandingsForThreeOf4ByFilterSeriesByScratch(List<int> squadList, int selectedTournament)
    {
        using (NineTapDb db = new())
        {
            string con = db.Database.GetDbConnection().ConnectionString;
            string query = $@"SELECT MemberId
    , FirstName
    , LastName
    , (Game1 + Game2 + Game3 + Game4)
    - (
        CASE 
            WHEN Game1 < Game2 AND Game1 < Game3 AND Game1 < Game4 THEN Game1
            WHEN Game2 < Game1 AND Game2 < Game3 AND Game2 < Game4 THEN Game2
            WHEN Game3 < Game1 AND Game3 < Game2 AND Game3 < Game4 THEN Game3
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
WHERE TournamentId = @tourneyId AND SquadNumber IN ({string.Join(",", squadList)})
ORDER BY Score DESC";
            using SqlCommand queryCmd = new(query, new SqlConnection(con));
            queryCmd.Parameters.AddWithValue("@tourneyYear", DateTime.Today.Year - 1);
            queryCmd.Parameters.AddWithValue("@tourneyId", selectedTournament);
            queryCmd.Connection.Open();
            SqlDataReader rdr = queryCmd.ExecuteReader();

            List<MemberScores> memberScores = [];
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
    public static List<MemberScores> GetStandingsForTournamentByFilterSeriesByScratch(List<int> squadList, int selectedTournament, bool isThreeOfFourTournament = false)
    {
        using (NineTapDb db = new())
        {
            List<MemberScoresInterim> returnedList = [];
            foreach (int squad in squadList)
            {
                returnedList.AddRange(
                    (from g in (db.Participants.Include(b => b.Member)
                         .Include(b => b.Game)
                         .Where(b => b.Tournament.Id == selectedTournament).Where(b => b.Squad == squad))
                     orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4) descending
                     select new MemberScoresInterim {
                         MemberId = g.Member.Number,
                         FirstName = g.Member.FirstName,
                         LastName = g.Member.LastName,
                         Game1Score = g.Game.Game1,
                         Game2Score = g.Game.Game2,
                         Game3Score = g.Game.Game3,
                         Game4Score = g.Game.Game4,
                         Score = 0,
                         LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(),
                         Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && 
                         (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1))))
                     }).ToList());
            }

            List<MemberScores> memberScores = [];
            // Use member interim to manually add up the game scores to avoid trying to add null on the database end
            // and causing the players score to be null
            foreach (var memberInterim in returnedList)
            {
                int?[] scores = [memberInterim.Game1Score, memberInterim.Game2Score, memberInterim.Game3Score, memberInterim.Game4Score];
                int totalUsableGames = 0;
                foreach (int? score in scores)
                {
                    if (score != null)
                    {
                        memberInterim.Score += score + memberInterim.HandicapValue + memberInterim.BonusPinValue;
                        totalUsableGames++;
                    }
                }

                if (isThreeOfFourTournament && totalUsableGames == 4)
                {
                    // Find lowest score in scores
                    int lowestScore = scores.Min(s => s.Value);
                    memberInterim.Score -= lowestScore + memberInterim.HandicapValue + memberInterim.BonusPinValue;
                }

                memberScores.Add(memberInterim);
            }

            return memberScores;
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

public class MemberScoresInterim : MemberScores
{
    public int? Game1Score { get; internal set; }
    public int? Game2Score { get; internal set; }
    public int? Game3Score { get; internal set; }
    public int? Game4Score { get; internal set; }
    public int? HandicapValue { get; internal set; }
    public int? BonusPinValue { get; internal set; }
}

