using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NineTapTour.Models;
using System.Collections;

namespace NineTapTour.Forms
{
    /// <summary>
    /// What does this class do?
    /// </summary>
    public partial class TournamentStats : Form
    {
        /// <summary>
        /// TournamentStats() begins when for is instantiated.
        /// </summary>
        public TournamentStats()
        {
            InitializeComponent();
        }

        /// <summary>
        /// TournamentStats_Load() is the main method that populates the form initially.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TournamentStats_Load(object sender, EventArgs e)
        {
            if (!frmMemberScores.selectedTournament.ThreeOutOf4)
            {
                Tournament selectedTournament = new Tournament();
                selectedTournament = frmMemberScores.selectedTournament;
                lblTournamentName.Text = "Tournament ID: (" + selectedTournament.Id + ")\nTournament Location: " + selectedTournament.Location + "\nDate: " + selectedTournament.Date;

                NineTapDb db = new NineTapDb();
                var tournamentStatsList = (from p in db.Participants
                                           join m in db.Members on p.Member.Id equals m.Id
                                           join g in db.Games on p.Game.Id equals g.Id
                                           join t in db.Tournaments on p.Tournament.Id equals t.Id
                                           where t.Id == selectedTournament.Id
                                           orderby (g.Game1 + g.Game2 + g.Game3 + g.Game4) descending
                                           select new
                                           {
                                               p.Member.Number,
                                               p.Member.FirstName,
                                               p.Member.LastName,
                                               p.Squad,
                                               ScratchTotal = ((g.Game1.HasValue ? g.Game1 : 0) + (g.Game2.HasValue ? g.Game2 : 0) + (g.Game3.HasValue ? g.Game3 : 0) + (g.Game4.HasValue ? g.Game4 : 0)),
                                               GameTotal = (((g.Game1.HasValue ? g.Game1 : 0) + (g.Handicap + g.Bonus)) + ((g.Game2.HasValue ? g.Game2 : 0) + (g.Handicap + g.Bonus)) + ((g.Game3.HasValue ? g.Game3 : 0) + (g.Handicap + g.Bonus)) + ((g.Game4.HasValue ? g.Game4 : 0) + (g.Handicap + g.Bonus))),
                                               g.Game1,
                                               g.Game2,
                                               g.Game3,
                                               g.Game4,
                                               p.Game.Handicap,
                                               p.Game.Bonus
                                           }).ToList();

                List<TournamentStatsList> statsList = new List<TournamentStatsList>();
                foreach (var item in tournamentStatsList)
                {
                    TournamentStatsList list = new TournamentStatsList
                    {
                        Id = item.Number,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Squad = item.Squad,
                        ScratchTotal = item.ScratchTotal,
                        Top3Scores = item.ScratchTotal + (item.Handicap * 3) + (item.Bonus * 3),
                        Game1 = item.Game1,
                        Game2 = item.Game2,
                        Game3 = item.Game3,
                        Game4 = item.Game4,
                        Handicap = item.Handicap,
                        Bonus = item.Bonus
                    };
                    statsList.Add(list);
                }

                dgvTournamentStats.DataSource = BuildDataTable(statsList);
            }
            else
            {
                Tournament selectedTournament = new Tournament();
                selectedTournament = frmMemberScores.selectedTournament;
                lblTournamentName.Text = "Tournament ID: (" + selectedTournament.Id + ")\nTournament Location: " + selectedTournament.Location + "\nDate: " + selectedTournament.Date;

                NineTapDb db = new NineTapDb();

                SqlConnection con = new SqlConnection(GetConnection());
                SqlCommand gameOrder = new SqlCommand();
                gameOrder.Connection = con;
                gameOrder.CommandText = @"SELECT Members.Id, Members.FirstName, Members.LastName, Game1, Game2, Game3, Game4, Games.Handicap, Participants.SquadNumber, Games.Bonus
                                        FROM Games JOIN Participants ON Games.Id = Participants.Game_Id
		                                JOIN Tournaments ON Participants.Tournament_Id = Tournaments.Id
		                                JOIN Members ON Members.Id = Participants.Member_Id                                        
                                        WHERE Tournament_Id = @TID
                                        ORDER BY Members.LastName";

                gameOrder.Parameters.AddWithValue("@TID", selectedTournament.Id);

                
                // Open connection
                con.Open();

                // Execute command(query)
                SqlDataReader reader = gameOrder.ExecuteReader();
                List<TournamentStatsList> statsList = new List<TournamentStatsList>();

                // View results
                while (reader.Read())
                {
                    TournamentStatsList temp = new TournamentStatsList();
                    temp.Handicap = Convert.ToInt32(reader["Handicap"]);
                    temp.Bonus = Convert.ToInt32(reader["Bonus"]);
                    List<int?> scores = new List<int?> { Convert.ToInt32(reader["Game1"]), Convert.ToInt32(reader["Game2"]), Convert.ToInt32(reader["Game3"]), Convert.ToInt32(reader["Game4"]) };

                    List<int> topScores = GetTop3OutOf4(scores);
                    int scratchTotal = 0;

                    for (int i = 0; i < 3; i++)
                    {
                        scratchTotal += topScores[i];
                    }

                    temp.ScratchTotal = scratchTotal;
                    temp.Top3Scores = temp.ScratchTotal + (temp.Handicap * 3) + (temp.Bonus * 3);
                    temp.Id = Convert.ToInt32(reader["Id"]);
                    temp.FirstName = reader["FirstName"].ToString();
                    temp.LastName = reader["LastName"].ToString();
                    temp.Squad = Convert.ToInt32(reader["SquadNumber"]);
                    temp.Game1 = Convert.ToInt32(reader["Game1"]);
                    temp.Game2 = Convert.ToInt32(reader["Game2"]);
                    temp.Game3 = Convert.ToInt32(reader["Game3"]);
                    temp.Game4 = Convert.ToInt32(reader["Game4"]);

                    statsList.Add(temp);
                }

                dgvTournamentStats.DataSource = BuildDataTable(statsList);
            }
        }        

