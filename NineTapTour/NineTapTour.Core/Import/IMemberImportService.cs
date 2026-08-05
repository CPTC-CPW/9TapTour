using NineTapTour.Core.Entities;

namespace NineTapTour.Core.Import;

/// <summary>
/// Imports a single member's legacy game history from one Excel workbook,
/// creating tournaments for each unique date and linking games to the member
/// through participants. This is the flow behind the member data form's
/// "Import Data" button.
/// </summary>
public interface IMemberImportService
{
    /// <summary>
    /// Parses the given workbook and persists the member's game history. When
    /// the member already has player history nothing is imported and the result
    /// reports one skipped record with a warning. On success the member's
    /// average, handicap, bonus, and money earned are updated and saved.
    /// </summary>
    ImportResult ImportMemberHistory(string pathAndFileName, Member member);
}
