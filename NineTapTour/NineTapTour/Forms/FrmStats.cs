using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using NineTapTour.Database;
using System.Data.Entity;
using NineTapTour.Models;

namespace NineTapTour.Forms
{
    public partial class FrmStats : Form
    {
        private Member mem;
        private int memId;
        private int memNum;
        private string memName;
        static int TURN_BOLD_IF_BOWLED_OVER_NUMBER = 250;
        List<PlayerHistory> ToBeAdd;
        List<PlayerHistory> ph;
        int RegionID;


        public FrmStats(int memberId, string memberName, Member currentMem, List<PlayerHistory> ToBeAdded, int RegionID)
        {
            InitializeComponent();
            this.memId = memberId;
            this.memNum = currentMem.Number;
            this.RegionID = RegionID;
            this.memName = memberName;
            this.mem = currentMem;
            this.dataGridView1.DoubleBuffered(false);
            this.ToBeAdd = ToBeAdded;
            dataGridView1.DataSource = tableview();
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            if(ToBeAdded.Count == 0)
            {
                btnSaveChanges.Enabled = true;
            }
            else
            {
                btnSaveChanges.Enabled = false;
            }

            dataGridView1.SuspendLayout();
            var column = dataGridView1.Columns[0];
            column.Width = 51;
            column = dataGridView1.Columns[1];
            column.Width = 66;

            for (int h = 2; h < dataGridView1.Columns.Count; h++)
            {
                column = dataGridView1.Columns[h];
                column.Width = 55;
            }
            ph = PlayerHistoryDB.getMemberPlayerHistoryByTotal(memNum, RegionID);


                        
        }
    

       

        struct statHolder
        {
            public statHolder(DateTime Date,
                                string Location,
                                int Squad,
                                int Id,
                                string FirstName,
                                string LastName,
                                int? Game1,
                                int? Game2,
                                int? Game3,
                                int? Game4,
                                int? Handicap,
                                int? Bonus)
            {
                this.Date = Date;
                this.Location = Location;
                this.Squad = Squad;
                this.Id = Id;
                this.FirstName = FirstName;
                this.LastName = LastName;
                this.Game1 = Game1;
                this.Game2 = Game2;
                this.Game3 = Game3;
                this.Game4 = Game4;

                ScratchTotal = ((Game1.HasValue ? Game1 : 0) + (Game2.HasValue ? Game2 : 0) + (Game3.HasValue ? Game3 : 0) + (Game4.HasValue ? Game4 : 0));

                GameTotal = (((Game1.HasValue ? Game1 : 0) + (Handicap + Bonus)) + ((Game2.HasValue ? Game2 : 0) + (Handicap + Bonus)) + ((Game3.HasValue ? Game3 : 0) + (Handicap + Bonus)) + ((Game4.HasValue ? Game4 : 0) + (Handicap + Bonus)));

                AvgPerGame = ((Game1.HasValue ? Game1 : 0) + (Game2.HasValue ? Game2 : 0) + (Game3.HasValue ? Game3 : 0) + (Game4.HasValue ? Game4 : 0));

                int div = ((Game1.HasValue ? 1 : 0) + (Game2.HasValue ? 1 : 0) + (Game3.HasValue ? 1 : 0) + (Game4.HasValue ? 1 : 0));
                if (div != 0)
                {
                    AvgPerGame /= div;
                }

                this.Handicap = Handicap;
                this.Bonus = Bonus;
            }

            public DateTime Date;
            public string Location;
            public int Squad;
            public int Id;
            public string FirstName;
            public string LastName;
            public int? Game1;
            public int? Game2;
            public int? Game3;
            public int? Game4;
            public int? ScratchTotal;
            public int? GameTotal;
            public int? AvgPerGame;
            public int? Handicap;
            public int? Bonus;
        }

