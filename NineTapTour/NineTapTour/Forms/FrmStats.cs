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
        List<PlayerHistory> ph;
        int RegionID;


        public FrmStats(int memberId, string memberName, Member currentMem, int RegionID)
        {
            InitializeComponent();
            this.memId = memberId;
            this.memNum = currentMem.Number;
            this.RegionID = RegionID;
            this.memName = memberName;
            this.mem = currentMem;
            this.dataGridView1.DoubleBuffered(false);
            dataGridView1.DataSource = tableview();
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
      
            // Disable saving if nothing to save
            btnSaveChanges.Enabled = (dataGridView1.RowCount == 0) ? false : true;

            dataGridView1.SuspendLayout();
            var column = dataGridView1.Columns[0]; // Games *
            column.Width = 45;
            column = dataGridView1.Columns[1]; // Date * 
            column.Width = 65;

            column = dataGridView1.Columns[2]; // Game 1 *
            column.Width = 35;

            column = dataGridView1.Columns[3]; // Game 2 *
            column.Width = 35;

            column = dataGridView1.Columns[4]; // Game 3 *
            column.Width = 35;

            column = dataGridView1.Columns[5]; //Game 4 *
            column.Width = 35;

            column = dataGridView1.Columns[6]; // Scratch total
            column.Width = 45;

            column = dataGridView1.Columns[7]; //Game total with handicap
            column.Width = 55;

            column = dataGridView1.Columns[8]; // Entry Avg
            column.Width = 45;

            column = dataGridView1.Columns[9]; // 30 entry avg
            column.Width = 45;

            column = dataGridView1.Columns[10]; // Adj Avg
            column.Width = 45;

            column = dataGridView1.Columns[11]; // Handicap
            column.Width = 45;

            column = dataGridView1.Columns[12]; // Bonus
            column.Width = 45;

            column = dataGridView1.Columns[13]; // Pro Pot
            column.Width = 45;

            column = dataGridView1.Columns[14]; // Place
            column.Width = 45;

            column = dataGridView1.Columns[15]; // Money Won
            column.Width = 45;

            column = dataGridView1.Columns[16]; // Notes
            column.Width = 180;

            column = dataGridView1.Columns[17]; // game ID
            column.Width = 45;

            foreach (DataGridViewColumn col in dataGridView1.Columns) // to center header cell titles
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
                        ));
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
            dtGames.Columns.Add("Gm1");
            dtGames.Columns.Add("Gm2");
            dtGames.Columns.Add("Gm3");
            dtGames.Columns.Add("Gm4"); 
            dtGames.Columns.Add("Scratch Total", typeof(Int32)).ReadOnly = true;
            dtGames.Columns.Add("Game Total w/HDCP", typeof(Int32)).ReadOnly = true;
            dtGames.Columns.Add("Entry AVG", typeof(Int32)).ReadOnly = true;
            dtGames.Columns.Add("30 Entry AVG", typeof(Int32)).ReadOnly = true;
            dtGames.Columns.Add("Adj. AVG");
            dtGames.Columns.Add("HDCP").ReadOnly = true;
            dtGames.Columns.Add("Bonus").ReadOnly = true;
            dtGames.Columns.Add("Pro Pot").ReadOnly = true;
            dtGames.Columns.Add("Place").ReadOnly = true;

            string moneyWonWithTotal = $"Money Won ({PlayerHistoryDB.GetTotalMoneyWon(memNum, RegionID)})";
            dtGames.Columns.Add(moneyWonWithTotal, typeof(Decimal));
            dtGames.Columns.Add("Notes");
            dtGames.Columns.Add("GmID").ReadOnly = true;

            foreach (var item in temp)
            {
                DataRow newRow = dtGames.NewRow();
                newRow["Games"] = item.GamesPlayed;
                newRow["Date"] = item.TournamentDate.ToShortDateString();
                #region display_fix_till_more_perm_fix_in_importation
                if(item.GamesPlayed == 0)
                {
                    continue;
                }
                // some entries in the imported excel files have 0 gamesTotal and no relevant data to be
                // imported which would cause crash when displaying 
                #endregion

                if (item.Game1 == 0)
                    newRow["Gm1"] = null;
                else
                    newRow["Gm1"] = item.Game1;

                if (item.Game2 == 0)
                    newRow["Gm2"] = null;
                else
                    newRow["Gm2"] = item.Game2;
                if (item.Game3 == 0)
                    newRow["Gm3"] = null;
                else
                    newRow["Gm3"] = item.Game3;
                if (item.Game4 == 0)
                    newRow["Gm4"] = null;
                else
                    newRow["Gm4"] = item.Game4;
                newRow["Scratch Total"] = item.ScratchTotal;
                newRow["Game Total w/HDCP"] = item.TotalScore;
              
                newRow["30 Entry AVG"] = item.trueAVG;

                if (item.AVG == 0)
                    newRow["Adj. AVG"] = null;
                else
                    newRow["Adj. AVG"] = item.AVG;
                newRow["HDCP"] = item.HandiCap;
                newRow["Bonus"] = item.Bonus;
                newRow["Pro Pot"] = item.ProPot;
                newRow[moneyWonWithTotal] = item.MoneyWon;
                newRow["Place"] = item.PPHG;
                newRow["Notes"] = item.Notes;
                newRow["GmID"] = item.GameID;

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


        /// <summary>
        /// Changes the background color for the top thirty cells in the "30 Entry AVG" column
        /// Changes background color of bonus cells if game was in a tournament that bonus pins reset to 0 (player cashed)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        { 
            dataGridView1.SuspendLayout();

            int currRowCount = e.RowIndex + 1; 
            string columnName = dataGridView1.Columns[e.ColumnIndex].Name;

            // Only first 30 rows should be highlighted
            const int THIRTY_ENTRIES = 30;
            DataGridViewCell currCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (columnName == "30 Entry AVG" && currRowCount <= THIRTY_ENTRIES)
            {
                currCell.Style.BackColor = Color.GreenYellow;
            }
            else if (columnName == "Bonus")
            {
                int currBonus = Convert.ToInt32(currCell.Value);
                const int MONEY_WON_INDEX = 15;
                double currMoneyWon = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[MONEY_WON_INDEX].Value);
                bool currBonusHasReset = false;

                // change current bonus pins cell background color if 0 has reset
                if (currBonus == 0 && currMoneyWon > 0)
                {
                    currCell.Style.BackColor = Color.HotPink;
                    currBonusHasReset = true;
                }

                const int DATE_INDEX = 1;
                DateTime currDate = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[DATE_INDEX].Value);
                int currRowIndex = e.RowIndex - 1;

                // while the same tournament in a different row above
                while (currRowIndex >= 0 && currDate == Convert.ToDateTime(dataGridView1.Rows[currRowIndex].Cells[DATE_INDEX].Value))
                {
                    // if bonus has reset and is the same tournament, then highlight this
                    if (currBonusHasReset)
                    {
                        dataGridView1.Rows[currRowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.HotPink;
                    }
                    else // if bonus hasn't reset then check to see if it does reset in a different row in same tournament
                    {
                        int nextBonus = Convert.ToInt32(dataGridView1.Rows[currRowIndex].Cells[e.ColumnIndex].Value);
                        double nextMoneyWon = Convert.ToDouble(dataGridView1.Rows[currRowIndex].Cells[MONEY_WON_INDEX].Value);

                        if (nextBonus == 0 && nextMoneyWon > 0)
                        {
                            currCell.Style.BackColor = Color.HotPink;
                        }
                    }
                    currRowIndex--;
                }
            }
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

                PlayerHistoryDB.AddOrUpdatePlayerHistory(item);
            }
            //refresh page
            dataGridView1.DataSource = tableview();
        }
    }
}
