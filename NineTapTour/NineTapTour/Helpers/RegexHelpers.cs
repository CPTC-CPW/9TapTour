using System.Text.RegularExpressions;

namespace NineTapTour.Forms;

public static partial class RegexHelpers
{

    [GeneratedRegex(@"\d+")]
    public static partial Regex GetDigitsRegex();

    [GeneratedRegex("[^0-9]")]
    public static partial Regex StripNonNumericRegex();
}