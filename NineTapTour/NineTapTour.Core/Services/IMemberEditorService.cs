using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;

namespace NineTapTour.Core.Services;

/// <summary>
/// Member editor rules extracted from FrmMemberData (UpdateMemberInfo and
/// SaveMemberData): number assignment, handicap from average, senior from date
/// of birth, bonus range, money earned from history, and dues status.
/// </summary>
public interface IMemberEditorService
{
    /// <summary>
    /// Loads the member with the given number for editing. A number of 0 (or one
    /// that does not exist) yields a blank new member; 0 is assigned the next
    /// free number.
    /// </summary>
    MemberEditModel Load(int memberNumber);

    /// <summary>Neighbouring member numbers for record navigation.</summary>
    MemberNavigation GetNavigation(int memberNumber);

    /// <summary>
    /// Validates and saves the member. New members whose number is not already in
    /// use receive the next free number, exactly as the desktop editor does.
    /// </summary>
    MemberSaveResult Save(Member input);

    /// <summary>True when dues are overdue: not a lifetime member and the last payment is more than a year old.</summary>
    bool IsPaymentDue(Member member);
}
