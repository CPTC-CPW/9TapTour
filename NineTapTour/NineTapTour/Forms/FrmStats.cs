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
using System.Data.Entity;

namespace NineTapTour.Forms
{
    public partial class FrmStats : Form
    {
        public FrmStats(int memberNumber, string memberName, Member currentMem)
        {
            InitializeComponent();
            memNum = memberNumber;
            memName = memberName;
            mem = currentMem;
            dataGridView1.DataSource = tableview();
            //var column = dataGridView1.Columns[4];
            //column.Width = 30;
            
            
        }
        private Member mem;
        private int memNum;
        private string memName;

        struct statHolder
        {
            public statHolder(DateTime Date,
                                string Location,
                                int Squad,
                                int Id,
                                string FirstName,
                                string LastName,
                                int? Game1,
                                int? Game2,
                                int? Game3,
                                int? Game4,
                                int? Handicap,
                                int? Bonus)
            {
                this.Date = Date;
                this.Location = Location;
                this.Squad = Squad;
                this.Id = Id;
                this.FirstName = FirstName;
                this.LastName = LastName;
                this.Game1 = Game1;
                this.Game2 = Game2;
                this.Game3 = Game3;
                this.Game4 = Game4;

                ScratchTotal = ((Game1.HasValue ? Game1 : 0) + (Game2.HasValue ? Game2 : 0) + (Game3.HasValue ? Game3 : 0) + (Game4.HasValue ? Game4 : 0));

                GameTotal = (((Game1.HasValue ? Game1 : 0) + (Handicap + Bonus)) + ((Game2.HasValue ? Game2 : 0) + (Handicap + Bonus)) + ((Game3.HasValue ? Game3 : 0) + (Handicap + Bonus)) + ((Game4.HasValue ? Game4 : 0) + (Handicap + Bonus)));

                AvgPerGame = ((Game1.HasValue ? Game1 : 0) + (Game2.HasValue ? Game2 : 0) + (Game3.HasValue ? Game3 : 0) + (Game4.HasValue ? Game4 : 0));
                    
                int div = ((Game1.HasValue ? 1 : 0) + (Game2.HasValue ? 1 : 0) + (Game3.HasValue ? 1 : 0) + (Game4.HasValue ? 1 : 0));
                if (div != 0)
                {
                    AvgPerGame /= div;
                }

                this.Handicap = Handicap;
                this.Bonus = Bonus;
            }

            public DateTime Date;
            public string Location;
            public int Squad;
            public int Id;
            public string FirstName;
            public string LastName;
            public int? Game1;
            public int? Game2;
            public int? Game3;
            public int? Game4;
            public int? ScratchTotal;
            public int? GameTotal;
            public int? AvgPerGame;
            public int? Handicap;
            public int? Bonus;
        }

