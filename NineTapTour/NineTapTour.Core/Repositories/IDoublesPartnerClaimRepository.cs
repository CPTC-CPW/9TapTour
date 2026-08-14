#nullable disable
using NineTapTour.Core.Entities;
using System.Collections.Generic;

namespace NineTapTour.Core.Repositories;

/// <summary>
/// Data access for doubles partner claims. Instance replacement for the old static DoublesPartnerClaimDB;
/// method names and behavior are unchanged.
/// </summary>
public interface IDoublesPartnerClaimRepository
{
    List<DoublesPartnerClaim> GetClaimsByTournament(int tournamentId);
    bool ClaimExists(int tournamentId, int sourceMemberId, int partnerMemberId, int squad);
    bool AddClaim(int tournamentId, int sourceMemberId, int partnerMemberId, int squad);
    void RemoveClaimsForPair(int tournamentId, int memberId1, int memberId2, int squad);
}
