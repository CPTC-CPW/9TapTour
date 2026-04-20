using CalcService = NineTapTour.Calculations.Calculations;
using NineTapTour.Database;
using NineTapTour.Models;
using NineTapTour.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NineTapTour.Forms
{
    public partial class FrmFinalizeTournament : Form
    {
        private Tournament selectedTournament;

        // Top-level controls — access these to adjust later
        private Panel pnlToolbar;
        private CheckBox chkDirCheck;
        private CheckBox chkAdjAvg;
        private Button btnFinalizeTournament;
        private SplitContainer splitMain;
        private Panel pnlPlayerInfo;
        private Label lblPlayerInfo;
        private DataGridView dgvTournament;
        private DataGridView dgvDetail;

        private List<WinnerListMemberViewModel> _currentTournamentBowlers;
        private int _displayedDetailMemberNumber = -1;
        private readonly HashSet<int> _invalidRowIndices = [];

        private static readonly string[] GameScoreColumns = ["colGame1", "colGame2", "colGame3", "colGame4"];

        // GameIds whose colBonus cell shows a post-deduction preview value.
        // The original (pre-deduction) bonus is kept in Game.Bonus; only Member.Bonus
        // receives the deducted value on finalization.
        private readonly HashSet<int> _cashingGameIds = [];

        public FrmFinalizeTournament(Tournament selectedTournament)
        {
            this.selectedTournament = selectedTournament;

            InitializeComponent();
        }

        private void FrmFinalizeTournament_Load(object sender, EventArgs e)
        {
            BuildGrids();
            LoadTournamentGrid();
            dgvTournament.SelectionChanged += DgvTournament_SelectionChanged;
            DgvTournament_SelectionChanged(dgvTournament, EventArgs.Empty);
        }

        private void BuildGrids()
        {
            SuspendLayout();

            // --- Toolbar panel ---
            pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 40 };

            chkDirCheck = new CheckBox { Text = "Dir Check", Location = new Point(10, 10), AutoSize = true };
            chkAdjAvg   = new CheckBox { Text = "Adj Avg",   Location = new Point(95, 10), AutoSize = true };

            btnFinalizeTournament = new Button
            {
                Text   = "Finalize Tournament",
                Size   = new Size(160, 26),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnFinalizeTournament.Click += BtnFinalizeTournament_Click;
            pnlToolbar.Controls.AddRange([chkDirCheck, chkAdjAvg, btnFinalizeTournament]);

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

            // Add to form — Fill first, Top-docked toolbar second so toolbar is laid out first
            Controls.Add(splitMain);
            Controls.Add(pnlToolbar);

            ResumeLayout(true);

            // Set after layout so pnlToolbar already has its docked width
            btnFinalizeTournament.Location = new Point(pnlToolbar.ClientSize.Width - btnFinalizeTournament.Width - 12, 7);

            splitMain.SplitterDistance = splitMain.Height / 2;
        }

        private DataGridView CreateTournamentGrid()
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
                new DataGridViewTextBoxColumn  { Name = "colBonus",        HeaderText = "Bonus",           Width = 45  },
                new DataGridViewTextBoxColumn  { Name = "colEarnings",     HeaderText = "Earnings",        Width = 60  },
                new DataGridViewTextBoxColumn  { Name = "colNotes",        HeaderText = "Notes",           AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill }
            );

            return dgv;
        }

        private void LoadTournamentGrid()
        {
            _cashingGameIds.Clear();
            _currentTournamentBowlers = TournamentDB.GetWinnerListMemberData(selectedTournament.Id);
            List<ExcelMember> members = BuildExcelMemberList(_currentTournamentBowlers);

            // Compute the correct placement for each member using only their best entry
            List<ExcelMember> deduped = CalcService.CalculatePlaceStandings(members, removeDuplicates: true);
            Dictionary<int, int> bestStandingByMember = deduped.ToDictionary(m => m.MemberNumber, m => m.PlaceStanding);

            // Determine the cash line so bonus deductions can be previewed in the grid
            int totalEntries = _currentTournamentBowlers.Count;
            int compEntries  = _currentTournamentBowlers.Count(b => b.IsComp);
            int cashLine     = CalcService.GetQtyOfMembersThatCanPlace(totalEntries, compEntries);

            // Sort all entries by score descending so the best entry per member is encountered first
            members.Sort((x, y) => y.TotalScore.CompareTo(x.TotalScore));

            // First occurrence of each member gets the standing; every duplicate entry gets 0
            var seenMembers = new HashSet<int>();
            foreach (ExcelMember m in members)
            {
                m.PlaceStanding = seenMembers.Add(m.MemberNumber)
                    ? bestStandingByMember[m.MemberNumber]
                    : 0;
            }

            // Display order: groups ordered by best place standing, all entries for the
            // same member clustered together, placed entry first within each group
            members.Sort((x, y) =>
            {
                // Primary: order groups by the member's best place standing
                int xBest = bestStandingByMember[x.MemberNumber];
                int yBest = bestStandingByMember[y.MemberNumber];
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

            // Lookup for original nullable game scores — avoids null-vs-0 ambiguity
            var bowlerByGameId = _currentTournamentBowlers.ToDictionary(b => b.GameId);

            dgvTournament.Rows.Clear();
            foreach (ExcelMember m in members)
            {
                WinnerListMemberViewModel orig = bowlerByGameId[m.GameId];

                // A game is checked when it has a recorded score (non-null) — used as default
                // when UseGame flags have never been explicitly saved.
                bool g1Checked = orig.Game1.HasValue;
                bool g2Checked = orig.Game2.HasValue;
                bool g3Checked = orig.Game3.HasValue;
                bool g4Checked = orig.Game4.HasValue;

                // 3-of-4: uncheck the lowest-scoring game when all 4 are present
                // Only auto-apply when the flags have never been explicitly saved
                bool useGameNeverSaved = orig.UseGame1 == null && orig.UseGame2 == null
                                      && orig.UseGame3 == null && orig.UseGame4 == null;
                if (selectedTournament.ThreeOutOf4 && useGameNeverSaved)
                {
                    var validScores = new[]
                    {
                        (Score: orig.Game1, Game: 1),
                        (Score: orig.Game2, Game: 2),
                        (Score: orig.Game3, Game: 3),
                        (Score: orig.Game4, Game: 4)
                    }.Where(x => x.Score.HasValue).ToList();

                    if (validScores.Count == 4)
                    {
                        int lowestGame = validScores.MinBy(x => x.Score!.Value).Game;
                        if (lowestGame == 1) g1Checked = false;
                        else if (lowestGame == 2) g2Checked = false;
                        else if (lowestGame == 3) g3Checked = false;
                        else if (lowestGame == 4) g4Checked = false;
                    }
                }

                // Restore explicitly saved use-game flags (overrides defaults and 3-of-4 auto logic)
                if (orig.UseGame1.HasValue) g1Checked = orig.UseGame1.Value;
                if (orig.UseGame2.HasValue) g2Checked = orig.UseGame2.Value;
                if (orig.UseGame3.HasValue) g3Checked = orig.UseGame3.Value;
                if (orig.UseGame4.HasValue) g4Checked = orig.UseGame4.Value;

                int scratch = (g1Checked ? (orig.Game1 ?? 0) : 0) + (g2Checked ? (orig.Game2 ?? 0) : 0)
                                 + (g3Checked ? (orig.Game3 ?? 0) : 0) + (g4Checked ? (orig.Game4 ?? 0) : 0);
                int checkedGames = (g1Checked ? 1 : 0) + (g2Checked ? 1 : 0)
                                 + (g3Checked ? 1 : 0) + (g4Checked ? 1 : 0);
                // Pre-deduct bonus for members who will cash (placing within cash line)
                int memberPlacing = bestStandingByMember.TryGetValue(m.MemberNumber, out int p) ? p : 0;
                bool isCashing    = memberPlacing > 0 && memberPlacing <= cashLine;
                int displayBonus  = isCashing
                    ? CalcService.DeductFromBonusPins(memberPlacing, m.Bonus)
                    : m.Bonus;
                if (isCashing)
                    _cashingGameIds.Add(m.GameId);

                int hdcpTotal = scratch + (checkedGames * (m.Handicap + displayBonus));
                int entryAvg = checkedGames > 0 ? scratch / checkedGames : 0;

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
                    orig.AdjustedAvg > 0 ? (object)orig.AdjustedAvg : 0,  // ADJ AVG — restored from DB
                    orig.KeepAdjustedAvg,  // Director Check — restored from DB
                    null,  // Squad
                    m.Handicap,
                    displayBonus,
                    m.MoneyWon > 0 ? (object)m.MoneyWon : null,  // Earnings
                    null   // Notes
                );
                dgvTournament.Rows[rowIdx].Tag = m.GameId;
                ApplySandbaggingHighlight(rowIdx, orig.LeagueAverage);
            }

            // Validate all rows so previously-valid rows are not incorrectly flagged on open
            for (int i = 0; i < dgvTournament.Rows.Count; i++)
                ValidateRow(i);
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
                ExcelMember m = new()
                {
                    MemberNumber = b.MemberNumber,
                    Name         = b.BowlerName,
                    Handicap     = Convert.ToInt32(b.Handicap),
                    Bonus        = Convert.ToInt32(b.Bonus),
                    MoneyWon     = b.MoneyWon,
                    GameId       = b.GameId,
                    Game1Score   = Convert.ToInt32(b.Game1),
                    Game2Score   = Convert.ToInt32(b.Game2),
                    Game3Score   = Convert.ToInt32(b.Game3),
                    Game4Score   = Convert.ToInt32(b.Game4)
                };

                if (selectedTournament.ThreeOutOf4)
                {
                    List<int> scores = new[] { b.Game1, b.Game2, b.Game3, b.Game4 }
                        .Where(g => g.HasValue).Select(g => g.Value).ToList();
                    if (scores.Count == 4)
                        scores.Remove(scores.Min());
                    m.TotalScore = scores.Sum() + (scores.Count * (m.Handicap + m.Bonus));
                }
                else
                {
                    int validGames = (b.Game1.HasValue ? 1 : 0) + (b.Game2.HasValue ? 1 : 0)
                                   + (b.Game3.HasValue ? 1 : 0) + (b.Game4.HasValue ? 1 : 0);
                    int scratch    = (b.Game1 ?? 0) + (b.Game2 ?? 0) + (b.Game3 ?? 0) + (b.Game4 ?? 0);
                    m.TotalScore   = scratch + (validGames * (m.Handicap + m.Bonus));
                }

                members.Add(m);
            }
            return members;
        }

        private DataGridView CreateDetailGrid()
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
        /// Recomputes Scratch Total, HDCP Total, and Entry AVG for the given row
        /// based on which game checkboxes are currently checked.
        /// </summary>
        private void RecalculateTournamentRow(int rowIndex)
        {
            var row = dgvTournament.Rows[rowIndex];

            int GetCheckedScore(string scoreCol, string checkCol)
            {
                bool isChecked = row.Cells[checkCol].Value as bool? ?? false;
                if (!isChecked) return 0;
                return Convert.ToInt32(row.Cells[scoreCol].Value ?? 0);
            }

            int hdcp  = Convert.ToInt32(row.Cells["colHdcp"].Value  ?? 0);
            int bonus = Convert.ToInt32(row.Cells["colBonus"].Value ?? 0);

            bool c1 = row.Cells["colGame1Check"].Value as bool? ?? false;
            bool c2 = row.Cells["colGame2Check"].Value as bool? ?? false;
            bool c3 = row.Cells["colGame3Check"].Value as bool? ?? false;
            bool c4 = row.Cells["colGame4Check"].Value as bool? ?? false;

            int g1 = GetCheckedScore("colGame1", "colGame1Check");
            int g2 = GetCheckedScore("colGame2", "colGame2Check");
            int g3 = GetCheckedScore("colGame3", "colGame3Check");
            int g4 = GetCheckedScore("colGame4", "colGame4Check");

            int scratch      = g1 + g2 + g3 + g4;
            int checkedGames = (c1 ? 1 : 0) + (c2 ? 1 : 0) + (c3 ? 1 : 0) + (c4 ? 1 : 0);
            int hdcpTotal    = scratch + (checkedGames * (hdcp + bonus));
            int entryAvg     = checkedGames > 0 ? scratch / checkedGames : 0;

            row.Cells["colScratchTotal"].Value = scratch;
            row.Cells["colHdcpTotal"].Value    = hdcpTotal;
            row.Cells["colEntryAvg"].Value     = entryAvg;
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
                    int newHdcp = CalcService.CalculateHandicapPins(adjAvg);
                    dgvTournament.Rows[e.RowIndex].Cells["colHdcp"].Value = newHdcp;
                    RecalculateTournamentRow(e.RowIndex);
                }

                // Copy ADJ AVG to all other entries for the same member
                object memberNumber = dgvTournament.Rows[e.RowIndex].Cells["colMemberNumber"].Value;
                foreach (DataGridViewRow row in dgvTournament.Rows)
                {
                    if (row.Index == e.RowIndex) continue;
                    if (Equals(row.Cells["colMemberNumber"].Value, memberNumber))
                    {
                        row.Cells["colAdjAvg"].Value = dgvTournament.Rows[e.RowIndex].Cells["colAdjAvg"].Value;
                        row.Cells["colHdcp"].Value = dgvTournament.Rows[e.RowIndex].Cells["colHdcp"].Value;
                        RecalculateTournamentRow(row.Index);
                        ValidateRow(row.Index);
                        dgvTournament.InvalidateRow(row.Index);
                        PersistRowToDatabase(row.Index);
                    }
                }

                ValidateRow(e.RowIndex);
                dgvTournament.InvalidateRow(e.RowIndex);
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
            if (row.Tag is not int gameId) return;

            Game game = GameDB.GetGame(gameId);
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

            // Bonus — for cashing entries the cell shows a post-deduction preview;
            // preserve the original pre-deduction value in the Game record so reopening
            // the form does not deduct again on each open.
            int bonus = 0;
            if (row.Cells["colBonus"].Value != null)
                int.TryParse(row.Cells["colBonus"].Value.ToString(), out bonus);
            if (_cashingGameIds.Contains(gameId))
            {
                WinnerListMemberViewModel orig = _currentTournamentBowlers.FirstOrDefault(b => b.GameId == gameId);
                game.Bonus = orig != null ? Convert.ToInt32(orig.Bonus) : bonus;
            }
            else
            {
                game.Bonus = bonus;
            }

            // Director Check → persisted as KeepAdjustedAvg
            game.KeepAdjustedAvg = row.Cells["colDirCheck"].Value as bool? ?? false;

            // Earnings
            decimal earnings = 0;
            if (row.Cells["colEarnings"].Value != null)
                decimal.TryParse(row.Cells["colEarnings"].Value.ToString(), out earnings);
            game.MoneyWon = earnings > 0 ? earnings : null;

            // Notes
            game.Notes = row.Cells["colNotes"].Value as string;

            GameDB.AddOrUpdateGame(game);
        }

        /// <summary>
        /// Persists the current game-check states to the database for the given row.
        /// </summary>
        private void UpdateGameUseFlags(int rowIndex)
        {
            var row = dgvTournament.Rows[rowIndex];
            if (row.Tag is not int gameId) return;

            Game game = GameDB.GetGame(gameId);
            if (game == null) return;

            game.UseGame1 = row.Cells["colGame1Check"].Value as bool? ?? false;
            game.UseGame2 = row.Cells["colGame2Check"].Value as bool? ?? false;
            game.UseGame3 = row.Cells["colGame3Check"].Value as bool? ?? false;
            game.UseGame4 = row.Cells["colGame4Check"].Value as bool? ?? false;

            GameDB.AddOrUpdateGame(game);
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

            if (!dirChecked || adjAvg == 0)
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
            using var db = new NineTapDb();

            // Track which members we've already updated so we do it once per member
            var updatedMembers = new HashSet<int>();

            for (int i = 0; i < dgvTournament.Rows.Count; i++)
            {
                var row = dgvTournament.Rows[i];
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

                // Update bonus on the game from the grid — preserve the original pre-deduction
                // value for cashing entries so the Game record is not corrupted across sessions.
                int bonus = 0;
                if (row.Cells["colBonus"].Value != null)
                    int.TryParse(row.Cells["colBonus"].Value.ToString(), out bonus);
                if (_cashingGameIds.Contains(gameId))
                {
                    WinnerListMemberViewModel orig = _currentTournamentBowlers.FirstOrDefault(b => b.GameId == gameId);
                    game.Bonus = orig != null ? Convert.ToInt32(orig.Bonus) : bonus;
                }
                else
                {
                    game.Bonus = bonus;
                }

                // Compute league average for this game
                int memberNumber = Convert.ToInt32(row.Cells["colMemberNumber"].Value);
                double leagueAvg = FinalizeTempDB.Get30GameAverage(memberNumber, selectedTournament.Id);
                game.LeagueAverage = leagueAvg;

                // Update the Member record once per member
                if (updatedMembers.Add(memberNumber))
                {
                    Member member = MemberDB.GetMember(memberNumber, db);
                    if (member != null && member.Id > 0)
                    {
                        member.Average = adjAvg;
                        member.Handicap = CalcService.CalculateHandicapPins(adjAvg);
                        // Bonus already reflects any deduction (applied at grid load time)
                        member.Bonus = bonus;
                    }
                }
            }

            Tournament tourn = db.Tournaments.Find(selectedTournament.Id);
            if (tourn != null)
                tourn.IsTournamentFinalized = true;

            db.SaveChanges();

            MessageBox.Show(
                "Tournament has been finalized successfully.",
                "Finalized",
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
            const int sandbagThreshold = 40;
            var row = dgvTournament.Rows[rowIndex];

            foreach (string scoreCol in GameScoreColumns)
            {
                var cell = row.Cells[scoreCol];
                if (leagueAverage > 0
                    && cell.Value != null
                    && int.TryParse(cell.Value.ToString(), out int score)
                    && score > 0
                    && (leagueAverage - score) >= sandbagThreshold)
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
                PlayerHistoryDB.GetMemberPlayerHistory(memberNumber);

            dgvDetail.Rows.Clear();

            // --- Current tournament rows (blue highlight) ---
            // Pull live values from dgvTournament so edits are immediately reflected.
            // Entries are reversed so the first entry for the member appears at the top.
            var currentEntries = _currentTournamentBowlers
                .Where(b => b.MemberNumber == memberNumber)
                .Reverse()
                .ToList();

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
                    null,                                        // Place — not yet finalized
                    b.MoneyWon > 0 ? (object)b.MoneyWon : null,
                    null                                         // Notes
                );
                dgvDetail.Rows[rowIdx].DefaultCellStyle.BackColor = Color.LightBlue;
            }

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

                    // Highlight the 30 AVG cell for entries within the rolling 30-game window
                    if (historyIndex < thirtyGameWindow)
                        dgvDetail.Rows[rowIdx].Cells["colDetail30Avg"].Style.BackColor = Color.LightGreen;
                    historyIndex++;
                }
            }

            decimal lifetimeEarnings = history.Sum(h => h.MoneyWon);
            UpdateDetailGridHeaders(history.Take(thirtyGameWindow).ToList(), lifetimeEarnings);

            double leagueAvg = history.FirstOrDefault()?.trueAVG ?? 0;
            lblPlayerInfo.Text = $"Mem#   {memberNumber}        {memberName,-35}AVG   {(int)Math.Round(leagueAvg)}";
        }

        /// <summary>
        /// Updates the column headers of <see cref="dgvDetail"/> to show the sum (or computed
        /// average) of the provided entries — typically the last 30 finalized games.
        /// </summary>
        private void UpdateDetailGridHeaders(List<PlayerHistoryViewModel> last30, decimal lifetimeEarnings)
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
            dgvDetail.Columns["colDetail30Avg"].HeaderText    = $"30 AVG\n({avg30:0.#})";
            dgvDetail.Columns["colDetailEarnings"].HeaderText = $"Earnings\n({lifetimeEarnings:0.00})";
        }
    }
}
