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
    /// Each pairing links two members as a team. A pair (A,B) is considered
    /// identical to (B,A) — duplicate pairings are rejected.
    /// </summary>
    public class FrmDoublesTeamPairing : Form
    {
        private readonly Tournament _tournament;

        // Header
        private Label lblHeader;

        // Input area
        private Label lblMember1Label;
        private TextBox txtMember1Number;
        private Label lblMember1Name;
        private Label lblMember2Label;
        private TextBox txtMember2Number;
        private Label lblMember2Name;
        private Button btnAddPair;

        // Pairings grid
        private DataGridView dgvPairings;

        // Bottom
        private Button btnClose;

        public FrmDoublesTeamPairing(Tournament tournament)
        {
            _tournament = tournament;
            InitializeControls();
            LoadPairings();
        }

        private void InitializeControls()
        {
            SuspendLayout();

            Text = "Doubles Pairings";
            Size = new Size(680, 520);
            MinimumSize = new Size(580, 420);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;

            // --- Header ---
            lblHeader = new Label
            {
                Text = $"Doubles Pairings — {_tournament.TourneyNameDate}",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 36,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };

            // --- Input panel ---
            var pnlInput = new Panel { Dock = DockStyle.Top, Height = 64, Padding = new Padding(8, 8, 8, 4) };

            lblMember1Label = new Label { Text = "Bowler 1 #:", Location = new Point(8, 12), AutoSize = true };
            txtMember1Number = new TextBox { Location = new Point(80, 8), Width = 70 };
            lblMember1Name = new Label { Location = new Point(158, 12), Width = 160, AutoSize = false, Text = string.Empty };

            lblMember2Label = new Label { Text = "Bowler 2 #:", Location = new Point(8, 38), AutoSize = true };
            txtMember2Number = new TextBox { Location = new Point(80, 34), Width = 70 };
            lblMember2Name = new Label { Location = new Point(158, 38), Width = 160, AutoSize = false, Text = string.Empty };

            btnAddPair = new Button
            {
                Text = "Add Pair",
                Size = new Size(100, 52),
                Location = new Point(328, 6),
                BackColor = Color.LightGreen
            };
            btnAddPair.Click += BtnAddPair_Click;

            txtMember1Number.Leave += (s, e) => LookupMemberName(txtMember1Number, lblMember1Name);
            txtMember2Number.Leave += (s, e) => LookupMemberName(txtMember2Number, lblMember2Name);
            txtMember1Number.KeyDown += TxtMember_KeyDown;
            txtMember2Number.KeyDown += TxtMember_KeyDown;

            pnlInput.Controls.AddRange([lblMember1Label, txtMember1Number, lblMember1Name,
                                         lblMember2Label, txtMember2Number, lblMember2Name,
                                         btnAddPair]);

            // --- Grid ---
            dgvPairings = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvPairings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTeamId",   HeaderText = "ID",         Visible = false });
            dgvPairings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMem1Num",  HeaderText = "Bowler 1 #", FillWeight = 15 });
            dgvPairings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMem1Name", HeaderText = "Bowler 1 Name", FillWeight = 35 });
            dgvPairings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMem2Num",  HeaderText = "Bowler 2 #", FillWeight = 15 });
            dgvPairings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMem2Name", HeaderText = "Bowler 2 Name", FillWeight = 35 });

            var btnCol = new DataGridViewButtonColumn
            {
                Name = "colRemove",
                HeaderText = string.Empty,
                Text = "Remove",
                UseColumnTextForButtonValue = true,
                FillWeight = 20
            };
            dgvPairings.Columns.Add(btnCol);
            dgvPairings.CellClick += DgvPairings_CellClick;

            // --- Bottom bar ---
            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 40 };
            btnClose = new Button { Text = "Close", Size = new Size(88, 26) };
            btnClose.Click += (s, e) => Close();
            pnlBottom.Controls.Add(btnClose);
            pnlBottom.Resize += (s, _) =>
            {
                btnClose.Location = new Point(pnlBottom.Width - btnClose.Width - 8,
                                              (pnlBottom.Height - btnClose.Height) / 2);
            };

            Controls.Add(dgvPairings);    // Fill — added first so Top/Bottom dock panels overlay it
            Controls.Add(pnlBottom);
            Controls.Add(pnlInput);
            Controls.Add(lblHeader);

            ResumeLayout(false);
        }

        // ----------------------------------------------------------------
        // Member name look-up helpers
        // ----------------------------------------------------------------

        private void TxtMember_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                var txt = (TextBox)sender;
                var lbl = txt == txtMember1Number ? lblMember1Name : lblMember2Name;
                LookupMemberName(txt, lbl);
                SelectNext(txt);
                e.SuppressKeyPress = true;
            }
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
            if (m?.Id > 0)
            {
                lbl.Text = $"{m.FirstName} {m.LastName}";
            }
            else
            {
                lbl.Text = "Not found";
                lbl.ForeColor = Color.Red;
            }
        }

        private void SelectNext(TextBox current)
        {
            if (current == txtMember1Number)
                txtMember2Number.Focus();
            else
                btnAddPair.Focus();
        }

        // ----------------------------------------------------------------
        // Add pair
        // ----------------------------------------------------------------

        private void BtnAddPair_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtMember1Number.Text.Trim(), out int num1))
            {
                MessageBox.Show("Please enter a valid member number for Bowler 1.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txtMember2Number.Text.Trim(), out int num2))
            {
                MessageBox.Show("Please enter a valid member number for Bowler 2.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (num1 == num2)
            {
                MessageBox.Show("A bowler cannot be paired with themselves.", "Invalid Pairing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id1 = MemberDB.GetMemberIdByNumber(num1);
            int id2 = MemberDB.GetMemberIdByNumber(num2);

            if (id1 == 0)
            {
                MessageBox.Show($"Bowler 1 (#{num1}) was not found in the member database.", "Member Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (id2 == 0)
            {
                MessageBox.Show($"Bowler 2 (#{num2}) was not found in the member database.", "Member Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verify both members have Participant records in this tournament
            var participants = TournamentDB.GetUniqueTourMembers(_tournament);
            bool inTourney1 = participants.Any(m => m.Id == id1);
            bool inTourney2 = participants.Any(m => m.Id == id2);
            if (!inTourney1)
            {
                MessageBox.Show($"Bowler 1 (#{num1}) has not been entered into this tournament yet.", "Not In Tournament", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!inTourney2)
            {
                MessageBox.Show($"Bowler 2 (#{num2}) has not been entered into this tournament yet.", "Not In Tournament", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool added = DoublesTeamDB.AddTeam(_tournament.Id, id1, id2);
            if (!added)
            {
                MessageBox.Show($"The pairing #{num1} / #{num2} already exists for this tournament.", "Duplicate Pairing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            txtMember1Number.Clear();
            txtMember2Number.Clear();
            lblMember1Name.Text = string.Empty;
            lblMember2Name.Text = string.Empty;
            txtMember1Number.Focus();
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
            var result = MessageBox.Show("Remove this pairing?", "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DoublesTeamDB.RemoveTeam(teamId);
                LoadPairings();
            }
        }

        // ----------------------------------------------------------------
        // Grid population
        // ----------------------------------------------------------------

        private void LoadPairings()
        {
            dgvPairings.Rows.Clear();
            List<DoublesTeam> teams = DoublesTeamDB.GetTeamsByTournament(_tournament.Id);
            foreach (var team in teams)
            {
                dgvPairings.Rows.Add(
                    team.Id,
                    team.Member1.Number,
                    $"{team.Member1.FirstName} {team.Member1.LastName}",
                    team.Member2.Number,
                    $"{team.Member2.FirstName} {team.Member2.LastName}"
                );
            }
        }
    }
}
