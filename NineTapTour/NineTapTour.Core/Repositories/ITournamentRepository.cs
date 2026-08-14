#nullable disable
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.ViewModels;
using System;
using System.Collections.Generic;

namespace NineTapTour.Core.Repositories;

/// <summary>
/// Data access for tournaments. Instance replacement for the old static TournamentDB;
/// method names and behavior are unchanged.
/// </summary>
public interface ITournamentRepository
{
    void AddTournament(Tournament tourn);
    void AddTournament(Tournament tourn, NineTapDb db);
    bool UpdateTournament(Tournament tourn);
    List<Tournament> GetTournamentList();
    List<Tournament> GetTournamentList(NineTapDb db);
    List<Participant> GetTournamentMemberList(Tournament tourn);
    List<Participant> GetTournamentMemberListInOrder(Tournament tourn);
    int GetTotalNumberParticipantsInTournament(Tournament tourn);
    List<Member> GetUniqueTourMembers(Tournament tourn);
    List<Member> GetUniqueTourMembersByDate(DateTime start, DateTime end);
    void AddMemberToTournament(Participant player);
    void AddMemberToTournament(Participant player, NineTapDb db);
    Tournament GetTourneyByID(int tournID);
    List<Member> GetAllActiveMembers();
    void DeleteTournament(Tournament tourn);
    List<WinnerListMemberViewModel> GetWinnerListMemberData(int tournamentId);
}
