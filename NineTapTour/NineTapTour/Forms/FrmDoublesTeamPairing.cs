using NineTapTour.Database;
using NineTapTour.Models;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NineTapTour.Forms
{
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
        private Button btnAddPairs;
        private Button btnImportExcel;

        // Dynamic partner rows (y=72+)
        private Panel pnlPartners;
        private readonly List<(TextBox NumBox, Label NameLabel)> _partnerControls = new();
        private List<Member> _existingPartnersForBowler = new();
        private bool _populatingPartners = false;

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
            Size            = new Size(800, 640);
            MinimumSize     = new Size(600, 440);
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

            // Squad row (y=8)
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

            // Bowler + partner-count row (y=40)
            lblBowlerNum = new Label { Text = "Bowler #:", Location = new Point(8, 44), AutoSize = true };
            txtBowlerNumber = new TextBox { Location = new Point(68, 40), Width = 65 };
            lblBowlerName   = new Label  { Location = new Point(140, 44), Width = 170, AutoSize = false, Text = string.Empty };

            lblPartnerCountLabel = new Label { Text = "# of Partners:", Location = new Point(318, 44), AutoSize = true };
            txtPartnerCount      = new TextBox { Location = new Point(408, 40), Width = 35 };

            btnAddPairs = new Button
            {
                Text      = "Add Pairs",
                Size      = new Size(90, 26),
                Location  = new Point(452, 38),
                BackColor = Color.LightGreen,
                Enabled   = false
            };
            btnAddPairs.Click += BtnAddPairs_Click;

            btnImportExcel = new Button
            {
                Text = "Import Excel",
                Size = new Size(96, 26),
                Location = new Point(548, 0)
            };
            btnImportExcel.Click += BtnImportExcel_Click;

            // Secondary squad picker — visible only when cboSquad = "All Squads" (y=40)
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
            txtPartnerCount.Leave   += (s2, e2) => RebuildPartnerControls();

            pnlInput.Controls.Add(lblSquad);
            pnlInput.Controls.Add(cboSquad);
            pnlInput.Controls.Add(lblBowlerNum);
            pnlInput.Controls.Add(txtBowlerNumber);
            pnlInput.Controls.Add(lblBowlerName);
            pnlInput.Controls.Add(lblPartnerCountLabel);
            pnlInput.Controls.Add(txtPartnerCount);
            pnlInput.Controls.Add(btnAddPairs);
            pnlInput.Controls.Add(btnImportExcel);
            pnlInput.Controls.Add(lblAddSquad);
            pnlInput.Controls.Add(cboAddSquad);
            pnlInput.Controls.Add(pnlPartners);

            // --- Summary panel ---
            pnlSummary = new Panel { Dock = DockStyle.Top, Height = 52, Padding = new Padding(8, 4, 8, 4) };
            lblTotalTeams = new Label { Location = new Point(8, 6), AutoSize = true, Text = "Total Teams: 0" };
            lblSquadBreakdown = new Label { Location = new Point(8, 24), AutoSize = true, Text = "By Squad: none" };
            lblDiscrepancies = new Label { Location = new Point(360, 6), Size = new Size(340, 38), AutoSize = false, Text = "Discrepancies: none" };
            pnlSummary.Controls.Add(lblTotalTeams);
            pnlSummary.Controls.Add(lblSquadBreakdown);
            pnlSummary.Controls.Add(lblDiscrepancies);

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
            int x = ClientSize.Width - pnlBowlerList.Width - 8;
            int y = ClientSize.Height - pnlBowlerList.Height - 48; // keep clear of bottom bar

            if (x < 0) x = 0;
            if (y < lblHeader.Bottom + 4) y = lblHeader.Bottom + 4;

            pnlBowlerList.Location = new Point(x, y);
            pnlBowlerList.BringToFront();
        }

        // ----------------------------------------------------------------
        // Dynamic partner rows
        // ----------------------------------------------------------------

        private void RebuildPartnerControls()
        {
            pnlPartners.Controls.Clear();
            _partnerControls.Clear();
            btnAddPairs.Enabled = false;

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
                btnAddPairs.Enabled = true;
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

                numBox.Leave   += (s2, e2) => LookupMemberName((TextBox)s2, nameLbl);
                numBox.KeyDown += (s2, e2) => TxtPartnerBox_KeyDown((TextBox)s2, capturedIndex, e2);

                pnlPartners.Controls.Add(lbl);
                pnlPartners.Controls.Add(numBox);
                pnlPartners.Controls.Add(nameLbl);
                _partnerControls.Add((numBox, nameLbl));
            }

            pnlPartners.Height  = count * 28 + 8;
            pnlInput.Height     = 76 + pnlPartners.Height;
            btnAddPairs.Enabled = true;

            // Focus the first slot that has no pre-filled value
            int firstEmpty = _existingPartnersForBowler.Count;
            if (firstEmpty < _partnerControls.Count)
                _partnerControls[firstEmpty].NumBox.Focus();
            else
                btnAddPairs.Focus();
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
                e.SuppressKeyPress = true;
            }
        }

        private void TxtPartnerBox_KeyDown(TextBox sender, int index, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                LookupMemberName(sender, _partnerControls[index].NameLabel);
                if (index + 1 < _partnerControls.Count)
                    _partnerControls[index + 1].NumBox.Focus();
                else
                    btnAddPairs.Focus();
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

        private void BtnAddPairs_Click(object sender, EventArgs e)
        {
            int targetSquad = GetTargetSquad();
            if (targetSquad == 0)
            {
                MessageBox.Show("Please select which squad to add pairs to.", "Squad Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtBowlerNumber.Text.Trim(), out int mainNum))
            {
                MessageBox.Show("Please enter a valid member number for the bowler.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int mainId = MemberDB.GetMemberIdByNumber(mainNum);
            if (mainId == 0)
            {
                MessageBox.Show($"Bowler #{mainNum} was not found in the member database.", "Member Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            HashSet<int> validIds = GetValidMemberIds(targetSquad);

            if (!validIds.Contains(mainId))
            {
                MessageBox.Show($"Bowler #{mainNum} has not been entered into Squad {targetSquad} yet.", "Not In Squad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int expectedCount = int.TryParse(txtPartnerCount.Text.Trim(), out int parsedExpected)
                ? Math.Max(0, parsedExpected)
                : 0;
            DoublesPartnerPlanDB.UpsertPlan(_tournament.Id, mainId, targetSquad, expectedCount);

            int claimsAdded = 0;
            int teamsAdded = 0;
            int skipped = 0;
            int partnerEntriesAttempted = 0;
            var skipReasons = new List<string>();

            foreach (var (numBox, _) in _partnerControls)
            {
                string partnerText = numBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(partnerText))
                    continue;

                partnerEntriesAttempted++;

                if (!int.TryParse(partnerText, out int partnerNum))
                {
                    skipped++;
                    skipReasons.Add($"'{partnerText}' is not a valid number");
                    continue;
                }

                int partnerId = MemberDB.GetMemberIdByNumber(partnerNum);

                if (partnerId == 0)
                {
                    skipped++;
                    skipReasons.Add($"#{partnerNum} not found in member database");
                    continue;
                }
                if (partnerId == mainId)
                {
                    skipped++;
                    skipReasons.Add($"#{partnerNum} is the same bowler as #{mainNum} \u2014 a bowler cannot be paired with themselves");
                    continue;
                }
                if (!validIds.Contains(partnerId))
                {
                    skipped++;
                    skipReasons.Add($"#{partnerNum} has not been entered into Squad {targetSquad}");
                    continue;
                }

                bool claimAdded = DoublesPartnerClaimDB.AddClaim(_tournament.Id, mainId, partnerId, targetSquad);
                if (!claimAdded)
                {
                    skipped++;
                    skipReasons.Add($"#{mainNum} already listed #{partnerNum} in Squad {targetSquad}");
                    continue;
                }

                claimsAdded++;

                bool teamAdded = DoublesTeamDB.AddTeam(_tournament.Id, mainId, partnerId, targetSquad);
                if (teamAdded)
                    teamsAdded++;
            }

            if (claimsAdded > 0 || skipped > 0)
            {
                string details = string.Join("\n  \u2022 ", skipReasons);
                MessageBox.Show(
                    $"{claimsAdded} partner claim(s) saved, {teamsAdded} unique team(s) created, {skipped} skipped"
                    + (skipReasons.Count > 0 ? $":\n  \u2022 {details}" : "."),
                    "Add Pairs Result",
                    MessageBoxButtons.OK,
                    claimsAdded == 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    partnerEntriesAttempted == 0
                        ? $"Saved expected partner count ({expectedCount}) for bowler #{mainNum} in Squad {targetSquad}."
                        : "No changes were made.",
                    "Add Pairs Result",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            // Reset inputs
            txtBowlerNumber.Clear();
            lblBowlerName.Text = string.Empty;
            txtPartnerCount.Clear();
            pnlPartners.Controls.Clear();
            _partnerControls.Clear();
            _existingPartnersForBowler.Clear();
            pnlPartners.Height  = 0;
            pnlInput.Height     = 76;
            btnAddPairs.Enabled = false;
            txtBowlerNumber.Focus();

            LoadPairings();
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

            var summary = ImportBowlersAndExpectedCounts(open.FileName);

            string details = summary.Errors.Count > 0
                ? "\n\nIssues:\n- " + string.Join("\n- ", summary.Errors.Take(25))
                : string.Empty;

            if (summary.Errors.Count > 25)
                details += $"\n- ...and {summary.Errors.Count - 25} more.";

            MessageBox.Show(
                $"Import complete.\nRows processed: {summary.RowsProcessed}\nPlans added/updated: {summary.PlansUpserted}\nParticipants created: {summary.ParticipantsCreated}\nRows skipped: {summary.RowsSkipped}{details}",
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
                lstBowlers.SelectedIndex = 0;
                UpdateBowlerNavButtons();
                return;
            }

            int restoreIndex = items.FindIndex(i => i.MemberId == previousMemberId);
            if (restoreIndex >= 0)
                lstBowlers.SelectedIndex = restoreIndex;

            UpdateBowlerNavButtons();
        }

        private void LstBowlers_SelectedIndexChanged(object sender, EventArgs e)
        {
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
        }

        private sealed class ImportSummary
        {
            public int RowsProcessed { get; set; }
            public int RowsSkipped { get; set; }
            public int PlansUpserted { get; set; }
            public int ParticipantsCreated { get; set; }
            public List<string> Errors { get; } = new();
        }

        // ----------------------------------------------------------------
        // Remove pair
        // ----------------------------------------------------------------

        private void DgvPairings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != dgvPairings.Columns["colRemove"].Index)
                return;

            int teamId = (int)dgvPairings.Rows[e.RowIndex].Cells["colTeamId"].Value;
            if (MessageBox.Show("Remove this pairing?", "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
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
}
