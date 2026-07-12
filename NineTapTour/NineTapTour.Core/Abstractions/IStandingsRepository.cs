using System.Collections.Generic;
using NineTapTour.Models;

namespace NineTapTour.Abstractions
{
    /// <summary>Standings/scores queries. Part of the former static <c>ParticipantsDB</c>.</summary>
    public interface IStandingsRepository
    {
        List<MemberScores> GetGameMemberScores(int TournamentID);
        List<MemberScores> GetSeniorMemberScores(int selectedTourneyId);
        List<MemberScores> GetStandingsForTournamentByHandicap(int selectedTournament, bool isThreeOfFourTournament = false);
        List<MemberScores> GetStandingsForTournamentByScratch(int selectedTournament, bool isThreeOfFourTournament = false);
        List<MemberScores> GetStandingsForTournamentByFilterSeriesByHandicap(List<int> squadList, int selectedTournament, bool isThreeOfFourTournament = false);
        List<MemberScores> GetStandingsForTournamentByFilterSeriesByScratch(List<int> squadList, int selectedTournament, bool isThreeOfFourTournament = false);
    }
}
