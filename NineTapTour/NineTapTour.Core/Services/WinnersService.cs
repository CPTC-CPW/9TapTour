#nullable disable
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Calculations;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NineTapTour.Core.Services;

/// <summary>
/// Headless winners-list and payout building logic.
/// Logic was moved verbatim from FrmTournamentResults (M7.2); the pure computations
/// are public statics so they can be characterization-tested without a database.
/// </summary>
public class WinnersService : IWinnersService
{
    private readonly ITournamentRepository tournamentRepository;
    private readonly IDoublesTeamRepository doublesTeamRepository;
    private readonly IMemberRepository memberRepository;
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public WinnersService(
        ITournamentRepository tournamentRepository,
        IDoublesTeamRepository doublesTeamRepository,
        IMemberRepository memberRepository,
        IDbContextFactory<NineTapDb> dbFactory)
    {
        this.tournamentRepository = tournamentRepository;
        this.doublesTeamRepository = doublesTeamRepository;
        this.memberRepository = memberRepository;
        this.dbFactory = dbFactory;
    }

    public WinnersListResult BuildWinnersList(WinnersListRequest request)
    {
        List<WinnerListMemberViewModel> bowlers = tournamentRepository.GetWinnerListMemberData(request.TournamentId);

        // Batch-query each member's handicap from their most recent finalized prior tournament.
        // Bonus is read directly from the Member record via MemberBonus (not game history).
        var memberNumbers = bowlers.Select(b => b.MemberNumber).Distinct().ToHashSet();
        var prevHdcpByMember = BuildPrevHdcpByMember(memberNumbers, request.TournamentId);

        if (request.Doubles)
        {
            List<DoublesTeam> teams = doublesTeamRepository.GetTeamsByTournament(request.TournamentId);
            return ComputeDoublesWinnersRows(bowlers, teams, prevHdcpByMember);
        }

        return ComputeWinnersRows(bowlers, prevHdcpByMember, request.ThreeOutOf4);
    }

    /// <summary>
    /// Pure computation of the singles winners list from pre-fetched bowler data:
    /// resolves each member's handicap (previous-tournament value when positive,
    /// otherwise the stored game handicap) and computes the handicap total score.
    /// For 3-of-4 tournaments the lowest of four games is dropped (zeroed).
    /// </summary>
    public static WinnersListResult ComputeWinnersRows(
        List<WinnerListMemberViewModel> bowlers,
        IReadOnlyDictionary<int, int> prevHdcpByMember,
        bool threeOutOf4)
    {
        List<ExcelMember> tournyBowlers = [];
        int compEntries = 0;

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
                    : Convert.ToInt32(b.Handicap),
                Bonus = b.MemberBonus,
                MoneyWon = b.MoneyWon,
                SidePot = b.SidePot,
                GameId = b.GameId,
                // If the game scores are null then a 0 will be placed in the the game
                Game1Score = Convert.ToInt32(b.Game1),
                Game2Score = Convert.ToInt32(b.Game2),
                Game3Score = Convert.ToInt32(b.Game3),
                Game4Score = Convert.ToInt32(b.Game4)
            };

