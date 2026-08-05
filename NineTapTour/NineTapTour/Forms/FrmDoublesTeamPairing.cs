using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.Services;
using NineTapTour.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NineTapTour.Forms;

/// <summary>
/// Allows the director to create and manage doubles pairings for a tournament.
/// The director enters a main bowler number and how many partners that bowler has,
/// then fills in each partner member number. A squad dropdown scopes both
/// validation and the pairings grid to a specific squad.
/// </summary>
public class FrmDoublesTeamPairing : Form
{
    private const int BowlerPanelWidth = 340;
    private const int BowlerPanelFixedHeight = 210;

    private readonly Tournament _tournament;
    private readonly IFormFactory _formFactory;
    private readonly ITournamentSession session;
    private readonly IMemberRepository memberRepository;
    private readonly ITournamentRepository tournamentRepository;
    private readonly IDoublesPairingService doublesPairingService;

    // Header
    private Label lblHeader;

    // Input panel (top-docked; grows with partner rows)
    private Panel pnlInput;

    // Squad selector row (y=8)
    private Label lblSquad;
    private ComboBox cboSquad;

    // Bowler + partner-count row (y=40)
    private Label lblBowlerNum;
    private TextBox txtBowlerNumber;
    private Label lblBowlerName;
    private Label lblPartnerCountLabel;
    private TextBox txtPartnerCount;
    private Label lblAutoSaveStatus;
    private Button btnImportExcel;

    // Dynamic partner rows (y=72+)
    private Panel pnlPartners;
    private readonly List<(TextBox NumBox, Label NameLabel)> _partnerControls = [];
    private List<Member> _existingPartnersForBowler = [];
    private bool _populatingPartners = false;
    private bool _suppressBowlerLoad = false;

    // Pairings grid
    private DataGridView dgvPairings;

    // Bowler navigation panel
    private Panel pnlBowlerList;
    private Label lblBowlerList;
    private Panel pnlBowlerNavButtons;
    private Button btnPrevBowler;
    private Button btnNextBowler;
    private ListBox lstBowlers;

    // Summary panel
    private Panel pnlSummary;
    private Label lblTotalTeams;
    private Label lblSquadBreakdown;
    private Label lblDiscrepancies;
    private Button btnFixDiscrepancies;

    // Secondary squad picker (shown when cboSquad = "All Squads")
    private Label lblAddSquad;
    private ComboBox cboAddSquad;

    // Bottom bar
    private Button btnClose;

    // ----------------------------------------------------------------
    // Construction
    // ----------------------------------------------------------------

    public FrmDoublesTeamPairing(Tournament tournament, IFormFactory formFactory, IMemberRepository memberRepository, ITournamentRepository tournamentRepository, ITournamentSession session, IDoublesPairingService doublesPairingService)
    {
        _tournament = tournament;
        _formFactory = formFactory;
        this.session = session;
        this.memberRepository = memberRepository;
        this.tournamentRepository = tournamentRepository;
        this.doublesPairingService = doublesPairingService;
        InitializeControls();
        LoadPairings();
    }

    private void InitializeControls()
    {
        SuspendLayout();

        Text            = "Doubles Pairings";
        Size            = new Size(1100, 640);
        MinimumSize     = new Size(1100, 440);
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.Sizable;

        // --- Header ---
        lblHeader = new Label
        {
            Text      = $"Doubles Pairings \u2014 {_tournament.TourneyNameDate}",
            Font      = new Font("Arial", 12, FontStyle.Bold),
            Dock      = DockStyle.Top,
            Height    = 36,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(8, 0, 0, 0)
        };

        // --- Input panel (top-docked) ---
        pnlInput = new Panel { Dock = DockStyle.Top, Height = 76, Padding = new Padding(8, 0, 8, 4) };

        // Squad row 
        lblSquad = new Label { Text = "Squad:", Location = new Point(8, 12), AutoSize = true };
        cboSquad = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location      = new Point(55, 8),
            Width         = 120
        };
        cboSquad.Items.Add("All Squads");
        for (int s = 1; s <= _tournament.Squads; s++)
            cboSquad.Items.Add($"Squad {s}");
        cboSquad.SelectedIndex = 0;
        cboSquad.SelectedIndexChanged += CboSquad_SelectedIndexChanged;

