using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using NineTapTour.Models;
using NineTapTour.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace NineTapTour.Forms;

/// <summary>
/// This class handles the tournament stats table for frmMemberScores.
/// </summary>
public partial class FrmTournamentStats : Form
{

    private readonly ITournamentStatsRepository _statsRepo;

    public FrmTournamentStats()
    {
        InitializeComponent();
    }

    [ActivatorUtilitiesConstructor]
    public FrmTournamentStats(ITournamentStatsRepository statsRepo)
    {
        InitializeComponent();
        _statsRepo = statsRepo;
    }

    /// <summary>
    /// TournamentStats_Load() is the main method that populates the form initially.
    /// This queries the database based on whether the tournament is "three out of four"
    /// and then creates a TournamentStatsList object for each record. Then a 
    /// List<TournamentStatsList> is sent to a DataTable builder method and populates
    /// the DataGridView.
    /// comment author: Nelson_Nyland
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void TournamentStats_Load(object sender, EventArgs e)
    {
        if (!FrmMemberScoresHelpers.selectedTournament.ThreeOutOf4)
        {
            Tournament selectedTournament = FrmMemberScoresHelpers.selectedTournament;
            lblTournamentName.Text = "Tournament ID: (" + selectedTournament.Id + ")\nTournament Location: " + selectedTournament.Location + "\nDate: " + selectedTournament.Date;

            // Grabs a list of TournamentStatsList from the database
            List<TournamentStatsList> statsList = _statsRepo.GetTournamentStatsList(selectedTournament.Id);
            
            // Send to form
            dgvTournamentStats.DataSource = BuildDataTable(statsList);
        }
        else
        {
            Tournament selectedTournament = FrmMemberScoresHelpers.selectedTournament;
            lblTournamentName.Text = "Tournament ID: (" + selectedTournament.Id + ")\nTournament Location: " + selectedTournament.Location + "\nDate: " + selectedTournament.Date;

            List<TournamentStatsList> statsList = _statsRepo.Get3OutOf4TournamentStatsList(selectedTournament.Id);

            // send to form
            dgvTournamentStats.DataSource = BuildDataTable(statsList);
        }
    }

    /// <summary>
    /// This method sorts scores and removes the lowest if 4 scores are present
    /// It returns  a list with the 3 highest scores listOfValidScores
    /// </summary>
    /// <param name="scores"></param>  
    public static List<int> GetTop3OutOf4(List<int?> scores)
    {
        List<int> listOfValidScores = [];
        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i].HasValue)
                listOfValidScores.Add(scores[i].Value);
        }

        //after sorting I want to get rid of lowest score  
        listOfValidScores.Sort();
        if (listOfValidScores.Count == 4)
            listOfValidScores.Remove(listOfValidScores[0]);

        listOfValidScores.Reverse();
        return listOfValidScores;
    }

    /// <summary>
    /// GetConnection() returns a connection string to the database within the quotes.
    /// </summary>
    /// <returns>Database ConnectionString</returns>
    public static string GetConnection()
    {
        return ConfigurationManager.ConnectionStrings["NineTapDbConnection"].ConnectionString;
    }

    /// <summary>
    /// BtnPrint_Click() is called when Print button is clicked on the tournamentStats form.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnPrint_Click(object sender, EventArgs e)
    {
        printDialog1.Document = printDocument1;
        if (printDialog1.ShowDialog() == DialogResult.OK)
        {
            printDocument1.Print();
        }
    }

    /// <summary>
    /// PrintDocument1_PrintPage() is called after choosing where to save or print the tournamentStats table.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
    {
        Bitmap bm = new(this.dgvTournamentStats.Width, this.dgvTournamentStats.Height);
        this.dgvTournamentStats.DrawToBitmap(bm, new Rectangle(0, 0, 1582, 621));
        e.Graphics.DrawImage(bm, 0, 0);
    }

    /// <summary>
    /// BuildDataTable() Boxes up the tournamentStatsList object into a data table object 
    /// that the datagridview is willing to accept and sort.
    /// </summary>
    /// <param name="statsList"></param>
    /// <returns>Datatable object</returns>
    private static DataTable BuildDataTable(List<TournamentStatsList> statsList)
    {
        DataTable data = new("Tournament Stats");

        data.Columns.Add("ID", System.Type.GetType("System.Int32"));
        data.Columns.Add("First Name", System.Type.GetType("System.String"));
        data.Columns.Add("Last Name", System.Type.GetType("System.String"));
        data.Columns.Add("Squad", System.Type.GetType("System.Int32"));
        data.Columns.Add("Scratch Total", System.Type.GetType("System.Int32"));
        data.Columns.Add("Top3Scores", System.Type.GetType("System.Int32"));
        data.Columns.Add("Game 1", System.Type.GetType("System.Int32"));
        data.Columns.Add("Game 2", System.Type.GetType("System.Int32"));
        data.Columns.Add("Game 3", System.Type.GetType("System.Int32"));
        data.Columns.Add("Game 4", System.Type.GetType("System.Int32"));
        data.Columns.Add("Handicap", System.Type.GetType("System.Int32"));
        data.Columns.Add("Bonus", System.Type.GetType("System.Int32"));

        // Make first four columns required
        for (int i = 0; i < 4; i++)
        {
            data.Columns[i].AllowDBNull = false;
        }

        // Add statsList to DataTable
        foreach (var item in statsList)
        {
            data.Rows.Add(
            [
            item.Id,
            item.FirstName,
            item.LastName,
            item.Squad,
            item.ScratchTotal,
            item.Top3Scores,
            item.Game1,
            item.Game2,
            item.Game3,
            item.Game4,
            item.Handicap,
            item.Bonus
            ]);
        }

        // Return data table object
        return data;
    }
}