        /// <summary>
        /// Populates the stats page for the member selected
        /// </summary>
        /// 
        public void populateStats()
        {

            var db = new NineTapDb();
            var temp = (from p in db.Participants
                         join m in db.Members on p.Member.Id equals m.Id
                         join g in db.Games on p.Game.Id equals g.Id
                         join t in db.Tournaments on p.Tournament.Id equals t.Id
                         where memNum == p.Member.Number
                         orderby t.Date descending
                         select new
                         {
                             t.Date,
                             t.Location,
                             p.Squad,
                             p.Member.Id,
                             p.Member.FirstName,
                             p.Member.LastName,
                             g.Game1,
                             g.Game2,
                             g.Game3,
                             g.Game4,
                             ScratchTotal = 0,
                             GameTotal = 0,
                             AvgPerGame = 0,                       
                             g.Handicap,
                             g.Bonus
                         }).ToList();

            List<statHolder> stats = new List<statHolder>();
            for (int i = 0; i < temp.Count; i++)
            {
                stats.Add(new statHolder(
                            temp[i].Date,
                             temp[i].Location,
                             temp[i].Squad,
                             temp[i].Id,
                             temp[i].FirstName,
                             temp[i].LastName,
                             temp[i].Game1,
                             temp[i].Game2,
                             temp[i].Game3,
                             temp[i].Game4,
                             temp[i].Handicap,
                             temp[i].Bonus
                        )
                    );
            }
            double sum = 0;
            double count = 0;
            #region Game 1 Average
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Game1);
            }
            txtGame1.Text = String.Format("{0:N2}", (sum / count));
            #endregion
            #region Game 2 Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Game2);
            }
            txtGame2.Text = String.Format("{0:N2}", (sum / count));
            #endregion
            #region Game 3 Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Game3);
            }
            txtGame3.Text = String.Format("{0:N2}", (sum / count));
            #endregion
            #region Game 4 Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Game4);
            }
            txtGame4.Text = String.Format("{0:N2}",(sum / count));
            #endregion
            #region Scratch Total Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].ScratchTotal);
            }
            txtScratchTotal.Text = String.Format("{0:N2}", (sum / count));
            #endregion
            #region Game Total Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].GameTotal);
            }
            txtGameTotal.Text = String.Format("{0:N2}", (sum / count));
            #endregion
            #region Average Game Score
            sum = 0;
            foreach(var item in stats)
            {
                sum += Convert.ToDouble(item.AvgPerGame);
            }

            txtAveragePerGame.Text = (sum / stats.Count()).ToString();            
            #endregion           

            #region Handicap Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Handicap);
            }
            txtHandicap.Text = String.Format("{0:N2}", (sum / count));
            #endregion
            #region Bonus Pins Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Bonus);
            }
            txtBonus.Text = String.Format("{0:N2}", (sum / count));
            #endregion
            
        }

        public DataTable tableview()
        {
           
            DataTable dtGames = new DataTable();
            var db = new NineTapDb();
            var temp = (from p in db.PlayerHistory
                        where memNum == p.MemberNumber
                        orderby p.TournamentDate descending, p.hisID descending
                        select new
                        {
                            p.GamesPlayed,
                            p.TournamentDate,
                            p.Game1,
                            p.Game2,
                            p.Game3,
                            p.Game4,
                            ScratchTotal = p.Game1 + p.Game2 + p.Game3 + p.Game4,
                            TotalScore = (p.Game1 + p.Bonus + p.HandiCap) + (p.Game2 + p.Bonus + p.HandiCap) + (p.Game3 + p.Bonus + p.HandiCap) + (p.Game4 + p.Bonus + p.HandiCap),
                            p.HandiCap,
                            p.Bonus,
                            p.ProPot,
                            p.MoneyWon,
                            p.PPHG,
                            p.Notes
                        });
            dtGames.Columns.Add("Games");
            dtGames.Columns.Add("Date");
            dtGames.Columns.Add("Game1");
            //dtGames.Columns.Add(new DataColumn("Selected", typeof(bool)));
            dtGames.Columns.Add("Game2");
            dtGames.Columns.Add("Game3");
            dtGames.Columns.Add("Game4");
            dtGames.Columns.Add("Scratch Total");
            dtGames.Columns.Add("Average \n Per \n Game");
            dtGames.Columns.Add("Game Total");
            dtGames.Columns.Add("Handicap");
            dtGames.Columns.Add("Bonus");
            dtGames.Columns.Add("Pro Pot");
            dtGames.Columns.Add("Place");
            dtGames.Columns.Add("Money Won");
            dtGames.Columns.Add("Notes");

            foreach (var item in temp)
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
                newRow["Scratch Total"] = item.ScratchTotal;
                newRow["Game Total"] = item.TotalScore;
                newRow["Average \n Per \n Game"] = Convert.ToDouble((item.Game1 + item.Game2 + item.Game3 + item.Game4) / item.GamesPlayed);
                newRow["Handicap"] = item.HandiCap;
                newRow["Bonus"] = item.Bonus;
                newRow["Pro Pot"] = item.ProPot;
                newRow["Money Won"] = item.MoneyWon;
                newRow["Place"] = item.PPHG;
                newRow["Notes"] = item.Notes;

                dtGames.Rows.Add(newRow);
            
            }
            
            return dtGames;
        }

        private void FrmStats_Load(object sender, EventArgs e)
        {
            string[] firstname = mem.FirstName.Split(' ');
            lblName.Text = firstname[0] + "    " + mem.LastName;
            lblMemberNumber.Text = Convert.ToString(memNum);
            lblStartAvg.Text = mem.StartAvg.ToString();
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
            Bitmap bm = new Bitmap(this.dataGridView1.Width, this.dataGridView1.Height);
            this.dataGridView1.DrawToBitmap(bm, new Rectangle(0, 0, this.dataGridView1.Width, this.dataGridView1.Height));
            e.Graphics.DrawImage(bm, 0, 0);
        }

 
    }
}
