using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Calculations;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.ViewModels;

namespace NineTapTour.Core.Services;

public class FinalizeGridService : IFinalizeGridService
{
    private const int ThirtyGameWindow = 30;

    private readonly ITournamentRepository tournamentRepository;
    private readonly IMemberRepository memberRepository;
    private readonly IGameRepository gameRepository;
    private readonly IFinalizeTempRepository finalizeTempRepository;
    private readonly IPlayerHistoryRepository playerHistoryRepository;
    private readonly IDoublesTeamRepository doublesTeamRepository;
    private readonly IDbContextFactory<NineTapDb> dbFactory;
    private readonly IFinalizeCalculationService calc;

    public FinalizeGridService(
        ITournamentRepository tournamentRepository,
        IMemberRepository memberRepository,
        IGameRepository gameRepository,
        IFinalizeTempRepository finalizeTempRepository,
        IPlayerHistoryRepository playerHistoryRepository,
        IDoublesTeamRepository doublesTeamRepository,
        IDbContextFactory<NineTapDb> dbFactory,
        IFinalizeCalculationService finalizeCalculationService)
    {
        this.tournamentRepository = tournamentRepository;
        this.memberRepository = memberRepository;
        this.gameRepository = gameRepository;
        this.finalizeTempRepository = finalizeTempRepository;
        this.playerHistoryRepository = playerHistoryRepository;
        this.doublesTeamRepository = doublesTeamRepository;
        this.dbFactory = dbFactory;
        calc = finalizeCalculationService;
    }

    #region Build

    public FinalizeGridModel BuildGrid(int tournamentId, IReadOnlyDictionary<int, int>? newBonusOverridesByMember = null)
    {
        Tournament tournament = GetTournament(tournamentId);
        List<WinnerListMemberViewModel> bowlers = tournamentRepository.GetWinnerListMemberData(tournamentId);
        bool isFinalized = tournament.IsTournamentFinalized;
        HashSet<int> memberNumbers = [.. bowlers.Select(b => b.MemberNumber)];
        Dictionary<int, (int Hdcp, int Bonus)> prevHB = LoadPreviousHandicapAndBonus(memberNumbers, tournamentId);

        FinalizeGridModel grid = tournament.Doubles
            ? BuildDoublesGrid(tournament, bowlers, isFinalized, memberNumbers, prevHB)
            : BuildSinglesGrid(tournament, bowlers, isFinalized, memberNumbers, prevHB);

        ApplyNewBonusPreviews(grid, newBonusOverridesByMember);
        foreach (FinalizeGridRow row in grid.Rows)
        {
            row.IsValid = calc.IsRowValid(row.DirectorCheck, row.AdjAvg);
        }

        return grid;
    }

