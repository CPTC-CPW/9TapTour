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
            dataGridView1.DataSource = DataView(temptourn);
        }
        public DataTable DataView(Tournament tourn)
        {
            var db = new NineTapDb();
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            
            dt.Columns.Add("Game 1");
            dt.Columns.Add(new DataColumn("Valid Score?", typeof(bool)));
            dt.Columns.Add("Game 2");
            dt.Columns.Add(new DataColumn("Valid Score?", typeof(bool)));
            dt.Columns.Add("Game 3");
            dt.Columns.Add(new DataColumn("Valid Score?", typeof(bool)));
            dt.Columns.Add("Game 4");
            dt.Columns.Add(new DataColumn("Valid Score?", typeof(bool)));
            dt.Columns.Add("True Avg");
            dt.Columns.Add("Adjusted Avg");
            dt.Columns.Add(new DataColumn("Keep True Avg?", typeof(bool)));
            dt.Columns.Add("Scratch Total");
            dt.Columns.Add("Squad");
            dt.Columns.Add("Game Avg");
            dt.Columns.Add("Handicap");
            dt.Columns.Add("Bonus");
            dt.Columns.Add("Pro Pot");
            dt.Columns.Add("Notes");
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
                            
                            
                        });


            return dt;
        }
    }
}
