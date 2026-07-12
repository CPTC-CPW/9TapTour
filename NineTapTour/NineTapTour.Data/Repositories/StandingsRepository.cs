using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Abstractions;
using NineTapTour.Calculations;
using NineTapTour.Database;   // NineTapDb
using NineTapTour.Models;     // MemberScores, MemberScoresInterim

namespace NineTapTour.Data.Repositories
{
    /// <summary>EF Core implementation of <see cref="IStandingsRepository"/> (part of former <c>ParticipantsDB</c>).</summary>
    public sealed class StandingsRepository : IStandingsRepository
    {
        private readonly IDbContextFactory<NineTapDb> _factory;

        public StandingsRepository(IDbContextFactory<NineTapDb> factory) => _factory = factory;

        /// <summary>
        /// Returns a list of MemberScores with the same given TournamentID
        /// </summary>
        public List<MemberScores> GetGameMemberScores(int TournamentID)
        {
            using var db = _factory.CreateDbContext();

            // Pull each participant's four game scores in a single query, then expand to one
            // MemberScores per game score client-side (this is a per-game "high game" report).
            var rows = db.Participants
                .AsNoTracking()
                .Where(b => b.Tournament.Id == TournamentID)
                .Select(b => new ParticipantScoreRow
                {
                    Number = b.Member.Number,
                    FirstName = b.Member.FirstName,
                    LastName = b.Member.LastName,
                    IsLifetimeMember = b.Member.IsLifetimeMember,
                    LastPayment = b.Member.LastPayment,
                    Game1 = b.Game.Game1,
                    Game2 = b.Game.Game2,
                    Game3 = b.Game.Game3,
                    Game4 = b.Game.Game4
                })
                .ToList();

            return ExpandToPerGameScores(rows);
        }

        /// <summary>
        /// Gets a list of Senior scores for the Senior Report
        /// </summary>
        public List<MemberScores> GetSeniorMemberScores(int selectedTourneyId)
        {
            using var db = _factory.CreateDbContext();

            var rows = db.Participants
                .AsNoTracking()
                .Where(b => b.Tournament.Id == selectedTourneyId && b.Member.IsSenior)
                .Select(b => new ParticipantScoreRow
                {
                    Number = b.Member.Number,
                    FirstName = b.Member.FirstName,
                    LastName = b.Member.LastName,
                    IsLifetimeMember = b.Member.IsLifetimeMember,
                    LastPayment = b.Member.LastPayment,
                    Game1 = b.Game.Game1,
                    Game2 = b.Game.Game2,
                    Game3 = b.Game.Game3,
                    Game4 = b.Game.Game4
                })
                .ToList();

            List<MemberScores> temp = ExpandToPerGameScores(rows);
            temp.Sort(new MemberScoresComparer());
            return temp;
        }

