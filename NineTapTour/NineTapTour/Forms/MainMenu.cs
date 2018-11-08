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
        public FrmMain currMainFrm { get; set; }
        public int regionID { get; set; }
        /// <summary>
        /// Opens the "Main Menu" form.
        /// </summary>
        public MainMenu()
        {
            InitializeComponent();
            //check to see if any regions exist, if not create a local region(for first time start up)
            if(NineTapRegionDB.getNumberOfRegions() == 0)
            {
                NineTapRegion nTemp = new NineTapRegion();
                nTemp.NineTapRegionName = "Local";
                NineTapRegionDB.AddRegion(nTemp);
            }
            //set the global int region so it can be used to filter each region throughout the program
            List <NineTapRegion>  nList = NineTapRegionDB.GetRegionList();
            cbxRegionSelect.DataSource = nList;
            cbxRegionSelect.DisplayMember = "NineTapRegionName";
            this.regionID = nList[cbxRegionSelect.SelectedIndex].NineTapRegionID;
           
           

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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbout_Click(object sender, EventArgs e)
        {
            ((FrmMain)MdiParent).RegionID = regionID; // Retrieving ID from menMain

            ((FrmMain)MdiParent).menuHighlight(btnAbout.Text); // Highlighting corresponding tab

            ((FrmMain)MdiParent).AboutToolStripMenuItem_Click(sender, e); // Activate the click method for About
        }

        /// <summary>
        /// Brings up the "Member Data" form when the "Member Data" button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMemberData_Click(object sender, EventArgs e)
        {
            ((FrmMain)MdiParent).RegionID = regionID;
            ((FrmMain)MdiParent).menuHighlight(btnMemberData.Text);
            ((FrmMain)MdiParent).memberToolStripMenuItem_Click(sender, e);
        }
        /// <summary>
        /// Brings up the "Member Scores" form when the "Member Scores" button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMemberScores_Click(object sender, EventArgs e)
        {
            ((FrmMain)MdiParent).RegionID = regionID;
            ((FrmMain)MdiParent).menuHighlight(btnMemberScores.Text);
            ((FrmMain)MdiParent).tournamentToolStripMenuItem_Click(sender, e);
        }

        private void MainMenu_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Font drawFont = new Font("Arial", 12);
            SolidBrush drawBrush = new SolidBrush(Color.White);
            PointF drawPoint = new PointF(10, 2);
            g.DrawString("Version: 1.4.3", drawFont, drawBrush, drawPoint);
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

            List<NineTapRegion> nList = NineTapRegionDB.GetRegionList();
            string name = NineTapRegionDB.getRegionByID(regionID).NineTapRegionName;
            if (MessageBox.Show($"This button will delete all data stored in the {NineTapRegionDB.getRegionByID(regionID).NineTapRegionName} database, are you sure you want to clear  data?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                frmPleaseWait pl = new frmPleaseWait();
                pl.Show();
                //Delete Player History where HisID = selected regionID
                List<PlayerHistory> phis = PlayerHistoryDB.getAllPlayerHistory(regionID);
                foreach (var p in phis)
                {
                    PlayerHistoryDB.DeletePlayerHistory(p);
                }
                // Delete FinilizeTemp where FinalizeID = selected regionID
                List<FinalizeTemp> fin = FinalizeTempDB.GetFinalizeList(regionID);
                foreach (var f in fin)
                {
                    FinalizeTempDB.DeleteFinilizeTemp(f);
                }
                // Delete Participants where Participant RegionID = regionID
                List<Participant> par = FinalizeTempDB.GetparticpantList(regionID);
                foreach (var p in par)
                {
                    FinalizeTempDB.deleteParticipant(p);
                }
                // Delete Games where GameRegionID = regionID
                List<Game> gam = FinalizeTempDB.GetGameList(regionID);
                foreach (var g in gam)
                {
                    PlayerHistoryDB.DeleteGame(g);
                }
                //delete Tournaments where Tournament RegionID = Region ID
                List<Tournament> tourn = TournamentDb.GetTournamentList(regionID);
                foreach (var t in tourn)
                {
                    TournamentDb.deleteTournament(t);
                }

                //Delete from Member Table where Memmber RegionID is = selected region ID
                List<Member> mem = MemberDb.GetMemberList(regionID);
                foreach (var m in mem)
                {
                    MemberDb.DeleteMember(m);
                }

                //delete  the region itself
                NineTapRegion ntr = NineTapRegionDB.getRegionByID(regionID);
                NineTapRegionDB.deleteRegion(ntr);

                if (NineTapRegionDB.getNumberOfRegions() == 0) // recreate the local region select again if it nothing exists here anymore
                {
                    NineTapRegion n = new NineTapRegion();
                    n.NineTapRegionID = 1;
                    n.NineTapRegionName = "Local";
                    NineTapRegionDB.AddRegion(n);

                }

                refreshRegionlist();

                pl.Close();
                MessageBox.Show(name + " Database was successfully cleared!");


            }
        }

        // on load grabs an updated version of the all the player informantion so you dont have to go their page to update their player information to the right information
        private void MainMenu_Load(object sender, EventArgs e)
        {
            //List<Member> memberList = MemberDb.GetMemberList(regionID);
            
            //for(int i = 0; i < memberList.Count; i++)
            //{

            //    List<PlayerHistory> ph = PlayerHistoryDB.getLastFiveFromPlayerhistory(memberList[i].Number,regionID);
            //    if (ph.Count > 0)
            //    {
            //        memberList[i].StartAvg = ph[0].AVG;
            //        memberList[i].Average = Convert.ToInt32(ph[0].trueAVG);
            //        MemberDb.AddMember(memberList[i]);
            //    }
            //}

        }

        private void cbxRegionSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<NineTapRegion> nList = NineTapRegionDB.GetRegionList();
            this.regionID = nList[cbxRegionSelect.SelectedIndex].NineTapRegionID;
            try// added a try catch block in order to catch the error that occurs at the very first launch of the program.
               //(the MDi parent is not set yet, so it has to skip over this step on its very first launch or the program wont start)
            {
                ((FrmMain)MdiParent).RegionID = regionID;
                ((FrmMain)MdiParent)._membersList = MemberDb.GetMemberList(regionID).OrderBy(m => m.Number);
            }
            catch
            {

            }

        }

        public int getRegionID()
        {
            List<NineTapRegion> nList = NineTapRegionDB.GetRegionList();
            return nList[cbxRegionSelect.SelectedIndex].NineTapRegionID;
        }

        public void refreshRegionlist()
        {
            List<NineTapRegion> nList = NineTapRegionDB.GetRegionList();
            cbxRegionSelect.DataSource = nList;
            cbxRegionSelect.DisplayMember = "NineTapRegionName";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var region = new FrmAddRegion(regionID);
            region.ShowDialog();
            refreshRegionlist();
        }

	}
}