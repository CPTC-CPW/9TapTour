using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NineTapTour.Models;

namespace NineTapTour.Forms
{

    public partial class FrmFinalizeTournament : Form
    {
        #region Constant Values

        //USE THIS IF YOU NEED TO MOVE AROUND THE ORDER IN WHICH THE COLUMNS ARE DISPLAYED 
        const int STANDING_COLUMN = 0;
        const int MEMBER_NUMBER_COLUMN = 1;
        const int NAME_COLUMN = 2;
        const int GAME_1_COLUMN = 3;
        const int GAME_1_VALID_COLUMN = 4;
        const int GAME_2_COLUMN = 5;
        const int GAME_2_VALID_COLUMN = 6;
        const int GAME_3_COLUMN = 7;
        const int GAME_3_VALID_COLUMN = 8;
        const int GAME_4_COLUMN = 9;
        const int GAME_4_VALID_COLUMN = 10;
        const int SCRATCH_TOTAL_COLUMN = 11;
        const int HANDICAP_TOTAL_COLUMN = 12;
        const int ENTRY_AVERAGE_COLUMN = 13;
        const int THIRTY_ENTRY_AVERAGE_COLUMN = 14;
        const int ADJUSTED_AVG_COLUMN = 15;
        const int DIRECTOR_CHECK_COLUMN = 16;
        const int SQUAD_COLUMN = 17;
        const int HANDICAP_COLUMN = 18;
        const int BONUS_COLUMN = 19;
        const int PRO_POT_COLUMN = 20;
        const int NOTES_COLUMN_ = 21;
        const int GAME_ID_COLUMN = 22;

        //USE THIS IF YOU WANT TO CHANGE THE NAME OF EACH COLUMN 
        //Column names must be unique but the HeaderText can be changed in the DataGridView to change the text that is displayed
        const string STANDING_COLUMN_NAME = "Standing";
        const string MEMBER_NUMBER_COLUMN_NAME = "Member Number";
        const string GAME_ID_COLUMN_NAME = "GameID";
        const string NAME_COLUMN_NAME = "Name";
        const string GAME_1_COLUMN_NAME = "Game 1";
        const string GAME_1_VALID_COLUMN_NAME = "1?";
        const string GAME_2_COLUMN_NAME = "Game 2";
        const string GAME_2_VALID_COLUMN_NAME = "2?";
        const string GAME_3_COLUMN_NAME = "Game 3";
        const string GAME_3_VALID_COLUMN_NAME = "3?";
        const string GAME_4_COLUMN_NAME = "Game 4";
        const string GAME_4_VALID_COLUMN_NAME = "4?";
        const string SCRATCH_TOTAL_COLUMN_NAME = "Scratch Total";
        const string HANDICAP_TOTAL_COLUMN_NAME = "HDCP Total";
        const string ENTRY_AVERAGE_COLUMN_NAME = "Entry AVG";
        const string THIRTY_ENTRY_AVERAGE_COLUMN_NAME = "30 Entry AVG";
        const string ADJUSTED_AVG_COLUMN_NAME = "ADJ AVG";
        const string DIRECTOR_CHECK_COLUMN_NAME = "Director Check";
        const string SQUAD_COLUMN_NAME = "Squad";
        const string HANDICAP_COLUMN_NAME = "HDCP";
        const string BONUS_COLUMN_NAME = "Bonus";
        const string PRO_POT_COLUMN_NAME = "Pro Pot";
        const string NOTES_COLUMN_NAME = "Notes";

        #endregion

        private int RegionID;
        private Tournament currTournament; //current tournament

        public FrmFinalizeTournament(Tournament t, int region)
        {
            InitializeComponent();
            currTournament = t;
            RegionID = region;
        }

        private void FrmFinalizeTournament_Load(object sender, EventArgs e)
        {
#if DEBUG
            CheckBox toggleAllDirectorCheck = new CheckBox();
            toggleAllDirectorCheck.Text = "Dir Check";
            toggleAllDirectorCheck.CheckedChanged += new EventHandler(ToggleDirectorCheck_CheckChanged);
            toggleAllDirectorCheck.Location = new Point(10, 0);
            Controls.Add(toggleAllDirectorCheck);
            CheckBox toggleAllAdjustedAverages = new CheckBox();
            toggleAllAdjustedAverages.Text = "Adj Avg";
            toggleAllAdjustedAverages.CheckedChanged += new EventHandler(ToggleAllAdjustedAverages_CheckChanged);
            toggleAllAdjustedAverages.Location = new Point(120, 0);
            Controls.Add(toggleAllAdjustedAverages);
#endif
            createDataGridView(currTournament);
            InitializeGameCellFormatting();
            sizeFinalizeGridView(); // resizes the columns of finalize form
        }

#if DEBUG
        private void ToggleDirectorCheck_CheckChanged(object sender, EventArgs e)
        {
            List<FinalizeTemp> FinalizeTableList = GetListFromTable(currTournament);
            for (int i = 0; i < FinalizeTableList.Count; i++)
            {
                //if Toggle is checked, check all Director checks
                if ((sender as CheckBox).Checked)
                {
                    dataGridView1.Rows[i].Cells[DIRECTOR_CHECK_COLUMN].Value = true;
                }
                else //Toggle is unchecked, uncheck all Director Check Boxes
                {
                    dataGridView1.Rows[i].Cells[DIRECTOR_CHECK_COLUMN].Value = false;
                }
            }
        }

        private void ToggleAllAdjustedAverages_CheckChanged(object sender, EventArgs e)
        {
            bool resetAdjustedAverages = false;
            List<FinalizeTemp> FinalizeTableList = GetListFromTable(currTournament);
            for (int i = 0; i < FinalizeTableList.Count; i++)
            {
                var adjustedAverage = 
                    dataGridView1.Rows[i].Cells[ADJUSTED_AVG_COLUMN].Value;
                if (adjustedAverage.Equals(0))
                {
                    dataGridView1.Rows[i].Cells[ADJUSTED_AVG_COLUMN].Value =
                        dataGridView1.Rows[i].Cells[THIRTY_ENTRY_AVERAGE_COLUMN].Value;
                }
                else
                {
                    resetAdjustedAverages = true;
                }                
            }
            if (resetAdjustedAverages)
            {
                ResetAdjustedAverages(FinalizeTableList);
            }
        }

