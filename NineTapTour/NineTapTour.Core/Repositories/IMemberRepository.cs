#nullable disable
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using System.Collections.Generic;

namespace NineTapTour.Core.Repositories;

/// <summary>
/// Data access for members. Instance replacement for the old static MemberDB;
/// method names and behavior are unchanged.
/// </summary>
public interface IMemberRepository
{
    void AddOrUpdateMember(Member temp);
    bool MemberExists(Member Temp);
    List<Member> GetMemberList();
    Member GetMember(int memberNumber);
    Member GetMember(int memberNumber, NineTapDb db);
    Member GetMemberByGameId(int gameID);
    int GetMemberIdByNumber(int memberNumber);
    int GetMemberNumberbyID(int memberID);
    int GetLastMemberNumber();

    /// <summary>Members matching the search filters, ordered by number, with Region loaded.</summary>
    List<Member> Search(Models.MemberSearchCriteria criteria);

    /// <summary>Active members who last bowled on or before the date (or never), ordered by number.</summary>
    List<Member> GetInactiveCandidates(System.DateTime lastBowledOnOrBefore);

    /// <summary>Marks the members inactive; returns how many were updated.</summary>
    int SetInactive(IEnumerable<int> memberIds);
}
