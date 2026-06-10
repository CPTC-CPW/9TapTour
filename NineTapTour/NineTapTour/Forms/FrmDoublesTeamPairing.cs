using NineTapTour.Database;
using NineTapTour.Models;
using ClosedXML.Excel;
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
    private const int BowlerPanelWidth = 260;
    private const int BowlerPanelFixedHeight = 210;

    private readonly Tournament _tournament;

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

    public FrmDoublesTeamPairing(Tournament tournament)
    {
        _tournament = tournament;
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
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
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
            Dock = DockStyle.Fill
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
        int mainId = MemberDB.GetMemberIdByNumber(mainNum);
        if (mainId == 0) return;

        int targetSquad = GetTargetSquad();
        if (targetSquad == 0) return;

        // Show only partners this bowler has explicitly claimed.
        List<DoublesPartnerClaim> allClaims = DoublesPartnerClaimDB.GetClaimsByTournament(_tournament.Id);
        _existingPartnersForBowler = allClaims
            .Where(c => c.Squad == targetSquad && c.SourceMember.Id == mainId)
            .Select(c => c.PartnerMember)
            .ToList();

        int expectedCount = DoublesPartnerPlanDB.GetExpectedPartnerCount(_tournament.Id, mainId, targetSquad);

        // Always show exactly the planned partner count for the selected bowler.
        // This prevents stale higher counts when navigating between bowlers.
        txtPartnerCount.Text = expectedCount.ToString();
        RebuildPartnerControls();
        } // end try
        finally { _populatingPartners = false; }
    }

    private static void LookupMemberName(TextBox txt, Label lbl)
    {
        lbl.ForeColor = SystemColors.ControlText;
        if (!int.TryParse(txt.Text.Trim(), out int num))
        {
            lbl.Text = string.Empty;
            return;
        }
        Member m = MemberDB.GetMember(num);
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
        if (!int.TryParse(txtBowlerNumber.Text.Trim(), out int mainNum)) return;
        int mainId = MemberDB.GetMemberIdByNumber(mainNum);
        if (mainId == 0) return;
        int targetSquad = GetTargetSquad();
        if (targetSquad == 0) return;
        if (!int.TryParse(txtPartnerCount.Text.Trim(), out int expectedCount) || expectedCount < 0) return;
        DoublesPartnerPlanDB.UpsertPlan(_tournament.Id, mainId, targetSquad, expectedCount);
        ShowAutoSaveStatus($"Plan saved: {expectedCount} partner(s) for #{mainNum}");
    }

    private void TrySaveClaim(int capturedIndex)
    {
        if (_populatingPartners) return;
        if (capturedIndex >= _partnerControls.Count) return;
        var (numBox, _) = _partnerControls[capturedIndex];
        string partnerText = numBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(partnerText)) return;

        if (!int.TryParse(txtBowlerNumber.Text.Trim(), out int mainNum))
            { ShowAutoSaveStatus("Enter a valid bowler number first.", error: true); return; }
        int mainId = MemberDB.GetMemberIdByNumber(mainNum);
        if (mainId == 0)
            { ShowAutoSaveStatus($"Bowler #{mainNum} not found.", error: true); return; }
        int targetSquad = GetTargetSquad();
        if (targetSquad == 0)
            { ShowAutoSaveStatus("Select a squad first.", error: true); return; }
        if (!int.TryParse(partnerText, out int partnerNum))
            { ShowAutoSaveStatus($"'{partnerText}' is not a valid member number.", error: true); return; }
        int partnerId = MemberDB.GetMemberIdByNumber(partnerNum);
        if (partnerId == 0)
            { ShowAutoSaveStatus($"#{partnerNum} not found.", error: true); return; }
        if (partnerId == mainId)
            { ShowAutoSaveStatus($"#{partnerNum} cannot be paired with themselves.", error: true); return; }

        HashSet<int> validIds = GetValidMemberIds(targetSquad);
        if (!validIds.Contains(mainId))
            { ShowAutoSaveStatus($"#{mainNum} not in Squad {targetSquad}.", error: true); return; }
        if (!validIds.Contains(partnerId))
            { ShowAutoSaveStatus($"#{partnerNum} not in Squad {targetSquad}.", error: true); return; }

        bool claimAdded = DoublesPartnerClaimDB.AddClaim(_tournament.Id, mainId, partnerId, targetSquad);
        DoublesTeamDB.AddTeam(_tournament.Id, mainId, partnerId, targetSquad);
        LoadPairings();

        if (claimAdded)
            ShowAutoSaveStatus($"#{mainNum} & #{partnerNum} saved (Squad {targetSquad}).");
        // If claim already existed (e.g. pre-filled box was tabbed through), show nothing
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

        // Snapshot current state for re-import diff
        List<(int MemberNumber, int Squad)> prevParticipants;
        Dictionary<(int MemberNumber, int Squad), int> prevPlans;
        using (var db = new NineTapDb())
        {
            prevParticipants = db.Participants
                .Where(p => p.Tournament.Id == _tournament.Id)
                .Select(p => new { p.Member.Number, p.Squad })
                .AsEnumerable()
                .Select(x => (x.Number, x.Squad))
                .ToList();
        }
        var existingPlansList = DoublesPartnerPlanDB.GetPlansByTournament(_tournament.Id);
        prevPlans = existingPlansList.ToDictionary(
            p => (p.Member.Number, p.Squad),
            p => p.ExpectedPartnerCount);

        bool isReimport = prevParticipants.Count > 0;
        var summary = ImportBowlersAndExpectedCounts(open.FileName);

        // Compute re-import diff
        if (isReimport)
        {
            var processedSet = summary.ProcessedEntries;
            var allProcessedNums = new HashSet<int>(processedSet.Select(entry => entry.MemberNumber));

            foreach (var (memberNumber, squad) in prevParticipants)
            {
                if (!processedSet.Contains((memberNumber, squad)))
                {
                    if (!allProcessedNums.Contains(memberNumber))
                        summary.RemovedFromTournament.Add($"#{memberNumber} removed from tournament entirely");
                    else
                        summary.RemovedFromSquad.Add($"#{memberNumber} no longer in Squad {squad}");
                }
            }

            var updatedPlans = DoublesPartnerPlanDB.GetPlansByTournament(_tournament.Id);
            var updatedPlanDict = updatedPlans.ToDictionary(
                p => (p.Member.Number, p.Squad),
                p => p.ExpectedPartnerCount);

            foreach (var kvp in prevPlans)
            {
                if (updatedPlanDict.TryGetValue(kvp.Key, out int newCount) && newCount != kvp.Value)
                    summary.PartnerCountChanged.Add($"#{kvp.Key.MemberNumber} (Squad {kvp.Key.Squad}): {kvp.Value} \u2192 {newCount} partners");
            }
        }

        string details = summary.Errors.Count > 0
            ? "\n\nIssues:\n- " + string.Join("\n- ", summary.Errors.Take(25))
            : string.Empty;

        if (summary.Errors.Count > 25)
            details += $"\n- ...and {summary.Errors.Count - 25} more.";

        string diffSection = string.Empty;
        if (isReimport && (summary.RemovedFromTournament.Count > 0 || summary.RemovedFromSquad.Count > 0 || summary.PartnerCountChanged.Count > 0))
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
        FrmMemberScoresHelpers.overallListOfParticipants = TournamentDB.GetTournamentMemberList(_tournament);
        if (Owner is FrmMemberScores memberScoresForm)
            memberScoresForm.RefreshParticipantsAfterDoublesImport();
    }

    private ImportSummary ImportBowlersAndExpectedCounts(string filePath)
    {
        var summary = new ImportSummary();

        using var workbook = new XLWorkbook(filePath);
        foreach (var ws in workbook.Worksheets)
        {
            if (!TryParseSquadSheetName(ws.Name, out int squad))
                continue;

            if (squad < 1 || squad > _tournament.Squads)
            {
                summary.Errors.Add($"Sheet '{ws.Name}': squad is out of range for this tournament.");
                continue;
            }

            int row = 9;
            while (!ws.Cell(row, 2).IsEmpty())
            {
                summary.RowsProcessed++;

                if (ws.Cell(row, 3).IsEmpty())
                {
                    summary.RowsSkipped++;
                    row++;
                    continue;
                }

                if (!TryReadIntCell(ws, row, 3, out int memberNumber))
                {
                    summary.RowsSkipped++;
                    summary.Errors.Add($"Sheet '{ws.Name}', row {row}: invalid member number in column C.");
                    row++;
                    continue;
                }

                if (!TryReadIntCell(ws, row, 10, out int expectedCount))
                {
                    summary.RowsSkipped++;
                    summary.Errors.Add($"Sheet '{ws.Name}', row {row}: invalid partner count in column J.");
                    row++;
                    continue;
                }

                if (expectedCount < 0)
                {
                    summary.RowsSkipped++;
                    summary.Errors.Add($"Sheet '{ws.Name}', row {row}: partner count cannot be negative.");
                    row++;
                    continue;
                }

                int memberId = MemberDB.GetMemberIdByNumber(memberNumber);
                if (memberId == 0)
                {
                    summary.RowsSkipped++;
                    summary.Errors.Add($"Sheet '{ws.Name}', row {row}: member #{memberNumber} not found.");
                    row++;
                    continue;
                }

                bool participantAlreadyExisted = ParticipantExists(memberId, squad);
                bool ensured = ParticipantsDB.EnsureParticipantExists(_tournament.Id, memberId, squad);
                if (!ensured)
                {
                    summary.RowsSkipped++;
                    summary.Errors.Add($"Sheet '{ws.Name}', row {row}: could not create participant for #{memberNumber}.");
                    row++;
                    continue;
                }

                if (!participantAlreadyExisted)
                    summary.ParticipantsCreated++;

                DoublesPartnerPlanDB.UpsertPlan(_tournament.Id, memberId, squad, expectedCount);
                summary.PlansUpserted++;
                summary.ProcessedEntries.Add((memberNumber, squad));

                row++;
            }
        }

        return summary;
    }

    private bool ParticipantExists(int memberId, int squad)
    {
        using var db = new NineTapDb();
        return db.Participants.Any(p =>
            p.Tournament.Id == _tournament.Id &&
            p.Member.Id == memberId &&
            p.Squad == squad);
    }

    private static bool TryReadIntCell(IXLWorksheet ws, int row, int column, out int value)
    {
        value = 0;
        var cell = ws.Cell(row, column);
        if (cell.IsEmpty())
            return false;

        if (cell.TryGetValue<int>(out int asInt))
        {
            value = asInt;
            return true;
        }

        if (cell.TryGetValue<double>(out double asDouble))
        {
            value = Convert.ToInt32(Math.Round(asDouble));
            return true;
        }

        return int.TryParse(cell.GetString().Trim(), out value);
    }

    private static bool TryParseSquadSheetName(string sheetName, out int squad)
    {
        squad = 0;
        if (string.IsNullOrWhiteSpace(sheetName))
            return false;

        string normalized = sheetName.Trim();
        if (!normalized.StartsWith("Squad ", StringComparison.OrdinalIgnoreCase))
            return false;

        return int.TryParse(normalized.Substring(6).Trim(), out squad);
    }

    private void PopulateBowlersList(bool selectFirst = false)
    {
        int previousMemberId = lstBowlers.SelectedItem is BowlerListItem selected ? selected.MemberId : 0;

        using var db = new NineTapDb();
        var participants = db.Participants
            .Where(p => p.Tournament.Id == _tournament.Id)
            .Select(p => new
            {
                p.Squad,
                MemberId = p.Member.Id,
                p.Member.Number,
                p.Member.FirstName,
                p.Member.LastName
            })
            .ToList();

        if (cboSquad.SelectedIndex > 0)
            participants = participants.Where(p => p.Squad == cboSquad.SelectedIndex).ToList();

        var plans = DoublesPartnerPlanDB.GetPlansByTournament(_tournament.Id);
        var claims = DoublesPartnerClaimDB.GetClaimsByTournament(_tournament.Id);

        var items = participants
            .OrderBy(p => p.Squad)
            .ThenBy(p => p.Number)
            .Select(p => new BowlerListItem
            {
                Squad = p.Squad,
                MemberId = p.MemberId,
                MemberNumber = p.Number,
                Display = $"S{p.Squad}  #{p.Number}  {p.FirstName} {p.LastName}  (Planned {plans.FirstOrDefault(x => x.Squad == p.Squad && x.Member.Id == p.MemberId)?.ExpectedPartnerCount ?? 0}, Entered {claims.Count(c => c.Squad == p.Squad && c.SourceMember.Id == p.MemberId)})"
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

            int restoreIndex = items.FindIndex(i => i.MemberId == previousMemberId);
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

    private void UpdateSummaryLabels(List<DoublesTeam> teams)
    {
        lblTotalTeams.Text = $"Total Teams (Tournament): {teams.Count}";

        var bySquad = teams
            .GroupBy(t => t.Squad)
            .OrderBy(g => g.Key)
            .Select(g => $"S{g.Key}: {g.Count()}");

        lblSquadBreakdown.Text = "By Squad: " + (bySquad.Any() ? string.Join(" | ", bySquad) : "none");

        var plans = DoublesPartnerPlanDB.GetPlansByTournament(_tournament.Id);
        var claims = DoublesPartnerClaimDB.GetClaimsByTournament(_tournament.Id);

        int countMismatches = plans.Count(p =>
            claims.Count(c => c.Squad == p.Squad && c.SourceMember.Id == p.Member.Id) != p.ExpectedPartnerCount);

        int reciprocalMissing = claims.Count(c =>
            !claims.Any(r =>
                r.Squad == c.Squad &&
                r.SourceMember.Id == c.PartnerMember.Id &&
                r.PartnerMember.Id == c.SourceMember.Id));

        lblDiscrepancies.Text = $"Discrepancies: count mismatch {countMismatches}, missing reciprocal {reciprocalMissing}";
        lblDiscrepancies.ForeColor = (countMismatches > 0 || reciprocalMissing > 0) ? Color.DarkRed : Color.DarkGreen;
        btnFixDiscrepancies.Enabled = (countMismatches > 0 || reciprocalMissing > 0);
    }

    private void BtnFixDiscrepancies_Click(object sender, EventArgs e)
    {
        using var dlg = new FrmDoublesDiscrepancies(_tournament);
        dlg.ShowDialog(this);
        LoadPairings();
    }

    private sealed class ImportSummary
    {
        public int RowsProcessed { get; set; }
        public int RowsSkipped { get; set; }
        public int PlansUpserted { get; set; }
        public int ParticipantsCreated { get; set; }
        public List<string> Errors { get; } = new();
        public HashSet<(int MemberNumber, int Squad)> ProcessedEntries { get; } = new();
        public List<string> RemovedFromSquad { get; } = new();
        public List<string> RemovedFromTournament { get; } = new();
        public List<string> PartnerCountChanged { get; } = new();
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

        bool claim1Exists = DoublesPartnerClaimDB.ClaimExists(_tournament.Id, mem1Id, mem2Id, squad);
        bool claim2Exists = DoublesPartnerClaimDB.ClaimExists(_tournament.Id, mem2Id, mem1Id, squad);

        string claimNote = (claim1Exists || claim2Exists)
            ? "\n\nThis will also remove the associated partner claims."
            : string.Empty;

        if (MessageBox.Show($"Remove this pairing?{claimNote}", "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            if (claim1Exists || claim2Exists)
                DoublesPartnerClaimDB.RemoveClaimsForPair(_tournament.Id, mem1Id, mem2Id, squad);
            DoublesTeamDB.RemoveTeam(teamId);
            LoadPairings();
        }
    }

    // ----------------------------------------------------------------
    // Squad-aware helpers
    // ----------------------------------------------------------------

    private HashSet<int> GetMemberIdsInSquad(int squad)
    {
        using var db = new NineTapDb();
        return new HashSet<int>(
            db.Participants
              .Where(p => p.Tournament.Id == _tournament.Id && p.Squad == squad)
              .Select(p => p.Member.Id));
    }

    private HashSet<int> GetValidMemberIds(int squadIndex)
    {
        if (squadIndex > 0)
            return GetMemberIdsInSquad(squadIndex);

        return new HashSet<int>(
            TournamentDB.GetUniqueTourMembers(_tournament).Select(m => m.Id));
    }

    // ----------------------------------------------------------------
    // Grid population
    // ----------------------------------------------------------------

    private void LoadPairings()
    {
        int selectedSquad = cboSquad.SelectedIndex;   // 0=All, 1=Squad1, etc.

        dgvPairings.Rows.Clear();
        List<DoublesTeam> teams = DoublesTeamDB.GetTeamsByTournament(_tournament.Id);
        List<DoublesTeam> allTeams = [.. teams];

        if (selectedSquad > 0)
            teams = teams.FindAll(t => t.Squad == selectedSquad);

        // Sort: by squad asc, then by member1 number
        teams.Sort((a, b) => a.Squad != b.Squad ? a.Squad.CompareTo(b.Squad)
                                                : a.Member1.Number.CompareTo(b.Member1.Number));

        foreach (var team in teams)
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

        UpdateSummaryLabels(allTeams);
        PopulateBowlersList();
    }
}
