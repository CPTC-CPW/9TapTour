using NineTapTour.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Database;
using NineTapTour.Services;
using System;
using System.Windows.Forms;
using System.Drawing;
using NineTapTour.Core.Data;
using NineTapTour.Core.Services;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;

namespace NineTapTour.Forms;

public partial class FrmMain : Form
{
    private readonly IFormNavigator navigator;
    private readonly IFormFactory formFactory;
    private readonly IDbContextFactory<NineTapDb> dbFactory;
    private readonly ITournamentSession session;
    private readonly IDatabaseMaintenanceService databaseMaintenance;
    private readonly IFileDialogService fileDialogService;
    private readonly IMessageService messageService;

    /// <summary>
    /// Keeps track of the currently active menu item on the menu strip.
    /// </summary>
    public ToolStripMenuItem ActiveItem;

    /// <summary>
    /// Opens Main form
    /// Retrieves information from the database in order.
    /// </summary>
    public FrmMain(IFormNavigator navigator, IFormFactory formFactory, IDbContextFactory<NineTapDb> dbFactory, ITournamentSession session,
        IDatabaseMaintenanceService databaseMaintenance, IFileDialogService fileDialogService, IMessageService messageService)
    {
        this.navigator = navigator;
        this.session = session;
        this.formFactory = formFactory;
        this.dbFactory = dbFactory;
        this.databaseMaintenance = databaseMaintenance;
        this.fileDialogService = fileDialogService;
        this.messageService = messageService;

        InitializeComponent();

        // Set initial size of the application to the maximum size of the screen's working area and maximize the window
        Size = new Size( Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height );
        WindowState = FormWindowState.Maximized;

        navigator.RegisterMdiParent(this);

        //sets the first item of the menu bar to the active item and highlights it.
        ActiveItem = (ToolStripMenuItem)menMain.Items[0];
        ActiveItem.BackColor = SystemColors.ActiveCaption;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        navigator.ShowSingleton<FrmMainMenu>();
    }

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
        navigator.ShowSingleton<FrmAbout>();
    }

    /// <summary>
    /// Opens the 'Main Menu' form when the "Main Menu" menu item is clicked.
    /// </summary>
    public void MainMenuToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (session.HasUnsavedBowlerData)
        {
            DialogResult result = MessageBox.Show("You have unsaved bowler data, are you sure you want to switch screens?", "Unsaved Data", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.No)
            {
                return;
            }
            session.HasUnsavedBowlerData = false;
        }

        navigator.ShowSingleton<FrmMainMenu>();
    }

    /// <summary>
    /// Opens the 'Member Data' form when the "Member Data" menu item is clicked.
    /// </summary>
    public void MemberDataToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (session.HasUnsavedBowlerData)
        {
            DialogResult result = MessageBox.Show("You have unsaved bowler data, are you sure you want to switch screens?", "Unsaved Data", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.No)
            {
                return;
            }
            session.HasUnsavedBowlerData = false;
        }
        navigator.ShowSingleton<FrmMemberData>();
    }

    /// <summary>
    /// Opens the 'Member Scores' form when the "Member Scores" menu item is clicked.
    /// </summary>
    public void TournamentToolStripMenuItem_Click(object sender, EventArgs e)
    {
        navigator.ShowSingleton<FrmMemberScores>();
    }

    /// <summary>
    /// Opens the 'Reports' form when the "Reports" menu item is clicked.
    /// </summary>
    public void ReportsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (session.HasUnsavedBowlerData)
        {
            DialogResult result = MessageBox.Show("You have unsaved bowler data, are you sure you want to switch screens?", "Unsaved Data", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.No)
            {
                return;
            }
            session.HasUnsavedBowlerData = false;
        }

        navigator.ShowSingleton<FrmReports>();
    }

    private void UpdateInactiveMembersToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        var updateFrmActiveMem = formFactory.Create<FrmUpdateActiveMem>();
        updateFrmActiveMem.Show();
    }

    private void BackupDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        string backupPath = fileDialogService.PickSaveFile(
            "Backup file |*.bak", ".bak", databaseMaintenance.CreateBackupName(),
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        if (backupPath == null)
        {
            return;
        }

        databaseMaintenance.BackupTo(backupPath);
        messageService.ShowInfo("Backup successful");
    }

    private void RestoreDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show("Restoring the database will restart the application.", "Warning", MessageBoxButtons.OKCancel) == DialogResult.OK)
        {
            string backupPath = fileDialogService.PickOpenFile(
                "Backup file |*.bak", ".bak",
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            if (backupPath == null)
            {
                return;
            }

            databaseMaintenance.RestoreFrom(backupPath);
            messageService.ShowInfo("Database successfully restored from backup!");
            Application.Restart();
        }
    }
    
    private void LabelPrintToolStripMenuItem_Click(object sender, EventArgs e)
    {
        FrmLabelPrint labelsToPrint = formFactory.Create<FrmLabelPrint>();
        labelsToPrint.ShowDialog();
    }
}
