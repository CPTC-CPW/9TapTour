namespace NineTapTour.Core.Abstractions;

/// <summary>
/// User-facing notifications. The WinForms implementation shows MessageBoxes;
/// tests use a fake; a future website surfaces them its own way.
/// </summary>
public interface IMessageService
{
    void ShowInfo(string message, string title = "");

    void ShowError(string message, string title = "Error");

    /// <summary>Asks a yes/no question; returns true for yes.</summary>
    bool Confirm(string message, string title = "");
}
