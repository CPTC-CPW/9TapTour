namespace NineTapTour.Core.Abstractions;

/// <summary>
/// File and folder picking. Returns the chosen path, or null when the user
/// cancels. Keeps services headless so they can be tested and reused.
/// </summary>
public interface IFileDialogService
{
    string? PickSaveFile(string filter, string defaultExt, string suggestedName, string? initialDirectory = null);

    string? PickOpenFile(string filter, string defaultExt, string? initialDirectory = null);

    string? PickFolder();
}
