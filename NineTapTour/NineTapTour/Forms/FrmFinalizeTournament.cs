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
        public FrmFinalizeTournament()
        {
            InitializeComponent();
        }
        public DataTable DataView()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Last Name");
            dt.Columns.Add("First Name");
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
            dt.Columns.Add("Date");
            dt.Columns.Add("Date");
            dt.Columns.Add("Date");
            dt.Columns.Add("Date");
            dt.Columns.Add("Date");
            dt.Columns.Add("Date");
            dt.Columns.Add("Date");
            dt.Columns.Add("Date");


            return dt;
        }
    }
}