    private FinalizeGridModel BuildSinglesGrid(Tournament tournament, List<WinnerListMemberViewModel> bowlers,
        bool isFinalized, HashSet<int> memberNumbers, Dictionary<int, (int Hdcp, int Bonus)> prevHB)
    {
        List<ExcelMember> members = BuildExcelMemberList(bowlers, prevHB, isFinalized, tournament.ThreeOutOf4);
        Dictionary<int, int> bestStandingByMember;
        HashSet<int> seededCashingGameIds = [];
        int cashLine;

        if (tournament.IsTwoDay)
        {
            // 2-day championships use the place standings written by the results screen.
            Dictionary<int, int> storedPlaceByGameId = bowlers.ToDictionary(b => b.GameId, b => b.PlaceStanding ?? 0);
            foreach (ExcelMember m in members)
            {
                m.PlaceStanding = storedPlaceByGameId.TryGetValue(m.GameId, out int stored) ? stored : 0;
            }

            bestStandingByMember = bowlers
                .GroupBy(b => b.MemberNumber)
                .ToDictionary(
                    g => g.Key,
                    g => g.Where(b => (b.PlaceStanding ?? 0) > 0)
                          .Select(b => b.PlaceStanding!.Value)
                          .DefaultIfEmpty(int.MaxValue)
                          .Min());

            foreach (WinnerListMemberViewModel b in bowlers.Where(b => (b.MoneyWon ?? 0) > 0))
            {
                seededCashingGameIds.Add(b.GameId);
            }

            cashLine = 0; // no automatic bonus deduction for 2-day championships
        }
        else
        {
            List<ExcelMember> deduped = TournamentCalculations.CalculatePlaceStandings(members, removeDuplicates: true);
            bestStandingByMember = deduped.ToDictionary(m => m.MemberNumber, m => m.PlaceStanding);

            int totalEntries = bowlers.Count;
            int compEntries = bowlers.Count(b => b.IsComp);
            cashLine = TournamentCalculations.GetQtyOfMembersThatCanPlace(totalEntries, compEntries);
        }

        Dictionary<int, int> currentCountByMember = bowlers
            .GroupBy(b => b.MemberNumber)
            .ToDictionary(g => g.Key, g => g.Count());
        Dictionary<int, int> historicalCountByMember = LoadHistoricalEntryCounts(tournament.Id);
        Dictionary<int, (int Scratch, int Games)> history30ByMember =
            LoadHistory30(memberNumbers, tournament.Id, member => currentCountByMember.TryGetValue(member, out int c) ? c : 0);

        // Best entry per member is encountered first, so it gets the standing.
        members.Sort((x, y) => y.TotalScore.CompareTo(x.TotalScore));
        if (!tournament.IsTwoDay)
        {
            HashSet<int> seenMembers = [];
            foreach (ExcelMember m in members)
            {
                m.PlaceStanding = seenMembers.Add(m.MemberNumber) ? bestStandingByMember[m.MemberNumber] : 0;
            }
        }

        HashSet<int> placedGameIds = [.. members.Where(m => m.PlaceStanding > 0).Select(m => m.GameId)];

        if (tournament.IsTwoDay)
        {
            members.Sort((x, y) => x.PlaceStanding != y.PlaceStanding
                ? x.PlaceStanding.CompareTo(y.PlaceStanding)
                : y.TotalScore.CompareTo(x.TotalScore));
        }
        else
        {
            members.Sort((x, y) =>
            {
                int xBest = bestStandingByMember[x.MemberNumber];
                int yBest = bestStandingByMember[y.MemberNumber];
                if (xBest != yBest) return xBest.CompareTo(yBest);
                if (x.MemberNumber != y.MemberNumber) return x.MemberNumber.CompareTo(y.MemberNumber);
                bool xPlaced = x.PlaceStanding > 0;
                bool yPlaced = y.PlaceStanding > 0;
                if (xPlaced && !yPlaced) return -1;
                if (!xPlaced && yPlaced) return 1;
                return y.TotalScore.CompareTo(x.TotalScore);
            });
        }

        Dictionary<int, WinnerListMemberViewModel> bowlerByGameId = bowlers.ToDictionary(b => b.GameId);
        List<FinalizeGridRow> rows = [];
        foreach (ExcelMember m in members)
        {
            WinnerListMemberViewModel orig = bowlerByGameId[m.GameId];

            UseGameFlags useFlags = calc.DetermineUseGameDefaults(
                orig.Game1, orig.Game2, orig.Game3, orig.Game4,
                orig.UseGame1, orig.UseGame2, orig.UseGame3, orig.UseGame4,
                tournament.ThreeOutOf4);

            int baseBonus = ResolveCarryInBonus(orig, isFinalized);
            bool hasPrevHB = prevHB.TryGetValue(m.MemberNumber, out (int Hdcp, int Bonus) prev);
            int displayHdcp = calc.ResolveDisplayHandicap(hasPrevHB ? prev.Hdcp : null, m.Handicap, orig.AdjustedAvg);

            FinalizeRowResult rowCalc = calc.RecalculateRow(new FinalizeRowInput(
                orig.Game1, orig.Game2, orig.Game3, orig.Game4,
                useFlags.Game1, useFlags.Game2, useFlags.Game3, useFlags.Game4,
                displayHdcp, orig.AdjustedAvg, baseBonus));

            int displayAdjAvg = orig.AdjustedAvg > 0 ? orig.AdjustedAvg : (int)Math.Round(orig.LeagueAverage);
            bool isPlaced = placedGameIds.Contains(m.GameId);
            int sidePot = orig.SidePot.HasValue ? (int)orig.SidePot.Value : 0;

            FinalizeGridRow row = new()
            {
                GameId = m.GameId,
                Standing = m.PlaceStanding > 0 ? m.PlaceStanding : null,
                MemberNumber = m.MemberNumber,
                Name = m.Name,
                Game1 = orig.Game1,
                Game2 = orig.Game2,
                Game3 = orig.Game3,
                Game4 = orig.Game4,
                UseGame1 = useFlags.Game1,
                UseGame2 = useFlags.Game2,
                UseGame3 = useFlags.Game3,
                UseGame4 = useFlags.Game4,
                ScratchTotal = rowCalc.ScratchTotal,
                HdcpTotal = rowCalc.HdcpTotal,
                EntryAvg = rowCalc.EntryAvg,
                AdjAvg = displayAdjAvg,
                DirectorCheck = orig.KeepAdjustedAvg,
                Squad = orig.Squad,
                Hdcp = displayHdcp,
                NewHdcp = calc.ComputeNewHdcpPreview(displayAdjAvg),
                Bonus = baseBonus,
                Earnings = EarningsCell(isPlaced, m.MoneyWon, orig.SidePot),
                Notes = null,
                IsPlaced = isPlaced,
                IsCashing = seededCashingGameIds.Contains(m.GameId),
                LeagueAverage = orig.LeagueAverage,
                Placing = bestStandingByMember.TryGetValue(m.MemberNumber, out int placing) ? placing : 0,
                HistoricalEntries = historicalCountByMember.TryGetValue(m.MemberNumber, out int hc) ? hc : 0,
                CurrentEntries = currentCountByMember.TryGetValue(m.MemberNumber, out int cc) ? cc : 0,
                SidePot = sidePot,
            };
            ApplySandbagging(row);
            rows.Add(row);
        }

        FinalizeGridModel grid = new()
        {
            TournamentId = tournament.Id,
            TournamentName = tournament.TourneyNameDate,
            TournamentDate = tournament.Date,
            IsFinalized = isFinalized,
            IsDoubles = false,
            IsTwoDay = tournament.IsTwoDay,
            ThreeOutOf4 = tournament.ThreeOutOf4,
            ShowGame3 = true,
            ShowGame4 = !tournament.IsOnlyThreeGames,
            CashLine = cashLine,
            Rows = rows,
            BestStandingByMember = bestStandingByMember,
        };

        Apply30EntryAverages(grid, history30ByMember);
        return grid;
    }