        /// <summary>
        /// Populates the stats page for the member selected
        /// </summary>
        /// 
        public void populateStats()
        {

            var db = new NineTapDb();
            var temp = (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        join g in db.Games on p.Game.Id equals g.Id
                        join t in db.Tournaments on p.Tournament.Id equals t.Id
                        where memNum == p.Member.Number
                        orderby t.Date descending
                        select new
                        {
                            t.Date,
                            t.Location,
                            p.Squad,
                            p.Member.Id,
                            p.Member.FirstName,
                            p.Member.LastName,
                            g.Game1,
                            g.Game2,
                            g.Game3,
                            g.Game4,
                            ScratchTotal = 0,
                            GameTotal = 0,
                            AvgPerGame = 0,
                            g.Handicap,
                            g.Bonus
                        }).ToList();

            List<statHolder> stats = new List<statHolder>();
            for (int i = 0; i < temp.Count; i++)
            {
                stats.Add(new statHolder(
                            temp[i].Date,
                             temp[i].Location,
                             temp[i].Squad,
                             temp[i].Id,
                             temp[i].FirstName,
                             temp[i].LastName,
                             temp[i].Game1,
                             temp[i].Game2,
                             temp[i].Game3,
                             temp[i].Game4,
                             temp[i].Handicap,
                             temp[i].Bonus
                        )
                    );
            }
            double sum = 0;
            double count = 0;
            #region Game 1 Average
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Game1);
            }
            txtGame1.Text = String.Format("{0:N2}", (sum / count));
            #endregion
            #region Game 2 Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Game2);
            }
            txtGame2.Text = String.Format("{0:N2}", (sum / count));
            #endregion
            #region Game 3 Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Game3);
            }
            txtGame3.Text = String.Format("{0:N2}", (sum / count));
            #endregion
            #region Game 4 Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Game4);
            }
            txtGame4.Text = String.Format("{0:N2}", (sum / count));
            #endregion
            #region Scratch Total Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].ScratchTotal);
            }
            txtScratchTotal.Text = String.Format("{0:N2}", (sum / count));
            #endregion
            #region Game Total Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].GameTotal);
            }
            txtGameTotal.Text = String.Format("{0:N2}", (sum / count));
            #endregion
            #region Average Game Score
            sum = 0;
            foreach (var item in stats)
            {
                sum += Convert.ToDouble(item.AvgPerGame);
            }

            txtAveragePerGame.Text = (sum / stats.Count()).ToString();
            #endregion

            #region Handicap Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Handicap);
            }
            txtHandicap.Text = String.Format("{0:N2}", (sum / count));
            #endregion
            #region Bonus Pins Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Bonus);
            }
            txtBonus.Text = String.Format("{0:N2}", (sum / count));
            #endregion

        }

        public DataTable tableview()
        {

            DataTable dtGames = new DataTable();
            var db = new NineTapDb();
            var temp = (from p in db.PlayerHistory
                        where p.MemberNumber == memNum && p.regionID == RegionID
                        orderby p.TournamentDate descending, p.hisID descending
                        select new
                        {
                            p.hisID,
                            p.GameID,
                            p.GamesPlayed,
                            p.TournamentDate,
                            p.Game1,
                            p.Game2,
                            p.Game3,
                            p.Game4,
                            ScratchTotal = p.Game1 + p.Game2 + p.Game3 + p.Game4,
                            TotalScore = (p.Game1 + p.Bonus + p.HandiCap) + (p.Game2 + p.Bonus + p.HandiCap) + (p.Game3 + p.Bonus + p.HandiCap) + (p.Game4 + p.Bonus + p.HandiCap),
                            p.AverageForGame,
                            p.trueAVG,
                            p.AVG,
                            p.HandiCap,
                            p.Bonus,
                            p.ProPot,
                            p.MoneyWon,
                            p.PPHG,
                            p.Notes
                        });
            dtGames.Columns.Add("Games").ReadOnly = true;
            dtGames.Columns.Add("Date", typeof(DateTime));
            dtGames.Columns.Add("Game1");
            //dtGames.Columns.Add(new DataColumn("Selected", typeof(bool)));
            dtGames.Columns.Add("Game2");
            dtGames.Columns.Add("Game3");
            dtGames.Columns.Add("Game4"); 
            dtGames.Columns.Add("Scratch Total", typeof(Int32)).ReadOnly = true;
            dtGames.Columns.Add("Game Total w/HDCP", typeof(Int32)).ReadOnly = true;
            dtGames.Columns.Add("Entry AVG", typeof(Int32)).ReadOnly = true;
            dtGames.Columns.Add("30 Entry AVG", typeof(Int32)).ReadOnly = true;
            dtGames.Columns.Add("Adjusted AVG");
            dtGames.Columns.Add("Handicap").ReadOnly = true;
            dtGames.Columns.Add("Bonus").ReadOnly = true;
            dtGames.Columns.Add("Pro Pot").ReadOnly = true;
            dtGames.Columns.Add("Place").ReadOnly = true;
            dtGames.Columns.Add("Money Won", typeof(Decimal));
            dtGames.Columns.Add("Notes");
            dtGames.Columns.Add("GameID").ReadOnly = true;


            foreach (var item in ToBeAdd)
            {
                DataRow newRow = dtGames.NewRow();
                newRow["Games"] = item.GamesPlayed;
                newRow["Date"] = item.TournamentDate.ToShortDateString();
                if (item.Game1 == 0)
                    newRow["Game1"] = null;
                else
                    newRow["Game1"] = item.Game1;

                if (item.Game2 == 0)
                    newRow["Game2"] = null;
                else
                    newRow["Game2"] = item.Game2;
                if (item.Game3 == 0)
                    newRow["Game3"] = null;
                else
                    newRow["Game3"] = item.Game3;
                if (item.Game4 == 0)
                    newRow["Game4"] = null;
                else
                    newRow["Game4"] = item.Game4;
                newRow["Scratch Total"] = item.TotalScore;
                newRow["Game Total w/HDCP"] = item.TotalScore + (item.HandiCap * item.GamesPlayed);
                newRow["Entry AVG"] = item.AverageForGame;
                newRow["30 Entry AVG"] = item.trueAVG;
                if (item.AVG == 0)
                    newRow["Adjusted AVG"] = null;
                else
                    newRow["Adjusted AVG"] = item.AVG;
                newRow["Handicap"] = item.HandiCap;
                newRow["Bonus"] = item.Bonus;
                newRow["Pro Pot"] = item.ProPot;
                newRow["Money Won"] = item.MoneyWon;
                newRow["Place"] = item.PPHG;
                newRow["Notes"] = item.Notes;
                newRow["GameID"] = item.GameID;

                dtGames.Rows.Add(newRow);

            }



            foreach (var item in temp)
            {

                DataRow newRow = dtGames.NewRow();
                newRow["Games"] = item.GamesPlayed;
                newRow["Date"] = item.TournamentDate.ToShortDateString();
                if (item.Game1 == 0)
                    newRow["Game1"] = null;
                else
                    newRow["Game1"] = item.Game1;

                if (item.Game2 == 0)
                    newRow["Game2"] = null;
                else
                    newRow["Game2"] = item.Game2;
                if (item.Game3 == 0)
                    newRow["Game3"] = null;
                else
                    newRow["Game3"] = item.Game3;
                if (item.Game4 == 0)
                    newRow["Game4"] = null;
                else
                    newRow["Game4"] = item.Game4;
                newRow["Scratch Total"] = item.ScratchTotal;
                newRow["Game Total w/HDCP"] = item.TotalScore;
                newRow["Entry AVG"] = Convert.ToDouble((item.Game1 + item.Game2 + item.Game3 + item.Game4) / item.GamesPlayed);
                newRow["30 Entry AVG"] = item.trueAVG;
                if (item.AVG == 0)
                    newRow["Adjusted AVG"] = null;
                else
                    newRow["Adjusted AVG"] = item.AVG;
                newRow["Handicap"] = item.HandiCap;
                newRow["Bonus"] = item.Bonus;
                newRow["Pro Pot"] = item.ProPot;
                newRow["Money Won"] = item.MoneyWon;
                newRow["Place"] = item.PPHG;
                newRow["Notes"] = item.Notes;
                newRow["GameID"] = item.GameID;

                dtGames.Rows.Add(newRow);

            }

            return dtGames;
        }






        private void FrmStats_Load(object sender, EventArgs e)
        {
          
            try
            {
               
                lblName.Text = mem.FirstName + "    " + mem.LastName;
            }
            catch
            {
                lblName.Text = mem.FirstName + "   " + mem.LastName;
            }
            lblMemberNumber.Text = Convert.ToString(memNum);
            try
            {
                lblStartAvg.Text = mem.StartAvg.ToString();
            }
            catch
            {
                lblStartAvg.Text = 0.ToString();
            }
            List<PlayerHistory> Last30 = PlayerHistoryDB.getTop30FromPlayerHistory(mem.Number);
            int game1AVG = 0;
            int game2AVG = 0;
            int game3AVG = 0;
            int game4AVG = 0;
            int scratchTotal = 0;
            int gameTotal = 0;
            if (Last30.Count > 0)
            {
                for (int i = 0; i < Last30.Count; i++)
                {

                    game1AVG += Last30[i].Game1 ?? 0;
                    game2AVG += Last30[i].Game2 ?? 0;
                    game3AVG += Last30[i].Game3 ?? 0;
                    game4AVG += Last30[i].Game4 ?? 0;
                    scratchTotal += (Last30[i].Game1 ?? 0) + (Last30[i].Game2 ?? 0) + (Last30[i].Game3 ?? 0) + (Last30[i].Game4 ?? 0);
                    int total = (Last30[i].Game1 != null) ? (Last30[i].Game1 ?? 0 + Last30[i].HandiCap + Last30[i].Bonus) : 0;
                    total += (Last30[i].Game2 != null) ? (Last30[i].Game2 ?? 0 + Last30[i].HandiCap + Last30[i].Bonus) : 0;
                    total += (Last30[i].Game3 != null) ? (Last30[i].Game3 ?? 0 + Last30[i].HandiCap + Last30[i].Bonus) : 0;
                    total += (Last30[i].Game4 != null) ? (Last30[i].Game4 ?? 0 + Last30[i].HandiCap + Last30[i].Bonus) : 0;
                    gameTotal = total;
                }

                game1AVG /= Last30.Count;
                game2AVG /= Last30.Count;
                game3AVG /= Last30.Count;
                game4AVG /= Last30.Count;

                txtGame1.Text = game1AVG.ToString();
                txtGame2.Text = game2AVG.ToString();
                txtGame3.Text = game3AVG.ToString();
                txtGame4.Text = game4AVG.ToString();
                txtScratchTotal.Text = scratchTotal.ToString();
                txtGameTotal.Text = gameTotal.ToString();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            printDialog1.Document = printDocument1;
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap bm = new Bitmap(this.dataGridView1.Width, this.dataGridView1.Height);
            this.dataGridView1.DrawToBitmap(bm, new Rectangle(0, 0, this.dataGridView1.Width, this.dataGridView1.Height));
            e.Graphics.DrawImage(bm, 0, 0);
        }
        //makes the 30 game avg column green and potential games to be added to light blue
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        { 
            dataGridView1.SuspendLayout();

            dataGridView1.SuspendLayout();
            var column = dataGridView1.Columns["Notes"];
            column.Width = 95;


            int top5 = 0;
            
     

            for(int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int t = 0; t < ToBeAdd.Count; t++)
                {
                    if (ToBeAdd[t].GameID == Convert.ToInt32(dataGridView1.Rows[i].Cells[17].Value))
                    {
                        for (int r = 0; r < dataGridView1.ColumnCount; r++)
                        {
                            dataGridView1.Rows[i].Cells[r].Style.BackColor = Color.LightBlue;                   
                        }

                    }
                }
            }

            int gameCount = dataGridView1.RowCount; // variable for how many rows will show up
            const int THIRTY_ENTRY = 30; // variable for how many rows are highlighted as per client request
            if (gameCount > THIRTY_ENTRY)
            {
                for (int i = 0; i < THIRTY_ENTRY; i++)
                {
                    dataGridView1.Rows[i].Cells[9].Style.BackColor = Color.GreenYellow;
                }
            }
            else
            {
                for (int z = 0; z < gameCount; z++)
                {
                    dataGridView1.Rows[z].Cells[9].Style.BackColor = Color.GreenYellow;
                }
            }


            //sets any game over 250 to bold black
            //for (int k = 0; k < dataGridView1.Rows.Count; k++)
            //{
            //    int game1;
            //    bool Game1 = int.TryParse(dataGridView1.Rows[k].Cells[2].Value.ToString(), out game1);

            //    int game2;
            //    bool Game2 = int.TryParse(dataGridView1.Rows[k].Cells[3].Value.ToString(), out game2);

            //    int game3;
            //    bool Game3 = int.TryParse(dataGridView1.Rows[k].Cells[4].Value.ToString(), out game3);

            //    int game4;
            //    bool Game4 = int.TryParse(dataGridView1.Rows[k].Cells[5].Value.ToString(), out game4);


            //    if (game1 > TURN_BOLD_IF_BOWLED_OVER_NUMBER && Game1 == true)
            //    {
            //        dataGridView1.Rows[k].Cells[2].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            //        dataGridView1[2, k].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            //    }
            //    if (game2 > TURN_BOLD_IF_BOWLED_OVER_NUMBER && Game2 == true)
            //    {
            //        dataGridView1.Rows[k].Cells[3].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            //    }
            //    if (game3 > TURN_BOLD_IF_BOWLED_OVER_NUMBER && Game3 == true)
            //    {
            //        dataGridView1.Rows[k].Cells[4].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            //    }
            //    if (game4 > TURN_BOLD_IF_BOWLED_OVER_NUMBER && Game4 == true)
            //    {
            //        dataGridView1.Rows[k].Cells[5].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            //    }

            //}

            //sets there career top 5 scratch series scores to red font
            //for (int j = 0; j < dataGridView1.Rows.Count; j++)
            //{
            //    if (top5 <= 5)
            //    {
            //        if (Convert.ToInt32(dataGridView1.Rows[j].Cells[6].Value) == ph[0].TotalScore)
            //        {
            //            dataGridView1.Rows[j].Cells[6].Style.ForeColor = Color.Red;
            //            top5++;
            //        }
            //        if (Convert.ToInt32(dataGridView1.Rows[j].Cells[6].Value) == ph[1].TotalScore)
            //        {
            //            dataGridView1.Rows[j].Cells[6].Style.ForeColor = Color.Red;
            //            top5++;
            //        }
            //        if (Convert.ToInt32(dataGridView1.Rows[j].Cells[6].Value) == ph[2].TotalScore)
            //        {
            //            dataGridView1.Rows[j].Cells[6].Style.ForeColor = Color.Red;
            //            top5++;
            //        }
            //        if (Convert.ToInt32(dataGridView1.Rows[j].Cells[6].Value) == ph[3].TotalScore)
            //        {
            //            dataGridView1.Rows[j].Cells[6].Style.ForeColor = Color.Red;
            //            top5++;
            //        }
            //        if (Convert.ToInt32(dataGridView1.Rows[j].Cells[6].Value) == ph[4].TotalScore)
            //        {
            //            dataGridView1.Rows[j].Cells[6].Style.ForeColor = Color.Red;
            //            top5++;
            //        }
            //    }
            //}


            dataGridView1.ResumeLayout();

        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            //grab untouched playerhistory
            List<PlayerHistory> pHist = PlayerHistoryDB.getMemberPlayerHistory(mem.Number, RegionID);


            //RESTORE THE DATAGRID BACK TO THE DATE DESCINDING 
            dataGridView1.Sort(dataGridView1.Columns["Date"], System.ComponentModel.ListSortDirection.Descending);
            
            //if valid, store new info from slots in playerhistory
            for(int saveX = 0; saveX < dataGridView1.RowCount; saveX++)
            {
                for(int saveY = 1; saveY < dataGridView1.ColumnCount;) //start loop at 1 to avoid editing "games played" slot
                {
                    pHist[saveX].TournamentDate = Convert.ToDateTime(dataGridView1[saveY, saveX].Value);
                    saveY++;
                    //Skips null values becuase they cant convert to ints
                    if(dataGridView1[saveY, saveX].Value.ToString() !=  "")
                    {
                        pHist[saveX].Game1 = Convert.ToInt32(dataGridView1[saveY, saveX].Value);        
                    }
                    saveY++;
                    if (dataGridView1[saveY, saveX].Value.ToString() != "")
                    {
                        pHist[saveX].Game2 = Convert.ToInt32(dataGridView1[saveY, saveX].Value);
                    }
                    saveY++;
                    if (dataGridView1[saveY, saveX].Value.ToString() != "")
                    {
                        pHist[saveX].Game3 = Convert.ToInt32(dataGridView1[saveY, saveX].Value);
                    }
                    saveY++;
                    if (dataGridView1[saveY, saveX].Value.ToString() != "")
                    {
                        pHist[saveX].Game4 = Convert.ToInt32(dataGridView1[saveY, saveX].Value);
                    }
                    saveY++;
                    pHist[saveX].TotalScore = (pHist[saveX].Game1 ?? 0) + (pHist[saveX].Game2 ?? 0) + (pHist[saveX].Game3 ?? 0) + (pHist[saveX].Game4 ?? 0);
                    saveY++;
                    //skip total score with handicap. not apart of Playerhistory class
                    saveY++;
                    pHist[saveX].AverageForGame = Convert.ToDouble(pHist[saveX].TotalScore / pHist[saveX].GamesPlayed);
                    saveY++;
                    //skip 30 game avg. doesnt need to be adjusted here. more complicated, adjust seperately.
                    saveY++;
                    if (dataGridView1[saveY, saveX].Value.ToString() != "")
                    {
                        pHist[saveX].AVG = Convert.ToInt32(dataGridView1[saveY, saveX].Value);
                    }                    
                    saveY++; 
                    pHist[saveX].HandiCap = Convert.ToInt32(dataGridView1[saveY, saveX].Value);
                    saveY++;
                    pHist[saveX].Bonus = Convert.ToInt32(dataGridView1[saveY, saveX].Value);
                    saveY++;
                    pHist[saveX].ProPot = Convert.ToString(dataGridView1[saveY, saveX].Value);
                    saveY++;
                    pHist[saveX].PPHG = Convert.ToString(dataGridView1[saveY, saveX].Value);
                    saveY++;
                    pHist[saveX].MoneyWon = Convert.ToDecimal(dataGridView1[saveY, saveX].Value);
                    saveY++;
                    pHist[saveX].Notes = Convert.ToString(dataGridView1[saveY, saveX].Value);
                    saveY++;
                    //skip gameID, should never be editted
                    saveY++;
                }
            }
            //update info
            foreach(var item in pHist)
            {
                /* Prevents stats from disappearing from frmStats after Save button is clicked. 
                   RegionID in PlayerHistory class was being reset to default value of zero. */
                item.regionID = RegionID;

                PlayerHistoryDB.AddPlayerHistory2(item);
            }
            //refresh page
            dataGridView1.DataSource = tableview();

        }
    }
}
