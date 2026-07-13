namespace NineTapTour.Models
{
    /// <summary>
    /// The kind of doubles pairing discrepancy found for a tournament.
    /// </summary>
    public enum DoublesDiscrepancyType
    {
        /// <summary>Claim A→B exists but the reciprocal B→A does not.</summary>
        MissingReciprocal,

        /// <summary>A bowler's planned partner count differs from their actual claim count.</summary>
        CountMismatch
    }

    /// <summary>
    /// A single doubles pairing discrepancy. Carries the member numbers/names the UI needs so the
    /// grid can render without re-querying. Produced by <c>IDoublesDiscrepancyService</c>.
    /// </summary>
    public sealed class DoublesDiscrepancy
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
}
