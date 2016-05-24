using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NineTapTour.Database;

namespace NineTapTour.Forms
{
    public partial class FrmTournamentStats : Form
    {
        public FrmTournamentStats()
        {
            InitializeComponent();
        }

        private void FrmTournamentStats_Load(object sender, EventArgs e)
        {
            lblTournamentName.Text = FrmTournaments.selectedTournament.Event;
            lblTournamentLocation.Text = FrmTournaments.selectedTournament.Location;
            lblTournamentDate.Text = FrmTournaments.selectedTournament.Date.ToString("MMMM dd, yyyy");
            NineTapDb db = new NineTapDb();
            int tourneyID = FrmTournaments.selectedTournament.Id;
            //var earnings = (from t in db.Tournaments
            //                join p in db.Participants on t.Id equals p.Id
            //                join m in db.Members on p.Id equals m.Id
            //                where t.Id == tourneyID
            //                orderby m.MoneyEarned descending
            //                select new { m.FirstName, m.LastName, m.MoneyEarned }).ToList();
            //foreach (var i in earnings)
            //{
            //    lbxTopEarnings.Items.Add(i.FirstName + " " + i.LastName + ": " + String.Format("{0:C2}",i.MoneyEarned));
            //}
        }
    }
}
