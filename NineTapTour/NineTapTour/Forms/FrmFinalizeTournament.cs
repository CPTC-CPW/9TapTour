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

namespace NineTapTour.Forms
{

    public partial class FrmFinalizeTournament : Form
    {
        List<FinalizeTemp> FinalizeTableList = new List<FinalizeTemp>();
        private Tournament currentT;
        private List<TopScores> topscores;
        public FrmFinalizeTournament(Tournament tourn, List<TopScores> topscores)
        {
            InitializeComponent();
            this.dataGridView1.DoubleBuffered(false);
            this.currentT = tourn;
            this.topscores = topscores;
            FinalizeTableList = GetAllInitialParticipantGameList(tourn);
            createDataGridView(tourn);



        }

        //USE THIS IF YOU NEED TO MOVE AROUND THE ORDER IN WHICH THE COLUMNS ARE DISPLAYED 
        static int INDEX_COLUMN = 0;
        static int GAME_ID_COLUMN = 1;
        static int NAME_COLUMN = 2;
        static int GAME_1_COLUMN = 3;
        static int GAME_1_VALID_COLUMN = 4;
        static int GAME_2_COLUMN = 5;
        static int GAME_2_VALID_COLUMN = 6;
        static int GAME_3_COLUMN = 7;
        static int GAME_3_VALID_COLUMN = 8;
        static int GAME_4_COLUMN = 9;
        static int GAME_4_VALID_COLUMN = 10;
        static int SCRATCH_TOTAL_COLUMN = 11;
        static int HANDICAP_TOTAL_COLUMN = 12;
        static int ENTRY_AVERAGE_COLUMN = 13;
        static int THIRTY_ENTRY_AVERAGE_COLUMN = 14;
        static int ADJUSTED_AVG_COLUMN = 15;
        static int DIRECTOR_CHECK_COLUMN = 16;
        static int SQUAD_COLUMN = 17;
        static int HANDICAP_COLUMN = 18;
        static int BONUS_COLUMN = 19;
        static int PRO_POT_COLUMN = 20;
        static int NOTES_COLUMN_ = 21;


        //USE THIS IF YOU WANT TO CHANGE THE NAME OF EACH COLUMN 
        static string INDEX_COLUMN_NAME = "Index";
        static string GAME_ID_COLUMN_NAME = "GameID";
        static string NAME_COLUMN_NAME = "Name";
        static string GAME_1_COLUMN_NAME = "Game 1";
        static string GAME_1_VALID_COLUMN_NAME = "Valid Score1?";
        static string GAME_2_COLUMN_NAME = "Game 2";
        static string GAME_2_VALID_COLUMN_NAME = "Valid Score2?";
        static string GAME_3_COLUMN_NAME = "Game 3";
        static string GAME_3_VALID_COLUMN_NAME = "Valid Score3?";
        static string GAME_4_COLUMN_NAME = "Game 4";
        static string GAME_4_VALID_COLUMN_NAME = "Valid Score4?";
        static string SCRATCH_TOTAL_COLUMN_NAME = "Scratch Total";
        static string HANDICAP_TOTAL_COLUMN_NAME = "Handicap Total";
        static string ENTRY_AVERAGE_COLUMN_NAME = "Entry AVG";
        static string THIRTY_ENTRY_AVERAGE_COLUMN_NAME = "30 Entry AVG";
        static string ADJUSTED_AVG_COLUMN_NAME = "ADJ AVG";
        static string DIRECTOR_CHECK_COLUMN_NAME = "Director Check";
        static string SQUAD_COLUMN_NAME = "Squad";
        static string HANDICAP_COLUMN_NAME = "Handicap";
        static string BONUS_COLUMN_NAME = "Bonus";
        static string PRO_POT_COLUMN_NAME = "Pro Pot";
        static string NOTES_COLUMN_NAME = "Notes";



