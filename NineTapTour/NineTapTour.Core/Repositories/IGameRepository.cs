#nullable disable
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using System.Collections.Generic;

namespace NineTapTour.Core.Repositories;

/// <summary>
/// Data access for games. Instance replacement for the old static GameDB;
/// method names and behavior are unchanged.
/// </summary>
public interface IGameRepository
{
    void AddOrUpdateGame(Game game);
    void AddOrUpdateGame(Game game, NineTapDb db);
    Game GetGame(int gameID);
    void AddOrUpdateSomeGames(List<Game> games);
    Game GetGameInTournament(int memberID, int tournamentID, int squad);
    int GetGameID(NineTapDb db, int memberId, int tournyId, int squad);
    List<Game> GetFinalizedGamesByTournament(int tournamentId);
    List<Game> GetFinalizedGamesByMember(int memberNumber, int regionId);
    bool IsGameFinalized(int gameId);
}
