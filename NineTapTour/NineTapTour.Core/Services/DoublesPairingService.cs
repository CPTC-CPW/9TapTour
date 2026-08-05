#nullable disable
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Core.Services;

/// <summary>
/// Headless doubles pairing logic. Excel import, plan/claim autosave decisions,
/// discrepancy set math, and pairing reconciliation were moved verbatim from
/// FrmDoublesTeamPairing and FrmDoublesDiscrepancies (M7.4); the pure computations
/// are public statics so they can be characterization-tested without a database.
/// </summary>
public class DoublesPairingService : IDoublesPairingService
{
    private readonly IMemberRepository memberRepository;
    private readonly ITournamentRepository tournamentRepository;
    private readonly IParticipantRepository participantRepository;
    private readonly IDoublesTeamRepository doublesTeamRepository;
    private readonly IDoublesPartnerPlanRepository doublesPartnerPlanRepository;
    private readonly IDoublesPartnerClaimRepository doublesPartnerClaimRepository;
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public DoublesPairingService(
        IMemberRepository memberRepository,
        ITournamentRepository tournamentRepository,
        IParticipantRepository participantRepository,
        IDoublesTeamRepository doublesTeamRepository,
        IDoublesPartnerPlanRepository doublesPartnerPlanRepository,
        IDoublesPartnerClaimRepository doublesPartnerClaimRepository,
        IDbContextFactory<NineTapDb> dbFactory)
    {
        this.memberRepository = memberRepository;
        this.tournamentRepository = tournamentRepository;
        this.participantRepository = participantRepository;
        this.doublesTeamRepository = doublesTeamRepository;
        this.doublesPartnerPlanRepository = doublesPartnerPlanRepository;
        this.doublesPartnerClaimRepository = doublesPartnerClaimRepository;
        this.dbFactory = dbFactory;
    }

    // ----------------------------------------------------------------
    // Excel import
    // ----------------------------------------------------------------

    public DoublesImportSummary ImportBowlersAndExpectedCounts(int tournamentId, int squadCount, string filePath)
    {
        // Snapshot current state for the re-import diff
        List<(int MemberNumber, int Squad)> prevParticipants;
        using (NineTapDb db = dbFactory.CreateDbContext())
        {
            prevParticipants = db.Participants
                .Where(p => p.Tournament.Id == tournamentId)
                .Select(p => new { p.Member.Number, p.Squad })
                .AsEnumerable()
                .Select(x => (x.Number, x.Squad))
                .ToList();
        }
        List<DoublesPartnerPlan> existingPlansList = doublesPartnerPlanRepository.GetPlansByTournament(tournamentId);
        Dictionary<(int MemberNumber, int Squad), int> prevPlans = existingPlansList.ToDictionary(
            p => (p.Member.Number, p.Squad),
            p => p.ExpectedPartnerCount);

        bool isReimport = prevParticipants.Count > 0;

        DoublesImportSummary summary;
        using (XLWorkbook workbook = new(filePath))
        {
            summary = RunImport(
                workbook,
                squadCount,
                memberRepository.GetMemberIdByNumber,
                (memberId, squad) => ParticipantExists(tournamentId, memberId, squad),
                (memberId, squad) => participantRepository.EnsureParticipantExists(tournamentId, memberId, squad),
                (memberId, squad, expectedCount) => doublesPartnerPlanRepository.UpsertPlan(tournamentId, memberId, squad, expectedCount));
        }

        // Compute re-import diff
        if (isReimport)
        {
            List<DoublesPartnerPlan> updatedPlans = doublesPartnerPlanRepository.GetPlansByTournament(tournamentId);
            Dictionary<(int MemberNumber, int Squad), int> updatedPlanDict = updatedPlans.ToDictionary(
                p => (p.Member.Number, p.Squad),
                p => p.ExpectedPartnerCount);

            ApplyReimportDiff(summary, prevParticipants, prevPlans, updatedPlanDict);
        }

        return summary;
    }

