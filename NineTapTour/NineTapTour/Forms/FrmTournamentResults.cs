using NineTapTour.Database;
using CalcService = NineTapTour.Calculations.Calculations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NineTapTour.Models;
using ClosedXML.Excel;
using NineTapTour.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace NineTapTour.Forms;

public partial class FrmTournamentResults : Form
{
    // Names of the Columns in the DataGridView
    const string PLACE_STANDING_COLUMN_NAME = "Place";
    const string FULLNAME_COLUMN_NAME = "Full Name";
    const string HANDICAP_COLUMN_NAME = "H/B*";
    const string TOTAL_SCORE_COLUMN_NAME = "Total Score";
    const string EARNINGS_COLUMN_NAME = "Earnings";
    const string MEMBER_ID_COLUMN_NAME = "Member ID";
    const string GAME_ID_COLUMN_NAME = "Game ID";
    const string PROGRESSIVEPOT_COLUMN_NAME = "Progressive Pot";
    // 2-day mode only
    const string MEMBER_NUMBER_COLUMN_NAME = "Member Number";
    const string SQUAD_COLUMN_NAME = "Squad";

    readonly DataTable dt = new(); // Instantiate Data Table
    readonly NineTapDb db = new(); // Get access to database
    readonly Tournament tourny = FrmMemberScoresHelpers.selectedTournament; // Get Tournament
    static int totalTournamentEntries;  // Total number of entries for all squads in tournament
    static int clientInput; // how many winners the client wants to see
    List<ExcelMember> clientRequested = [];
    List<ExcelMember> winners = [];

    // Team View controls (doubles only)
    private DataGridView dgvTeamView;
    private Button btnTeamView;
    private bool _inTeamView = false;

    // 2-day championship controls
    private DataTable _dt2Day;
    private Panel _pnlRoundSetup;
    private NumericUpDown _nudStartPlace;
    private NumericUpDown _nudEndPlace;
    private TextBox _txtRoundEarnings;
    private ComboBox _cbxRoundSquad;
    private Button _btnAddRound;

    /* Floor directors get a comp entry into tournament when they help with tournament. 
     * They don't pay the entry fee, but do qualify to cash.
     */
    static int compEntries;

