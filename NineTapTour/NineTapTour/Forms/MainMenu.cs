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


namespace NineTapTour.Forms
{
    public partial class MainMenu : Form
    {
        public FrmMain CurrentMainFrm { get; set; }
        public int RegionID { get; set; }

        #region MainMenu
        public MainMenu()
        {
            InitializeComponent();

            //check to see if any regions exist, if not create a local region(for first time start up)
            if(NineTapRegionDB.GetNumberOfRegions() == 0)
            {
                NineTapRegion nTemp = new NineTapRegion();
                nTemp.NineTapRegionName = "Local";
                NineTapRegionDB.AddRegion(nTemp);
            }
        }

        private void MainMenu_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Font drawFont = new Font("Arial", 12);
            SolidBrush drawBrush = new SolidBrush(Color.White);
            PointF drawPoint = new PointF(10, 2);
            g.DrawString("Version: 1.8.6", drawFont, drawBrush, drawPoint);
#if DEBUG
            drawBrush.Color = Color.Red;
            drawPoint.Y += 16;
            g.DrawString("DEVELOPMENT VERSION NOT FOR PRODUCTION", drawFont, drawBrush, drawPoint);
#endif
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            //set the global int region so it can be used to filter each region throughout the program
            List<NineTapRegion> nList = NineTapRegionDB.GetRegionList();
            cbxRegionSelect.DataSource = nList;
            cbxRegionSelect.DisplayMember = nameof(NineTapRegion.NineTapRegionName);
            this.RegionID = nList[cbxRegionSelect.SelectedIndex].NineTapRegionID;
        }
        #endregion

        #region Buttons
        private void Exit_Click(object sender, EventArgs e)
        {
            MdiParent.Close();
        }

        /// <summary>
        /// Opens FrmAbout.cs and highlights the corresponding tab on the menMain
        /// menu strip on FrmMain.cs
        /// Brings up a separate page for the 'About' information when clicked
        /// </summary>
        private void btnAbout_Click(object sender, EventArgs e)
        {
            ((FrmMain)MdiParent).RegionID = RegionID; // Retrieving ID from menMain
            ((FrmMain)MdiParent).MenuHighlight(btnAbout.Text); // Highlighting corresponding tab; "About"
            ((FrmMain)MdiParent).AboutToolStripMenuItem_Click(sender, e); // Activate the click method for About
            EnableHomeNavigation();

        }

        private void btnMemberData_Click(object sender, EventArgs e)
        {
            ((FrmMain)MdiParent).RegionID = RegionID;
            ((FrmMain)MdiParent).MenuHighlight(btnMemberData.Text); //"Member Info"
            ((FrmMain)MdiParent).memberToolStripMenuItem_Click(sender, e);

            EnableHomeNavigation();

        }

        private void btnMemberScores_Click(object sender, EventArgs e)
        {
            ((FrmMain)MdiParent).RegionID = RegionID;
            ((FrmMain)MdiParent).MenuHighlight(btnMemberScores.Text); // "Member Scores"
            ((FrmMain)MdiParent).tournamentToolStripMenuItem_Click(sender, e);
            EnableHomeNavigation();

        }

        // This is the code behind for the delete database button. Per Rob, we don't need this 
        // at this time. Keeping the code incase it's needed in the future.
        private void btnDropDataBase1_Click_1(object sender, EventArgs e)
        {
            string name = NineTapRegionDB.GetRegionByID(RegionID).NineTapRegionName;
            if (MessageBox.Show($"This button will delete all data stored in the {name} database, are you sure you want to clear  data?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                frmPleaseWait pl = new frmPleaseWait();
                pl.Show();
                //Delete Player History where HisID = selected regionID
                List<PlayerHistory> phis = PlayerHistoryDB.GetAllPlayerHistory(RegionID);
                foreach (var p in phis)
                {
                    PlayerHistoryDB.DeletePlayerHistory(p);
                }

                // Delete FinilizeTemp where FinalizeID = selected regionID
                List<FinalizeTemp> fin = FinalizeTempDB.GetFinalizeListByRegionID(RegionID);

                foreach (var f in fin)
                {
                    FinalizeTempDB.DeleteFinalizeTemp(f);
                }

                // Delete Participants where Participant RegionID = regionID
                List<Participant> par = FinalizeTempDB.GetParticipantListByRegionID(RegionID);

                foreach (var p in par)
                {
                    FinalizeTempDB.DeleteParticipant(p);
                }

                // Delete Games where GameRegionID = regionID
                List<Game> gam = FinalizeTempDB.GetGameListByRegionID(RegionID);

                foreach (var g in gam)
                {
                    PlayerHistoryDB.DeleteGame(g);
                }

                //delete Tournaments where Tournament RegionID = Region ID
                List<Tournament> tourn = TournamentDB.GetTournamentList(RegionID);

                foreach (var t in tourn)
                {
                    TournamentDB.DeleteTournament(t);
                }

                //Delete from Member Table where Memmber RegionID is = selected region ID
                List<Member> mem = MemberDB.GetMemberList(RegionID);

                foreach (var m in mem)
                {
                    MemberDB.DeleteMember(m);
                }

                //delete  the region itself
                NineTapRegion ntr = NineTapRegionDB.GetRegionByID(RegionID);
                NineTapRegionDB.DeleteRegion(ntr);

                if (NineTapRegionDB.GetNumberOfRegions() == 0) // recreate the local region select again if it nothing exists here anymore
                {
                    NineTapRegion n = new NineTapRegion();
                    n.NineTapRegionName = "Local";
                    NineTapRegionDB.AddRegion(n);
                }

                RefreshRegionlist();

                pl.Close();
                MessageBox.Show(name + " Database was successfully cleared!");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var region = new FrmAddRegion(RegionID);
            region.ShowDialog();
            RefreshRegionlist();
        }
        #endregion

        #region CheckBoxes
        private void cbxRegionSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<NineTapRegion> nList = NineTapRegionDB.GetRegionList();
            this.RegionID = nList[cbxRegionSelect.SelectedIndex].NineTapRegionID;

            try
            {   // added a try catch block in order to catch the error that occurs at the very first launch of the program.
                //(the MDi parent is not set yet, so it has to skip over this step on its very first launch or the program wont start)
                ((FrmMain)MdiParent).RegionID = RegionID;
                ((FrmMain)MdiParent)._membersList = MemberDB.GetMemberList(RegionID).OrderBy(m => m.Number);
            }
            catch
            {

            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets FrmMain MdiParent.Home to ture if the active form is FrmMain
        /// </summary>
        private void EnableHomeNavigation()
        {
            if (!(FrmMain.ActiveForm is MainMenu))
            {
                ((FrmMain)MdiParent).Home.Enabled = true;
            }
        }

        /// <summary>
        /// Returns the currently selected RegionID or -1 if no region is selected
        /// </summary>
        public int GetRegionID()
        {
            if (cbxRegionSelect.SelectedIndex >= 0)
            {
                List<NineTapRegion> nList = NineTapRegionDB.GetRegionList();
                return nList[cbxRegionSelect.SelectedIndex].NineTapRegionID;
            }
            return -1;
        }

        /// <summary>
        /// Refreshes the Region list
        /// </summary>
        public void RefreshRegionlist()
        {
            List<NineTapRegion> nList = NineTapRegionDB.GetRegionList();
            cbxRegionSelect.DataSource = nList;
            cbxRegionSelect.DisplayMember = "NineTapRegionName";
        }
        #endregion
    }
}