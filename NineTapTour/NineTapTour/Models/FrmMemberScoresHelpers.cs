using NineTapTour.State;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NineTapTour.Models;

/// <summary>
/// Legacy static access point for tournament session state plus a couple of small UI helpers.
/// The three state members now delegate to the DI-managed <see cref="ITournamentSessionState"/>
/// singleton (set in <c>Program.Main</c>) so legacy static access and injected session state share
/// one storage during the migration to constructor injection.
/// </summary>
public static class FrmMemberScoresHelpers
{
    /// <summary>The DI session-state singleton. Defaults to a standalone instance for design-time/tests.</summary>
    public static ITournamentSessionState Session { get; set; } = new TournamentSessionState();

    public static Tournament selectedTournament
    {
        get => Session.SelectedTournament;
        set => Session.SelectedTournament = value;
    }

    public static List<Participant> overallListOfParticipants
    {
        get => Session.OverallListOfParticipants;
        set => Session.OverallListOfParticipants = value;
    }

    public static bool unsavedBowlerData
    {
        get => Session.UnsavedBowlerData;
        set => Session.UnsavedBowlerData = value;
    }

    /// <summary>Checks a string for numeric values. True if it parses as an int.</summary>
    public static bool IsNumeric(string str) => int.TryParse(str, out _);

    /// <summary>Checks for an empty text box.</summary>
    public static bool IsEmpty(TextBox box) => string.IsNullOrEmpty(box.Text.Trim());
}
