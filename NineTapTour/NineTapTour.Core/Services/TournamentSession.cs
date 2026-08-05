#nullable disable
using NineTapTour.Core.Entities;
using System.Collections.Generic;

namespace NineTapTour.Core.Services;

public class TournamentSession : ITournamentSession
{
    public Tournament SelectedTournament { get; set; }

    public List<Participant> Participants { get; set; }

    public bool HasUnsavedBowlerData { get; set; }

    public List<double> MoneyEarnings { get; set; }

    public bool IsSized { get; set; }
}
