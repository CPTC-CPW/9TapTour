using NineTapTour.Core.Abstractions;
using System.Windows.Forms;

namespace NineTapTour.Services;

public class WinFormsFileDialogService : IFileDialogService
{
    public string PickSaveFile(string filter, string defaultExt, string suggestedName, string initialDirectory = null)
    {
        using SaveFileDialog dialog = new()
        {
            Filter = filter,
            DefaultExt = defaultExt,
            FileName = suggestedName,
        };
        if (initialDirectory != null)
        {
            dialog.InitialDirectory = initialDirectory;
        }
        return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
    }

    public string PickOpenFile(string filter, string defaultExt, string initialDirectory = null)
    {
        using OpenFileDialog dialog = new()
        {
            Filter = filter,
            DefaultExt = defaultExt,
        };
        if (initialDirectory != null)
        {
            dialog.InitialDirectory = initialDirectory;
        }
        return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
    }

    public string PickFolder()
    {
        using FolderBrowserDialog dialog = new();
        return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
    }
}