    private FinalizeGridModel BuildDoublesGrid(Tournament tournament, List<WinnerListMemberViewModel> bowlers,
        bool isFinalized, HashSet<int> memberNumbers, Dictionary<int, (int Hdcp, int Bonus)> prevHB)
    {
        // Each member has one entry per doubles squad (two current games).
        Dictionary<int, (int Scratch, int Games)> history30ByMember = LoadHistory30(memberNumbers, tournament.Id, _ => 2);

        List<DoublesTeam> teams = doublesTeamRepository.GetTeamsByTournament(tournament.Id);
        Dictionary<int, List<WinnerListMemberViewModel>> bowlersByMemberId = bowlers
            .GroupBy(b => b.MemberId)
            .ToDictionary(g => g.Key, g => g.ToList());

        List<(int CombinedHdcpTotal, WinnerListMemberViewModel M1, WinnerListMemberViewModel M2,
              int Hdcp1, int BaseBonus1, int Hdcp2, int BaseBonus2)> teamRows = [];

        foreach (DoublesTeam team in teams)
        {
            if (!bowlersByMemberId.TryGetValue(team.Member1.Id, out List<WinnerListMemberViewModel>? entries1)) continue;
            if (!bowlersByMemberId.TryGetValue(team.Member2.Id, out List<WinnerListMemberViewModel>? entries2)) continue;

            WinnerListMemberViewModel? m1 = entries1.FirstOrDefault(e => e.Squad == team.Squad);
            WinnerListMemberViewModel? m2 = entries2.FirstOrDefault(e => e.Squad == team.Squad);
            if (m1 == null || m2 == null) continue;

            bool has1 = prevHB.TryGetValue(m1.MemberNumber, out (int Hdcp, int Bonus) hb1);
            bool has2 = prevHB.TryGetValue(m2.MemberNumber, out (int Hdcp, int Bonus) hb2);

            int hdcp1 = has1 && hb1.Hdcp > 0 ? hb1.Hdcp : m1.Handicap ?? 0;
            int hdcp2 = has2 && hb2.Hdcp > 0 ? hb2.Hdcp : m2.Handicap ?? 0;
            int baseBonus1 = ResolveCarryInBonus(m1, isFinalized);
            int baseBonus2 = ResolveCarryInBonus(m2, isFinalized);

            int scratch1 = (m1.Game1 ?? 0) + (m1.Game2 ?? 0);
            int scratch2 = (m2.Game1 ?? 0) + (m2.Game2 ?? 0);
            int combined = calc.ComputeCombinedHdcpTotal(scratch1, 2, hdcp1, baseBonus1, scratch2, 2, hdcp2, baseBonus2);

            teamRows.Add((combined, m1, m2, hdcp1, baseBonus1, hdcp2, baseBonus2));
        }

        teamRows.Sort((a, b) => b.CombinedHdcpTotal.CompareTo(a.CombinedHdcpTotal));

        int totalTeams = teamRows.Count;
        int cashLine = totalTeams > 0 ? TournamentCalculations.GetQtyOfMembersThatCanPlace(totalTeams, 0) : 0;
        int[] teamPlaces = calc.AssignTeamPlaces([.. teamRows.Select(r => r.CombinedHdcpTotal)]);

        List<FinalizeGridRow> rows = [];
        Dictionary<int, int> bestStandingByMember = [];
        for (int t = 0; t < totalTeams; t++)
        {
            (int combined, WinnerListMemberViewModel m1, WinnerListMemberViewModel m2, int hdcp1, int baseBonus1, int hdcp2, int baseBonus2) = teamRows[t];
            int place = teamPlaces[t];

            rows.Add(BuildDoublesRow(m1, m2, place, t, combined, hdcp1, baseBonus1));
            rows.Add(BuildDoublesRow(m2, m1, place, t, combined, hdcp2, baseBonus2));

            bestStandingByMember[m1.MemberNumber] = Math.Min(place, bestStandingByMember.GetValueOrDefault(m1.MemberNumber, int.MaxValue));
            bestStandingByMember[m2.MemberNumber] = Math.Min(place, bestStandingByMember.GetValueOrDefault(m2.MemberNumber, int.MaxValue));
        }

        FinalizeGridModel grid = new()
        {
            TournamentId = tournament.Id,
            TournamentName = tournament.TourneyNameDate,
            TournamentDate = tournament.Date,
            IsFinalized = isFinalized,
            IsDoubles = true,
            IsTwoDay = tournament.IsTwoDay,
            ThreeOutOf4 = tournament.ThreeOutOf4,
            ShowGame3 = false,
            ShowGame4 = false,
            CashLine = cashLine,
            Rows = rows,
            BestStandingByMember = bestStandingByMember,
        };

        Apply30EntryAverages(grid, history30ByMember);
        return grid;
    }

