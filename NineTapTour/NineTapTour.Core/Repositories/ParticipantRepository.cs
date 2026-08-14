#nullable disable
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Calculations;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Core.Repositories;

public class ParticipantRepository : IParticipantRepository
{
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public ParticipantRepository(IDbContextFactory<NineTapDb> dbFactory)
    {
        this.dbFactory = dbFactory;
    }

    /// <summary>
    /// Ensures a participant row (and empty game row) exists for the member in a tournament squad.
    /// Returns true when the participant already existed or was created successfully.
    /// </summary>
    public bool EnsureParticipantExists(int tournamentId, int memberId, int squad)
    {
        using var db = dbFactory.CreateDbContext();

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
    /// Returns the count of participants who have not yet had any scores entered (Game1 is null),
    /// both as a total and broken down by squad.
    /// </summary>
    public (int Total, Dictionary<int, int> BySquad) GetParticipantNoScoreCounts(int tournamentId)
    {
        using var db = dbFactory.CreateDbContext();
        var rows = db.Participants
            .Include(p => p.Game)
            .Where(p => p.Tournament.Id == tournamentId && p.Game.Game1 == null)
            .Select(p => p.Squad)
            .ToList();

        var bySquad = rows
            .GroupBy(squad => squad)
            .ToDictionary(g => g.Key, g => g.Count());

        return (rows.Count, bySquad);
    }

    /// <summary>
    /// Returns a list of all participants in a tournament
    /// </summary>
    public List<Participant> GetParticipants(int TournamentID)
    {
        using (var db = dbFactory.CreateDbContext())
        {
           return [.. db.Participants.Include("Member").Include("Game").Include("Tournament")
                .Where(p => p.Tournament.Id == TournamentID)
                .OrderBy(p => p.Member.Id)];
        }
    }

    /// <summary>
    /// Returns a list of MemberScores with the same given TournamentID
    /// </summary>
    public List<MemberScores> GetGameMemberScores(int TournamentID)
    {
        using (var db = dbFactory.CreateDbContext())
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
    public List<MemberScores> GetSeniorMemberScores(int selectedTourneyId)
    {
        using (var db = dbFactory.CreateDbContext())
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
    /// Returns 3-of-4 scratch standings: the sum of a participant's games with the
    /// lowest game dropped (only when all four games are present), ordered by score
    /// descending. EF-based implementation (2026-08-14) replacing the former raw SQL,
    /// which crashed on lifetime members and null games, inverted the Paid flag, and
    /// returned Members.Id instead of Member.Number.
    /// </summary>
    public List<MemberScores> GetStandingsForThreeOf4ByScratch(int selectedTournament)
    {
        using (var db = dbFactory.CreateDbContext())
        {
            List<MemberScoresInterim> interimScores = [.. (from g in (db.Participants.Include(b => b.Member)
                        .Include(b => b.Game)
                        .Where(b => b.Tournament.Id == selectedTournament))
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
                            (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1)))),
                        Squad = g.Squad
                    })];

            List<MemberScores> memberScores = [];

            // Sum the games in memory so a null game contributes nothing instead of
            // nulling out the player's total.
            foreach (var memberInterim in interimScores)
            {
                int?[] scores = [memberInterim.Game1Score, memberInterim.Game2Score, memberInterim.Game3Score, memberInterim.Game4Score];
                int totalUsableGames = 0;
                foreach (int? score in scores)
                {
                    if (score != null)
                    {
                        memberInterim.Score += score;
                        totalUsableGames++;
                    }
                }

                if (totalUsableGames == 4)
                {
                    // Find lowest score in scores
                    int lowestScore = scores.Min(s => s.Value);
                    memberInterim.Score -= lowestScore;
                }

                memberScores.Add(memberInterim);
            }

            return [.. memberScores.OrderByDescending(m => m.Score)];
        }
    }

    /// <summary>
    ///
    /// </summary>
    public List<MemberScores> GetStandingsForTournamentByHandicap(int selectedTournament, bool isThreeOfFourTournament = false)
    {
        using (var db = dbFactory.CreateDbContext())
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
                            (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1)))),
                        Squad = g.Squad
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
    /// Returns scratch standings (games only, no handicap or bonus). Fixed
    /// 2026-08-14: the interim projection now populates the per-game scores (they
    /// were previously left null, so every row's Score summed to 0) and the scratch
    /// summation no longer adds the null HandicapValue/BonusPinValue.
    /// </summary>
    public List<MemberScores> GetStandingsForTournamentByScratch(int selectedTournament, bool isThreeOfFourTournament = false)
    {
        using (var db = dbFactory.CreateDbContext())
        {
            List<MemberScoresInterim> interimScores = [.. (from g in (db.Participants.Include(b => b.Member)
                        .Include(b => b.Game)
                        .Where(b => b.Tournament.Id == selectedTournament))
                    orderby ((g.Game.Game1) + (g.Game.Game2) + (g.Game.Game3) + (g.Game.Game4)) descending
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
                            (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1)))),
                        Squad = g.Squad
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
                        memberInterim.Score += score;
                        totalUsableGames++;
                    }
                }

                if (isThreeOfFourTournament && totalUsableGames == 4)
                {
                    // Find lowest score in scores
                    int lowestScore = scores.Min(s => s.Value);
                    memberInterim.Score -= lowestScore;
                }

                memberScores.Add(memberInterim);
            }

            return memberScores;
        }
    }

    //Note:When printing, these methods get the desired squads in squadList instead of the qualifyBySquad radio btns

    /// <summary>
    /// Returns 3-of-4 handicap standings for the selected squads: per played game,
    /// score plus handicap plus bonus; the lowest game (plus one handicap+bonus) is
    /// dropped only when all four games are present. Filters to the squad list and
    /// orders globally by score descending.
    /// </summary>
    /// <param name="squadList">A list of all the selected squads</param>
    /// <param name="selectedTournament"></param>
    /// <returns></returns>
    public List<MemberScores> GetStandingsForThreeOutOf4ByFilterSeriesByHandicap(List<int> squadList, int selectedTournament)
    {
        // EF-based implementation (2026-08-14) replacing the former raw SQL, which
        // crashed on lifetime members and null games, inverted the Paid flag,
        // returned Members.Id instead of Member.Number, and interpolated the squad
        // list into the SQL text. The old SQL filtered all squads in one IN clause
        // with a single global ordering, so this filters and orders the same way.
        using (var db = dbFactory.CreateDbContext())
        {
            List<MemberScoresInterim> interimScores = [.. (from g in (db.Participants.Include(b => b.Member)
                        .Include(b => b.Game)
                        .Where(b => b.Tournament.Id == selectedTournament)
                        .Where(b => squadList.Contains(b.Squad)))
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
                            (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1)))),
                        Squad = g.Squad
                    })];

            List<MemberScores> memberScores = [];

            // Sum handicap+bonus per played game in memory so a null game contributes
            // nothing instead of nulling out the player's total. When all four games
            // are present, dropping the lowest game plus one handicap+bonus is
            // numerically identical to the old SQL's sum + H*3 + B*3 - lowest.
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

                if (totalUsableGames == 4)
                {
                    // Find lowest score in scores
                    int lowestScore = scores.Min(s => s.Value);
                    memberInterim.Score -= lowestScore + memberInterim.HandicapValue + memberInterim.BonusPinValue;
                }

                memberScores.Add(memberInterim);
            }

            return [.. memberScores.OrderByDescending(m => m.Score)];
        }
    }

    /// <summary>
    ///
    /// </summary>
    public List<MemberScores> GetStandingsForTournamentByFilterSeriesByHandicap(List<int> squadList, int selectedTournament, bool isThreeOfFourTournament = false)
    {
        using (var db = dbFactory.CreateDbContext())
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
                            (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1)))),
                         Squad = squad
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
    /// Returns 3-of-4 scratch standings for the selected squads: the sum of a
    /// participant's games with the lowest game dropped (only when all four games
    /// are present), filtered to the squad list and ordered globally by score
    /// descending. EF-based implementation (2026-08-14) replacing the former raw
    /// SQL, which crashed on lifetime members and null games, inverted the Paid
    /// flag, returned Members.Id instead of Member.Number, and interpolated the
    /// squad list into the SQL text.
    /// </summary>
    public List<MemberScores> GetStandingsForThreeOf4ByFilterSeriesByScratch(List<int> squadList, int selectedTournament)
    {
        using (var db = dbFactory.CreateDbContext())
        {
            List<MemberScoresInterim> interimScores = [.. (from g in (db.Participants.Include(b => b.Member)
                        .Include(b => b.Game)
                        .Where(b => b.Tournament.Id == selectedTournament)
                        .Where(b => squadList.Contains(b.Squad)))
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
                            (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1)))),
                        Squad = g.Squad
                    })];

            List<MemberScores> memberScores = [];

            // Sum the games in memory so a null game contributes nothing instead of
            // nulling out the player's total.
            foreach (var memberInterim in interimScores)
            {
                int?[] scores = [memberInterim.Game1Score, memberInterim.Game2Score, memberInterim.Game3Score, memberInterim.Game4Score];
                int totalUsableGames = 0;
                foreach (int? score in scores)
                {
                    if (score != null)
                    {
                        memberInterim.Score += score;
                        totalUsableGames++;
                    }
                }

                if (totalUsableGames == 4)
                {
                    // Find lowest score in scores
                    int lowestScore = scores.Min(s => s.Value);
                    memberInterim.Score -= lowestScore;
                }

                memberScores.Add(memberInterim);
            }

            return [.. memberScores.OrderByDescending(m => m.Score)];
        }
    }

    /// <summary>
    /// Returns scratch standings (games only, no handicap or bonus) for the selected
    /// squads, ordered per squad. Fixed 2026-08-14: the scratch summation no longer
    /// adds the null HandicapValue/BonusPinValue, which previously nulled the totals.
    /// </summary>
    public List<MemberScores> GetStandingsForTournamentByFilterSeriesByScratch(List<int> squadList, int selectedTournament, bool isThreeOfFourTournament = false)
    {
        using (var db = dbFactory.CreateDbContext())
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
                         (g.Member.LastPayment.Value <= DateTime.Now.AddYears(-1)))),
                         Squad = squad
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
                        memberInterim.Score += score;
                        totalUsableGames++;
                    }
                }

                if (isThreeOfFourTournament && totalUsableGames == 4)
                {
                    // Find lowest score in scores
                    int lowestScore = scores.Min(s => s.Value);
                    memberInterim.Score -= lowestScore;
                }

                memberScores.Add(memberInterim);
            }

            return memberScores;
        }
    }

    public int GetParticipantID(NineTapDb db, int memberId, int tournyId, int squad)
    {
        return (from p in db.Participants
                where p.Member.Id == memberId
                    && p.Tournament.Id == tournyId
                    && p.Squad == squad
                select p.Id).FirstOrDefault();
    }
}