    /// <summary>
    /// Runs the workbook import against the given lookups and writers. Kept as a
    /// static with delegate seams so the row parsing and per-row decision chain can
    /// be characterization-tested with in-memory workbooks and dictionaries.
    /// Logic moved verbatim from FrmDoublesTeamPairing.ImportBowlersAndExpectedCounts.
    /// </summary>
    public static DoublesImportSummary RunImport(
        XLWorkbook workbook,
        int squadCount,
        Func<int, int> getMemberIdByNumber,
        Func<int, int, bool> participantExists,
        Func<int, int, bool> ensureParticipantExists,
        Action<int, int, int> upsertPlan)
    {
        DoublesImportSummary summary = new();

        foreach (IXLWorksheet ws in workbook.Worksheets)
        {
            if (!TryParseSquadSheetName(ws.Name, out int squad))
                continue;

            if (squad < 1 || squad > squadCount)
            {
                summary.Errors.Add($"Sheet '{ws.Name}': squad is out of range for this tournament.");
                continue;
            }

            int row = 9;
            while (!ws.Cell(row, 2).IsEmpty())
            {
                summary.RowsProcessed++;

                if (ws.Cell(row, 3).IsEmpty())
                {
                    summary.RowsSkipped++;
                    row++;
                    continue;
                }

                if (!TryReadIntCell(ws, row, 3, out int memberNumber))
                {
                    summary.RowsSkipped++;
                    summary.Errors.Add($"Sheet '{ws.Name}', row {row}: invalid member number in column C.");
                    row++;
                    continue;
                }

                if (!TryReadIntCell(ws, row, 10, out int expectedCount))
                {
                    summary.RowsSkipped++;
                    summary.Errors.Add($"Sheet '{ws.Name}', row {row}: invalid partner count in column J.");
                    row++;
                    continue;
                }

                if (expectedCount < 0)
                {
                    summary.RowsSkipped++;
                    summary.Errors.Add($"Sheet '{ws.Name}', row {row}: partner count cannot be negative.");
                    row++;
                    continue;
                }

                int memberId = getMemberIdByNumber(memberNumber);
                if (memberId == 0)
                {
                    summary.RowsSkipped++;
                    summary.Errors.Add($"Sheet '{ws.Name}', row {row}: member #{memberNumber} not found.");
                    row++;
                    continue;
                }

                bool participantAlreadyExisted = participantExists(memberId, squad);
                bool ensured = ensureParticipantExists(memberId, squad);
                if (!ensured)
                {
                    summary.RowsSkipped++;
                    summary.Errors.Add($"Sheet '{ws.Name}', row {row}: could not create participant for #{memberNumber}.");
                    row++;
                    continue;
                }

                if (!participantAlreadyExisted)
                    summary.ParticipantsCreated++;

                upsertPlan(memberId, squad, expectedCount);
                summary.PlansUpserted++;
                summary.ProcessedEntries.Add((memberNumber, squad));

                row++;
            }
        }

        return summary;
    }

    /// <summary>
    /// Fills the summary's re-import diff lists: participants no longer present
    /// (by tournament or by squad) and plans whose partner count changed.
    /// Logic moved verbatim from FrmDoublesTeamPairing.BtnImportExcel_Click.
    /// </summary>
    public static void ApplyReimportDiff(
        DoublesImportSummary summary,
        List<(int MemberNumber, int Squad)> previousParticipants,
        Dictionary<(int MemberNumber, int Squad), int> previousPlans,
        Dictionary<(int MemberNumber, int Squad), int> updatedPlans)
    {
        HashSet<(int MemberNumber, int Squad)> processedSet = summary.ProcessedEntries;
        HashSet<int> allProcessedNums = new(processedSet.Select(entry => entry.MemberNumber));

        foreach ((int memberNumber, int squad) in previousParticipants)
        {
            if (!processedSet.Contains((memberNumber, squad)))
            {
                if (!allProcessedNums.Contains(memberNumber))
                    summary.RemovedFromTournament.Add($"#{memberNumber} removed from tournament entirely");
                else
                    summary.RemovedFromSquad.Add($"#{memberNumber} no longer in Squad {squad}");
            }
        }

        foreach (KeyValuePair<(int MemberNumber, int Squad), int> kvp in previousPlans)
        {
            if (updatedPlans.TryGetValue(kvp.Key, out int newCount) && newCount != kvp.Value)
                summary.PartnerCountChanged.Add($"#{kvp.Key.MemberNumber} (Squad {kvp.Key.Squad}): {kvp.Value} → {newCount} partners");
        }
    }

