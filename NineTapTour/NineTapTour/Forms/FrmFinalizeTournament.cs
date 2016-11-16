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

        public FrmFinalizeTournament(Tournament tourn)
        {
            Tournament temptourn = tourn;
            InitializeComponent();
            dataGridView1.DataSource = DataView(temptourn, GetAllParticipantGameList(tourn)); //By default populates all datagrid with all participant for tournament.            
            dataGridViewTableFormat(dataGridView1);
        }


        //formatting for the datagrid view on form.  Sets column widths for check boxes and sets read only on columns not to be editted.
        private void dataGridViewTableFormat(DataGridView dataGridView1)
        {
            var column = dataGridView1.Columns[2];
            column.Width = 50;
            var column1 = dataGridView1.Columns[4];
            column1.Width = 50;
            var column2 = dataGridView1.Columns[6];
            column2.Width = 50;
            var column3 = dataGridView1.Columns[8];
            column3.Width = 50;
            var column4 = dataGridView1.Columns[11];
            column4.Width = 40;
            dataGridView1.Columns["Game 1"].ReadOnly = true;
            dataGridView1.Columns["Game 2"].ReadOnly = true;
            dataGridView1.Columns["Game 3"].ReadOnly = true;
            dataGridView1.Columns["Game 4"].ReadOnly = true;
            dataGridView1.Columns["True Avg"].ReadOnly = true;
            dataGridView1.Columns["Scratch Total"].ReadOnly = true;
            dataGridView1.Columns["Squad"].ReadOnly = true;
            dataGridView1.Columns["Game Avg"].ReadOnly = true;
            dataGridView1.Columns["Handicap"].ReadOnly = true;
            dataGridView1.Columns["Bonus"].ReadOnly = true;
            dataGridView1.Columns["Pro Pot"].ReadOnly = true;
        }


        //creates the dataview that will populate the datagridview table on form
        public DataTable DataView(Tournament tourn, List<GameParticipant> participantsList)
        {
            var db = new NineTapDb();
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            
            dt.Columns.Add("Game 1");
            dt.Columns.Add(new DataColumn("Valid Score1?", typeof(bool)));
            dt.Columns.Add("Game 2");
            dt.Columns.Add(new DataColumn("Valid Score2?", typeof(bool)));
            dt.Columns.Add("Game 3");
            dt.Columns.Add(new DataColumn("Valid Score3?", typeof(bool)));
            dt.Columns.Add("Game 4");
            dt.Columns.Add(new DataColumn("Valid Score4?", typeof(bool)));
            dt.Columns.Add("True Avg");
            dt.Columns.Add("Adj Avg");
            dt.Columns.Add(new DataColumn("Keep True Avg?", typeof(bool)));
            dt.Columns.Add("Scratch Total");
            dt.Columns.Add("Squad");
            dt.Columns.Add("Game Avg");
            dt.Columns.Add("Handicap");
            dt.Columns.Add("Bonus");
            dt.Columns.Add("Pro Pot");
            dt.Columns.Add("Notes");

            //whatever list of participants you pass into method will be populated into grid
            List<GameParticipant> temp = participantsList;

            //loops thru each person's info in tournament and populates the dataview with data from DB.
            foreach (var item in temp)
            {
                DataRow newRow = dt.NewRow();
                newRow["Name"] = item.FirstName + " " + item.LastName;
                newRow["Game 1"] = item.Game1;
                newRow["Valid Score1?"] = true; 
                newRow["Game 2"] = item.Game2;
                newRow["Valid Score2?"] = true;
                newRow["Game 3"] = item.Game3;
                newRow["Valid Score3?"] = true;
                newRow["Game 4"] = item.Game4;
                newRow["Valid Score4?"] = true;
                //TODO: Base this off historical records for member off their last 30 games.
                newRow["True Avg"] = (item.Game1 + item.Game2 + item.Game3 + item.Game4)/4;
                //TODO: Add field in member to hold adjusted average that's inputted by user based of calculated "True avg";
                newRow["Adj Avg"] = 0;
                newRow["Keep True Avg?"] = false;
                newRow["Scratch Total"] = item.Game1 + item.Game2 + item.Game3 + item.Game4;
                newRow["Squad"] = item.Squad;
                newRow["Game Avg"] = (item.Game1 + item.Game2 + item.Game3 + item.Game4) / 4;
                newRow["Handicap"] = item.Handicap;
                newRow["Bonus"] = item.Bonus;
                dt.Rows.Add(newRow);

            }


            return dt;
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
                if(p.Squad == Squad)
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
                            m.FirstName,
                            m.LastName,
                            p.Squad,
                            g.Game1,
                            g.Game2,
                            g.Game3,
                            g.Game4,
                            g.Handicap,
                            g.Bonus,


                        }).ToList();
            foreach (var item in temp)
            {
                GameParticipant NewParticipant = new GameParticipant();
                NewParticipant.FirstName = item.FirstName;
                NewParticipant.LastName = item.LastName;
                NewParticipant.Game1 = (int)item.Game1;
                NewParticipant.Game2 = (int) item.Game2;
                NewParticipant.Game3 = (int) item.Game3;
                NewParticipant.Game4 = (int) item.Game4;
                //TODO: Base this off historical records for member off their last 30 games.
                NewParticipant.ScratchTotal = (int) (item.Game1 + item.Game2 + item.Game3 + item.Game4);
                NewParticipant.Squad = item.Squad;
                NewParticipant.GameAvg = (int) (item.Game1 + item.Game2 + item.Game3 + item.Game4) / 4;
                NewParticipant.Handicap = (int) item.Handicap;
                NewParticipant.Bonus =(int)item.Bonus;
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
        /// This Class represents an object of Participant Info for a specific game.
        /// </summary>
        public class GameParticipant {

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
            public int GameAvg { get; set; }
            public int Handicap { get; set; }
            public int Bonus { get; set; }

        }


    }
}
