#nullable disable
using ClosedXML.Excel;
using NineTapTour.Core.Calculations;
using NineTapTour.Core.Models;

namespace NineTapTour.Core.Import;

/// <summary>
/// Shared ClosedXML parsing for the legacy weekly-book workbooks. The member
/// data form and the member history import tool read the same sheet layout but
/// with subtly different conventions (null-coalescing sentinels vs try/catch
/// sentinels, different player-number fallbacks), so each flow keeps its own
/// clearly named methods instead of merging behavior. Logic is moved verbatim
/// from FrmMemberData.ProcessExcelFile and MemberImportTest.FrmMain.
/// </summary>
public static class ExcelWorkbookReader
{
    /// <summary>
    /// Reads the player header (name, original average, member number) from row 1
    /// of a worksheet. Moved verbatim from FrmMemberData.ProcessExcelFile.
    /// </summary>
    public static ExcelPlayerHeader ReadMemberDataHeader(IXLWorksheet ws)
    {
        string[] playerFinalFirstAndMiddle = ["", ""];
        string playerLastName = "";
        string firstAndMiddle = "";
        string playerFullName = ws.Cell(1, 2).GetString();
        NameParser.SplitName(ref playerLastName, ref firstAndMiddle, playerFullName);
        // Quirk preserved from the form: the split result was never copied into
        // playerFinalFirstAndMiddle, so first/middle names stay empty.
        string[] first0middle1 = firstAndMiddle.Split(' ');
        int playerOrgAVG = ws.Cell(1, 10).GetValue<int?>() ?? -1;
        string playerNumber = ws.Cell(1, 14).GetString();
        playerNumber = RegexHelpers.StripNonNumericRegex().Replace(playerNumber, string.Empty);

        int.TryParse(playerNumber, out int playerNumberAsInt);
        if (playerNumberAsInt != 0)
        {
            playerNumberAsInt = Convert.ToInt32(RegexHelpers.StripNonNumericRegex().Replace(playerNumber, string.Empty));
        }
        else if (playerNumberAsInt == 0)
        {
            string[] playerNumberAfterSplit = playerNumber.Split('/');
            playerNumberAsInt = Convert.ToInt32(RegexHelpers.StripNonNumericRegex().Replace(playerNumberAfterSplit[^1], string.Empty));
        }

        return new ExcelPlayerHeader
        {
            FirstName = playerFinalFirstAndMiddle[0],
            MiddleName = playerFinalFirstAndMiddle.Length > 1 ? playerFinalFirstAndMiddle[1] : "",
            LastName = playerLastName,
            OriginalAverage = playerOrgAVG,
            PlayerNumber = playerNumberAsInt
        };
    }

    /// <summary>
    /// Reads one game row using the member data form's conventions: missing
    /// numeric cells become -1 (-1000 for handicap, 0 for cash) via null
    /// coalescing, and the date cell must contain a valid date.
    /// Moved verbatim from FrmMemberData.ProcessExcelFile.
    /// </summary>
    public static ExcelRow ReadMemberDataRow(IXLWorksheet ws, int row, ExcelPlayerHeader player)
    {
        return new ExcelRow
        {
            PlayerFirstName = player.FirstName,
            PlayerMiddleName = player.MiddleName,
            PlayerLastName = player.LastName,
            PlayerOrginalAVG = player.OriginalAverage,
            PlayerNumber = player.PlayerNumber,
            GameTotal = ws.Cell(row, 1).GetValue<int?>() ?? -1,
            Date = ws.Cell(row, 2).GetDateTime(),
            Game1 = ws.Cell(row, 3).GetValue<int?>() ?? -1,
            Game2 = ws.Cell(row, 4).GetValue<int?>() ?? -1,
            Game3 = ws.Cell(row, 5).GetValue<int?>() ?? -1,
            Game4 = ws.Cell(row, 6).GetValue<int?>() ?? -1,
            Total = ws.Cell(row, 7).GetValue<int?>() ?? -1,
            AverageOfRow = ws.Cell(row, 8).GetValue<double?>() ?? -1,
            TrueAverage = ws.Cell(row, 9).GetValue<double?>() ?? -1,
            AVG = ws.Cell(row, 10).GetValue<int?>() ?? -1,
            HandyCap = ws.Cell(row, 11).GetValue<int?>() ?? -1000,
            Bonus = ws.Cell(row, 12).GetValue<int?>() ?? -1,
            FinPPHG = ws.Cell(row, 14).GetString(),
            Cash = ws.Cell(row, 15).GetValue<double?>() ?? 0,
            Notes = ws.Cell(row, 16).GetString()
        };
    }