        private void ResetAdjustedAverages(List<FinalizeTemp> FinalizeTableList)
        {
            for (int i = 0; i < FinalizeTableList.Count; i++)
            {
                dataGridView1.Rows[i].Cells[ADJUSTED_AVG_COLUMN].Value = 0;
            }
        }
#endif      
        public void sizeFinalizeGridView() {
            int columnCount = 22;
            for (int colWidth = 0; colWidth < columnCount; colWidth++)
            {
                dataGridView1.Columns[colWidth].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }

            dataGridView1.Columns[STANDING_COLUMN].Width = 50;  
            dataGridView1.Columns[MEMBER_NUMBER_COLUMN].Width = 50;
            dataGridView1.Columns[NAME_COLUMN].Width = 150;
            dataGridView1.Columns[GAME_1_COLUMN].Width = 42;
            dataGridView1.Columns[GAME_1_VALID_COLUMN].Width = 25;
            dataGridView1.Columns[GAME_2_COLUMN].Width = 42;
            dataGridView1.Columns[GAME_2_VALID_COLUMN].Width = 25;
            dataGridView1.Columns[GAME_3_COLUMN].Width = 42;
            dataGridView1.Columns[GAME_3_VALID_COLUMN].Width = 25;
            dataGridView1.Columns[GAME_4_COLUMN].Width = 42;
            dataGridView1.Columns[GAME_4_VALID_COLUMN].Width = 25;
            dataGridView1.Columns[SCRATCH_TOTAL_COLUMN].Width = 50;
            dataGridView1.Columns[HANDICAP_TOTAL_COLUMN].Width = 50;
            dataGridView1.Columns[ENTRY_AVERAGE_COLUMN].Width = 45;
            dataGridView1.Columns[THIRTY_ENTRY_AVERAGE_COLUMN].Width = 45;
            dataGridView1.Columns[ADJUSTED_AVG_COLUMN].Width = 40;
            dataGridView1.Columns[DIRECTOR_CHECK_COLUMN].Width = 50;
            dataGridView1.Columns[SQUAD_COLUMN].Width = 40;
            dataGridView1.Columns[HANDICAP_COLUMN].Width = 35; 
            dataGridView1.Columns[BONUS_COLUMN].Width = 35;
            dataGridView1.Columns[PRO_POT_COLUMN].Width = 40;
            dataGridView1.Columns[NOTES_COLUMN_].Width = 225;
            dataGridView1.Columns[GAME_ID_COLUMN].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; 
        }

        private void createDataGridView(Tournament tourn)
        {
            List<FinalizeTemp> FinalizeTableList = GetAllInitialParticipantGameList(currTournament);

            //Below is a multithreaded version of a foreach loop to spread processing across all available cores
            Parallel.ForEach(FinalizeTableList, item =>
            {
                int gplayed = 0;
                Game g = GameDB.GetGame(item.GameId);
                FinalizeTemp temp = FinalizeTempDB.getFinalizeID(g);

                if (temp.FinalizeID <= 0)
                {
                    temp = new FinalizeTemp
                    {
                        FinalizeRegionID = RegionID,
                        FinalizeID = FinalizeTableList.Count,
                        TournamentID = tourn.Id,
                        GameId = item.GameId,
                        Squad = item.Squad,
                        AdjustedAvg = 0,
                    };
                }

                temp.Notes = item.Notes;
                temp.memberNumber = MemberDb.GetMemberNumberbyID(item.MemberId);
                temp.MemberId = item.MemberId;
                temp.FirstName = item.FirstName;
                temp.LastName = item.LastName;
                temp.Handicap = item.Handicap;
                temp.Bonus = item.Bonus;
                temp.Game1 = g.Game1;
                temp.Game2 = g.Game2;
                temp.Game3 = g.Game3;
                temp.Game4 = g.Game4;
                temp.UseGame1 = item.UseGame1;
                temp.UseGame2 = item.UseGame2;
                temp.UseGame3 = item.UseGame3;
                temp.UseGame4 = item.UseGame4;
                temp.ScratchTotal = (temp.Game1 ?? 0) + (temp.Game2 ?? 0) + (temp.Game3 ?? 0) + (temp.Game4 ?? 0);
                int hTotal = (temp.Game1.HasValue) ? ((temp.Game1 ?? 0) + temp.Bonus + temp.Handicap) : 0;
                hTotal += (temp.Game2.HasValue) ? ((temp.Game2 ?? 0) + temp.Bonus + temp.Handicap) : 0;
                hTotal += (temp.Game3.HasValue) ? ((temp.Game3 ?? 0) + temp.Bonus + temp.Handicap) : 0;
                hTotal += (temp.Game4.HasValue) ? ((temp.Game4 ?? 0) + temp.Bonus + temp.Handicap) : 0;
                temp.HandicapTotal = hTotal;
                if (item.Game1 > 0)
                {
                    gplayed++;
                }
                if (item.Game2 > 0)
                {
                    gplayed++;
                }
                if (item.Game3 > 0)
                {
                    gplayed++;
                }
                if (item.Game4 > 0)
                {
                    gplayed++;
                }
                temp.GameAvg = ((temp.Game1 ?? 0) + (temp.Game2 ?? 0) + (temp.Game3 ?? 0) + (temp.Game4 ?? 0)) / gplayed;

                //grabs running league average 
                List<PlayerHistory> ExistingPlayerHistory = PlayerHistoryDB.getMemberPlayerHistory(item.memberNumber, RegionID);
                if (ExistingPlayerHistory.Count == 0)
                {
                    temp.LeagueAverage = CalcThirtyLeagueAverage(item.memberNumber, FinalizeTableList.Where(f => f.memberNumber == item.memberNumber && f.Squad <= item.Squad && (f.UseGame1 || f.UseGame2 || f.UseGame3 || f.UseGame4)).Select(f => f.GameAvg).ToList());
                }
                else
                {
                    for (int u = 0; u < ExistingPlayerHistory.Count; u++)
                    {
                        if (ExistingPlayerHistory[u].GameID == temp.GameId)
                        {
                            break; //dont adjust the average if the PlayerHistory with said game id already exists;
                        }
                        else if (u == ExistingPlayerHistory.Count - 1) // after looking at all the history, if its not in the playerhistory list, then adjust the league avg
                        {
                            temp.LeagueAverage = CalcThirtyLeagueAverage(item.memberNumber, FinalizeTableList.Where(f => f.memberNumber == item.memberNumber && f.Squad <= item.Squad && (f.UseGame1 || f.UseGame2 || f.UseGame3 || f.UseGame4)).Select(f => f.GameAvg).ToList());
                        }
                    }
                }
                FinalizeTempDB.AddFinalizeTemp(temp);
            });

            //pulls a list from the finalizetemp table and seeds the dataview with the table info.
            List<FinalizeTemp> DataViewList = GetListFromTable(tourn);

            // Links FinalizeTemp to an integer that is placing information
            Dictionary<FinalizeTemp, int> membersPlacingMap = Calculations.Calculations.CalculatePlaceStandings(DataViewList);

            dataGridView1.DataSource = SetDataView(membersPlacingMap); //By default populates all datagrid with all participant for tournament.

            dataGridView1.Columns[GAME_ID_COLUMN].Visible = false;

            //Replace "Valid Score #?" column header with an empty string
            dataGridView1.Columns[GAME_1_VALID_COLUMN].HeaderText = string.Empty;
            dataGridView1.Columns[GAME_2_VALID_COLUMN].HeaderText = string.Empty;
            dataGridView1.Columns[GAME_3_VALID_COLUMN].HeaderText = string.Empty;
            dataGridView1.Columns[GAME_4_VALID_COLUMN].HeaderText = string.Empty;

            ////Sort DataGridView by TrueAverage
            //this.dataGridView1.Sort(this.dataGridView1.Columns["True Avg"], ListSortDirection.Descending);
#if DEBUG
            // resets the adjusted averages
            ResetAdjustedAverages(FinalizeTableList);
#endif
        }