    /// <summary>
    /// Reads an integer cell, accepting int, double (rounded), or numeric text.
    /// Moved verbatim from FrmDoublesTeamPairing.TryReadIntCell.
    /// </summary>
    public static bool TryReadIntCell(IXLWorksheet ws, int row, int column, out int value)
    {
        value = 0;
        IXLCell cell = ws.Cell(row, column);
        if (cell.IsEmpty())
            return false;

        if (cell.TryGetValue<int>(out int asInt))
        {
            value = asInt;
            return true;
        }

        if (cell.TryGetValue<double>(out double asDouble))
        {
            value = Convert.ToInt32(Math.Round(asDouble));
            return true;
        }

        return int.TryParse(cell.GetString().Trim(), out value);
    }

    /// <summary>
    /// Parses worksheet names of the form "Squad N" (case-insensitive) into a squad number.
    /// Moved verbatim from FrmDoublesTeamPairing.TryParseSquadSheetName.
    /// </summary>
    public static bool TryParseSquadSheetName(string sheetName, out int squad)
    {
        squad = 0;
        if (string.IsNullOrWhiteSpace(sheetName))
            return false;

        string normalized = sheetName.Trim();
        if (!normalized.StartsWith("Squad ", StringComparison.OrdinalIgnoreCase))
            return false;

        return int.TryParse(normalized.Substring(6).Trim(), out squad);
    }

    // ----------------------------------------------------------------
    // Plan/claim autosave
    // ----------------------------------------------------------------

    public DoublesPlanSaveResult SavePartnerPlan(int tournamentId, string bowlerNumberText, int targetSquad, string partnerCountText)
    {
        if (!int.TryParse(bowlerNumberText.Trim(), out int mainNum))
            return new DoublesPlanSaveResult(false, null);
        int mainId = memberRepository.GetMemberIdByNumber(mainNum);
        if (mainId == 0)
            return new DoublesPlanSaveResult(false, null);
        if (targetSquad == 0)
            return new DoublesPlanSaveResult(false, null);
        if (!int.TryParse(partnerCountText.Trim(), out int expectedCount) || expectedCount < 0)
            return new DoublesPlanSaveResult(false, null);
        doublesPartnerPlanRepository.UpsertPlan(tournamentId, mainId, targetSquad, expectedCount);
        return new DoublesPlanSaveResult(true, $"Plan saved: {expectedCount} partner(s) for #{mainNum}");
    }

    public DoublesClaimSaveResult SavePartnerClaim(Tournament tournament, string bowlerNumberText, string partnerNumberText, int targetSquad)
    {
        string partnerText = partnerNumberText.Trim();
        if (string.IsNullOrWhiteSpace(partnerText))
            return new DoublesClaimSaveResult(false, null, false);

        if (!int.TryParse(bowlerNumberText.Trim(), out int mainNum))
            return new DoublesClaimSaveResult(false, "Enter a valid bowler number first.", true);
        int mainId = memberRepository.GetMemberIdByNumber(mainNum);
        if (mainId == 0)
            return new DoublesClaimSaveResult(false, $"Bowler #{mainNum} not found.", true);
        if (targetSquad == 0)
            return new DoublesClaimSaveResult(false, "Select a squad first.", true);
        if (!int.TryParse(partnerText, out int partnerNum))
            return new DoublesClaimSaveResult(false, $"'{partnerText}' is not a valid member number.", true);
        int partnerId = memberRepository.GetMemberIdByNumber(partnerNum);
        if (partnerId == 0)
            return new DoublesClaimSaveResult(false, $"#{partnerNum} not found.", true);
        if (partnerId == mainId)
            return new DoublesClaimSaveResult(false, $"#{partnerNum} cannot be paired with themselves.", true);

        HashSet<int> validIds = GetValidMemberIds(tournament, targetSquad);
        if (!validIds.Contains(mainId))
            return new DoublesClaimSaveResult(false, $"#{mainNum} not in Squad {targetSquad}.", true);
        if (!validIds.Contains(partnerId))
            return new DoublesClaimSaveResult(false, $"#{partnerNum} not in Squad {targetSquad}.", true);

        bool claimAdded = doublesPartnerClaimRepository.AddClaim(tournament.Id, mainId, partnerId, targetSquad);
        doublesTeamRepository.AddTeam(tournament.Id, mainId, partnerId, targetSquad);

        // When the claim already existed (e.g. a pre-filled box was tabbed through) no status is shown
        return new DoublesClaimSaveResult(true,
            claimAdded ? $"#{mainNum} & #{partnerNum} saved (Squad {targetSquad})." : null,
            false);
    }

