using System.Collections.Generic;
using NineTapTour.Models;

namespace NineTapTour.State
{
    /// <summary>
    /// Holds UI-session state that used to live in the static <c>FrmMemberScoresHelpers</c> fields,
    /// so forms can share it via dependency injection instead of global mutable statics.
    /// Registered as a singleton for the lifetime of the app.
    /// </summary>
    public interface ITournamentSessionState
    {
        /// <summary>The tournament currently selected in Member Scores.</summary>
        Tournament SelectedTournament { get; set; }

        /// <summary>The participant list for the currently selected tournament.</summary>
        List<Participant> OverallListOfParticipants { get; set; }

        /// <summary>True when the user has entered bowler scores that have not yet been saved.</summary>
        bool UnsavedBowlerData { get; set; }
    }
}
