#nullable disable
namespace NineTapTour.Core.Import;

/// <summary>
/// Parses bowler names from the header cell of the legacy Excel weekly books.
/// Moved verbatim from FrmMemberData.SplitName; the characterization tests in
/// NineTapTourTests freeze this behavior, quirks included.
/// </summary>
public static class NameParser
{
    /// <summary>
    /// Takes the imported excel row for playerFullName and splits it into playerLastName and firstAndMiddle strings
    /// </summary>
    /// <param name="playerLastName"></param>
    /// <param name="firstAndMiddle"></param>
    /// <param name="playerFullName"></param>
    public static void SplitName(ref string playerLastName, ref string firstAndMiddle, string playerFullName)
    {
        if (playerFullName.Contains(','))
        {
            playerLastName = playerFullName[..playerFullName.IndexOf(',')];
            firstAndMiddle = playerFullName[(playerFullName.IndexOf(',') + 2)..];
        }
        // Checks to see if a period instead of a comma was accidentally placed in member name. (Rob's Request)
        else if (playerFullName.Contains('.'))
        {
            playerLastName = playerFullName[..playerFullName.IndexOf('.')];
            firstAndMiddle = playerFullName[(playerFullName.IndexOf('.') + 2)..];
        }
    }
}
