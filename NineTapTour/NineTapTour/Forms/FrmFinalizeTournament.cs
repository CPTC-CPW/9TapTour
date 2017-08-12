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
   
        public FrmFinalizeTournament(Tournament tourn)
        {
            InitializeComponent();
            this.dataGridView1.DoubleBuffered(false);
            this.currentT = tourn;
            FinalizeTableList = GetAllInitialParticipantGameList(tourn);
            createDataGridView(tourn);
        }



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
                    temp.LeagueAverage = Convert.ToInt32(LeagueAvgFromPlayerHistory(item.MemberId));
                }
                else
                {
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
                    temp.LeagueAverage = Convert.ToInt32(LeagueAvgFromPlayerHistory(item.MemberId));
                    temp.Handicap = item.Handicap;
                    temp.Bonus = item.Bonus;
                }
                FinalizeTempDB.AddFinalizeTempOnstart(temp);
            }
            //pulls a list from the finalizetemp table and seeds the dataview with the table info.
            List<FinalizeTemp> DataViewList = GetListFromTable(tourn);
            dataGridView1.DataSource = DataView(tourn, DataViewList); //By default populates all datagrid with all participant for tournament.


            ////Sort DataGridView by TrueAverage
            //this.dataGridView1.Sort(this.dataGridView1.Columns["True Avg"], ListSortDirection.Descending);

            //sets sizes of check box columns "Valid Score1, ValidScore2, ValidScore3, Valid Score 4, and Keep True Avg?"
            dataGridView1.SuspendLayout();
            var column = dataGridView1.Columns[1];
            for (int i = 2; i <= 12; i++)
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
            dt.Columns.Add(new DataColumn("Director Check", typeof(bool)));
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
                if (item.FirstName.Length > 1)
                {
                    string[] aftersplit = item.FirstName.Split(' ');
                    newRow["Name"] = aftersplit[0] + " " + item.LastName;//1
                }
               else
                {
                    newRow["Name"] = item.FirstName + " " + item.LastName;//1
                }
                
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
                newRow["Director Check"] = false;//13
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
                        orderby p.FirstName, p.ScratchTotal ascending
                        join t in db.Tournaments on p.TournamentID equals t.Id
                        where tourn.Id == t.Id
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
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), 13.ToString()) == 0)
            {
                int gameId = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value);
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
                            dataGridView1.Rows[i].Cells[13].Value = true;
                        }
                    }
                    
                }
                else
                {
                    for (int i = 0; i < FinalizeTableList.Count; i++)
                    {
                        if (FinalizeTableList[i].MemberId == memId)
                        {
                            dataGridView1.Rows[i].Cells[13].Value = false;
                        }
                    }


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
            if (string.Compare(dataGridView1.CurrentCell.OwningColumn.Index.ToString(), 13.ToString()) == 0)
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
                return (avg = sum / temp.Count());
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
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "Valid Score1?" && e.Value != null)
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


        private void check_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            dataGridView1.SuspendLayout();
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "Valid Score1?" && e.Value != null)
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
            int gameId = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value);



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


            PlayerHistory ph = new PlayerHistory();
            List<FinalizeTemp> FinalizeTableList = GetListFromTable(currentT);
            List<Participant> partlist = TournamentDb.GetTournamentMemberList(currentT);


            for (int i = 0; i < FinalizeTableList.Count; i++)
            {
                if (dataGridView1[13, i].Value.ToString() == "False") 
                {
                    dataGridView1.Rows[i].Cells[13].Style.BackColor = Color.Red;
                }
                else if(dataGridView1[13, i].Value.ToString() == "True")
                {
                    dataGridView1.Rows[i].Cells[13].Style.BackColor = Color.White;
                }

            }

            for (int i = 0; i <= FinalizeTableList.Count - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[13].Style.BackColor == Color.Red)
                {
                    break;
                }

                for (int p = 0; p < partlist.Count; p++)
                {
                    partlist[p].Tournament = currentT;
                    partlist[p].Game = FinalizeTempDB.getGame(FinalizeTableList[p].GameId);
                }

                int gamesPlayed = 0;
                int currentindex = 0;

                foreach (var item in FinalizeTableList)
                {

                    gamesPlayed = 0;
                    ph.GameID = item.GameId;
                    Game g = FinalizeTempDB.getGame(item.GameId);
                    int memId;
                    using (var db = new NineTapDb())
                    {
                        memId = db.Participants.Include(b => b.Game).Include(b => b.Member).First(p => p.Game.Id == g.Id).Member.Id;
                    }
                    Member currentMember = MemberDb.GetMember(memId);


                    ph.TournamentDate = currentT.Date;
                    ph.MemberNumber = item.MemberId;

                    if (dataGridView1[3, currentindex].Value.ToString() == "True")
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
                    if (dataGridView1[5, currentindex].Value.ToString() == "True")
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
                    if (dataGridView1[7, currentindex].Value.ToString() == "True")
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
                    if (dataGridView1[9, currentindex].Value.ToString() == "True")
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
                    ph.AverageForGame = item.GameAvg;
                    ph.trueAVG = LeagueAvgFromPlayerHistory(ph.MemberNumber);
                    ph.AVG = Convert.ToInt32(dataGridView1[12, currentindex].Value);
                    ph.ProPot = dataGridView1[18, currentindex].Value.ToString();
                    for (int j = 0; j < partlist.Count; j++)
                    {
                        if (ph.MemberNumber == partlist[j].Member.Number)
                        {

                            ph.MoneyWon = Convert.ToDecimal(partlist[j].Game.MoneyWon);
                        }
                    }

                    g.Bonus = item.Bonus;
                    ph.Bonus = item.Bonus;
                    g.Game1 = item.Game1;
                    ph.Game1 = item.Game1;
                    g.Game2 = item.Game2;
                    ph.Game2 = item.Game2;
                    g.Game3 = item.Game3;
                    ph.Game3 = item.Game3;
                    g.Game4 = item.Game4;
                    ph.Game4 = item.Game4;



                    g.Handicap = item.Handicap;
                    ph.HandiCap = item.Handicap;
                    g.InputtedAvg = ph.AVG;
                    g.Notes = dataGridView1[19, currentindex].Value.ToString();
                    ph.Notes = g.Notes;
                    currentMember.StartAvg = ph.AVG;
                    ph.hisID = PlayerHistoryDB.getHisID(ph);
                    PlayerHistoryDB.AddPlayerHistory2(ph);
                    MemberDb.AddMember(currentMember);
                    FinalizeTableList[currentindex].FinalizeID = FinalizeTempDB.getFinalizeID(g).FinalizeID;
                    FinalizeTableList[currentindex].AdjustedAvg = ph.AVG;
                    FinalizeTempDB.AddFinalizeTempOnFinalize(FinalizeTableList[currentindex]);
                    currentindex++;


                }

                //THIS WILL ADD A BONUS PIN DEPENDING ON HOW MANY THEY CURRENTLY HAVE AND IF THERE LAST 3 TOURNAMENTS RESULT IN NOT CASHING OR PLACING
                //List<int> alreadychecked = new List<int>();

                //for (int bonus = 0; bonus <= FinalizeTableList.Count - 1; bonus++)
                //{
                //    //skip over the current person if they bonus pin was already adjusted this tournament
                //    if (alreadychecked.Contains(FinalizeTableList[bonus].MemberId))
                //    {
                //        break;
                //    }
                //    else
                //    {
                //        //max of five bonus pins allowed, if at 5 keep at five until needs to be adjusted from cashing out or winning tournament
                //        if (FinalizeTableList[bonus].Bonus != 5)
                //        {
                //            Member current = MemberDb.GetMember(FinalizeTableList[bonus].MemberId);
                //            current.Bonus = 0;
                //            List<PlayerHistory> playerhistory = PlayerHistoryDB.getLastFiveFromPlayerhistory(FinalizeTableList[i].MemberId);
                //            if (playerhistory.Count <= 3)
                //            {
                //                alreadychecked.Add(current.Number);
                              
                //            }
                //            else
                //            {
                //                //if the last 3 tournaments  were not on the same day, meaning there last three recorded history were not at the same tournament and the last 3 recorded bonus pins are equal to the current members bonus pins
                //                if (
                //                   playerhistory[1].TournamentDate != playerhistory[2].TournamentDate &&
                //                   playerhistory[2].TournamentDate != playerhistory[3].TournamentDate &&
                //                   playerhistory[3].TournamentDate != playerhistory[1].TournamentDate &&
                //                   playerhistory[1].Bonus == playerhistory[2].Bonus &&
                //                   playerhistory[2].Bonus == playerhistory[3].Bonus &&
                //                   playerhistory[3].Bonus == playerhistory[1].Bonus)
                //                {
                //                    current = MemberDb.GetMember(FinalizeTableList[bonus].MemberId);
                //                    current.Bonus += 1;
                //                    MemberDb.AddMember(current);
                //                    alreadychecked.Add(current.Number);
                //                }

                                    
              
                //              }
                //            }
                //          }
                //        }

                Close();

            }
        

        }
       

    }





        /***/

    }



