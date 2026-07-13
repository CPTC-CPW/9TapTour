using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Abstractions;
using NineTapTour.Calculations;
using NineTapTour.Database;
using NineTapTour.Models;
using NineTapTour.Models.ViewModels;

namespace NineTapTour.Data.Services
{
    /// <summary>
    /// Builds a tournament's winners list. Extracted verbatim from FrmTournamentResults
    /// (BuildWinnersList / BuildWinnersListDoubles / BuildPrevHdcpByMember) so the results
    /// computation is UI-free and testable.
    /// </summary>
    public sealed class TournamentResultsService : ITournamentResultsService
    {
        private readonly ITournamentRepository _tournamentRepo;
        private readonly IDoublesRepository _doublesRepo;
        private readonly IDbContextFactory<NineTapDb> _factory;

        public TournamentResultsService(
            ITournamentRepository tournamentRepo,
            IDoublesRepository doublesRepo,
            IDbContextFactory<NineTapDb> factory)
        {
            _tournamentRepo = tournamentRepo;
            _doublesRepo = doublesRepo;
            _factory = factory;
        }

        public WinnersListResult BuildWinnersList(int tournamentId, bool isDoubles, bool isThreeOfFour)
        {
            List<WinnerListMemberViewModel> bowlers = _tournamentRepo.GetWinnerListMemberData(tournamentId);

            var result = new WinnersListResult { TotalEntries = bowlers.Count };

            if (isDoubles)
            {
                int doublesCompEntries = 0;
                result.Bowlers = BuildWinnersListDoubles(bowlers, tournamentId, ref doublesCompEntries);
                result.CompEntries = doublesCompEntries;
                return result;
            }

            int compEntries = 0;
            var tournyBowlers = new List<ExcelMember>();

            // Batch-query each member's handicap from their most recent finalized prior tournament.
            var memberNumbers = bowlers.Select(b => b.MemberNumber).Distinct().ToHashSet();
            var prevHdcpByMember = BuildPrevHdcpByMember(memberNumbers, tournamentId);

            foreach (var b in bowlers)
            {
                if (b.IsComp)
                {
                    compEntries++;
                }

                ExcelMember m = new()
                {
                    MemberNumber = b.MemberNumber,
                    Name = b.BowlerName,
                    Handicap = prevHdcpByMember.TryGetValue(b.MemberNumber, out int prevHdcp) && prevHdcp > 0
                        ? prevHdcp
                        : System.Convert.ToInt32(b.Handicap),
                    Bonus = b.MemberBonus,
                    MoneyWon = b.MoneyWon,
                    SidePot = b.SidePot,
                    GameId = b.GameId,
                    Game1Score = System.Convert.ToInt32(b.Game1),
                    Game2Score = System.Convert.ToInt32(b.Game2),
                    Game3Score = System.Convert.ToInt32(b.Game3),
                    Game4Score = System.Convert.ToInt32(b.Game4)
                };

                if (isThreeOfFour)
                {
                    List<int> scores = [m.Game1Score, m.Game2Score, m.Game3Score, m.Game4Score];
                    scores.RemoveAll(x => x == 0);

                    if (scores.Count == 4)
                    {
                        int minScore = scores.Min();
                        scores.Remove(minScore);
                        if (m.Game1Score == minScore) m.Game1Score = 0;
                        else if (m.Game2Score == minScore) m.Game2Score = 0;
                        else if (m.Game3Score == minScore) m.Game3Score = 0;
                        else if (m.Game4Score == minScore) m.Game4Score = 0;
                    }

                    m.TotalScore = scores.Sum() + (scores.Count * (m.Handicap + m.Bonus));
                }
                else
                {
                    int totalValidGames = 0;
                    if (m.Game1Score > 0) totalValidGames++;
                    if (m.Game2Score > 0) totalValidGames++;
                    if (m.Game3Score > 0) totalValidGames++;
                    if (m.Game4Score > 0) totalValidGames++;

                    m.TotalScore = m.Game1Score + m.Game2Score + m.Game3Score
                        + m.Game4Score + (totalValidGames * (m.Handicap + m.Bonus));
                }
                tournyBowlers.Add(m);
            }

            result.Bowlers = tournyBowlers;
            result.CompEntries = compEntries;
            return result;
        }

