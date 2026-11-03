using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineTapTour.Deployment;

/// <summary>
/// A small window that says what's happening while setup work runs before the app's main window opens.
/// </summary>
public static class ProgressWindow
{
    /// <summary>
    /// Shows the window until the work finishes. Returns the exception the work failed with, or null.
    /// </summary>
    public static Exception Run(string title, string message, Func<Task> work)
    {
        Exception failure = null;

        using Form frmProgress = new()
        {
            Text = title,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterScreen,
            MaximizeBox = false,
            MinimizeBox = false,
            ControlBox = false,
            ClientSize = new Size(420, 110),
            BackColor = SystemColors.Control,
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
        };
        Label lblMessage = new()
        {
            Text = message,
            Location = new Point(15, 15),
            Size = new Size(390, 45),
        };
        ProgressBar prgWorking = new()
        {
            Style = ProgressBarStyle.Marquee,
            Location = new Point(15, 70),
            Size = new Size(390, 22),
        };
        frmProgress.Controls.Add(lblMessage);
        frmProgress.Controls.Add(prgWorking);

        frmProgress.Shown += async (sender, e) =>
        {
            try
            {
                await work();
            }
            catch (Exception ex)
            {
                failure = ex;
            }
            frmProgress.Close();
        };

        Application.Run(frmProgress);
        return failure;
    }
}
