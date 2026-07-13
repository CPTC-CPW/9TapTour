namespace NineTapTour.Models
{
    /// <summary>
    /// The finalized state of one grid row (a bowler's game entry) gathered from the finalize UI.
    /// The form reads its grid cells into these DTOs; <c>IFinalizationService</c> then applies and
    /// persists them, so the finalize sequencing/computation is UI-free and testable.
    /// </summary>
    public sealed class FinalizeGameInput
    {
        public int GameId { get; set; }

        /// <summary>True for an individual member row of a doubles team (only 2 games are used).</summary>
        public bool IsDoublesMember { get; set; }

        public int MemberNumber { get; set; }

        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }

        public bool UseGame1 { get; set; }
        public bool UseGame2 { get; set; }
        public bool UseGame3 { get; set; }
        public bool UseGame4 { get; set; }

        public int AdjustedAvg { get; set; }
        public int Handicap { get; set; }

        /// <summary>Bonus value shown in the grid (used for the Member record and non-cashing games).</summary>
        public int BonusFromGrid { get; set; }

        public int PlaceStanding { get; set; }
        public decimal Earnings { get; set; }

        /// <summary>Director-check flag → persisted as Game.KeepAdjustedAvg.</summary>
        public bool DirectorCheck { get; set; }

        public bool IsCashing { get; set; }
        public bool IsThirdEntryBonus { get; set; }

        /// <summary>
        /// The bowler's pre-tournament base bonus (from the original winner-list entry). Used for the
        /// half-rate doubles calc, and to preserve the Game bonus for cashing/third-entry singles.
        /// </summary>
        public int OriginalBaseBonus { get; set; }
    }
}
