using System.Collections.Generic;
using NineTapTour.Models;

namespace NineTapTour.Abstractions
{
    /// <summary>Data access for <see cref="Game"/> records. Replaces the static <c>GameDB</c>.</summary>
    public interface IGameRepository
    {
        void AddOrUpdateGame(Game game);
        Game GetGame(int gameID);
        void AddOrUpdateSomeGames(List<Game> games);
        Game GetGameInTournament(int memberID, int tournamentID, int squad);
        int GetGameID(int memberId, int tournyId, int squad);
        List<Game> GetFinalizedGamesByTournament(int tournamentId);
        List<Game> GetFinalizedGamesByMember(int memberNumber, int regionId);
        bool IsGameFinalized(int gameId);
    }
}
