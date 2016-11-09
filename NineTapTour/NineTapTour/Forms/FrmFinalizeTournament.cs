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
            //sets sizes of check box columns "Valid Score1, ValidScore2, ValidScore3, Valid Score 4, and Keep True Avg?"
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

        }
        //creates the dataview that will populate the datagridview table on form
        public DataTable DataView(Tournament tourn)
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
    }
}
