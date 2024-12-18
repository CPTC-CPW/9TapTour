using NineTapTour.Models;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NineTapTour.Models;
public static class FrmMemberScoresHelpers
{

#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static Tournament selectedTournament;
    public static List<Participant> overallListOfParticipants;
    public static bool unsavedBowlerData = false;
#pragma warning restore CA2211 // Non-constant fields should not be visible

    /// <summary>
    /// Checks a string for numeric values
    /// true if all are numeric
    /// </summary>
    /// <param name="str"></param>
    /// <returns>isNum</returns>
    public static bool IsNumeric(string str)
    {
        bool isNum = int.TryParse(str, out _);
        return isNum;
    }

    /// <summary>
    /// check for empty text box
    /// </summary>
    /// <param name="box"></param>
    /// <returns></returns>
    public static bool IsEmpty(TextBox box)
    {
        if (string.IsNullOrEmpty(box.Text.Trim()))
        {
            return true;
        }
        return false;
    }
}