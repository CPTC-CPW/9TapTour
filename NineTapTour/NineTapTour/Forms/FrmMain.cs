using NineTapTour.Database;
using System;
using System.Windows.Forms;
using System.Drawing;
using NineTapTour.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NineTapTour.Forms;

public partial class FrmMain : Form
{
    /// <summary>
    /// Keeps track of the currently active menu item on the menu strip.
    /// </summary>
    public ToolStripMenuItem ActiveItem;
    
    /// <summary>
    /// Opens Main form 
    /// Retrieves information from the database in order.
    /// </summary>
    public FrmMain()
    {
        InitializeComponent();

        // Set initial size of the application to the maximum size of the screen's working area and maximize the window
        Size = new Size( Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height );
        WindowState = FormWindowState.Maximized;

        // Run migrations on startup
        var migrator = new NineTapDb().Database.GetService<IMigrator>();
        migrator.Migrate();
        
        var mainMenu = Application.OpenForms["MainMenu"] as FrmMainMenu;
        OpenOrDisplayForm(ref mainMenu);

        //sets the first item of the menu bar to the active item and highlights it.
        ActiveItem = (ToolStripMenuItem)menMain.Items[0];
        ActiveItem.BackColor = SystemColors.ActiveCaption;
    }

    /// <summary>
    /// Opens/Displays the specified form. Ensures the form is on top when selected.
    /// </summary>
    /// <typeparam name="T">forms that have already been opened(?)</typeparam>
    /// <param name="form">forms that haven't been opened yet(?)</param>
    public void OpenOrDisplayForm<T>(ref T form) where T : Form, new()
    {
        if (form != null)
        {   
            form.BringToFront();
            form.Activate();   
        }
        else
        {
            form = new T
            {
                MdiParent = this,
            };
        }
        form.WindowState = FormWindowState.Maximized;
        form.ControlBox = false;
        form.MinimizeBox = false;
        form.MaximizeBox = false;
        form.Show();
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
        var aboutForm = Application.OpenForms["FrmAbout"] as FrmAbout;
        OpenOrDisplayForm(ref aboutForm);
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

        var mainMenu = Application.OpenForms["MainMenu"] as FrmMainMenu;

        OpenOrDisplayForm(ref mainMenu);
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
        var newfrmMemberData = Application.OpenForms["FrmMemberData"] as FrmMemberData;
        OpenOrDisplayForm(ref newfrmMemberData);
    }

    /// <summary>
    /// Opens the 'Member Scores' form when the "Member Scores" menu item is clicked.
    /// </summary>
    public void TournamentToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var newfrmMemberScores = Application.OpenForms["frmMemberScores"] as FrmMemberScores;
        OpenOrDisplayForm(ref newfrmMemberScores);     
    }

    private void UpdateInactiveMembersToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        var UpdatefrmActiveMem = new FrmUpdateActiveMem();          
        UpdatefrmActiveMem.Show();
    }

    private void BackupDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        DatabaseManagement.BackupDatabase();
    }

    private void RestoreDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show("Restoring the database will restart the application.", "Warning", MessageBoxButtons.OKCancel) == DialogResult.OK)
        {
            if (DatabaseManagement.RestoreDatabase())
            {
                MessageBox.Show("Database successfully restored from backup!");
                Application.Restart();
            }
        }
    }
    
    private void LabelPrintToolStripMenuItem_Click(object sender, EventArgs e)
    {
        FrmLabelPrint labelsToPrint = new();
        labelsToPrint.ShowDialog();
    }
}
