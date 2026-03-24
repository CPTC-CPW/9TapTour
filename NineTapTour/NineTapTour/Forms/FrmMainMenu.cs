using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NineTapTour.Models;
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
        ((FrmMain)MdiParent).menuHighlight(btnAbout.Text); // Highlighting corresponding tab; "About"
        ((FrmMain)MdiParent).AboutToolStripMenuItem_Click(sender, e); // Activate the click method for About
        enableHomeNavigation();
    }

    /// <summary>
    /// Brings up the "Member Data" form when the "Member Data" button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnMemberData_Click(object sender, EventArgs e)
    {
        ((FrmMain)MdiParent).menuHighlight(btnMemberData.Text); //"Member Info"
        ((FrmMain)MdiParent).memberToolStripMenuItem_Click(sender, e);

        enableHomeNavigation();
    }

    private void enableHomeNavigation()
    {
        if (FrmMain.ActiveForm is not FrmMainMenu)
        {
            ((FrmMain)MdiParent).Home.Enabled = true;
        }
    }

    /// <summary>
    /// Brings up the "Member Scores" form when the "Member Scores" button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnMemberScores_Click(object sender, EventArgs e)
    {
        ((FrmMain)MdiParent).menuHighlight(btnMemberScores.Text); // "Member Scores"
        ((FrmMain)MdiParent).tournamentToolStripMenuItem_Click(sender, e);
        enableHomeNavigation();
    }

    private void MainMenu_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        Font drawFont = new("Arial", 12);
        SolidBrush drawBrush = new(Color.White);
        PointF drawPoint = new(10, 2);
        g.DrawString("Version: 3.0.3", drawFont, drawBrush, drawPoint);
#if DEBUG
        drawBrush.Color = Color.Red;
        drawPoint.Y += 16;
        g.DrawString("DEVELOPMENT VERSION NOT FOR PRODUCTION", drawFont, drawBrush, drawPoint);
#endif
    }

    // This is the code behind for the delete database button. Per Rob, we don't need this 
    // at this time. Keeping the code incase it's needed in the future.
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
}