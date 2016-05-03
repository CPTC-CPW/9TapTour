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
                             g.Game1,
                             g.Game2,
                             g.Game3,
                             g.Game4
                             ,
                             Gametotal = ((g.Game1.HasValue ? g.Game1 : 0) + (g.Game2.HasValue ? g.Game2 : 0) + (g.Game3.HasValue ? g.Game3 : 0) + (g.Game4.HasValue ? g.Game4 : 0))
                             ,
                             AvgOfRow = (((g.Game1.HasValue ? g.Game1 : 0) + (g.Game2.HasValue ? g.Game2 : 0) + (g.Game3.HasValue ? g.Game3 : 0) + (g.Game4.HasValue ? g.Game4 : 0)) /
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
            for(int i = 0; i < stats.Count; i++)
            {
                count++;
                sum += Convert.ToInt32(stats[i].Game1);
            }
            txtGame1.Text = (sum / count).ToString();
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

        }
    }
}
