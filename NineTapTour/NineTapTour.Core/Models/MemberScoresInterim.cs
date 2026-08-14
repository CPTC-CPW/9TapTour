#nullable disable
using NineTapTour.Core.Entities;

namespace NineTapTour.Core.Models;

public class MemberScoresInterim : MemberScores
{
    public int? Game1Score { get; internal set; }
    public int? Game2Score { get; internal set; }
    public int? Game3Score { get; internal set; }
    public int? Game4Score { get; internal set; }
    public int? HandicapValue { get; internal set; }
    public int? BonusPinValue { get; internal set; }
}
