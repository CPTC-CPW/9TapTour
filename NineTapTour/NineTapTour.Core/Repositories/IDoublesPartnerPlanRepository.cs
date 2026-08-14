#nullable disable
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using System.Collections.Generic;

namespace NineTapTour.Core.Repositories;

/// <summary>
/// Data access for doubles partner plans. Instance replacement for the old static DoublesPartnerPlanDB;
/// method names and behavior are unchanged.
/// </summary>
public interface IDoublesPartnerPlanRepository
{
    List<DoublesPartnerPlan> GetPlansByTournament(int tournamentId);
    int GetExpectedPartnerCount(int tournamentId, int memberId, int squad);
    void UpsertPlan(int tournamentId, int memberId, int squad, int expectedPartnerCount);
    void UpsertPlan(NineTapDb db, int tournamentId, int memberId, int squad, int expectedPartnerCount);
}