        private void createDataGridView(Tournament tourn)
        {
          

            foreach (var item in FinalizeTableList)
            {
                FinalizeTemp temp;
               
                Game g = FinalizeTempDB.getGame(item.GameId);
                if (FinalizeTempDB.getFinalizeID(g).FinalizeID > 0)
                {
                    temp = FinalizeTempDB.getFinalizeID(g);
                    temp.TournamentID = tourn.Id;
                    temp.ScratchTotal = temp.Game1 + temp.Game2 + temp.Game3 + temp.Game4;
                    temp.HandicapTotal = ((temp.Game1 + temp.Bonus + temp.Handicap) + (temp.Game2 + temp.Bonus + temp.Handicap) + (temp.Game3 + temp.Bonus + temp.Handicap) + (temp.Game4 + temp.Bonus + temp.Handicap));
                }
                else
                {
                    List<PlayerHistory> p = PlayerHistoryDB.getMemberPlayerHistory(item.MemberId);
                    temp = new FinalizeTemp();
                    temp.FinalizeID = FinalizeTableList.Count;
                    temp.TournamentID = tourn.Id;
                    temp.GameId = item.GameId;
                    temp.MemberId = item.MemberId;
                    temp.FirstName = item.FirstName;
                    temp.LastName = item.LastName;
                    temp.Squad = item.Squad;
                    temp.Game1 = item.Game1;
                    temp.Game2 = item.Game2;
                    temp.Game3 = item.Game3;
                    temp.Game4 = item.Game4;
                    temp.Notes = item.Notes;
                    temp.UseGame1 = item.UseGame1;
                    temp.UseGame2 = item.UseGame2;
                    temp.UseGame3 = item.UseGame3;
                    temp.UseGame4 = item.UseGame4;
                    temp.AdjustedAvg = 0;
                    temp.ScratchTotal = temp.Game1 + temp.Game2 + temp.Game3 + temp.Game4;
                    temp.GameAvg = (temp.Game1 + temp.Game2 + temp.Game3 + temp.Game4) / 4;
                    temp.LeagueAverage = Convert.ToInt32((LeagueAvgFromPlayerHistory(item.MemberId) + temp.GameAvg));
                    if (p.Count >= 30)
                    {
                        temp.LeagueAverage = temp.LeagueAverage / 30;
                    }
                    else if(p.Count > 0)
                    {
                        temp.LeagueAverage = temp.LeagueAverage / p.Count;
                    }
                    else
                    {
                        temp.LeagueAverage = temp.LeagueAverage;
                    }
                    temp.Handicap = item.Handicap;
                    temp.Bonus = item.Bonus;
                    temp.HandicapTotal = ((temp.Game1 + temp.Bonus + temp.Handicap) + (temp.Game2 + temp.Bonus + temp.Handicap) + (temp.Game3 + temp.Bonus + temp.Handicap) + (temp.Game4 + temp.Bonus + temp.Handicap));
                }
                FinalizeTempDB.AddFinalizeTempOnstart(temp);
            }
            //pulls a list from the finalizetemp table and seeds the dataview with the table info.
            List<FinalizeTemp> DataViewList = GetListFromTable(tourn);
            FinalizeTableList = DataViewList;

            dataGridView1.DataSource = DataView(tourn, DataViewList); //By default populates all datagrid with all participant for tournament.






            ////Sort DataGridView by TrueAverage
            //this.dataGridView1.Sort(this.dataGridView1.Columns["True Avg"], ListSortDirection.Descending);

        //sets sizes of check box columns "Valid Score1, ValidScore2, ValidScore3, Valid Score 4, and Keep True Avg?"
        dataGridView1.SuspendLayout();
            var column = dataGridView1.Columns[NAME_COLUMN];
            for (int i = 0; i <= 20 ; i++)
            {
                
                column = dataGridView1.Columns[i];

                if (column.Index == GAME_1_VALID_COLUMN || column.Index == GAME_2_VALID_COLUMN || column.Index == GAME_3_VALID_COLUMN || column.Index == GAME_4_VALID_COLUMN || column.Index == DIRECTOR_CHECK_COLUMN)
                {
                    column.Width = 50;
                }
                else if(column.Index == NAME_COLUMN)
                {
                    column.Width = 100;
                }
                else
                {
                    column.Width = 75;
                }
            }
            dataGridView1.ResumeLayout();


            dataGridView1.AllowUserToAddRows = false;
        }



 

