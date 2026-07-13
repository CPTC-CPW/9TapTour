using System.Collections.Generic;
using System.Linq;
using NineTapTour.Abstractions;
using NineTapTour.Models;

namespace NineTapTour.Services
{
    /// <summary>
    /// Pure implementation of <see cref="IDoublesDiscrepancyService"/>. Extracted verbatim from the
    /// discrepancy computation duplicated in FrmDoublesDiscrepancies (detailed list) and
    /// FrmDoublesTeamPairing (summary counts). No database access — callers supply the plans/claims.
    /// </summary>
    public sealed class DoublesDiscrepancyService : IDoublesDiscrepancyService
    {
        public IReadOnlyList<DoublesDiscrepancy> FindDiscrepancies(
            IReadOnlyList<DoublesPartnerPlan> plans,
            IReadOnlyList<DoublesPartnerClaim> claims)
        {
            var results = new List<DoublesDiscrepancy>();

            // --- Missing reciprocals: claim A→B with no matching B→A in the same squad ---
            foreach (var claim in claims)
            {
                bool hasReciprocal = claims.Any(r =>
                    r.Squad == claim.Squad &&
                    r.SourceMember.Id == claim.PartnerMember.Id &&
                    r.PartnerMember.Id == claim.SourceMember.Id);

                if (!hasReciprocal)
                {
                    results.Add(new DoublesDiscrepancy
                    {
                        Type = DoublesDiscrepancyType.MissingReciprocal,
                        Squad = claim.Squad,
                        SourceMemberId = claim.SourceMember.Id,
                        SourceMemberNumber = claim.SourceMember.Number,
                        SourceMemberName = $"{claim.SourceMember.FirstName} {claim.SourceMember.LastName}",
                        PartnerMemberId = claim.PartnerMember.Id,
                        PartnerMemberNumber = claim.PartnerMember.Number,
                        PartnerMemberName = $"{claim.PartnerMember.FirstName} {claim.PartnerMember.LastName}"
                    });
                }
            }

            // --- Count mismatches: planned partner count differs from actual claim count ---
            foreach (var plan in plans)
            {
                int actual = claims.Count(c => c.Squad == plan.Squad && c.SourceMember.Id == plan.Member.Id);
                if (actual != plan.ExpectedPartnerCount)
                {
                    results.Add(new DoublesDiscrepancy
                    {
                        Type = DoublesDiscrepancyType.CountMismatch,
                        Squad = plan.Squad,
                        SourceMemberId = plan.Member.Id,
                        SourceMemberNumber = plan.Member.Number,
                        SourceMemberName = $"{plan.Member.FirstName} {plan.Member.LastName}",
                        PlannedCount = plan.ExpectedPartnerCount,
                        ActualCount = actual
                    });
                }
            }

            return results;
        }
    }
}
