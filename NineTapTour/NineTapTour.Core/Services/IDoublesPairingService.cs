#nullable disable
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using System.Collections.Generic;

namespace NineTapTour.Core.Services;

/// <summary>
/// Headless doubles pairing logic extracted from FrmDoublesTeamPairing and
/// FrmDoublesDiscrepancies (M7.4). The forms read their controls into plain values,
/// this service performs the Excel import, autosave decisions, and discrepancy
/// set math, and the forms render the results and confirm fixes.
/// </summary>
public interface IDoublesPairingService
{
    /// <summary>
    /// Imports bowlers and expected partner counts from a "Squad N" workbook,
    /// creating participants and upserting partner plans. When participants already
    /// exist (a re-import) the summary also lists removals and count changes
    /// versus the previous import.
    /// </summary>
    DoublesImportSummary ImportBowlersAndExpectedCounts(int tournamentId, int squadCount, string filePath);

    /// <summary>
    /// Saves the expected-partner-count plan for the bowler entered in the form.
    /// Silently does nothing when any input is invalid, matching the old TrySavePlan.
    /// </summary>
    DoublesPlanSaveResult SavePartnerPlan(int tournamentId, string bowlerNumberText, int targetSquad, string partnerCountText);

    /// <summary>
    /// Validates and saves a single partner claim (and its team record) for the
    /// bowler entered in the form, matching the old TrySaveClaim decision chain.
    /// </summary>
    DoublesClaimSaveResult SavePartnerClaim(Tournament tournament, string bowlerNumberText, string partnerNumberText, int targetSquad);

    /// <summary>
    /// Builds the squad-filtered, sorted team list for the pairings grid plus the
    /// summary-panel totals, per-squad breakdown, and discrepancy counts.
    /// </summary>
    DoublesPairingsView GetPairingsView(int tournamentId, int selectedSquad);

    /// <summary>
    /// Builds the imported-bowlers navigation list with planned and entered
    /// partner counts, ordered by squad then member number.
    /// </summary>
    List<DoublesBowlerRosterEntry> GetBowlerRoster(int tournamentId, int selectedSquad);

    /// <summary>
    /// Returns the partners a bowler has explicitly claimed in a squad and the
    /// planned partner count, used to pre-populate the partner entry rows.
    /// </summary>
    DoublesBowlerPlanState GetBowlerPlanState(int tournamentId, int memberId, int squad);

    /// <summary>
    /// Computes all pairing discrepancies for the tournament: missing reciprocals
    /// first (in claim order) followed by count mismatches (in plan order).
    /// </summary>
    List<DoublesDiscrepancy> GetDiscrepancies(int tournamentId);

    /// <summary>
    /// True when a directional claim exists in either direction for the pair,
    /// so the form can warn before removing a pairing.
    /// </summary>
    bool PairHasClaims(int tournamentId, int memberId1, int memberId2, int squad);

    /// <summary>
    /// Removes a doubles team and, when requested, the claims for the pair.
    /// </summary>
    void RemovePairing(int tournamentId, int teamId, int memberId1, int memberId2, int squad, bool removeClaims);

    /// <summary>Adds the reverse claim B→A and ensures the DoublesTeam exists.</summary>
    void FixReciprocal(int tournamentId, int sourceMemberId, int partnerMemberId, int squad);

    /// <summary>Removes the directional claim A→B (and B→A if present) plus the team.</summary>
    void RemoveClaimAndTeam(int tournamentId, int sourceMemberId, int partnerMemberId, int squad);

    /// <summary>
    /// Adds the missing reverse claim for every claim without a reciprocal and
    /// returns how many were fixed.
    /// </summary>
    int FixAllMissingReciprocals(int tournamentId);
}