    private FinalizeGridRow BuildDoublesRow(WinnerListMemberViewModel me, WinnerListMemberViewModel partner,
        int place, int teamIndex, int combinedHdcpTotal, int hdcp, int baseBonus)
    {
        bool use1 = me.UseGame1 ?? me.Game1.HasValue;
        bool use2 = me.UseGame2 ?? me.Game2.HasValue;
        int scratch = (use1 ? me.Game1 ?? 0 : 0) + (use2 ? me.Game2 ?? 0 : 0);
        int games = (use1 ? 1 : 0) + (use2 ? 1 : 0);
        int adjAvg = me.AdjustedAvg > 0 ? me.AdjustedAvg : (int)Math.Round(me.LeagueAverage);

        FinalizeGridRow row = new()
        {
            GameId = me.GameId,
            PartnerGameId = partner.GameId,
            TeamIndex = teamIndex,
            Standing = place,
            MemberNumber = me.MemberNumber,
            Name = me.BowlerName,
            Game1 = me.Game1,
            Game2 = me.Game2,
            UseGame1 = use1,
            UseGame2 = use2,
            ScratchTotal = scratch,
            HdcpTotal = combinedHdcpTotal,
            EntryAvg = games > 0 ? scratch / games : 0,
            AdjAvg = adjAvg,
            DirectorCheck = me.KeepAdjustedAvg,
            Squad = me.Squad,
            Hdcp = hdcp,
            NewHdcp = calc.ComputeNewHdcpPreview(adjAvg),
            Bonus = baseBonus,
            Earnings = me.MoneyWon > 0 ? me.MoneyWon : null,
            Notes = $"Partner: {partner.BowlerName}",
            IsPlaced = true,
            LeagueAverage = me.LeagueAverage,
            Placing = place,
        };
        ApplySandbagging(row);
        return row;
    }

    /// <summary>
    /// Carry-in bonus an entry is scored with. Before finalization this is the
    /// member's running Member.Bonus (same source as WinnersService); once
    /// finalized the entry's own Game.Bonus is authoritative.
    /// </summary>
    private static int ResolveCarryInBonus(WinnerListMemberViewModel entry, bool isFinalized)
    {
        return isFinalized ? entry.Bonus ?? 0 : entry.MemberBonus;
    }

    private static decimal? EarningsCell(bool isPlaced, decimal? moneyWon, decimal? sidePot)
    {
        if (isPlaced)
        {
            decimal total = (moneyWon ?? 0) + (sidePot ?? 0);
            return total > 0 ? total : null;
        }
        return moneyWon > 0 ? moneyWon : null;
    }

    private List<ExcelMember> BuildExcelMemberList(List<WinnerListMemberViewModel> bowlers,
        Dictionary<int, (int Hdcp, int Bonus)> prevHB, bool isFinalized, bool threeOutOf4)
    {
        List<ExcelMember> members = [];
        foreach (WinnerListMemberViewModel b in bowlers)
        {
            bool hasPrev = prevHB.TryGetValue(b.MemberNumber, out (int Hdcp, int Bonus) prev);
            ExcelMember m = new()
            {
                MemberNumber = b.MemberNumber,
                Name = b.BowlerName,
                Handicap = hasPrev && prev.Hdcp > 0 ? prev.Hdcp : b.Handicap ?? 0,
                Bonus = ResolveCarryInBonus(b, isFinalized),
                MoneyWon = b.MoneyWon,
                GameId = b.GameId,
                Game1Score = b.Game1 ?? 0,
                Game2Score = b.Game2 ?? 0,
                Game3Score = b.Game3 ?? 0,
                Game4Score = b.Game4 ?? 0,
            };
            m.TotalScore = calc.ComputeEntryTotalScore(b.Game1, b.Game2, b.Game3, b.Game4, m.Handicap, m.Bonus, threeOutOf4);
            members.Add(m);
        }
        return members;
    }

    private void ApplySandbagging(FinalizeGridRow row)
    {
        int?[] games = row.Games;
        for (int i = 0; i < 4; i++)
        {
            row.Sandbagging[i] = games[i].HasValue && calc.IsSandbaggingScore(row.LeagueAverage, games[i]!.Value);
        }
    }

    private void Apply30EntryAverages(FinalizeGridModel grid, Dictionary<int, (int Scratch, int Games)> history30ByMember)
    {
        foreach (IGrouping<int, FinalizeGridRow> memberRows in grid.Rows.GroupBy(r => r.MemberNumber))
        {
            int currentScratch = 0;
            int currentGames = 0;
            foreach (FinalizeGridRow row in memberRows)
            {
                (int scratch, int games) = CheckedScratchAndGames(row);
                currentScratch += scratch;
                currentGames += games;
            }

            history30ByMember.TryGetValue(memberRows.Key, out (int Scratch, int Games) history);
            double avg30 = calc.Compute30EntryAverage(history.Scratch, history.Games, currentScratch, currentGames);
            foreach (FinalizeGridRow row in memberRows)
            {
                row.ThirtyEntryAvg = avg30 > 0 ? avg30 : null;
            }
        }
    }

