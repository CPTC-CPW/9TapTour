namespace NineTapTour.Core.Import;

/// <summary>
/// Bulk legacy import used by the member import tool: scans Excel workbooks
/// (one per member), creating tournaments, games, and participants for every
/// game row. Progress text is streamed through the supplied IProgress.
/// </summary>
public interface IMemberHistoryImportService
{
    /// <summary>
    /// Imports every Excel file found directly in the given folder.
    /// </summary>
    ImportResult ImportFolder(string folderPath, IProgress<string> progress);

    /// <summary>
    /// Imports the given files, skipping any that are not Excel workbooks.
    /// </summary>
    ImportResult ImportFiles(string[] files, IProgress<string> progress);
}