    // ----------------------------------------------------------------
    // Pairings grid + summary reconciliation
    // ----------------------------------------------------------------

    public DoublesPairingsView GetPairingsView(int tournamentId, int selectedSquad)
    {
        List<DoublesTeam> allTeams = doublesTeamRepository.GetTeamsByTournament(tournamentId);
        List<DoublesTeam> teams = FilterAndSortTeams(allTeams, selectedSquad);

        List<DoublesPartnerPlan> plans = doublesPartnerPlanRepository.GetPlansByTournament(tournamentId);
        List<DoublesPartnerClaim> claims = doublesPartnerClaimRepository.GetClaimsByTournament(tournamentId);

        int countMismatches = plans.Count(p =>
            claims.Count(c => c.Squad == p.Squad && c.SourceMember.Id == p.Member.Id) != p.ExpectedPartnerCount);

        int reciprocalMissing = claims.Count(c =>
            !claims.Any(r =>
                r.Squad == c.Squad &&
                r.SourceMember.Id == c.PartnerMember.Id &&
                r.PartnerMember.Id == c.SourceMember.Id));

        return new DoublesPairingsView(teams, allTeams.Count, ComputeSquadBreakdown(allTeams), countMismatches, reciprocalMissing);
    }

    /// <summary>
    /// Filters the tournament's teams to a squad (0 = all) and sorts by squad
    /// then bowler-1 number. Moved verbatim from FrmDoublesTeamPairing.LoadPairings.
    /// </summary>
    public static List<DoublesTeam> FilterAndSortTeams(List<DoublesTeam> teams, int selectedSquad)
    {
        List<DoublesTeam> filtered = selectedSquad > 0
            ? teams.FindAll(t => t.Squad == selectedSquad)
            : [.. teams];

        // Sort: by squad asc, then by member1 number
        filtered.Sort((a, b) => a.Squad != b.Squad ? a.Squad.CompareTo(b.Squad)
                                                   : a.Member1.Number.CompareTo(b.Member1.Number));
        return filtered;
    }

    /// <summary>
    /// Groups teams by squad in ascending order for the summary breakdown.
    /// Moved verbatim from FrmDoublesTeamPairing.UpdateSummaryLabels.
    /// </summary>
    public static List<DoublesSquadTeamCount> ComputeSquadBreakdown(List<DoublesTeam> teams)
    {
        return teams
            .GroupBy(t => t.Squad)
            .OrderBy(g => g.Key)
            .Select(g => new DoublesSquadTeamCount(g.Key, g.Count()))
            .ToList();
    }

    // ----------------------------------------------------------------
    // Bowler roster + partner pre-population
    // ----------------------------------------------------------------

    public List<DoublesBowlerRosterEntry> GetBowlerRoster(int tournamentId, int selectedSquad)
    {
        List<DoublesParticipantInfo> participants;
        using (NineTapDb db = dbFactory.CreateDbContext())
        {
            participants = db.Participants
                .Where(p => p.Tournament.Id == tournamentId)
                .Select(p => new DoublesParticipantInfo(p.Squad, p.Member.Id, p.Member.Number, p.Member.FirstName, p.Member.LastName))
                .ToList();
        }

        List<DoublesPartnerPlan> plans = doublesPartnerPlanRepository.GetPlansByTournament(tournamentId);
        List<DoublesPartnerClaim> claims = doublesPartnerClaimRepository.GetClaimsByTournament(tournamentId);

        return BuildBowlerRoster(participants, selectedSquad, plans, claims);
    }

