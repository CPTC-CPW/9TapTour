using System.Collections.Generic;
using NineTapTour.Models;

namespace NineTapTour.Abstractions
{
    public interface IDoublesRepository
    {
        // DoublesTeam
        List<DoublesTeam> GetTeamsByTournament(int tournamentId);
        bool TeamExists(int tournamentId, int memberId1, int memberId2, int squad);
        bool AddTeam(int tournamentId, int memberId1, int memberId2, int squad);
        void RemoveTeam(int teamId);

        // DoublesPartnerPlan
        List<DoublesPartnerPlan> GetPlansByTournament(int tournamentId);
        int GetExpectedPartnerCount(int tournamentId, int memberId, int squad);
        void UpsertPlan(int tournamentId, int memberId, int squad, int expectedPartnerCount);

        // DoublesPartnerClaim
        List<DoublesPartnerClaim> GetClaimsByTournament(int tournamentId);
        bool ClaimExists(int tournamentId, int sourceMemberId, int partnerMemberId, int squad);
        bool AddClaim(int tournamentId, int sourceMemberId, int partnerMemberId, int squad);
        void RemoveClaimsForPair(int tournamentId, int memberId1, int memberId2, int squad);
    }
}