        /// <summary>
        /// What does this method do?
        /// </summary>
        /// <param name="scores"></param>
        /// <returns></returns>
        public static List<int> GetTop3OutOf4(List<int?> scores)
        {
            List<int> listOfValidScores = new List<int>();
            for (int i = 0; i < scores.Count - 1; i++)
            {
                if (scores[i].HasValue)
                {
                    listOfValidScores.Add(scores[i].Value);
                }
            }

            listOfValidScores.Sort();
            listOfValidScores.Reverse();
            return listOfValidScores;
        }

        /// <summary>
        /// GetConnection() returns a connection string to the database within the quotes.
        /// </summary>
        /// <returns>Database ConnectionString</returns>
        public static string GetConnection()
        {
            return ConfigurationManager.ConnectionStrings["NineTapDbConnection"].ConnectionString;
        }

        /// <summary>
        /// BtnPrint_Click() is called when Print button is clicked on the tournamentStats form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            printDialog1.Document = printDocument1;
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        /// <summary>
        /// PrintDocument1_PrintPage() is called after choosing where to save or print the tournamentStats table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap bm = new Bitmap(this.dgvTournamentStats.Width, this.dgvTournamentStats.Height);
            this.dgvTournamentStats.DrawToBitmap(bm, new Rectangle(0, 0, this.dgvTournamentStats.Width, this.dgvTournamentStats.Height));
            e.Graphics.DrawImage(bm, 0, 0);
        }

        /// <summary>
        /// BuildDataTable() Boxes up the tournamentStatsList object into a data table object 
        /// that the datagridview is willing to accept and sort.
        /// </summary>
        /// <param name="statsList"></param>
        /// <returns>Datatable object</returns>
        private DataTable BuildDataTable(List<TournamentStatsList> statsList)
        {
            DataTable data = new DataTable("Tournament Stats");

            data.Columns.Add("ID", System.Type.GetType("System.Int32"));
            data.Columns.Add("First Name", System.Type.GetType("System.String"));
            data.Columns.Add("Last Name", System.Type.GetType("System.String"));
            data.Columns.Add("Squad", System.Type.GetType("System.Int32"));
            data.Columns.Add("Scratch Total", System.Type.GetType("System.Int32"));
            data.Columns.Add("Top3Scores", System.Type.GetType("System.Int32"));
            data.Columns.Add("Game 1", System.Type.GetType("System.Int32"));
            data.Columns.Add("Game 2", System.Type.GetType("System.Int32"));
            data.Columns.Add("Game 3", System.Type.GetType("System.Int32"));
            data.Columns.Add("Game 4", System.Type.GetType("System.Int32"));
            data.Columns.Add("Handicap", System.Type.GetType("System.Int32"));
            data.Columns.Add("Bonus", System.Type.GetType("System.Int32"));

            // Make first four columns required
            for (int i = 0; i < 4; i++)
            {
                data.Columns[i].AllowDBNull = false;
            }

            // Make id unique
            data.Constraints.Add(new UniqueConstraint(data.Columns["ID"]));

            // Add statsList to DataTable
            foreach (var item in statsList)
            {
                data.Rows.Add(new object[]
                {
                item.Id,
                item.FirstName,
                item.LastName,
                item.Squad,
                item.ScratchTotal,
                item.Top3Scores,
                item.Game1,
                item.Game2,
                item.Game3,
                item.Game4,
                item.Handicap,
                item.Bonus
                });
            }

            // Return data table object
            return data;
        }

    }

    /// <summary>
    /// What does this class do?
    /// </summary>
    public partial class TournamentStatsList
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Squad { get; set; }
        public int? ScratchTotal { get; set; }
        public int? Top3Scores { get; set; }
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }
        public int? Handicap { get; set; }
        public int? Bonus { get; set; }
    }    
}
