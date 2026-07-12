using System.Collections.Generic;
using NineTapTour.Models;

namespace NineTapTour.Abstractions
{
    /// <summary>Participant CRUD/queries. Part of the former static <c>ParticipantsDB</c>.</summary>
    public interface IParticipantRepository
    {
        bool EnsureParticipantExists(int tournamentId, int memberId, int squad);
        (int Total, Dictionary<int, int> BySquad) GetParticipantNoScoreCounts(int tournamentId);
        List<Participant> GetParticipants(int tournamentId);
        int GetParticipantID(int memberId, int tournyId, int squad);
    }
}
