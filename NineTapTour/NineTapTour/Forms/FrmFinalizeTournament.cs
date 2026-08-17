using CalcService = NineTapTour.Core.Calculations.TournamentCalculations;
using NineTapTour.Database;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NineTapTour.Helpers;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace NineTapTour.Forms;

public partial class FrmFinalizeTournament : Form
{
    private readonly Tournament selectedTournament;
    private readonly ITournamentRepository tournamentRepository;
    private readonly IMemberRepository memberRepository;
    private readonly IGameRepository gameRepository;
    private readonly IFinalizeTempRepository finalizeTempRepository;
    private readonly IPlayerHistoryRepository playerHistoryRepository;
    private readonly IDoublesTeamRepository doublesTeamRepository;
    private readonly IDbContextFactory<NineTapDb> dbFactory;
    private readonly IFinalizeCalculationService finalizeCalculationService;

    // Top-level controls — access these to adjust later
    private Panel pnlToolbar;
    private Label lblFinalizedBanner;
    private Button btnFinalizeTournament;
    private Button btnUndoFinalize;
    private SplitContainer splitMain;
    private Panel pnlPlayerInfo;
    private Label lblPlayerInfo;
    private DataGridView dgvTournament;
    private DataGridView dgvDetail;

    private List<WinnerListMemberViewModel> _currentTournamentBowlers;
    private int _displayedDetailMemberNumber = -1;
    private readonly HashSet<int> _invalidRowIndices = [];

    private static readonly string[] GameScoreColumns = ["colGame1", "colGame2", "colGame3", "colGame4"];

    // GameIds belonging to members who cashed, kept in sync by UpdateNewBonusPreview.
    // Drives the detail grid's bonus highlight and entry ordering.
    private readonly HashSet<int> _cashingGameIds = [];

    // Best place standing per member number, computed in LoadTournamentGrid and consumed
    // by LoadDetailGrid to populate the Place column for current-tournament entries.
    private Dictionary<int, int> _bestStandingByMember = [];
    // Game IDs whose entry received a non-zero place standing (the "primary" entry).
    private readonly HashSet<int> _placedGameIds = [];
    // Per member: (Scratch, Games) from the last (30 − currentEntryCount) finalized
    // entries in prior tournaments. Combined with live grid values in UpdateAll30AvgForMember.
    private readonly Dictionary<int, (int Scratch, int Games)> _history30ByMember = [];

    // Per member: handicap derived from the member's most recent finalized tournament
    // (excluding the current tournament). Used as the source for colHdcp, overriding the
    // potentially-stale per-entry Game.Handicap snapshot. The bonus half of the tuple is
    // not used for the grid — carry-in bonus comes from Member.Bonus, matching
    // WinnersService so the finalize grid and FrmTournamentResults agree.
    private readonly Dictionary<int, (int Hdcp, int Bonus)> _prevTournHBByMember = [];

    // Inputs the New Bonus preview needs beyond the live Bonus and Earnings cells.
    private record BonusContext(
        int Placing, int HistoricalEntries, int CurrentEntries, int SidePot,
        bool IsDoubles, bool DoublesCashing);
    private readonly Dictionary<int, BonusContext> _bonusContextByGameId = [];

    // Lowest placement that cashes, from GetQtyOfMembersThatCanPlace. Rounds down to 0
    // in tournaments with fewer than five entries, which is why ComputeBonusPreview also
    // treats any member with place money as a casher.
    private int _cashLine;

    // True when the tournament was already finalized before the form opened, or once
    // FinalizeAllGames completes successfully this session.
    private bool _isFinalized = false;

    // Team View overlay (doubles only) — read-only summary with one row per team.
    private DataGridView dgvTeamView;
    private Button btnTeamView;
    private bool _inTeamView = false;

    // Snapshot of each game's editable fields taken at form-open time.
    // "Undo Changes" writes these values back to the DB and reloads the grid,
    // reverting any director edits made during the current session.
    private record GameSnapshot(
        int? Game1, int? Game2, int? Game3, int? Game4,
        bool? UseGame1, bool? UseGame2, bool? UseGame3, bool? UseGame4,
        int AdjustedAvg, bool KeepAdjustedAvg,
        int? Handicap, int? Bonus, decimal? MoneyWon, string Notes);
    private readonly Dictionary<int, GameSnapshot> _gameSnapshot = [];

    /// <summary>
    /// Tag type for individual doubles rows: carries this member's game ID and the
    /// partner's game ID so the partner row can be located for combined HDCP recalculation.
    /// </summary>
    private record DoubleMemberRowTag(int MyGameId, int PartnerGameId);

    public FrmFinalizeTournament(
        Tournament selectedTournament,
        ITournamentRepository tournamentRepository,
        IMemberRepository memberRepository,
        IGameRepository gameRepository,
        IFinalizeTempRepository finalizeTempRepository,
        IPlayerHistoryRepository playerHistoryRepository,
        IDoublesTeamRepository doublesTeamRepository,
        IDbContextFactory<NineTapDb> dbFactory,
        IFinalizeCalculationService finalizeCalculationService)
    {
        this.selectedTournament = selectedTournament;
        this.tournamentRepository = tournamentRepository;
        this.memberRepository = memberRepository;
        this.gameRepository = gameRepository;
        this.finalizeTempRepository = finalizeTempRepository;
        this.playerHistoryRepository = playerHistoryRepository;
        this.doublesTeamRepository = doublesTeamRepository;
        this.dbFactory = dbFactory;
        this.finalizeCalculationService = finalizeCalculationService;

        InitializeComponent();
    }

    private void FrmFinalizeTournament_Load(object sender, EventArgs e)
    {
        // Set before the grids load — LoadDetailGrid skips the live preview rows once
        // this tournament's entries are part of the member's finalized history.
        _isFinalized = selectedTournament.IsTournamentFinalized;

        BuildGrids();
        LoadTournamentGrid();
        dgvTournament.SelectionChanged += DgvTournament_SelectionChanged;
        DgvTournament_SelectionChanged(dgvTournament, EventArgs.Empty);
        FormClosing += FrmFinalizeTournament_FormClosing;

        if (_isFinalized) ApplyFinalizedState();

        // Snapshot all editable game fields immediately after load so "Undo Changes"
        // can restore the original DB state for the rest of the session.
        using var db = dbFactory.CreateDbContext();
        // Collect all game IDs from grid rows (handles both singles int tags and DoublesRowTag)
        var gameIds = new HashSet<int>();
        foreach (DataGridViewRow row in dgvTournament.Rows)
        {
            if (row.Tag is int gId) gameIds.Add(gId);
            else if (row.Tag is DoubleMemberRowTag dmt) gameIds.Add(dmt.MyGameId);
        }
        foreach (var g in db.Games.Where(g => gameIds.Contains(g.Id)))
            _gameSnapshot[g.Id] = new GameSnapshot(
                g.Game1, g.Game2, g.Game3, g.Game4,
                g.UseGame1, g.UseGame2, g.UseGame3, g.UseGame4,
                g.AdjustedAvg, g.KeepAdjustedAvg,
                g.Handicap, g.Bonus, g.MoneyWon, g.Notes);
    }