    /// <summary>
    /// Reconciles participants against plans and claims to produce the navigation
    /// list entries with planned/entered counts, filtered by squad (0 = all) and
    /// ordered by squad then member number.
    /// Moved verbatim from FrmDoublesTeamPairing.PopulateBowlersList.
    /// </summary>
    public static List<DoublesBowlerRosterEntry> BuildBowlerRoster(
        List<DoublesParticipantInfo> participants, int selectedSquad,
        List<DoublesPartnerPlan> plans, List<DoublesPartnerClaim> claims)
    {
        if (selectedSquad > 0)
            participants = participants.Where(p => p.Squad == selectedSquad).ToList();

        return participants
            .OrderBy(p => p.Squad)
            .ThenBy(p => p.MemberNumber)
            .Select(p => new DoublesBowlerRosterEntry(
                p.Squad,
                p.MemberId,
                p.MemberNumber,
                p.FirstName,
                p.LastName,
                plans.FirstOrDefault(x => x.Squad == p.Squad && x.Member.Id == p.MemberId)?.ExpectedPartnerCount ?? 0,
                claims.Count(c => c.Squad == p.Squad && c.SourceMember.Id == p.MemberId)))
            .ToList();
    }

    public DoublesBowlerPlanState GetBowlerPlanState(int tournamentId, int memberId, int squad)
    {
        // Show only partners this bowler has explicitly claimed.
        List<DoublesPartnerClaim> allClaims = doublesPartnerClaimRepository.GetClaimsByTournament(tournamentId);
        List<Member> claimedPartners = allClaims
            .Where(c => c.Squad == squad && c.SourceMember.Id == memberId)
            .Select(c => c.PartnerMember)
            .ToList();

        int expectedCount = doublesPartnerPlanRepository.GetExpectedPartnerCount(tournamentId, memberId, squad);
        return new DoublesBowlerPlanState(claimedPartners, expectedCount);
    }

    // ----------------------------------------------------------------
    // Discrepancies
    // ----------------------------------------------------------------

    public List<DoublesDiscrepancy> GetDiscrepancies(int tournamentId)
    {
        List<DoublesPartnerPlan> plans = doublesPartnerPlanRepository.GetPlansByTournament(tournamentId);
        List<DoublesPartnerClaim> claims = doublesPartnerClaimRepository.GetClaimsByTournament(tournamentId);
        return ComputeDiscrepancies(plans, claims);
    }

    /// <summary>
    /// Computes the discrepancy list: missing reciprocals first (in claim order)
    /// followed by count mismatches (in plan order).
    /// Moved verbatim from FrmDoublesDiscrepancies.LoadDiscrepancies.
    /// </summary>
    public static List<DoublesDiscrepancy> ComputeDiscrepancies(List<DoublesPartnerPlan> plans, List<DoublesPartnerClaim> claims)
    {
        List<DoublesDiscrepancy> discrepancies = [];

        // --- Missing reciprocals ---
        foreach (DoublesPartnerClaim claim in claims)
        {
            bool hasReciprocal = claims.Any(r =>
                r.Squad == claim.Squad &&
                r.SourceMember.Id == claim.PartnerMember.Id &&
                r.PartnerMember.Id == claim.SourceMember.Id);

            if (!hasReciprocal)
            {
                discrepancies.Add(new DoublesDiscrepancy
                {
                    Type = DoublesDiscrepancyType.MissingReciprocal,
                    Squad = claim.Squad,
                    SourceMemberId = claim.SourceMember.Id,
                    SourceMemberNumber = claim.SourceMember.Number,
                    SourceMemberName = $"{claim.SourceMember.FirstName} {claim.SourceMember.LastName}",
                    PartnerMemberId = claim.PartnerMember.Id,
                    PartnerMemberNumber = claim.PartnerMember.Number,
                    PartnerMemberName = $"{claim.PartnerMember.FirstName} {claim.PartnerMember.LastName}"
                });
            }
        }

        // --- Count mismatches ---
        foreach (DoublesPartnerPlan plan in plans)
        {
            int actual = claims.Count(c => c.Squad == plan.Squad && c.SourceMember.Id == plan.Member.Id);
            if (actual != plan.ExpectedPartnerCount)
            {
                discrepancies.Add(new DoublesDiscrepancy
                {
                    Type = DoublesDiscrepancyType.CountMismatch,
                    Squad = plan.Squad,
                    SourceMemberId = plan.Member.Id,
                    SourceMemberNumber = plan.Member.Number,
                    SourceMemberName = $"{plan.Member.FirstName} {plan.Member.LastName}",
                    PlannedCount = plan.ExpectedPartnerCount,
                    ActualCount = actual
                });
            }
        }

        return discrepancies;
    }

