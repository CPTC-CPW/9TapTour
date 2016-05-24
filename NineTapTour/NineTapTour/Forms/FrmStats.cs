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
        }
        private Member mem;
        private int memNum;
        private string memName;

        /// <summary>
        /// Populates the stats page for the member selected
        /// </summary>
        public void populateStats()
        {
            var db = new NineTapDb();
            var stats = (from p in db.Participants
                         join g in db.Games on p.Game.Id equals g.Id
                         join t in db.Tournaments on p.Tournament.Id equals t.Id
                         where memNum == p.Member.Number
                         select new
                         {
                             t.Date,
                             p.Member.Id,
                             p.Member.FirstName,
                             p.Member.LastName,
                             p.Squad,                         
                             g.Game1,
                             g.Game2,
                             g.Game3,
                             g.Game4
                             ,
                             Gametotal = ((g.Game1.HasValue ? g.Game1 : 0) + (g.Game2.HasValue ? g.Game2 : 0) + (g.Game3.HasValue ? g.Game3 : 0) + (g.Game4.HasValue ? g.Game4 : 0))
                             ,
                             AvgPerGame = (((g.Game1.HasValue ? g.Game1 : 0) + (g.Game2.HasValue ? g.Game2 : 0) + (g.Game3.HasValue ? g.Game3 : 0) + (g.Game4.HasValue ? g.Game4 : 0)) /
                                      ((g.Game1.HasValue ? 1 : 0) + (g.Game2.HasValue ? 1 : 0) + (g.Game3.HasValue ? 1 : 0) + (g.Game4.HasValue ? 1 : 0)))
                             ,
                             p.Member.Average
                             ,
                             g.Handicap
                             ,
                             g.Bonus
                         }).ToList();
            double sum = 0;
            double count = 0;
            #region Game 1 Average
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Game1);
            }
            txtGame1.Text = (sum / count).ToString();
            #endregion
            #region Game 2 Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Game2);
            }
            txtGame2.Text = (sum / count).ToString();
            #endregion
            #region Game 3 Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Game3);
            }
            txtGame3.Text = (sum / count).ToString();
            #endregion
            #region Game 4 Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Game4);
            }
            txtGame4.Text = (sum / count).ToString();
            #endregion
            #region Game Total Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Gametotal);
            }
            txtGameTotal.Text = (sum / count).ToString();
            #endregion
            #region Average Game Score
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].AvgPerGame);
            }
            txtAveragePerGame.Text = (sum / count).ToString();
            #endregion
            #region Average On Record
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Average);
            }
            txtAverageOnFile.Text = (sum / count).ToString();
            #endregion
            #region Handicap Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Handicap);
            }
            txtHandicap.Text = (sum / count).ToString();
            #endregion
            #region Bonus Pins Average
            sum = 0;
            count = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Bonus);
            }
            txtBonus.Text = (sum / count).ToString();
            #endregion
            dataGridView1.DataSource = stats;
        }

        private void FrmStats_Load(object sender, EventArgs e)
        {
            lblName.Text = memName;
            lblMemberNumber.Text = Convert.ToString(memNum);
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