        // Member + partner-count row
        lblBowlerNum = new Label { Text = "Member #:", Location = new Point(8, 44), AutoSize = true };
        txtBowlerNumber = new TextBox { Location = new Point(75, 40), Width = 65 };
        lblBowlerName   = new Label  { Location = new Point(140, 44), Width = 170, AutoSize = false, Text = string.Empty };

        lblPartnerCountLabel = new Label { Text = "# of Partners:", Location = new Point(318, 44), AutoSize = true };
        txtPartnerCount      = new TextBox { Location = new Point(408, 40), Width = 35 };

        lblAutoSaveStatus = new Label
        {
            Location  = new Point(200, 12),
            Width     = 250,
            AutoSize  = false,
            Text      = string.Empty
        };

        btnImportExcel = new Button
        {
            Text = "Import Excel",
            Size = new Size(96, 26),
            Location = new Point(548, 0)
        };
        btnImportExcel.Click += BtnImportExcel_Click;

        var toolTip = new ToolTip();
        toolTip.SetToolTip(btnImportExcel, "Only .xlsx (Excel 2007+) format is supported. .xls files cannot be imported.");

        // Secondary squad picker — visible only when cboSquad = "All Squads"
        lblAddSquad = new Label { Text = "for Squad:", Location = new Point(552, 44), AutoSize = true, Visible = false };
        cboAddSquad = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location      = new Point(625, 40),
            Width         = 80,
            Visible       = false
        };
        for (int s = 1; s <= _tournament.Squads; s++)
            cboAddSquad.Items.Add($"Squad {s}");
        cboAddSquad.SelectedIndex = 0;

        // Partner rows container (y=72)
        pnlPartners = new Panel { Location = new Point(0, 72), Width = pnlInput.Width, Height = 0 };
        pnlInput.Resize += (s2, e2) => pnlPartners.Width = pnlInput.Width;

        // Wire events
        txtBowlerNumber.Leave   += (s2, e2) => LookupBowlerAndPopulate();
        txtBowlerNumber.KeyDown += TxtBowlerNumber_KeyDown;
        txtPartnerCount.KeyDown += TxtPartnerCount_KeyDown;
        txtPartnerCount.Leave   += (s2, e2) => { RebuildPartnerControls(); TrySavePlan(); };

        pnlInput.Controls.Add(lblSquad);
        pnlInput.Controls.Add(cboSquad);
        pnlInput.Controls.Add(lblBowlerNum);
        pnlInput.Controls.Add(txtBowlerNumber);
        pnlInput.Controls.Add(lblBowlerName);
        pnlInput.Controls.Add(lblPartnerCountLabel);
        pnlInput.Controls.Add(txtPartnerCount);
        pnlInput.Controls.Add(lblAutoSaveStatus);
        pnlInput.Controls.Add(btnImportExcel);
        pnlInput.Controls.Add(lblAddSquad);
        pnlInput.Controls.Add(cboAddSquad);
        pnlInput.Controls.Add(pnlPartners);

        // --- Summary panel ---
        pnlSummary = new Panel { Dock = DockStyle.Top, Height = 52, Padding = new Padding(8, 4, 8, 4) };
        lblTotalTeams = new Label { Location = new Point(8, 6), AutoSize = true, Text = "Total Teams: 0" };
        lblSquadBreakdown = new Label { Location = new Point(8, 24), AutoSize = true, Text = "By Squad: none" };
        lblDiscrepancies = new Label { Location = new Point(360, 6), Size = new Size(245, 38), AutoSize = false, Text = "Discrepancies: none" };
        btnFixDiscrepancies = new Button
        {
            Text     = "Fix Issues...",
            Size     = new Size(110, 26),
            Location = new Point(612, 12),
            Enabled  = false
        };
        btnFixDiscrepancies.Click += BtnFixDiscrepancies_Click;
        pnlSummary.Controls.Add(lblTotalTeams);
        pnlSummary.Controls.Add(lblSquadBreakdown);
        pnlSummary.Controls.Add(lblDiscrepancies);
        pnlSummary.Controls.Add(btnFixDiscrepancies);

        // --- Bowler list panel ---
        pnlBowlerList = new Panel
        {
            Dock = DockStyle.None,
            Width = BowlerPanelWidth,
            Height = BowlerPanelFixedHeight,
            MinimumSize = new Size(BowlerPanelWidth, BowlerPanelFixedHeight),
            MaximumSize = new Size(BowlerPanelWidth, BowlerPanelFixedHeight),
            Padding = new Padding(6)
        };
        lblBowlerList = new Label
        {
            Text = "Imported Bowlers",
            Dock = DockStyle.Top,
            Height = 20,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlBowlerNavButtons = new Panel
        {
            Dock = DockStyle.Top,
            Height = 30
        };
        btnPrevBowler = new Button
        {
            Text = "Previous",
            Size = new Size(92, 24),
            Location = new Point(0, 3)
        };
        btnPrevBowler.Click += BtnPrevBowler_Click;

        btnNextBowler = new Button
        {
            Text = "Next",
            Size = new Size(92, 24),
            Location = new Point(98, 3)
        };
        btnNextBowler.Click += BtnNextBowler_Click;

        pnlBowlerNavButtons.Controls.Add(btnPrevBowler);
        pnlBowlerNavButtons.Controls.Add(btnNextBowler);

        lstBowlers = new ListBox
        {
            Dock = DockStyle.Fill,
            HorizontalScrollbar = true
        };
        lstBowlers.SelectedIndexChanged += LstBowlers_SelectedIndexChanged;
        pnlBowlerList.Controls.Add(lstBowlers);
        pnlBowlerList.Controls.Add(pnlBowlerNavButtons);
        pnlBowlerList.Controls.Add(lblBowlerList);

        // --- Spacer before pairings grid ---
        Panel pnlSpacer = new Panel { Dock = DockStyle.Top, Height = 60 };

        // --- Pairings grid ---
        dgvPairings = new DataGridView
        {
            Dock                  = DockStyle.Fill,
            ReadOnly              = true,
            AllowUserToAddRows    = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible     = false,
            SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill
        };
        dgvPairings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTeamId",   HeaderText = "ID",            Visible    = false });
        dgvPairings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMem1Id",   HeaderText = "Mem1Id",        Visible    = false });
        dgvPairings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMem2Id",   HeaderText = "Mem2Id",        Visible    = false });
        dgvPairings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSquad",    HeaderText = "Squad",         FillWeight = 12 });
        dgvPairings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMem1Num",  HeaderText = "Bowler 1 #",    FillWeight = 15 });
        dgvPairings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMem1Name", HeaderText = "Bowler 1 Name", FillWeight = 34 });
        dgvPairings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMem2Num",  HeaderText = "Bowler 2 #",    FillWeight = 15 });
        dgvPairings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMem2Name", HeaderText = "Bowler 2 Name", FillWeight = 34 });
        var btnCol = new DataGridViewButtonColumn
        {
            Name                        = "colRemove",
            HeaderText                  = string.Empty,
            Text                        = "Remove",
            UseColumnTextForButtonValue = true,
            FillWeight                  = 20
        };
        dgvPairings.Columns.Add(btnCol);
        dgvPairings.CellClick += DgvPairings_CellClick;

        // --- Bottom bar ---
        var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 40 };
        btnClose = new Button { Text = "Close", Size = new Size(88, 26) };
        btnClose.Click += (s2, e2) => Close();
        pnlBottom.Controls.Add(btnClose);
        pnlBottom.Resize += (s2, e2) =>
            btnClose.Location = new Point(pnlBottom.Width - btnClose.Width - 8,
                                         (pnlBottom.Height - btnClose.Height) / 2);

        Controls.Add(dgvPairings);
        Controls.Add(pnlSpacer);
        Controls.Add(pnlBowlerList);
        Controls.Add(pnlBottom);
        Controls.Add(pnlSummary);
        Controls.Add(pnlInput);
        Controls.Add(lblHeader);

        // Initialise secondary squad picker visibility to match initial cboSquad state
        bool startAllSquads = cboSquad.SelectedIndex == 0;
        lblAddSquad.Visible = startAllSquads;
        cboAddSquad.Visible = startAllSquads;

        PositionBowlerPanel();
        Resize += (s2, e2) => PositionBowlerPanel();

        PopulateBowlersList();

        ResumeLayout(false);
    }

    private void PositionBowlerPanel()
    {
        pnlBowlerList.Location = new Point(740, 2);
        pnlBowlerList.BringToFront();
    }

    // ----------------------------------------------------------------
    // Dynamic partner rows
    // ----------------------------------------------------------------

    private void RebuildPartnerControls()
    {
        pnlPartners.Controls.Clear();
        _partnerControls.Clear();

        if (!int.TryParse(txtPartnerCount.Text.Trim(), out int count) || count < 0 || count > 20)
        {
            pnlPartners.Height = 0;
            pnlInput.Height    = 76;
            return;
        }

        if (count == 0)
        {
            pnlPartners.Height = 0;
            pnlInput.Height = 76;
            return;
        }

        for (int i = 0; i < count; i++)
        {
            int capturedIndex = i;
            int y = i * 28 + 4;

            var lbl     = new Label   { Text = $"Partner {i + 1} #:", Location = new Point(8,   y + 4), AutoSize = true };
            var numBox  = new TextBox { Location = new Point(88, y), Width = 65 };
            var nameLbl = new Label   { Location = new Point(160, y + 4), Width = 200, AutoSize = false, Text = string.Empty };

            // Pre-fill existing partners
            if (i < _existingPartnersForBowler.Count)
            {
                var p = _existingPartnersForBowler[i];
                numBox.Text   = p.Number.ToString();
                nameLbl.Text  = $"{p.FirstName} {p.LastName}";
                nameLbl.ForeColor = Color.Gray;   // visual hint: already paired
            }

            numBox.Leave   += (s2, e2) => { LookupMemberName((TextBox)s2, nameLbl); TrySavePlan(); TrySaveClaim(capturedIndex); };
            numBox.KeyDown += (s2, e2) => TxtPartnerBox_KeyDown((TextBox)s2, capturedIndex, e2);

            pnlPartners.Controls.Add(lbl);
            pnlPartners.Controls.Add(numBox);
            pnlPartners.Controls.Add(nameLbl);
            _partnerControls.Add((numBox, nameLbl));
        }

        pnlPartners.Height  = count * 28 + 8;
        pnlInput.Height     = 76 + pnlPartners.Height;

        // Focus the first slot that has no pre-filled value
        int firstEmpty = _existingPartnersForBowler.Count;

        // Delays the focus so the other controls can be built and avoid a cascade of focus/leave events
        if (firstEmpty < _partnerControls.Count)
        {
            var target = _partnerControls[firstEmpty].NumBox;

            BeginInvoke(new Action(() =>
            {
                if (!target.IsDisposed && target.IsHandleCreated)
                    target.Focus();
            }));
        }

    }

    // ----------------------------------------------------------------
    // Keyboard navigation
    // ----------------------------------------------------------------

    private void CboSquad_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool allSquads = cboSquad.SelectedIndex == 0;
        lblAddSquad.Visible = allSquads;
        cboAddSquad.Visible = allSquads;
        LoadPairings();
    }

    private void TxtBowlerNumber_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
        {
            LookupBowlerAndPopulate();
            e.SuppressKeyPress = true;
        }
    }

    private void TxtPartnerCount_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
        {
            RebuildPartnerControls();
            TrySavePlan();
            e.SuppressKeyPress = true;
        }
    }

    private void TxtPartnerBox_KeyDown(TextBox sender, int index, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
        {
            LookupMemberName(sender, _partnerControls[index].NameLabel);
            TrySaveClaim(index);
            if (index + 1 < _partnerControls.Count)
                _partnerControls[index + 1].NumBox.Focus();
            else
                lblAutoSaveStatus.Focus();
            e.SuppressKeyPress = true;
        }
    }

    // ----------------------------------------------------------------
    // Member name look-up
    // ----------------------------------------------------------------

    /// <summary>
    /// Looks up the bowler name, then auto-populates existing partners
    /// in the target squad so the director can see who is already paired.
    /// </summary>
    private void LookupBowlerAndPopulate()
    {
        if (_populatingPartners) return;
        _populatingPartners = true;
        try
        {
        LookupMemberName(txtBowlerNumber, lblBowlerName);
        _existingPartnersForBowler.Clear();

        if (!int.TryParse(txtBowlerNumber.Text.Trim(), out int mainNum))
            return;
        int mainId = memberRepository.GetMemberIdByNumber(mainNum);
        if (mainId == 0) return;

        int targetSquad = GetTargetSquad();
        if (targetSquad == 0) return;

        // Show only partners this bowler has explicitly claimed.
        DoublesBowlerPlanState state = doublesPairingService.GetBowlerPlanState(_tournament.Id, mainId, targetSquad);
        _existingPartnersForBowler = state.ClaimedPartners;

        // Always show exactly the planned partner count for the selected bowler.
        // This prevents stale higher counts when navigating between bowlers.
        txtPartnerCount.Text = state.ExpectedPartnerCount.ToString();
        RebuildPartnerControls();
        } // end try
        finally { _populatingPartners = false; }
    }

    private void LookupMemberName(TextBox txt, Label lbl)
    {
        lbl.ForeColor = SystemColors.ControlText;
        if (!int.TryParse(txt.Text.Trim(), out int num))
        {
            lbl.Text = string.Empty;
            return;
        }
        Member m = memberRepository.GetMember(num);
        if (m != null && m.Id > 0)
        {
            lbl.Text = $"{m.FirstName} {m.LastName}";
        }
        else
        {
            lbl.Text      = "Not found";
            lbl.ForeColor = Color.Red;
        }
    }

    // ----------------------------------------------------------------
    // Add pairs
    // ----------------------------------------------------------------

    /// <summary>
    /// Returns the 1-based target squad for adding pairs.
    /// Uses cboSquad when a specific squad is selected,
    /// otherwise falls back to the inline cboAddSquad picker.
    /// </summary>
    private int GetTargetSquad()
    {
        if (cboSquad.SelectedIndex > 0)
            return cboSquad.SelectedIndex;   // SelectedIndex == squad number (1-based)
        if (cboAddSquad.SelectedIndex >= 0)
            return cboAddSquad.SelectedIndex + 1;
        return 0;
    }

    // ----------------------------------------------------------------
    // Auto-save helpers (replace former BtnAddPairs_Click)
    // ----------------------------------------------------------------

    private void TrySavePlan()
    {
        if (_populatingPartners) return;
        DoublesPlanSaveResult result = doublesPairingService.SavePartnerPlan(
            _tournament.Id, txtBowlerNumber.Text, GetTargetSquad(), txtPartnerCount.Text);
        if (result.Saved)
            ShowAutoSaveStatus(result.StatusMessage);
    }

    private void TrySaveClaim(int capturedIndex)
    {
        if (_populatingPartners) return;
        if (capturedIndex >= _partnerControls.Count) return;
        var (numBox, _) = _partnerControls[capturedIndex];

        DoublesClaimSaveResult result = doublesPairingService.SavePartnerClaim(
            _tournament, txtBowlerNumber.Text, numBox.Text, GetTargetSquad());

        if (result.Persisted)
            LoadPairings();

        // A null message means a silent no-op (empty input, or the claim already
        // existed because a pre-filled box was tabbed through)
        if (result.StatusMessage != null)
            ShowAutoSaveStatus(result.StatusMessage, result.IsError);
    }

    private void ShowAutoSaveStatus(string message, bool error = false)
    {
        lblAutoSaveStatus.Text      = message;
        lblAutoSaveStatus.ForeColor = error ? Color.DarkRed : Color.DarkGreen;
    }

    private void BtnImportExcel_Click(object sender, EventArgs e)
    {
        using var open = new OpenFileDialog
        {
            Title = "Import Doubles Bowlers",
            Filter = "Excel Files (*.xlsx)|*.xlsx",
            Multiselect = false
        };

        if (open.ShowDialog(this) != DialogResult.OK)
            return;

        DoublesImportSummary summary = doublesPairingService.ImportBowlersAndExpectedCounts(
            _tournament.Id, _tournament.Squads, open.FileName);

        string details = summary.Errors.Count > 0
            ? "\n\nIssues:\n- " + string.Join("\n- ", summary.Errors.Take(25))
            : string.Empty;

        if (summary.Errors.Count > 25)
            details += $"\n- ...and {summary.Errors.Count - 25} more.";

        string diffSection = string.Empty;
        if (summary.RemovedFromTournament.Count > 0 || summary.RemovedFromSquad.Count > 0 || summary.PartnerCountChanged.Count > 0)
        {
            var diffLines = new List<string>();
            if (summary.RemovedFromTournament.Count > 0)
                diffLines.Add("Removed from tournament:\n  \u2022 " + string.Join("\n  \u2022 ", summary.RemovedFromTournament));
            if (summary.RemovedFromSquad.Count > 0)
                diffLines.Add("Removed from squad:\n  \u2022 " + string.Join("\n  \u2022 ", summary.RemovedFromSquad));
            if (summary.PartnerCountChanged.Count > 0)
                diffLines.Add("Partner count changed:\n  \u2022 " + string.Join("\n  \u2022 ", summary.PartnerCountChanged));
            diffSection = "\n\nChanges vs. previous import:\n" + string.Join("\n", diffLines);
        }

        MessageBox.Show(
            $"Import complete.\nRows processed: {summary.RowsProcessed}\nPlans added/updated: {summary.PlansUpserted}\nParticipants created: {summary.ParticipantsCreated}\nRows skipped: {summary.RowsSkipped}{details}{diffSection}",
            "Doubles Import",
            MessageBoxButtons.OK,
            summary.Errors.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

        RefreshOwnerParticipants();
        LoadPairings();
        PopulateBowlersList(selectFirst: true);
    }

    private void RefreshOwnerParticipants()
    {
        session.Participants = tournamentRepository.GetTournamentMemberList(_tournament);
        if (Owner is FrmMemberScores memberScoresForm)
            memberScoresForm.RefreshParticipantsAfterDoublesImport();
    }

    private void PopulateBowlersList(bool selectFirst = false)
    {
        // Track both member and squad so multi-squad bowlers restore to the correct squad entry.
        (int MemberId, int Squad) previousKey = lstBowlers.SelectedItem is BowlerListItem selected
            ? (selected.MemberId, selected.Squad) : (0, 0);

        List<DoublesBowlerRosterEntry> roster = doublesPairingService.GetBowlerRoster(_tournament.Id, cboSquad.SelectedIndex);

        List<BowlerListItem> items = roster
            .Select(r => new BowlerListItem
            {
                Squad = r.Squad,
                MemberId = r.MemberId,
                MemberNumber = r.MemberNumber,
                Display = $"S{r.Squad}  #{r.MemberNumber}  {r.FirstName} {r.LastName}  (Planned {r.PlannedCount}, Entered {r.EnteredCount})"
            })
            .ToList();

        _suppressBowlerLoad = true;
        try
        {
            lstBowlers.BeginUpdate();
            lstBowlers.DataSource = null;
            lstBowlers.DataSource = items;
            lstBowlers.DisplayMember = nameof(BowlerListItem.Display);
            lstBowlers.EndUpdate();

            if (items.Count == 0)
            {
                UpdateBowlerNavButtons();
                return;
            }

            if (selectFirst)
            {
                _suppressBowlerLoad = false; // allow LookupBowlerAndPopulate for this deliberate selection
                lstBowlers.SelectedIndex = 0;
                UpdateBowlerNavButtons();
                return;
            }

            // Prefer exact (member + squad) match; fall back to first squad if not found.
            int restoreIndex = items.FindIndex(i => i.MemberId == previousKey.MemberId && i.Squad == previousKey.Squad);
            if (restoreIndex < 0)
                restoreIndex = items.FindIndex(i => i.MemberId == previousKey.MemberId);
            if (restoreIndex >= 0)
                lstBowlers.SelectedIndex = restoreIndex;
        }
        finally
        {
            _suppressBowlerLoad = false;
        }

        UpdateBowlerNavButtons();
    }

    private void LstBowlers_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_suppressBowlerLoad) return;
        if (lstBowlers.SelectedItem is not BowlerListItem item)
            return;

        if (cboSquad.SelectedIndex == 0)
            cboAddSquad.SelectedIndex = Math.Max(0, item.Squad - 1);

        txtBowlerNumber.Text = item.MemberNumber.ToString();
        LookupBowlerAndPopulate();
        UpdateBowlerNavButtons();
    }

    private void BtnPrevBowler_Click(object sender, EventArgs e)
    {
        if (lstBowlers.Items.Count == 0)
            return;

        if (lstBowlers.SelectedIndex <= 0)
            lstBowlers.SelectedIndex = lstBowlers.Items.Count - 1;
        else
            lstBowlers.SelectedIndex--;
    }

    private void BtnNextBowler_Click(object sender, EventArgs e)
    {
        if (lstBowlers.Items.Count == 0)
            return;

        if (lstBowlers.SelectedIndex < 0 || lstBowlers.SelectedIndex >= lstBowlers.Items.Count - 1)
            lstBowlers.SelectedIndex = 0;
        else
            lstBowlers.SelectedIndex++;
    }

    private void UpdateBowlerNavButtons()
    {
        bool hasBowlers = lstBowlers.Items.Count > 0;
        btnPrevBowler.Enabled = hasBowlers;
        btnNextBowler.Enabled = hasBowlers;
    }

    private sealed class BowlerListItem
    {
        public int Squad { get; set; }
        public int MemberId { get; set; }
        public int MemberNumber { get; set; }
        public string Display { get; set; }
    }

    private void UpdateSummaryLabels(DoublesPairingsView view)
    {
        lblTotalTeams.Text = $"Total Teams (Tournament): {view.TotalTeamCount}";

        var bySquad = view.TeamsBySquad.Select(g => $"S{g.Squad}: {g.Count}");

        lblSquadBreakdown.Text = "By Squad: " + (bySquad.Any() ? string.Join(" | ", bySquad) : "none");

        lblDiscrepancies.Text = $"Discrepancies: count mismatch {view.CountMismatches}, missing reciprocal {view.MissingReciprocals}";
        lblDiscrepancies.ForeColor = (view.CountMismatches > 0 || view.MissingReciprocals > 0) ? Color.DarkRed : Color.DarkGreen;
        btnFixDiscrepancies.Enabled = (view.CountMismatches > 0 || view.MissingReciprocals > 0);
    }

    private void BtnFixDiscrepancies_Click(object sender, EventArgs e)
    {
        using var dlg = _formFactory.Create<FrmDoublesDiscrepancies>(_tournament);
        dlg.ShowDialog(this);
        LoadPairings();
    }

    // ----------------------------------------------------------------
    // Remove pair
    // ----------------------------------------------------------------

    private void DgvPairings_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != dgvPairings.Columns["colRemove"].Index)
            return;

        var row    = dgvPairings.Rows[e.RowIndex];
        int teamId = (int)row.Cells["colTeamId"].Value;
        int mem1Id = (int)row.Cells["colMem1Id"].Value;
        int mem2Id = (int)row.Cells["colMem2Id"].Value;
        int squad  = (int)row.Cells["colSquad"].Value;

        bool hasClaims = doublesPairingService.PairHasClaims(_tournament.Id, mem1Id, mem2Id, squad);

        string claimNote = hasClaims
            ? "\n\nThis will also remove the associated partner claims."
            : string.Empty;

        if (MessageBox.Show($"Remove this pairing?{claimNote}", "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            doublesPairingService.RemovePairing(_tournament.Id, teamId, mem1Id, mem2Id, squad, hasClaims);
            LoadPairings();
        }
    }

    // ----------------------------------------------------------------
    // Grid population
    // ----------------------------------------------------------------

    private void LoadPairings()
    {
        int selectedSquad = cboSquad.SelectedIndex;   // 0=All, 1=Squad1, etc.

        dgvPairings.Rows.Clear();
        DoublesPairingsView view = doublesPairingService.GetPairingsView(_tournament.Id, selectedSquad);

        foreach (var team in view.Teams)
        {
            dgvPairings.Rows.Add(
                team.Id,
                team.Member1.Id,
                team.Member2.Id,
                team.Squad,
                team.Member1.Number,
                $"{team.Member1.FirstName} {team.Member1.LastName}",
                team.Member2.Number,
                $"{team.Member2.FirstName} {team.Member2.LastName}"
            );
        }

        UpdateSummaryLabels(view);
        PopulateBowlersList();
    }
}
