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
    public partial class FrmFinalizeTournament : Form
    {

        List<GameParticipant> ListGameParticipants = new List<GameParticipant>();


        public FrmFinalizeTournament(Tournament tourn)
        {
            Tournament temptourn = tourn;
            InitializeComponent();

            ListGameParticipants = GetAllParticipantGameList(tourn);

            //dataGridView1.DataSource = DataView(temptourn, ListGameParticipants); //By default populates all datagrid with all participant for tournament.

            //Sort DataGridView by TrueAverage
            //this.dataGridView1.Sort(this.dataGridView1.Columns["True Avg"], ListSortDirection.Descending);

            //sets sizes of check box columns "Valid Score1, ValidScore2, ValidScore3, Valid Score 4, and Keep True Avg?"
            var column = dataGridView1.Columns[3];
            column.Width = 50;
            var column1 = dataGridView1.Columns[5];
            column1.Width = 50;
            var column2 = dataGridView1.Columns[7];
            column2.Width = 50;
            var column3 = dataGridView1.Columns[9];
            column3.Width = 50;
            var column4 = dataGridView1.Columns[12];
            column4.Width = 40;

        }


        //creates the dataview that will populate the datagridview table on form
        public DataTable DataView(Tournament tourn, List<GameParticipant> participantsList)
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
            dt.Columns.Add("True Avg").ReadOnly = true;
            dt.Columns.Add("Adj Avg");
            dt.Columns.Add(new DataColumn("Keep True Avg?", typeof(bool)));
            dt.Columns.Add("Scratch Total").ReadOnly = true;
            dt.Columns.Add("Squad").ReadOnly = true;
            dt.Columns.Add("Game Avg").ReadOnly = true;
            dt.Columns.Add("Handicap").ReadOnly = true;
            dt.Columns.Add("Bonus").ReadOnly = true;
            dt.Columns.Add("Pro Pot");
            dt.Columns.Add("Notes");

            //whatever list of participants you pass into method will be populated into grid
            List<GameParticipant> temp = participantsList;

            //loops thru each person's info in tournament and populates the dataview with data from DB.
            foreach (var item in temp)
            {
                DataRow newRow = dt.NewRow();
                newRow["GameId"] = item.GameId;
                newRow["Name"] = item.FirstName + " " + item.LastName;
                newRow["Game 1"] = item.Game1;
                newRow["Valid Score1?"] = item.UseGame1;
                newRow["Game 2"] = item.Game2;
                newRow["Valid Score2?"] = item.UseGame2;
                newRow["Game 3"] = item.Game3;
                newRow["Valid Score3?"] = item.UseGame3;
                newRow["Game 4"] = item.Game4;
                newRow["Valid Score4?"] = item.UseGame4;
                //TODO: Base this off historical records for member off their last 30 games.
                newRow["True Avg"] = (item.Game1 + item.Game2 + item.Game3 + item.Game4) / 4;
                //TODO: Add field in member to hold adjusted average that's inputted by user based of calculated "True avg";
                newRow["Adj Avg"] = GetAdjustedAverage(item);
                newRow["Keep True Avg?"] = false;
                newRow["Scratch Total"] = item.Game1 + item.Game2 + item.Game3 + item.Game4;
                newRow["Squad"] = item.Squad;
                newRow["Game Avg"] = (item.Game1 + item.Game2 + item.Game3 + item.Game4) / 4;
                newRow["Handicap"] = item.Handicap;
                newRow["Bonus"] = item.Bonus;
                newRow["Notes"] = item.Notes;
                dt.Rows.Add(newRow);

            }


            return dt;
        }

        /// <summary>
        /// Checks to see if any games are not used in a game average.
        /// Then calculates and return Adjusted Game average.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int GetAdjustedAverage(GameParticipant p)
        {
            int AdjustedAverage = 0;
            int count = 0;

            if(p.UseGame1 == true)
            {
                AdjustedAverage += p.Game1;
                count++;
            }
            if (p.UseGame2 == true)
            {
                AdjustedAverage += p.Game2;
                count++;
            }
            if (p.UseGame3 == true)
            {
                AdjustedAverage += p.Game3;
                count++;
            }
            if (p.UseGame4 == true)
            {
                AdjustedAverage += p.Game4;
                count++;
            }

            return AdjustedAverage / count;

        }

        /// <summary>
        /// Gets list of Game Participants by squad
        /// </summary>
        /// <param name="tourn">active tournament</param>
        /// <param name="Squad">squad you want a list of </param>
        /// <returns>returns list of Participants from specified squad</returns>
        public List<GameParticipant> GameParticipantsBySquadList(Tournament tourn, int Squad)
        {
            List<GameParticipant> allParticipants = ParticipantSortByScore(tourn);
            List<GameParticipant> SquadParticipants = new List<GameParticipant>();
            foreach (GameParticipant p in allParticipants)
            {
                if (p.Squad == Squad)
                {
                    SquadParticipants.Add(p);
                }
            }
            return SquadParticipants;
        }

        /// <summary>
        /// THis method Gets a list of all participant objects for the tournament passed into method.
        /// </summary>
        /// <param name="tourn"> represent the tournament you want list of particpants from</param>
        /// <returns>List of Participants for specific tournament</returns>
        public List<GameParticipant> GetAllParticipantGameList(Tournament tourn)
        {
            var db = new NineTapDb();
            List<GameParticipant> ParticipantList = new List<GameParticipant>();
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
                GameParticipant NewParticipant = new GameParticipant();
                NewParticipant.GameId = item.Id;
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
                //TODO: Base this off historical records for member off their last 30 games.
                NewParticipant.ScratchTotal = (int)(item.Game1 + item.Game2 + item.Game3 + item.Game4);
                NewParticipant.Squad = item.Squad;
                NewParticipant.GameAvg = (int)(item.Game1 + item.Game2 + item.Game3 + item.Game4) / 4;
                NewParticipant.Handicap = (int)item.Handicap;
                NewParticipant.Bonus = (int)item.Bonus;
                ParticipantList.Add(NewParticipant);
            }

            return ParticipantList;
        }

        /// <summary>
        /// This method will get a list of all tournament participants and return a sort the list by scores.
        /// </summary>
        /// <param name="tourn">Tournament needing information from</param>
        /// <returns>sorted list of gameParticipants for specified tournament</returns>
        public List<GameParticipant> ParticipantSortByScore(Tournament tourn)
        {
            List<GameParticipant> sortParticipant = GetAllParticipantGameList(tourn);
            sortParticipant.Sort(delegate (GameParticipant c1, GameParticipant c2) { return c1.GameAvg.CompareTo(c2.GameAvg); });
            return sortParticipant;
        }

        /// <summary>
        /// gets list of Winning Participants
        /// </summary>
        /// <param name="tourn">active tournament</param>
        /// <returns>returns list of Top Scoring Participants</returns>
        public List<GameParticipant> TopScoreParticipants(Tournament tourn)
        {
            //This Method needs some work... to get the right list but more or less is the basic design.
            List<GameParticipant> SortedParticipantsByScore = ParticipantSortByScore(tourn);
            List<GameParticipant> TopParticipantsList = new List<GameParticipant>();

            //reverse instantiation if my sort method is backwards and 
            // add business rules applied to determine correct list of winners.
            for (int i = 0; i == 4; i++)
            {
                TopParticipantsList.Add(SortedParticipantsByScore[i]);
            }
            return TopParticipantsList;
        }

 
        /// <summary>
        /// This method doesnt have to return bool-GET GROUP CONSCIENCE
        /// This Method will save member and Game data to database.
        /// </summary>
        /// <returns>True if works, false if not.</returns>
        public bool SaveTournamentData()
        {
            var db = new NineTapDb();
            foreach (GameParticipant UpdatedParticipant in ListGameParticipants)
            {
                //Need to test this method to see whether entity framework will pull correct fields from
                // GameParticipant which contains data from two different tables.
                // If not I will manually have to grab selected fields and updata individually.
                var GameOriginal = db.Games.Find(UpdatedParticipant.GameId);
                var MemberOriginal = db.Members.Find(UpdatedParticipant.MemberId);

                if (GameOriginal != null)
                {
                    db.Entry(GameOriginal).CurrentValues.SetValues(UpdatedParticipant.GameId);
                    db.Entry(MemberOriginal).CurrentValues.SetValues(UpdatedParticipant.MemberId);
                    db.SaveChanges();// how can you test in entity if values saved properly.
                }

            }
            return true;
        }

        /// <summary>
        /// This Method Changes boolean values in Gameparticipants UseGame in 
        /// Global variable, ListGameparticipants if a checkbox has been 
        /// changed in the GridView.- this method is specific to checkboxes.
        /// </summary>
        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            //goes through every participant to see if row matches memberid
            foreach (GameParticipant p in ListGameParticipants)
            {
                if (Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value) == p.GameId)
                {
                    //changes value of game 1
                    if (dataGridView1.CurrentCell.ColumnIndex == 3)
                    {
                        if (p.UseGame1 == true)
                        {
                            p.UseGame1 = false;
                        }
                        else p.UseGame1 = true;
                    }
                    //changes value of game 2
                    if (dataGridView1.CurrentCell.ColumnIndex == 5)
                    {
                        if (p.UseGame2 == true)
                        {
                            p.UseGame2 = false;
                        }
                        else p.UseGame2 = true;
                    }
                    //changes value of game 3
                    if (dataGridView1.CurrentCell.ColumnIndex == 7)
                    {
                        if (p.UseGame3 == true)
                        {
                            p.UseGame3 = false;
                        }
                        else p.UseGame3 = true;
                    }
                    //changes value of game 4
                    if (dataGridView1.CurrentCell.ColumnIndex == 9)
                    {
                        if (p.UseGame4 == true)
                        {
                            p.UseGame4 = false;
                        }
                        else p.UseGame4 = true;
                    }

                    //Changes value of keeptrueAVG
                    //if (dataGridView1.CurrentCell.ColumnIndex == 13)
                    //{
                    //if (p.KeepTrueAvg == true)
                    //{
                    //    p.KeepTrueAvg = false;
                    //}
                    //else KeepTrueAvg = true;
                    //}

                }
            }
        }
 
        /// Method will update GameParticpants in Global variable, ListGameparticipants
        /// if a value has been changed in the GridView.- !This does NOT include checkboxes!
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
           //goes through every participant to see if row matches memberid
          foreach (GameParticipant p in ListGameParticipants)
          {
              if ((int)dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value == p.GameId)
             {
                    //Update Notes
                 if (dataGridView1.CurrentCell.ColumnIndex == 20)
                    {
                        p.Notes = dataGridView1.CurrentCell.Value.ToString();
                    }
  
                      //To do Change Adjusted Avg
                      if(dataGridView1.CurrentCell.ColumnIndex == 12)
                     {
                         p.imputtedAvg = Convert.ToInt32(dataGridView1.CurrentCell.Value);
                     }
                }
             }
         }

    }
           
        

        /// <summary>
        /// This Class represents an object of Participant Info for a specific game.
        /// </summary>
        public class GameParticipant
        {

            public int GameId { get; set; }
            public int MemberId { get; set; }
            public String FirstName { get; set; }
            public String LastName { get; set; }
            public int Squad { get; set; }
            public int Game1 { get; set; }
            public int Game2 { get; set; }
            public int Game3 { get; set; }
            public int Game4 { get; set; }
            public bool UseGame1 { get; set; }
            public bool UseGame2 { get; set; }
            public bool UseGame3 { get; set; }
            public bool UseGame4 { get; set; }
            public string Notes { get; set; }
            public int ScratchTotal { get; set; }
            public int imputtedAvg { get; set; } //Avg manually changed by user
            public int GameAvg { get; set; }
            public int Handicap { get; set; }
            public int Bonus { get; set; }

        }

     
    }

