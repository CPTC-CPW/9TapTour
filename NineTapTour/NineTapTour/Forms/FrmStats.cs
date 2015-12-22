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
        public FrmStats(int memberNumber, string memberName)
        {
            InitializeComponent();
            memNum = memberNumber;
            memName = memberName;
        }
        private int memNum;
        private string memName;
        private void button1_Click(object sender, EventArgs e)
        {

            // Used p.Member.Number instead of p.Member.Id because currently in the database, id started off as 12 due to 
            //changes in the database.
            var db = new NineTapDb();
            var games = (from p in db.Participants
                         join g in db.Games on p.Game.Id equals g.Id
                         join t in db.Tournaments on p.Tournament.Id equals t.Id
                         where memNum == p.Member.Number
                         select new { t.Date, g.Game1, g.Game2, g.Game3, g.Game4
                             , Gametotal = (g.Game1 + g.Game2 + g.Game3 + g.Game4)
                             , AvgOfRow = ((g.Game1 + g.Game2 + g.Game3 + g.Game4) / 4) 
                             , p.Member.Average
                             , g.Handicap
                             , g.Bonus
                             }).ToList();
            dataGridView1.DataSource = games;
        }

        private void FrmStats_Load(object sender, EventArgs e)
        {
            lblName.Text = memName;
            lblMemberNumber.Text = Convert.ToString(memNum);
        }
    }
}
