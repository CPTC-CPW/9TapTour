using NineTapTour.Core.Entities;

namespace NineTapTour.Core.Models;

/// <summary>
/// Everything the member editor screen shows for one member: the record (a
/// blank one with the next number when <see cref="IsNew"/>), the derived
/// 30-game average, the year dues are paid through, and whether payment is due.
/// </summary>
public sealed class MemberEditModel
{
    public required Member Member { get; init; }

    public bool IsNew { get; init; }

    /// <summary>Rolling 30-game true average from the most recent finalized entry, 0 when none.</summary>
    public int ThirtyGameAverage { get; init; }

    /// <summary>Year the membership is paid through, or empty for lifetime members and members with no payment.</summary>
    public string PaidToYear { get; init; } = string.Empty;

    /// <summary>True when the last payment is more than a year old and the member is not a lifetime member.</summary>
    public bool IsPaymentDue { get; init; }
}

/// <summary>
/// First/previous/next/last member numbers around the current one, for the
/// record-navigation buttons. Null when there is no such neighbour.
/// </summary>
public sealed record MemberNavigation(int? First, int? Previous, int? Next, int? Last, int Position, int Count);

/// <summary>Outcome of saving a member: the errors to show, or the saved record.</summary>
public sealed record MemberSaveResult(bool Success, IReadOnlyList<string> Errors, Member? Saved)
{
    public static MemberSaveResult Ok(Member saved)
    {
        return new MemberSaveResult(true, [], saved);
    }

    public static MemberSaveResult Failed(IReadOnlyList<string> errors)
    {
        return new MemberSaveResult(false, errors, null);
    }
}
