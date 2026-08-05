using NineTapTour.Database;
using NineTapTour.Core.Data;
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;


namespace NineTapTour.Forms;

public partial class FrmMainMenu : Form
{
    /// <summary>
    /// Opens the "Main Menu" form.
    /// </summary>
    public FrmMainMenu()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Closes the "Main Menu" form when the "Exit" button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Exit_Click(object sender, EventArgs e)
    {
        this.MdiParent.Close();
    }

    /// <summary>
    /// Opens FrmAbout.cs and highlights the corresponding tab on the menMain
    /// menu strip on FrmMain.cs
    /// Brings up a separate page for the 'About' information when clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAbout_Click(object sender, EventArgs e)
    {
        ((FrmMain)MdiParent).AboutToolStripMenuItem.PerformClick(); // Activate the click method for About
    }

    /// <summary>
    /// Brings up the "Member Data" form when the "Member Data" button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnMemberData_Click(object sender, EventArgs e)
    {
        ((FrmMain)MdiParent).memberToolStripMenuItem.PerformClick();
    }

    /// <summary>
    /// Brings up the "Member Scores" form when the "Member Scores" button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnMemberScores_Click(object sender, EventArgs e)
    {
        ((FrmMain)MdiParent).tournamentToolStripMenuItem.PerformClick();
    }

    private void btnDropDataBase1_Click_1(object sender, EventArgs e)
    {
        if (MessageBox.Show("This will permanently delete and recreate the entire database. All data will be lost. Are you sure?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
        {
            using (NineTapDb db = new())
            {
                db.Database.EnsureDeleted();
                db.Database.Migrate();
            }

            MessageBox.Show("Database was successfully dropped and recreated!");
        }
    }

    private void FrmMainMenu_Load(object sender, EventArgs e)
    {
        Text = "Version: 3.1.11";
#if DEBUG
        Text += " DEVELOPMENT ONLY";
#endif
    }
}