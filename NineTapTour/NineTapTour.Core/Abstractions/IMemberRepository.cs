using System.Collections.Generic;
using NineTapTour.Models;

namespace NineTapTour.Abstractions
{
    /// <summary>Data access for <see cref="Member"/> records. Replaces the static <c>MemberDB</c>.</summary>
    public interface IMemberRepository
    {
        void AddOrUpdateMember(Member member);
        bool MemberExists(Member member);
        List<Member> GetMemberList();
        Member GetMember(int memberNumber);
        Member GetMemberByGameId(int gameId);
        int GetMemberIdByNumber(int memberNumber);
        int GetMemberNumberById(int memberId);
        int GetLastMemberNumber();
        int GetFirstMemberNumber();
    }
}
