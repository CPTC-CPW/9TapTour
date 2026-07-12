using NineTapTour.Abstractions;
using NineTapTour.Database;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using NineTapTour.Models;
using Microsoft.Extensions.DependencyInjection;

namespace NineTapTour.Forms;

public partial class FrmMain : Form
{
    /// <summary>
    /// Keeps track of the currently active menu item on the menu strip.
    /// </summary>
    public ToolStripMenuItem ActiveItem;

    private readonly IServiceProvider _services;

    /// <summary>Designer constructor. Do not use at runtime.</summary>
    public FrmMain()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Opens the Main form. Child forms are created through the DI container.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public FrmMain(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;

        // Set initial size of the application to the maximum size of the screen's working area and maximize the window
        Size = new Size( Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height );
        WindowState = FormWindowState.Maximized;

        // Migrations are applied at startup in Program.Main.
        OpenOrDisplayForm(() => _services.GetRequiredService<FrmMainMenu>());

        //sets the first item of the menu bar to the active item and highlights it.
        ActiveItem = (ToolStripMenuItem)menMain.Items[0];
        ActiveItem.BackColor = SystemColors.ActiveCaption;
    }

    /// <summary>
    /// Opens/displays the form of type <typeparamref name="T"/> as an MDI child, reusing an existing
    /// open instance if there is one; otherwise the DI-provided <paramref name="factory"/> creates it.
    /// </summary>
    public T OpenOrDisplayForm<T>(Func<T> factory) where T : Form
    {
        T form = Application.OpenForms.OfType<T>().FirstOrDefault();
        if (form != null)
        {
            form.BringToFront();
            form.Activate();
        }
        else
        {
            form = factory();
            form.MdiParent = this;
        }
        form.WindowState = FormWindowState.Maximized;
        form.ControlBox = false;
        form.MinimizeBox = false;
        form.MaximizeBox = false;
        form.Show();
        return form;
    }

    /// <summary>
    /// Opens (or activates) the DI-resolved MDI child form of type <typeparamref name="T"/> and
    /// returns the instance. Used by child forms that need to open a sibling without touching DI directly.
    /// </summary>
    public T OpenChild<T>() where T : Form
        => OpenOrDisplayForm(() => _services.GetRequiredService<T>());

    /// <summary>
    /// Shows the current page the user is currently viewing and disables the menu item for that page. 
    /// Highlights the active menu item and unhighlights the previous active menu item.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainMenuToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        ToolStripMenuItem currentItem = (ToolStripMenuItem)e.ClickedItem;
        if (currentItem.HasDropDownItems)
        {
            return;
        }

        // Unhighlight the previous active menu item and enable it
        ActiveItem.Enabled = true;
        ActiveItem?.BackColor = SystemColors.Control;

        // Set the new active menu item and highlight it
        ActiveItem = currentItem;
        ActiveItem.BackColor = SystemColors.ActiveCaption;
        ActiveItem.Enabled = false;
    }

    /// <summary>
    /// Opens the 'About' form
    /// </summary>
    public void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        OpenOrDisplayForm(() => _services.GetRequiredService<FrmAbout>());
    }

    /// <summary>
    /// Opens the 'Main Menu' form when the "Main Menu" menu item is clicked.
    /// </summary>
    public void MainMenuToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (FrmMemberScoresHelpers.unsavedBowlerData)
        {
            DialogResult result = MessageBox.Show("You have unsaved bowler data, are you sure you want to switch screens?", "Unsaved Data", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.No)
            {
                return;
            }
            FrmMemberScoresHelpers.unsavedBowlerData = false;
        }

        OpenOrDisplayForm(() => _services.GetRequiredService<FrmMainMenu>());
    }

    /// <summary>
    /// Opens the 'Member Data' form when the "Member Data" menu item is clicked.
    /// </summary>
    public void MemberDataToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (FrmMemberScoresHelpers.unsavedBowlerData)
        {
            DialogResult result = MessageBox.Show("You have unsaved bowler data, are you sure you want to switch screens?", "Unsaved Data", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.No)
            {
                return;
            }
            FrmMemberScoresHelpers.unsavedBowlerData = false;
        }
        OpenOrDisplayForm(() => _services.GetRequiredService<FrmMemberData>());
    }

    /// <summary>
    /// Opens the 'Member Scores' form when the "Member Scores" menu item is clicked.
    /// </summary>
    public void TournamentToolStripMenuItem_Click(object sender, EventArgs e)
    {
        OpenOrDisplayForm(() => _services.GetRequiredService<FrmMemberScores>());
    }

    private void UpdateInactiveMembersToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        _services.GetRequiredService<FrmUpdateActiveMem>().Show();
    }

    private void BackupDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var admin = _services.GetRequiredService<IDatabaseAdminService>();
        using var saveFileDialog = new SaveFileDialog
        {
            Filter = "Backup file |*.bak",
            DefaultExt = ".bak",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            FileName = admin.CreateBackupName()
        };
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            admin.BackupDatabase(saveFileDialog.FileName);
            MessageBox.Show("Backup successful");
        }
    }

    private void RestoreDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show("Restoring the database will restart the application.", "Warning", MessageBoxButtons.OKCancel) != DialogResult.OK)
        {
            return;
        }

        using var openFileDialog = new OpenFileDialog
        {
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            Filter = "Backup file |*.bak",
            DefaultExt = ".bak"
        };
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            _services.GetRequiredService<IDatabaseAdminService>().RestoreDatabase(openFileDialog.FileName);
            MessageBox.Show("Database successfully restored from backup!");
            Application.Restart();
        }
    }

    private void LabelPrintToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var labelsToPrint = _services.GetRequiredService<FrmLabelPrint>();
        labelsToPrint.ShowDialog();
    }
}
