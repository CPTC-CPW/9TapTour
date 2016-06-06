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
                                               p.Member.Id,
                                               p.Member.FirstName,
                                               p.Member.LastName,
                                               p.Squad,
                                               ScratchTotal = ((g.Game1.HasValue ? g.Game1 : 0) + (g.Game2.HasValue ? g.Game2 : 0) + (g.Game3.HasValue ? g.Game3 : 0) + (g.Game4.HasValue ? g.Game4 : 0)),
                                               GameTotal = (((g.Game1.HasValue ? g.Game1 : 0) + (m.Handicap + m.Bonus)) + ((g.Game2.HasValue ? g.Game2 : 0) + (m.Handicap + m.Bonus)) + ((g.Game3.HasValue ? g.Game3 : 0) + (m.Handicap + m.Bonus)) + ((g.Game4.HasValue ? g.Game4 : 0) + (m.Handicap + m.Bonus))),
                                               g.Game1,
                                               g.Game2,
                                               g.Game3,
                                               g.Game4,
                                               p.Member.Handicap,
                                               p.Member.Bonus
                                           }).ToList();

                dgvTournamentStats.DataSource = tournamentStatsList;
            }
            else
            {
                Tournament selectedTournament = new Tournament();
                selectedTournament = frmMemberScores.selectedTournament;
                lblTournamentName.Text = "Tournament ID: (" + selectedTournament.Id + ")\nTournament Location: " + selectedTournament.Location + "\nDate: " + selectedTournament.Date;

                NineTapDb db = new NineTapDb();
                List<int> listOfScores = new List<int>();


                var temp = (from g in db.Games
                            join p in db.Participants on g.Id equals p.Game.Id
                            join t in db.Tournaments on p.Tournament.Id equals t.Id
                            join m in db.Members on p.Member.Id equals m.Id
                            orderby (g.Game1 + g.Game2 + g.Game3 + g.Game4) descending
                            where t.Id == selectedTournament.Id
                            select new {
                                m.Id,                                
                                g.Game1,
                                g.Game2,
                                g.Game3,
                                g.Game4
                            }).ToList();

                HashSet<int> total = new HashSet<int>();
                //List<int> total = new List<int>();

                foreach (var s in temp)
                {
                    int one = Convert.ToInt32(s.Game1);
                    int two = Convert.ToInt32(s.Game2);
                    int three = Convert.ToInt32(s.Game3);
                    int four = Convert.ToInt32(s.Game4);
                    listOfScores.Add(one);
                    listOfScores.Add(two);
                    listOfScores.Add(three);
                    listOfScores.Add(four);
                    listOfScores.Sort();
                    listOfScores.Reverse();
                    
                    total.Add(listOfScores[0] + listOfScores[1] + listOfScores[2]);                                        

                    listOfScores.Clear();
                }

                    var tournamentStatsList = (from p in db.Participants
                                           join m in db.Members on p.Member.Id equals m.Id
                                           join g in db.Games on p.Game.Id equals g.Id
                                           join t in db.Tournaments on p.Tournament.Id equals t.Id
                                           where t.Id == selectedTournament.Id
                                           orderby (g.Game1 + g.Game2 + g.Game3 + g.Game4) descending
                                           select new
                                           {
                                               p.Member.Id,
                                               p.Member.FirstName,
                                               p.Member.LastName,
                                               p.Squad,
                                               ScratchTotal = ((g.Game1.HasValue ? g.Game1 : 0) + (g.Game2.HasValue ? g.Game2 : 0) + (g.Game3.HasValue ? g.Game3 : 0) + (g.Game4.HasValue ? g.Game4 : 0)),
                                               GameTotal = (((g.Game1.HasValue ? g.Game1 : 0) + (m.Handicap + m.Bonus)) + ((g.Game2.HasValue ? g.Game2 : 0) + (m.Handicap + m.Bonus)) + ((g.Game3.HasValue ? g.Game3 : 0) + (m.Handicap + m.Bonus)) + ((g.Game4.HasValue ? g.Game4 : 0) + (m.Handicap + m.Bonus))),
                                               g.Game1,
                                               g.Game2,
                                               g.Game3,
                                               g.Game4,
                                               p.Member.Handicap,
                                               p.Member.Bonus
                                           }).ToList();
                
                dgvTournamentStats.DataSource = tournamentStatsList;
            }            
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
}