        //creates the dataview that will populate the datagridview table on form pulls from the finalizetemp table
        // CHANGE THESE IN THE ORDER YOU WANT THEM TO BE SEEN ON THE GRID VIEW (0 == far left) AND THEN CHANGE THE STATIC INTS AT THE TOP IN ORDER TO CHANGE THERE ORDER ON THE GRID VIEW WITHOUT HAVING TO TOUNCH ANY OTHER CODE
        public DataTable SetDataView(Dictionary<FinalizeTemp,int> participantsList)
        {
            var db = new NineTapDb();
            DataTable dt = new DataTable();
            dt.Columns.Add(STANDING_COLUMN_NAME, typeof(int)); //0
            dt.Columns.Add(MEMBER_NUMBER_COLUMN_NAME, typeof(int)).ReadOnly = true; //1
            dt.Columns.Add(NAME_COLUMN_NAME, typeof(string)).ReadOnly = true; //2
            dt.Columns.Add(GAME_1_COLUMN_NAME, typeof(string)).ReadOnly = true; //3
            dt.Columns.Add(GAME_1_VALID_COLUMN_NAME, typeof(bool)); //4
            dt.Columns.Add(GAME_2_COLUMN_NAME, typeof(string)).ReadOnly = true; //5
            dt.Columns.Add(GAME_2_VALID_COLUMN_NAME, typeof(bool)); //6
            dt.Columns.Add(GAME_3_COLUMN_NAME, typeof(string)).ReadOnly = true; //7
            dt.Columns.Add(GAME_3_VALID_COLUMN_NAME, typeof(bool));//8
            dt.Columns.Add(GAME_4_COLUMN_NAME, typeof(string)).ReadOnly = true;//9
            dt.Columns.Add(GAME_4_VALID_COLUMN_NAME, typeof(bool));//10
            dt.Columns.Add(SCRATCH_TOTAL_COLUMN_NAME, typeof(int));//11
            dt.Columns.Add(HANDICAP_TOTAL_COLUMN_NAME, typeof(int));//12
            dt.Columns.Add(ENTRY_AVERAGE_COLUMN_NAME, typeof(int));//13
            dt.Columns.Add(THIRTY_ENTRY_AVERAGE_COLUMN_NAME, typeof(int));     //14  
            dt.Columns.Add(ADJUSTED_AVG_COLUMN_NAME, typeof(int)); //15
            dt.Columns.Add(DIRECTOR_CHECK_COLUMN_NAME, typeof(bool));//16
            dt.Columns.Add(SQUAD_COLUMN_NAME, typeof(int)).ReadOnly = true;//16
            dt.Columns.Add(HANDICAP_COLUMN_NAME, typeof(int)).ReadOnly = true;//17
            dt.Columns.Add(BONUS_COLUMN_NAME, typeof(int)).ReadOnly = true;//18
            dt.Columns.Add(PRO_POT_COLUMN_NAME, typeof(int));//19
            dt.Columns.Add(NOTES_COLUMN_NAME, typeof(string));//20

            dt.Columns.Add(GAME_ID_COLUMN_NAME, typeof(int)).ReadOnly = true; //21

            //whatever list of participants you pass into method will be populated into grid
            List<FinalizeTemp> temp = participantsList.Keys.ToList();

            //loops thru each person's info in tournament and populates the dataview with data from DB.
            foreach (var item in temp)
            {
                DataRow newRow = dt.NewRow();

                // 0 signifies duplicate entry
                if (participantsList[item] != 0)
                    newRow[STANDING_COLUMN_NAME] = participantsList[item];

                newRow[MEMBER_NUMBER_COLUMN_NAME] = item.memberNumber;
                newRow[NAME_COLUMN_NAME] = item.FirstName + " " + item.LastName;
                newRow[GAME_1_COLUMN_NAME] = item.Game1;
                newRow[GAME_1_VALID_COLUMN_NAME] = item.UseGame1;
                newRow[GAME_2_COLUMN_NAME] = item.Game2;
                newRow[GAME_2_VALID_COLUMN_NAME] = item.UseGame2;
                newRow[GAME_3_COLUMN_NAME] = item.Game3;
                newRow[GAME_3_VALID_COLUMN_NAME] = item.UseGame3;
                newRow[GAME_4_COLUMN_NAME] = item.Game4;
                newRow[GAME_4_VALID_COLUMN_NAME] = item.UseGame4;
                newRow[THIRTY_ENTRY_AVERAGE_COLUMN_NAME] = item.LeagueAverage;
                newRow[ENTRY_AVERAGE_COLUMN_NAME] = item.GameAvg;
                newRow[ADJUSTED_AVG_COLUMN_NAME] = item.AdjustedAvg;
                newRow[DIRECTOR_CHECK_COLUMN_NAME] = false;
                newRow[SCRATCH_TOTAL_COLUMN_NAME] = item.ScratchTotal;
                newRow[SQUAD_COLUMN_NAME] = item.Squad;
                newRow[HANDICAP_COLUMN_NAME] = item.Handicap;
                newRow[BONUS_COLUMN_NAME] = item.Bonus;
                newRow[NOTES_COLUMN_NAME] = item.Notes;
                newRow[HANDICAP_TOTAL_COLUMN_NAME] = item.HandicapTotal;
                newRow[GAME_ID_COLUMN_NAME] = item.GameId;
                dt.Rows.Add(newRow);
            }
            return dt;
        }
  
        /// <summary>
        /// THis method Gets a list of all participant objects for the tournament passed into method.
        /// </summary>
        /// <param name="tourn"> represent the tournament you want list of particpants from</param>
        /// <returns>List of Participants for specific tournament</returns>
        public List<FinalizeTemp> GetAllInitialParticipantGameList(Tournament tourn)
        {
            var db = new NineTapDb();
            List<FinalizeTemp> ParticipantList = new List<FinalizeTemp>();
            var temp = (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        join g in db.Games on p.Game.Id equals g.Id
                        join t in db.Tournaments on p.Tournament.Id equals t.Id
                        where tourn.Id == p.Tournament.Id
                        orderby m.FirstName descending
                        select new
                        {
                            g.Id,
                            m.FirstName,
                            m.LastName,
                            MemberId = m.Id,
                            p.Squad,
                            g.Game1,
                            g.Game2,
                            g.Game3,
                            g.Game4,
                            g.UseGame1,
                            g.UseGame2,
                            g.UseGame3,
                            g.UseGame4,
                            g.Notes,
                            g.Handicap,
                            g.Bonus,
                            m.Number,
                            t.TourneyRegion
                        }).ToList();
            foreach (var item in temp)
            {
                int gplayed = 0;
                FinalizeTemp NewParticipant = new FinalizeTemp();
                NewParticipant.GameId = item.Id;
                NewParticipant.MemberId = item.MemberId;
                NewParticipant.FirstName = item.FirstName;
                NewParticipant.LastName = item.LastName;
                NewParticipant.Game1 = item.Game1;
                NewParticipant.Game2 = item.Game2;
                NewParticipant.Game3 = item.Game3;
                NewParticipant.Game4 = item.Game4;

                NewParticipant.UseGame1 = item.UseGame1 ?? item.Game1.HasValue;
                NewParticipant.UseGame2 = item.UseGame2 ?? item.Game2.HasValue;
                NewParticipant.UseGame3 = item.UseGame3 ?? item.Game3.HasValue;
                NewParticipant.UseGame4 = item.UseGame4 ?? item.Game4.HasValue;

                if (item.Game1.HasValue)
                {
                    gplayed++;
                }

                if (item.Game2.HasValue)
                {
                    gplayed++;
                }

                if (item.Game3.HasValue)
                {
                    gplayed++;
                }

                if (item.Game4.HasValue)
                {
                    gplayed++;
                }

                NewParticipant.Notes = item.Notes;
                NewParticipant.ScratchTotal = (item.Game1 ?? 0) + (item.Game2 ?? 0) + (item.Game3 ?? 0) + (item.Game4 ?? 0);
                NewParticipant.Squad = item.Squad;
                NewParticipant.GameAvg = ((item.Game1 ?? 0) + (item.Game2 ?? 0) + (item.Game3 ?? 0) + (item.Game4 ?? 0)) / gplayed;
                NewParticipant.Handicap = item.Handicap ?? 0;
                NewParticipant.Bonus = item.Bonus ?? 0;

                int hTotal = (item.Game1 != null) ? ((item.Game1 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0)) : 0;
                hTotal += (item.Game2 != null) ? ((item.Game2 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0)) : 0;
                hTotal += (item.Game3 != null) ? ((item.Game3 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0)) : 0;
                hTotal += (item.Game4 != null) ? ((item.Game4 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0)) : 0;

                NewParticipant.HandicapTotal = hTotal;

                NewParticipant.memberNumber = item.Number;
                NewParticipant.FinalizeRegionID = item.TourneyRegion;

                ParticipantList.Add(NewParticipant);
            }

            return ParticipantList;
        }

