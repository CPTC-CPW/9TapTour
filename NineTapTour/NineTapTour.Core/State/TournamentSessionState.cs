using System.Collections.Generic;
using NineTapTour.Models;

namespace NineTapTour.State
{
    /// <inheritdoc cref="ITournamentSessionState"/>
    public sealed class TournamentSessionState : ITournamentSessionState
    {
        public Tournament SelectedTournament { get; set; }

        public List<Participant> OverallListOfParticipants { get; set; }

        public bool UnsavedBowlerData { get; set; }
    }
}
