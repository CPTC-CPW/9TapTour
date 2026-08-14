#nullable disable
using NineTapTour.Core.Entities;
using System.Collections.Generic;

namespace NineTapTour.Core.Services;

/// <summary>
/// Cross-form session state for the currently selected tournament. Replaces
/// the FrmMemberScoresHelpers / TempVariablesForGlobalLevel static fields.
/// Registered as a singleton in the desktop app; a future website registers a
/// per-request implementation.
/// </summary>
public interface ITournamentSession
{
    Tournament SelectedTournament { get; set; }

    /// <summary>All participants of the selected tournament (was overallListOfParticipants).</summary>
    List<Participant> Participants { get; set; }

    bool HasUnsavedBowlerData { get; set; }

    /// <summary>Per-place money amounts carried between the results forms.</summary>
    List<double> MoneyEarnings { get; set; }

    bool IsSized { get; set; }
}