        //makes a list from the finalizetemp table to be used in dataview source
        public List<FinalizeTemp> GetListFromTable(Tournament tourn)
        {
            var db = new NineTapDb();
            //get list of participants by tournament
            return db.FinalizeTemp
                            .Where(p => p.TournamentID == tourn.Id)
                            .OrderBy(p => p.FirstName)
                            .ThenBy(p => p.Squad)
                            .ToList();
        }


        /// <summary>
        /// This method handles the changes made when any GAME_VALID or DIRECTOR_CHECK checkboxes are changed, including updating the FinalizeTemp table in the DB.
        /// The DataGridView.CellValueChanged event occurs when the user-specified value is committed, which typically occurs when focus leaves the cell.
        /// </summary>
        private void dataGridView1_OnCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Check if cell changed was a GAME_VALID cell
                if (e.ColumnIndex == GAME_1_VALID_COLUMN ||
                    e.ColumnIndex == GAME_2_VALID_COLUMN ||
                    e.ColumnIndex == GAME_3_VALID_COLUMN ||
                    e.ColumnIndex == GAME_4_VALID_COLUMN)
                {
                    DataGridViewCell clickedCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    bool isCellChecked = Convert.ToBoolean(clickedCell.Value);
                    UpdateAvg(clickedCell.RowIndex);
                    UpdateLeagueAvg(e.RowIndex);
                    CheckBoxDBSet(clickedCell.RowIndex, clickedCell.ColumnIndex, isCellChecked);
                    SetGameCellFormatting(GetCorrespondingGameCell(clickedCell), isCellChecked);
                    dataGridView1_CellClick(null, null);
                }

                // Check if cell changed was a DIRECTOR_CHECK cell
                // If the DIRECTOR_CHECK cell was clicked, 
                else if (e.ColumnIndex == DIRECTOR_CHECK_COLUMN)
                {

                    //Grabs the cell that contains the Adjust avg 
                    int adjustedAverage = Convert.ToInt32(dataGridView1[ADJUSTED_AVG_COLUMN, e.RowIndex].Value);

                    //if true sets to backcolor to white automatically if director check button gets checked after entering number
                    if (adjustedAverage > 0)
                    {
                        dataGridView1[ADJUSTED_AVG_COLUMN, e.RowIndex].Style.BackColor = Color.White;
                        dataGridView1[DIRECTOR_CHECK_COLUMN, e.RowIndex].Style.BackColor = Color.White;
                    }


                    //this code changes all of that member's games to match the clicked DIRECTOR_CHECK.
                    int memberNum = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[MEMBER_NUMBER_COLUMN].Value);
                    bool isCellChecked = Convert.ToBoolean(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);

                    DataRow[] rows = ((DataTable)dataGridView1.DataSource).Select(String.Format("[{0}] = {1}", MEMBER_NUMBER_COLUMN_NAME, memberNum));

