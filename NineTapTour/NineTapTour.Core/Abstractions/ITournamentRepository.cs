using System;
using System.Collections.Generic;
using NineTapTour.Models;
using NineTapTour.Models.ViewModels;

namespace NineTapTour.Abstractions
{
    /// <summary>Data access for <see cref="Tournament"/> records. Replaces the static <c>TournamentDB</c>.</summary>
    public interface ITournamentRepository
    {
        void AddTournament(Tournament tourn);
        bool UpdateTournament(Tournament tourn);
        List<Tournament> GetTournamentList();
        List<Participant> GetTournamentMemberList(Tournament tourn);
        List<Participant> GetTournamentMemberListInOrder(Tournament tourn);
        int GetTotalNumberParticipantsInTournament(Tournament tourn);
        List<Member> GetUniqueTourMembers(Tournament tourn);
        List<Member> GetUniqueTourMembersByDate(DateTime start, DateTime end);
        void AddMemberToTournament(Participant player);
        Tournament GetTourneyByID(int tournID);
        List<Member> GetAllActiveMembers();
        void DeleteTournament(Tournament tourn);
        List<WinnerListMemberViewModel> GetWinnerListMemberData(int tournamentId);
    }
}
