#nullable disable
using NineTapTour.Core.Entities;
using System.Collections.Generic;

namespace NineTapTour.Core.Models;

/// <summary>
/// Result of the doubles bowler/expected-partner-count Excel import.
/// Moved verbatim from the FrmDoublesTeamPairing.ImportSummary inner class (M7.4).
/// The re-import diff lists are only populated when a previous import existed.
/// </summary>
public class DoublesImportSummary
{
    public int RowsProcessed { get; set; }
    public int RowsSkipped { get; set; }
    public int PlansUpserted { get; set; }
    public int ParticipantsCreated { get; set; }
    public List<string> Errors { get; } = new();
    public HashSet<(int MemberNumber, int Squad)> ProcessedEntries { get; } = new();
    public List<string> RemovedFromSquad { get; } = new();
    public List<string> RemovedFromTournament { get; } = new();
    public List<string> PartnerCountChanged { get; } = new();
}

/// <summary>
/// Result of a partner-plan autosave attempt. When nothing valid was entered
/// the plan is silently not saved and <see cref="StatusMessage"/> is null,
/// matching the original TrySavePlan behavior.
/// </summary>
public record DoublesPlanSaveResult(bool Saved, string StatusMessage);

/// <summary>
/// Result of a partner-claim autosave attempt. <see cref="Persisted"/> is true when the
/// claim/team were written (the form should refresh its grid). <see cref="StatusMessage"/>
/// is null for silent no-ops (empty input, or a pre-filled claim that already existed).
/// </summary>
public record DoublesClaimSaveResult(bool Persisted, string StatusMessage, bool IsError);

/// <summary>
/// The two kinds of pairing discrepancy. Values match the old
/// FrmDoublesDiscrepancies.DiscrepancyType enum stored in the grid's hidden column.
/// </summary>
public enum DoublesDiscrepancyType
{
    MissingReciprocal = 0,
    CountMismatch = 1
}

/// <summary>
/// One pairing discrepancy: either claim A→B without B→A (MissingReciprocal),
/// or a bowler whose actual claim count differs from the planned count (CountMismatch).
/// Moved verbatim from the FrmDoublesDiscrepancies.DiscrepancyItem inner class.
/// </summary>
public class DoublesDiscrepancy
{
    public DoublesDiscrepancyType Type { get; set; }
    public int Squad { get; set; }
    public int SourceMemberId { get; set; }
    public int SourceMemberNumber { get; set; }
    public string SourceMemberName { get; set; }
    // MissingReciprocal only
    public int PartnerMemberId { get; set; }
    public int PartnerMemberNumber { get; set; }
    public string PartnerMemberName { get; set; }
    // CountMismatch only
    public int PlannedCount { get; set; }
    public int ActualCount { get; set; }
}

/// <summary>
/// Team count for one squad, used by the summary panel breakdown.
/// </summary>
public record DoublesSquadTeamCount(int Squad, int Count);

/// <summary>
/// Everything the pairings form needs to render its grid and summary panel:
/// the squad-filtered, sorted teams plus tournament-wide totals and discrepancy counts.
/// </summary>
public record DoublesPairingsView(
    List<DoublesTeam> Teams,
    int TotalTeamCount,
    List<DoublesSquadTeamCount> TeamsBySquad,
    int CountMismatches,
    int MissingReciprocals);

/// <summary>
/// One entry in the imported-bowlers navigation list: participant identity plus
/// the planned and entered partner counts reconciled from plans and claims.
/// </summary>
public record DoublesBowlerRosterEntry(
    int Squad, int MemberId, int MemberNumber,
    string FirstName, string LastName,
    int PlannedCount, int EnteredCount);

/// <summary>
/// Flat participant projection used as input to the roster reconciliation.
/// </summary>
public record DoublesParticipantInfo(int Squad, int MemberId, int MemberNumber, string FirstName, string LastName);

/// <summary>
/// The partners a bowler has explicitly claimed in a squad plus the planned
/// partner count, used to pre-populate the partner entry rows.
/// </summary>
public record DoublesBowlerPlanState(List<Member> ClaimedPartners, int ExpectedPartnerCount);
