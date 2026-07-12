using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using NineTapTour.Database;
using NineTapTour.Models;
using NineTapTour.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NineTapTour.Forms
{
    public partial class FrmPrintByDate : Form
    {
        private readonly ITournamentRepository _tournamentRepo;
        private readonly IDbContextFactory<NineTapDb> _dbFactory;

        public FrmPrintByDate()
        {
            InitializeComponent();
        }

        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public FrmPrintByDate(ITournamentRepository tournamentRepo, IDbContextFactory<NineTapDb> dbFactory)
        {
            InitializeComponent();
            _tournamentRepo = tournamentRepo;
            _dbFactory = dbFactory;
        }

        private void dateTimeStart_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimeStart.Value > dateTimeEnd.Value)
            {
                dateTimeStart.Value = dateTimeEnd.Value;
            }
        }

        private void dateTimeEnd_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimeEnd.Value > dateTimeStart.Value)
            {
                dateTimeEnd.Value = dateTimeStart.Value;
            }
        }

        List<Tournament> tours;
        List<Member> members;
        private void btnCheck_Click(object sender, EventArgs e)
        {
            using (var db = _dbFactory.CreateDbContext())
            {
                tours = [.. (from t in db.Tournaments
                         orderby t.Date descending
                         where t.Date >= dateTimeStart.Value && t.Date <= dateTimeEnd.Value
                         select t)];
            }
            members = _tournamentRepo.GetUniqueTourMembersByDate(dateTimeStart.Value, dateTimeEnd.Value);

            if (tours.Count > 0)
            {
                btnPrint.Enabled = true;
                listTours.DataSource = tours;
                listTours.DisplayMember = "TourneyNameDate";
            }
            else
            {
                btnPrint.Enabled = false;
                listTours.Items.Clear();
            }
            lblNumMems.Text = members.Count + " members will be printed.";
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //Print.printByTourDate(dateTimeStart.Value, dateTimeEnd.Value);
            Print.PrintByMemberList(members);
        }
    }
}