    private void FrmFinalizeTournament_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_isFinalized) return;

        var result = MessageBox.Show(
            "The tournament has not been finalized yet. Member records (Average, Handicap, Bonus) will not be updated until you finalize.\n\nClose anyway?",
            "Tournament Not Finalized",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (result == DialogResult.No)
            e.Cancel = true;
    }

    private void BuildGrids()
    {
        SuspendLayout();

        // --- Toolbar panel ---
        pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 40 };

        btnFinalizeTournament = new Button
        {
            Text = "Finalize Tournament",
            Size = new Size(160, 26)
        };
        btnFinalizeTournament.Click += BtnFinalizeTournament_Click;

        btnUndoFinalize = new Button
        {
            Text    = "Undo Changes",
            Size    = new Size(160, 26),
            Enabled = true
        };
        btnUndoFinalize.Click += BtnUndoFinalize_Click;

        btnTeamView = new Button
        {
            Text    = "Team View",
            Size    = new Size(100, 26),
            Visible = false   // shown only for doubles after the grid is loaded
        };
        btnTeamView.Click += BtnTeamView_Click;

        pnlToolbar.Controls.AddRange([btnFinalizeTournament, btnUndoFinalize, btnTeamView]);
        pnlToolbar.Resize += (s, _) => PinToolbarButtons();

        // --- "Already finalized" banner (shown by ApplyFinalizedState) ---
        lblFinalizedBanner = new Label
        {
            Dock      = DockStyle.Top,
            Height    = 36,
            Font      = new Font("Arial", 16, FontStyle.Bold),
            ForeColor = Color.Red,
            TextAlign = ContentAlignment.MiddleCenter,
            Text      = "THIS TOURNAMENT HAS ALREADY BEEN FINALIZED",
            Visible   = false
        };

        // --- SplitContainer (top grid / bottom grid) ---
        splitMain = new SplitContainer
        {
            Dock        = DockStyle.Fill,
            Orientation = Orientation.Horizontal
        };

        // Top grid
        dgvTournament = CreateTournamentGrid();
        dgvTournament.DoubleBuffered(true);
        dgvTournament.CurrentCellDirtyStateChanged += DgvTournament_CurrentCellDirtyStateChanged;
        dgvTournament.CellValueChanged             += DgvTournament_CellValueChanged;
        dgvTournament.CellEndEdit                  += DgvTournament_CellEndEdit;
        dgvTournament.RowPostPaint                 += DgvTournament_RowPostPaint;
        splitMain.Panel1.Controls.Add(dgvTournament);

        // Team View overlay — sits in the same panel as dgvTournament, hidden by default
        dgvTeamView = new DataGridView
        {
            Dock                        = DockStyle.Fill,
            AllowUserToAddRows          = false,
            AllowUserToDeleteRows       = false,
            ReadOnly                    = true,
            Visible                     = false,
            AutoSizeColumnsMode         = DataGridViewAutoSizeColumnsMode.None,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            ColumnHeadersHeight         = 52,
            SelectionMode               = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersWidth             = 25
        };
        dgvTeamView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
        dgvTeamView.DoubleBuffered(true);
        splitMain.Panel1.Controls.Add(dgvTeamView);

        // Player info label at the top of the lower panel
        pnlPlayerInfo = new Panel { Dock = DockStyle.Top, Height = 36 };
        lblPlayerInfo = new Label
        {
            Dock      = DockStyle.Fill,
            Font      = new Font("Arial", 14, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(5, 0, 0, 0),
            Text      = string.Empty
        };
        pnlPlayerInfo.Controls.Add(lblPlayerInfo);

        // Bottom grid — add Fill first so Top-docked pnlPlayerInfo is processed first
        dgvDetail = CreateDetailGrid();
        dgvDetail.DoubleBuffered(true);
        splitMain.Panel2.Controls.Add(dgvDetail);
        splitMain.Panel2.Controls.Add(pnlPlayerInfo);

        // Add to form — Fill first, then the Top-docked controls in bottom-to-top order
        // (the last one added is laid out first, so the banner sits above the toolbar)
        Controls.Add(splitMain);
        Controls.Add(pnlToolbar);
        Controls.Add(lblFinalizedBanner);

        ResumeLayout(true);

        // Set after layout so pnlToolbar already has its docked width
        PinToolbarButtons();

        splitMain.SplitterDistance = splitMain.Height / 2;
    }

    /// <summary>
    /// Puts the form into read-only "already finalized" mode: shows the red banner,
    /// disables the Finalize and Undo Changes buttons, and locks the tournament grid so
    /// results that have already been written to the member records cannot be edited.
    /// </summary>
    private void ApplyFinalizedState()
    {
        lblFinalizedBanner.Visible    = true;
        btnFinalizeTournament.Enabled = false;
        btnUndoFinalize.Enabled       = false;
        dgvTournament.ReadOnly        = true;
    }

    private void PinToolbarButtons()
    {
        int x = pnlToolbar.ClientSize.Width - btnFinalizeTournament.Width - 12;
        btnFinalizeTournament.Location = new Point(x, 7);
        btnUndoFinalize.Location       = new Point(x - btnUndoFinalize.Width - 8, 7);
        btnTeamView?.Location          = new Point(x - btnUndoFinalize.Width - btnTeamView.Width - 16, 7);
    }

    private static DataGridView CreateTournamentGrid()
    {
        var dgv = new DataGridView
        {
            Dock                          = DockStyle.Fill,
            AllowUserToAddRows            = false,
            AllowUserToDeleteRows         = false,
            AutoSizeColumnsMode           = DataGridViewAutoSizeColumnsMode.None,
            ColumnHeadersHeightSizeMode   = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            ColumnHeadersHeight           = 52,
            SelectionMode                 = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersWidth               = 25
        };
        dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

        dgv.Columns.AddRange(
            new DataGridViewTextBoxColumn  { Name = "colStanding",     HeaderText = "Standing",        Width = 60,  ReadOnly = true },
            new DataGridViewTextBoxColumn  { Name = "colMemberNumber", HeaderText = "Member\nNumber",  Width = 65,  ReadOnly = true },
            new DataGridViewTextBoxColumn  { Name = "colName",         HeaderText = "Name",            Width = 150, ReadOnly = true },
            new DataGridViewTextBoxColumn  { Name = "colGame1",        HeaderText = "Game\n1",         Width = 50  },
            new DataGridViewCheckBoxColumn { Name = "colGame1Check",   HeaderText = "",                Width = 25  },
            new DataGridViewTextBoxColumn  { Name = "colGame2",        HeaderText = "Game\n2",         Width = 50  },
            new DataGridViewCheckBoxColumn { Name = "colGame2Check",   HeaderText = "",                Width = 25  },
            new DataGridViewTextBoxColumn  { Name = "colGame3",        HeaderText = "Game\n3",         Width = 50  },
            new DataGridViewCheckBoxColumn { Name = "colGame3Check",   HeaderText = "",                Width = 25  },
            new DataGridViewTextBoxColumn  { Name = "colGame4",        HeaderText = "Game\n4",         Width = 50  },
            new DataGridViewCheckBoxColumn { Name = "colGame4Check",   HeaderText = "",                Width = 25  },
            new DataGridViewTextBoxColumn  { Name = "colScratchTotal", HeaderText = "Scratch\nTotal",  Width = 55,  ReadOnly = true },
            new DataGridViewTextBoxColumn  { Name = "colHdcpTotal",    HeaderText = "HDCP\nTotal",     Width = 55,  ReadOnly = true },
            new DataGridViewTextBoxColumn  { Name = "colEntryAvg",     HeaderText = "Entry\nAVG",      Width = 50,  ReadOnly = true },
            new DataGridViewTextBoxColumn  { Name = "col30EntryAvg",   HeaderText = "30\nEntry\nAVG",  Width = 55,  ReadOnly = true },
            new DataGridViewTextBoxColumn  { Name = "colAdjAvg",       HeaderText = "ADJ\nAVG",        Width = 50  },
            new DataGridViewCheckBoxColumn { Name = "colDirCheck",     HeaderText = "Director\nCheck", Width = 58  },
            new DataGridViewTextBoxColumn  { Name = "colSquad",        HeaderText = "Squad",           Width = 45,  ReadOnly = true },
            new DataGridViewTextBoxColumn  { Name = "colHdcp",         HeaderText = "HDCP",            Width = 45,  ReadOnly = true },
            new DataGridViewTextBoxColumn  { Name = "colNewHdcp",      HeaderText = "New\nHDCP",       Width = 50,  ReadOnly = true },
            new DataGridViewTextBoxColumn  { Name = "colBonus",        HeaderText = "Bonus",           Width = 45  },
            new DataGridViewTextBoxColumn  { Name = "colNewBonus",     HeaderText = "New\nBonus",      Width = 50,  ReadOnly = true },
            new DataGridViewTextBoxColumn  { Name = "colEarnings",     HeaderText = "Earnings",        Width = 60  },
            new DataGridViewTextBoxColumn  { Name = "colNotes",        HeaderText = "Notes",           AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill }
        );

        return dgv;
    }

    /// <summary>
    /// The carry-in bonus an entry is scored with — the value shown in the Bonus column.
    /// Before finalization this is the member's running Member.Bonus, the same source
    /// WinnersService uses, so this grid agrees with FrmTournamentResults. Once finalized
    /// the entry's own Game.Bonus is authoritative, because Member.Bonus has already been
    /// advanced to the post-tournament value.
    /// </summary>
    private int ResolveCarryInBonus(WinnerListMemberViewModel entry) =>
        _isFinalized ? Convert.ToInt32(entry.Bonus) : entry.MemberBonus;

    private void LoadTournamentGrid()
    {
        _cashingGameIds.Clear();
        _placedGameIds.Clear();
        _bonusContextByGameId.Clear();
        _currentTournamentBowlers = tournamentRepository.GetWinnerListMemberData(selectedTournament.Id);

        // Doubles tournaments: delegate to the doubles grid builder
        if (selectedTournament.Doubles)
        {
            LoadTournamentGridDoubles();
            return;
        }

        // Pre-compute member numbers and previous-tournament H/B BEFORE BuildExcelMemberList
        // so that place standings (and therefore isCashing) are based on the correct H/B.
        var memberNumbersInTournament = _currentTournamentBowlers
            .Select(b => b.MemberNumber).Distinct().ToHashSet();

        _prevTournHBByMember.Clear();
        using (var dbPrev = dbFactory.CreateDbContext())
        {
            var latestApprovedEntries = dbPrev.Participants
                .Where(p => memberNumbersInTournament.Contains(p.Member.Number)
                         && p.Tournament.Id != selectedTournament.Id
                         && p.Game.IsFinalized
                         && p.Game.AdjustedAvg > 0)
                .GroupBy(p => p.Member.Number)
                .Select(g => new
                {
                    MemberNumber = g.Key,
                    LatestDate = g.Max(p => p.Tournament.Date)
                })
                .ToList();

            foreach (var item in latestApprovedEntries)
            {
                List<PreviousEntrySnapshot> prevEntries = dbPrev.Participants
                    .Where(p => p.Member.Number == item.MemberNumber
                             && p.Tournament.Id != selectedTournament.Id
                             && p.Game.IsFinalized
                             && p.Tournament.Date == item.LatestDate)
                    .Select(p => new PreviousEntrySnapshot(p.Game.AdjustedAvg, p.Game.Bonus ?? 0, p.Game.MoneyWon ?? 0))
                    .ToList();

                if (prevEntries.Count == 0) continue;

                _prevTournHBByMember[item.MemberNumber] =
                    finalizeCalculationService.ComputePreviousHandicapAndBonus(prevEntries);
            }
        }

        List<ExcelMember> members = BuildExcelMemberList(_currentTournamentBowlers);

        // 2-day championships: use stored place standings (written by FrmTournamentResults)
        // instead of recalculating from scores. _cashingGameIds is seeded from MoneyWon > 0;
        // _cashLine is unused (isCashing resolves to false, suppressing bonus deduction preview).
        if (selectedTournament.IsTwoDay)
        {
            var storedPlaceByGameId = _currentTournamentBowlers.ToDictionary(b => b.GameId, b => b.PlaceStanding ?? 0);
            foreach (var m in members)
                m.PlaceStanding = storedPlaceByGameId.TryGetValue(m.GameId, out int sp) ? sp : 0;

            _bestStandingByMember = _currentTournamentBowlers
                .GroupBy(b => b.MemberNumber)
                .ToDictionary(
                    g => g.Key,
                    g => g.Where(b => (b.PlaceStanding ?? 0) > 0)
                          .Select(b => b.PlaceStanding!.Value)
                          .DefaultIfEmpty(int.MaxValue)
                          .Min());

            foreach (var b in _currentTournamentBowlers.Where(b => (b.MoneyWon ?? 0) > 0))
                _cashingGameIds.Add(b.GameId);

            _cashLine = 0; // unused; isCashing = false for all rows (no auto bonus deduction)
        }
        else
        {
            // Compute the correct placement for each member using only their best entry
            List<ExcelMember> deduped = CalcService.CalculatePlaceStandings(members, removeDuplicates: true);
            _bestStandingByMember = deduped.ToDictionary(m => m.MemberNumber, m => m.PlaceStanding);

            // Determine the cash line so bonus deductions can be previewed in the grid
            int totalEntries = _currentTournamentBowlers.Count;
            int compEntries  = _currentTournamentBowlers.Count(b => b.IsComp);
            _cashLine        = CalcService.GetQtyOfMembersThatCanPlace(totalEntries, compEntries);
        }

        // Count current-tournament entries per member and finalized historical entries per member.
        // Used to detect members reaching their 3rd total entry in this tournament.
        var currentCountByMember = _currentTournamentBowlers
            .GroupBy(b => b.MemberNumber)
            .ToDictionary(g => g.Key, g => g.Count());

        var historicalCountByMember = new Dictionary<int, int>();
        using (var db = dbFactory.CreateDbContext())
        {
            var historicalCounts = db.Participants
                .Where(p => p.Tournament.Id != selectedTournament.Id
                         && p.Game.IsFinalized)
                .GroupBy(p => p.Member.Number)
                .Select(g => new { MemberNumber = g.Key, Count = g.Count() })
                .ToList();
            foreach (var item in historicalCounts)
                historicalCountByMember[item.MemberNumber] = item.Count;
        }

        // Precompute per-member historical scratch/games for the 30-entry AVG preview.
        // We load the last (30 − currentCount) finalized entries per member so that
        // adding in live current-entry values gives an up-to-date running average.
        _history30ByMember.Clear();
        using (var dbH = dbFactory.CreateDbContext())
        {
            var allHistory = dbH.Participants
                .Where(p => p.Tournament.Id != selectedTournament.Id
                         && p.Game.IsFinalized
                         && memberNumbersInTournament.Contains(p.Member.Number))
                .OrderByDescending(p => p.Tournament.Date)
                .ThenByDescending(p => p.Game.Id)
                .Select(p => new
                {
                    MemberNumber = p.Member.Number,
                    G1 = p.Game.Game1, G2 = p.Game.Game2,
                    G3 = p.Game.Game3, G4 = p.Game.Game4,
                    U1 = p.Game.UseGame1, U2 = p.Game.UseGame2,
                    U3 = p.Game.UseGame3, U4 = p.Game.UseGame4
                })
                .ToList();

            foreach (var grp in allHistory.GroupBy(x => x.MemberNumber))
            {
                int memberNum = grp.Key;
                int currCount = currentCountByMember.TryGetValue(memberNum, out int cc) ? cc : 0;
                (int totalScratch, int totalGames) = finalizeCalculationService.Compute30EntryHistory(
                    grp.Select(g => new HistoryGameEntry(g.G1, g.G2, g.G3, g.G4, g.U1, g.U2, g.U3, g.U4)),
                    currCount);
                if (totalGames > 0)
                    _history30ByMember[memberNum] = (totalScratch, totalGames);
            }
        }

        // Sort all entries by score descending so the best entry per member is encountered first
        members.Sort((x, y) => y.TotalScore.CompareTo(x.TotalScore));

        if (!selectedTournament.IsTwoDay)
        {
            // First occurrence of each member gets the standing; every duplicate entry gets 0
            var seenMembers = new HashSet<int>();
            foreach (ExcelMember m in members)
            {
                m.PlaceStanding = seenMembers.Add(m.MemberNumber)
                    ? _bestStandingByMember[m.MemberNumber]
                    : 0;
            }
        }

        foreach (ExcelMember m in members)
            if (m.PlaceStanding > 0) _placedGameIds.Add(m.GameId);

        // Display order:
        // - 2-day: grouped by round group (PlaceStanding), sorted by score within each group
        // - Non-2-day: groups ordered by best place standing, all entries for the
        //   same member clustered together, placed entry first within each group
        if (selectedTournament.IsTwoDay)
        {
            // 2-day: sort by round group's PlaceStanding, then by score (high to low) within each group
            members.Sort((x, y) =>
            {
                // Primary: order by round group (PlaceStanding)
                if (x.PlaceStanding != y.PlaceStanding)
                    return x.PlaceStanding.CompareTo(y.PlaceStanding);

                // Within same round group: higher scores first
                return y.TotalScore.CompareTo(x.TotalScore);
            });
        }
        else
        {
            // Non-2-day: groups ordered by best place standing, all entries for the
            // same member clustered together, placed entry first within each group
            members.Sort((x, y) =>
            {
                // Primary: order groups by the member's best place standing
                int xBest = _bestStandingByMember[x.MemberNumber];
                int yBest = _bestStandingByMember[y.MemberNumber];
                if (xBest != yBest) return xBest.CompareTo(yBest);

                // Tied members: keep each member's own entries together by member number
                if (x.MemberNumber != y.MemberNumber) return x.MemberNumber.CompareTo(y.MemberNumber);

                // Same member: placed entry first, then higher scores first
                bool xPlaced = x.PlaceStanding > 0;
                bool yPlaced = y.PlaceStanding > 0;
                if (xPlaced && !yPlaced) return -1;
                if (!xPlaced && yPlaced) return 1;
                return y.TotalScore.CompareTo(x.TotalScore);
            });
        }

        // Lookup for original nullable game scores — avoids null-vs-0 ambiguity
        var bowlerByGameId = _currentTournamentBowlers.ToDictionary(b => b.GameId);

        dgvTournament.Rows.Clear();
        foreach (ExcelMember m in members)
        {
            WinnerListMemberViewModel orig = bowlerByGameId[m.GameId];

            // A game is checked when it has a recorded score; 3-of-4 tournaments uncheck
            // the lowest of four; explicitly saved use-game flags override both defaults.
            UseGameFlags useFlags = finalizeCalculationService.DetermineUseGameDefaults(
                orig.Game1, orig.Game2, orig.Game3, orig.Game4,
                orig.UseGame1, orig.UseGame2, orig.UseGame3, orig.UseGame4,
                selectedTournament.ThreeOutOf4);
            bool g1Checked = useFlags.Game1;
            bool g2Checked = useFlags.Game2;
            bool g3Checked = useFlags.Game3;
            bool g4Checked = useFlags.Game4;

            int baseBonus = ResolveCarryInBonus(orig);

            // Everything the New Bonus preview needs that is not a live grid cell.
            int memberPlacing = _bestStandingByMember.TryGetValue(m.MemberNumber, out int p) ? p : 0;
            int histCount     = historicalCountByMember.TryGetValue(m.MemberNumber, out int hc) ? hc : 0;
            int currCount     = currentCountByMember.TryGetValue(m.MemberNumber, out int cc) ? cc : 0;
            _bonusContextByGameId[m.GameId] = new BonusContext(
                memberPlacing, histCount, currCount, orig.SidePot.HasValue ? (int)orig.SidePot.Value : 0,
                IsDoubles: false, DoublesCashing: false);

            bool hasPrevHB = _prevTournHBByMember.TryGetValue(m.MemberNumber, out var prevHB);
            int displayHdcp = finalizeCalculationService.ResolveDisplayHandicap(
                hasPrevHB ? prevHB.Hdcp : null, m.Handicap, orig.AdjustedAvg);

            // colHdcpTotal uses the carry-in bonus so it matches FrmTournamentResults.
            // colNewBonus shows the deducted/bumped value the member carries out.
            FinalizeRowResult rowCalc = finalizeCalculationService.RecalculateRow(new FinalizeRowInput(
                orig.Game1, orig.Game2, orig.Game3, orig.Game4,
                g1Checked, g2Checked, g3Checked, g4Checked,
                displayHdcp, orig.AdjustedAvg, baseBonus));
            int scratch   = rowCalc.ScratchTotal;
            int hdcpTotal = rowCalc.HdcpTotal;
            int entryAvg  = rowCalc.EntryAvg;

            // ADJ AVG shown in the grid; also drives the New HDCP preview, matching
            // the CalculateHandicapPins(adjAvg) write to Member.Handicap on finalize.
            int displayAdjAvg = orig.AdjustedAvg > 0 ? orig.AdjustedAvg : (int)Math.Round(orig.LeagueAverage);

            int rowIdx = dgvTournament.Rows.Add(
                m.PlaceStanding > 0 ? (object)m.PlaceStanding : null,
                m.MemberNumber,
                m.Name,
                orig.Game1,   // null if game not played
                g1Checked,
                orig.Game2,
                g2Checked,
                orig.Game3,
                g3Checked,
                orig.Game4,
                g4Checked,
                scratch,
                hdcpTotal,
                entryAvg,
                null,  // 30 Entry AVG
                displayAdjAvg,  // ADJ AVG — restored from DB, defaulting to the member's current average
                orig.KeepAdjustedAvg,  // Director Check — restored from DB
                orig.Squad,
                displayHdcp,
                finalizeCalculationService.ComputeNewHdcpPreview(displayAdjAvg),  // New HDCP preview
                baseBonus,
                null,  // New Bonus — filled in below once every row's Earnings are loaded
                _placedGameIds.Contains(m.GameId)
                    ? ((m.MoneyWon ?? 0) + (orig.SidePot ?? 0) > 0 ? (object)((m.MoneyWon ?? 0) + (orig.SidePot ?? 0)) : null)
                    : (m.MoneyWon > 0 ? (object)m.MoneyWon : null),  // Earnings (SidePot only on placed entry)
                null   // Notes
            );
            dgvTournament.Rows[rowIdx].Tag = m.GameId;
            ApplySandbaggingHighlight(rowIdx, orig.LeagueAverage);
        }

        // If the tournament is a 3 game format, hide the Game 4 column and its checkbox column since they are not used.
        if (selectedTournament.IsOnlyThreeGames)
        {
            dgvTournament.Columns["colGame4"].Visible = false;
            dgvTournament.Columns["colGame4Check"].Visible = false;
        }

        // Populate 30 Entry AVG for all member rows now that all entries are loaded
        foreach (int memberNum in _currentTournamentBowlers.Select(b => b.MemberNumber).Distinct())
            UpdateAll30AvgForMember(memberNum);

        // New Bonus reads every row's Earnings for the member, so it runs after the fill
        for (int i = 0; i < dgvTournament.Rows.Count; i++)
            UpdateNewBonusPreview(i);

        // Validate all rows so previously-valid rows are not incorrectly flagged on open
        for (int i = 0; i < dgvTournament.Rows.Count; i++)
            ValidateRow(i);
    }

    /// <summary>
    /// Builds the tournament grid for doubles tournaments.
    /// Each DoublesTeam produces two individual rows (one per member) that share the
    /// same place standing and combined HDCP total, making them appear as ties.
    /// colGame3 / colGame4 are hidden because each member only bowls 2 games.
    /// </summary>
    private void LoadTournamentGridDoubles()
    {
        // --- Build prevTournHBByMember for all members in this tournament ---
        var memberNumbersInTournament = _currentTournamentBowlers
            .Select(b => b.MemberNumber).Distinct().ToHashSet();

        _prevTournHBByMember.Clear();
        using (var dbPrev = dbFactory.CreateDbContext())
        {
            var latestApproved = dbPrev.Participants
                .Where(p => memberNumbersInTournament.Contains(p.Member.Number)
                         && p.Tournament.Id != selectedTournament.Id
                         && p.Game.IsFinalized
                         && p.Game.AdjustedAvg > 0)
                .GroupBy(p => p.Member.Number)
                .Select(g => new { MemberNumber = g.Key, LatestDate = g.Max(p => p.Tournament.Date) })
                .ToList();

            foreach (var item in latestApproved)
            {
                List<PreviousEntrySnapshot> prevEntries = dbPrev.Participants
                    .Where(p => p.Member.Number == item.MemberNumber
                             && p.Tournament.Id != selectedTournament.Id
                             && p.Game.IsFinalized
                             && p.Tournament.Date == item.LatestDate)
                    .Select(p => new PreviousEntrySnapshot(p.Game.AdjustedAvg, p.Game.Bonus ?? 0, p.Game.MoneyWon ?? 0))
                    .ToList();

                if (prevEntries.Count == 0) continue;

                _prevTournHBByMember[item.MemberNumber] =
                    finalizeCalculationService.ComputePreviousHandicapAndBonus(prevEntries);
            }
        }

        // --- Precompute 30-entry history for each member ---
        _history30ByMember.Clear();
        using (var dbH = dbFactory.CreateDbContext())
        {
            var allHistory = dbH.Participants
                .Where(p => p.Tournament.Id != selectedTournament.Id
                         && p.Game.IsFinalized
                         && memberNumbersInTournament.Contains(p.Member.Number))
                .OrderByDescending(p => p.Tournament.Date)
                .ThenByDescending(p => p.Game.Id)
                .Select(p => new
                {
                    MemberNumber = p.Member.Number,
                    G1 = p.Game.Game1, G2 = p.Game.Game2,
                    G3 = p.Game.Game3, G4 = p.Game.Game4,
                    U1 = p.Game.UseGame1, U2 = p.Game.UseGame2,
                    U3 = p.Game.UseGame3, U4 = p.Game.UseGame4
                })
                .ToList();

            foreach (var grp in allHistory.GroupBy(x => x.MemberNumber))
            {
                int memberNum = grp.Key;
                // Each member has 1 entry per doubles squad (2 current games)
                (int totalScratch, int totalGames) = finalizeCalculationService.Compute30EntryHistory(
                    grp.Select(g => new HistoryGameEntry(g.G1, g.G2, g.G3, g.G4, g.U1, g.U2, g.U3, g.U4)),
                    currentEntryCount: 2);
                if (totalGames > 0)
                    _history30ByMember[memberNum] = (totalScratch, totalGames);
            }
        }

        // --- Build team list with combined HDCP totals ---
        List<DoublesTeam> teams = doublesTeamRepository.GetTeamsByTournament(selectedTournament.Id);

        var bowlersByMemberId = _currentTournamentBowlers
            .GroupBy(b => b.MemberId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var teamRows = new List<(int CombinedHdcpTotal,
                                 WinnerListMemberViewModel M1, WinnerListMemberViewModel M2,
                                 int Hdcp1, int BaseBonus1,
                                 int Hdcp2, int BaseBonus2)>();

        foreach (var team in teams)
        {
            if (!bowlersByMemberId.TryGetValue(team.Member1.Id, out var entries1)) continue;
            if (!bowlersByMemberId.TryGetValue(team.Member2.Id, out var entries2)) continue;

            var m1 = entries1.FirstOrDefault(e => e.Squad == team.Squad);
            var m2 = entries2.FirstOrDefault(e => e.Squad == team.Squad);
            if (m1 == null || m2 == null) continue;

            bool has1 = _prevTournHBByMember.TryGetValue(m1.MemberNumber, out var hb1);
            bool has2 = _prevTournHBByMember.TryGetValue(m2.MemberNumber, out var hb2);

            int hdcp1     = has1 && hb1.Hdcp > 0 ? hb1.Hdcp : Convert.ToInt32(m1.Handicap);
            int hdcp2     = has2 && hb2.Hdcp > 0 ? hb2.Hdcp : Convert.ToInt32(m2.Handicap);
            int baseBonus1 = ResolveCarryInBonus(m1);
            int baseBonus2 = ResolveCarryInBonus(m2);

            int scratch1 = (m1.Game1 ?? 0) + (m1.Game2 ?? 0);
            int scratch2 = (m2.Game1 ?? 0) + (m2.Game2 ?? 0);
            int combinedHdcpTotal = finalizeCalculationService.ComputeCombinedHdcpTotal(
                scratch1, 2, hdcp1, baseBonus1,
                scratch2, 2, hdcp2, baseBonus2);

            teamRows.Add((combinedHdcpTotal, m1, m2, hdcp1, baseBonus1, hdcp2, baseBonus2));
        }

        // Sort descending by combined HDCP total
        teamRows.Sort((a, b) => b.CombinedHdcpTotal.CompareTo(a.CombinedHdcpTotal));

        // --- Assign place standings with tie detection ---
        int totalTeams = teamRows.Count;
        int compTeams  = 0;   // doubles teams are not comp entries in the current model
        _cashLine      = totalTeams > 0 ? CalcService.GetQtyOfMembersThatCanPlace(totalTeams, compTeams) : 0;

        int[] teamPlaces = finalizeCalculationService.AssignTeamPlaces(
            [.. teamRows.Select(r => r.CombinedHdcpTotal)]);

        // --- Populate grid ---
        dgvTournament.Rows.Clear();

        Color[] teamColors = [SystemColors.Window, Color.AliceBlue];

        for (int t = 0; t < totalTeams; t++)
        {
            var (combinedHdcpTotal, m1, m2, hdcp1, baseBonus1, hdcp2, baseBonus2) = teamRows[t];
            int  place     = teamPlaces[t];
            bool isCashing = place <= _cashLine || (m1.MoneyWon ?? 0) > 0 || (m2.MoneyWon ?? 0) > 0;
            Color rowColor = teamColors[t % 2];

            if (isCashing)
            {
                _cashingGameIds.Add(m1.GameId);
                _cashingGameIds.Add(m2.GameId);
            }

            _bonusContextByGameId[m1.GameId] = new BonusContext(
                place, 0, 0, SidePot: 0, IsDoubles: true, DoublesCashing: isCashing);
            _bonusContextByGameId[m2.GameId] = new BonusContext(
                place, 0, 0, SidePot: 0, IsDoubles: true, DoublesCashing: isCashing);

            _placedGameIds.Add(m1.GameId);
            _placedGameIds.Add(m2.GameId);

            // ---- Row for Member 1 ----
            bool m1g1c = m1.UseGame1 ?? m1.Game1.HasValue;
            bool m1g2c = m1.UseGame2 ?? m1.Game2.HasValue;
            int  s1    = (m1g1c ? m1.Game1 ?? 0 : 0) + (m1g2c ? m1.Game2 ?? 0 : 0);
            int  g1c   = (m1g1c ? 1 : 0) + (m1g2c ? 1 : 0);
            int  adjAvg1 = m1.AdjustedAvg > 0 ? m1.AdjustedAvg : (int)Math.Round(m1.LeagueAverage);

            int rowIdx1 = dgvTournament.Rows.Add(
                (object)place,
                m1.MemberNumber,
                m1.BowlerName,
                m1.Game1, m1g1c,
                m1.Game2, m1g2c,
                null, false, null, false,   // colGame3/4 will be hidden
                s1,
                combinedHdcpTotal,
                g1c > 0 ? s1 / g1c : 0,
                null,   // 30 Entry AVG
                adjAvg1,
                m1.KeepAdjustedAvg,
                m1.Squad,
                hdcp1,
                finalizeCalculationService.ComputeNewHdcpPreview(adjAvg1),  // New HDCP preview
                baseBonus1,
                null,   // New Bonus — filled in below
                m1.MoneyWon > 0 ? (object)m1.MoneyWon : null,
                $"Partner: {m2.BowlerName}"
            );
            dgvTournament.Rows[rowIdx1].Tag = new DoubleMemberRowTag(m1.GameId, m2.GameId);
            dgvTournament.Rows[rowIdx1].DefaultCellStyle.BackColor = rowColor;

            // ---- Row for Member 2 ----
            bool m2g1c = m2.UseGame1 ?? m2.Game1.HasValue;
            bool m2g2c = m2.UseGame2 ?? m2.Game2.HasValue;
            int  s2    = (m2g1c ? m2.Game1 ?? 0 : 0) + (m2g2c ? m2.Game2 ?? 0 : 0);
            int  g2c   = (m2g1c ? 1 : 0) + (m2g2c ? 1 : 0);
            int  adjAvg2 = m2.AdjustedAvg > 0 ? m2.AdjustedAvg : (int)Math.Round(m2.LeagueAverage);

            int rowIdx2 = dgvTournament.Rows.Add(
                (object)place,
                m2.MemberNumber,
                m2.BowlerName,
                m2.Game1, m2g1c,
                m2.Game2, m2g2c,
                null, false, null, false,
                s2,
                combinedHdcpTotal,
                g2c > 0 ? s2 / g2c : 0,
                null,
                adjAvg2,
                m2.KeepAdjustedAvg,
                m2.Squad,
                hdcp2,
                finalizeCalculationService.ComputeNewHdcpPreview(adjAvg2),  // New HDCP preview
                baseBonus2,
                null,   // New Bonus — filled in below
                m2.MoneyWon > 0 ? (object)m2.MoneyWon : null,
                $"Partner: {m1.BowlerName}"
            );
            dgvTournament.Rows[rowIdx2].Tag = new DoubleMemberRowTag(m2.GameId, m1.GameId);
            dgvTournament.Rows[rowIdx2].DefaultCellStyle.BackColor = rowColor;
        }

        // Populate 30-entry AVG and the New Bonus preview for each member
        foreach (int memberNum in memberNumbersInTournament)
            UpdateAll30AvgForMember(memberNum);

        for (int i = 0; i < dgvTournament.Rows.Count; i++)
            UpdateNewBonusPreview(i);

        // Hide columns not used in doubles (each member bowls only 2 games)
        dgvTournament.Columns["colGame3"].Visible      = false;
        dgvTournament.Columns["colGame3Check"].Visible = false;
        dgvTournament.Columns["colGame4"].Visible      = false;
        dgvTournament.Columns["colGame4Check"].Visible = false;

        // Validate all rows
        for (int i = 0; i < dgvTournament.Rows.Count; i++)
            ValidateRow(i);

        // Show the Team View toggle now that the doubles grid is loaded
        btnTeamView.Visible = true;
        PinToolbarButtons();
    }

    /// <summary>
    /// Converts a list of <see cref="WinnerListMemberViewModel"/> into a list of <see cref="ExcelMember"/>,
    /// calculating each member's <see cref="ExcelMember.TotalScore"/> (with handicap and bonus)
    /// according to the tournament format.
    /// </summary>
    private List<ExcelMember> BuildExcelMemberList(List<WinnerListMemberViewModel> bowlers)
    {
        List<ExcelMember> members = [];
        foreach (var b in bowlers)
        {
            bool hasPrevHBbem = _prevTournHBByMember.TryGetValue(b.MemberNumber, out var prevHBbem);
            ExcelMember m = new()
            {
                MemberNumber = b.MemberNumber,
                Name         = b.BowlerName,
                Handicap     = hasPrevHBbem && prevHBbem.Hdcp > 0 ? prevHBbem.Hdcp : Convert.ToInt32(b.Handicap),
                // Same carry-in bonus the grid shows, so place standings computed here
                // agree with the ones FrmTournamentResults produced
                Bonus        = ResolveCarryInBonus(b),
                MoneyWon     = b.MoneyWon,
                GameId       = b.GameId,
                Game1Score   = Convert.ToInt32(b.Game1),
                Game2Score   = Convert.ToInt32(b.Game2),
                Game3Score   = Convert.ToInt32(b.Game3),
                Game4Score   = Convert.ToInt32(b.Game4)
            };

            m.TotalScore = finalizeCalculationService.ComputeEntryTotalScore(
                b.Game1, b.Game2, b.Game3, b.Game4,
                m.Handicap, m.Bonus, selectedTournament.ThreeOutOf4);

            members.Add(m);
        }
        return members;
    }

    private static DataGridView CreateDetailGrid()
    {
        var dgv = new DataGridView
        {
            Dock                          = DockStyle.Fill,
            AllowUserToAddRows            = false,
            AllowUserToDeleteRows         = false,
            AutoSizeColumnsMode           = DataGridViewAutoSizeColumnsMode.None,
            ColumnHeadersHeightSizeMode   = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            ColumnHeadersHeight           = 42,
            SelectionMode                 = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersWidth               = 25
        };
        dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

        dgv.Columns.AddRange(
            new DataGridViewTextBoxColumn { Name = "colDetailGames",       HeaderText = "Games",         Width = 55,  ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "colDetailDate",        HeaderText = "Date",          Width = 80,  ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "colDetailGame1",       HeaderText = "Game1",         Width = 60,  ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "colDetailGame2",       HeaderText = "Game2",         Width = 60,  ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "colDetailGame3",       HeaderText = "Game3",         Width = 60,  ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "colDetailGame4",       HeaderText = "Game4",         Width = 60,  ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "colDetailScratch",     HeaderText = "Scratch",       Width = 60,  ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "colDetailWHdcp",       HeaderText = "w/HDCP",        Width = 60,  ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "colDetailEntry",       HeaderText = "Entry",         Width = 55,  ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "colDetail30Avg",       HeaderText = "30 AVG",        Width = 65,  ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "colDetailAdjustedAvg", HeaderText = "Adjusted\nAVG", Width = 65,  ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "colDetailHandicap",    HeaderText = "Handicap",      Width = 60,  ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "colDetailBonus",       HeaderText = "Bonus",         Width = 50  },
            new DataGridViewTextBoxColumn { Name = "colDetailPlace",       HeaderText = "Place",         Width = 50  },
            new DataGridViewTextBoxColumn { Name = "colDetailEarnings",    HeaderText = "Earnings",      Width = 70  },
            new DataGridViewTextBoxColumn { Name = "colDetailNotes",       HeaderText = "Notes",         AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill }
        );

        return dgv;
    }

    /// <summary>
    /// Updates the read-only New HDCP preview cell from the row's current ADJ AVG.
    /// Shows the handicap the Member record will receive when the tournament is
    /// finalized (FinalizeAllGames writes CalculateHandicapPins(adjAvg) to Member.Handicap).
    /// </summary>
    private void UpdateNewHdcpPreview(int rowIndex)
    {
        var row = dgvTournament.Rows[rowIndex];
        int adjAvg = 0;
        if (row.Cells["colAdjAvg"].Value != null)
            int.TryParse(row.Cells["colAdjAvg"].Value.ToString(), out adjAvg);
        row.Cells["colNewHdcp"].Value = finalizeCalculationService.ComputeNewHdcpPreview(adjAvg);
    }

    /// <summary>
    /// Updates the read-only New Bonus cell: the bonus pins the member will carry out of
    /// this tournament, derived from the row's carry-in Bonus cell, the member's place,
    /// and the place money showing in the Earnings cells.
    /// </summary>
    private void UpdateNewBonusPreview(int rowIndex)
    {
        var row = dgvTournament.Rows[rowIndex];
        int gameId = ResolveRowGameId(row);
        if (gameId == 0 || !_bonusContextByGameId.TryGetValue(gameId, out BonusContext ctx)) return;

        int baseBonus = ParseCellInt(row.Cells["colBonus"].Value) ?? 0;

        // Doubles pairs lose half as many pins as an individual placing the same
        if (ctx.IsDoubles)
        {
            row.Cells["colNewBonus"].Value =
                FinalizeCalculationService.ComputeHalfRateBonus(baseBonus, ctx.Placing, ctx.DoublesCashing);
            return;
        }

        BonusPreviewResult preview = finalizeCalculationService.ComputeBonusPreview(
            baseBonus, ctx.Placing, _cashLine, selectedTournament.IsTwoDay,
            ctx.HistoricalEntries, ctx.CurrentEntries, ComputeMemberMoneyWon(row));

        row.Cells["colNewBonus"].Value = preview.DisplayBonus;

        // 2-day championships never auto-deduct, so IsCashing is always false there;
        // their _cashingGameIds are seeded from money won in LoadTournamentGrid instead.
        if (selectedTournament.IsTwoDay) return;

        if (preview.IsCashing) _cashingGameIds.Add(gameId);
        else                   _cashingGameIds.Remove(gameId);
    }

    /// <summary>
    /// Totals the place money a member won across all of their entries, reading the live
    /// Earnings cells. Side pots are subtracted because they do not affect bonus pins.
    /// </summary>
    private decimal ComputeMemberMoneyWon(DataGridViewRow memberRow)
    {
        object memberNumber = memberRow.Cells["colMemberNumber"].Value;
        decimal total = 0;

        foreach (DataGridViewRow r in dgvTournament.Rows)
        {
            if (!Equals(r.Cells["colMemberNumber"].Value, memberNumber)) continue;

            decimal earnings = 0;
            if (r.Cells["colEarnings"].Value != null)
                decimal.TryParse(r.Cells["colEarnings"].Value.ToString(), out earnings);

            int gameId  = ResolveRowGameId(r);
            int sidePot = gameId > 0 && _bonusContextByGameId.TryGetValue(gameId, out BonusContext c) ? c.SidePot : 0;
            total += Math.Max(earnings - sidePot, 0);
        }

        return total;
    }

    /// <summary>
    /// Returns the game ID a grid row represents — either a singles int tag or the
    /// member's own game ID from a doubles tag — or 0 when the row carries neither.
    /// </summary>
    private static int ResolveRowGameId(DataGridViewRow row) => row.Tag switch
    {
        int gameId              => gameId,
        DoubleMemberRowTag tag  => tag.MyGameId,
        _                       => 0
    };

    /// <summary>
    /// Recomputes Scratch Total, HDCP Total, and Entry AVG for the given row
    /// based on which game checkboxes are currently checked.
    /// </summary>
    private void RecalculateTournamentRow(int rowIndex)
    {
        var row = dgvTournament.Rows[rowIndex];

        UpdateNewHdcpPreview(rowIndex);
        UpdateNewBonusPreview(rowIndex);

        int hdcp   = Convert.ToInt32(row.Cells["colHdcp"].Value ?? 0);
        int adjAvg = 0;
        if (row.Cells["colAdjAvg"].Value != null)
            int.TryParse(row.Cells["colAdjAvg"].Value.ToString(), out adjAvg);

        // The Bonus cell holds the carry-in bonus, which is what scores this tournament
        int baseBonus = ParseCellInt(row.Cells["colBonus"].Value) ?? 0;

        FinalizeRowResult calc = finalizeCalculationService.RecalculateRow(new FinalizeRowInput(
            ParseCellInt(row.Cells["colGame1"].Value),
            ParseCellInt(row.Cells["colGame2"].Value),
            ParseCellInt(row.Cells["colGame3"].Value),
            ParseCellInt(row.Cells["colGame4"].Value),
            row.Cells["colGame1Check"].Value as bool? ?? false,
            row.Cells["colGame2Check"].Value as bool? ?? false,
            row.Cells["colGame3Check"].Value as bool? ?? false,
            row.Cells["colGame4Check"].Value as bool? ?? false,
            hdcp, adjAvg, baseBonus));

        // A missing handicap was derived from the ADJ AVG — write it back to the grid.
        // A valid stored handicap is never overwritten (Phase 1).
        if (calc.HandicapWasDerived)
            row.Cells["colHdcp"].Value = calc.ResolvedHandicap;

        // --- Doubles individual rows: use only 2 games; combine with partner for HDCP total ---
        if (row.Tag is DoubleMemberRowTag)
        {
            row.Cells["colScratchTotal"].Value = calc.ScratchTotal;
            row.Cells["colEntryAvg"].Value     = calc.EntryAvg;

            var partnerRow = FindDoublePartnerRow(row);
            if (partnerRow != null)
            {
                bool pc1 = partnerRow.Cells["colGame1Check"].Value as bool? ?? false;
                bool pc2 = partnerRow.Cells["colGame2Check"].Value as bool? ?? false;
                int pg1  = pc1 ? Convert.ToInt32(partnerRow.Cells["colGame1"].Value ?? 0) : 0;
                int pg2  = pc2 ? Convert.ToInt32(partnerRow.Cells["colGame2"].Value ?? 0) : 0;
                int partnerScratch = pg1 + pg2;
                int partnerGames   = (pc1 ? 1 : 0) + (pc2 ? 1 : 0);
                int partnerHdcp      = Convert.ToInt32(partnerRow.Cells["colHdcp"].Value ?? 0);
                int partnerBaseBonus = ParseCellInt(partnerRow.Cells["colBonus"].Value) ?? 0;

                int combinedHdcpTotal = finalizeCalculationService.ComputeCombinedHdcpTotal(
                    calc.ScratchTotal, calc.CheckedGames, calc.ResolvedHandicap, baseBonus,
                    partnerScratch, partnerGames, partnerHdcp, partnerBaseBonus);

                row.Cells["colHdcpTotal"].Value        = combinedHdcpTotal;
                partnerRow.Cells["colHdcpTotal"].Value = combinedHdcpTotal;
            }
            else
            {
                row.Cells["colHdcpTotal"].Value = calc.HdcpTotal;
            }

            if (row.Cells["colMemberNumber"].Value is int memberNum30d)
                UpdateAll30AvgForMember(memberNum30d);
            return;
        }

        // --- Standard (singles) path ---
        row.Cells["colScratchTotal"].Value = calc.ScratchTotal;
        row.Cells["colHdcpTotal"].Value    = calc.HdcpTotal;
        row.Cells["colEntryAvg"].Value     = calc.EntryAvg;

        if (row.Cells["colMemberNumber"].Value is int memberNum30)
            UpdateAll30AvgForMember(memberNum30);
    }

    /// <summary>
    /// Recomputes and updates the col30EntryAvg cell for every row belonging to the
    /// given member, combining live current-tournament game data with the pre-loaded
    /// historical scratch/games stored in <see cref="_history30ByMember"/>.
    /// </summary>
    private void UpdateAll30AvgForMember(int memberNumber)
    {
        int currScratch = 0, currGames = 0;
        foreach (DataGridViewRow row in dgvTournament.Rows)
        {
            if (row.Cells["colMemberNumber"].Value is not int mn || mn != memberNumber) continue;
            bool c1 = row.Cells["colGame1Check"].Value as bool? ?? false;
            bool c2 = row.Cells["colGame2Check"].Value as bool? ?? false;
            bool c3 = row.Cells["colGame3Check"].Value as bool? ?? false;
            bool c4 = row.Cells["colGame4Check"].Value as bool? ?? false;

            currScratch += (c1 ? Convert.ToInt32(row.Cells["colGame1"].Value ?? 0) : 0)
                         + (c2 ? Convert.ToInt32(row.Cells["colGame2"].Value ?? 0) : 0)
                         + (c3 ? Convert.ToInt32(row.Cells["colGame3"].Value ?? 0) : 0)
                         + (c4 ? Convert.ToInt32(row.Cells["colGame4"].Value ?? 0) : 0);
            currGames   += (c1 ? 1 : 0) + (c2 ? 1 : 0) + (c3 ? 1 : 0) + (c4 ? 1 : 0);
        }

        _history30ByMember.TryGetValue(memberNumber, out var hist);
        double avg30 = finalizeCalculationService.Compute30EntryAverage(
            hist.Scratch, hist.Games, currScratch, currGames);
        object avgVal = avg30 > 0 ? (object)avg30 : null;

        foreach (DataGridViewRow row in dgvTournament.Rows)
        {
            if (row.Cells["colMemberNumber"].Value is not int mn || mn != memberNumber) continue;
            row.Cells["col30EntryAvg"].Value = avgVal;
        }
    }

    /// <summary>
    /// Commits a checkbox edit the moment the cell becomes dirty so that
    /// <see cref="DgvTournament_CellValueChanged"/> fires immediately on click.
    /// </summary>
    private void DgvTournament_CurrentCellDirtyStateChanged(object sender, EventArgs e)
    {
        if (dgvTournament.IsCurrentCellDirty)
            dgvTournament.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    private void DgvTournament_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        string colName = dgvTournament.Columns[e.ColumnIndex].Name;

        if (colName is "colGame1Check" or "colGame2Check" or "colGame3Check" or "colGame4Check")
        {
            RecalculateTournamentRow(e.RowIndex);
            PersistRowToDatabase(e.RowIndex);
        }

        if (colName == "colDirCheck")
        {
            bool newValue = dgvTournament.Rows[e.RowIndex].Cells["colDirCheck"].Value as bool? ?? false;
            object memberNumber = dgvTournament.Rows[e.RowIndex].Cells["colMemberNumber"].Value;

            foreach (DataGridViewRow row in dgvTournament.Rows)
            {
                if (row.Index == e.RowIndex) continue;
                if (Equals(row.Cells["colMemberNumber"].Value, memberNumber))
                {
                    row.Cells["colDirCheck"].Value = newValue;
                    ValidateRow(row.Index);
                    dgvTournament.InvalidateRow(row.Index);
                }
            }

            ValidateRow(e.RowIndex);
            dgvTournament.InvalidateRow(e.RowIndex);
        }

        if (GameScoreColumns.Contains(colName))
        {
            RecalculateTournamentRow(e.RowIndex);
            // Re-evaluate sandbagging highlight using the stored league average in the row tag
            var row = dgvTournament.Rows[e.RowIndex];
            if (row.Tag is int gameId)
            {
                WinnerListMemberViewModel orig = _currentTournamentBowlers.FirstOrDefault(b => b.GameId == gameId);
                if (orig != null)
                    ApplySandbaggingHighlight(e.RowIndex, orig.LeagueAverage);
            }
        }

        if (colName is "colDirCheck" or "colAdjAvg")
        {
            ValidateRow(e.RowIndex);
            dgvTournament.InvalidateRow(e.RowIndex);
        }

        // Refresh detail grid when game checkboxes change
        if (colName is "colGame1Check" or "colGame2Check" or "colGame3Check" or "colGame4Check")
        {
            _displayedDetailMemberNumber = -1;
            DgvTournament_SelectionChanged(dgvTournament, EventArgs.Empty);
        }

        // Persist Director Check to the database immediately (checkbox commit)
        if (colName == "colDirCheck")
            PersistRowToDatabase(e.RowIndex);
    }

    /// <summary>
    /// Fires when a cell finishes editing (loses focus). Used for text columns
    /// like ADJ AVG, Bonus, game scores, and notes so we don't trigger per-keystroke.
    /// </summary>
    private void DgvTournament_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        string colName = dgvTournament.Columns[e.ColumnIndex].Name;

        if (colName == "colAdjAvg")
        {
            // Auto-calculate HDCP from ADJ AVG
            int adjAvg = 0;
            if (dgvTournament.Rows[e.RowIndex].Cells["colAdjAvg"].Value != null)
                int.TryParse(dgvTournament.Rows[e.RowIndex].Cells["colAdjAvg"].Value.ToString(), out adjAvg);

            if (adjAvg > 0)
            {
                RecalculateTournamentRow(e.RowIndex);
            }

            // Keep the New HDCP preview in sync even when ADJ AVG is cleared
            // (RecalculateTournamentRow only runs above when adjAvg > 0).
            UpdateNewHdcpPreview(e.RowIndex);

            // Copy ADJ AVG to all other entries for the same member
            object memberNumber = dgvTournament.Rows[e.RowIndex].Cells["colMemberNumber"].Value;
            foreach (DataGridViewRow row in dgvTournament.Rows)
            {
                if (row.Index == e.RowIndex) continue;
                if (Equals(row.Cells["colMemberNumber"].Value, memberNumber))
                {
                    row.Cells["colAdjAvg"].Value = dgvTournament.Rows[e.RowIndex].Cells["colAdjAvg"].Value;
                    RecalculateTournamentRow(row.Index);
                    ValidateRow(row.Index);
                    dgvTournament.InvalidateRow(row.Index);
                    PersistRowToDatabase(row.Index);
                }
            }

            ValidateRow(e.RowIndex);
            dgvTournament.InvalidateRow(e.RowIndex);
        }

        if (colName == "colBonus")
        {
            // Carry-in bonus belongs to the member, not the entry — mirror it onto their
            // other entries so every row scores with the same value.
            object memberNumber = dgvTournament.Rows[e.RowIndex].Cells["colMemberNumber"].Value;
            foreach (DataGridViewRow row in dgvTournament.Rows)
            {
                if (row.Index == e.RowIndex) continue;
                if (!Equals(row.Cells["colMemberNumber"].Value, memberNumber)) continue;

                row.Cells["colBonus"].Value = dgvTournament.Rows[e.RowIndex].Cells["colBonus"].Value;
                RecalculateTournamentRow(row.Index);
                PersistRowToDatabase(row.Index);
            }

            RecalculateTournamentRow(e.RowIndex);
        }

        // Earnings decide whether the member cashed, which drives the New Bonus preview
        // for every one of their entries.
        if (colName == "colEarnings")
        {
            object memberNumber = dgvTournament.Rows[e.RowIndex].Cells["colMemberNumber"].Value;
            foreach (DataGridViewRow row in dgvTournament.Rows)
                if (Equals(row.Cells["colMemberNumber"].Value, memberNumber))
                    UpdateNewBonusPreview(row.Index);
        }

        // Persist edited text columns to the database on focus loss
        if (colName is "colAdjAvg" or "colBonus" or "colEarnings" or "colNotes"
            or "colGame1" or "colGame2" or "colGame3" or "colGame4")
        {
            PersistRowToDatabase(e.RowIndex);
        }

        // Refresh detail grid when editable tournament columns change (on focus loss)
        if (colName is "colAdjAvg" or "colBonus" or "colGame1" or "colGame2" or "colGame3" or "colGame4")
        {
            _displayedDetailMemberNumber = -1;
            DgvTournament_SelectionChanged(dgvTournament, EventArgs.Empty);
        }
    }

    private static int? ParseCellInt(object cellValue)
    {
        if (cellValue == null) return null;
        if (cellValue is int i) return i;
        return int.TryParse(cellValue.ToString(), out int parsed) ? parsed : null;
    }

    /// <summary>
    /// Persists editable column values from the tournament grid row to the Game record in the database.
    /// </summary>
    private void PersistRowToDatabase(int rowIndex)
    {
        var row = dgvTournament.Rows[rowIndex];

        int gameId = ResolveRowGameId(row);
        if (gameId == 0) return;

        Game game = gameRepository.GetGame(gameId);
        if (game == null) return;

        // Game scores — parse safely whether value is a boxed int or a typed string
        game.Game1 = ParseCellInt(row.Cells["colGame1"].Value);
        game.Game2 = ParseCellInt(row.Cells["colGame2"].Value);
        game.Game3 = ParseCellInt(row.Cells["colGame3"].Value);
        game.Game4 = ParseCellInt(row.Cells["colGame4"].Value);

        // Use-game flags
        game.UseGame1 = row.Cells["colGame1Check"].Value as bool? ?? false;
        game.UseGame2 = row.Cells["colGame2Check"].Value as bool? ?? false;
        game.UseGame3 = row.Cells["colGame3Check"].Value as bool? ?? false;
        game.UseGame4 = row.Cells["colGame4Check"].Value as bool? ?? false;

        // ADJ AVG
        int adjAvg = 0;
        if (row.Cells["colAdjAvg"].Value != null)
            int.TryParse(row.Cells["colAdjAvg"].Value.ToString(), out adjAvg);
        game.AdjustedAvg = adjAvg;

        // Handicap
        int hdcp = 0;
        if (row.Cells["colHdcp"].Value != null)
            int.TryParse(row.Cells["colHdcp"].Value.ToString(), out hdcp);
        game.Handicap = hdcp;

        // Game.Bonus is the carry-in bonus this entry was scored with, so it stays stable
        // across reopens. The post-tournament value lives in New Bonus and is written to
        // Member.Bonus at finalization.
        game.Bonus = ParseCellInt(row.Cells["colBonus"].Value) ?? 0;

        // Director Check → persisted as KeepAdjustedAvg
        game.KeepAdjustedAvg = row.Cells["colDirCheck"].Value as bool? ?? false;

        // Earnings
        decimal earnings = 0;
        if (row.Cells["colEarnings"].Value != null)
            decimal.TryParse(row.Cells["colEarnings"].Value.ToString(), out earnings);
        game.MoneyWon = earnings > 0 ? earnings : null;

        // Notes
        game.Notes = row.Cells["colNotes"].Value as string;

        gameRepository.AddOrUpdateGame(game);
    }

    /// <summary>
    /// For a doubles individual row, finds the partner's row (the row whose
    /// <see cref="DoubleMemberRowTag.MyGameId"/> equals this row's
    /// <see cref="DoubleMemberRowTag.PartnerGameId"/>).
    /// Returns null when the row is not a doubles row or the partner is not in the grid.
    /// </summary>
    private DataGridViewRow FindDoublePartnerRow(DataGridViewRow row)
    {
        if (row.Tag is not DoubleMemberRowTag dmt) return null;
        foreach (DataGridViewRow r in dgvTournament.Rows)
        {
            if (r.Index == row.Index) continue;
            if (r.Tag is DoubleMemberRowTag d && d.MyGameId == dmt.PartnerGameId)
                return r;
        }
        return null;
    }

    private void BtnTeamView_Click(object sender, EventArgs e)
    {
        _inTeamView = !_inTeamView;
        if (_inTeamView)
        {
            BuildTeamView();
            dgvTeamView.Visible  = true;
            dgvTournament.Visible = false;
            btnTeamView.Text     = "Individual View";
        }
        else
        {
            dgvTeamView.Visible  = false;
            dgvTournament.Visible = true;
            btnTeamView.Text     = "Team View";
        }
    }

    /// <summary>
    /// Rebuilds <see cref="dgvTeamView"/> from the current doubles grid rows,
    /// collapsing each team into a single read-only summary row.
    /// </summary>
    private void BuildTeamView()
    {
        dgvTeamView.Columns.Clear();
        dgvTeamView.Rows.Clear();

        dgvTeamView.Columns.AddRange(
            new DataGridViewTextBoxColumn { Name = "tvPlace",    HeaderText = "Place",           Width = 55 },
            new DataGridViewTextBoxColumn { Name = "tvMember1",  HeaderText = "Member 1",        Width = 160 },
            new DataGridViewTextBoxColumn { Name = "tvMember2",  HeaderText = "Member 2",        Width = 160 },
            new DataGridViewTextBoxColumn { Name = "tvScratch1", HeaderText = "M1\nScratch",     Width = 65 },
            new DataGridViewTextBoxColumn { Name = "tvScratch2", HeaderText = "M2\nScratch",     Width = 65 },
            new DataGridViewTextBoxColumn { Name = "tvHdcpTotal",HeaderText = "Combined\nHDCP",  Width = 80 },
            new DataGridViewTextBoxColumn { Name = "tvBonus1",   HeaderText = "M1\nBonus",       Width = 55 },
            new DataGridViewTextBoxColumn { Name = "tvBonus2",   HeaderText = "M2\nBonus",       Width = 55 },
            new DataGridViewTextBoxColumn { Name = "tvEarn1",    HeaderText = "M1\nEarnings",    Width = 75 },
            new DataGridViewTextBoxColumn { Name = "tvEarn2",    HeaderText = "M2\nEarnings",    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill }
        );

        static int CellInt(DataGridViewCell cell)
        {
            if (cell.Value is int i) return i;
            if (cell.Value != null && int.TryParse(cell.Value.ToString(), out int p)) return p;
            return 0;
        }

        Color[] teamColors = [SystemColors.Window, Color.AliceBlue];

        // LoadTournamentGridDoubles always writes rows in consecutive pairs: [M1, M2, M1, M2, ...]
        // Step through 2 at a time — avoids any partner-lookup fragility.
        var doublesRows = dgvTournament.Rows
            .Cast<DataGridViewRow>()
            .Where(r => r.Tag is DoubleMemberRowTag)
            .ToList();

        for (int i = 0; i + 1 < doublesRows.Count; i += 2)
        {
            DataGridViewRow row     = doublesRows[i];
            DataGridViewRow partner = doublesRows[i + 1];

            int place     = CellInt(row.Cells["colStanding"]);
            string name1  = row.Cells["colName"].Value?.ToString() ?? "";
            string name2  = partner.Cells["colName"].Value?.ToString() ?? "";
            int scratch1  = CellInt(row.Cells["colScratchTotal"]);
            int scratch2  = CellInt(partner.Cells["colScratchTotal"]);
            int hdcpTotal = CellInt(row.Cells["colHdcpTotal"]);
            int bonus1    = CellInt(row.Cells["colBonus"]);
            int bonus2    = CellInt(partner.Cells["colBonus"]);
            string earn1  = row.Cells["colEarnings"].Value?.ToString() ?? "";
            string earn2  = partner.Cells["colEarnings"].Value?.ToString() ?? "";

            int rowIdx = dgvTeamView.Rows.Add(
                place > 0 ? (object)place : "",
                name1, name2,
                scratch1, scratch2,
                hdcpTotal,
                bonus1, bonus2,
                earn1, earn2);

            dgvTeamView.Rows[rowIdx].DefaultCellStyle.BackColor = teamColors[(i / 2) % 2];
        }
    }

    /// <summary>
    /// Persists the current game-check states to the database for the given row.
    /// </summary>
    private void UpdateGameUseFlags(int rowIndex)
    {
        var row = dgvTournament.Rows[rowIndex];

        // Doubles rows: persist individual game's use flags
        if (row.Tag is DoubleMemberRowTag dmt)
        {
            Game dblGame = gameRepository.GetGame(dmt.MyGameId);
            if (dblGame == null) return;
            dblGame.UseGame1 = row.Cells["colGame1Check"].Value as bool? ?? false;
            dblGame.UseGame2 = row.Cells["colGame2Check"].Value as bool? ?? false;
            gameRepository.AddOrUpdateGame(dblGame);
            return;
        }

        if (row.Tag is not int gameId) return;

        Game game = gameRepository.GetGame(gameId);
        if (game == null) return;

        game.UseGame1 = row.Cells["colGame1Check"].Value as bool? ?? false;
        game.UseGame2 = row.Cells["colGame2Check"].Value as bool? ?? false;
        game.UseGame3 = row.Cells["colGame3Check"].Value as bool? ?? false;
        game.UseGame4 = row.Cells["colGame4Check"].Value as bool? ?? false;

        gameRepository.AddOrUpdateGame(game);
    }

    /// <summary>
    /// Checks whether the given row passes finalization validation (Director Check
    /// is checked and ADJ AVG is non-zero) and updates <see cref="_invalidRowIndices"/>.
    /// </summary>
    private void ValidateRow(int rowIndex)
    {
        var row = dgvTournament.Rows[rowIndex];
        bool dirChecked = row.Cells["colDirCheck"].Value as bool? ?? false;
        int adjAvg = 0;
        if (row.Cells["colAdjAvg"].Value != null)
            int.TryParse(row.Cells["colAdjAvg"].Value.ToString(), out adjAvg);

        if (!finalizeCalculationService.IsRowValid(dirChecked, adjAvg))
            _invalidRowIndices.Add(rowIndex);
        else
            _invalidRowIndices.Remove(rowIndex);
    }

    /// <summary>
    /// Draws a red border around rows that failed finalization validation.
    /// </summary>
    private void DgvTournament_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
    {
        if (!_invalidRowIndices.Contains(e.RowIndex)) return;

        using var pen = new Pen(Color.Red, 2);
        var rect = new Rectangle(
            e.RowBounds.Left + 1,
            e.RowBounds.Top,
            e.RowBounds.Width - 3,
            e.RowBounds.Height - 1);
        e.Graphics.DrawRectangle(pen, rect);
    }

    /// <summary>
    /// Validates every row and, if all pass, marks all games as finalized in the database.
    /// </summary>
    private void BtnFinalizeTournament_Click(object sender, EventArgs e)
    {
        _invalidRowIndices.Clear();

        for (int i = 0; i < dgvTournament.Rows.Count; i++)
            ValidateRow(i);

        dgvTournament.Invalidate();

        if (_invalidRowIndices.Count > 0)
        {
            MessageBox.Show(
                "Some rows are missing a Director Check or have a zero Adjusted Average.\n" +
                "Please fix the highlighted rows before finalizing.",
                "Validation Failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        FinalizeAllGames();
    }

    /// <summary>
    /// Marks every game in the tournament as finalized, persists the adjusted average,
    /// and sets <see cref="Tournament.IsTournamentFinalized"/>.
    /// </summary>
    private void FinalizeAllGames()
    {
        using var db = dbFactory.CreateDbContext();

        // Track which members we've already updated so we do it once per member
        var updatedMembers = new HashSet<int>();

        for (int i = 0; i < dgvTournament.Rows.Count; i++)
        {
            var row = dgvTournament.Rows[i];

            // Doubles individual rows: finalize each member's own game record
            if (row.Tag is DoubleMemberRowTag dmt)
            {
                Game dblGame = db.Games.Find(dmt.MyGameId);
                if (dblGame == null) continue;

                int dblAdjAvg = 0;
                if (row.Cells["colAdjAvg"].Value != null)
                    int.TryParse(row.Cells["colAdjAvg"].Value.ToString(), out dblAdjAvg);

                dblGame.Game1           = ParseCellInt(row.Cells["colGame1"].Value);
                dblGame.Game2           = ParseCellInt(row.Cells["colGame2"].Value);
                dblGame.UseGame1        = row.Cells["colGame1Check"].Value as bool? ?? false;
                dblGame.UseGame2        = row.Cells["colGame2Check"].Value as bool? ?? false;
                dblGame.IsFinalized     = true;
                dblGame.AdjustedAvg     = dblAdjAvg;
                dblGame.KeepAdjustedAvg = row.Cells["colDirCheck"].Value as bool? ?? false;

                int dblHdcp = 0;
                if (row.Cells["colHdcp"].Value != null)
                    int.TryParse(row.Cells["colHdcp"].Value.ToString(), out dblHdcp);
                dblGame.Handicap = dblHdcp;

                int dblMemberNum = row.Cells["colMemberNumber"].Value is int dblMn ? dblMn : 0;
                dblGame.LeagueAverage = finalizeTempRepository.Get30GameAverage(dblMemberNum, selectedTournament.Id);

                // Game.Bonus keeps the carry-in value this entry was scored with;
                // New Bonus (the half-rate result for doubles) goes to the member below.
                int dblBaseBonus = ParseCellInt(row.Cells["colBonus"].Value) ?? 0;
                int dblNewBonus  = ParseCellInt(row.Cells["colNewBonus"].Value) ?? dblBaseBonus;
                int dblPlace     = row.Cells["colStanding"].Value is int dblP ? dblP : 0;
                dblGame.Bonus    = dblBaseBonus;

                // Director entered the full place prize; save each member's 50% share
                decimal dblEarnings = 0;
                if (row.Cells["colEarnings"].Value != null)
                    decimal.TryParse(row.Cells["colEarnings"].Value.ToString(), out dblEarnings);
                dblGame.MoneyWon      = dblEarnings > 0 ? dblEarnings / 2m : null;
                dblGame.PlaceStanding = dblPlace > 0 ? (int?)dblPlace : null;

                if (dblMemberNum > 0 && updatedMembers.Add(dblMemberNum))
                {
                    Member dblMember = memberRepository.GetMember(dblMemberNum, db);
                    if (dblMember != null && dblMember.Id > 0)
                    {
                        dblMember.Average  = dblAdjAvg;
                        dblMember.Handicap = CalcService.CalculateHandicapPins(dblAdjAvg);
                        dblMember.Bonus    = dblNewBonus;
                    }
                }
                continue;
            }

            if (row.Tag is not int gameId) continue;

            Game game = db.Games.Find(gameId);
            if (game == null) continue;

            game.IsFinalized = true;
            int adjAvg = 0;
            if (row.Cells["colAdjAvg"].Value != null)
                int.TryParse(row.Cells["colAdjAvg"].Value.ToString(), out adjAvg);
            game.AdjustedAvg = adjAvg;

            // Update handicap on the game from the grid
            int hdcp = 0;
            if (row.Cells["colHdcp"].Value != null)
                int.TryParse(row.Cells["colHdcp"].Value.ToString(), out hdcp);
            game.Handicap = hdcp;

            // Compute league average for this game
            int memberNumber = Convert.ToInt32(row.Cells["colMemberNumber"].Value);

            // Game.Bonus records the carry-in bonus this entry was scored with, so
            // re-finalizing never deducts a second time. New Bonus is the value the
            // member carries forward and is written to Member.Bonus below.
            int baseBonus = ParseCellInt(row.Cells["colBonus"].Value) ?? 0;
            int newBonus  = ParseCellInt(row.Cells["colNewBonus"].Value) ?? baseBonus;
            game.Bonus = baseBonus;
            double leagueAvg = finalizeTempRepository.Get30GameAverage(memberNumber, selectedTournament.Id);
            game.LeagueAverage = leagueAvg;

            // Update the Member record once per member
            if (updatedMembers.Add(memberNumber))
            {
                Member member = memberRepository.GetMember(memberNumber, db);
                if (member != null && member.Id > 0)
                {
                    member.Average = adjAvg;
                    member.Handicap = CalcService.CalculateHandicapPins(adjAvg);
                    // New Bonus already reflects the deduction for cashers and the +1 for
                    // third-entry new bowlers, plus any director edit to the Bonus cell.
                    // Rows are ordered placed-entry-first, so multi-entry members take the
                    // New Bonus from the entry they placed with.
                    member.Bonus = newBonus;
                }
            }
        }

        Tournament tourn = db.Tournaments.Find(selectedTournament.Id);
        tourn?.IsTournamentFinalized = true;

        db.SaveChanges();

        _isFinalized = true;
        ApplyFinalizedState();

        // These entries are now part of each member's finalized history, so reload the
        // detail grid to show them there instead of as live preview rows.
        _displayedDetailMemberNumber = -1;
        DgvTournament_SelectionChanged(dgvTournament, EventArgs.Empty);

        MessageBox.Show(
            "Tournament has been finalized successfully.",
            "Finalized",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void BtnUndoFinalize_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "This will revert all grid changes back to the values loaded when the form was opened.\n\nAre you sure?",
            "Undo Changes",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        if (result != DialogResult.Yes) return;

        UndoChanges();
    }

    /// <summary>
    /// Restores every game record to the values captured at form-open time,
    /// then reloads the grid. Only available before finalization.
    /// </summary>
    private void UndoChanges()
    {
        using var db = dbFactory.CreateDbContext();

        foreach (var (gameId, snap) in _gameSnapshot)
        {
            Game game = db.Games.Find(gameId);
            if (game == null) continue;

            game.Game1          = snap.Game1;
            game.Game2          = snap.Game2;
            game.Game3          = snap.Game3;
            game.Game4          = snap.Game4;
            game.UseGame1       = snap.UseGame1;
            game.UseGame2       = snap.UseGame2;
            game.UseGame3       = snap.UseGame3;
            game.UseGame4       = snap.UseGame4;
            game.AdjustedAvg    = snap.AdjustedAvg;
            game.KeepAdjustedAvg = snap.KeepAdjustedAvg;
            game.Handicap       = snap.Handicap;
            game.Bonus          = snap.Bonus;
            game.MoneyWon       = snap.MoneyWon;
            game.Notes          = snap.Notes;
        }

        db.SaveChanges();

        _invalidRowIndices.Clear();
        LoadTournamentGrid();

        MessageBox.Show(
            "All changes have been reverted to the original values.",
            "Changes Reverted",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void DgvTournament_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvTournament.SelectedRows.Count == 0) return;
        var row = dgvTournament.SelectedRows[0];
        if (row.Cells["colMemberNumber"].Value is not int memberNumber) return;
        if (memberNumber == _displayedDetailMemberNumber) return;
        string name = row.Cells["colName"].Value as string ?? string.Empty;
        _displayedDetailMemberNumber = memberNumber;
        LoadDetailGrid(memberNumber, name);
    }

    /// <summary>
    /// Highlights game cells (Game 1–4) in yellow when the score is 40 or more pins
    /// below the bowler's league average, indicating potential sandbagging.
    /// Clears any existing sandbagging highlight on cells that do not trigger the condition.
    /// Does nothing when <paramref name="leagueAverage"/> is zero (average not yet set).
    /// </summary>
    private void ApplySandbaggingHighlight(int rowIndex, double leagueAverage)
    {
        var row = dgvTournament.Rows[rowIndex];

        foreach (string scoreCol in GameScoreColumns)
        {
            var cell = row.Cells[scoreCol];
            if (cell.Value != null
                && int.TryParse(cell.Value.ToString(), out int score)
                && finalizeCalculationService.IsSandbaggingScore(leagueAverage, score))
            {
                cell.Style.BackColor = Color.Yellow;
            }
            else
            {
                cell.Style.BackColor = Color.Empty;
            }
        }
    }

    /// <summary>
    /// Populates <see cref="dgvDetail"/> with all tournament entries for the given member.
    /// Current-tournament rows appear at the top with a blue highlight.
    /// The 30 AVG cell is highlighted light green for the most recent 30 finalized entries.
    /// </summary>
    private void LoadDetailGrid(int memberNumber, string memberName)
    {
        const int thirtyGameWindow = 30;

        List<PlayerHistoryViewModel> history =
            playerHistoryRepository.GetMemberPlayerHistory(memberNumber);

        dgvDetail.Rows.Clear();

        // --- Current tournament rows (blue highlight) ---
        // Pull live values from dgvTournament so edits are immediately reflected.
        // Entries are sorted by w/HDCP descending when the member cashed,
        // or by squad number ascending when they did not cash.
        // Once the tournament is finalized these entries appear in the history section
        // below, so the preview rows are skipped to avoid listing each entry twice.
        bool memberCashed = _currentTournamentBowlers
            .Where(b => b.MemberNumber == memberNumber)
            .Any(b => _cashingGameIds.Contains(b.GameId));

        List<WinnerListMemberViewModel> currentEntries = _isFinalized
            ? []
            : memberCashed
            ? _currentTournamentBowlers
                .Where(b => b.MemberNumber == memberNumber)
                .OrderByDescending(b =>
                {
                    DataGridViewRow tr = null;
                    foreach (DataGridViewRow r in dgvTournament.Rows)
                        if (r.Tag is int gid && gid == b.GameId) { tr = r; break; }
                    if (tr != null)
                    {
                        bool c1 = tr.Cells["colGame1Check"].Value as bool? ?? false;
                        bool c2 = tr.Cells["colGame2Check"].Value as bool? ?? false;
                        bool c3 = tr.Cells["colGame3Check"].Value as bool? ?? false;
                        bool c4 = tr.Cells["colGame4Check"].Value as bool? ?? false;
                        int s  = (c1 ? Convert.ToInt32(tr.Cells["colGame1"].Value ?? 0) : 0)
                               + (c2 ? Convert.ToInt32(tr.Cells["colGame2"].Value ?? 0) : 0)
                               + (c3 ? Convert.ToInt32(tr.Cells["colGame3"].Value ?? 0) : 0)
                               + (c4 ? Convert.ToInt32(tr.Cells["colGame4"].Value ?? 0) : 0);
                        int gc = (c1 ? 1 : 0) + (c2 ? 1 : 0) + (c3 ? 1 : 0) + (c4 ? 1 : 0);
                        int h  = Convert.ToInt32(tr.Cells["colHdcp"].Value  ?? 0);
                        int bn = Convert.ToInt32(tr.Cells["colBonus"].Value ?? 0);
                        return s + gc * (h + bn);
                    }
                    int scratch = (b.Game1 ?? 0) + (b.Game2 ?? 0) + (b.Game3 ?? 0) + (b.Game4 ?? 0);
                    int games   = (b.Game1.HasValue ? 1 : 0) + (b.Game2.HasValue ? 1 : 0)
                                + (b.Game3.HasValue ? 1 : 0) + (b.Game4.HasValue ? 1 : 0);
                    return scratch + games * (Convert.ToInt32(b.Handicap) + Convert.ToInt32(b.Bonus));
                })
                .ToList()
            : _currentTournamentBowlers
                .Where(b => b.MemberNumber == memberNumber)
                .OrderByDescending(b => b.Squad)
                .ToList();

        // Pre-compute the historical portion of the 30-entry AVG window
        int histLimit     = Math.Max(30 - currentEntries.Count, 0);
        int histScratch30 = history.Take(histLimit).Sum(h => h.TotalScore);
        int histGames30   = history.Take(histLimit).Sum(h => h.GamesPlayed);
        int currScratch30 = 0, currGames30 = 0;
        var currentDetailRowIndices = new List<int>();

        foreach (WinnerListMemberViewModel b in currentEntries)
        {
            // Find the matching tournament grid row to read live-edited values
            DataGridViewRow tournRow = null;
            foreach (DataGridViewRow r in dgvTournament.Rows)
            {
                if (r.Tag is int gid && gid == b.GameId) { tournRow = r; break; }
            }

            int? dg1, dg2, dg3, dg4;
            int hdcp, bonus;
            if (tournRow != null)
            {
                bool c1 = tournRow.Cells["colGame1Check"].Value as bool? ?? false;
                bool c2 = tournRow.Cells["colGame2Check"].Value as bool? ?? false;
                bool c3 = tournRow.Cells["colGame3Check"].Value as bool? ?? false;
                bool c4 = tournRow.Cells["colGame4Check"].Value as bool? ?? false;

                dg1 = c1 ? ParseCellInt(tournRow.Cells["colGame1"].Value) : null;
                dg2 = c2 ? ParseCellInt(tournRow.Cells["colGame2"].Value) : null;
                dg3 = c3 ? ParseCellInt(tournRow.Cells["colGame3"].Value) : null;
                dg4 = c4 ? ParseCellInt(tournRow.Cells["colGame4"].Value) : null;

                hdcp  = Convert.ToInt32(tournRow.Cells["colHdcp"].Value  ?? 0);
                bonus = Convert.ToInt32(tournRow.Cells["colBonus"].Value ?? 0);
            }
            else
            {
                dg1 = b.UseGame1 == false ? null : (b.Game1.HasValue ? (int?)b.Game1.Value : null);
                dg2 = b.UseGame2 == false ? null : (b.Game2.HasValue ? (int?)b.Game2.Value : null);
                dg3 = b.UseGame3 == false ? null : (b.Game3.HasValue ? (int?)b.Game3.Value : null);
                dg4 = b.UseGame4 == false ? null : (b.Game4.HasValue ? (int?)b.Game4.Value : null);
                hdcp  = Convert.ToInt32(b.Handicap);
                bonus = Convert.ToInt32(b.Bonus);
            }

            int adjAvg = 0;
            if (tournRow?.Cells["colAdjAvg"].Value != null)
                int.TryParse(tournRow.Cells["colAdjAvg"].Value.ToString(), out adjAvg);

            int validGames = (dg1.HasValue ? 1 : 0) + (dg2.HasValue ? 1 : 0)
                           + (dg3.HasValue ? 1 : 0) + (dg4.HasValue ? 1 : 0);
            int scratch    = (dg1 ?? 0) + (dg2 ?? 0) + (dg3 ?? 0) + (dg4 ?? 0);
            int wHdcp      = scratch + (validGames * (hdcp + bonus));
            int entry      = validGames > 0 ? scratch / validGames : 0;

            int rowIdx = dgvDetail.Rows.Add(
                validGames,
                selectedTournament.Date.ToShortDateString(),
                dg1.HasValue ? (object)dg1.Value : null,
                dg2.HasValue ? (object)dg2.Value : null,
                dg3.HasValue ? (object)dg3.Value : null,
                dg4.HasValue ? (object)dg4.Value : null,
                scratch,
                wHdcp,
                entry,
                null,                                        // 30 AVG — not yet computed
                adjAvg > 0 ? (object)adjAvg : null,          // Adjusted AVG from tournament grid
                hdcp,
                bonus,
                (_placedGameIds.Contains(b.GameId) && _bestStandingByMember.TryGetValue(memberNumber, out int ps)) ? (object)ps : null,
                _placedGameIds.Contains(b.GameId)
                    ? ((b.MoneyWon ?? 0) + (b.SidePot ?? 0) > 0 ? (object)((b.MoneyWon ?? 0) + (b.SidePot ?? 0)) : null)
                    : (b.MoneyWon > 0 ? (object)b.MoneyWon : null),  // Earnings (SidePot only on placed entry)
                null                                         // Notes
            );
            currScratch30 += scratch;
            currGames30   += validGames;
            currentDetailRowIndices.Add(rowIdx);
            dgvDetail.Rows[rowIdx].DefaultCellStyle.BackColor = Color.LightBlue;
            // Only highlight the bonus cell on the single entry the bowler cashed with
            // (placed entry), not on every entry for that member.
            if (_cashingGameIds.Contains(b.GameId) && _placedGameIds.Contains(b.GameId))
                dgvDetail.Rows[rowIdx].Cells["colDetailBonus"].Style.BackColor = Color.LightSalmon;
        }

        // Fill in the 30 Entry AVG on current-tournament rows
        double preview30Avg = finalizeCalculationService.Compute30EntryAverage(
            histScratch30, histGames30, currScratch30, currGames30);
        object preview30Val = preview30Avg > 0 ? (object)preview30Avg : null;
        foreach (int ri in currentDetailRowIndices)
            dgvDetail.Rows[ri].Cells["colDetail30Avg"].Value = preview30Val;

        // --- Finalized historical rows (ordered by date descending from DB) ---
        // Group by tournament date, reverse entries within each date so the first
        // entry comes first, and place earnings on that first entry only.
        var historyByDate = history.GroupBy(h => h.TournamentDate.Date).ToList();
        int historyIndex = 0;
        foreach (var dateGroup in historyByDate)
        {
            var entriesForDate = dateGroup.Reverse().ToList();
            for (int j = 0; j < entriesForDate.Count; j++)
            {
                PlayerHistoryViewModel h = entriesForDate[j];
                int wHdcp = h.TotalScore + (h.GamesPlayed * (h.HandiCap + h.Bonus));
                int entry = h.GamesPlayed > 0 ? h.TotalScore / h.GamesPlayed : 0;

                // Earnings appear only on the first (top) entry for each date
                bool isFirstForDate = j == 0;
                decimal dateEarnings = isFirstForDate
                    ? entriesForDate.Sum(e => e.MoneyWon)
                    : 0;

                int rowIdx = dgvDetail.Rows.Add(
                    h.GamesPlayed,
                    h.TournamentDate.ToShortDateString(),
                    h.Game1.HasValue ? (object)h.Game1.Value : null,
                    h.Game2.HasValue ? (object)h.Game2.Value : null,
                    h.Game3.HasValue ? (object)h.Game3.Value : null,
                    h.Game4.HasValue ? (object)h.Game4.Value : null,
                    h.TotalScore,
                    wHdcp,
                    entry,
                    h.trueAVG > 0 ? (object)Math.Round(h.trueAVG, 1) : null,
                    h.AVG > 0 ? (object)h.AVG : null,
                    h.HandiCap,
                    h.Bonus,
                    !string.IsNullOrEmpty(h.PPHG) ? (object)h.PPHG : null,
                    dateEarnings > 0 ? (object)dateEarnings : null,
                    h.Notes
                );

                if (h.MoneyWon > 0)
                    dgvDetail.Rows[rowIdx].Cells["colDetailBonus"].Style.BackColor = Color.LightSalmon;
                // Highlight the 30 AVG cell for entries within the rolling 30-game window
                if (historyIndex < thirtyGameWindow)
                    dgvDetail.Rows[rowIdx].Cells["colDetail30Avg"].Style.BackColor = Color.LightGreen;
                historyIndex++;
            }
        }

        decimal lifetimeEarnings = history.Sum(h => h.MoneyWon);
        UpdateDetailGridHeaders(history.Take(thirtyGameWindow).ToList(), lifetimeEarnings, preview30Avg);

        double leagueAvg = history.FirstOrDefault()?.trueAVG ?? 0;
        lblPlayerInfo.Text = $"Mem#   {memberNumber}        {memberName,-35}AVG   {(int)Math.Round(leagueAvg)}";
    }

    /// <summary>
    /// Updates the column headers of <see cref="dgvDetail"/> to show the sum (or computed
    /// average) of the provided entries — typically the last 30 finalized games.
    /// </summary>
    private void UpdateDetailGridHeaders(List<PlayerHistoryViewModel> last30, decimal lifetimeEarnings, double preview30Avg = 0)
    {
        if (last30.Count == 0)
        {
            dgvDetail.Columns["colDetailGames"].HeaderText    = "Games";
            dgvDetail.Columns["colDetailGame1"].HeaderText    = "Game1";
            dgvDetail.Columns["colDetailGame2"].HeaderText    = "Game2";
            dgvDetail.Columns["colDetailGame3"].HeaderText    = "Game3";
            dgvDetail.Columns["colDetailGame4"].HeaderText    = "Game4";
            dgvDetail.Columns["colDetailScratch"].HeaderText  = "Scratch";
            dgvDetail.Columns["colDetailWHdcp"].HeaderText    = "w/HDCP";
            dgvDetail.Columns["colDetailEntry"].HeaderText    = "Entry";
            dgvDetail.Columns["colDetail30Avg"].HeaderText    = "30 AVG";
            dgvDetail.Columns["colDetailEarnings"].HeaderText = $"Earnings\n({lifetimeEarnings:0.00})";
            return;
        }

        int     totalGames = last30.Sum(h => h.GamesPlayed);
        int     game1Sum   = last30.Sum(h => h.Game1 ?? 0);
        int     game2Sum   = last30.Sum(h => h.Game2 ?? 0);
        int     game3Sum   = last30.Sum(h => h.Game3 ?? 0);
        int     game4Sum   = last30.Sum(h => h.Game4 ?? 0);
        int     scratch    = last30.Sum(h => h.TotalScore);
        int     wHdcpSum   = last30.Sum(h => h.TotalScore + h.GamesPlayed * (h.HandiCap + h.Bonus));
        int     entryAvg   = totalGames > 0 ? scratch / totalGames : 0;
        double  avg30      = totalGames > 0 ? (double)scratch / totalGames : 0;

        dgvDetail.Columns["colDetailGames"].HeaderText    = $"Games\n({totalGames})";
        dgvDetail.Columns["colDetailGame1"].HeaderText    = $"Game1\n({game1Sum})";
        dgvDetail.Columns["colDetailGame2"].HeaderText    = $"Game2\n({game2Sum})";
        dgvDetail.Columns["colDetailGame3"].HeaderText    = $"Game3\n({game3Sum})";
        dgvDetail.Columns["colDetailGame4"].HeaderText    = $"Game4\n({game4Sum})";
        dgvDetail.Columns["colDetailScratch"].HeaderText  = $"Scratch\n({scratch})";
        dgvDetail.Columns["colDetailWHdcp"].HeaderText    = $"w/HDCP\n({wHdcpSum})";
        dgvDetail.Columns["colDetailEntry"].HeaderText    = $"Entry\n({entryAvg})";
        double headerAvg30 = preview30Avg > 0 ? preview30Avg : avg30;
        dgvDetail.Columns["colDetail30Avg"].HeaderText    = $"30 AVG\n({headerAvg30:0.#})";
        dgvDetail.Columns["colDetailEarnings"].HeaderText = $"Earnings\n({lifetimeEarnings:0.00})";
    }
}
