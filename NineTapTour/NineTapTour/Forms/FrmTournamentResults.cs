using NineTapTour.Core.Export;
using NineTapTour.Database;
using NineTapTour.Core.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Helpers;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.Services;

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

    private readonly ITournamentRepository tournamentRepository;
    private readonly ITournamentSession session;
    private readonly IMemberRepository memberRepository;
    private readonly IGameRepository gameRepository;
    private readonly IDoublesTeamRepository doublesTeamRepository;
    private readonly IDbContextFactory<NineTapDb> dbFactory;
    private readonly IWinnersService winnersService;
    private readonly ISeriesReportExcelExporter seriesReportExporter;

    readonly DataTable dt = new(); // Instantiate Data Table
    readonly NineTapDb db; // Get access to database
    readonly Tournament tourny; // Get Tournament
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
    public FrmTournamentResults(
        ITournamentRepository tournamentRepository,
        IMemberRepository memberRepository,
        IGameRepository gameRepository,
        IDoublesTeamRepository doublesTeamRepository,
        IDbContextFactory<NineTapDb> dbFactory,
        ITournamentSession session,
        IWinnersService winnersService,
        ISeriesReportExcelExporter seriesReportExporter)
    {
        this.tournamentRepository = tournamentRepository;
        this.session = session;
        this.memberRepository = memberRepository;
        this.gameRepository = gameRepository;
        this.doublesTeamRepository = doublesTeamRepository;
        this.dbFactory = dbFactory;
        this.winnersService = winnersService;
        this.seriesReportExporter = seriesReportExporter;
        db = dbFactory.CreateDbContext();
        tourny = session.SelectedTournament;

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
            WinnersListResult winnersResult = winnersService.BuildWinnersList(
                new WinnersListRequest(tourny.Id, tourny.Doubles, tourny.ThreeOutOf4));
            winners = winnersResult.Winners;
            totalTournamentEntries = winnersResult.TotalEntries;
            compEntries = winnersResult.CompEntries;
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
            session.MoneyEarnings = twoDayWinnings;

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
        session.MoneyEarnings = Winnings;

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

            g.PlaceStanding = WinnersService.ParsePlaceStanding(dgvTournamentResults[PLACE_STANDING_COLUMN_NAME, currentIndex].Value);
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
        if (session.MoneyEarnings != null)
        {
            MonEarnCount = session.MoneyEarnings.Count;
        }

        // Create rows and populate with each member's data for each row
        for (int wc = 0; wc < clientRequested.Count; wc++)
        {
            DataRow newRow = dt.NewRow();
            if (MonEarnCount > 0)
            {
                if (wc < MonEarnCount)
                {
                    newRow[EARNINGS_COLUMN_NAME] = session.MoneyEarnings[wc];
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
                newRow[EARNINGS_COLUMN_NAME] = session.MoneyEarnings[tr];
            }
            else
            {
                newRow[EARNINGS_COLUMN_NAME] = earnings;
            }
            // For doubles, consecutive filler rows share a team place (2 rows per team slot)
            int fillerPlace = WinnersService.ComputeFillerPlace(tourny.Doubles, clientRequested.Count, tr);
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
        List<string> placeValues = [];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            placeValues.Add(dt.Rows[i][PLACE_STANDING_COLUMN_NAME]?.ToString());
        }
        List<string> markedPlaces = WinnersService.ApplyTieMarkers(placeValues);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (markedPlaces[i] != placeValues[i])
                dt.Rows[i][PLACE_STANDING_COLUMN_NAME] = markedPlaces[i];
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
    /// Creates the Team View button and grid overlay for doubles tournaments.
    /// </summary>
    private void InitTeamViewControls()
    {
        if (!tourny.Doubles) return;

        btnTeamView = new Button
        {
            Text    = "Team View",
            Size    = new Size(105, 23),
            Location = new Point(tbClientInputCount.Right + 8, tbClientInputCount.Top)
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

        // ComputeDoublesWinnersRows always writes `winners` in consecutive pairs: [T1M1, T1M2, T2M1, T2M2, ...]
        // BuildTeamPairings steps through by 2 to reconstruct team pairings — avoids re-querying
        // the teams table and any member-number matching fragility (e.g. the same member in multiple squads).
        List<DoublesTeamPairing> teamPairs = WinnersService.BuildTeamPairings(winners, clientInput);

        Color[] teamColors = [SystemColors.Window, Color.AliceBlue];

        for (int i = 0; i < teamPairs.Count; i++)
        {
            var (m1, m2, place, isTie) = teamPairs[i];
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

        // ComputeDoublesWinnersRows writes pairs consecutively: [T1M1, T1M2, T2M1, T2M2, ...]
        List<DoublesTeamPairing> teamPairs = WinnersService.BuildTeamPairings(winners, clientInput);

        foreach (var (m1, m2, place, _) in teamPairs)
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

        string placeGroupLabel = WinnersService.Build2DayPlaceGroupLabel(startPlace, endPlace);

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
        TwoDayAutoFillResult fill = winnersService.AutoFillTwoDayMember(memberNumber, tourny.Id);

        if (fill.Status == TwoDayAutoFillStatus.MemberNotFound)
        {
            MessageBox.Show($"Member number {memberNumber} not found.");
            dataRow[MEMBER_NUMBER_COLUMN_NAME] = DBNull.Value;
            return;
        }

        if (fill.Status == TwoDayAutoFillStatus.GameNotFound)
        {
            MessageBox.Show($"No game entry found for member {memberNumber} in this 2-day tournament.\n" +
                            $"Make sure scores have been entered in Member Scores first.");
            dataRow[MEMBER_NUMBER_COLUMN_NAME] = DBNull.Value;
            return;
        }

        dataRow[FULLNAME_COLUMN_NAME]    = fill.FullName;
        dataRow[HANDICAP_COLUMN_NAME]    = fill.HandicapDisplay;
        dataRow[TOTAL_SCORE_COLUMN_NAME] = fill.TotalScore;
        dataRow[MEMBER_ID_COLUMN_NAME]   = fill.MemberNumber;
        dataRow[GAME_ID_COLUMN_NAME]     = fill.GameId;
    }

    /// <summary>
    /// Loads any previously saved 2-day results from the database into <see cref="_dt2Day"/>.
    /// Called once during form load so the director can close and reopen the form between rounds.
    /// </summary>
    private void LoadExisting2DayData()
    {
        var saved = tournamentRepository.GetWinnerListMemberData(tourny.Id)
            .Where(b => b.PlaceStanding > 0)
            .OrderBy(b => b.PlaceStanding)
            .ToList();

        if (saved.Count == 0) return;

        foreach (var b in saved)
        {
            DataRow row = _dt2Day.NewRow();
            int placeStart = b.PlaceStanding ?? 0;
            string placeLabel = string.IsNullOrWhiteSpace(b.PlaceStandingLabel)
                ? (placeStart > 0 ? WinnersService.GetOrdinalWithTie(placeStart, false) : "")
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

    private static bool TryGet2DayPlaceGroup(DataRow row, out int placeStart, out string placeLabel)
    {
        placeStart = 0;
        placeLabel = "";

        string explicitLabel = row.Table.Columns.Contains(PLACE_GROUP_LABEL_COLUMN_NAME)
            ? Convert.ToString(row[PLACE_GROUP_LABEL_COLUMN_NAME])?.Trim() ?? ""
            : "";
        string displayValue = Convert.ToString(row[PLACE_STANDING_COLUMN_NAME])?.Trim() ?? "";

        string sourceText = !string.IsNullOrWhiteSpace(explicitLabel) ? explicitLabel : displayValue;
        if (!WinnersService.TryParsePlaceStartFromText(sourceText, out placeStart))
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
        // Pick the template first so earnings and progressive pot can be read
        // into the DGV before the DB save below.
        string saveFile;
        using OpenFileDialog openDialog = new()
        {
            Title  = "Select Existing Results File",
            Filter = FileHelper.GetExcelFilterStringForFileDialogs(),
        };
        if (openDialog.ShowDialog() != DialogResult.OK)
            return;
        saveFile = openDialog.FileName;

        // Preserve the template's extension (.xlsm keeps macros, .xlsx is default).
        string templateExt = Path.GetExtension(saveFile).ToLowerInvariant();
        string outputExt   = templateExt == ".xlsm" ? ".xlsm" : ".xlsx";

        string tourneyDate    = tourny.Date.ToString("MM/dd/yyyy");
        string tournamentDate = tourneyDate.Replace("/", "-");
        string fileName       = tourny.Location + " " + tourny.Event + " " + tournamentDate + outputExt;

        // For standard (non-2-day) tournaments, read earnings and progressive pot from
        // the pre-filled template into the DGV so the DB save below records the correct values.
        if (!tourny.IsTwoDay)
        {
            try
            {
                List<TemplateEarningsRow> templateRows =
                    seriesReportExporter.ReadEarningsAndPots(saveFile, dgvTournamentResults.RowCount);
                for (int idx = 0; idx < templateRows.Count; idx++)
                {
                    dgvTournamentResults[EARNINGS_COLUMN_NAME, idx].Value = templateRows[idx].Earnings;
                    dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, idx].Value = templateRows[idx].ProgressivePot;
                }
            }
            catch { /* If the template cannot be read, proceed with DGV values as-is. */ }
        }

        if (tourny.IsTwoDay)
        {
            // Save 2-day results to DB via the DGV (which is bound to _dt2Day)
            for (int i = 0; i < dgvTournamentResults.RowCount; i++)
            {
                object gameIdCell = dgvTournamentResults[GAME_ID_COLUMN_NAME, i].Value;
                if (gameIdCell == null || gameIdCell == DBNull.Value) continue;
                if (!int.TryParse(gameIdCell.ToString(), out int gameId) || gameId <= 0) continue;

                Game g = gameRepository.GetGame(gameId);
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
                Game g = gameRepository.GetGame(gameId);

                g.PlaceStanding = WinnersService.ParsePlaceStanding(dgvTournamentResults[PLACE_STANDING_COLUMN_NAME, currentIndex].Value);
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

        // Select the source table: 2-day grid (sorted by Place), doubles Team View, or the standard winners table.
        DataTable exportTable = tourny.IsTwoDay
            ? _dt2Day.DefaultView.ToTable()
            : (tourny.Doubles && _inTeamView)
                ? BuildTeamViewExportTable()
                : dt;

        // Flatten the export table into POCO rows for the headless exporter.
        bool hasGroupLabelColumn = exportTable.Columns.Contains(PLACE_GROUP_LABEL_COLUMN_NAME);
        List<SeriesReportRow> exportRows = [];
        foreach (DataRow row in exportTable.Rows)
        {
            exportRows.Add(new SeriesReportRow(
                row[PLACE_STANDING_COLUMN_NAME]?.ToString(),
                hasGroupLabelColumn ? row[PLACE_GROUP_LABEL_COLUMN_NAME]?.ToString() : null,
                row[FULLNAME_COLUMN_NAME]?.ToString(),
                row[HANDICAP_COLUMN_NAME]?.ToString(),
                row[TOTAL_SCORE_COLUMN_NAME]?.ToString(),
                row[MEMBER_ID_COLUMN_NAME]?.ToString(),
                row[EARNINGS_COLUMN_NAME]?.ToString()));
        }

        // Preload membership-current status for rows that carry a numeric member number.
        var memberNumbers = exportRows
            .Select(r => r.MemberNumberText)
            .Where(s => int.TryParse(s, out _))
            .Select(int.Parse)
            .Distinct()
            .ToList();
        Dictionary<int, bool> isMembershipCurrentByMemberNumber =
            winnersService.GetMembershipCurrentByMemberNumber(memberNumbers);

        try
        {
            // Save dialog
            SaveFileDialog savefile = new()
            {
                Filter = FileHelper.GetExcelFilterStringForFileDialogs(),
                FileName = fileName
            };
            DialogResult result = savefile.ShowDialog();
            if (result == DialogResult.OK)
            {
                var request = new SeriesReportExportRequest(
                    tourny.Location,
                    tourny.Event,
                    tourny.Date,
                    tourny.IsTwoDay,
                    tourny.Doubles && !_inTeamView && !tourny.IsTwoDay,
                    exportRows,
                    isMembershipCurrentByMemberNumber);
                seriesReportExporter.Export(saveFile, savefile.FileName, request);
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
                // the same PlaceStanding (set by WinnersService.ComputeDoublesWinnersRows), so we filter directly
                // instead of calling MakeTopMembersByPlacementList, which re-ranks by TotalScore
                // and would place tied pairs at 1,1,3,3,5,5... causing only half as many teams
                // to pass a simple <= clientInput threshold.
                if (tourny.Doubles)
                {
                    // Preserve the consecutive-pair order written by ComputeDoublesWinnersRows.
                // Sorting by MemberNumber within the same place would interleave members
                // from different tied teams, breaking the [T1M1, T1M2, T2M1, T2M2, ...] layout.
                clientRequested = [.. winners.Where(m => m.PlaceStanding <= clientInput)];
                }
                else
                {
                    clientRequested = Core.Calculations.TournamentCalculations.MakeTopMembersByPlacementList(winners, clientInput);
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
}
