namespace NineTapTour.Models
{
    /// <summary>
    /// A <see cref="MemberScores"/> carrying the raw per-game data needed to compute a series score
    /// in memory (so a not-yet-bowled null game never nulls the whole total). Setters are public so
    /// the Data layer can populate it from an EF projection.
    /// </summary>
    public class MemberScoresInterim : MemberScores
    {
        public int? Game1Score { get; set; }
        public int? Game2Score { get; set; }
        public int? Game3Score { get; set; }
        public int? Game4Score { get; set; }
        public int? HandicapValue { get; set; }
        public int? BonusPinValue { get; set; }
    }
}
