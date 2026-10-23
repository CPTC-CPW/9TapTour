using System;

namespace NineTapTour.Core.Calculations;

/// <summary>
/// 9-Tap membership dues rules. A membership is paid THRU the end of a calendar
/// year: a payment made January 1 through September 30 covers the year it was
/// made in, and a payment made October 1 through December 31 covers the
/// following year.
/// </summary>
public static class MembershipDues
{
    /// <summary>
    /// Payments made in this month or later count toward the next year.
    /// </summary>
    public const int NextYearCutoffMonth = 10;

    /// <summary>
    /// Text shown in place of a paid-thru year for lifetime members.
    /// </summary>
    public const string LifetimeText = "life ";

    /// <summary>
    /// Returns the year a payment made on <paramref name="lastPayment"/> keeps the
    /// membership valid through (until December 31 of that year).
    /// </summary>
    public static int PaidThroughYear(DateTime lastPayment) =>
        lastPayment.Month >= NextYearCutoffMonth ? lastPayment.Year + 1 : lastPayment.Year;

    /// <summary>
    /// Returns the paid-thru year shown on reports: <see cref="LifetimeText"/> for
    /// lifetime members, an empty string when no payment is on file, otherwise
    /// the four-digit year.
    /// </summary>
    public static string FormatPaidThroughYear(bool isLifetimeMember, DateTime? lastPayment)
    {
        if (isLifetimeMember)
            return LifetimeText;
        return lastPayment.HasValue ? PaidThroughYear(lastPayment.Value).ToString() : string.Empty;
    }

    /// <summary>
    /// Returns true when the member is a lifetime member or their last payment
    /// keeps them paid through the year of <paramref name="today"/>.
    /// </summary>
    public static bool IsCurrent(bool isLifetimeMember, DateTime? lastPayment, DateTime today) =>
        isLifetimeMember || (lastPayment.HasValue && PaidThroughYear(lastPayment.Value) >= today.Year);
}
