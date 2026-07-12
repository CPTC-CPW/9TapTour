using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NineTapTour.Database;

namespace NineTapTour.Forms;

public partial class FrmTournamentsByYear : Form
{
    private readonly IDbContextFactory<NineTapDb> _dbFactory;

    public FrmTournamentsByYear()
    {
        InitializeComponent();
    }

    [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
    public FrmTournamentsByYear(IDbContextFactory<NineTapDb> dbFactory)
    {
        InitializeComponent();
        _dbFactory = dbFactory;
    }

    private void TournamentsByYear_Load(object sender, EventArgs e)
    {
        btnSearch.Enabled = false;
        cbxYear.Items.Clear();
        foreach (int y in GetYearsForTournamentDropdown())
        {
            cbxYear.Items.Add(y);
        }
    }

    /// <summary>
    /// Gets the last 25 years from the current year down plus 1 year. This is used to populate the combo box for searching tournaments by year.
    /// </summary>
    /// <returns>List of Years from current to 25 years ago</returns>
    private static int[] GetYearsForTournamentDropdown()
    {
        int currentYear = DateTime.Now.Year + 1;
        int[] years = new int[25];

        for (int i = 0; i < years.Length; i++)
            years[i] = currentYear - i;

        return years;
    }


    private void CbxYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnSearch.Enabled = true;
    }

    private void BtnSearch_Click(object sender, EventArgs e)
    {
        PopulateTournamentsByYear(Convert.ToInt32(cbxYear.Text));
    }

    /// <summary>
    /// Gets all the tournaments from a specific year
    /// </summary>
    /// <param name="selectedYear">Year selected</param>
    public void PopulateTournamentsByYear(int selectedYear)
    {
        using NineTapDb db = _dbFactory.CreateDbContext();
        // Phase 6: Use Tournament.TourneyRegion.NineTapRegionID for proper FK relationship
        var tournaments = (from t in db.Tournaments
                           orderby t.Date descending
                           where t.Date.Year == selectedYear
                           select new
                           {
                               t.Id,
                               t.Date,
                               t.Location,
                               t.Event,
                               t.Doubles,
                               t.ThreeOutOf4,
                               t.Notes,
                               t.Sponsors
                           }).ToList();
        dgvAllTournaments.DataSource = tournaments;
    }
}