    // ----------------------------------------------------------------
    // Fix / remove actions
    // ----------------------------------------------------------------

    public bool PairHasClaims(int tournamentId, int memberId1, int memberId2, int squad)
    {
        bool claim1Exists = doublesPartnerClaimRepository.ClaimExists(tournamentId, memberId1, memberId2, squad);
        bool claim2Exists = doublesPartnerClaimRepository.ClaimExists(tournamentId, memberId2, memberId1, squad);
        return claim1Exists || claim2Exists;
    }

    public void RemovePairing(int tournamentId, int teamId, int memberId1, int memberId2, int squad, bool removeClaims)
    {
        if (removeClaims)
            doublesPartnerClaimRepository.RemoveClaimsForPair(tournamentId, memberId1, memberId2, squad);
        doublesTeamRepository.RemoveTeam(teamId);
    }

    public void FixReciprocal(int tournamentId, int sourceMemberId, int partnerMemberId, int squad)
    {
        // Add the missing reverse claim
        doublesPartnerClaimRepository.AddClaim(tournamentId, partnerMemberId, sourceMemberId, squad);
        // Ensure the team record exists (order-independent; AddTeam is a no-op if duplicate)
        doublesTeamRepository.AddTeam(tournamentId, sourceMemberId, partnerMemberId, squad);
    }

    public void RemoveClaimAndTeam(int tournamentId, int sourceMemberId, int partnerMemberId, int squad)
    {
        doublesPartnerClaimRepository.RemoveClaimsForPair(tournamentId, sourceMemberId, partnerMemberId, squad);

        // Also remove the DoublesTeam if it exists
        List<DoublesTeam> teams = doublesTeamRepository.GetTeamsByTournament(tournamentId);
        DoublesTeam team = teams.FirstOrDefault(t =>
            t.Squad == squad &&
            ((t.Member1.Id == sourceMemberId && t.Member2.Id == partnerMemberId) ||
             (t.Member1.Id == partnerMemberId && t.Member2.Id == sourceMemberId)));
        if (team != null)
            doublesTeamRepository.RemoveTeam(team.Id);
    }

    public int FixAllMissingReciprocals(int tournamentId)
    {
        List<DoublesPartnerClaim> claims = doublesPartnerClaimRepository.GetClaimsByTournament(tournamentId);

        int fixedCount = 0;
        foreach (DoublesPartnerClaim claim in claims)
        {
            bool hasReciprocal = claims.Any(r =>
                r.Squad == claim.Squad &&
                r.SourceMember.Id == claim.PartnerMember.Id &&
                r.PartnerMember.Id == claim.SourceMember.Id);

            if (!hasReciprocal)
            {
                FixReciprocal(tournamentId, claim.SourceMember.Id, claim.PartnerMember.Id, claim.Squad);
                fixedCount++;
            }
        }

        return fixedCount;
    }

    // ----------------------------------------------------------------
    // Squad-aware helpers
    // ----------------------------------------------------------------

    private bool ParticipantExists(int tournamentId, int memberId, int squad)
    {
        using NineTapDb db = dbFactory.CreateDbContext();
        return db.Participants.Any(p =>
            p.Tournament.Id == tournamentId &&
            p.Member.Id == memberId &&
            p.Squad == squad);
    }

    private HashSet<int> GetMemberIdsInSquad(int tournamentId, int squad)
    {
        using NineTapDb db = dbFactory.CreateDbContext();
        return new HashSet<int>(
            db.Participants
              .Where(p => p.Tournament.Id == tournamentId && p.Squad == squad)
              .Select(p => p.Member.Id));
    }

    private HashSet<int> GetValidMemberIds(Tournament tournament, int squadIndex)
    {
        if (squadIndex > 0)
            return GetMemberIdsInSquad(tournament.Id, squadIndex);

        return new HashSet<int>(
            tournamentRepository.GetUniqueTourMembers(tournament).Select(m => m.Id));
    }
}
