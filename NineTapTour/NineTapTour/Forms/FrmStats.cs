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
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.Services;
using NineTapTour.Helpers;

namespace NineTapTour.Forms
{
    public partial class FrmStats : Form
    {
        private readonly Member mem;
        private readonly int memId;
        private readonly int memNum;
        private readonly string memName;
        private readonly IPlayerHistoryRepository playerHistoryRepository;
        private readonly IStatsService statsService;
        readonly List<PlayerHistoryViewModel> ph;


        public FrmStats(int memberId, string memberName, Member currentMem, IPlayerHistoryRepository playerHistoryRepository, IStatsService statsService)
        {
            this.playerHistoryRepository = playerHistoryRepository;
            this.statsService = statsService;

            InitializeComponent();
            this.memId = memberId;
            this.memNum = currentMem.Number;
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

            column = dataGridView1.Columns[13]; // Place
            column.Width = 45;

            column = dataGridView1.Columns[14]; // Money Won
            column.Width = 45;

            column = dataGridView1.Columns[15]; // Notes
            column.Width = 180;

            column = dataGridView1.Columns[16]; // game ID
            column.Width = 45;

            foreach (DataGridViewColumn col in dataGridView1.Columns) // to center header cell titles
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            ph = playerHistoryRepository.GetMemberPlayerHistory(memNum);
        }

        /// <summary>
        /// Populates the stats page for the member selected.
        /// The query and math live in StatsService (M7.5); this only formats the results.
        /// </summary>
        public void populateStats()
        {
            MemberStatAverages averages = statsService.GetMemberStatAverages(memNum);

            txtGame1.Text = String.Format("{0:N2}", averages.Game1Average);
            txtGame2.Text = String.Format("{0:N2}", averages.Game2Average);
            txtGame3.Text = String.Format("{0:N2}", averages.Game3Average);
            txtGame4.Text = String.Format("{0:N2}", averages.Game4Average);
            txtScratchTotal.Text = String.Format("{0:N2}", averages.ScratchTotalAverage);
            txtGameTotal.Text = String.Format("{0:N2}", averages.GameTotalAverage);
            txtAveragePerGame.Text = averages.AveragePerGame.ToString();
            txtHandicap.Text = String.Format("{0:N2}", averages.HandicapAverage);
            txtBonus.Text = String.Format("{0:N2}", averages.BonusAverage);
        }

        public DataTable tableview()
        {
            DataTable dtGames = new();

            // Query and display shaping live in StatsService (M7.5);
            // this method only boxes the rows into a DataTable for the grid.
            MemberStatsResult stats = statsService.GetMemberStats(memNum);

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
            dtGames.Columns.Add("Place").ReadOnly = true;

            string moneyWonWithTotal = $"Money Won ({stats.TotalMoneyWon})";
            dtGames.Columns.Add(moneyWonWithTotal, typeof(Decimal));
            dtGames.Columns.Add("Notes");
            dtGames.Columns.Add("GmID").ReadOnly = true;

            foreach (MemberStatsRow item in stats.Rows)
            {
                DataRow newRow = dtGames.NewRow();
                newRow["Games"] = item.GamesPlayed;
                newRow["Date"] = item.TournamentDate.ToShortDateString();
                newRow["Gm1"] = item.Game1; // Zero scores were shaped to null by the service
                newRow["Gm2"] = item.Game2;
                newRow["Gm3"] = item.Game3;
                newRow["Gm4"] = item.Game4;
                newRow["Scratch Total"] = item.ScratchTotal;
                newRow["Game Total w/HDCP"] = item.HandicapTotal;
                newRow["30 Entry AVG"] = item.LeagueAverage;
                newRow["Adj. AVG"] = item.AdjustedAvg;
                newRow["HDCP"] = item.Handicap;
                newRow["Bonus"] = item.Bonus;
                newRow[moneyWonWithTotal] = item.MoneyWon;
                newRow["Place"] = item.Place;
                newRow["Notes"] = item.Notes;
                newRow["GmID"] = item.GameId;

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
                lblStartAvg.Text = mem.Average.ToString();
            }
            catch
            {
                lblStartAvg.Text = 0.ToString();
            }

            // The last-30-entries math lives in StatsService (M7.5); this only displays it
            Last30Averages last30 = statsService.GetLast30Averages(mem.Number);

            if (last30.EntryCount > 0)
            {
                txtGame1.Text = last30.Game1Average.ToString();
                txtGame2.Text = last30.Game2Average.ToString();
                txtGame3.Text = last30.Game3Average.ToString();
                txtGame4.Text = last30.Game4Average.ToString();
                txtScratchTotal.Text = last30.ScratchTotal.ToString();
                txtGameTotal.Text = last30.GameTotal.ToString();
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
            Bitmap bm = new(this.dataGridView1.Width, this.dataGridView1.Height);
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
                const int MONEY_WON_INDEX = 14;
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
            //grab untouched player history view models
            List<PlayerHistoryViewModel> pHist = playerHistoryRepository.GetMemberPlayerHistory(mem.Number);

            //RESTORE THE DATAGRID BACK TO THE DATE DESCINDING 
            dataGridView1.Sort(dataGridView1.Columns["Date"], System.ComponentModel.ListSortDirection.Descending);
            
            // NOTE: PlayerHistoryViewModel is read-only. Updates should be made through Game entities.
            // This section needs refactoring to update Game entities directly via GameDB
            MessageBox.Show("Save functionality needs to be updated to work with the new data model. " +
                          "Please use the Finalize Tournament form to make adjustments to game data.",
                          "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            /* TODO: Refactor this to update Game entities instead of PlayerHistory
            //if valid, store new info from slots in player history view model
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

                    //skip total score with handicap. not apart of player history view model
                    saveY++;

                    pHist[saveX].AverageForEntry = Convert.ToDouble(pHist[saveX].TotalScore / pHist[saveX].GamesPlayed);

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
            */

            //refresh page
            dataGridView1.DataSource = tableview();
        }
    }
}
