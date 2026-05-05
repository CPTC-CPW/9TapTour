using NineTapTour.Database;
using NineTapTour.Models;
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

        // Dynamic partner rows (y=72+)
        private Panel pnlPartners;
        private readonly List<(TextBox NumBox, Label NameLabel)> _partnerControls = new();
        private List<Member> _existingPartnersForBowler = new();
        private bool _populatingPartners = false;

        // Pairings grid
        private DataGridView dgvPairings;

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
            Size            = new Size(720, 580);
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
                Location  = new Point(455, 38),
                BackColor = Color.LightGreen,
                Enabled   = false
            };
            btnAddPairs.Click += BtnAddPairs_Click;

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
            pnlInput.Controls.Add(lblAddSquad);
            pnlInput.Controls.Add(cboAddSquad);
            pnlInput.Controls.Add(pnlPartners);

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
            Controls.Add(pnlBottom);
            Controls.Add(pnlInput);
            Controls.Add(lblHeader);

            // Initialise secondary squad picker visibility to match initial cboSquad state
            bool startAllSquads = cboSquad.SelectedIndex == 0;
            lblAddSquad.Visible = startAllSquads;
            cboAddSquad.Visible = startAllSquads;

            ResumeLayout(false);
        }

        // ----------------------------------------------------------------
        // Dynamic partner rows
        // ----------------------------------------------------------------

        private void RebuildPartnerControls()
        {
            pnlPartners.Controls.Clear();
            _partnerControls.Clear();
            btnAddPairs.Enabled = false;

            if (!int.TryParse(txtPartnerCount.Text.Trim(), out int count) || count < 1 || count > 20)
            {
                pnlPartners.Height = 0;
                pnlInput.Height    = 76;
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

            // Find all existing partners for this bowler in the target squad
            List<DoublesTeam> allTeams = DoublesTeamDB.GetTeamsByTournament(_tournament.Id);
            _existingPartnersForBowler = allTeams
                .Where(t => t.Squad == targetSquad &&
                            (t.Member1.Id == mainId || t.Member2.Id == mainId))
                .Select(t => t.Member1.Id == mainId ? t.Member2 : t.Member1)
                .ToList();

            // Set count to at least the number of existing partners; keep whatever the user
            // already typed if it's higher so we show extra empty slots.
            int requested = int.TryParse(txtPartnerCount.Text.Trim(), out int c) && c > 0 ? c : 0;
            int newCount  = Math.Max(requested, _existingPartnersForBowler.Count);
            if (newCount == 0) newCount = 1;   // always show at least one slot

            txtPartnerCount.Text = newCount.ToString();
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

            int added = 0, skipped = 0;
            var skipReasons = new List<string>();

            foreach (var (numBox, _) in _partnerControls)
            {
                if (!int.TryParse(numBox.Text.Trim(), out int partnerNum))
                {
                    skipped++;
                    skipReasons.Add($"'{numBox.Text.Trim()}' is not a valid number");
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

                bool ok = DoublesTeamDB.AddTeam(_tournament.Id, mainId, partnerId, targetSquad);
                if (ok)
                    added++;
                else
                {
                    skipped++;
                    skipReasons.Add($"#{mainNum} / #{partnerNum} are already paired in Squad {targetSquad}");
                }
            }

            if (skipped > 0)
            {
                string details = string.Join("\n  \u2022 ", skipReasons);
                MessageBox.Show(
                    $"{added} pairing(s) added, {skipped} skipped:\n  \u2022 {details}",
                    "Add Pairs Result",
                    MessageBoxButtons.OK,
                    added == 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
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
        }
    }
}