        //creates the dataview that will populate the datagridview table on form pulls from the finalizetemp table
        // CHANGE THESE IN THE ORDER YOU WANT THEM TO BE SEEN ON THE GRID VIEW (0 == far left) AND THEN CHANGE THE STATIC INTS AT THE TOP IN ORDER TO CHANGE THERE ORDER ON THE GRID VIEW WITHOUT HAVING TO TOUNCH ANY OTHER CODE
        public DataTable DataView(Tournament tourn, List<FinalizeTemp> participantsList)
        {
            var db = new NineTapDb();
            DataTable dt = new DataTable();
            dt.Columns.Add(INDEX_COLUMN_NAME); //0
            dt.Columns.Add(GAME_ID_COLUMN_NAME).ReadOnly = true; //1
            dt.Columns.Add(NAME_COLUMN_NAME).ReadOnly = true; //2
            dt.Columns.Add(GAME_1_COLUMN_NAME).ReadOnly = true; //3
            dt.Columns.Add(new DataColumn(GAME_1_VALID_COLUMN_NAME, typeof(bool))); //4
            dt.Columns.Add(GAME_2_COLUMN_NAME).ReadOnly = true; //5
            dt.Columns.Add(new DataColumn(GAME_2_VALID_COLUMN_NAME, typeof(bool))); //6
            dt.Columns.Add(GAME_3_COLUMN_NAME).ReadOnly = true; //7
            dt.Columns.Add(new DataColumn(GAME_3_VALID_COLUMN_NAME, typeof(bool)));//8
            dt.Columns.Add(GAME_4_COLUMN_NAME).ReadOnly = true;//9
            dt.Columns.Add(new DataColumn(GAME_4_VALID_COLUMN_NAME, typeof(bool)));//10
            dt.Columns.Add(SCRATCH_TOTAL_COLUMN_NAME);//11
            dt.Columns.Add(HANDICAP_TOTAL_COLUMN_NAME);//12
            dt.Columns.Add(ENTRY_AVERAGE_COLUMN_NAME);//13
            dt.Columns.Add(THIRTY_ENTRY_AVERAGE_COLUMN_NAME).ReadOnly = true;     //14  
            dt.Columns.Add(ADJUSTED_AVG_COLUMN_NAME); //15
            dt.Columns.Add(new DataColumn(DIRECTOR_CHECK_COLUMN_NAME, typeof(bool)));//16
            dt.Columns.Add(SQUAD_COLUMN_NAME).ReadOnly = true;//16
            dt.Columns.Add(HANDICAP_COLUMN_NAME).ReadOnly = true;//17
            dt.Columns.Add(BONUS_COLUMN_NAME).ReadOnly = true;//18
            dt.Columns.Add(PRO_POT_COLUMN_NAME);//19
            dt.Columns.Add(NOTES_COLUMN_NAME);//20
            


            //whatever list of participants you pass into method will be populated into grid
            List<FinalizeTemp> temp = participantsList;
            int index = 1;
            //loops thru each person's info in tournament and populates the dataview with data from DB.
            foreach (var item in temp)
            {
                DataRow newRow = dt.NewRow();
                newRow[GAME_ID_COLUMN_NAME] = item.GameId;
                if (item.FirstName.Length > 1)
                {
                    string[] aftersplit = item.FirstName.Split(' ');
                    try
                    {
                        newRow[NAME_COLUMN_NAME] = aftersplit[0] + " " + item.LastName;
                    }
                    catch
                    {
                        newRow[NAME_COLUMN_NAME] = item.FirstName + " " + item.LastName;
                    }
                }
                else
                {
                    newRow[NAME_COLUMN_NAME] = item.FirstName + " " + item.LastName;
                }
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
                newRow[INDEX_COLUMN_NAME] = index;
                newRow[HANDICAP_TOTAL_COLUMN_NAME] = item.HandicapTotal;
                dt.Rows.Add(newRow);
                for (int c = 0; c < topscores.Count; c++)
                {
                    if(topscores[c].Game1 == item.Game1 && topscores[c].Game2 == item.Game2 && topscores[c].Game3 == item.Game3 && topscores[c].Game4 == item.Game4 && topscores[c].memberNumber == item.MemberId)
                    {
                        topscores[c].GameID = item.GameId;
                    }
                }
                index++;
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
                         


                        }).ToList();
            foreach (var item in temp)
            {
                FinalizeTemp NewParticipant = new FinalizeTemp();
                NewParticipant.GameId = item.Id;
                NewParticipant.MemberId = item.MemberId;
                NewParticipant.FirstName = item.FirstName;
                NewParticipant.LastName = item.LastName;
                NewParticipant.Game1 = (int)item.Game1;
                NewParticipant.Game2 = (int)item.Game2;
                NewParticipant.Game3 = (int)item.Game3;
                NewParticipant.Game4 = (int)item.Game4;
                if (item.UseGame1 == null)
                {
                    NewParticipant.UseGame1 = true;
                }
                else
                {
                    NewParticipant.UseGame1 = (bool)item.UseGame1;
                }
                if (item.UseGame2 == null)
                {
                    NewParticipant.UseGame2 = true;
                }
                else
                {
                    NewParticipant.UseGame2 = (bool)item.UseGame1;
                }
                if (item.UseGame3 == null)
                {
                    NewParticipant.UseGame3 = true;
                }
                else
                {
                    NewParticipant.UseGame3 = (bool)item.UseGame1;
                }
                if (item.UseGame4 == null)
                {
                    NewParticipant.UseGame4 = true;
                }
                else
                {
                    NewParticipant.UseGame4 = (bool)item.UseGame1;
                }

                NewParticipant.Notes = item.Notes;
                NewParticipant.ScratchTotal = (int)(item.Game1 + item.Game2 + item.Game3 + item.Game4);
                NewParticipant.Squad = item.Squad;
                NewParticipant.GameAvg = (int)(item.Game1 + item.Game2 + item.Game3 + item.Game4) / 4;
                NewParticipant.Handicap = (int)item.Handicap;
                try
                {
                    NewParticipant.Bonus = (int)item.Bonus;
                }
                catch
                {
                    NewParticipant.Bonus = 0;
                }
                ParticipantList.Add(NewParticipant);
            }

            return ParticipantList;
        }




