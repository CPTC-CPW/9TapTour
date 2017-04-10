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
        public FrmFinalizeTournament(Tournament tourn)
        {
            InitializeComponent();
            createDataGridView(tourn);
            this.dataGridView1.DoubleBuffered(true);
        }

        private void createDataGridView(Tournament tourn)
        {
            List<FinalizeTemp> FinalizeTableList = GetAllInitialParticipantGameList(tourn);
            foreach (var item in FinalizeTableList)
            {
                FinalizeTemp temp = new FinalizeTemp();
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
                temp.ScratchTotal = temp.Game1 + temp.Game2 + temp.Game3 + temp.Game4;
                temp.GameAvg = (temp.Game1 + temp.Game2 + temp.Game3 + temp.Game4) / 4;
                temp.LeagueAverage = Convert.ToInt32(LeagueAverage(item.MemberId));
                temp.AdjustedAvg = 0;
                temp.Handicap = item.Handicap;
                temp.Bonus = item.Bonus;
                FinalizeTempDB.AddFinalizeTemp(temp);
            }
            //pulls a list from the finalizetemp table and seeds the dataview with the table info.
            List<FinalizeTemp> DataViewList = GetListFromTable();
            dataGridView1.DataSource = DataView(tourn, DataViewList); //By default populates all datagrid with all participant for tournament.


            ////Sort DataGridView by TrueAverage
            //this.dataGridView1.Sort(this.dataGridView1.Columns["True Avg"], ListSortDirection.Descending);

            //sets sizes of check box columns "Valid Score1, ValidScore2, ValidScore3, Valid Score 4, and Keep True Avg?"
            dataGridView1.SuspendLayout();
            var column = dataGridView1.Columns[1];
            for(int i = 2; i <=12; i++)
            {
                column = dataGridView1.Columns[i];
                column.Width = 50;
            }
            dataGridView1.ResumeLayout();


            dataGridView1.AllowUserToAddRows = false;
        }


        //creates the dataview that will populate the datagridview table on form pulls from the finalizetemp table
        public DataTable DataView(Tournament tourn, List<FinalizeTemp> participantsList)
        {
            var db = new NineTapDb();
            DataTable dt = new DataTable();
            dt.Columns.Add("GameId").ReadOnly = true;
            dt.Columns.Add("Name").ReadOnly = true;
            dt.Columns.Add("Game 1").ReadOnly = true;
            dt.Columns.Add(new DataColumn("Valid Score1?", typeof(bool)));
            dt.Columns.Add("Game 2").ReadOnly = true;
            dt.Columns.Add(new DataColumn("Valid Score2?", typeof(bool)));
            dt.Columns.Add("Game 3").ReadOnly = true;
            dt.Columns.Add(new DataColumn("Valid Score3?", typeof(bool)));
            dt.Columns.Add("Game 4").ReadOnly = true;
            dt.Columns.Add(new DataColumn("Valid Score4?", typeof(bool)));
            dt.Columns.Add("30 Game Avg").ReadOnly = true;
            dt.Columns.Add("Game Avg");
            dt.Columns.Add("Adj Avg");
            dt.Columns.Add(new DataColumn("Keep True Avg?", typeof(bool)));
            dt.Columns.Add("Scratch Total");
            dt.Columns.Add("Squad").ReadOnly = true;
            dt.Columns.Add("Handicap").ReadOnly = true;
            dt.Columns.Add("Bonus").ReadOnly = true;
            dt.Columns.Add("Pro Pot");
            dt.Columns.Add("Notes");

            //whatever list of participants you pass into method will be populated into grid
            List<FinalizeTemp> temp = participantsList;

            //loops thru each person's info in tournament and populates the dataview with data from DB.
            foreach (var item in temp)
            {
                DataRow newRow = dt.NewRow();
                newRow["GameId"] = item.GameId;//0
                newRow["Name"] = item.FirstName + " " + item.LastName;//1
                newRow["Game 1"] = item.Game1;//2
                newRow["Valid Score1?"] = item.UseGame1;//3
                newRow["Game 2"] = item.Game2;//4
                newRow["Valid Score2?"] = item.UseGame2;//5
                newRow["Game 3"] = item.Game3;//6
                newRow["Valid Score3?"] = item.UseGame3;//7
                newRow["Game 4"] = item.Game4;//8
                newRow["Valid Score4?"] = item.UseGame4;//9
                newRow["30 Game Avg"] = item.LeagueAverage; //10
                newRow["Game Avg"] = item.GameAvg;//11
                newRow["Adj Avg"] = item.AdjustedAvg;//12
                newRow["Keep True Avg?"] = false;//13
                newRow["Scratch Total"] = item.ScratchTotal;//14
                newRow["Squad"] = item.Squad;//15
                newRow["Handicap"] = item.Handicap;//16
                newRow["Bonus"] = item.Bonus;//17
                newRow["Notes"] = item.Notes;//18
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
                if(item.UseGame1 == null)
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
                NewParticipant.Bonus = (int)item.Bonus;
                ParticipantList.Add(NewParticipant);
            }

            return ParticipantList;
        }




        //makes a list from the finalizetemp table to be used in dataview source
        public List<FinalizeTemp> GetListFromTable()
        {
            var db = new NineTapDb();
            List<FinalizeTemp> ParticipantList = new List<FinalizeTemp>();
            var temp = (from p in db.FinalizeTemp
                        orderby p.GameId descending
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
                            p.Bonus


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
                ParticipantList.Add(NewParticipant);
            }

            return ParticipantList;
        }
        //Updates the finalizetemp table when check box for Use Game Score is clicked on.
        private void dataGridView1_OnCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), 3.ToString()) == 0)
            {
                bool checkBoxStatus = Convert.ToBoolean(dataGridView1.CurrentCell.EditedFormattedValue);
                //checkBoxStatus gives you whether checkbox cell value of selected row for the
                //"CheckBoxColumn" column value is checked or not. 
                FinalizeTemp temp = new FinalizeTemp();
                var row = dataGridView1.CurrentCell.RowIndex;
                NineTapDb db = new NineTapDb();
                if (checkBoxStatus)
                {
                    dataGridView1.Rows[row].Cells[3].Value = true;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, 2, true);

                }
                else
                {
                    dataGridView1.Rows[row].Cells[3].Value = false;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, 2, false);
                }
            }
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), 5.ToString()) == 0)
            {
                bool checkBoxStatus = Convert.ToBoolean(dataGridView1.CurrentCell.EditedFormattedValue);
                //checkBoxStatus gives you whether checkbox cell value of selected row for the
                //"CheckBoxColumn" column value is checked or not. 
                var row = dataGridView1.CurrentCell.RowIndex;
                if (checkBoxStatus)
                {
                    dataGridView1.Rows[row].Cells[5].Value = true;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, 4, true);

                }
                else
                {
                    dataGridView1.Rows[row].Cells[5].Value = false;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, 4, false);
                }
            }
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), 7.ToString()) == 0)
            {
                bool checkBoxStatus = Convert.ToBoolean(dataGridView1.CurrentCell.EditedFormattedValue);
                //checkBoxStatus gives you whether checkbox cell value of selected row for the
                //"CheckBoxColumn" column value is checked or not. 
                var row = dataGridView1.CurrentCell.RowIndex;
                if (checkBoxStatus)
                {
                    dataGridView1.Rows[row].Cells[7].Value = true;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, 6, true);

                }
                else
                {
                    dataGridView1.Rows[row].Cells[7].Value = false;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, 6, false);

                }
            }
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), 9.ToString()) == 0)
            {
                bool checkBoxStatus = Convert.ToBoolean(dataGridView1.CurrentCell.EditedFormattedValue);
                //checkBoxStatus gives you whether checkbox cell value of selected row for the
                //"CheckBoxColumn" column value is checked or not. 
                var row = dataGridView1.CurrentCell.RowIndex;
                if (checkBoxStatus)
                {
                    dataGridView1.Rows[row].Cells[9].Value = true;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, 8, true);


                }
                else
                {
                    dataGridView1.Rows[row].Cells[9].Value = false;
                    UpdateAvg(row);
                    CheckBoxDBSet(row, 8, false);

                }
            }
        }

        private void dataGridView1_OnCellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // End of edition on each click on column of checkbox
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), 3.ToString()) == 0)
            {
                dataGridView1.EndEdit();
            }
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), 5.ToString()) == 0)
            {
                dataGridView1.EndEdit();
            }
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), 7.ToString()) == 0)
            {
                dataGridView1.EndEdit();
            }
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), 9.ToString()) == 0)
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
            var GameId = Convert.ToInt32(dataGridView1.Rows[row].Cells[0].Value);
            temp = db.FinalizeTemp.First(f => f.GameId == GameId);
            if (cell == 2)
                temp.UseGame1 = set;
            if (cell == 4)
                temp.UseGame2 = set;
            if (cell == 6)
                temp.UseGame3 = set;
            if (cell == 8)
                temp.UseGame4 = set;
            temp.GameAvg = Convert.ToInt32(dataGridView1.Rows[row].Cells[11].Value);
            temp.ScratchTotal = Convert.ToInt32(dataGridView1.Rows[row].Cells[14].Value);
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
            if (Convert.ToBoolean(dataGridView1.Rows[row].Cells[3].Value) == true)
            {
                sum += Convert.ToInt32((dataGridView1.Rows[row].Cells[2].Value));
                count++;
            }
            if (Convert.ToBoolean(dataGridView1.Rows[row].Cells[5].Value) == true)
            {
                sum += Convert.ToInt32((dataGridView1.Rows[row].Cells[4].Value));
                count++;
            }
            if (Convert.ToBoolean(dataGridView1.Rows[row].Cells[7].Value) == true)
            {
                sum += Convert.ToInt32((dataGridView1.Rows[row].Cells[6].Value));
                count++;
            }
            if (Convert.ToBoolean(dataGridView1.Rows[row].Cells[9].Value) == true)
            {
                sum += Convert.ToInt32((dataGridView1.Rows[row].Cells[8].Value));
                count++;
            }
            if (count == 0)
            {
                dataGridView1.Rows[row].Cells[11].Value = 0;
                dataGridView1.Rows[row].Cells[14].Value = 0;
            }
            else
            {
                dataGridView1.Rows[row].Cells[11].Value = sum / count;
                dataGridView1.Rows[row].Cells[14].Value = sum;
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

        public void RankGridView()
        {
            int Rank = 1;

            for (int Row = 0; Row < dataGridView1.Rows.Count; Row++)
            {
                dataGridView1.Rows[Row].Cells[0].Value = Rank;
                //Here we are updatng placestandings by adjustedAvg. Should we Rank by trueavg?
                if (Convert.ToInt32(dataGridView1.Rows[Row].Cells[12].Value) != Convert.ToInt32(dataGridView1.Rows[Row + 1].Cells[12].Value))
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
            if(this.dataGridView1.Columns[e.ColumnIndex].Name == "Valid Score1?" && e.Value != null)
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
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "Valid Score2?" && e.Value != null)
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
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "Valid Score3?" && e.Value != null)
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
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "Valid Score4?" && e.Value != null)
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
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "30 Game Avg" && e.Value != null)
            {
                if(Convert.ToInt32(e.Value) > Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 2].Value) + 50 && dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 2].Style.BackColor != Color.Red)
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

    }
}

