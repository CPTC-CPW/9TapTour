#nullable disable
using System.Text.RegularExpressions;

namespace NineTapTour.Core.Calculations;

public static partial class RegexHelpers
{

    [GeneratedRegex(@"\d+")]
    public static partial Regex GetDigitsRegex();

    [GeneratedRegex("[^0-9]")]
    public static partial Regex StripNonNumericRegex();

    [GeneratedRegex(@"^(\d+)(st|nd|rd|th)?\s*-\s*(\d+)(st|nd|rd|th)?$", RegexOptions.IgnoreCase, "en-US")]
    public static partial Regex PlacingRange();

    [GeneratedRegex(@"^(\d+)(st|nd|rd|th|T)?$", RegexOptions.IgnoreCase, "en-US")]
    public static partial Regex SinglePlacing();

    /// <summary>
    /// Matches the leading digits of a legacy place-standing cell, which has many
    /// variations ("4th", "17th tie", "9thHM", ...). Group 1 is the numeric place.
    /// </summary>
    [GeneratedRegex(@"^\s*(\d+)")]
    public static partial Regex LeadingDigits();

    /// <summary>
    /// Matches the formula that totals a legacy book page's CASH column, such
    /// as "SUM(O3:O46)" (with or without a leading "=" or "$" anchors removed).
    /// Group 1 is the last row of the summed range.
    /// </summary>
    [GeneratedRegex(@"^=?SUM\(O\d+:O(\d+)\)$", RegexOptions.IgnoreCase, "en-US")]
    public static partial Regex CashTotalFormula();

    /// <summary>
    /// Matches a month/day/year date written in a legacy book's notes, such as
    /// "House Shot 10/4/14" or "Nob Hill 7-7-12". Groups 1-3 are month, day, year.
    /// </summary>
    [GeneratedRegex(@"(?<!\d)(\d{1,2})[/-](\d{1,2})[/-](\d{4}|\d{2})(?!\d)")]
    public static partial Regex NotesDate();
}