        private List<ExcelMember> BuildWinnersListDoubles(List<WinnerListMemberViewModel> bowlers, int tournamentId, ref int compEntries)
        {
            var memberNumbers = bowlers.Select(b => b.MemberNumber).Distinct().ToHashSet();
            var prevHdcpByMember = BuildPrevHdcpByMember(memberNumbers, tournamentId);

            List<DoublesTeam> teams = _doublesRepo.GetTeamsByTournament(tournamentId);
            var bowlersByMemberId = bowlers.GroupBy(b => b.MemberId).ToDictionary(g => g.Key, g => g.ToList());

            var teamRows = new List<(int CombinedHdcpTotal,
                                     WinnerListMemberViewModel M1, WinnerListMemberViewModel M2,
                                     int H1, int B1, int H2, int B2)>();

            foreach (var team in teams)
            {
                if (!bowlersByMemberId.TryGetValue(team.Member1.Id, out var e1)) continue;
                if (!bowlersByMemberId.TryGetValue(team.Member2.Id, out var e2)) continue;

                var m1 = e1.FirstOrDefault(e => e.Squad == team.Squad);
                var m2 = e2.FirstOrDefault(e => e.Squad == team.Squad);
                if (m1 == null || m2 == null) continue;

                int hdcp1 = prevHdcpByMember.TryGetValue(m1.MemberNumber, out int ph1) && ph1 > 0 ? ph1 : System.Convert.ToInt32(m1.Handicap);
                int hdcp2 = prevHdcpByMember.TryGetValue(m2.MemberNumber, out int ph2) && ph2 > 0 ? ph2 : System.Convert.ToInt32(m2.Handicap);
                int bonus1 = m1.MemberBonus;
                int bonus2 = m2.MemberBonus;

                int combinedHdcpTotal = (m1.Game1 ?? 0) + (m1.Game2 ?? 0)
                                      + (m2.Game1 ?? 0) + (m2.Game2 ?? 0)
                                      + 2 * (hdcp1 + bonus1)
                                      + 2 * (hdcp2 + bonus2);

                teamRows.Add((combinedHdcpTotal, m1, m2, hdcp1, bonus1, hdcp2, bonus2));
            }

            teamRows.Sort((a, b) => b.CombinedHdcpTotal.CompareTo(a.CombinedHdcpTotal));
            int[] teamPlaces = new int[teamRows.Count];
            if (teamRows.Count > 0)
            {
                teamPlaces[0] = 1;
                for (int i = 1; i < teamRows.Count; i++)
                    teamPlaces[i] = teamRows[i].CombinedHdcpTotal == teamRows[i - 1].CombinedHdcpTotal
                        ? teamPlaces[i - 1]
                        : i + 1;
            }

            var result = new List<ExcelMember>();
            for (int t = 0; t < teamRows.Count; t++)
            {
                var (combinedHdcpTotal, m1, m2, h1, b1, h2, b2) = teamRows[t];
                int place = teamPlaces[t];

                if (m1.IsComp) compEntries++;
                if (m2.IsComp) compEntries++;

                result.Add(new ExcelMember
                {
                    MemberNumber = m1.MemberNumber,
                    Name = m1.BowlerName,
                    Handicap = h1,
                    Bonus = b1,
                    MoneyWon = m1.MoneyWon,
                    SidePot = m1.SidePot,
                    GameId = m1.GameId,
                    Game1Score = m1.Game1 ?? 0,
                    Game2Score = m1.Game2 ?? 0,
                    Game3Score = 0,
                    Game4Score = 0,
                    TotalScore = combinedHdcpTotal,
                    PlaceStanding = place
                });

                result.Add(new ExcelMember
                {
                    MemberNumber = m2.MemberNumber,
                    Name = m2.BowlerName,
                    Handicap = h2,
                    Bonus = b2,
                    MoneyWon = m2.MoneyWon,
                    SidePot = m2.SidePot,
                    GameId = m2.GameId,
                    Game1Score = m2.Game1 ?? 0,
                    Game2Score = m2.Game2 ?? 0,
                    Game3Score = 0,
                    Game4Score = 0,
                    TotalScore = combinedHdcpTotal,
                    PlaceStanding = place
                });
            }

            return result;
        }

        private Dictionary<int, int> BuildPrevHdcpByMember(HashSet<int> memberNumbers, int excludeTournamentId)
        {
            var result = new Dictionary<int, int>();
            if (memberNumbers.Count == 0) return result;

            using var dbPrev = _factory.CreateDbContext();

            var latestDates = dbPrev.Participants
                .Where(p => memberNumbers.Contains(p.Member.Number)
                         && p.Tournament.Id != excludeTournamentId
                         && p.Game.IsFinalized
                         && p.Game.AdjustedAvg > 0)
                .GroupBy(p => p.Member.Number)
                .Select(g => new { MemberNumber = g.Key, LatestDate = g.Max(p => p.Tournament.Date) })
                .ToList();

            foreach (var item in latestDates)
            {
                int? adjAvg = dbPrev.Participants
                    .Where(p => p.Member.Number == item.MemberNumber
                             && p.Tournament.Id != excludeTournamentId
                             && p.Game.IsFinalized
                             && p.Tournament.Date == item.LatestDate
                             && p.Game.AdjustedAvg > 0)
                    .Select(p => (int?)p.Game.AdjustedAvg)
                    .FirstOrDefault();

                if (adjAvg.HasValue)
                    result[item.MemberNumber] = TournamentCalculations.CalculateHandicapPins(adjAvg.Value);
            }

            return result;
        }
    }
}
