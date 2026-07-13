using System;
using NineTapTour.Abstractions;
using NineTapTour.Core.Models;
using NineTapTour.Forms; // RegexHelpers
using NineTapTour.Models;

namespace NineTapTour.Services
{
    /// <summary>
    /// Pure implementation of <see cref="IMemberImportService"/>. Extracted verbatim from
    /// FrmMemberData.ProcessExcelFile (SplitName, the member-number cleanup, and the row→Game map).
    /// </summary>
    public sealed class MemberImportService : IMemberImportService
    {
        public (string LastName, string FirstAndMiddle) SplitName(string playerFullName)
        {
            string lastName = "";
            string firstAndMiddle = "";

            if (playerFullName.Contains(','))
            {
                lastName = playerFullName[..playerFullName.IndexOf(',')];
                firstAndMiddle = playerFullName[(playerFullName.IndexOf(',') + 2)..];
            }
            // Checks to see if a period instead of a comma was accidentally placed in member name. (Rob's Request)
            else if (playerFullName.Contains('.'))
            {
                lastName = playerFullName[..playerFullName.IndexOf('.')];
                firstAndMiddle = playerFullName[(playerFullName.IndexOf('.') + 2)..];
            }

            return (lastName, firstAndMiddle);
        }

        public int ParseMemberNumber(string rawNumber)
        {
            string playerNumber = RegexHelpers.StripNonNumericRegex().Replace(rawNumber, string.Empty);

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

            return playerNumberAsInt;
        }

        public Game BuildGameFromRow(ExcelRow row)
        {
            return new Game
            {
                Game1 = row.Game1 >= 0 ? row.Game1 : null,
                Game2 = row.Game2 >= 0 ? row.Game2 : null,
                Game3 = row.Game3 >= 0 ? row.Game3 : null,
                Game4 = row.Game4 >= 0 ? row.Game4 : null,
                Handicap = row.HandyCap >= 0 ? row.HandyCap : null,
                Bonus = row.Bonus >= 0 ? row.Bonus : null,
                MoneyWon = row.Cash > 0 ? Convert.ToDecimal(row.Cash) : null,
                Notes = row.Notes,
                IsFinalized = true, // Mark as finalized since it's legacy data
                AdjustedAvg = row.AVG,
                LeagueAverage = row.TrueAverage,
                UseGame1 = row.Game1 >= 0,
                UseGame2 = row.Game2 >= 0,
                UseGame3 = row.Game3 >= 0,
                UseGame4 = row.Game4 >= 0
            };
        }
    }
}
