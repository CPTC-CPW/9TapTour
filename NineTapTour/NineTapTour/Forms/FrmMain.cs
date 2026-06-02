using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;
using NineTapTour.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.ComponentModel;

namespace NineTapTour.Forms
{
    public partial class FrmMain : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IOrderedEnumerable<Member> MembersList { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Tournament> TournamentList { get; set; }
        public System.Windows.Forms.ToolStripMenuItem ActiveItem;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FrmMemberData CurrFrmMemberData { get; set; }

        private FrmMainMenu MainMenu { get; set; }

        private Size MaxWorkAreaScreenSize { get; set; }
        
        /// <summary>
        /// Opens Main form 
        /// Retrieves information from the database in order.
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();

            //this size is the height and width of the primary screen minus the start bar (if the user has a start bar)
            MaxWorkAreaScreenSize = new Size( Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height );

            // Run migrations on startup
            var migrator = new NineTapDb().Database.GetService<IMigrator>();
            migrator.Migrate();

            MembersList = MemberDB.GetMemberList().OrderBy(m => m.Number);
            TournamentList = TournamentDB.GetTournamentList();

            //sets the height and width of the parent form... this can not be resized later... all child forms must 
            //fit in its bounds... the only exception is using a scrollbar on the side or bottom...
            SetHeightAndWidth(MaxWorkAreaScreenSize);
            
            //on start up make sure regionID is set 
            var mainMenu = Application.OpenForms["MainMenu"] as FrmMainMenu;
            OpenOrDisplayForm(ref mainMenu);
            MainMenu = mainMenu;

            //sets the first item of the menu bar to the active item and highlights it.
            ActiveItem = (System.Windows.Forms.ToolStripMenuItem)menMain.Items[0];
            ActiveItem.BackColor = SystemColors.ActiveCaption;
        }

        /// <summary>
        /// This methoud takes in a Size (width and height) and will set the application to that size
        /// this size should be set to the working area of the primary monitor...
        /// </summary>
        /// <param name="workingArea">The working array of the primary monitor</param>
        private void SetHeightAndWidth(Size workingArea)
        {
            this.Size = workingArea;
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
                    Dock = DockStyle.Fill                      
               
                };
            }
            form.WindowState = FormWindowState.Maximized;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutForm = Application.OpenForms["FrmAbout"] as FrmAbout;
            OpenOrDisplayForm(ref aboutForm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void mainMenuToolStripMenuItem_Click(object sender, EventArgs e)
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void memberToolStripMenuItem_Click(object sender, EventArgs e)
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
            CurrFrmMemberData = newfrmMemberData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void tournamentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newfrmMemberScores = Application.OpenForms["frmMemberScores"] as FrmMemberScores;
            OpenOrDisplayForm(ref newfrmMemberScores);     
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menMain_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            var s = e.Item.GetType().ToString();
            if (s == "System.Windows.Forms.MdiControlStrip+ControlBoxMenuItem")
            {
                e.Item.Visible = false;
            }

            if (e.Item.Text == "")
            {
                e.Item.Visible = false;
            }
        }

        private void updateInactiveMembersToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //update the regionID
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
        
        private void labelPrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmLabelPrint labelsToPrint = new();
            labelsToPrint.ShowDialog();
        }
    }
}