        //makes a list from the finalizetemp table to be used in dataview source
        public List<FinalizeTemp> GetListFromTable(Tournament tourn)
        {
            var db = new NineTapDb();
            List<FinalizeTemp> ParticipantList = new List<FinalizeTemp>();
            var temp = (from p in db.FinalizeTemp
                        join t in db.Tournaments on p.TournamentID equals t.Id
                        where tourn.Id == t.Id
                        orderby p.ScratchTotal ascending, p.HandicapTotal descending,  p.FirstName descending
                        select new
                        {
                            p.FinalizeID,
                            p.TournamentID,
                            p.GameId,
                            p.MemberId,
                            p.FirstName,
                            p.LastName,
                            p.Squad,
                            p.Game1,
                            p.Game2,
                            p.Game3,
                            p.Game4,
                            p.UseGame1,
                            p.UseGame2,
                            p.UseGame3,
                            p.UseGame4,
                            p.AdjustedAvg,
                            p.LeagueAverage,
                            p.Notes,
                            p.ScratchTotal,
                            p.KeepAdjustedAvg,
                            p.GameAvg,
                            p.Handicap,
                            p.Bonus,
                            p.HandicapTotal


                        }).ToList();
            foreach (var item in temp)
            {
                FinalizeTemp NewParticipant = new FinalizeTemp();
                NewParticipant.FinalizeID = item.FinalizeID;
                NewParticipant.GameId = item.GameId;
                NewParticipant.TournamentID = item.TournamentID;
                NewParticipant.MemberId = item.MemberId;
                NewParticipant.FirstName = item.FirstName;
                NewParticipant.LastName = item.LastName;
                NewParticipant.Game1 = (int)item.Game1;
                NewParticipant.Game2 = (int)item.Game2;
                NewParticipant.Game3 = (int)item.Game3;
                NewParticipant.Game4 = (int)item.Game4;
                NewParticipant.UseGame1 = (bool)item.UseGame1;
                NewParticipant.UseGame2 = (bool)item.UseGame2;
                NewParticipant.UseGame3 = (bool)item.UseGame3;
                NewParticipant.UseGame4 = (bool)item.UseGame4;
                NewParticipant.Notes = item.Notes;
                NewParticipant.ScratchTotal = (int)item.ScratchTotal;
                NewParticipant.Squad = item.Squad;
                NewParticipant.KeepAdjustedAvg = item.KeepAdjustedAvg;
                NewParticipant.GameAvg = (int)item.GameAvg;
                NewParticipant.LeagueAverage = (int)item.LeagueAverage;
                NewParticipant.AdjustedAvg = (int)item.AdjustedAvg;
                NewParticipant.Handicap = (int)item.Handicap;
                NewParticipant.Bonus = (int)item.Bonus;
                NewParticipant.HandicapTotal = (int)item.HandicapTotal;
                ParticipantList.Add(NewParticipant);
            }

            return ParticipantList;
        }



