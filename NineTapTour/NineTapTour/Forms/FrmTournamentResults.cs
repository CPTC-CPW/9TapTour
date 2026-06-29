using NineTapTour.Database;
using CalcService = NineTapTour.Calculations.TournamentCalculations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NineTapTour.Models;
using ClosedXML.Excel;
using NineTapTour.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Helpers;

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
    const string PLACE_GROUP_LABEL_COLUMN_NAME = "Place Group Label";
    const string PLACE_SORT_START_COLUMN_NAME = "Place Sort Start";

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
                object gameIdObj = _dt2Day.Rows[i][GAME_ID_COLUMN_NAME];
                if (gameIdObj == DBNull.Value || !int.TryParse(gameIdObj.ToString(), out int gameId) || gameId <= 0) continue;

                if (!TryGet2DayPlaceGroup(_dt2Day.Rows[i], out int placeStart, out string placeLabel))
                {
                    MessageBox.Show($"Invalid place grouping on row {i + 1}. Use a numeric place or a range like 46th - 59th.");
                    continue;
                }

                Game g = db.Games.Find(gameId);
                if (g == null) continue;

                g.PlaceStanding = placeStart;
                g.PlaceStandingLabel = placeLabel;
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

        // Save all changes made to the dataGridView.
        // Track processed GameIds so a member on multiple teams in the same squad (same Game
        // record) is only written once — the first row wins, preventing a later row from
        // overwriting the place and earnings already saved.
        var savedGameIds = new HashSet<int>();
        for (int currentIndex = 0; currentIndex < clientRequested.Count; currentIndex++)
        {
            int gameId = Convert.ToInt32(dgvTournamentResults[GAME_ID_COLUMN_NAME, currentIndex].Value.ToString());
            if (!savedGameIds.Add(gameId)) continue;
            // Use the form-level context's identity map (Find) so EF Core never sees two
            // instances of the same Game key — fixes the "already being tracked" exception.
            Game g = db.Games.Find(gameId);
            if (g == null) continue;

            g.PlaceStanding = ParsePlaceStanding(dgvTournamentResults[PLACE_STANDING_COLUMN_NAME, currentIndex].Value);
            g.PlaceStandingLabel = null;
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
                            && int.TryParse(dt.Rows[i - 1][PLACE_STANDING_COLUMN_NAME]?.ToString()?.TrimEnd('T'), out int prev)
                            && prev == place)
                       || (i < dt.Rows.Count - 1
                            && int.TryParse(dt.Rows[i + 1][PLACE_STANDING_COLUMN_NAME]?.ToString()?.TrimEnd('T'), out int next)
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

        // Batch-query each member's handicap from their most recent finalized prior tournament.
        // Bonus is read directly from the Member record via MemberBonus (not game history).
        var memberNumbers = bowlers.Select(b => b.MemberNumber).Distinct().ToHashSet();
        var prevHdcpByMember = BuildPrevHdcpByMember(memberNumbers, tourny.Id);

        foreach (var b in bowlers)
        {
            if (b.IsComp)
            {
                compEntries++;
            }

            ExcelMember m = new()
            {
                MemberNumber = b.MemberNumber,
                Name = b.BowlerName,
                Handicap = prevHdcpByMember.TryGetValue(b.MemberNumber, out int prevHdcp) && prevHdcp > 0
                    ? prevHdcp
                    : Convert.ToInt32(b.Handicap),
                Bonus = b.MemberBonus,
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
        // Batch-query each member's handicap from their most recent finalized prior tournament.
        // Bonus is read directly from the Member record via MemberBonus (not game history).
        var memberNumbers = bowlers.Select(b => b.MemberNumber).Distinct().ToHashSet();
        var prevHdcpByMember = BuildPrevHdcpByMember(memberNumbers, tourny.Id);

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

            int hdcp1  = prevHdcpByMember.TryGetValue(m1.MemberNumber, out int ph1) && ph1 > 0 ? ph1 : Convert.ToInt32(m1.Handicap);
            int hdcp2  = prevHdcpByMember.TryGetValue(m2.MemberNumber, out int ph2) && ph2 > 0 ? ph2 : Convert.ToInt32(m2.Handicap);
            int bonus1 = m1.MemberBonus;
            int bonus2 = m2.MemberBonus;

            int combinedHdcpTotal = (m1.Game1 ?? 0) + (m1.Game2 ?? 0)
                                  + (m2.Game1 ?? 0) + (m2.Game2 ?? 0)
                                  + 2 * (hdcp1 + bonus1)
                                  + 2 * (hdcp2 + bonus2);

            teamRows.Add((combinedHdcpTotal, m1, m2, hdcp1, bonus1, hdcp2, bonus2));
        }

        // Sort descending, assign places with tie detection
        teamRows.Sort((a, b) => b.CombinedHdcpTotal.CompareTo(a.CombinedHdcpTotal));
        int[] teamPlaces = new int[teamRows.Count];
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
    /// Batch-queries the most recent finalized tournament prior to this one for each member
    /// in <paramref name="memberNumbers"/> and returns the handicap computed from that
    /// entry's AdjustedAvg. Members with no qualifying prior entry are absent from the result.
    /// Bonus is intentionally excluded — callers should read it directly from
    /// <see cref="WinnerListMemberViewModel.MemberBonus"/> or <see cref="Member.Bonus"/>.
    /// </summary>
    private Dictionary<int, int> BuildPrevHdcpByMember(HashSet<int> memberNumbers, int excludeTournamentId)
    {
        var result = new Dictionary<int, int>();
        if (memberNumbers.Count == 0) return result;

        using var dbPrev = new NineTapDb();

        var latestDates = dbPrev.Participants
            .Where(p => memberNumbers.Contains(p.Member.Number)
                     && p.Tournament.Id != excludeTournamentId
                     && p.Game.IsFinalized
                     && p.Game.AdjustedAvg > 0)
            .GroupBy(p => p.Member.Number)
            .Select(g => new { MemberNumber = g.Key, LatestDate = g.Max(p => p.Tournament.Date) })
            .ToList();

        foreach (var item in latestDates)
        {
            int? adjAvg = dbPrev.Participants
                .Where(p => p.Member.Number == item.MemberNumber
                         && p.Tournament.Id != excludeTournamentId
                         && p.Game.IsFinalized
                         && p.Tournament.Date == item.LatestDate
                         && p.Game.AdjustedAvg > 0)
                .Select(p => (int?)p.Game.AdjustedAvg)
                .FirstOrDefault();

            if (adjAvg.HasValue)
                result[item.MemberNumber] = CalcService.CalculateHandicapPins(adjAvg.Value);
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

    /// <summary>
    /// Builds an export table that mirrors doubles Team View rows (one row per team).
    /// Handicap is intentionally blank for team export, and Full Name contains both partners.
    /// </summary>
    private DataTable BuildTeamViewExportTable()
    {
        DataTable teamDt = new();
        teamDt.Columns.Add(PLACE_STANDING_COLUMN_NAME).ReadOnly = true;
        teamDt.Columns.Add(FULLNAME_COLUMN_NAME).ReadOnly = true;
        teamDt.Columns.Add(HANDICAP_COLUMN_NAME).ReadOnly = true;
        teamDt.Columns.Add(TOTAL_SCORE_COLUMN_NAME).ReadOnly = true;
        teamDt.Columns.Add(EARNINGS_COLUMN_NAME).ReadOnly = true;
        teamDt.Columns.Add(MEMBER_ID_COLUMN_NAME).ReadOnly = true;
        teamDt.Columns.Add(GAME_ID_COLUMN_NAME).ReadOnly = true;
        teamDt.Columns.Add(PROGRESSIVEPOT_COLUMN_NAME).ReadOnly = true;

        // BuildWinnersListDoubles writes pairs consecutively: [T1M1, T1M2, T2M1, T2M2, ...]
        var teamPairs = new List<(ExcelMember M1, ExcelMember M2, int Place)>();
        for (int i = 0; i + 1 < winners.Count; i += 2)
        {
            var m1 = winners[i];
            var m2 = winners[i + 1];
            if (m1.PlaceStanding > clientInput) continue;
            teamPairs.Add((m1, m2, m1.PlaceStanding));
        }
        teamPairs.Sort((a, b) => a.Place.CompareTo(b.Place));

        foreach (var (m1, m2, place) in teamPairs)
        {
            decimal earn1 = m1.MoneyWon ?? 0m;
            decimal earn2 = m2.MoneyWon ?? 0m;
            decimal side1 = m1.SidePot ?? 0m;
            decimal side2 = m2.SidePot ?? 0m;

            DataRow row = teamDt.NewRow();
            row[PLACE_STANDING_COLUMN_NAME] = place;
            row[FULLNAME_COLUMN_NAME] = $"{m1.Name} & {m2.Name}";
            row[HANDICAP_COLUMN_NAME] = ""; // user requested handicap ignored in team export
            row[TOTAL_SCORE_COLUMN_NAME] = m1.TotalScore;
            row[EARNINGS_COLUMN_NAME] = earn1 + earn2;
            row[MEMBER_ID_COLUMN_NAME] = "";
            row[GAME_ID_COLUMN_NAME] = "";
            row[PROGRESSIVEPOT_COLUMN_NAME] = side1 + side2;
            teamDt.Rows.Add(row);
        }

        return teamDt;
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
        x += 100;
        _btnAddRound = new Button { Text = "Add Round", Location = new Point(x, 2), Size = new Size(90, 28) };
        _btnAddRound.Click += BtnAddRound_Click;
        _pnlRoundSetup.Controls.Add(_btnAddRound);

        Controls.Add(_pnlRoundSetup);

        // Set up the 2-day DataTable (no ReadOnly on DataTable columns so auto-fill can write freely)
        _dt2Day = new DataTable();
        _dt2Day.Columns.Add(PLACE_STANDING_COLUMN_NAME);
        _dt2Day.Columns.Add(PLACE_GROUP_LABEL_COLUMN_NAME);
        _dt2Day.Columns.Add(PLACE_SORT_START_COLUMN_NAME, typeof(int));
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
        _dt2Day.DefaultView.Sort               = PLACE_SORT_START_COLUMN_NAME + " ASC, " + TOTAL_SCORE_COLUMN_NAME + " DESC";
        dgvTournamentResults.AllowUserToAddRows = false;

        // Hide internal/lookup columns
        dgvTournamentResults.Columns[MEMBER_ID_COLUMN_NAME].Visible = false;
        dgvTournamentResults.Columns[GAME_ID_COLUMN_NAME].Visible   = false;
        dgvTournamentResults.Columns[SQUAD_COLUMN_NAME].Visible     = false;
        dgvTournamentResults.Columns[PLACE_GROUP_LABEL_COLUMN_NAME].Visible = false;
        dgvTournamentResults.Columns[PLACE_SORT_START_COLUMN_NAME].Visible  = false;

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

        // Reload any previously saved entries from the database so the director
        // can close and reopen the form between rounds.
        LoadExisting2DayData();
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

        string placeGroupLabel = Build2DayPlaceGroupLabel(startPlace, endPlace);

        int firstNewRow = _dt2Day.Rows.Count;
        for (int place = startPlace; place <= endPlace; place++)
        {
            DataRow newRow = _dt2Day.NewRow();
            newRow[PLACE_STANDING_COLUMN_NAME] = placeGroupLabel;
            newRow[PLACE_GROUP_LABEL_COLUMN_NAME] = placeGroupLabel;
            newRow[PLACE_SORT_START_COLUMN_NAME] = startPlace;
            newRow[MEMBER_NUMBER_COLUMN_NAME]  = "";
            newRow[FULLNAME_COLUMN_NAME]       = "";
            newRow[HANDICAP_COLUMN_NAME]       = "";
            newRow[TOTAL_SCORE_COLUMN_NAME]    = "";
            newRow[EARNINGS_COLUMN_NAME]       = earnings;
            newRow[PROGRESSIVEPOT_COLUMN_NAME] = "0.00";
            newRow[MEMBER_ID_COLUMN_NAME]      = DBNull.Value;
            newRow[GAME_ID_COLUMN_NAME]        = DBNull.Value;
            newRow[SQUAD_COLUMN_NAME]          = DBNull.Value;
            _dt2Day.Rows.Add(newRow);
        }

        // Move focus to the first Member Number cell in the new block.
        // DefaultView.Sort is active, so the DGV view index differs from the DataTable row index;
        // find the view row that wraps the first newly-added DataRow.
        if (firstNewRow < _dt2Day.Rows.Count)
        {
            DataRow firstAdded = _dt2Day.Rows[firstNewRow];
            for (int i = 0; i < dgvTournamentResults.Rows.Count; i++)
            {
                if (dgvTournamentResults.Rows[i].DataBoundItem is DataRowView drv && drv.Row == firstAdded)
                {
                    dgvTournamentResults.CurrentCell = dgvTournamentResults[MEMBER_NUMBER_COLUMN_NAME, i];
                    dgvTournamentResults.BeginEdit(true);
                    break;
                }
            }
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

        object cellValue = dgvTournamentResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        string cellText = cellValue?.ToString()?.Trim() ?? "";
        if (!int.TryParse(cellText, out int memberNumber) || memberNumber <= 0) return;

        // DataBoundItem is a DataRowView; capturing it now ensures we target the correct
        // DataRow even after DefaultView.Sort may have reordered the displayed rows.
        if (dgvTournamentResults.Rows[e.RowIndex].DataBoundItem is not DataRowView drv) return;
        DataRow dataRow = drv.Row;

        // Defer so the DataTable value is fully committed before reading it back
        BeginInvoke(() => AutoFillMemberRow(dataRow, memberNumber));
    }

    /// <summary>
    /// Looks up the member and their best game entry in this tournament across all squads, then auto-fills
    /// Full Name, H/B*, Total Score, and the hidden Member ID / Game ID columns.
    /// Handicap is derived from the member's most recent finalized previous tournament's AdjustedAvg.
    /// Bonus is read directly from the Member record, not from game history.
    /// </summary>
    private void AutoFillMemberRow(DataRow dataRow, int memberNumber)
    {
        Member member = MemberDB.GetMember(memberNumber);
        if (member == null || member.Id == 0)
        {
            MessageBox.Show($"Member number {memberNumber} not found.");
            dataRow[MEMBER_NUMBER_COLUMN_NAME] = DBNull.Value;
            return;
        }

        // Bonus always comes from the Member record.
        // Handicap comes from the most recent finalized prior tournament's AdjustedAvg (falls back to Member.Handicap).
        int bonus = member.Bonus;
        var prevHdcpByMember = BuildPrevHdcpByMember(new HashSet<int> { memberNumber }, tourny.Id);
        int hdcp = prevHdcpByMember.TryGetValue(memberNumber, out int prevHdcp)
            ? prevHdcp
            : (member.Handicap ?? 0);

        // Get the highest-scoring game entry for this member in this tournament (all squads).
        // ScratchTotal is [NotMapped] so ordering must happen client-side after fetching candidates.
        Game game;
        using (var dbGame = new NineTapDb())
        {
            game = dbGame.Participants
                .Where(p => p.Member.Id == member.Id && p.Tournament.Id == tourny.Id)
                .Select(p => p.Game)
                .AsEnumerable()
                .OrderByDescending(g => g.ScratchTotal)
                .ThenByDescending(g => g.GamesPlayed)
                .ThenByDescending(g => g.Id)
                .FirstOrDefault();
        }

        if (game == null)
        {
            MessageBox.Show($"No game entry found for member {memberNumber} in this 2-day tournament.\n" +
                            $"Make sure scores have been entered in Member Scores first.");
            dataRow[MEMBER_NUMBER_COLUMN_NAME] = DBNull.Value;
            return;
        }

        int totalScore = game.ScratchTotal + (game.GamesPlayed * (hdcp + bonus));

        dataRow[FULLNAME_COLUMN_NAME]    = member.FirstName + " " + member.LastName;
        dataRow[HANDICAP_COLUMN_NAME]    = $"{hdcp} + {bonus}";
        dataRow[TOTAL_SCORE_COLUMN_NAME] = totalScore;
        dataRow[MEMBER_ID_COLUMN_NAME]   = member.Number;
        dataRow[GAME_ID_COLUMN_NAME]     = game.Id;
    }

    /// <summary>
    /// Loads any previously saved 2-day results from the database into <see cref="_dt2Day"/>.
    /// Called once during form load so the director can close and reopen the form between rounds.
    /// </summary>
    private void LoadExisting2DayData()
    {
        var saved = TournamentDB.GetWinnerListMemberData(tourny.Id)
            .Where(b => b.PlaceStanding > 0)
            .OrderBy(b => b.PlaceStanding)
            .ToList();

        if (saved.Count == 0) return;

        foreach (var b in saved)
        {
            DataRow row = _dt2Day.NewRow();
            int placeStart = b.PlaceStanding ?? 0;
            string placeLabel = string.IsNullOrWhiteSpace(b.PlaceStandingLabel)
                ? (placeStart > 0 ? GetOrdinalWithTie(placeStart, false) : "")
                : b.PlaceStandingLabel;

            row[PLACE_STANDING_COLUMN_NAME] = placeLabel;
            row[PLACE_GROUP_LABEL_COLUMN_NAME] = placeLabel;
            row[PLACE_SORT_START_COLUMN_NAME] = placeStart;
            row[MEMBER_NUMBER_COLUMN_NAME]  = b.MemberNumber;
            row[FULLNAME_COLUMN_NAME]       = "";
            row[HANDICAP_COLUMN_NAME]       = "";
            row[TOTAL_SCORE_COLUMN_NAME]    = "";
            row[EARNINGS_COLUMN_NAME]       = b.MoneyWon ?? 0m;
            row[PROGRESSIVEPOT_COLUMN_NAME] = b.SidePot?.ToString("F2") ?? "0.00";
            row[MEMBER_ID_COLUMN_NAME]      = b.MemberId;
            row[GAME_ID_COLUMN_NAME]        = b.GameId;
            row[SQUAD_COLUMN_NAME]          = DBNull.Value;
            _dt2Day.Rows.Add(row);
        }

        // Fill in Name, H/B, and TotalScore for each pre-loaded row
        for (int i = 0; i < _dt2Day.Rows.Count; i++)
        {
            if (!int.TryParse(_dt2Day.Rows[i][MEMBER_NUMBER_COLUMN_NAME]?.ToString(), out int memberNumber)) continue;
            AutoFillMemberRow(_dt2Day.Rows[i], memberNumber);
        }
    }

    #endregion

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

    private static string Build2DayPlaceGroupLabel(int startPlace, int endPlace)
    {
        return $"{GetOrdinalWithTie(startPlace, false)} - {GetOrdinalWithTie(endPlace, false)}";
    }

    private static bool TryParsePlaceStartFromText(string text, out int placeStart)
    {
        placeStart = 0;
        if (string.IsNullOrWhiteSpace(text)) return false;

        Match range = RegexHelpers.PlacingRange().Match(text.Trim());
        if (range.Success)
            return int.TryParse(range.Groups[1].Value, out placeStart) && placeStart > 0;

        Match single = RegexHelpers.SinglePlacing().Match(text.Trim());
        return single.Success
            && int.TryParse(single.Groups[1].Value, out placeStart)
            && placeStart > 0;
    }

    private static bool TryGet2DayPlaceGroup(DataRow row, out int placeStart, out string placeLabel)
    {
        placeStart = 0;
        placeLabel = "";

        string explicitLabel = row.Table.Columns.Contains(PLACE_GROUP_LABEL_COLUMN_NAME)
            ? Convert.ToString(row[PLACE_GROUP_LABEL_COLUMN_NAME])?.Trim() ?? ""
            : "";
        string displayValue = Convert.ToString(row[PLACE_STANDING_COLUMN_NAME])?.Trim() ?? "";

        string sourceText = !string.IsNullOrWhiteSpace(explicitLabel) ? explicitLabel : displayValue;
        if (!TryParsePlaceStartFromText(sourceText, out placeStart))
            return false;

        if (string.IsNullOrWhiteSpace(explicitLabel))
            placeLabel = sourceText;
        else
            placeLabel = explicitLabel;

        if (row.Table.Columns.Contains(PLACE_SORT_START_COLUMN_NAME))
            row[PLACE_SORT_START_COLUMN_NAME] = placeStart;
        if (row.Table.Columns.Contains(PLACE_GROUP_LABEL_COLUMN_NAME))
            row[PLACE_GROUP_LABEL_COLUMN_NAME] = placeLabel;

        return true;
    }

    private void ExportToExcel()
    {
        if (tourny.IsTwoDay)
        {
            // Save 2-day results to DB via the DGV (which is bound to _dt2Day)
            for (int i = 0; i < dgvTournamentResults.RowCount; i++)
            {
                object gameIdCell = dgvTournamentResults[GAME_ID_COLUMN_NAME, i].Value;
                if (gameIdCell == null || gameIdCell == DBNull.Value) continue;
                if (!int.TryParse(gameIdCell.ToString(), out int gameId) || gameId <= 0) continue;

                Game g = GameDB.GetGame(gameId);
                if (g == null) continue;

                if (dgvTournamentResults.Rows[i].DataBoundItem is not DataRowView dataRowView) continue;
                if (!TryGet2DayPlaceGroup(dataRowView.Row, out int placeStart, out string placeLabel))
                {
                    MessageBox.Show($"Invalid place grouping on row {i + 1}. Use a numeric place or a range like 46th - 59th.");
                    continue;
                }

                g.PlaceStanding = placeStart;
                g.PlaceStandingLabel = placeLabel;
                g.MoneyWon = Convert.ToDecimal(dgvTournamentResults[EARNINGS_COLUMN_NAME, i].Value);

                if (decimal.TryParse(Convert.ToString(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, i].Value), out decimal _))
                    g.SidePot = Convert.ToDecimal(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, i].Value);
                else
                    g.Notes = $"Progressive Pot was entered as: {Convert.ToString(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, i].Value)}";

                db.SaveChanges();
            }
        }

        // Saves participants' place standing and earnings won to the database.
        // Guard against duplicate GameIds (member on multiple teams in the same squad).
        if (!tourny.IsTwoDay)
        {
        var exportSavedGameIds = new HashSet<int>();
        for (int currentIndex = 0; currentIndex < dgvTournamentResults.RowCount; currentIndex++)
        {
            int gameId = Convert.ToInt32(dgvTournamentResults[GAME_ID_COLUMN_NAME, currentIndex].Value.ToString());
            if (!exportSavedGameIds.Add(gameId)) continue;
            Game g = GameDB.GetGame(gameId);

            g.PlaceStanding = ParsePlaceStanding(dgvTournamentResults[PLACE_STANDING_COLUMN_NAME, currentIndex].Value);
            g.PlaceStandingLabel = null;
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
        } // end if (!tourny.IsTwoDay)

        string tourneyDate = tourny.Date.ToString("MM/dd/yyyy");
        string tournyDateDash = tourneyDate.Replace("/", "-");
        string tournamentDate = tournyDateDash; // Already formatted
        string fileName = tourny.Location + " " + tourny.Event + " " + tournamentDate + ".xlsx";

        string saveFile;
        using OpenFileDialog openDialog = new()
        {
            Title  = "Select Existing Results File",
            Filter = FileHelper.GetExcelFilterStringForFileDialogs(),
        };
        if (openDialog.ShowDialog() != DialogResult.OK)
            return;
        saveFile = openDialog.FileName;

        // Select the source table: 2-day grid (sorted by Place), doubles Team View, or the standard winners table.
        DataTable exportTable = tourny.IsTwoDay
            ? _dt2Day.DefaultView.ToTable()
            : (tourny.Doubles && _inTeamView)
                ? BuildTeamViewExportTable()
                : dt;

        // For doubles: detect bowlers who placed multiple times and consolidate their earnings.
        Dictionary<int, decimal> doublesConsolidatedEarnings = null;
        HashSet<int> doublesSecondaryRowIndices = null;
        Dictionary<int, List<string>> doublesPlaceLabelsForMemo = null;

        if (tourny.Doubles && !_inTeamView && !tourny.IsTwoDay)
        {
            var consolidation = BuildDoublesConsolidation(exportTable);
            if (consolidation.SecondaryRowIndices.Count > 0)
            {
                doublesConsolidatedEarnings = consolidation.CombinedEarnings;
                doublesSecondaryRowIndices   = consolidation.SecondaryRowIndices;
                doublesPlaceLabelsForMemo    = consolidation.PlaceLabelsForMemo;
            }
        }

        // Preload membership-current status for rows that carry a numeric member number.
        var memberNumbers = exportTable.Rows.Cast<DataRow>()
            .Select(r => r[MEMBER_ID_COLUMN_NAME]?.ToString())
            .Where(s => int.TryParse(s, out _))
            .Select(int.Parse)
            .Distinct()
            .ToList();

        var isMembershipCurrentByMemberNumber = new Dictionary<int, bool>();
        if (memberNumbers.Count > 0)
        {
            using var dbMembers = new NineTapDb();
            isMembershipCurrentByMemberNumber = dbMembers.Members
                .Where(m => memberNumbers.Contains(m.Number))
                .Select(m => new { m.Number, m.IsLifetimeMember, m.LastPayment })
                .ToDictionary(
                    x => x.Number,
                    x => x.IsLifetimeMember
                        || (x.LastPayment.HasValue && (x.LastPayment.Value.Year + 1) >= DateTime.Today.Year));
        }

        try
        {
            using var workbook = new XLWorkbook(saveFile);
            var ws = workbook.Worksheet(1);
            ws.Cell(1, 1).Value = tourny.Location + tourny.Event;
            ws.Cell(2, 1).Value = tourny.Date;

            // For 2-Day tournaments, we need to change "Total Score" header
            if (tourny.IsTwoDay)
                ws.Cell(3, 7).Value = "Qualifying Score";

            var resultIdxToExcelRow        = new Dictionary<int, int>();
            var excelRowsWithProgressivePot = new HashSet<int>();
            int excelRow = 4;
            int i = 0;
            while (i < exportTable.Rows.Count)
            {
                var row = exportTable.Rows[i];
                int currentPlace = 0;
                string placeDisplay = row[PLACE_STANDING_COLUMN_NAME]?.ToString() ?? "";
                if (tourny.IsTwoDay)
                {
                    if (TryParsePlaceStartFromText(row[PLACE_GROUP_LABEL_COLUMN_NAME]?.ToString(), out int parsedFromLabel))
                        currentPlace = parsedFromLabel;
                    else if (TryParsePlaceStartFromText(placeDisplay, out int parsedFromDisplay))
                        currentPlace = parsedFromDisplay;
                }
                else
                {
                    int.TryParse(placeDisplay.TrimEnd('T'), out currentPlace);
                }

                // Check for tie: if previous or next row has the same place
                bool isTie = false;
                if (i > 0)
                {
                    int prevPlace = 0;
                    int.TryParse(exportTable.Rows[i - 1][PLACE_STANDING_COLUMN_NAME]?.ToString()?.TrimEnd('T'), out prevPlace);
                    if (prevPlace == currentPlace) isTie = true;
                }
                if (i < exportTable.Rows.Count - 1)
                {
                    int nextPlace = 0;
                    int.TryParse(exportTable.Rows[i + 1][PLACE_STANDING_COLUMN_NAME]?.ToString()?.TrimEnd('T'), out nextPlace);
                    if (nextPlace == currentPlace) isTie = true;
                }
                // Parse the progressive pot once; spVal = 0 if the cell is empty or non-numeric.
                decimal.TryParse(row[PROGRESSIVEPOT_COLUMN_NAME]?.ToString(), out decimal spVal);
                // Use InvariantCulture so the decimal separator in the formula literal is always "."
                // regardless of the system locale (e.g. prevents "20,5" in German locales).
                string sidePotValue = spVal.ToString(System.Globalization.CultureInfo.InvariantCulture);

                if (tourny.IsTwoDay)
                {
                    string groupedLabel = row[PLACE_GROUP_LABEL_COLUMN_NAME]?.ToString();
                    ws.Cell(excelRow, 1).Value = string.IsNullOrWhiteSpace(groupedLabel)
                        ? placeDisplay
                        : groupedLabel;
                }
                else
                {
                    // Use ordinal with tie for place standing
                    ws.Cell(excelRow, 1).Value = GetOrdinalWithTie(currentPlace, isTie);
                }
                ws.Cell(excelRow, 2).Value = row[FULLNAME_COLUMN_NAME]?.ToString();
                ws.Cell(excelRow, 6).Value = row[HANDICAP_COLUMN_NAME]?.ToString();
                ws.Cell(excelRow, 7).Value = row[TOTAL_SCORE_COLUMN_NAME]?.ToString();
                resultIdxToExcelRow[i] = excelRow;

                double earningsForExcel = 0;
                if (row[EARNINGS_COLUMN_NAME] != null)
                    double.TryParse(row[EARNINGS_COLUMN_NAME].ToString(), out earningsForExcel);
                if (doublesConsolidatedEarnings != null)
                {
                    if (doublesSecondaryRowIndices.Contains(i))
                        earningsForExcel = 0;
                    else if (int.TryParse(row[MEMBER_ID_COLUMN_NAME]?.ToString(), out int mNum)
                             && doublesConsolidatedEarnings.TryGetValue(mNum, out decimal combined))
                        earningsForExcel = (double)combined;
                }
                ws.Cell(excelRow, 9).Value = earningsForExcel.ToString("C0");
                ws.Cell(excelRow, 11).Value = currentPlace;
                ws.Cell(excelRow, 12).Value = row[MEMBER_ID_COLUMN_NAME]?.ToString();
                ws.Cell(excelRow, 15).FormulaA1 = $"=I{excelRow}-M{excelRow}-N{excelRow}+{sidePotValue}";
                // Write as a numeric value so Excel treats it as a number, not text.
                ws.Cell(excelRow, 8).Value = spVal;

                // Always explicitly set or clear the Membership$ (Column M) background so that
                // pre-existing orange from a previous export never bleeds into a current member's row.
                bool membershipNotCurrent = int.TryParse(Convert.ToString(row[MEMBER_ID_COLUMN_NAME]), out int memberNumber)
                    && isMembershipCurrentByMemberNumber.TryGetValue(memberNumber, out bool isCurrent)
                    && !isCurrent;
                ws.Cell(excelRow, 13).Style.Fill.BackgroundColor =
                    membershipNotCurrent ? XLColor.Orange : XLColor.NoColor;

                if (currentPlace >= 1 && currentPlace <= 3)
                {
                    excelRowsWithProgressivePot.Add(excelRow);
                    ws.Cell(excelRow + 1, 9).Value = spVal;
                    excelRow++;
                }

                i++;
                excelRow++;
            }

            if (tourny.Doubles && doublesSecondaryRowIndices?.Count > 0)
            {
                UpdateCheckSheetsForDoubles(workbook, resultIdxToExcelRow, doublesSecondaryRowIndices,
                    excelRowsWithProgressivePot, doublesPlaceLabelsForMemo, exportTable);
            }

            // Set total payout
            double money = 0;
            for (int k = 0; k < exportTable.Rows.Count; k++)
                money += Convert.ToDouble(exportTable.Rows[k][EARNINGS_COLUMN_NAME]);
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
                    // Preserve the consecutive-pair order written by BuildWinnersListDoubles.
                // Sorting by MemberNumber within the same place would interleave members
                // from different tied teams, breaking the [T1M1, T1M2, T2M1, T2M2, ...] layout.
                clientRequested = [.. winners.Where(m => m.PlaceStanding <= clientInput)];
                }
                else
                {
                    clientRequested = Calculations.TournamentCalculations.MakeTopMembersByPlacementList(winners, clientInput);
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

    private static bool TryExtractFirstAmount(string text, out decimal amount)
    {
        amount = 0m;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        Match match = Regex.Match(text.Replace(",", ""), @"-?\d+(\.\d+)?");
        if (!match.Success)
            return false;

        return decimal.TryParse(
            match.Value,
            System.Globalization.NumberStyles.Number,
            System.Globalization.CultureInfo.InvariantCulture,
            out amount);
    }

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

        List<string> lines = [.. clipboard
            .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))];

        int currentRow = 0;
        int placesProcessed = 0; // Track which place we're on (1st, 2nd, 3rd)
        bool expectProgressivePot = false; // Flag to track if next line should be progressive pot

        foreach (string line in lines)
        {
            // Stop processing if we've filled all rows in the grid
            if (currentRow >= dgvTournamentResults.RowCount)
                break;

            // Only assign progressive pot if we just assigned earnings and were expecting it
            if (expectProgressivePot && TryExtractFirstAmount(line, out decimal progressiveAmount))
            {
                // Subtracting one to ensure progressive pot goes to the correct bowler
                dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentRow - 1].Value = progressiveAmount;
                expectProgressivePot = false; // Reset flag after processing
            }
            else
            {
                TryExtractFirstAmount(line, out decimal earningsAmount);
                dgvTournamentResults[EARNINGS_COLUMN_NAME, currentRow].Value = earningsAmount;
                placesProcessed++;

                // After 1st, 2nd, and 3rd place, expect a progressive pot line next
                expectProgressivePot = placesProcessed <= 3;
                currentRow++;
            }
        }
    }

    /// <summary>
    /// Groups exportTable rows by MemberNumber and identifies members who placed more than once
    /// (possible in doubles when the same person bowled in multiple squads on different teams).
    /// Returns: combined earnings for each multi-placer, the secondary row indices to zero out,
    /// and the ordered place labels to use in the combined check's memo line.
    /// </summary>
    private (Dictionary<int, decimal> CombinedEarnings,
             HashSet<int> SecondaryRowIndices,
             Dictionary<int, List<string>> PlaceLabelsForMemo)
        BuildDoublesConsolidation(DataTable exportTable)
    {
        var seen         = new Dictionary<int, int>();           // memberNum → first row index
        var earningsMap  = new Dictionary<int, decimal>();
        var labelsMap    = new Dictionary<int, List<string>>();
        var secondary    = new HashSet<int>();

        for (int i = 0; i < exportTable.Rows.Count; i++)
        {
            var row = exportTable.Rows[i];
            if (!int.TryParse(row[MEMBER_ID_COLUMN_NAME]?.ToString(), out int memberNum) || memberNum <= 0)
                continue;

            decimal.TryParse(row[EARNINGS_COLUMN_NAME]?.ToString(), out decimal earn);
            string placeStr = row[PLACE_STANDING_COLUMN_NAME]?.ToString() ?? "";
            int.TryParse(placeStr.TrimEnd('T'), out int placeNum);
            string placeLabel = placeNum > 0 ? GetOrdinalWithTie(placeNum, placeStr.EndsWith("T")) : "";

            if (seen.TryAdd(memberNum, i))
            {
                earningsMap[memberNum] = earn;
                labelsMap[memberNum]   = [placeLabel];
            }
            else
            {
                earningsMap[memberNum] += earn;
                labelsMap[memberNum].Add(placeLabel);
                secondary.Add(i);
            }
        }

        // Only include members who placed more than once
        var combined = earningsMap
            .Where(kv => labelsMap[kv.Key].Count > 1)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        var memoLabels = labelsMap
            .Where(kv => kv.Value.Count > 1)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        return (combined, secondary, memoLabels);
    }

    /// <summary>
    /// For each non-Results worksheet, remaps formulas that reference Results rows belonging to
    /// secondary (duplicate) bowler placements to the correct unique bowler row, and updates
    /// progressive-pot earnings formulas accordingly.  Also writes the combined place-label memo
    /// into the B3 cell of any single-check sheet that belongs to a multi-placer.
    /// </summary>
    private void UpdateCheckSheetsForDoubles(
        XLWorkbook workbook,
        Dictionary<int, int> resultIdxToExcelRow,
        HashSet<int> secondaryResultIndices,
        HashSet<int> excelRowsWithProgressivePot,
        Dictionary<int, List<string>> multiPlacerPlaceLabels,
        DataTable exportTable)
    {
        // All bowler excel rows in ascending order (including secondary)
        var allBowlerRowsSorted = resultIdxToExcelRow.Values.OrderBy(r => r).ToList();

        // Secondary excel rows
        var secondaryExcelRows = secondaryResultIndices
            .Where(i => resultIdxToExcelRow.ContainsKey(i))
            .Select(i => resultIdxToExcelRow[i])
            .ToHashSet();

        // Unique sequence: bowler rows excluding secondary, in order
        var uniqueSequence = allBowlerRowsSorted.Where(r => !secondaryExcelRows.Contains(r)).ToList();

        // Map memberNum → primary excel row (first/best placement)
        var memberToPrimaryRow = new Dictionary<int, int>();
        for (int i = 0; i < exportTable.Rows.Count; i++)
        {
            if (!resultIdxToExcelRow.TryGetValue(i, out int exRow)) continue;
            if (secondaryResultIndices.Contains(i)) continue;
            if (!int.TryParse(exportTable.Rows[i][MEMBER_ID_COLUMN_NAME]?.ToString(), out int mn) || mn <= 0) continue;
            memberToPrimaryRow.TryAdd(mn, exRow);
        }

        // Build memo text keyed by primary excel row
        var memoByPrimaryRow = new Dictionary<int, string>();
        foreach (var (mn, labels) in multiPlacerPlaceLabels)
        {
            if (memberToPrimaryRow.TryGetValue(mn, out int exRow))
                memoByPrimaryRow[exRow] = string.Join(", ", labels);
        }

        var allBowlerRowsSet = new HashSet<int>(allBowlerRowsSorted);
        var rowNumPattern    = new Regex(@"Results!([A-Z]+)(\d+)", RegexOptions.IgnoreCase);

        foreach (var ws in workbook.Worksheets)
        {
            if (ws.Name.Equals("Results", StringComparison.OrdinalIgnoreCase)) continue;

            var formulaCells = ws.CellsUsed().Where(c => c.HasFormula).ToList();
            if (formulaCells.Count == 0) continue;

            // Collect all Results bowler-row numbers this sheet references
            var sheetRefRows = new HashSet<int>();
            foreach (var cell in formulaCells)
            {
                foreach (Match m in rowNumPattern.Matches(cell.FormulaA1))
                {
                    if (int.TryParse(m.Groups[2].Value, out int rowNum) && allBowlerRowsSet.Contains(rowNum))
                        sheetRefRows.Add(rowNum);
                }
            }
            if (sheetRefRows.Count == 0) continue;

            // Build old→new remap: position k in allBowlerRowsSorted → uniqueSequence[k]
            var rowRemap = new Dictionary<int, int>();
            foreach (int checkRow in sheetRefRows)
            {
                int pos = allBowlerRowsSorted.IndexOf(checkRow);
                if (pos < 0 || pos >= uniqueSequence.Count) continue;
                int newRow = uniqueSequence[pos];
                if (newRow != checkRow)
                    rowRemap[checkRow] = newRow;
            }

            // Apply formula remaps (process longer row numbers first to avoid partial matches)
            if (rowRemap.Count > 0)
            {
                foreach (var cell in formulaCells)
                {
                    string formula = cell.FormulaA1;
                    string updated = formula;
                    foreach (var (oldRow, newRow) in rowRemap.OrderByDescending(kv => kv.Key))
                    {
                        bool newHasPot = excelRowsWithProgressivePot.Contains(newRow);
                        updated = ApplyRowRemap(updated, oldRow, newRow, newHasPot);
                    }
                    if (!ReferenceEquals(updated, formula) && updated != formula)
                        cell.FormulaA1 = updated;
                }
            }

            // Update B3 memo for combined-earner checks (single-check-per-sheet: B3 is literally row 3 col B)
            foreach (var (primaryRow, memoText) in memoByPrimaryRow)
            {
                if (!sheetRefRows.Contains(primaryRow)) continue;
                foreach (var cell in formulaCells)
                {
                    string f = cell.FormulaA1.Trim();
                    if (f.Equals("B3", StringComparison.OrdinalIgnoreCase) ||
                        f.Equals("$B$3", StringComparison.OrdinalIgnoreCase))
                    {
                        cell.Value = memoText;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Rewrites all Results!{col}{oldRow} references in <paramref name="formula"/> to point to
    /// <paramref name="newRow"/>.  Column I is handled specially: if the new row has a
    /// progressive-pot row below it the earnings formula expands to I{n}+I{n+1}, otherwise
    /// it collapses to just I{n}.  A placeholder character prevents double-substitution.
    /// </summary>
    private static string ApplyRowRemap(string formula, int oldRow, int newRow, bool newHasPot)
    {
        const string ph = "\x01";

        // Neutralize column I (handle progressive-pot combo and single ref uniformly)
        string potCombo = $@"Results!I{oldRow}\s*\+\s*Results!I{oldRow + 1}";
        formula = Regex.Replace(formula, potCombo, ph, RegexOptions.IgnoreCase);
        formula = Regex.Replace(formula, $@"Results!I{oldRow}(?!\d)", ph, RegexOptions.IgnoreCase);

        // Restore I with correct progressive-pot handling
        string iValue = newHasPot
            ? $"Results!I{newRow}+Results!I{newRow + 1}"
            : $"Results!I{newRow}";
        formula = formula.Replace(ph, iValue);

        // Replace all remaining column references for oldRow → newRow (I already resolved above)
        formula = Regex.Replace(
            formula,
            $@"Results!([A-Z]+){oldRow}(?!\d)",
            m => m.Groups[1].Value.Equals("I", StringComparison.OrdinalIgnoreCase)
                ? m.Value
                : $"Results!{m.Groups[1].Value}{newRow}",
            RegexOptions.IgnoreCase);

        return formula;
    }
}