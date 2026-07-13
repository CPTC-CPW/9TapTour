using System.Collections.Generic;
using NineTapTour.Models;

namespace NineTapTour.Abstractions
{
    /// <summary>
    /// Finds doubles pairing discrepancies (missing reciprocal claims and planned-vs-actual count
    /// mismatches) from a tournament's partner plans and claims. Pure logic extracted from
    /// FrmDoublesDiscrepancies / FrmDoublesTeamPairing so both forms share one implementation and it
    /// can be unit-tested without a database.
    /// </summary>
    public interface IDoublesDiscrepancyService
    {
        IReadOnlyList<DoublesDiscrepancy> FindDiscrepancies(
            IReadOnlyList<DoublesPartnerPlan> plans,
            IReadOnlyList<DoublesPartnerClaim> claims);
    }
}
