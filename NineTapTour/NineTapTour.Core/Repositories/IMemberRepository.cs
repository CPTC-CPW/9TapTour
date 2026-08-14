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
}