        /// <summary>
        ///
        /// </summary>
        public List<MemberScores> GetStandingsForTournamentByHandicap(int selectedTournament, bool isThreeOfFourTournament = false)
        {
            using var db = _factory.CreateDbContext();

            List<MemberScoresInterim> memberInterimScores = [.. (from g in (db.Participants.AsNoTracking().Include(b => b.Member)
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
                memberInterim.Score = TournamentCalculations.ComputeSeriesScore(new int?[] { memberInterim.Game1Score, memberInterim.Game2Score, memberInterim.Game3Score, memberInterim.Game4Score }, memberInterim.HandicapValue ?? 0, memberInterim.BonusPinValue ?? 0, includeHandicap: true, isThreeOfFourTournament: isThreeOfFourTournament);
                memberScores.Add(memberInterim);
            }

            return memberScores;
        }

        /// <summary>
        ///
        /// </summary>
        public List<MemberScores> GetStandingsForTournamentByScratch(int selectedTournament, bool isThreeOfFourTournament = false)
        {
            using var db = _factory.CreateDbContext();

            List<MemberScoresInterim> interimScores = [.. (from g in (db.Participants.AsNoTracking().Include(b => b.Member)
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
            // and causing the players score to be null. Scratch totals do NOT include handicap or bonus pins.
            foreach (var memberInterim in interimScores)
            {
                memberInterim.Score = TournamentCalculations.ComputeSeriesScore(new int?[] { memberInterim.Game1Score, memberInterim.Game2Score, memberInterim.Game3Score, memberInterim.Game4Score }, memberInterim.HandicapValue ?? 0, memberInterim.BonusPinValue ?? 0, includeHandicap: false, isThreeOfFourTournament: isThreeOfFourTournament);
                memberScores.Add(memberInterim);
            }

            return memberScores;
        }

        /// <summary>
        /// These GetStandings calls will return multiple squads by getting all the members of a squad then appending that list to the returned value.
        /// Once all the squads in squadList have been appended to the returnedList it is returned.
        /// </summary>
        /// <param name="squadList">A list of all the selected squads</param>
        /// <param name="selectedTournament"></param>
        /// <returns></returns>
        public List<MemberScores> GetStandingsForTournamentByFilterSeriesByHandicap(List<int> squadList, int selectedTournament, bool isThreeOfFourTournament = false)
        {
            using var db = _factory.CreateDbContext();

            List<MemberScoresInterim> returnedList = [];
            foreach (int squad in squadList)
            {
                returnedList.AddRange(
                    (from g in (db.Participants.AsNoTracking().Include(b => b.Member)
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
                memberInterim.Score = TournamentCalculations.ComputeSeriesScore(new int?[] { memberInterim.Game1Score, memberInterim.Game2Score, memberInterim.Game3Score, memberInterim.Game4Score }, memberInterim.HandicapValue ?? 0, memberInterim.BonusPinValue ?? 0, includeHandicap: true, isThreeOfFourTournament: isThreeOfFourTournament);
                memberScores.Add(memberInterim);
            }

            return memberScores;
        }

        /// <summary>
        ///
        /// </summary>
        public List<MemberScores> GetStandingsForTournamentByFilterSeriesByScratch(List<int> squadList, int selectedTournament, bool isThreeOfFourTournament = false)
        {
            using var db = _factory.CreateDbContext();

            List<MemberScoresInterim> returnedList = [];
            foreach (int squad in squadList)
            {
                returnedList.AddRange(
                    (from g in (db.Participants.AsNoTracking().Include(b => b.Member)
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
            // and causing the players score to be null. Scratch totals do NOT include handicap or bonus pins.
            foreach (var memberInterim in returnedList)
            {
                memberInterim.Score = TournamentCalculations.ComputeSeriesScore(new int?[] { memberInterim.Game1Score, memberInterim.Game2Score, memberInterim.Game3Score, memberInterim.Game4Score }, memberInterim.HandicapValue ?? 0, memberInterim.BonusPinValue ?? 0, includeHandicap: false, isThreeOfFourTournament: isThreeOfFourTournament);
                memberScores.Add(memberInterim);
            }

            return memberScores;
        }

        /// <summary>
        /// Expands each participant row into one <see cref="MemberScores"/> per game score.
        /// </summary>
        private static List<MemberScores> ExpandToPerGameScores(List<ParticipantScoreRow> rows)
        {
            var result = new List<MemberScores>(rows.Count * 4);
            foreach (ParticipantScoreRow row in rows)
            {
                string lastPaymentYear = row.IsLifetimeMember
                    ? "life "
                    : (row.LastPayment.HasValue ? row.LastPayment.Value.Year.ToString() : null);
                bool paid = row.IsLifetimeMember
                    || !(row.LastPayment != null && row.LastPayment.Value <= DateTime.Now.AddYears(-1));

                foreach (int? gameScore in new[] { row.Game1, row.Game2, row.Game3, row.Game4 })
                {
                    result.Add(new MemberScores
                    {
                        MemberId = row.Number,
                        FirstName = row.FirstName,
                        LastName = row.LastName,
                        Score = gameScore,
                        LastPaymentYear = lastPaymentYear,
                        Paid = paid
                    });
                }
            }
            return result;
        }

        private sealed class ParticipantScoreRow
        {
            public int Number { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public bool IsLifetimeMember { get; set; }
            public DateTime? LastPayment { get; set; }
            public int? Game1 { get; set; }
            public int? Game2 { get; set; }
            public int? Game3 { get; set; }
            public int? Game4 { get; set; }
        }
    }
}
