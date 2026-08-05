using NineTapTour.Core.Abstractions;
using System.Windows.Forms;

namespace NineTapTour.Services;

public class WinFormsMessageService : IMessageService
{
    public void ShowInfo(string message, string title = "")
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public void ShowError(string message, string title = "Error")
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public bool Confirm(string message, string title = "")
    {
        return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }
}
