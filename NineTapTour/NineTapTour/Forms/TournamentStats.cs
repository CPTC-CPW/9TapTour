using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NineTapTour.Models;

namespace NineTapTour.Forms
{
    public partial class TournamentStats : Form
    {
        public TournamentStats()
        {
            InitializeComponent();
        }

        private void TournamentStats_Load(object sender, EventArgs e)
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

                dgvTournamentStats.DataSource = tournamentStatsList;                               
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

                try
                {
                    // open connection
                    con.Open();

                    // execute command(query)
                    SqlDataReader reader = gameOrder.ExecuteReader();
                    List<TournamentStatsList> listOfTourney = new List<TournamentStatsList>();

                    // view results
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

                        listOfTourney.Add(temp);
                    }                    
                    dgvTournamentStats.DataSource = listOfTourney;
                }
                catch (SqlException)
                {

                }
                finally
                {
                    con.Dispose();
                }
            }
        }

        public static List<int> GetTop3OutOf4(List<int?> scores)
        {            
            List<int> listOfValidScores = new List<int>();
            for(int i = 0; i < scores.Count; i++)
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

        public static string GetConnection()
        {
            return ConfigurationManager.ConnectionStrings["NineTapDbConnection"].ConnectionString;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            printDialog1.Document = printDocument1;
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap bm = new Bitmap(this.dgvTournamentStats.Width, this.dgvTournamentStats.Height);
            this.dgvTournamentStats.DrawToBitmap(bm, new Rectangle(0, 0, this.dgvTournamentStats.Width, this.dgvTournamentStats.Height));
            e.Graphics.DrawImage(bm, 0, 0);
        }
    }

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
