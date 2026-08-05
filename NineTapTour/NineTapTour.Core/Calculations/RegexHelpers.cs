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
}