    #region Form Initilizers and Closers
    public FrmTournamentResults()
    {
        InitializeComponent();
    }
    private void FrmTournamentResults_Load(object sender, EventArgs e)
    {
        // Set compEntries to 0; increment when building winners list
        compEntries = 0;

        // Display tournament name, date and type (if applicable) on form
        lblTournamentName.Text = tourny.TourneyNameDate;
        if (tourny.Doubles)
        {
            lblTournamentName.Text += " (DOUBLES TOURNAMENT)";
        }
        if (tourny.ThreeOutOf4)
        {
            lblTournamentName.Text += " (3 OUT OF 4 TOURNAMENT)";
        }

        // For doubles tournaments the director enters the number of *teams* they want to place
        if (tourny.Doubles)
        {
            lblClientRequestCount.Text = "How many teams would you like places for?";
        }

        if (tourny.IsTwoDay)
            lblTournamentName.Text += " (2-DAY CHAMPIONSHIP)";

        InitTeamViewControls();

        if (tourny.IsTwoDay)
        {
            InitTwoDayControls();
        }
        else
        {
            // Create a List<ExcelMember> and populate it with this tournament's participants
            winners = BuildWinnersList();
            ActiveControl = tbClientInputCount;
        }
    }
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);

        if (e.CloseReason == CloseReason.WindowsShutDown) return;

        if (tourny.IsTwoDay)
        {
            if (_dt2Day == null || _dt2Day.Rows.Count == 0) return;

            List<double> twoDayWinnings = [];
            for (int i = 0; i < _dt2Day.Rows.Count; i++)
                twoDayWinnings.Add(Convert.ToDouble(_dt2Day.Rows[i][EARNINGS_COLUMN_NAME]));
            TempVariablesForGlobalLevel.MoneyEarnings = twoDayWinnings;

            for (int i = 0; i < _dt2Day.Rows.Count; i++)
            {
                var gameIdObj = _dt2Day.Rows[i][GAME_ID_COLUMN_NAME];
                if (gameIdObj == DBNull.Value || !int.TryParse(gameIdObj.ToString(), out int gameId) || gameId <= 0) continue;

                Game g = db.Games.Find(gameId);
                if (g == null) continue;

                g.PlaceStanding = ParsePlaceStanding(_dt2Day.Rows[i][PLACE_STANDING_COLUMN_NAME]);
                g.MoneyWon = Convert.ToDecimal(_dt2Day.Rows[i][EARNINGS_COLUMN_NAME]);

                if (decimal.TryParse(Convert.ToString(_dt2Day.Rows[i][PROGRESSIVEPOT_COLUMN_NAME]), out decimal _))
                    g.SidePot = Convert.ToDecimal(_dt2Day.Rows[i][PROGRESSIVEPOT_COLUMN_NAME]);
                else
                    g.Notes = $"Progressive Pot was entered as: {Convert.ToString(_dt2Day.Rows[i][PROGRESSIVEPOT_COLUMN_NAME])}";

                db.SaveChanges();
            }
            return;
        }

        if (dgvTournamentResults.DataSource == null || dt.Rows.Count == 0) return;

        List<double> Winnings = [];
        for (int winningList = 0; winningList < dt.Rows.Count; winningList++)
        {
            Winnings.Add(Convert.ToDouble(dgvTournamentResults[EARNINGS_COLUMN_NAME, winningList].Value));
        }
        TempVariablesForGlobalLevel.MoneyEarnings = Winnings;

        // Save all changes made to the dataGridView
        for (int currentIndex = 0; currentIndex < clientRequested.Count; currentIndex++)
        {
            int gameId = Convert.ToInt32(dgvTournamentResults[GAME_ID_COLUMN_NAME, currentIndex].Value.ToString());
            // Use the form-level context's identity map (Find) so EF Core never sees two
            // instances of the same Game key — fixes the "already being tracked" exception.
            Game g = db.Games.Find(gameId);
            if (g == null) continue;

            g.PlaceStanding = ParsePlaceStanding(dgvTournamentResults[PLACE_STANDING_COLUMN_NAME, currentIndex].Value);
            g.MoneyWon = Convert.ToDecimal(dgvTournamentResults[EARNINGS_COLUMN_NAME, currentIndex].Value);

            // if user enters something other than a decimal number
            if (Decimal.TryParse(Convert.ToString(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value), out decimal _))
            {
                g.SidePot = Convert.ToDecimal(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value);
            }
            else
            {
                g.Notes = $"Progressive Pot was entered as: {Convert.ToString(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value)}";
            }

            // Phase 4: Removed g.gameRegionID assignment - stored in Participant entity
            // db.Entry is not needed — Find already tracks the entity in the form-level context.
            db.SaveChanges();
        }
    }
    #endregion

    /// <summary>
    /// Creates the DataGridView table and populates it with the list of cashed winners
    /// </summary>
    private void CreateDataGridView(List<ExcelMember> clientRequested, int clientInput)
    {
        // Create data table and add columns, columns with ReadOnly set to False are editable      
        dt.Columns.Add(PLACE_STANDING_COLUMN_NAME).ReadOnly = true;
        dt.Columns.Add(FULLNAME_COLUMN_NAME).ReadOnly = true;
        dt.Columns.Add(HANDICAP_COLUMN_NAME).ReadOnly = true;
        dt.Columns.Add(TOTAL_SCORE_COLUMN_NAME).ReadOnly = true;
        dt.Columns.Add(EARNINGS_COLUMN_NAME).ReadOnly = false;
        dt.Columns.Add(MEMBER_ID_COLUMN_NAME).ReadOnly = true;
        dt.Columns.Add(GAME_ID_COLUMN_NAME).ReadOnly = true;
        dt.Columns.Add(PROGRESSIVEPOT_COLUMN_NAME).ReadOnly = false;
      
        double earnings = 0.00;

        int MonEarnCount = 0;
        if (TempVariablesForGlobalLevel.MoneyEarnings != null)
        {
            MonEarnCount = TempVariablesForGlobalLevel.MoneyEarnings.Count;
        }

        // Create rows and populate with each member's data for each row
        for (int wc = 0; wc < clientRequested.Count; wc++)
        {
            DataRow newRow = dt.NewRow();
            if (MonEarnCount > 0)
            {
                if (wc < MonEarnCount)
                {
                    newRow[EARNINGS_COLUMN_NAME] = TempVariablesForGlobalLevel.MoneyEarnings[wc];
                }
                else
                {
                    newRow[EARNINGS_COLUMN_NAME] = earnings;
                }
            }
            else
            {
                newRow[EARNINGS_COLUMN_NAME] = Convert.ToInt32(clientRequested[wc].MoneyWon);
            }
            
            newRow[PLACE_STANDING_COLUMN_NAME] = clientRequested[wc].PlaceStanding;
            newRow[FULLNAME_COLUMN_NAME] = clientRequested[wc].Name;
            newRow[HANDICAP_COLUMN_NAME] = (clientRequested[wc].Handicap) + " + " + clientRequested[wc].Bonus;
            newRow[TOTAL_SCORE_COLUMN_NAME] = clientRequested[wc].TotalScore;
            newRow[MEMBER_ID_COLUMN_NAME] = clientRequested[wc].MemberNumber;
            newRow[GAME_ID_COLUMN_NAME] = clientRequested[wc].GameId;

            if (clientRequested[wc].SidePot == null)
            {
                newRow[PROGRESSIVEPOT_COLUMN_NAME] = "0.00";
            }
            else
            {
                newRow[PROGRESSIVEPOT_COLUMN_NAME] = clientRequested[wc].SidePot;
            }
            dt.Rows.Add(newRow);
        }

        for (int tr = clientRequested.Count; tr < clientInput ; tr++)
        {
            DataRow newRow = dt.NewRow();
            if (MonEarnCount > 0 && tr < MonEarnCount)
            {
                newRow[EARNINGS_COLUMN_NAME] = TempVariablesForGlobalLevel.MoneyEarnings[tr];
            }
            else
            {
                newRow[EARNINGS_COLUMN_NAME] = earnings;
            }
            // For doubles, consecutive filler rows share a team place (2 rows per team slot)
            int fillerPlace;
            if (tourny.Doubles)
            {
                int filledTeams   = clientRequested.Count / 2;
                int fillerOffset  = tr - clientRequested.Count;
                fillerPlace = filledTeams + fillerOffset / 2 + 1;
            }
            else
            {
                fillerPlace = tr + 1;
            }
            newRow[PLACE_STANDING_COLUMN_NAME] = fillerPlace;
            newRow[FULLNAME_COLUMN_NAME] = "";
            newRow[HANDICAP_COLUMN_NAME] = "";
            newRow[TOTAL_SCORE_COLUMN_NAME] = "";
            newRow[MEMBER_ID_COLUMN_NAME] = "";
            newRow[GAME_ID_COLUMN_NAME] = tr;
            newRow[PROGRESSIVEPOT_COLUMN_NAME] = "0.00";
            dt.Rows.Add(newRow);
        }

        // Append "T" to the place standing of any tied rows (same numeric place as a neighbor)
        // Temporarily set the place standing column to editable so we can modify the values, then set it back to read-only when done
        dt.Columns[PLACE_STANDING_COLUMN_NAME].ReadOnly = false;
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (!int.TryParse(dt.Rows[i][PLACE_STANDING_COLUMN_NAME]?.ToString(), out int place) || place == 0)
                continue;
            bool isTie = (i > 0
                            && int.TryParse(dt.Rows[i - 1][PLACE_STANDING_COLUMN_NAME]?.ToString(), out int prev)
                            && prev == place)
                       || (i < dt.Rows.Count - 1
                            && int.TryParse(dt.Rows[i + 1][PLACE_STANDING_COLUMN_NAME]?.ToString(), out int next)
                            && next == place);
            if (isTie)
                dt.Rows[i][PLACE_STANDING_COLUMN_NAME] = $"{place}T";
        }
        dt.Columns[PLACE_STANDING_COLUMN_NAME].ReadOnly = true;

        // If there is data in datatable rows, then set datatable as source for datagridview
        // Hide the GameId and MemberId columns and don't allow user to add rows
        // Size datagridview columns to fit contents with name column filling rest of datagridview
        // Set the current cell to the cell in column 4, row 0 (MoneyWon column)
        if (dt.Rows.Count > 0)
        {
            dgvTournamentResults.DataSource = dt;

            dgvTournamentResults.Columns[MEMBER_ID_COLUMN_NAME].Visible = false;
            dgvTournamentResults.Columns[GAME_ID_COLUMN_NAME].Visible = false;
            dgvTournamentResults.AllowUserToAddRows = false;

            dgvTournamentResults.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvTournamentResults.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvTournamentResults.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvTournamentResults.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvTournamentResults.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvTournamentResults.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            
            dgvTournamentResults.CurrentCell = dgvTournamentResults[4, 0];
        }
    }

    /// <summary>
    /// Tabs the user though the cells of dgvTournamentResults
    /// </summary>
    private void DgvTournamentResults_CellEnter(object sender, DataGridViewCellEventArgs e)
    {
        if (dgvTournamentResults.CurrentRow.Cells[e.ColumnIndex].ReadOnly)
        {
            SendKeys.Send("{tab}");
        }
    }

    /// <summary>
    /// Returns a list of tourament winners
    /// </summary>
    /// <returns> List<ExcelMember> </returns>
    private List<ExcelMember> BuildWinnersList()
    {
        List<ExcelMember> tournyBowlers = [];
        List<WinnerListMemberViewModel> bowlers = TournamentDB.GetWinnerListMemberData(tourny.Id);

        totalTournamentEntries = bowlers.Count;

        if (tourny.Doubles)
            return BuildWinnersListDoubles(bowlers);

        // Batch-query each member's previous tournament handicap and bonus.
        // Uses the same min/max cash logic as FrmFinalizeTournament and FrmMemberScores.
        var memberNumbers = bowlers.Select(b => b.MemberNumber).Distinct().ToHashSet();
        var prevHBByMember = new Dictionary<int, (int Hdcp, int Bonus)>();
        using (var dbPrev = new NineTapDb())
        {
            var latestApproved = dbPrev.Participants
                .Where(p => memberNumbers.Contains(p.Member.Number)
                         && p.Tournament.Id != tourny.Id
                         && p.Game.IsFinalized
                         && p.Game.AdjustedAvg > 0)
                .GroupBy(p => p.Member.Number)
                .Select(g => new { MemberNumber = g.Key, LatestDate = g.Max(p => p.Tournament.Date) })
                .ToList();

            foreach (var item in latestApproved)
            {
                var prevEntries = dbPrev.Participants
                    .Where(p => p.Member.Number == item.MemberNumber
                             && p.Tournament.Id != tourny.Id
                             && p.Game.IsFinalized
                             && p.Tournament.Date == item.LatestDate)
                    .Select(p => new { p.Game.AdjustedAvg, Bonus = p.Game.Bonus ?? 0, MoneyWon = p.Game.MoneyWon ?? 0 })
                    .ToList();

                if (prevEntries.Count == 0) continue;

                var withAvg  = prevEntries.FirstOrDefault(e => e.AdjustedAvg > 0);
                int prevHdcp = withAvg != null ? CalcService.CalculateHandicapPins(withAvg.AdjustedAvg) : 0;
                int prevBonus = prevEntries.Any(e => e.MoneyWon > 0)
                    ? prevEntries.Min(e => e.Bonus)
                    : prevEntries.Max(e => e.Bonus);

                prevHBByMember[item.MemberNumber] = (prevHdcp, prevBonus);
            }
        }

        foreach (var b in bowlers)
        {
            if (b.IsComp)
            {
                compEntries++;
            }

            bool hasPrevHB = prevHBByMember.TryGetValue(b.MemberNumber, out var prevHB);
            ExcelMember m = new()
            {
                MemberNumber = b.MemberNumber,
                Name = b.BowlerName,
                Handicap = hasPrevHB && prevHB.Hdcp > 0 ? prevHB.Hdcp : Convert.ToInt32(b.Handicap),
                Bonus    = hasPrevHB ? prevHB.Bonus : Convert.ToInt32(b.Bonus),
                MoneyWon = b.MoneyWon,
                SidePot = b.SidePot,
                GameId = b.GameId,
                // If the game scores are null then a 0 will be placed in the the game
                Game1Score = Convert.ToInt32(b.Game1),
                Game2Score = Convert.ToInt32(b.Game2),
                Game3Score = Convert.ToInt32(b.Game3),
                Game4Score = Convert.ToInt32(b.Game4)
            };

            if (tourny.ThreeOutOf4)
            {
                List<int> scores = [m.Game1Score, m.Game2Score, m.Game3Score, m.Game4Score];

                // Remove the 0s from the scores list
                scores.RemoveAll(x => x == 0);

                // remove lowest score if there are 4 games
                if (scores.Count == 4)
                {
                    int minScore = scores.Min();
                    scores.Remove(minScore);
                    if (m.Game1Score == minScore)
                        m.Game1Score = 0;
                    else if (m.Game2Score == minScore)
                        m.Game2Score = 0;
                    else if (m.Game3Score == minScore)
                        m.Game3Score = 0;
                    else if (m.Game4Score == minScore)
                        m.Game4Score = 0;
                }

                m.TotalScore = scores.Sum()
                    + (scores.Count * (m.Handicap + m.Bonus));
            }
            else
            {
                int totalValidGames = 0;
                if (m.Game1Score > 0)
                    totalValidGames++;
                if (m.Game2Score > 0)
                    totalValidGames++;
                if (m.Game3Score > 0)
                    totalValidGames++;
                if (m.Game4Score > 0)
                    totalValidGames++;
                
                m.TotalScore = m.Game1Score + m.Game2Score + m.Game3Score
                    + m.Game4Score + (totalValidGames * (m.Handicap + m.Bonus));
            }
            tournyBowlers.Add(m);
        }
        return tournyBowlers;
    }

    /// <summary>
    /// Builds the winners list for a doubles tournament.
    /// Each DoublesTeam produces two ExcelMember entries with the same PlaceStanding and
    /// TotalScore (combined team HDCP total) so they appear as ties in the results grid.
    /// MoneyWon is already stored at the individual 50% share by FrmFinalizeTournament.
    /// </summary>
    private List<ExcelMember> BuildWinnersListDoubles(List<WinnerListMemberViewModel> bowlers)
    {
        // Build prevHBByMember (same logic as the singles path)
        var memberNumbers = bowlers.Select(b => b.MemberNumber).Distinct().ToHashSet();
        var prevHBByMember = new Dictionary<int, (int Hdcp, int Bonus)>();
        using (var dbPrev = new NineTapDb())
        {
            var latestApproved = dbPrev.Participants
                .Where(p => memberNumbers.Contains(p.Member.Number)
                         && p.Tournament.Id != tourny.Id
                         && p.Game.IsFinalized
                         && p.Game.AdjustedAvg > 0)
                .GroupBy(p => p.Member.Number)
                .Select(g => new { MemberNumber = g.Key, LatestDate = g.Max(p => p.Tournament.Date) })
                .ToList();

            foreach (var item in latestApproved)
            {
                var prevEntries = dbPrev.Participants
                    .Where(p => p.Member.Number == item.MemberNumber
                             && p.Tournament.Id != tourny.Id
                             && p.Game.IsFinalized
                             && p.Tournament.Date == item.LatestDate)
                    .Select(p => new { p.Game.AdjustedAvg, Bonus = p.Game.Bonus ?? 0, MoneyWon = p.Game.MoneyWon ?? 0 })
                    .ToList();

                if (prevEntries.Count == 0) continue;

                var withAvg  = prevEntries.FirstOrDefault(e => e.AdjustedAvg > 0);
                int prevHdcp = withAvg != null ? CalcService.CalculateHandicapPins(withAvg.AdjustedAvg) : 0;
                int prevBonus = prevEntries.Any(e => e.MoneyWon > 0)
                    ? prevEntries.Min(e => e.Bonus)
                    : prevEntries.Max(e => e.Bonus);

                prevHBByMember[item.MemberNumber] = (prevHdcp, prevBonus);
            }
        }

        // Load doubles teams and match to bowler entries
        List<DoublesTeam> teams = DoublesTeamDB.GetTeamsByTournament(tourny.Id);
        var bowlersByMemberId   = bowlers.GroupBy(b => b.MemberId).ToDictionary(g => g.Key, g => g.ToList());

        var teamRows = new List<(int CombinedHdcpTotal,
                                 WinnerListMemberViewModel M1, WinnerListMemberViewModel M2,
                                 int H1, int B1, int H2, int B2)>();

        foreach (var team in teams)
        {
            if (!bowlersByMemberId.TryGetValue(team.Member1.Id, out var e1)) continue;
            if (!bowlersByMemberId.TryGetValue(team.Member2.Id, out var e2)) continue;

            var m1 = e1.FirstOrDefault(e => e.Squad == team.Squad);
            var m2 = e2.FirstOrDefault(e => e.Squad == team.Squad);
            if (m1 == null || m2 == null) continue;

            bool has1 = prevHBByMember.TryGetValue(m1.MemberNumber, out var hb1);
            bool has2 = prevHBByMember.TryGetValue(m2.MemberNumber, out var hb2);

            int hdcp1  = has1 && hb1.Hdcp > 0 ? hb1.Hdcp : Convert.ToInt32(m1.Handicap);
            int hdcp2  = has2 && hb2.Hdcp > 0 ? hb2.Hdcp : Convert.ToInt32(m2.Handicap);
            int bonus1 = has1 ? hb1.Bonus : Convert.ToInt32(m1.Bonus);
            int bonus2 = has2 ? hb2.Bonus : Convert.ToInt32(m2.Bonus);

            int combinedHdcpTotal = (m1.Game1 ?? 0) + (m1.Game2 ?? 0)
                                  + (m2.Game1 ?? 0) + (m2.Game2 ?? 0)
                                  + 2 * (hdcp1 + bonus1)
                                  + 2 * (hdcp2 + bonus2);

            teamRows.Add((combinedHdcpTotal, m1, m2, hdcp1, bonus1, hdcp2, bonus2));
        }

        // Sort descending, assign places with tie detection
        teamRows.Sort((a, b) => b.CombinedHdcpTotal.CompareTo(a.CombinedHdcpTotal));
        var teamPlaces = new int[teamRows.Count];
        if (teamRows.Count > 0)
        {
            teamPlaces[0] = 1;
            for (int i = 1; i < teamRows.Count; i++)
                teamPlaces[i] = teamRows[i].CombinedHdcpTotal == teamRows[i - 1].CombinedHdcpTotal
                    ? teamPlaces[i - 1]
                    : i + 1;
        }

        var result = new List<ExcelMember>();
        for (int t = 0; t < teamRows.Count; t++)
        {
            var (combinedHdcpTotal, m1, m2, h1, b1, h2, b2) = teamRows[t];
            int place = teamPlaces[t];

            if (m1.IsComp) compEntries++;
            if (m2.IsComp) compEntries++;

            result.Add(new ExcelMember
            {
                MemberNumber  = m1.MemberNumber,
                Name          = m1.BowlerName,
                Handicap      = h1,
                Bonus         = b1,
                MoneyWon      = m1.MoneyWon,    // already stored as 50% share by FrmFinalizeTournament
                SidePot       = m1.SidePot,
                GameId        = m1.GameId,
                Game1Score    = m1.Game1 ?? 0,
                Game2Score    = m1.Game2 ?? 0,
                Game3Score    = 0,
                Game4Score    = 0,
                TotalScore    = combinedHdcpTotal,
                PlaceStanding = place
            });

            result.Add(new ExcelMember
            {
                MemberNumber  = m2.MemberNumber,
                Name          = m2.BowlerName,
                Handicap      = h2,
                Bonus         = b2,
                MoneyWon      = m2.MoneyWon,
                SidePot       = m2.SidePot,
                GameId        = m2.GameId,
                Game1Score    = m2.Game1 ?? 0,
                Game2Score    = m2.Game2 ?? 0,
                Game3Score    = 0,
                Game4Score    = 0,
                TotalScore    = combinedHdcpTotal,
                PlaceStanding = place
            });
        }

        return result;
    }

    /// <summary>
    /// Creates the Team View button and grid overlay for doubles tournaments.
    /// </summary>
    private void InitTeamViewControls()
    {
        if (!tourny.Doubles) return;

        btnTeamView = new Button
        {
            Text    = "Team View",
            Size    = new Size(105, 23),
            Location = new Point(tbClientInputCount.Right + 8, btnPaste.Top)
        };
        btnTeamView.Click += BtnTeamView_Click;
        Controls.Add(btnTeamView);

        dgvTeamView = new DataGridView
        {
            Location               = dgvTournamentResults.Location,
            Size                   = dgvTournamentResults.Size,
            Visible                = false,
            AllowUserToAddRows     = false,
            AllowUserToDeleteRows  = false,
            ReadOnly               = true,
            AutoSizeColumnsMode    = DataGridViewAutoSizeColumnsMode.None,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
            SelectionMode          = DataGridViewSelectionMode.FullRowSelect,
            Anchor                 = dgvTournamentResults.Anchor
        };
        dgvTeamView.DoubleBuffered(true);
        Controls.Add(dgvTeamView);
    }

    private void BtnTeamView_Click(object sender, EventArgs e)
    {
        if (dgvTeamView == null) return;
        _inTeamView = !_inTeamView;

        if (_inTeamView)
        {
            if (clientRequested.Count > 0)
                PopulateTeamView();
            dgvTeamView.Visible         = true;
            dgvTournamentResults.Visible = false;
            btnTeamView.Text             = "Individual View";
        }
        else
        {
            dgvTeamView.Visible          = false;
            dgvTournamentResults.Visible = true;
            btnTeamView.Text             = "Team View";
        }
    }

    /// <summary>
    /// Rebuilds <see cref="dgvTeamView"/> from <see cref="clientRequested"/>,
    /// collapsing each DoublesTeam into one row showing both partners' data.
    /// </summary>
    private void PopulateTeamView()
    {
        dgvTeamView.Columns.Clear();
        dgvTeamView.Rows.Clear();

        dgvTeamView.Columns.AddRange(
            new DataGridViewTextBoxColumn { Name = "tvPlace",   HeaderText = "Place",          Width = 55 },
            new DataGridViewTextBoxColumn { Name = "tvMember1", HeaderText = "Member 1",       AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill },
            new DataGridViewTextBoxColumn { Name = "tvHB1",     HeaderText = "M1 H/B",         Width = 75 },
            new DataGridViewTextBoxColumn { Name = "tvMember2", HeaderText = "Member 2",       AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill },
            new DataGridViewTextBoxColumn { Name = "tvHB2",     HeaderText = "M2 H/B",         Width = 75 },
            new DataGridViewTextBoxColumn { Name = "tvTotal",   HeaderText = "Combined Total", Width = 90 },
            new DataGridViewTextBoxColumn { Name = "tvEarn1",   HeaderText = "M1 Earnings",    Width = 85 },
            new DataGridViewTextBoxColumn { Name = "tvEarn2",   HeaderText = "M2 Earnings",    Width = 85 }
        );

        // BuildWinnersListDoubles always writes `winners` in consecutive pairs: [T1M1, T1M2, T2M1, T2M2, ...]
        // Step through by 2 to reconstruct team pairings — avoids re-querying DoublesTeamDB and
        // any member-number matching fragility (e.g. the same member in multiple squads).
        var teamPairs = new List<(ExcelMember M1, ExcelMember M2, int Place)>();
        for (int i = 0; i + 1 < winners.Count; i += 2)
        {
            var m1 = winners[i];
            var m2 = winners[i + 1];
            if (m1.PlaceStanding > clientInput) continue;
            teamPairs.Add((m1, m2, m1.PlaceStanding));
        }
        teamPairs.Sort((a, b) => a.Place.CompareTo(b.Place));

        Color[] teamColors = [SystemColors.Window, Color.AliceBlue];

        for (int i = 0; i < teamPairs.Count; i++)
        {
            var (m1, m2, place) = teamPairs[i];
            bool isTie = (i > 0 && teamPairs[i - 1].Place == place)
                      || (i < teamPairs.Count - 1 && teamPairs[i + 1].Place == place);
            string placeStr = isTie ? $"{place}T" : $"{place}";

            string earn1 = m1.MoneyWon.HasValue ? m1.MoneyWon.Value.ToString("C2") : "$0.00";
            string earn2 = m2.MoneyWon.HasValue ? m2.MoneyWon.Value.ToString("C2") : "$0.00";

            int rowIdx = dgvTeamView.Rows.Add(
                placeStr,
                m1.Name, $"{m1.Handicap} + {m1.Bonus}",
                m2.Name, $"{m2.Handicap} + {m2.Bonus}",
                m1.TotalScore,   // combined total is the same for both members
                earn1, earn2);

            dgvTeamView.Rows[rowIdx].DefaultCellStyle.BackColor = teamColors[i % 2];
        }
    }

    private void BtnExportToExcel_Click(object sender, EventArgs e)
    {
        ExportToExcel();
    }

    #region 2-Day Championship Entry

    /// <summary>
    /// Sets up the 2-day championship round-entry UI, hiding the normal "how many winners" controls
    /// and replacing them with a round-setup panel. The existing dgvTournamentResults is reused,
    /// bound to a new _dt2Day DataTable with an editable Member Number column.
    /// </summary>
    private void InitTwoDayControls()
    {
        lblClientRequestCount.Visible = false;
        tbClientInputCount.Visible    = false;
        btnPaste.Visible              = false;

        // Build the round-setup panel
        _pnlRoundSetup = new Panel
        {
            Location = new Point(34, 120),
            Size     = new Size(dgvTournamentResults.Width, 35),
            Anchor   = AnchorStyles.Top | AnchorStyles.Left
        };

        int x = 0;
        _pnlRoundSetup.Controls.Add(new Label { Text = "Start Place:", AutoSize = true, Location = new Point(x, 8) });
        x += 80;
        _nudStartPlace = new NumericUpDown { Location = new Point(x, 4), Width = 55, Minimum = 1, Maximum = 999, Value = 1 };
        _pnlRoundSetup.Controls.Add(_nudStartPlace);
        x += 63;
        _pnlRoundSetup.Controls.Add(new Label { Text = "End Place:", AutoSize = true, Location = new Point(x, 8) });
        x += 73;
        _nudEndPlace = new NumericUpDown { Location = new Point(x, 4), Width = 55, Minimum = 1, Maximum = 999, Value = 1 };
        _pnlRoundSetup.Controls.Add(_nudEndPlace);
        x += 63;
        _pnlRoundSetup.Controls.Add(new Label { Text = "Earnings $:", AutoSize = true, Location = new Point(x, 8) });
        x += 72;
        _txtRoundEarnings = new TextBox { Location = new Point(x, 4), Width = 80, Text = "0.00" };
        _pnlRoundSetup.Controls.Add(_txtRoundEarnings);
        x += 88;
        _pnlRoundSetup.Controls.Add(new Label { Text = "Squad:", AutoSize = true, Location = new Point(x, 8) });
        x += 50;
        _cbxRoundSquad = new ComboBox { Location = new Point(x, 4), Width = 60, DropDownStyle = ComboBoxStyle.DropDownList };
        for (int s = 1; s <= tourny.Squads; s++)
            _cbxRoundSquad.Items.Add(s);
        if (_cbxRoundSquad.Items.Count > 0)
            _cbxRoundSquad.SelectedIndex = 0;
        _pnlRoundSetup.Controls.Add(_cbxRoundSquad);
        x += 68;
        _btnAddRound = new Button { Text = "Add Round", Location = new Point(x, 2), Size = new Size(90, 28) };
        _btnAddRound.Click += BtnAddRound_Click;
        _pnlRoundSetup.Controls.Add(_btnAddRound);

        Controls.Add(_pnlRoundSetup);

        // Set up the 2-day DataTable (no ReadOnly on DataTable columns so auto-fill can write freely)
        _dt2Day = new DataTable();
        _dt2Day.Columns.Add(PLACE_STANDING_COLUMN_NAME);
        _dt2Day.Columns.Add(MEMBER_NUMBER_COLUMN_NAME);
        _dt2Day.Columns.Add(FULLNAME_COLUMN_NAME);
        _dt2Day.Columns.Add(HANDICAP_COLUMN_NAME);
        _dt2Day.Columns.Add(TOTAL_SCORE_COLUMN_NAME);
        _dt2Day.Columns.Add(EARNINGS_COLUMN_NAME);
        _dt2Day.Columns.Add(PROGRESSIVEPOT_COLUMN_NAME);
        _dt2Day.Columns.Add(MEMBER_ID_COLUMN_NAME);
        _dt2Day.Columns.Add(GAME_ID_COLUMN_NAME);
        _dt2Day.Columns.Add(SQUAD_COLUMN_NAME);

        dgvTournamentResults.DataSource        = _dt2Day;
        dgvTournamentResults.AllowUserToAddRows = false;

        // Hide internal/lookup columns
        dgvTournamentResults.Columns[MEMBER_ID_COLUMN_NAME].Visible = false;
        dgvTournamentResults.Columns[GAME_ID_COLUMN_NAME].Visible   = false;
        dgvTournamentResults.Columns[SQUAD_COLUMN_NAME].Visible     = false;

        // ReadOnly at DGV level (DataTable columns remain writable for auto-fill)
        dgvTournamentResults.Columns[PLACE_STANDING_COLUMN_NAME].ReadOnly = true;
        dgvTournamentResults.Columns[FULLNAME_COLUMN_NAME].ReadOnly       = true;
        dgvTournamentResults.Columns[HANDICAP_COLUMN_NAME].ReadOnly       = true;
        dgvTournamentResults.Columns[TOTAL_SCORE_COLUMN_NAME].ReadOnly    = true;

        // Column sizing
        dgvTournamentResults.Columns[PLACE_STANDING_COLUMN_NAME].AutoSizeMode  = DataGridViewAutoSizeColumnMode.AllCells;
        dgvTournamentResults.Columns[MEMBER_NUMBER_COLUMN_NAME].AutoSizeMode   = DataGridViewAutoSizeColumnMode.AllCells;
        dgvTournamentResults.Columns[FULLNAME_COLUMN_NAME].AutoSizeMode        = DataGridViewAutoSizeColumnMode.Fill;
        dgvTournamentResults.Columns[HANDICAP_COLUMN_NAME].AutoSizeMode        = DataGridViewAutoSizeColumnMode.AllCells;
        dgvTournamentResults.Columns[TOTAL_SCORE_COLUMN_NAME].AutoSizeMode     = DataGridViewAutoSizeColumnMode.AllCells;
        dgvTournamentResults.Columns[EARNINGS_COLUMN_NAME].AutoSizeMode        = DataGridViewAutoSizeColumnMode.AllCells;
        dgvTournamentResults.Columns[PROGRESSIVEPOT_COLUMN_NAME].AutoSizeMode  = DataGridViewAutoSizeColumnMode.AllCells;

        // Wire the auto-fill event
        dgvTournamentResults.CellEndEdit += Dgv2Day_CellEndEdit;
    }

    /// <summary>
    /// Adds one row per placing (from Start Place to End Place) into the 2-day grid,
    /// pre-filling Place and Earnings from the round-setup panel.
    /// </summary>
    private void BtnAddRound_Click(object sender, EventArgs e)
    {
        int startPlace = (int)_nudStartPlace.Value;
        int endPlace   = (int)_nudEndPlace.Value;

        if (endPlace < startPlace)
        {
            MessageBox.Show("End place must be greater than or equal to start place.");
            return;
        }

        if (!decimal.TryParse(_txtRoundEarnings.Text, out decimal earnings))
        {
            MessageBox.Show("Please enter a valid earnings amount (e.g. 50.00).");
            return;
        }

        int squad = _cbxRoundSquad.SelectedItem != null ? (int)_cbxRoundSquad.SelectedItem : 1;

        int firstNewRow = _dt2Day.Rows.Count;
        for (int place = startPlace; place <= endPlace; place++)
        {
            DataRow newRow = _dt2Day.NewRow();
            newRow[PLACE_STANDING_COLUMN_NAME] = place;
            newRow[MEMBER_NUMBER_COLUMN_NAME]  = "";
            newRow[FULLNAME_COLUMN_NAME]       = "";
            newRow[HANDICAP_COLUMN_NAME]       = "";
            newRow[TOTAL_SCORE_COLUMN_NAME]    = "";
            newRow[EARNINGS_COLUMN_NAME]       = earnings;
            newRow[PROGRESSIVEPOT_COLUMN_NAME] = "0.00";
            newRow[MEMBER_ID_COLUMN_NAME]      = DBNull.Value;
            newRow[GAME_ID_COLUMN_NAME]        = DBNull.Value;
            newRow[SQUAD_COLUMN_NAME]          = squad;
            _dt2Day.Rows.Add(newRow);
        }

        // Move focus to the first Member Number cell in the new block
        if (firstNewRow < _dt2Day.Rows.Count)
        {
            dgvTournamentResults.CurrentCell = dgvTournamentResults[MEMBER_NUMBER_COLUMN_NAME, firstNewRow];
            dgvTournamentResults.BeginEdit(true);
        }
    }

    /// <summary>
    /// Fires when the user finishes editing a cell in the 2-day grid.
    /// If the edited cell is the Member Number column, triggers auto-fill for that row.
    /// </summary>
    private void Dgv2Day_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
        if (dgvTournamentResults.Columns[e.ColumnIndex].Name != MEMBER_NUMBER_COLUMN_NAME) return;

        var cellValue = dgvTournamentResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        string cellText = cellValue?.ToString()?.Trim() ?? "";
        if (!int.TryParse(cellText, out int memberNumber) || memberNumber <= 0) return;

        var squadObj = _dt2Day.Rows[e.RowIndex][SQUAD_COLUMN_NAME];
        int squad = squadObj != DBNull.Value && int.TryParse(squadObj.ToString(), out int sq) ? sq : 1;

        // Defer so the DataTable value is fully committed before reading it back
        BeginInvoke(() => AutoFillMemberRow(e.RowIndex, memberNumber, squad));
    }

    /// <summary>
    /// Looks up the member and their game for the given squad, then auto-fills
    /// Full Name, H/B*, Total Score, and the hidden Member ID / Game ID columns.
    /// H/B is pulled from the member's most recent finalized previous tournament,
    /// matching the same logic used by BuildWinnersList and FrmMemberScores.
    /// </summary>
    private void AutoFillMemberRow(int rowIndex, int memberNumber, int squad)
    {
        Member member = MemberDB.GetMember(memberNumber);
        if (member == null || member.Id == 0)
        {
            MessageBox.Show($"Member number {memberNumber} not found.");
            _dt2Day.Rows[rowIndex][MEMBER_NUMBER_COLUMN_NAME] = DBNull.Value;
            return;
        }

        // Look up H/B from the most recent finalized tournament (same logic as BuildWinnersList)
        int hdcp  = member.Handicap ?? 0;
        int bonus = member.Bonus;

        using (var dbPrev = new NineTapDb())
        {
            var latestDate = dbPrev.Participants
                .Where(p => p.Member.Number == memberNumber
                         && p.Tournament.Id != tourny.Id
                         && p.Game.IsFinalized
                         && p.Game.AdjustedAvg > 0)
                .Select(p => (DateTime?)p.Tournament.Date)
                .Max();

            if (latestDate != null)
            {
                var prevEntries = dbPrev.Participants
                    .Where(p => p.Member.Number == memberNumber
                             && p.Tournament.Id != tourny.Id
                             && p.Game.IsFinalized
                             && p.Tournament.Date == latestDate)
                    .Select(p => new { p.Game.AdjustedAvg, Bonus = p.Game.Bonus ?? 0, MoneyWon = p.Game.MoneyWon ?? 0 })
                    .ToList();

                if (prevEntries.Count > 0)
                {
                    var withAvg = prevEntries.FirstOrDefault(e => e.AdjustedAvg > 0);
                    if (withAvg != null)
                        hdcp = CalcService.CalculateHandicapPins(withAvg.AdjustedAvg);
                    bonus = prevEntries.Any(e => e.MoneyWon > 0)
                        ? prevEntries.Min(e => e.Bonus)
                        : prevEntries.Max(e => e.Bonus);
                }
            }
        }

        // Get the game entry for this member + tournament + squad
        Game game = GameDB.GetGameInTournament(member.Id, tourny.Id, squad);
        if (game == null)
        {
            MessageBox.Show($"No game entry found for member {memberNumber} in squad {squad}.\n" +
                            $"Make sure scores have been entered in Member Scores first.");
            _dt2Day.Rows[rowIndex][MEMBER_NUMBER_COLUMN_NAME] = DBNull.Value;
            return;
        }

        int totalScore = game.ScratchTotal + (game.GamesPlayed * (hdcp + bonus));

        _dt2Day.Rows[rowIndex][FULLNAME_COLUMN_NAME]    = member.FirstName + " " + member.LastName;
        _dt2Day.Rows[rowIndex][HANDICAP_COLUMN_NAME]    = $"{hdcp} + {bonus}";
        _dt2Day.Rows[rowIndex][TOTAL_SCORE_COLUMN_NAME] = totalScore;
        _dt2Day.Rows[rowIndex][MEMBER_ID_COLUMN_NAME]   = member.Number;
        _dt2Day.Rows[rowIndex][GAME_ID_COLUMN_NAME]     = game.Id;
    }

    #endregion

    /// <summary>
    /// Returns the ordinal representation of a given place, optionally appending a tie indicator.
    /// </summary>
    /// <param name="place">The numeric position to convert to an ordinal string. Must be a positive integer.</param>
    /// <param name="isTie">A boolean indicating whether to append a tie indicator. If <see langword="true"/>, "T" is appended to the
    /// result.</param>
    /// <returns>A string representing the ordinal form of the specified place, with an optional tie indicator.</returns>
    private static string GetOrdinalWithTie(int place, bool isTie)
    {
        string suffix;
        int ones = place % 10;
        int tens = (place % 100) / 10;
        if (tens == 1)
            suffix = "th";
        else if (ones == 1)
            suffix = "st";
        else if (ones == 2)
            suffix = "nd";
        else if (ones == 3)
            suffix = "rd";
        else
            suffix = "th";
        return $"{place}{suffix}{(isTie ? "T" : "" )}";
    }

    /// <summary>
    /// Parses a place-standing cell value that may have a trailing "T" tie indicator
    /// and returns the numeric portion as a <see cref="byte"/>.
    /// </summary>
    private static byte ParsePlaceStanding(object value)
    {
        string s = value?.ToString()?.TrimEnd('T') ?? "0";
        return byte.TryParse(s, out byte result) ? result : (byte)0;
    }

    private void ExportToExcel()
    {
        if (tourny.IsTwoDay)
        {
            // Save 2-day results to DB via the DGV (which is bound to _dt2Day)
            for (int i = 0; i < dgvTournamentResults.RowCount; i++)
            {
                var gameIdCell = dgvTournamentResults[GAME_ID_COLUMN_NAME, i].Value;
                if (gameIdCell == null || gameIdCell == DBNull.Value) continue;
                if (!int.TryParse(gameIdCell.ToString(), out int gameId) || gameId <= 0) continue;

                Game g = GameDB.GetGame(gameId);
                if (g == null) continue;

                g.PlaceStanding = ParsePlaceStanding(dgvTournamentResults[PLACE_STANDING_COLUMN_NAME, i].Value);
                g.MoneyWon = Convert.ToDecimal(dgvTournamentResults[EARNINGS_COLUMN_NAME, i].Value);

                if (decimal.TryParse(Convert.ToString(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, i].Value), out decimal _))
                    g.SidePot = Convert.ToDecimal(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, i].Value);
                else
                    g.Notes = $"Progressive Pot was entered as: {Convert.ToString(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, i].Value)}";

                db.SaveChanges();
            }
            MessageBox.Show("2-Day championship results saved successfully.");
            return;
        }

        // Saves participants' place standing and earnings won to the database
        for (int currentIndex = 0; currentIndex < dgvTournamentResults.RowCount; currentIndex++)
        {
            int gameId = Convert.ToInt32(dgvTournamentResults[GAME_ID_COLUMN_NAME, currentIndex].Value.ToString());
            Game g = GameDB.GetGame(gameId);

            g.PlaceStanding = ParsePlaceStanding(dgvTournamentResults[PLACE_STANDING_COLUMN_NAME, currentIndex].Value);
            g.MoneyWon = Convert.ToDecimal(dgvTournamentResults[EARNINGS_COLUMN_NAME, currentIndex].Value);

            // if user enters something other than a decimal number
            if (Decimal.TryParse(Convert.ToString(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value), out decimal _))
            {
                g.SidePot = Convert.ToDecimal(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value);
            }
            else
            {
                g.Notes = $"Progressive Pot was entered as: {Convert.ToString(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value)}";
            }
            
            // Phase 4: Removed g.gameRegionID assignment - stored in Participant entity
            db.SaveChanges();
        }

        string getFilePath = Path.GetFullPath("Resources/TournamentResultsTemplate.xlsx");
        string tourneyDate = tourny.Date.ToString("MM/dd/yyyy");
        string tournyDateDash = tourneyDate.Replace("/", "-");
        string tournamentDate = tournyDateDash; // Already formatted
        string fileName = tourny.Location + " " + tourny.Event + " " + tournamentDate + ".xlsx";
        string saveFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TournamentResultsCopy.xlsx");
        File.Copy(getFilePath, saveFile, true);

        try
        {
            using (var workbook = new XLWorkbook(saveFile))
            {
                var ws = workbook.Worksheet(1);
                ws.Cell(1, 1).Value = tourny.Location + tourny.Event;
                ws.Cell(2, 1).Value = tourny.Date;

                int excelRow = 4;
                int i = 0;
                while (i < dt.Rows.Count)
                {
                    if (excelRow >= 35)
                    {
                        ws.Row(excelRow).InsertRowsAbove(1);
                        ws.Range($"G{excelRow}:H{excelRow}").Merge();
                    }

                    var row = dt.Rows[i];
                    int currentPlace = 0;
                    int.TryParse(row[PLACE_STANDING_COLUMN_NAME]?.ToString()?.TrimEnd('T'), out currentPlace);
                    // Check for tie: if previous or next row has the same place
                    bool isTie = false;
                    if (i > 0)
                    {
                        int prevPlace = 0;
                        int.TryParse(dt.Rows[i - 1][PLACE_STANDING_COLUMN_NAME]?.ToString()?.TrimEnd('T'), out prevPlace);
                        if (prevPlace == currentPlace) isTie = true;
                    }
                    if (i < dt.Rows.Count - 1)
                    {
                        int nextPlace = 0;
                        int.TryParse(dt.Rows[i + 1][PLACE_STANDING_COLUMN_NAME]?.ToString()?.TrimEnd('T'), out nextPlace);
                        if (nextPlace == currentPlace) isTie = true;
                    }
                    // Parse the progressive pot once; spVal = 0 if the cell is empty or non-numeric.
                    decimal.TryParse(row[PROGRESSIVEPOT_COLUMN_NAME]?.ToString(), out decimal spVal);
                    // Use InvariantCulture so the decimal separator in the formula literal is always "."
                    // regardless of the system locale (e.g. prevents "20,5" in German locales).
                    string sidePotValue = spVal.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    // Use ordinal with tie for place standing
                    ws.Cell(excelRow, 1).Value = GetOrdinalWithTie(currentPlace, isTie);
                    ws.Cell(excelRow, 2).Value = row[FULLNAME_COLUMN_NAME]?.ToString();
                    ws.Cell(excelRow, 6).Value = row[HANDICAP_COLUMN_NAME]?.ToString();
                    ws.Cell(excelRow, 7).Value = row[TOTAL_SCORE_COLUMN_NAME]?.ToString();
                    ws.Cell(excelRow, 9).Value = row[EARNINGS_COLUMN_NAME] != null
                        ? double.TryParse(row[EARNINGS_COLUMN_NAME].ToString(), out var val)
                            ? val.ToString("C0")
                            : row[EARNINGS_COLUMN_NAME]?.ToString()
                        : "$0";
                    ws.Cell(excelRow, 11).Value = row[PLACE_STANDING_COLUMN_NAME]?.ToString()?.TrimEnd('T');
                    ws.Cell(excelRow, 12).Value = row[MEMBER_ID_COLUMN_NAME]?.ToString();
                    ws.Cell(excelRow, 15).FormulaA1 = $"=I{excelRow}-M{excelRow}-N{excelRow}+{sidePotValue}";
                    // Write as a numeric value so Excel treats it as a number, not text.
                    ws.Cell(excelRow, 8).Value = spVal;

                    // Any entry that placed 1st–3rd gets a progressive pot row directly below it.
                    // The template pre-formats those rows at positions 5, 7, and 9 (covering excelRows 4, 6, 8).
                    // When extra ties push a 4th (or more) top-3 entry past row 8, insert a new row so the
                    // progressive pot slot is always present regardless of how many bowlers tied into places 1–3.
                    if (currentPlace >= 1 && currentPlace <= 3)
                    {
                        if (excelRow > 8)
                        {
                            ws.Row(excelRow + 1).InsertRowsAbove(1);
                            ws.Range($"G{excelRow + 1}:H{excelRow + 1}").Merge();
                        }
                        // Reuse the already-parsed value — no second TryParse needed.
                        ws.Cell(excelRow + 1, 9).Value = spVal;
                        excelRow++;
                    }

                    i++;
                    excelRow++;
                }

                // Set total payout
                double money = 0;
                for (int k = 0; k < dt.Rows.Count; k++)
                    money += Convert.ToDouble(dt.Rows[k][EARNINGS_COLUMN_NAME]);
                ws.Cell(2, 8).Value = money;

                // Save dialog
                SaveFileDialog savefile = new()
                {
                    Filter = FileHelper.GetExcelFilterStringForFileDialogs(),
                    FileName = fileName
                };
                DialogResult result = savefile.ShowDialog();
                if (result == DialogResult.OK)
                {
                    workbook.SaveAs(savefile.FileName);
                    MessageBox.Show("Excel file created , you can find the file at: " + savefile.FileName);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Must choose a file to export to.\n *Must have at least 20 bowlers and 4 money winners*\n" + ex.Message);
        }
    }

    /// <summary>
    /// Runs AcceptClientInputForResults if the user presses the "Enter" key
    /// </summary>
    private void TbClientInputCount_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            AcceptClientInputForResults();
        }
    }

    /// <summary>
    /// Clears dgvTournamentResults and repopulates with the winners
    /// </summary>
    private void AcceptClientInputForResults()
    {
        this.dgvTournamentResults.DataSource = null;
        this.dgvTournamentResults.Rows.Clear();
        this.dgvTournamentResults.Columns.Clear();

        if (tbClientInputCount.Text == null || tbClientInputCount.Text == "")
        {
            MessageBox.Show("Please Enter Number Of Winners");
        }
        else
        {
            try
            {
                clientInput = Convert.ToInt32(tbClientInputCount.Text);
                tbClientInputCount.Enabled = false;

                // For doubles, clientInput = number of teams. Both members of each team share
                // the same PlaceStanding (set by BuildWinnersListDoubles), so we filter directly
                // instead of calling MakeTopMembersByPlacementList, which re-ranks by TotalScore
                // and would place tied pairs at 1,1,3,3,5,5... causing only half as many teams
                // to pass a simple <= clientInput threshold.
                if (tourny.Doubles)
                {
                    clientRequested = [.. winners
                        .Where(m => m.PlaceStanding <= clientInput)
                        .OrderBy(m => m.PlaceStanding)
                        .ThenBy(m => m.MemberNumber)];
                }
                else
                {
                    clientRequested = Calculations.Calculations.MakeTopMembersByPlacementList(winners, clientInput);
                }

                // For doubles the grid shows 2 rows per team; scale the display slot count accordingly
                int gridRowCount = tourny.Doubles ? clientInput * 2 : clientInput;

                // Create datagridview and populate with cashedWinners list
                CreateDataGridView(clientRequested, gridRowCount);
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a nunmber");
            }
        }
    }

    /// <summary>
    /// Populates dgvTournamentResults when clicked on
    /// </summary>
    private void BtnPaste_Click(object sender, EventArgs e)
    {
        // Stops this method from working if user didnt enter the number of winners
        if (string.IsNullOrWhiteSpace(tbClientInputCount.Text))
        {
            MessageBox.Show("Please enter the number of winners first");
            return;
        }

        // Stops this method from working if the user did not copy from Excel first
        string clipboard = Clipboard.GetText();
        if (string.IsNullOrWhiteSpace(clipboard))
        {
            MessageBox.Show("Please copy the earnings from Excel first");
            return;
        }

        // Removes all $ symboles
        clipboard = clipboard.Replace("$", "");

        // Lines becomes clipboard as an array
        string[] lines = clipboard.Replace("\n", "").Split('\r');
        // Lines2 becomes an empty version of lines
        string[] lines2 = new string[lines.Length];

        // Populates lines2 with all values in lines
        for(int t = 0; t < lines.Length; t++)
        {
           lines2[t] = lines[t];
        }
        int row = 0;
        int col = 4;

        int pasteAble = Convert.ToInt32(tbClientInputCount.Text) + 3; // +3 for the pro pot entries
        int pasteCount = lines.Length;
        int paste;
        if (pasteCount < pasteAble)
        {
            paste = pasteCount - 1;
        }
        else
        {
            paste = pasteAble; 
        }

        // Populates dgvTournamentResults
        for (int i = 0; i < paste; i++)
        {
            string check = lines2[i];
            if (check != "")
            {
                if (i == 1 || i == 3 || i == 5)
                {
                    dgvTournamentResults[col + 3, row].Value = lines2[i];
                    row++;
                }
                else
                {
                    dgvTournamentResults[col, row].Value = lines2[i];
                    if (i > 5) {
                        row++;
                    }
                }
            }
        }
    }
}
