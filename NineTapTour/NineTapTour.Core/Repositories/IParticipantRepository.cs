#nullable disable
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using System.Collections.Generic;

namespace NineTapTour.Core.Repositories;

/// <summary>
/// Data access for participants. Instance replacement for the old static ParticipantsDB;
/// method names and behavior are unchanged.
/// </summary>
public interface IParticipantRepository
{
    bool EnsureParticipantExists(int tournamentId, int memberId, int squad);
    (int Total, Dictionary<int, int> BySquad) GetParticipantNoScoreCounts(int tournamentId);
    List<Participant> GetParticipants(int TournamentID);
    List<MemberScores> GetGameMemberScores(int TournamentID);
    List<MemberScores> GetSeniorMemberScores(int selectedTourneyId);
    List<MemberScores> GetStandingsForThreeOf4ByScratch(int selectedTournament);
    List<MemberScores> GetStandingsForTournamentByHandicap(int selectedTournament, bool isThreeOfFourTournament = false);
    List<MemberScores> GetStandingsForTournamentByScratch(int selectedTournament, bool isThreeOfFourTournament = false);
    List<MemberScores> GetStandingsForThreeOutOf4ByFilterSeriesByHandicap(List<int> squadList, int selectedTournament);
    List<MemberScores> GetStandingsForTournamentByFilterSeriesByHandicap(List<int> squadList, int selectedTournament, bool isThreeOfFourTournament = false);
    List<MemberScores> GetStandingsForThreeOf4ByFilterSeriesByScratch(List<int> squadList, int selectedTournament);
    List<MemberScores> GetStandingsForTournamentByFilterSeriesByScratch(List<int> squadList, int selectedTournament, bool isThreeOfFourTournament = false);
    int GetParticipantID(NineTapDb db, int memberId, int tournyId, int squad);
}