                    foreach (DataRow row in rows)
                    {
                        row.SetField(DIRECTOR_CHECK_COLUMN, isCellChecked);
                    }
                }
            }
        }

        /// <summary>
        /// This method updates the 30 Game League Average for the game associated with the rowIndex passed in,
        /// and the games for the same member, that come cronologically after that game.
        /// </summary>
        private void UpdateLeagueAvg(int rowIndex)
        {
            int memberNum = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells[MEMBER_NUMBER_COLUMN].Value);
            int initialSquadNum = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells[SQUAD_COLUMN].Value);

            // This method queries the DataGridView for the rows belonging to the same member, as the passed in rowIndex.
            DataRow[] rows = ((DataTable)dataGridView1.DataSource).Select(String.Format("[{0}] = {1}", MEMBER_NUMBER_COLUMN_NAME, memberNum));

            foreach (DataRow row in rows)
            {
                int squadNum = row.Field<int>(SQUAD_COLUMN_NAME);
                // If the squad number is equal to or greater than the passed in row's squad number, it needs to be updated.
                if (squadNum >= initialSquadNum)
                {
                    // This list is required by the CalcThirtyLeagueAverage method. It is the Game Averages from the current game, and all the games previous in the current tournament.
                    List<int> previousGameAverages = rows
                        .Where(r => r.Field<int>(SQUAD_COLUMN_NAME) <= squadNum && (r.Field<bool>(GAME_1_VALID_COLUMN_NAME) || r.Field<bool>(GAME_2_VALID_COLUMN_NAME) || r.Field<bool>(GAME_3_VALID_COLUMN_NAME) || r.Field<bool>(GAME_4_VALID_COLUMN_NAME)))
                        .Select(r => r.Field<int>(ENTRY_AVERAGE_COLUMN_NAME))
                        .ToList();
                    row.SetField(THIRTY_ENTRY_AVERAGE_COLUMN, CalcThirtyLeagueAverage(memberNum, previousGameAverages));
                }
            }
        }

        /// <summary>
        /// This method checks if the clicked column is an "Is Game Valid?" or "Director Check" column, and fires the EndEdit() method on the data grid view.
        /// If the EndEdit() method isn't called, the data grid view wouldn't see the column as edited until the user click "out" of the cell.
        /// The DataGridView.CellMouseUp event fires when the user releases a mouse button while over a cell.
        /// </summary>
        private void dataGridView1_OnCellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 &&
                (e.ColumnIndex == GAME_1_VALID_COLUMN ||
                e.ColumnIndex == GAME_2_VALID_COLUMN ||
                e.ColumnIndex == GAME_3_VALID_COLUMN ||
                e.ColumnIndex == GAME_4_VALID_COLUMN ||
                e.ColumnIndex == DIRECTOR_CHECK_COLUMN))
            {
                dataGridView1.EndEdit();
            }
        }

        /// <summary>
        /// Checks the bool state of the check box and updates row based off gameID in FinalizeTempDB
        /// </summary>
        /// <param name="row"></param> row that is being checked
        /// <param name="cell"></param> cell that is being checked
        /// <param name="set"></param> setting UseGame bool flag in FinalizeTemp table to true or false
        private void CheckBoxDBSet(int row, int cell, bool set)
        {
            NineTapDb db = new NineTapDb();
            FinalizeTemp temp = new FinalizeTemp();
            var GameId = Convert.ToInt32(dataGridView1.Rows[row].Cells[GAME_ID_COLUMN].Value);
            temp = db.FinalizeTemp.First(f => f.GameId == GameId);

            if (cell == GAME_1_COLUMN)
                temp.UseGame1 = set;

            if (cell == GAME_2_COLUMN)
                temp.UseGame2 = set;

            if (cell == GAME_3_COLUMN)
                temp.UseGame3 = set;

            if (cell == GAME_4_COLUMN)
                temp.UseGame4 = set;

            temp.GameAvg = Convert.ToInt32(dataGridView1.Rows[row].Cells[ENTRY_AVERAGE_COLUMN].Value);
            temp.ScratchTotal = Convert.ToInt32(dataGridView1.Rows[row].Cells[SCRATCH_TOTAL_COLUMN].Value);
            temp.HandicapTotal = Convert.ToInt32(dataGridView1.Rows[row].Cells[HANDICAP_TOTAL_COLUMN].Value);
            temp.LeagueAverage = Convert.ToInt32(dataGridView1.Rows[row].Cells[THIRTY_ENTRY_AVERAGE_COLUMN].Value);
            db.Entry(temp).State = EntityState.Modified;
            db.SaveChanges();
            this.dataGridView1.CellValueChanged += this.dataGridView1_OnCellValueChanged;
        }

        //updates computed average in column 10 when check box is changed.
        private void UpdateAvg(int row)
        {
            this.dataGridView1.CellValueChanged -= this.dataGridView1_OnCellValueChanged;
            int sum = 0;
            int sumAndHand = 0;
            int count = 0;
            int sumWHandicap = 0;
            int HDCPwithBonus = Convert.ToInt32((dataGridView1.Rows[row].Cells[HANDICAP_COLUMN].Value)) + Convert.ToInt32((dataGridView1.Rows[row].Cells[BONUS_COLUMN].Value));
            if (Convert.ToBoolean(dataGridView1.Rows[row].Cells[GAME_1_VALID_COLUMN].Value) == true)
            {
                sum += Convert.ToInt32((dataGridView1.Rows[row].Cells[GAME_1_COLUMN].Value));
                sumAndHand += Convert.ToInt32((dataGridView1.Rows[row].Cells[GAME_1_COLUMN].Value));
                sumWHandicap = sumAndHand += HDCPwithBonus;
                count++;
            }

            if (Convert.ToBoolean(dataGridView1.Rows[row].Cells[GAME_2_VALID_COLUMN].Value) == true)
            {
                sum += Convert.ToInt32((dataGridView1.Rows[row].Cells[GAME_2_COLUMN].Value));
                sumAndHand += Convert.ToInt32((dataGridView1.Rows[row].Cells[GAME_2_COLUMN].Value));
                sumWHandicap = sumAndHand += HDCPwithBonus;
                count++;
            }

            if (Convert.ToBoolean(dataGridView1.Rows[row].Cells[GAME_3_VALID_COLUMN].Value) == true)
            {
                sum += Convert.ToInt32((dataGridView1.Rows[row].Cells[GAME_3_COLUMN].Value));
                sumAndHand += Convert.ToInt32((dataGridView1.Rows[row].Cells[GAME_3_COLUMN].Value));
                sumWHandicap = sumAndHand += HDCPwithBonus;
                count++;
            }

            if (Convert.ToBoolean(dataGridView1.Rows[row].Cells[GAME_4_VALID_COLUMN].Value) == true)
            {
                sum += Convert.ToInt32((dataGridView1.Rows[row].Cells[GAME_4_COLUMN].Value));
                sumAndHand += Convert.ToInt32((dataGridView1.Rows[row].Cells[GAME_4_COLUMN].Value));
                sumWHandicap = sumAndHand += HDCPwithBonus;
                count++;
            }

            if (count == 0)
            {
                dataGridView1.Rows[row].Cells[ENTRY_AVERAGE_COLUMN].Value = 0;
                dataGridView1.Rows[row].Cells[SCRATCH_TOTAL_COLUMN].Value = 0;
                dataGridView1.Rows[row].Cells[HANDICAP_TOTAL_COLUMN].Value = 0;

            }
            else
            {
                dataGridView1.Rows[row].Cells[ENTRY_AVERAGE_COLUMN].Value = sum / count;
                dataGridView1.Rows[row].Cells[SCRATCH_TOTAL_COLUMN].Value = sum;
                dataGridView1.Rows[row].Cells[HANDICAP_TOTAL_COLUMN].Value = sumWHandicap;
            }
        }

        //calculates league average for member based off last 30 games or total games played if less than 30.
        public double LeagueAverage(int memID)
        {
            double sum = 0;
            double average = 0;
            var db = new NineTapDb();
            var temp = (
                        from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        join g in db.Games on p.Game.Id equals g.Id
                        join t in db.Tournaments on p.Tournament.Id equals t.Id
                        where memID == m.Id
                        orderby t.Date descending
                        select new
                        {
                            t.Date,
                            g.Game1,
                            g.Game2,
                            g.Game3,
                            g.Game4,
                            Average = (g.Game1 + g.Game2 + g.Game3 + g.Game4) / 4
                        }).Take(30).ToList();

            if (temp.Count > 0)
            {
                foreach (var item in temp)
                {
                    sum += Convert.ToDouble(item.Average);
                }
                return (average = sum / temp.Count());
            }
            return 0;
        }

        public double LeagueAvgFromPlayerHistory(int mem, int howmany, int regionid)
        {
            double sum = 0;
            var db = new NineTapDb();
            var temp = (from p in db.PlayerHistory
                        where p.MemberNumber == mem && p.regionID == regionid
                        orderby p.TournamentDate descending
                        select new
                        {
                            p.TournamentDate,
                            p.Game1,
                            p.Game2,
                            p.Game3,
                            p.Game4,
                            p.trueAVG,
                            p.AverageForGame
                        }).Take(howmany).ToList();

            if (temp.Count > 0)
            {

                foreach (var item in temp)
                {
                    sum += Convert.ToDouble(item.AverageForGame);
                }
                return sum;
            }
            return 0;
        }

        public void RankGridView()
        {
            int Rank = 1;

            for (int Row = 0; Row < dataGridView1.Rows.Count; Row++)
            {
                dataGridView1.Rows[Row].Cells[GAME_ID_COLUMN].Value = Rank;
                //Here we are updatng placestandings by adjustedAvg. Should we Rank by trueavg?
                if (Convert.ToInt32(dataGridView1.Rows[Row].Cells[ADJUSTED_AVG_COLUMN].Value) != Convert.ToInt32(dataGridView1.Rows[Row + 1].Cells[ADJUSTED_AVG_COLUMN].Value))
                {
                    Rank++;
                }
            }
        }

        /// <summary>
        /// This method will get a list of all tournament participants and return a sort the list by scores.
        /// </summary>
        /// <param name="tourn">Tournament needing information from</param>
        /// <returns>sorted list of gameParticipants for specified tournament</returns>
        public void SortByScore()
        {
            this.dataGridView1.Sort(this.dataGridView1.Columns["True Avg"], ListSortDirection.Descending);
            RankGridView();
        }

        /// <summary>
        /// This method takes in a GAME_VALID_COLUMN cell and returns the correct corresponding GAME_COLUMN cell or vis versa.
        /// </summary>
        /// <param name="cell">A DataGridViewCell of either GAME_COLUMN or GAME_VALID_COLUMN type.</param>
        /// <returns>The corresponding DataGridViewCell to the passed in GAME DataGridViewCell.</returns>
        private DataGridViewCell GetCorrespondingGameCell(DataGridViewCell cell)
        {
            switch (cell.ColumnIndex)
            {
                case GAME_1_COLUMN:
                    return dataGridView1.Rows[cell.RowIndex].Cells[GAME_1_VALID_COLUMN];
                case GAME_2_COLUMN:
                    return dataGridView1.Rows[cell.RowIndex].Cells[GAME_2_VALID_COLUMN];
                case GAME_3_COLUMN:
                    return dataGridView1.Rows[cell.RowIndex].Cells[GAME_3_VALID_COLUMN];
                case GAME_4_COLUMN:
                    return dataGridView1.Rows[cell.RowIndex].Cells[GAME_4_VALID_COLUMN];
                case GAME_1_VALID_COLUMN:
                    return dataGridView1.Rows[cell.RowIndex].Cells[GAME_1_COLUMN];
                case GAME_2_VALID_COLUMN:
                    return dataGridView1.Rows[cell.RowIndex].Cells[GAME_2_COLUMN];
                case GAME_3_VALID_COLUMN:
                    return dataGridView1.Rows[cell.RowIndex].Cells[GAME_3_COLUMN];
                case GAME_4_VALID_COLUMN:
                    return dataGridView1.Rows[cell.RowIndex].Cells[GAME_4_COLUMN];
                default:
                    return null;
            }
        }

        /// <summary>
        /// This method iterates over every row in dataGridView1 and sets the formatting of the game cells appropriately.
        /// </summary>
        private void InitializeGameCellFormatting()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                SetGameCellFormatting(row.Cells[GAME_1_COLUMN], Convert.ToBoolean(row.Cells[GAME_1_VALID_COLUMN].Value));
                SetGameCellFormatting(row.Cells[GAME_2_COLUMN], Convert.ToBoolean(row.Cells[GAME_2_VALID_COLUMN].Value));
                SetGameCellFormatting(row.Cells[GAME_3_COLUMN], Convert.ToBoolean(row.Cells[GAME_3_VALID_COLUMN].Value));
                SetGameCellFormatting(row.Cells[GAME_4_COLUMN], Convert.ToBoolean(row.Cells[GAME_4_VALID_COLUMN].Value));
            }
        }

        /// <summary>
        /// This method sets the formatting of a game cell to either a valid or invalid state.
        /// Valid format depends on the value of the game compared to the member's thirty game average.
        /// Invalid format is strikeout font style with a red background.
        /// </summary>
        /// <param name="cell">The GAME_COLUMN cell to set the state of.</param>
        /// <param name="isGameCellValid">The state to set the cell to.</param>

        private void SetGameCellFormatting(DataGridViewCell cell, bool isGameCellValid)
        {
            if (isGameCellValid && Int32.TryParse(cell.Value.ToString(), out int gameValue))
            {
                //Sets the style back to default
                cell.Style.Font = null;

                // Check the game's value compared to the member's past thirty games average.
                int thirtyAvg = Convert.ToInt32(dataGridView1.Rows[cell.RowIndex].Cells[THIRTY_ENTRY_AVERAGE_COLUMN].Value);
                if (gameValue > thirtyAvg - 50)
                {
                    // If the value is within acceptable parameters, change the background back to white.
                    cell.Style.BackColor = (cell.RowIndex % 2 == 0) ? Color.White : Color.LightGray;
                }
                else
                {
                    // If the value is <= 50 below their thirty games average, set the background color to orange.
                    cell.Style.BackColor = Color.Orange;
                }
            }
            else
            {
                cell.Style.BackColor = Color.Red;
            }
        }
        

        private void RefreshMemberView(List<PlayerHistory> temporary)
        {
            DataTable dtGames = new DataTable();

            dtGames.Columns.Add("Games").ReadOnly = true;
            dtGames.Columns.Add("Date", typeof(DateTime));
            dtGames.Columns.Add("Game1");
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

            // Money Won label string is referenced multiple locations
            string moneyWon = "Money Won";
            decimal totalMoneyEarned = 0;

            foreach (var item in temporary)
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
                newRow["Game Total w/HDCP"] = item.TotalScore + ((item.HandiCap + item.Bonus) * item.GamesPlayed);
                newRow["Entry AVG"] = item.AverageForGame;
                newRow["30 Entry AVG"] = item.trueAVG;

                if (item.AVG == 0)
                    newRow["Adjusted AVG"] = null;
                else
                    newRow["Adjusted AVG"] = item.AVG;

                newRow["Handicap"] = item.HandiCap;
                newRow["Bonus"] = item.Bonus;
                newRow["Pro Pot"] = item.ProPot;
                newRow[moneyWon] = item.MoneyWon;
                newRow["Place"] = item.PPHG;
                newRow["Notes"] = item.Notes;
                newRow["GameID"] = item.GameID;

                dtGames.Rows.Add(newRow);

                // To know total to add to the Money Won heading label
                totalMoneyEarned += item.MoneyWon;
            }

            string moneyWonWithTotal = $"{moneyWon} ({totalMoneyEarned + PlayerHistoryDB.GetTotalMoneyWon(temporary[0].MemberNumber, RegionID)})";
            dtGames.Columns[moneyWon].ColumnName = moneyWonWithTotal;

            List<PlayerHistory> currentHistory = PlayerHistoryDB.getMemberPlayerHistoryCount(temporary[0].MemberNumber, RegionID);

            foreach (var item in currentHistory)
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
                newRow["Game Total w/HDCP"] = item.TotalScore + ((item.HandiCap + item.Bonus) * item.GamesPlayed);
                newRow["Entry AVG"] = Convert.ToDouble((item.Game1 + item.Game2 + item.Game3 + item.Game4) / item.GamesPlayed);
                newRow["30 Entry AVG"] = item.trueAVG;

                if (item.AVG == 0)
                    newRow["Adjusted AVG"] = null;
                else
                    newRow["Adjusted AVG"] = item.AVG;

                newRow["Handicap"] = item.HandiCap;
                newRow["Bonus"] = item.Bonus;
                newRow["Pro Pot"] = item.ProPot;
                newRow[moneyWonWithTotal] = item.MoneyWon;
                newRow["Place"] = item.PPHG;
                newRow["Notes"] = item.Notes;
                newRow["GameID"] = item.GameID;

                dtGames.Rows.Add(newRow);
            }

            dataGridView2.DataSource = dtGames;


            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                for (int t = 0; t < temporary.Count; t++)
                {
                    if (temporary[i].GameID == Convert.ToInt32(dataGridView2.Rows[i].Cells[17].Value))
                    {
                        for (int r = 0; r < dataGridView2.ColumnCount; r++)
                        {
                            dataGridView2.Rows[i].Cells[r].Style.BackColor = Color.LightBlue;
                        }
                    }
                }

                for (int j = 0; j < dataGridView2.RowCount; j++)
                {
                    dataGridView2.Rows[j].Cells[9].Style.BackColor = Color.GreenYellow;
                }
            }
        }

        /// <summary>
        /// This method populates the second DataGridView with information about the player associated with the cell that triggered the event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //set name, member score and currentr avg based of of what row is selected.
            int gameId = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[GAME_ID_COLUMN].Value);
            using (var db = new NineTapDb())
            {
                //set labels to selected index
                int memberNumber = db.Participants.Include(b => b.Game).Include(b => b.Member).First(p => p.Game.Id == gameId).Member.Number;
                Member Cmember = MemberDb.GetMember(memberNumber, RegionID);
                lblMemberNumber.Text = Cmember.Number.ToString();
                lblName.Text = Cmember.FirstName + " " + Cmember.LastName;
                lblStartAvg.Text = Cmember.StartAvg.ToString();

                try
                {
                    List<PlayerHistory> temporary = new List<PlayerHistory>();

                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        int tempMemberNumber = Convert.ToInt32(dataGridView1.Rows[i].Cells[MEMBER_NUMBER_COLUMN].Value);
                        if (tempMemberNumber == memberNumber)
                        {
                            PlayerHistory p = new PlayerHistory();

                            p.MemberNumber = tempMemberNumber;
                            int tempgameplayed = 0;

                            if (Convert.ToBoolean(dataGridView1[GAME_1_VALID_COLUMN, i].Value))
                            {
                                tempgameplayed++;
                                p.Game1 = Convert.ToInt32(dataGridView1.Rows[i].Cells[GAME_1_COLUMN].Value);
                            }
                            else
                            {
                                p.Game1 = 0;

                            }

                            if (Convert.ToBoolean(dataGridView1[GAME_2_VALID_COLUMN, i].Value))
                            {
                                tempgameplayed++;
                                p.Game2 = Convert.ToInt32(dataGridView1.Rows[i].Cells[GAME_2_COLUMN].Value);

                            }
                            else
                            {
                                p.Game2 = 0;
                            }

                            if (Convert.ToBoolean(dataGridView1[GAME_3_VALID_COLUMN, i].Value))
                            {
                                tempgameplayed++;
                                p.Game3 = Convert.ToInt32(dataGridView1.Rows[i].Cells[GAME_3_COLUMN].Value);
                            }
                            else
                            {
                                p.Game3 = 0;

                            }

                            if (Convert.ToBoolean(dataGridView1[GAME_4_VALID_COLUMN, i].Value))
                            {
                                tempgameplayed++;
                                p.Game4 = Convert.ToInt32(dataGridView1.Rows[i].Cells[GAME_4_COLUMN].Value);
                            }
                            else
                            {
                                p.Game4 = 0;
                            }

                            p.GamesPlayed = tempgameplayed;
                            p.TournamentDate = currTournament.Date;
                            p.GameID = Convert.ToInt32(dataGridView1.Rows[i].Cells[GAME_ID_COLUMN].Value);

                            p.TotalScore = Convert.ToInt32(dataGridView1.Rows[i].Cells[SCRATCH_TOTAL_COLUMN].Value);
                            p.HandiCap = Convert.ToInt32(dataGridView1.Rows[i].Cells[HANDICAP_COLUMN].Value);
                            p.Bonus = Convert.ToInt32(dataGridView1.Rows[i].Cells[BONUS_COLUMN].Value);
                            p.MoneyWon = Convert.ToDecimal(GameDB.GetGame(gameId).MoneyWon);
                            p.PPHG = Convert.ToString(dataGridView1.Rows[i].Cells[STANDING_COLUMN].Value);
                            p.ProPot = dataGridView1[PRO_POT_COLUMN, i].Value.ToString();
                            p.Notes = dataGridView1[NOTES_COLUMN_, i].Value.ToString();
                            p.AverageForGame = Convert.ToDouble(dataGridView1[ENTRY_AVERAGE_COLUMN, i].Value);
                            p.trueAVG = Convert.ToInt32(dataGridView1.Rows[i].Cells[THIRTY_ENTRY_AVERAGE_COLUMN].Value);
                            p.AVG = Convert.ToInt32(dataGridView1[ADJUSTED_AVG_COLUMN, i].Value);

                            temporary.Add(p);
                        }
                    }

                    temporary.Reverse();
                    RefreshMemberView(temporary);
                }
                catch
                {
                    //catches the instance where cells technically do not exist. will not refresh if they dont exist yet.
                }
            }
        }

        private void btnFinalize_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            
            
            bool isDirectorCheckFinished = true; //int used to make sure all the director check boxes have been filled out

            List<FinalizeTemp> FinalizeTableList = GetListFromTable(currTournament);
            int gamesPlayed = 0;

            //checks to make sure all the director had adjusted avgs and checked the box to make sure they did so.
            for (int i = 0; i < FinalizeTableList.Count; i++)
            {
                //Gets avg from Adjust average column
                int adjustedAverage = Convert.ToInt32(dataGridView1[ADJUSTED_AVG_COLUMN, i].Value);

                //if true changes background color to red and doesn't submit
                if (adjustedAverage == 0)
                {
                    dataGridView1.Rows[i].Cells[ADJUSTED_AVG_COLUMN].Style.BackColor = Color.Red;
                    isDirectorCheckFinished = false;
                }
                //if director checkbox is checked set to white and continue
                if (Convert.ToBoolean(dataGridView1[DIRECTOR_CHECK_COLUMN, i].Value))
                {
                    dataGridView1.Rows[i].Cells[DIRECTOR_CHECK_COLUMN].Style.BackColor = (i % 2 == 0) ? Color.White : Color.LightGray;
                }
                else
                {
                    dataGridView1.Rows[i].Cells[DIRECTOR_CHECK_COLUMN].Style.BackColor = Color.Red;
                    isDirectorCheckFinished = false;
                }
            }

            //START FINALIZATION
            if (isDirectorCheckFinished) //if all the director check boxes are selected
            {
                // total comp entries for the current tournament
                int compEntriesQty = FinalizeTempDB.GetCompEntryQtyByTourney(currTournament.Id);

                //Multithreaded version of a for loop, spreads processing across all available cores
                Parallel.For(0, FinalizeTableList.Count, i =>
                {
                    gamesPlayed = 0;
                    int currGameId = FinalizeTableList[i].GameId;

                    PlayerHistory ph = new PlayerHistory();
                    ph.GameID = currGameId;

                    Game currGame = GameDB.GetGame(currGameId);
                    Member currMember = MemberDb.GetMemberByGameId(currGameId);

                    ph.TournamentDate = currTournament.Date;
                    ph.MemberNumber = currMember.Number;

                    int currDataGridRowIndex = FindCurrDataGridRowIndex(currGameId);

                    if (dataGridView1[GAME_1_VALID_COLUMN, currDataGridRowIndex].Value.ToString() == "True")
                    {
                        gamesPlayed++;
                        currGame.UseGame1 = true;
                        FinalizeTableList[i].UseGame1 = true;
                    }
                    else
                    {
                        currGame.UseGame1 = false;
                        FinalizeTableList[i].UseGame1 = false;
                    }

                    if (dataGridView1[GAME_2_VALID_COLUMN, currDataGridRowIndex].Value.ToString() == "True")
                    {
                        gamesPlayed++;
                        currGame.UseGame2 = true;
                        FinalizeTableList[i].UseGame2 = true;
                    }
                    else
                    {
                        currGame.UseGame2 = false;
                        FinalizeTableList[i].UseGame2 = false;
                    }

                    if (dataGridView1[GAME_3_VALID_COLUMN, currDataGridRowIndex].Value.ToString() == "True")
                    {
                        gamesPlayed++;
                        currGame.UseGame3 = true;
                        FinalizeTableList[i].UseGame3 = true;
                    }
                    else
                    {
                        currGame.UseGame3 = false;
                        FinalizeTableList[i].UseGame3 = false;
                    }

                    if (dataGridView1[GAME_4_VALID_COLUMN, currDataGridRowIndex].Value.ToString() == "True")
                    {
                        gamesPlayed++;
                        currGame.UseGame4 = true;
                        FinalizeTableList[i].UseGame4 = true;
                    }
                    else
                    {
                        currGame.UseGame4 = false;
                        FinalizeTableList[i].UseGame4 = false;
                    }

                    ph.GamesPlayed = gamesPlayed;
                    ph.AverageForGame = FinalizeTableList[i].GameAvg;
                    ph.trueAVG = FinalizeTableList[i].LeagueAverage;


                    ph.AVG = Convert.ToInt32(dataGridView1[ADJUSTED_AVG_COLUMN, currDataGridRowIndex].Value);

                    ph.ProPot = dataGridView1[PRO_POT_COLUMN, currDataGridRowIndex].Value.ToString();

                    ph.MoneyWon = Convert.ToDecimal(currGame.MoneyWon);

                    ph.Game1 = FinalizeTableList[i].Game1;
                    ph.Game2 = FinalizeTableList[i].Game2;
                    ph.Game3 = FinalizeTableList[i].Game3;
                    ph.Game4 = FinalizeTableList[i].Game4;

                    DataGridViewCell placeCell = dataGridView1[STANDING_COLUMN, currDataGridRowIndex];
                    byte placeStanding = (placeCell.Value == DBNull.Value) ? (byte) 0 : Convert.ToByte(placeCell.Value);

                    // if bowler's highest game in tournament (only lower scoring duplicates get 0s)
                    if (placeStanding > 0)
                    {
                        currGame.PlaceStanding = Convert.ToByte(placeStanding);
                        ph.PPHG = placeStanding.ToString();

                        // Adjust bonus pins only if game has not been finalized previously
                        if (!PlayerHistoryDB.PlayerHistoryExists(currGame.Id))
                        {
                            currMember.Bonus = Calculations.Calculations.GetAdjustedBonusPins(placeStanding, FinalizeTableList.Count, compEntriesQty,
                                                                                        currMember.Bonus, currMember.Number, RegionID, currTournament.Date);
                            ph.Bonus = currMember.Bonus;
                        }
                    }

                    ph.HandiCap = FinalizeTableList[i].Handicap;
                    currGame.InputtedAvg = ph.AVG;
                    currGame.Notes = dataGridView1[NOTES_COLUMN_, currDataGridRowIndex].Value.ToString();
                    ph.Notes = currGame.Notes;
                    currMember.StartAvg = ph.AVG;
                    ph.hisID = PlayerHistoryDB.getHisID(ph);
                    ph.regionID = RegionID;
                    currGame.gameRegionID = RegionID;

                    PlayerHistoryDB.AddPlayerHistory2(ph);
                    GameDB.AddOrUpdateGame(currGame);
                    MemberDb.AddOrUpdateMember(currMember);

                    FinalizeTableList[i].FinalizeID = FinalizeTempDB.getFinalizeID(currGame).FinalizeID;
                    FinalizeTableList[i].AdjustedAvg = ph.AVG;
                    FinalizeTableList[i].HandicapTotal = Convert.ToInt32(dataGridView1[HANDICAP_TOTAL_COLUMN, currDataGridRowIndex].Value);

                    FinalizeTempDB.AddFinalizeTemp(FinalizeTableList[i]);
                });

                Close();
            }
            else  // if all of the director checkboxes are not checked, then prompt user to check to finalize tournament
            {

            }
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Gets the row of data grid by the Game Id value stored in that row.
        /// Returns -1 if not found.
        /// </summary>
        /// <param name="currGameId"></param>
        /// <returns>The row index of the Game Id</returns>
        private int FindCurrDataGridRowIndex(int currGameId)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCell gameIdCell = row.Cells[GAME_ID_COLUMN];
                if (Convert.ToInt32(gameIdCell.Value) == currGameId)
                {
                    return gameIdCell.RowIndex;
                }
            }
            return -1;
        }

        /// <summary>
        /// This function takes the averages from the current games being finalized, grabs the appropriate amount of player history, and calculates the 30 game average for the player.
        /// </summary>
        /// <param name="memberNum">The Member Number of the player whose averages we are calculating.</param>
        /// <param name="currGameAverages">A list of the averages from the current games being finalized.</param>
        /// <returns></returns>
        private int CalcThirtyLeagueAverage(int memberNum, List<int> currGameAverages)
        {
            if (currGameAverages.Count > 0)
            {
                List<PlayerHistory> playerHistory = PlayerHistoryDB.getMemberPlayerHistory(memberNum, RegionID);
                int sumOfAllGameAverages = Convert.ToInt32(LeagueAvgFromPlayerHistory(memberNum, 30 - currGameAverages.Count, RegionID) + currGameAverages.Sum());

                if (playerHistory.Count >= 30 || (playerHistory.Count + currGameAverages.Count) > 30)
                {
                    return sumOfAllGameAverages / 30;
                }
                else
                {
                    return sumOfAllGameAverages / (playerHistory.Count + currGameAverages.Count);
                }
            }
            else
            {
                throw new ArgumentException("currGameAverages.Count was 0. This function calculates the avg from player history and the current games.");
            }
        }

        public void getLeagueSum(FinalizeTemp temp, List<FinalizeTemp> finalizeTableList)
        {
            //RUNNING LEAGUE AVG 
            int SumFromGamesNotAddedYet = 0;

            //checks to see if they bowled an any squads before the current selected squad, if your on this line then they bowled at leats once
            temp.memberNumber = MemberDb.GetMemberNumberbyID(temp.MemberId);
            List<PlayerHistory> p = PlayerHistoryDB.getMemberPlayerHistory(temp.memberNumber, RegionID);
            int howmanyTimesdidheybowlbeforethissquad = 1;

            for (int f = 0; f < finalizeTableList.Count; f++)
            {
                if (temp.MemberId == finalizeTableList[f].MemberId && finalizeTableList[f].Squad < temp.Squad)
                {
                    howmanyTimesdidheybowlbeforethissquad++;
                }
            }

            if (temp.Squad > 1)
            {
                for (int i = 0; i < finalizeTableList.Count; i++)
                {
                    if (temp.MemberId == finalizeTableList[i].MemberId && finalizeTableList[i].Squad < temp.Squad)
                    {
                        SumFromGamesNotAddedYet += finalizeTableList[i].GameAvg;
                    }
                }
            }

            temp.LeagueAverage = Convert.ToInt32(LeagueAvgFromPlayerHistory(temp.memberNumber, 30 - howmanyTimesdidheybowlbeforethissquad, RegionID) + SumFromGamesNotAddedYet + temp.GameAvg);

            // // after grabbing the sum, it then must divide by 30
            if (p.Count >= 30)
            {
                temp.LeagueAverage = temp.LeagueAverage / 30;
            }
            else if (p.Count > 0) // divides by as much player history as possible + how ever many times were bowled in current tournament
            {
                temp.LeagueAverage = temp.LeagueAverage / (p.Count + howmanyTimesdidheybowlbeforethissquad);
            }
            else // if they have no bowling history then divide the sum by the number currently bowled in the tournament
            {
                temp.LeagueAverage = temp.LeagueAverage / howmanyTimesdidheybowlbeforethissquad;
            }
        }

        /// <summary>
        /// This method ensures that after the DataGridView is sorted by the user,
        /// the formatting for invalid, and low scoring games still continues.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_Sorted(object sender, EventArgs e)
        {
            InitializeGameCellFormatting();
        }


    }
}