        //Updates the finalizetemp table when check box for Use Game Score is clicked on.
        private void dataGridView1_OnCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), GAME_1_VALID_COLUMN.ToString()) == 0)
            {
                bool checkBoxStatus = Convert.ToBoolean(dataGridView1.CurrentCell.EditedFormattedValue);
                //checkBoxStatus gives you whether checkbox cell value of selected row for the
                //"CheckBoxColumn" column value is checked or not. 
                FinalizeTemp temp = new FinalizeTemp();
                var row = dataGridView1.CurrentCell.RowIndex;
                NineTapDb db = new NineTapDb();
                if (checkBoxStatus)
                {
                    dataGridView1.Rows[row].Cells[GAME_1_VALID_COLUMN].Value = true;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, GAME_1_COLUMN , true);

                }
                else
                {
                    dataGridView1.Rows[row].Cells[GAME_1_VALID_COLUMN].Value = false;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, GAME_1_COLUMN, false);
                }
            }
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), GAME_2_VALID_COLUMN.ToString()) == 0)
            {
                bool checkBoxStatus = Convert.ToBoolean(dataGridView1.CurrentCell.EditedFormattedValue);
                //checkBoxStatus gives you whether checkbox cell value of selected row for the
                //"CheckBoxColumn" column value is checked or not. 
                var row = dataGridView1.CurrentCell.RowIndex;
                if (checkBoxStatus)
                {
                    dataGridView1.Rows[row].Cells[GAME_2_VALID_COLUMN].Value = true;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, GAME_2_COLUMN, true);

                }
                else
                {
                    dataGridView1.Rows[row].Cells[GAME_2_VALID_COLUMN].Value = false;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, GAME_2_COLUMN , false);
                }
            }
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), GAME_3_VALID_COLUMN.ToString()) == 0)
            {
                bool checkBoxStatus = Convert.ToBoolean(dataGridView1.CurrentCell.EditedFormattedValue);
                //checkBoxStatus gives you whether checkbox cell value of selected row for the
                //"CheckBoxColumn" column value is checked or not. 
                var row = dataGridView1.CurrentCell.RowIndex;
                if (checkBoxStatus)
                {
                    dataGridView1.Rows[row].Cells[GAME_3_VALID_COLUMN].Value = true;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, GAME_3_COLUMN, true);

                }
                else
                {
                    dataGridView1.Rows[row].Cells[GAME_3_VALID_COLUMN].Value = false;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, GAME_3_COLUMN , false);

                }
            }
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), GAME_4_VALID_COLUMN.ToString()) == 0)
            {
                bool checkBoxStatus = Convert.ToBoolean(dataGridView1.CurrentCell.EditedFormattedValue);
                //checkBoxStatus gives you whether checkbox cell value of selected row for the
                //"CheckBoxColumn" column value is checked or not. 
                var row = dataGridView1.CurrentCell.RowIndex;
                if (checkBoxStatus)
                {
                    dataGridView1.Rows[row].Cells[GAME_4_VALID_COLUMN].Value = true;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, GAME_4_COLUMN, true);


                }
                else
                {
                    dataGridView1.Rows[row].Cells[GAME_4_VALID_COLUMN].Value = false;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, GAME_4_COLUMN , false);

                }
            }
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), DIRECTOR_CHECK_COLUMN.ToString()) == 0)
            {
                int gameId = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[GAME_ID_COLUMN].Value);
                int memId;
                using (var db = new NineTapDb())
                {
                 memId = db.Participants.Include(b => b.Game).Include(b => b.Member).First(p => p.Game.Id == gameId).Member.Id;
                }
                bool checkBoxStatus = Convert.ToBoolean(dataGridView1.CurrentCell.EditedFormattedValue);
                //checkBoxStatus gives you whether checkbox cell value of selected row for the
                //"CheckBoxColumn" column value is checked or not. 

                if (checkBoxStatus)
                {
                    for(int i = 0; i < FinalizeTableList.Count; i++)
                    {
                        if(FinalizeTableList[i].MemberId == memId)
                        {
                            dataGridView1.Rows[i].Cells[DIRECTOR_CHECK_COLUMN].Value = true;
                        }
                    }
                    
                }
                else
                {
                    for (int i = 0; i < FinalizeTableList.Count; i++)
                    {
                        if (FinalizeTableList[i].MemberId == memId)
                        {
                            dataGridView1.Rows[i].Cells[DIRECTOR_CHECK_COLUMN].Value = false;
                        }
                    }


                }
            }

        }

        private void dataGridView1_OnCellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // End of edition on each click on column of checkbox
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), GAME_1_VALID_COLUMN.ToString()) == 0)
            {
                dataGridView1.EndEdit();
            }
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), GAME_2_VALID_COLUMN.ToString()) == 0)
            {
                dataGridView1.EndEdit();
            }
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), GAME_3_VALID_COLUMN.ToString()) == 0)
            {
                dataGridView1.EndEdit();
            }
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), GAME_4_VALID_COLUMN.ToString()) == 0)
            {
                dataGridView1.EndEdit();
            }
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), DIRECTOR_CHECK_COLUMN.ToString()) == 0)
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
            db.Entry(temp).State = EntityState.Modified;
            db.SaveChanges();
            this.dataGridView1.CellValueChanged += this.dataGridView1_OnCellValueChanged;
        }
        //updates computed average in column 10 when check box is changed.
        private void UpdateAvg(int row)
        {
            this.dataGridView1.CellValueChanged -= this.dataGridView1_OnCellValueChanged;
            int sum = 0;
            int count = 0;
            int sumWHandicap = 0;
            int HDCPwithBonus = Convert.ToInt32((dataGridView1.Rows[row].Cells[HANDICAP_COLUMN].Value)) + Convert.ToInt32((dataGridView1.Rows[row].Cells[BONUS_COLUMN].Value));
            if (Convert.ToBoolean(dataGridView1.Rows[row].Cells[GAME_1_VALID_COLUMN].Value) == true)
            {
                sum += Convert.ToInt32((dataGridView1.Rows[row].Cells[GAME_1_COLUMN].Value));
                sumWHandicap = sum + HDCPwithBonus;
                count++;
            }
            if (Convert.ToBoolean(dataGridView1.Rows[row].Cells[GAME_2_VALID_COLUMN].Value) == true)
            {
                sum += Convert.ToInt32((dataGridView1.Rows[row].Cells[GAME_2_COLUMN].Value));
                sumWHandicap = sum + HDCPwithBonus;
                count++;
            }
            if (Convert.ToBoolean(dataGridView1.Rows[row].Cells[GAME_3_VALID_COLUMN].Value) == true)
            {
                sum += Convert.ToInt32((dataGridView1.Rows[row].Cells[GAME_3_COLUMN].Value));
                sumWHandicap = sum + HDCPwithBonus;
                count++;
            }
            if (Convert.ToBoolean(dataGridView1.Rows[row].Cells[GAME_4_VALID_COLUMN].Value) == true)
            {
                sum += Convert.ToInt32((dataGridView1.Rows[row].Cells[GAME_4_COLUMN].Value));
                sumWHandicap = sum + HDCPwithBonus;
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
        public double LeagueAvgFromPlayerHistory(int mem)
        {
      
            double sum = 0;
            double avg = 0;
            var db = new NineTapDb();
            var temp = (from p in db.PlayerHistory
                        where p.MemberNumber == mem
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
                        }).Take(30).ToList(); 
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
                // WE can change the Color of Rows if Same Member Places Twice Here
            }
        }



        //public void UpdateMemberMoneyWon(GameParticipant p)
        //{
        //    var db = new NineTapDb();
        //    //Find member
        //    var member = db.Members.Find(p.MemberId);
        //    //Find Game
        //    var game = db.Games.Find(p.GameId);
        //    if (p.MoneyWon != game.MoneyWon)
        //    {
        //        //member.MoneyEarned -= game.MoneyWon;//Member doesnt contain a money earned property yet but member page has a text box for one.
        //        //member.MoneyEarned += p.MoneyWon;
        //        //db.Entry(member).CurrentValues.SetValues(member.Id);
        //        //db.SaveChanges();

        //        game.MoneyWon = p.MoneyWon;
        //        db.Entry(game).CurrentValues.SetValues(game.Id);
        //        db.SaveChanges();

        //    }
        //}

        /// <summary>
        /// This method recieves an individual GameParticipant object and saves its values to
        /// FinalizeTempTable
        /// </summary>
        /// <param name="UpdatedGame"></param>
        /// <returns>Return true if Game saved to database, false if not.</returns>
        //public bool SaveIndividualGame(GameParticipant UpdatedGame)
        //{
        //    var db = new NineTapDb();

        //    var GameOriginal = db.FinalizeTemp.Find(UpdatedGame.GameId);

        //    if (GameOriginal != null)
        //    {
        //        try
        //        {
        //            //update finalize temp table with new values.
        //            db.Entry(GameOriginal).CurrentValues.SetValues(UpdatedGame.GameId);
        //            db.SaveChanges();

        //        }
        //        catch
        //        {
        //            //return false if issue saving changes to database.
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        //return false if cant find game.
        //        return false;

        //    }
        //    return true;
        //}

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

        //formats cells based off bool value for valid score, strike thru score on previous score column.
        //changes background color of score to orange if 50 below 30 game avg.
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
        
            dataGridView1.SuspendLayout();
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == GAME_1_VALID_COLUMN_NAME && e.Value != null)
            {

                if (Convert.ToBoolean(e.Value) == true)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_1_COLUMN].Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_1_COLUMN].Style.BackColor = Color.White;
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_1_COLUMN].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
                   
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_1_COLUMN].Style.Font = new Font(dataGridView1.Font, FontStyle.Strikeout);
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_1_COLUMN].Style.BackColor = Color.Red;
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_1_COLUMN].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == GAME_2_VALID_COLUMN_NAME && e.Value != null)
            {
                if (Convert.ToBoolean(e.Value) == true)
                {
                    
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_2_COLUMN].Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_2_COLUMN].Style.BackColor = Color.White;
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_2_COLUMN].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
                   
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_2_COLUMN].Style.Font = new Font(dataGridView1.Font, FontStyle.Strikeout);
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_2_COLUMN].Style.BackColor = Color.Red;
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_2_COLUMN].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == GAME_3_VALID_COLUMN_NAME && e.Value != null)
            {
                if (Convert.ToBoolean(e.Value) == true)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_3_COLUMN].Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_3_COLUMN].Style.BackColor = Color.White;
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_3_COLUMN].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
                 
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_3_COLUMN].Style.Font = new Font(dataGridView1.Font, FontStyle.Strikeout);
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_3_COLUMN].Style.BackColor = Color.Red;
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_3_COLUMN].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == GAME_4_VALID_COLUMN_NAME && e.Value != null)
            {
                if (Convert.ToBoolean(e.Value) == true)
                {
                   
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_4_COLUMN].Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_4_COLUMN].Style.BackColor = Color.White;
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_4_COLUMN].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
                 
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_4_COLUMN].Style.Font = new Font(dataGridView1.Font, FontStyle.Strikeout);
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_4_COLUMN].Style.BackColor = Color.Red;
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_4_COLUMN].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == THIRTY_ENTRY_AVERAGE_COLUMN_NAME && e.Value != null)
            {
                if (Convert.ToInt32(e.Value) > Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[GAME_1_COLUMN].Value) + 50 && dataGridView1.Rows[e.RowIndex].Cells[GAME_1_COLUMN].Style.BackColor != Color.Red)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_1_COLUMN].Style.BackColor = Color.Orange;
                }
                if (Convert.ToInt32(e.Value) > Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[GAME_2_COLUMN].Value) + 50 && dataGridView1.Rows[e.RowIndex].Cells[GAME_2_COLUMN].Style.BackColor != Color.Red)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_2_COLUMN].Style.BackColor = Color.Orange;
                }
                if (Convert.ToInt32(e.Value) > Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[GAME_3_COLUMN].Value) + 50 && dataGridView1.Rows[e.RowIndex].Cells[GAME_3_COLUMN].Style.BackColor != Color.Red)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_3_COLUMN].Style.BackColor = Color.Orange;
                }
                if (Convert.ToInt32(e.Value) > Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[GAME_4_COLUMN].Value) + 50 && dataGridView1.Rows[e.RowIndex].Cells[GAME_4_COLUMN].Style.BackColor != Color.Red)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[GAME_4_COLUMN].Style.BackColor = Color.Orange;
                }
            }
            dataGridView1.ResumeLayout();
        }


        private void check_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            dataGridView1.SuspendLayout();
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == GAME_1_VALID_COLUMN_NAME && e.Value != null)
            {

                if (Convert.ToBoolean(e.Value) == true)
                {
              
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.BackColor = Color.White;
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
              
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Font = new Font(dataGridView1.Font, FontStyle.Strikeout);
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.BackColor = Color.Red;
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == GAME_2_VALID_COLUMN_NAME && e.Value != null)
            {
                if (Convert.ToBoolean(e.Value) == true)
                {
                  
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.BackColor = Color.White;
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
                    
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Font = new Font(dataGridView1.Font, FontStyle.Strikeout);
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.BackColor = Color.Red;
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == GAME_3_VALID_COLUMN_NAME && e.Value != null)
            {
                if (Convert.ToBoolean(e.Value) == true)
                {
               
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.BackColor = Color.White;
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
            
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Font = new Font(dataGridView1.Font, FontStyle.Strikeout);
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.BackColor = Color.Red;
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == GAME_4_VALID_COLUMN_NAME && e.Value != null)
            {
                if (Convert.ToBoolean(e.Value) == true)
                {

                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.BackColor = Color.White;
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
                    
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Font = new Font(dataGridView1.Font, FontStyle.Strikeout);
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.BackColor = Color.Red;
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == THIRTY_ENTRY_AVERAGE_COLUMN_NAME && e.Value != null)
            {
                if (Convert.ToInt32(e.Value) > Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 2].Value) + 50 && dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 2].Style.BackColor != Color.Red)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 2].Style.BackColor = Color.Orange;
                }
                if (Convert.ToInt32(e.Value) > Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 4].Value) + 50 && dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 4].Style.BackColor != Color.Red)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 4].Style.BackColor = Color.Orange;
                }
                if (Convert.ToInt32(e.Value) > Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 6].Value) + 50 && dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 6].Style.BackColor != Color.Red)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 6].Style.BackColor = Color.Orange;
                }
                if (Convert.ToInt32(e.Value) > Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 8].Value) + 50 && dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 8].Style.BackColor != Color.Red)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 8].Style.BackColor = Color.Orange;
                }
            }
            dataGridView1.ResumeLayout();
        }

        /***
        when you double clicke a cell, the selected cell(may not the clicked cell) will display the member's information 
         ***/
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show( dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[dataGridView1.CurrentCell.ColumnIndex].Value.ToString());
            ////press alt to make it work, do not know why
            int gameId = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[GAME_ID_COLUMN].Value);



            using (var db = new NineTapDb())
            {
                int memId = db.Participants.Include(b => b.Game).Include(b => b.Member).First(p => p.Game.Id == gameId).Member.Id;
                var temp = (from p in db.Participants
                            join m in db.Members on p.Member.Id equals m.Id
                            join g in db.Games on p.Game.Id equals g.Id//dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value//g.Id
                            join t in db.Tournaments on p.Tournament.Id equals t.Id
                            where p.Member.Id == memId

                            select new
                            {
                                g.Id,
                                m.FirstName,
                                m.LastName,
                                MemberId = m.Id,
                                TournId = t.Id,
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
                                //I believe it needs more information

                            }).ToList();


                FrmStats playerhistory = new FrmStats(memId,"",MemberDb.GetMember(memId));
                playerhistory.ShowDialog();
            }


        }

        private void btnFinalize_Click(object sender, EventArgs e)
        {
           
            int check = 0;
            PlayerHistory ph = new PlayerHistory();
            List<FinalizeTemp> FinalizeTableList = GetListFromTable(currentT);
            List<Participant> partlist = TournamentDb.GetTournamentMemberList(currentT);
            int gamesPlayed = 0;
            List<int> addedalreeady = new List<int>();
            int placing = 1;


            for (int i = 0; i < FinalizeTableList.Count; i++)
            {
                if (dataGridView1[DIRECTOR_CHECK_COLUMN, i].Value.ToString() == "False") 
                {
                    dataGridView1.Rows[i].Cells[DIRECTOR_CHECK_COLUMN].Style.BackColor = Color.Red;
                }
                else if(dataGridView1[DIRECTOR_CHECK_COLUMN, i].Value.ToString() == "True")
                {
                    dataGridView1.Rows[i].Cells[DIRECTOR_CHECK_COLUMN].Style.BackColor = Color.White;
                    check++;
                }

            }
            for (int p = 0; p < partlist.Count; p++)
            {
                partlist[p].Tournament = currentT;
                partlist[p].Game = FinalizeTempDB.getGame(FinalizeTableList[p].GameId);
            }

            if (check == FinalizeTableList.Count)
            {
                for (int currentindex = 0; currentindex < FinalizeTableList.Count; currentindex++)
                {
                    int memberEntryCount = 0;
                    gamesPlayed = 0;
                    ph.GameID = FinalizeTableList[currentindex].GameId;
                    Game g = FinalizeTempDB.getGame(FinalizeTableList[currentindex].GameId);

                    int memId;
                    using (var db = new NineTapDb())
                    {
                        memId = db.Participants.Include(b => b.Game).Include(b => b.Member).First(p => p.Game.Id == g.Id).Member.Id;
                    }

                    
                    Member currentMember = MemberDb.GetMember(memId);
                    List<PlayerHistory> pl = PlayerHistoryDB.getMemberPlayerHistory(memId);


                    ph.TournamentDate = currentT.Date;
                    ph.MemberNumber = FinalizeTableList[currentindex].MemberId;

                    if (dataGridView1[GAME_1_VALID_COLUMN, currentindex].Value.ToString() == "True")
                    {
                        gamesPlayed++;
                        g.UseGame1 = true;
                        FinalizeTableList[currentindex].UseGame1 = true;
                    }
                    else
                    {
                        g.UseGame1 = false;
                        FinalizeTableList[currentindex].UseGame1 = false;
                    }
                    if (dataGridView1[GAME_2_VALID_COLUMN, currentindex].Value.ToString() == "True")
                    {
                        gamesPlayed++;
                        g.UseGame2 = true;
                        FinalizeTableList[currentindex].UseGame2 = true;
                    }
                    else
                    {
                        g.UseGame2 = false;
                        FinalizeTableList[currentindex].UseGame2 = false;
                    }
                    if (dataGridView1[GAME_3_VALID_COLUMN, currentindex].Value.ToString() == "True")
                    {
                        gamesPlayed++;
                        g.UseGame3 = true;
                        FinalizeTableList[currentindex].UseGame3 = true;
                    }
                    else
                    {
                        g.UseGame3 = false;
                        FinalizeTableList[currentindex].UseGame3 = false;
                    }
                    if (dataGridView1[GAME_4_VALID_COLUMN, currentindex].Value.ToString() == "True")
                    {
                        gamesPlayed++;
                        g.UseGame4 = true;
                        FinalizeTableList[currentindex].UseGame4 = true;
                    }
                    else
                    {
                        g.UseGame4 = false;
                        FinalizeTableList[currentindex].UseGame4 = false;
                    }
                    ph.GamesPlayed = gamesPlayed;
                    ph.AverageForGame = FinalizeTableList[currentindex].GameAvg;
                    ph.trueAVG = (LeagueAvgFromPlayerHistory(ph.MemberNumber) + FinalizeTableList[currentindex].GameAvg);
                    
                    if (pl.Count >= 30)
                    {
                        ph.trueAVG = ph.trueAVG / 30;
                    }
                    else if(pl.Count == 0 && memberEntryCount > 1)
                    {
                        ph.trueAVG = pl.Count;
                    }
                    ph.AVG = Convert.ToInt32(dataGridView1[ADJUSTED_AVG_COLUMN, currentindex].Value);
                    ph.ProPot = dataGridView1[PRO_POT_COLUMN, currentindex].Value.ToString();
                    for (int j = 0; j < partlist.Count; j++)
                    {
                        if (ph.MemberNumber == partlist[j].Member.Number)
                        {
                            ph.MoneyWon = Convert.ToDecimal(partlist[j].Game.MoneyWon);
                        }
                    }

                    g.Game1 = FinalizeTableList[currentindex].Game1;
                    ph.Game1 = FinalizeTableList[currentindex].Game1;
                    g.Game2 = FinalizeTableList[currentindex].Game2;
                    ph.Game2 = FinalizeTableList[currentindex].Game2;
                    g.Game3 = FinalizeTableList[currentindex].Game3;
                    ph.Game3 = FinalizeTableList[currentindex].Game3;
                    g.Game4 = FinalizeTableList[currentindex].Game4;
                    ph.Game4 = FinalizeTableList[currentindex].Game4;





               

                    //Calculate Bonus Pins After Tournaments over
                  
                    
                        //if (!addedalreeady.Contains(memId))
                        //{
                        //    for (int j = 0; j < topscores.Count; j++)
                        //    {

                        //        if (FinalizeTableList[currentindex].GameId == topscores[j].GameID && placing <= 20)
                        //        {
                        //            Game ga = FinalizeTempDB.getGame(FinalizeTableList[currentindex].GameId);
                        //            ga.PlaceStanding = Convert.ToByte(placing);
                        //            PlayerHistoryDB.AddGame(ga);
                        //            currentMember.Bonus = Calculations.Calculations.CalculateBonusPins(true, placing, Convert.ToInt32(currentMember.Bonus), currentT.Doubles, memId);
                        //            placing++;
                        //            addedalreeady.Add(memId);
                        //        }
                        //        else if(FinalizeTableList[currentindex].GameId == topscores[j].GameID && placing > 20)
                        //        {                                  
                        //           Game ga = FinalizeTempDB.getGame(FinalizeTableList[currentindex].GameId);
                        //           ga.PlaceStanding = Convert.ToByte(placing);
                        //           PlayerHistoryDB.AddGame(ga);
                        //           currentMember.Bonus = Calculations.Calculations.CalculateBonusPins(false, placing, Convert.ToInt32(currentMember.Bonus), currentT.Doubles, memId);
                        //           placing++;
                        //           addedalreeady.Add(memId);

                        //    }
                        //    }
                        //}
                    






                    ph.PPHG = Convert.ToString(g.PlaceStanding);
                    g.Handicap = FinalizeTableList[currentindex].Handicap;
                    ph.HandiCap = FinalizeTableList[currentindex].Handicap;
                    g.InputtedAvg = ph.AVG;
                    g.Notes = dataGridView1[NOTES_COLUMN_, currentindex].Value.ToString();
                    ph.Notes = g.Notes;
                    currentMember.StartAvg = ph.AVG;                    
                    ph.hisID = PlayerHistoryDB.getHisID(ph);
                    PlayerHistoryDB.AddPlayerHistory2(ph);
                    MemberDb.AddMember(currentMember);
                    FinalizeTableList[currentindex].FinalizeID = FinalizeTempDB.getFinalizeID(g).FinalizeID;
                    FinalizeTableList[currentindex].AdjustedAvg = ph.AVG;
                    FinalizeTempDB.AddFinalizeTempOnFinalize(FinalizeTableList[currentindex]);

                }
                Close();
            }
          }

  
    }


    }


        /***/

    



