using NineTapTour.Core.Models;
using NineTapTour.Models;

namespace NineTapTour.Abstractions
{
    /// <summary>
    /// Pure parsing/mapping helpers for the legacy member-history Excel import. Extracted from
    /// FrmMemberData.ProcessExcelFile so the fiddly name/number parsing and row→Game mapping can be
    /// unit-tested without a workbook or a database. The form keeps the ClosedXML cell reads and the
    /// tournament/participant persistence.
    /// </summary>
    public interface IMemberImportService
    {
        /// <summary>
        /// Splits a "Last, First Middle" (or accidental "Last. First Middle") full name into its
        /// last-name and first-and-middle parts. Returns empty strings when no separator is present.
        /// </summary>
        (string LastName, string FirstAndMiddle) SplitName(string playerFullName);

        /// <summary>
        /// Cleans a raw member-number cell (strips non-numeric characters; if the result is 0, falls
        /// back to the numeric tail after the last '/').
        /// </summary>
        int ParseMemberNumber(string rawNumber);

        /// <summary>
        /// Maps a parsed import row to a finalized legacy <see cref="Game"/> (negative sentinels →
        /// null; UseGameN mirrors whether each game was present).
        /// </summary>
        Game BuildGameFromRow(ExcelRow row);
    }
}
