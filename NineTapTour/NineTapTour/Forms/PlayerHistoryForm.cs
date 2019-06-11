using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using NineTapTour.Models;

namespace NineTapTour.Database
{
    public partial class PlayerHistoryForm : Form
    {
        private int id;
       
        public PlayerHistoryForm(int id)
        {
            InitializeComponent();
            this.id = id;
            
            Member currentMember = MemberDB.GetMember(id,0 );

            createDataGridView(id);

            lblFullName.Text = ($"Name : {currentMember.FirstName} {currentMember.LastName}");
            lblMemberNumber.Text = ($"Member Number: {currentMember.Number}");
            lblMemberSrartAvg.Text = ($"Start avg : {currentMember.StartAvg}");
        }

        private void PlayerHistoryForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the '_NineTapTour_NineTapDbDataSet.Tournaments' table. You can move, or remove it, as needed.
            this.tournamentsTableAdapter.Fill(this._NineTapTour_NineTapDbDataSet.Tournaments);
        }


        private void createDataGridView(int id)
        {
            List<PlayerHistory> PlayerHistory = PlayerHistoryDB.GetTop30FromPlayerHistory(id);
            dtvPlayerHistory.DataSource = DataView(id, PlayerHistory);

            dtvPlayerHistory.SuspendLayout();
            var column = dtvPlayerHistory.Columns[1];
            for(int i = 2; i <= 15;  i++)
            {
                column = dtvPlayerHistory.Columns[i];
                column.Width = 50;
            }
            dtvPlayerHistory.ResumeLayout();
            dtvPlayerHistory.AllowUserToAddRows = false;
        }


        private DataTable DataView(int id, List<PlayerHistory> PlayerHistory)
        {
            var db = new NineTapDb();
            DataTable dt = new DataTable();
            dt.Columns.Add("Games Played").ReadOnly = true;
            dt.Columns.Add("Date").ReadOnly = true;
            dt.Columns.Add("Game 1").ReadOnly = true;
            dt.Columns.Add("Game 2").ReadOnly = true;
            dt.Columns.Add("Game 3").ReadOnly = true;
            dt.Columns.Add("Game 4").ReadOnly = true;
            dt.Columns.Add("Total").ReadOnly = true;
            dt.Columns.Add("Average of Row").ReadOnly = true;
            dt.Columns.Add("True Average").ReadOnly = true;
            dt.Columns.Add("AVG").ReadOnly = true;
            dt.Columns.Add("HandiCap").ReadOnly = true;
            dt.Columns.Add("Bonus").ReadOnly = true;
            dt.Columns.Add("ProPot").ReadOnly = true;
            dt.Columns.Add("PPHG").ReadOnly = true;
            dt.Columns.Add("Cash").ReadOnly = true;
            dt.Columns.Add("Notes").ReadOnly = true;

            List<PlayerHistory> temp = PlayerHistory;

            foreach (var item in temp)
            {
                DataRow newRow = dt.NewRow();
                newRow["Games Played"] = item.GamesPlayed;
                newRow["Date"] = item.TournamentDate.ToShortDateString();
                newRow["Game 1"] = item.Game1;
                newRow["Game 2"] = item.Game2;
                newRow["Game 3"] = item.Game3;
                newRow["Game 4"] = item.Game4;
                newRow["Total"] = item.TotalScore;
                newRow["Average of Row"] = item.AverageForGame;
                newRow["True Average"] = item.trueAVG;
                newRow["AVG"] = item.AVG;
                newRow["HandiCap"] = item.HandiCap;
                newRow["Bonus"] = item.Bonus;
                newRow["ProPot"] = item.ProPot;
                newRow["PPHG"] = item.PPHG;
                newRow["Cash"] = item.MoneyWon;
                newRow["Notes"] = item.Notes;

                dt.Rows.Add(newRow);
            }
            return dt;
        }
    }
}