    private static (int Scratch, int Games) CheckedScratchAndGames(FinalizeGridRow row)
    {
        int?[] games = row.Games;
        bool[] use = row.UseGames;
        int scratch = 0;
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (!use[i]) continue;
            scratch += games[i] ?? 0;
            count++;
        }
        return (scratch, count);
    }

    /// <summary>
    /// New Bonus preview for every row: the pins the member carries out. Singles
    /// deduct for cashers and award the third-entry pin; doubles use the half-rate
    /// rule and cash on either partner's earnings. 2-day championships never
    /// auto-deduct; the director's typed override wins there.
    /// </summary>
    private void ApplyNewBonusPreviews(FinalizeGridModel grid, IReadOnlyDictionary<int, int>? overrides)
    {
        Dictionary<int, FinalizeGridRow> rowByGameId = grid.Rows.ToDictionary(r => r.GameId);
        foreach (FinalizeGridRow row in grid.Rows)
        {
            bool isCashing;
            int newBonus;
            if (row.IsDoublesRow)
            {
                FinalizeGridRow? partner = row.PartnerGameId.HasValue ? rowByGameId.GetValueOrDefault(row.PartnerGameId.Value) : null;
                isCashing = (row.Placing > 0 && row.Placing <= grid.CashLine)
                    || (row.Earnings ?? 0) > 0
                    || (partner?.Earnings ?? 0) > 0;
                newBonus = FinalizeCalculationService.ComputeHalfRateBonus(row.Bonus, row.Placing, isCashing);
            }
            else
            {
                decimal memberMoneyWon = grid.Rows
                    .Where(r => r.MemberNumber == row.MemberNumber)
                    .Sum(PlaceMoneyInEarnings);
                BonusPreviewResult preview = calc.ComputeBonusPreview(row.Bonus, row.Placing, grid.CashLine, grid.IsTwoDay,
                    row.HistoricalEntries, row.CurrentEntries, memberMoneyWon);
                isCashing = preview.IsCashing;
                newBonus = preview.DisplayBonus;
            }

            if (grid.IsTwoDay && overrides != null && overrides.TryGetValue(row.MemberNumber, out int overrideBonus))
            {
                newBonus = overrideBonus;
            }

            row.NewBonus = newBonus;

            // 2-day cashing was seeded from money won when the grid was built.
            if (!grid.IsTwoDay)
            {
                row.IsCashing = isCashing;
            }
        }
    }

    /// <summary>
    /// Place money in a row's Earnings cell: the side pot folded into a placed
    /// singles entry's cell is stripped back out.
    /// </summary>
    private static decimal PlaceMoneyInEarnings(FinalizeGridRow row)
    {
        decimal earnings = row.Earnings ?? 0;
        if (row.IsDoublesRow || !row.IsPlaced)
        {
            return earnings;
        }
        return Math.Max(earnings - row.SidePot, 0);
    }

    #endregion

    #region Queries

    private Dictionary<int, (int Hdcp, int Bonus)> LoadPreviousHandicapAndBonus(HashSet<int> memberNumbers, int tournamentId)
    {
        Dictionary<int, (int Hdcp, int Bonus)> result = [];
        using NineTapDb db = dbFactory.CreateDbContext();

        var latestApproved = db.Participants
            .Where(p => memberNumbers.Contains(p.Member.Number)
                     && p.Tournament.Id != tournamentId
                     && p.Game.IsFinalized
                     && p.Game.AdjustedAvg > 0)
            .GroupBy(p => p.Member.Number)
            .Select(g => new { MemberNumber = g.Key, LatestDate = g.Max(p => p.Tournament.Date) })
            .ToList();

        foreach (var item in latestApproved)
        {
            List<PreviousEntrySnapshot> prevEntries = db.Participants
                .Where(p => p.Member.Number == item.MemberNumber
                         && p.Tournament.Id != tournamentId
                         && p.Game.IsFinalized
                         && p.Tournament.Date == item.LatestDate)
                .Select(p => new PreviousEntrySnapshot(p.Game.AdjustedAvg, p.Game.Bonus ?? 0, p.Game.MoneyWon ?? 0))
                .ToList();

            if (prevEntries.Count == 0) continue;
            result[item.MemberNumber] = calc.ComputePreviousHandicapAndBonus(prevEntries);
        }

        return result;
    }

    private Dictionary<int, int> LoadHistoricalEntryCounts(int tournamentId)
    {
        using NineTapDb db = dbFactory.CreateDbContext();
        return db.Participants
            .Where(p => p.Tournament.Id != tournamentId && p.Game.IsFinalized)
            .GroupBy(p => p.Member.Number)
            .Select(g => new { MemberNumber = g.Key, Count = g.Count() })
            .ToDictionary(x => x.MemberNumber, x => x.Count);
    }

    private Dictionary<int, (int Scratch, int Games)> LoadHistory30(HashSet<int> memberNumbers, int tournamentId,
        Func<int, int> currentEntryCountFor)
    {
        Dictionary<int, (int Scratch, int Games)> result = [];
        using NineTapDb db = dbFactory.CreateDbContext();
        var allHistory = db.Participants
            .Where(p => p.Tournament.Id != tournamentId
                     && p.Game.IsFinalized
                     && memberNumbers.Contains(p.Member.Number))
            .OrderByDescending(p => p.Tournament.Date)
            .ThenByDescending(p => p.Game.Id)
            .Select(p => new
            {
                MemberNumber = p.Member.Number,
                G1 = p.Game.Game1, G2 = p.Game.Game2, G3 = p.Game.Game3, G4 = p.Game.Game4,
                U1 = p.Game.UseGame1, U2 = p.Game.UseGame2, U3 = p.Game.UseGame3, U4 = p.Game.UseGame4,
            })
            .ToList();

        foreach (var group in allHistory.GroupBy(x => x.MemberNumber))
        {
            (int scratch, int games) = calc.Compute30EntryHistory(
                group.Select(g => new HistoryGameEntry(g.G1, g.G2, g.G3, g.G4, g.U1, g.U2, g.U3, g.U4)),
                currentEntryCountFor(group.Key));
            if (games > 0)
            {
                result[group.Key] = (scratch, games);
            }
        }

        return result;
    }

    private Tournament GetTournament(int tournamentId)
    {
        return tournamentRepository.GetTourneyByID(tournamentId)
            ?? throw new ArgumentException($"Tournament {tournamentId} was not found.", nameof(tournamentId));
    }

    #endregion

    #region Edit

    public FinalizeGridModel ApplyRowEdit(int tournamentId, FinalizeRowEdit edit, IReadOnlyDictionary<int, int>? newBonusOverridesByMember = null)
    {
        FinalizeGridModel grid = BuildGrid(tournamentId, newBonusOverridesByMember);
        if (grid.IsFinalized)
        {
            return grid;
        }

        FinalizeGridRow row = grid.Rows.FirstOrDefault(r => r.GameId == edit.GameId)
            ?? throw new ArgumentException($"Game {edit.GameId} is not in tournament {tournamentId}.", nameof(edit));

        row.Game1 = edit.Game1;
        row.Game2 = edit.Game2;
        row.Game3 = edit.Game3;
        row.Game4 = edit.Game4;
        row.UseGame1 = edit.UseGame1;
        row.UseGame2 = edit.UseGame2;
        row.UseGame3 = edit.UseGame3;
        row.UseGame4 = edit.UseGame4;
        row.AdjAvg = edit.AdjAvg;
        row.DirectorCheck = edit.DirectorCheck;
        row.Bonus = edit.Bonus;
        row.Earnings = edit.Earnings;
        row.Notes = edit.Notes;
        PersistRow(row);

        // ADJ AVG, carry-in Bonus, and Director Check belong to the member, not the
        // entry, so the desktop mirrored them onto the member's other rows.
        if (edit.Field is FinalizeEditField.AdjAvg or FinalizeEditField.Bonus or FinalizeEditField.DirectorCheck)
        {
            foreach (FinalizeGridRow other in grid.Rows.Where(r => r.MemberNumber == row.MemberNumber && r.GameId != row.GameId))
            {
                switch (edit.Field)
                {
                    case FinalizeEditField.AdjAvg:
                        other.AdjAvg = edit.AdjAvg;
                        break;
                    case FinalizeEditField.Bonus:
                        other.Bonus = edit.Bonus;
                        break;
                    case FinalizeEditField.DirectorCheck:
                        other.DirectorCheck = edit.DirectorCheck;
                        break;
                }
                PersistRow(other);
            }
        }

        return BuildGrid(tournamentId, newBonusOverridesByMember);
    }

    /// <summary>
    /// Writes a row's editable columns to its Game, exactly as the desktop's
    /// PersistRowToDatabase did: the displayed handicap and carry-in bonus are
    /// stored on the entry, and the side pot folded into a placed singles
    /// entry's Earnings cell is stripped back out before saving MoneyWon.
    /// </summary>
    private void PersistRow(FinalizeGridRow row)
    {
        Game? game = gameRepository.GetGame(row.GameId);
        if (game == null)
        {
            return;
        }

        game.Game1 = row.Game1;
        game.Game2 = row.Game2;
        game.Game3 = row.Game3;
        game.Game4 = row.Game4;
        game.UseGame1 = row.UseGame1;
        game.UseGame2 = row.UseGame2;
        game.UseGame3 = row.UseGame3;
        game.UseGame4 = row.UseGame4;
        game.AdjustedAvg = row.AdjAvg;
        game.Handicap = row.Hdcp;
        game.Bonus = row.Bonus;
        game.KeepAdjustedAvg = row.DirectorCheck;

        decimal placeMoney = PlaceMoneyInEarnings(row);
        game.MoneyWon = placeMoney > 0 ? placeMoney : null;
        game.Notes = row.Notes;

        gameRepository.AddOrUpdateGame(game);
    }

    #endregion

    #region Finalize

    public FinalizeOutcome Finalize(int tournamentId, IReadOnlyDictionary<int, int>? newBonusOverridesByMember = null)
    {
        FinalizeGridModel grid = BuildGrid(tournamentId, newBonusOverridesByMember);
        if (grid.IsFinalized)
        {
            return new FinalizeOutcome(false, "This tournament has already been finalized.", []);
        }

        List<int> invalid = [.. grid.Rows.Where(r => !r.IsValid).Select(r => r.GameId)];
        if (invalid.Count > 0)
        {
            return new FinalizeOutcome(false,
                "Some rows are missing a Director Check or have a zero Adjusted Average. Please fix the highlighted rows before finalizing.",
                invalid);
        }

        using NineTapDb db = dbFactory.CreateDbContext();
        HashSet<int> updatedMembers = [];

        foreach (FinalizeGridRow row in grid.Rows)
        {
            Game? game = db.Games.Find(row.GameId);
            if (game == null) continue;

            game.IsFinalized = true;
            game.AdjustedAvg = row.AdjAvg;
            game.KeepAdjustedAvg = row.DirectorCheck;
            game.Handicap = row.Hdcp;
            game.LeagueAverage = finalizeTempRepository.Get30GameAverage(row.MemberNumber, tournamentId);
            // Game.Bonus keeps the carry-in value this entry was scored with, so
            // re-finalizing never deducts twice; New Bonus goes to the member.
            game.Bonus = row.Bonus;

            if (row.IsDoublesRow)
            {
                game.Game1 = row.Game1;
                game.Game2 = row.Game2;
                game.UseGame1 = row.UseGame1;
                game.UseGame2 = row.UseGame2;
                // Director entered the full team prize; each member keeps a 50% share.
                decimal earnings = row.Earnings ?? 0;
                game.MoneyWon = earnings > 0 ? earnings / 2m : null;
                game.PlaceStanding = row.Standing > 0 ? row.Standing : null;
            }

            // Rows are ordered placed-entry-first, so a multi-entry member takes the
            // New Bonus from the entry they placed with.
            if (row.MemberNumber > 0 && updatedMembers.Add(row.MemberNumber))
            {
                Member member = memberRepository.GetMember(row.MemberNumber, db);
                if (member != null && member.Id > 0)
                {
                    member.Average = row.AdjAvg;
                    member.Handicap = TournamentCalculations.CalculateHandicapPins(row.AdjAvg);
                    member.Bonus = row.NewBonus;
                }
            }
        }

        Tournament? tournament = db.Tournaments.Find(tournamentId);
        if (tournament != null)
        {
            tournament.IsTournamentFinalized = true;
        }

        db.SaveChanges();
        return new FinalizeOutcome(true, "Tournament has been finalized successfully.", []);
    }

    #endregion

    #region Detail and team view

    public FinalizeDetailModel BuildDetail(FinalizeGridModel grid, int memberNumber)
    {
        List<PlayerHistoryViewModel> history = playerHistoryRepository.GetMemberPlayerHistory(memberNumber);
        List<FinalizeGridRow> memberRows = [.. grid.Rows.Where(r => r.MemberNumber == memberNumber)];
        string memberName = memberRows.FirstOrDefault()?.Name ?? string.Empty;

        // Once finalized these entries are part of the member's history below.
        bool memberCashed = memberRows.Any(r => r.IsCashing);
        List<FinalizeGridRow> currentEntries = grid.IsFinalized
            ? []
            : memberCashed
                ? [.. memberRows.OrderByDescending(LiveWithHandicap)]
                : [.. memberRows.OrderByDescending(r => r.Squad)];

        int historyLimit = Math.Max(ThirtyGameWindow - currentEntries.Count, 0);
        int historyScratch30 = history.Take(historyLimit).Sum(h => h.TotalScore);
        int historyGames30 = history.Take(historyLimit).Sum(h => h.GamesPlayed);
        int currentScratch30 = 0;
        int currentGames30 = 0;

        List<FinalizeDetailRow> rows = [];
        foreach (FinalizeGridRow r in currentEntries)
        {
            int? g1 = r.UseGame1 ? r.Game1 : null;
            int? g2 = r.UseGame2 ? r.Game2 : null;
            int? g3 = r.UseGame3 ? r.Game3 : null;
            int? g4 = r.UseGame4 ? r.Game4 : null;
            int validGames = (g1.HasValue ? 1 : 0) + (g2.HasValue ? 1 : 0) + (g3.HasValue ? 1 : 0) + (g4.HasValue ? 1 : 0);
            int scratch = (g1 ?? 0) + (g2 ?? 0) + (g3 ?? 0) + (g4 ?? 0);

            rows.Add(new FinalizeDetailRow
            {
                Games = validGames,
                Date = grid.TournamentDate,
                Game1 = g1,
                Game2 = g2,
                Game3 = g3,
                Game4 = g4,
                Scratch = scratch,
                WithHandicap = scratch + (validGames * (r.Hdcp + r.Bonus)),
                Entry = validGames > 0 ? scratch / validGames : 0,
                AdjustedAvg = r.AdjAvg > 0 ? r.AdjAvg : null,
                Handicap = r.Hdcp,
                Bonus = r.Bonus,
                Place = r.IsPlaced && grid.BestStandingByMember.TryGetValue(memberNumber, out int place) && place != int.MaxValue
                    ? place.ToString()
                    : null,
                Earnings = r.Earnings,
                IsCurrent = true,
                HighlightBonus = r.IsCashing && r.IsPlaced,
            });
            currentScratch30 += scratch;
            currentGames30 += validGames;
        }

        double preview30 = calc.Compute30EntryAverage(historyScratch30, historyGames30, currentScratch30, currentGames30);
        foreach (FinalizeDetailRow row in rows)
        {
            row.ThirtyAverage = preview30 > 0 ? preview30 : null;
        }

        // Finalized history: grouped by date, first entry of the date first, with
        // that date's earnings shown once on the first entry.
        int historyIndex = 0;
        foreach (IGrouping<DateTime, PlayerHistoryViewModel> dateGroup in history.GroupBy(h => h.TournamentDate.Date))
        {
            List<PlayerHistoryViewModel> entriesForDate = [.. dateGroup.Reverse()];
            for (int j = 0; j < entriesForDate.Count; j++)
            {
                PlayerHistoryViewModel h = entriesForDate[j];
                decimal dateEarnings = j == 0 ? entriesForDate.Sum(e => e.MoneyWon) : 0;
                rows.Add(new FinalizeDetailRow
                {
                    Games = h.GamesPlayed,
                    Date = h.TournamentDate,
                    Game1 = h.Game1,
                    Game2 = h.Game2,
                    Game3 = h.Game3,
                    Game4 = h.Game4,
                    Scratch = h.TotalScore,
                    WithHandicap = h.TotalScore + (h.GamesPlayed * (h.HandiCap + h.Bonus)),
                    Entry = h.GamesPlayed > 0 ? h.TotalScore / h.GamesPlayed : 0,
                    ThirtyAverage = h.trueAVG > 0 ? Math.Round(h.trueAVG, 1) : null,
                    AdjustedAvg = h.AVG > 0 ? h.AVG : null,
                    Handicap = h.HandiCap,
                    Bonus = h.Bonus,
                    Place = string.IsNullOrEmpty(h.PPHG) ? null : h.PPHG,
                    Earnings = dateEarnings > 0 ? dateEarnings : null,
                    Notes = h.Notes,
                    HighlightBonus = h.MoneyWon > 0,
                    HighlightThirtyAverage = historyIndex < ThirtyGameWindow,
                });
                historyIndex++;
            }
        }

        decimal lifetimeEarnings = history.Sum(h => h.MoneyWon);
        double leagueAvg = history.FirstOrDefault()?.trueAVG ?? 0;

        return new FinalizeDetailModel
        {
            MemberNumber = memberNumber,
            MemberName = memberName,
            LeagueAverage = (int)Math.Round(leagueAvg),
            ColumnHeaders = BuildDetailHeaders([.. history.Take(ThirtyGameWindow)], lifetimeEarnings, preview30),
            Rows = rows,
        };
    }

    private static int LiveWithHandicap(FinalizeGridRow row)
    {
        (int scratch, int games) = CheckedScratchAndGames(row);
        return scratch + games * (row.Hdcp + row.Bonus);
    }

    private static Dictionary<string, string> BuildDetailHeaders(List<PlayerHistoryViewModel> last30, decimal lifetimeEarnings, double preview30Avg)
    {
        if (last30.Count == 0)
        {
            return new Dictionary<string, string>
            {
                ["Games"] = "Games",
                ["Game1"] = "Game1",
                ["Game2"] = "Game2",
                ["Game3"] = "Game3",
                ["Game4"] = "Game4",
                ["Scratch"] = "Scratch",
                ["WHdcp"] = "w/HDCP",
                ["Entry"] = "Entry",
                ["ThirtyAvg"] = "30 AVG",
                ["Earnings"] = $"Earnings ({lifetimeEarnings:0.00})",
            };
        }

        int totalGames = last30.Sum(h => h.GamesPlayed);
        int scratch = last30.Sum(h => h.TotalScore);
        int wHdcpSum = last30.Sum(h => h.TotalScore + h.GamesPlayed * (h.HandiCap + h.Bonus));
        int entryAvg = totalGames > 0 ? scratch / totalGames : 0;
        double avg30 = totalGames > 0 ? (double)scratch / totalGames : 0;
        double headerAvg30 = preview30Avg > 0 ? preview30Avg : avg30;

        return new Dictionary<string, string>
        {
            ["Games"] = $"Games ({totalGames})",
            ["Game1"] = $"Game1 ({last30.Sum(h => h.Game1 ?? 0)})",
            ["Game2"] = $"Game2 ({last30.Sum(h => h.Game2 ?? 0)})",
            ["Game3"] = $"Game3 ({last30.Sum(h => h.Game3 ?? 0)})",
            ["Game4"] = $"Game4 ({last30.Sum(h => h.Game4 ?? 0)})",
            ["Scratch"] = $"Scratch ({scratch})",
            ["WHdcp"] = $"w/HDCP ({wHdcpSum})",
            ["Entry"] = $"Entry ({entryAvg})",
            ["ThirtyAvg"] = $"30 AVG ({headerAvg30:0.#})",
            ["Earnings"] = $"Earnings ({lifetimeEarnings:0.00})",
        };
    }

    public List<FinalizeTeamRow> BuildTeamView(FinalizeGridModel grid)
    {
        // Doubles rows are written in consecutive pairs [M1, M2, M1, M2, ...].
        List<FinalizeGridRow> doublesRows = [.. grid.Rows.Where(r => r.IsDoublesRow)];
        List<FinalizeTeamRow> teams = [];
        for (int i = 0; i + 1 < doublesRows.Count; i += 2)
        {
            FinalizeGridRow row = doublesRows[i];
            FinalizeGridRow partner = doublesRows[i + 1];
            teams.Add(new FinalizeTeamRow(
                row.Standing > 0 ? row.Standing : null,
                row.Name,
                partner.Name,
                row.ScratchTotal,
                partner.ScratchTotal,
                row.HdcpTotal,
                row.Bonus,
                partner.Bonus,
                row.Earnings,
                partner.Earnings));
        }
        return teams;
    }

    #endregion
}
