using System.Collections.Generic;

namespace NineTapTour.Models
{
    /// <summary>
    /// The computed tournament winners list plus the entry counts the results UI needs to determine
    /// how many places can cash. Produced by <c>ITournamentResultsService</c>.
    /// </summary>
    public sealed class WinnersListResult
    {
        public List<ExcelMember> Bowlers { get; set; } = new();

        /// <summary>Total number of entries (all squads) in the tournament.</summary>
        public int TotalEntries { get; set; }

        /// <summary>Number of comp entries (bowlers who do not pay an entry fee).</summary>
        public int CompEntries { get; set; }
    }
}