            if (threeOutOf4)
            {
                List<int> scores = [m.Game1Score, m.Game2Score, m.Game3Score, m.Game4Score];

                // Remove the 0s from the scores list
                scores.RemoveAll(x => x == 0);

                // remove lowest score if there are 4 games
                if (scores.Count == 4)
                {
                    int minScore = scores.Min();
                    scores.Remove(minScore);
                    if (m.Game1Score == minScore)
                        m.Game1Score = 0;
                    else if (m.Game2Score == minScore)
                        m.Game2Score = 0;
                    else if (m.Game3Score == minScore)
                        m.Game3Score = 0;
                    else if (m.Game4Score == minScore)
                        m.Game4Score = 0;
                }

                m.TotalScore = scores.Sum()
                    + (scores.Count * (m.Handicap + m.Bonus));
            }
            else
            {
                int totalValidGames = 0;
                if (m.Game1Score > 0)
                    totalValidGames++;
                if (m.Game2Score > 0)
                    totalValidGames++;
                if (m.Game3Score > 0)
                    totalValidGames++;
                if (m.Game4Score > 0)
                    totalValidGames++;

                m.TotalScore = m.Game1Score + m.Game2Score + m.Game3Score
                    + m.Game4Score + (totalValidGames * (m.Handicap + m.Bonus));
            }
            tournyBowlers.Add(m);
        }

        return new WinnersListResult(tournyBowlers, bowlers.Count, compEntries);
    }

    /// <summary>
    /// Pure computation of the doubles winners list from pre-fetched bowler and team data.
    /// Each DoublesTeam produces two ExcelMember entries with the same PlaceStanding and
    /// TotalScore (combined team HDCP total) so they appear as ties in the results grid.
    /// MoneyWon is already stored at the individual 50% share by FrmFinalizeTournament.
    /// </summary>
    public static WinnersListResult ComputeDoublesWinnersRows(
        List<WinnerListMemberViewModel> bowlers,
        List<DoublesTeam> teams,
        IReadOnlyDictionary<int, int> prevHdcpByMember)
    {
        int compEntries = 0;
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

            int hdcp1  = prevHdcpByMember.TryGetValue(m1.MemberNumber, out int ph1) && ph1 > 0 ? ph1 : Convert.ToInt32(m1.Handicap);
            int hdcp2  = prevHdcpByMember.TryGetValue(m2.MemberNumber, out int ph2) && ph2 > 0 ? ph2 : Convert.ToInt32(m2.Handicap);
            int bonus1 = m1.MemberBonus;
            int bonus2 = m2.MemberBonus;

            int combinedHdcpTotal = (m1.Game1 ?? 0) + (m1.Game2 ?? 0)
                                  + (m2.Game1 ?? 0) + (m2.Game2 ?? 0)
                                  + 2 * (hdcp1 + bonus1)
                                  + 2 * (hdcp2 + bonus2);

            teamRows.Add((combinedHdcpTotal, m1, m2, hdcp1, bonus1, hdcp2, bonus2));
        }

        // Sort descending, assign places with tie detection
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
                MemberNumber  = m1.MemberNumber,
                Name          = m1.BowlerName,
                Handicap      = h1,
                Bonus         = b1,
                MoneyWon      = m1.MoneyWon,    // already stored as 50% share by FrmFinalizeTournament
                SidePot       = m1.SidePot,
                GameId        = m1.GameId,
                Game1Score    = m1.Game1 ?? 0,
                Game2Score    = m1.Game2 ?? 0,
                Game3Score    = 0,
                Game4Score    = 0,
                TotalScore    = combinedHdcpTotal,
                PlaceStanding = place
            });

            result.Add(new ExcelMember
            {
                MemberNumber  = m2.MemberNumber,
                Name          = m2.BowlerName,
                Handicap      = h2,
                Bonus         = b2,
                MoneyWon      = m2.MoneyWon,
                SidePot       = m2.SidePot,
                GameId        = m2.GameId,
                Game1Score    = m2.Game1 ?? 0,
                Game2Score    = m2.Game2 ?? 0,
                Game3Score    = 0,
                Game4Score    = 0,
                TotalScore    = combinedHdcpTotal,
                PlaceStanding = place
            });
        }

        return new WinnersListResult(result, bowlers.Count, compEntries);
    }

    public Dictionary<int, int> BuildPrevHdcpByMember(HashSet<int> memberNumbers, int excludeTournamentId)
    {
        var result = new Dictionary<int, int>();
        if (memberNumbers.Count == 0) return result;

        using var dbPrev = dbFactory.CreateDbContext();

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

    public TwoDayAutoFillResult AutoFillTwoDayMember(int memberNumber, int tournamentId)
    {
        Member member = memberRepository.GetMember(memberNumber);
        if (member == null || member.Id == 0)
        {
            return new TwoDayAutoFillResult(TwoDayAutoFillStatus.MemberNotFound, "", "", 0, 0, 0);
        }

        // Bonus always comes from the Member record.
        // Handicap comes from the most recent finalized prior tournament's AdjustedAvg (falls back to Member.Handicap).
        int bonus = member.Bonus;
        var prevHdcpByMember = BuildPrevHdcpByMember([memberNumber], tournamentId);
        int hdcp = prevHdcpByMember.TryGetValue(memberNumber, out int prevHdcp)
            ? prevHdcp
            : (member.Handicap ?? 0);

        // Get the highest-scoring game entry for this member in this tournament (all squads).
        // ScratchTotal is [NotMapped] so ordering must happen client-side after fetching candidates.
        Game game;
        using (var dbGame = dbFactory.CreateDbContext())
        {
            game = dbGame.Participants
                .Where(p => p.Member.Id == member.Id && p.Tournament.Id == tournamentId)
                .Select(p => p.Game)
                .AsEnumerable()
                .OrderByDescending(g => g.ScratchTotal)
                .ThenByDescending(g => g.GamesPlayed)
                .ThenByDescending(g => g.Id)
                .FirstOrDefault();
        }

        if (game == null)
        {
            return new TwoDayAutoFillResult(TwoDayAutoFillStatus.GameNotFound, "", "", 0, 0, 0);
        }

        int totalScore = game.ScratchTotal + (game.GamesPlayed * (hdcp + bonus));

        return new TwoDayAutoFillResult(
            TwoDayAutoFillStatus.Success,
            member.FirstName + " " + member.LastName,
            $"{hdcp} + {bonus}",
            totalScore,
            member.Number,
            game.Id);
    }

    public Dictionary<int, bool> GetMembershipCurrentByMemberNumber(IReadOnlyCollection<int> memberNumbers)
    {
        var result = new Dictionary<int, bool>();
        if (memberNumbers.Count == 0) return result;

        using var dbMembers = dbFactory.CreateDbContext();
        return dbMembers.Members
            .Where(m => memberNumbers.Contains(m.Number))
            .Select(m => new { m.Number, m.IsLifetimeMember, m.LastPayment })
            .ToDictionary(
                x => x.Number,
                x => x.IsLifetimeMember
                    || (x.LastPayment.HasValue && (x.LastPayment.Value.Year + 1) >= DateTime.Today.Year));
    }

    /// <summary>
    /// Returns the ordinal display for a place standing (1st, 2nd, 3rd, 11th, ...)
    /// with a trailing "T" when the place is tied.
    /// </summary>
    public static string GetOrdinalWithTie(int place, bool isTie)
    {
        string suffix;
        int ones = place % 10;
        int tens = (place % 100) / 10;
        if (tens == 1)
            suffix = "th";
        else if (ones == 1)
            suffix = "st";
        else if (ones == 2)
            suffix = "nd";
        else if (ones == 3)
            suffix = "rd";
        else
            suffix = "th";
        return $"{place}{suffix}{(isTie ? "T" : "")}";
    }

    /// <summary>
    /// Builds the display label for a 2-day place grouping, e.g. "46th - 59th".
    /// </summary>
    public static string Build2DayPlaceGroupLabel(int startPlace, int endPlace)
    {
        return $"{GetOrdinalWithTie(startPlace, false)} - {GetOrdinalWithTie(endPlace, false)}";
    }

    /// <summary>
    /// Parses the starting place number from a place label that is either a range
    /// ("46th - 59th") or a single placing ("5", "3rd", "12T").
    /// </summary>
    public static bool TryParsePlaceStartFromText(string text, out int placeStart)
    {
        placeStart = 0;
        if (string.IsNullOrWhiteSpace(text)) return false;

        Match range = RegexHelpers.PlacingRange().Match(text.Trim());
        if (range.Success)
            return int.TryParse(range.Groups[1].Value, out placeStart) && placeStart > 0;

        Match single = RegexHelpers.SinglePlacing().Match(text.Trim());
        return single.Success
            && int.TryParse(single.Groups[1].Value, out placeStart)
            && placeStart > 0;
    }

    /// <summary>
    /// Parses a place-standing cell value that may have a trailing "T" tie indicator
    /// and returns the numeric portion as a <see cref="byte"/>.
    /// </summary>
    public static byte ParsePlaceStanding(object value)
    {
        string s = value?.ToString()?.TrimEnd('T') ?? "0";
        return byte.TryParse(s, out byte result) ? result : (byte)0;
    }

    /// <summary>
    /// Appends "T" to any place value that ties a neighboring row (same numeric place).
    /// Returns a new list; non-numeric or zero places pass through unchanged.
    /// </summary>
    public static List<string> ApplyTieMarkers(IReadOnlyList<string> placeValues)
    {
        List<string> result = [.. placeValues];
        for (int i = 0; i < result.Count; i++)
        {
            if (!int.TryParse(result[i], out int place) || place == 0)
                continue;
            bool isTie = (i > 0
                            && int.TryParse(result[i - 1]?.TrimEnd('T'), out int prev)
                            && prev == place)
                       || (i < result.Count - 1
                            && int.TryParse(result[i + 1]?.TrimEnd('T'), out int next)
                            && next == place);
            if (isTie)
                result[i] = $"{place}T";
        }
        return result;
    }

    /// <summary>
    /// Computes the place shown on an empty filler row below the cashed winners.
    /// For doubles, consecutive filler rows share a team place (2 rows per team slot).
    /// </summary>
    public static int ComputeFillerPlace(bool isDoubles, int filledRowCount, int fillerRowIndex)
    {
        if (isDoubles)
        {
            int filledTeams  = filledRowCount / 2;
            int fillerOffset = fillerRowIndex - filledRowCount;
            return filledTeams + fillerOffset / 2 + 1;
        }
        return fillerRowIndex + 1;
    }

    /// <summary>
    /// Reconstructs doubles team pairings from a winners list written in consecutive
    /// pairs ([T1M1, T1M2, T2M1, T2M2, ...]), keeping teams placing at or above
    /// <paramref name="maxPlace"/>, sorted by place, with tie detection.
    /// </summary>
    public static List<DoublesTeamPairing> BuildTeamPairings(IReadOnlyList<ExcelMember> winners, int maxPlace)
    {
        var teamPairs = new List<(ExcelMember M1, ExcelMember M2, int Place)>();
        for (int i = 0; i + 1 < winners.Count; i += 2)
        {
            var m1 = winners[i];
            var m2 = winners[i + 1];
            if (m1.PlaceStanding > maxPlace) continue;
            teamPairs.Add((m1, m2, m1.PlaceStanding));
        }
        teamPairs.Sort((a, b) => a.Place.CompareTo(b.Place));

        var result = new List<DoublesTeamPairing>();
        for (int i = 0; i < teamPairs.Count; i++)
        {
            var (m1, m2, place) = teamPairs[i];
            bool isTie = (i > 0 && teamPairs[i - 1].Place == place)
                      || (i < teamPairs.Count - 1 && teamPairs[i + 1].Place == place);
            result.Add(new DoublesTeamPairing(m1, m2, place, isTie));
        }
        return result;
    }
}
