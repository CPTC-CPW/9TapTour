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

namespace NineTapTour.Forms
{
    public partial class MainMenu : Form
    {
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
                nTemp.NineTapRegionID = NineTapRegionDB.getNumberOfRegions() + 1;
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
        /// Brings up a message box explaining what the 9-Tap Tour is about when the "About" button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbout_Click(object sender, EventArgs e)
        {
            // TODO finish 
            MessageBox.Show("9-tap tour inc. is a unique, fun, and professionally run tournament. Members enjoy our Beat the board format, with four games per squad. Yet the fun, big payouts, special pots and the 9 tap version of bowling itself, brings new excitement to tournaments. \n Approximately one bowler in every 5 entries will cash. Other optional ways to cash are: 9 tap Jackpot, Progressive Pot, high game pots, brackets, scratch game and series pot, and more depending on where and when you bowl these Side Pots may vary from time to time. 9 Tap Tour also has BIG added tournaments. Each quarterly Tournament may have eligibility requirements for members who bowl during that Quarter. \n");
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
#if DEBUG
            Graphics g = e.Graphics;
            Font drawFont = new Font("Arial", 12);
            SolidBrush drawBrush = new SolidBrush(Color.Red);
            PointF drawPoint = new PointF(10, 2);
            g.DrawString("DEVELOPMENT VERSION NOT FOR PRODUCTION", drawFont, drawBrush, drawPoint);
#endif
        }


        private void btnDropDataBase1_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("This button will delete all data stored in the database, are you sure you want to clear  data?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                System.Data.Entity.Database.Delete("name=NineTapDbConnection");
                MessageBox.Show("Data Cleared!,restart to continue.");
                this.Close();
                FrmMain m = new FrmMain();
                m.Close();

            }
        }
        // on load grabs an updated version of the all the player informantion so you dont have to go their page to update their player information to the right information
        private void MainMenu_Load(object sender, EventArgs e)
        {
            List<Member> memberList = MemberDb.GetMemberList(regionID);
            
            for(int i = 0; i < memberList.Count; i++)
            {

                List<PlayerHistory> ph = PlayerHistoryDB.getLastFiveFromPlayerhistory(memberList[i].Number,regionID);
                if (ph.Count > 0)
                {
                    memberList[i].StartAvg = ph[0].AVG;
                    memberList[i].Average = Convert.ToInt32(ph[0].trueAVG);
                    MemberDb.AddMember(memberList[i]);
                }
            }

        }

        private void cbxRegionSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<NineTapRegion> nList = NineTapRegionDB.GetRegionList();
            this.regionID = nList[cbxRegionSelect.SelectedIndex].NineTapRegionID;
            ((FrmMain)MdiParent).RegionID = regionID;
            ((FrmMain)MdiParent)._membersList = MemberDb.GetMemberList(regionID).OrderBy(m => m.Number);

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