    /// <summary>
    /// Extracts player information (name, number, average) from a worksheet using
    /// the history import tool's conventions (period-separator recovery, average
    /// string fallback, multiple player-number splitters). Assignments happen
    /// through the ref parameters exactly as the original did, so a partial
    /// extraction from one worksheet carries into the next attempt.
    /// Moved verbatim from MemberImportTest.FrmMain.ExtractPlayerInfoFromWorksheet.
    /// </summary>
    public static void ExtractHistoryPlayerInfo(IXLWorksheet ws, ref string[] playerFinalFirstAndMiddle,
        ref string playerLastName, ref int playerOrgAVG, ref int playerNumberAsInt, char[] splitters)
    {
        string firstAndMiddle = "";

        // Parse header for player name
        string playerFullName = ws.Cell(1, 2).GetString();
        if (!string.IsNullOrWhiteSpace(playerFullName))
        {
            if (playerFullName.Contains(','))
            {
                playerLastName = playerFullName[..playerFullName.IndexOf(',')];
                firstAndMiddle = playerFullName[(playerFullName.IndexOf(',') + 2)..];
            }
            else if (playerFullName.Contains('.'))
            {
                playerLastName = playerFullName[..playerFullName.IndexOf('.')];
                try
                {
                    firstAndMiddle = playerFullName[(playerFullName.IndexOf('.') + 2)..];
                }
                catch (ArgumentOutOfRangeException)
                {
                    int firstSpaceIndex = playerFullName.IndexOf(' ');
                    firstAndMiddle = firstSpaceIndex > -1 ? playerFullName[..firstSpaceIndex] : playerFullName;
                }
            }
        }

        string[] first0middle1 = firstAndMiddle.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < Math.Min(first0middle1.Length, playerFinalFirstAndMiddle.Length); i++)
        {
            playerFinalFirstAndMiddle[i] = first0middle1[i];
        }

        try
        {
            playerOrgAVG = ws.Cell(1, 10).GetValue<int>();
        }
        catch (Exception)
        {
            string orgString = ws.Cell(1, 10).GetString();
            string[] afterSplit = orgString.Split('-', '*', 'L');
            if (afterSplit.Length > 0 && int.TryParse(afterSplit[0], out int val))
                playerOrgAVG = val;
            else
                playerOrgAVG = -1;
        }

        string playerNumber = ws.Cell(1, 14).GetString();
        if (playerNumber == null)
        {
            playerNumberAsInt = 0;
            return;
        }

        playerNumber = RegexHelpers.StripNonNumericRegex().Replace(playerNumber, string.Empty);

        string[] playerNumberAfterSplit;
        int.TryParse(playerNumber, out playerNumberAsInt);
        if (playerNumberAsInt != 0)
        {
            playerNumberAsInt = Convert.ToInt32(RegexHelpers.StripNonNumericRegex().Replace(playerNumber, string.Empty));
        }
        else if (playerNumberAsInt == 0)
        {
            for (int i = 0; i < splitters.Length; i++)
            {
                try
                {
                    playerNumberAfterSplit = playerNumber.Split(splitters[i]);
                    playerNumberAsInt = Convert.ToInt32(RegexHelpers.StripNonNumericRegex().Replace(playerNumberAfterSplit[^1], string.Empty));
                }
                catch { }
            }
        }
    }

    /// <summary>
    /// Reads one game row using the history import tool's conventions: failed
    /// cell reads fall back to sentinels via try/catch, and the cash column is
    /// only read when the finalized-progressive-pot column has a value.
    /// Returns null when the row has no game data (GameTotal sentinel), matching
    /// the original skip-before-reading-the-remaining-columns behavior.
    /// Moved verbatim from MemberImportTest.FrmMain.ProcessExcelFile.
    /// </summary>
    public static ExcelRow ReadHistoryRow(IXLWorksheet ws, int row, string[] playerFinalFirstAndMiddle,
        string playerLastName, int playerOrgAVG, int playerNumberAsInt)
    {
        ExcelRow temp = new();

        // Populate excel row with reused player data
        temp.PlayerFirstName = playerFinalFirstAndMiddle[0];
        temp.PlayerMiddleName = playerFinalFirstAndMiddle[1];
        temp.PlayerLastName = playerLastName;
        temp.PlayerOrginalAVG = playerOrgAVG;
        temp.PlayerNumber = playerNumberAsInt;

        try { temp.GameTotal = ws.Cell(row, 1).GetValue<int>(); } catch { temp.GameTotal = -1; }
        try { temp.Date = ws.Cell(row, 2).GetDateTime(); } catch { temp.Date = new DateTime(); }
        try { temp.Game1 = ws.Cell(row, 3).GetValue<int>(); } catch { temp.Game1 = -1; }
        try { temp.Game2 = ws.Cell(row, 4).GetValue<int>(); } catch { temp.Game2 = -1; }
        try { temp.Game3 = ws.Cell(row, 5).GetValue<int>(); } catch { temp.Game3 = -1; }
        try { temp.Game4 = ws.Cell(row, 6).GetValue<int>(); } catch { temp.Game4 = -1; }
        try { temp.Total = ws.Cell(row, 7).GetValue<int>(); } catch { temp.Total = -1; }

        if (temp.GameTotal == -1)
        {
            // No game data in this row; skip it
            return null;
        }

        try { temp.AverageOfRow = ws.Cell(row, 8).GetValue<double>(); } catch { temp.AverageOfRow = -1; }
        try { temp.TrueAverage = ws.Cell(row, 9).GetValue<double>(); } catch { temp.TrueAverage = -1; }
        try { temp.AVG = ws.Cell(row, 10).GetValue<int>(); } catch { temp.AVG = -1; }
        try { temp.HandyCap = ws.Cell(row, 11).GetValue<int>(); } catch { temp.HandyCap = -1; }
        try { temp.Bonus = ws.Cell(row, 12).GetValue<int>(); } catch { temp.Bonus = -1; }
        temp.FinPPHG = ws.Cell(row, 14).GetString();
        try { if (!string.IsNullOrEmpty(temp.FinPPHG)) { temp.Cash = ws.Cell(row, 15).GetValue<double>(); } else { temp.Cash = 0; } } catch { temp.Cash = 0; }
        temp.Notes = ws.Cell(row, 16).GetString();

        return temp;
    }
}
