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


namespace NineTapTour.Forms
{
    public partial class FrmMainMenu : Form
    {
        private int regionID { get; set; }

        /// <summary>
        /// Opens the "Main Menu" form.
        /// </summary>
        public FrmMainMenu()
        {
            InitializeComponent();

            //check to see if any regions exist, if not create a local region(for first time start up)
            if(NineTapRegionDB.GetNumberOfRegions() == 0)
            {
                NineTapRegion nTemp = new();
                nTemp.NineTapRegionName = "Local";
                NineTapRegionDB.AddRegion(nTemp);
            }
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
            ((FrmMain)MdiParent).RegionID = regionID; // Retrieving ID from menMain
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
            ((FrmMain)MdiParent).RegionID = regionID;
            ((FrmMain)MdiParent).menuHighlight(btnMemberData.Text); //"Member Info"
            ((FrmMain)MdiParent).memberToolStripMenuItem_Click(sender, e);

            enableHomeNavigation();

        }

        private void enableHomeNavigation()
        {
            if (!(FrmMain.ActiveForm is FrmMainMenu))
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
            ((FrmMain)MdiParent).RegionID = regionID;
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
            string name = NineTapRegionDB.GetRegionByID(regionID).NineTapRegionName;
            if (MessageBox.Show($"This button will delete all data stored in the {name} database, are you sure you want to clear  data?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                frmPleaseWait pl = new();
                pl.Show();

                using NineTapDb db = new();

                // Bulk delete Participants for this region (via Member.NineTapRegionID)
                db.Participants.Where(p => p.Member.NineTapRegionID == regionID).ExecuteDelete();

                // Bulk delete Games for this region (via Participant.Member.NineTapRegionID)
                db.Games.Where(g => g.Participant.Member.NineTapRegionID == regionID).ExecuteDelete();

                // Bulk delete Tournaments for this region (via TourneyRegion)
                db.Tournaments.Where(t => t.TourneyRegion.NineTapRegionID == regionID).ExecuteDelete();

                // Bulk delete Members for this region
                db.Members.Where(m => m.NineTapRegionID == regionID).ExecuteDelete();

                // Delete the region itself
                db.NineTapRegion.Where(r => r.NineTapRegionID == regionID).ExecuteDelete();

                if (NineTapRegionDB.GetNumberOfRegions() == 0)
                {
                    NineTapRegion n = new();
                    n.NineTapRegionName = "Local";
                    NineTapRegionDB.AddRegion(n);
                }

                refreshRegionlist();

                pl.Close();
                MessageBox.Show(name + " Database was successfully cleared!");
            }
        }

        private void cbxRegionSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<NineTapRegion> nList = NineTapRegionDB.GetRegionList();
            this.regionID = nList[cbxRegionSelect.SelectedIndex].NineTapRegionID;

            try
            {   // added a try catch block in order to catch the error that occurs at the very first launch of the program.
                //(the MDi parent is not set yet, so it has to skip over this step on its very first launch or the program wont start)
                ((FrmMain)MdiParent).RegionID = regionID;
                ((FrmMain)MdiParent).MembersList = MemberDB.GetMemberList(regionID).OrderBy(m => m.Number);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Returns the currently selected RegionID or -1 if no region is selected
        /// </summary>
        /// <returns></returns>
        public int getRegionID()
        {
            if(cbxRegionSelect.SelectedIndex >= 0)
            {
                List<NineTapRegion> nList = NineTapRegionDB.GetRegionList();
                return nList[cbxRegionSelect.SelectedIndex].NineTapRegionID;
            }
            return -1;
        }

        public void refreshRegionlist()
        {
            List<NineTapRegion> nList = NineTapRegionDB.GetRegionList();
            cbxRegionSelect.DataSource = nList;
            cbxRegionSelect.DisplayMember = "NineTapRegionName";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var region = new FrmAddRegion();
            region.ShowDialog();
            refreshRegionlist();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            //set the global int region so it can be used to filter each region throughout the program
            List<NineTapRegion> nList = NineTapRegionDB.GetRegionList();
            cbxRegionSelect.DataSource = nList;
            cbxRegionSelect.DisplayMember = nameof(NineTapRegion.NineTapRegionName);
            this.regionID = nList[cbxRegionSelect.SelectedIndex].NineTapRegionID;
        }
    }
}