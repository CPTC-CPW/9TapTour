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
                new DataGridViewTextBoxColumn  { Name = "colProPot",       HeaderText = "Pro\nPot",        Width = 45  },
                new DataGridViewTextBoxColumn  { Name = "colNotes",        HeaderText = "Notes",           AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill }
            );

            return dgv;
        }

        private void LoadTournamentGrid()
        {
            _currentTournamentBowlers = TournamentDB.GetWinnerListMemberData(selectedTournament.Id);
            List<ExcelMember> members = BuildExcelMemberList(_currentTournamentBowlers);

            // Compute the correct placement for each member using only their best entry
            List<ExcelMember> deduped = CalcService.CalculatePlaceStandings(members, removeDuplicates: true);
            Dictionary<int, int> bestStandingByMember = deduped.ToDictionary(m => m.MemberNumber, m => m.PlaceStanding);

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

                // A game is checked when it has a recorded score (non-null)
                bool g1Checked = orig.Game1.HasValue;
                bool g2Checked = orig.Game2.HasValue;
                bool g3Checked = orig.Game3.HasValue;
                bool g4Checked = orig.Game4.HasValue;

                // 3-of-4: uncheck the lowest-scoring game when all 4 are present
                if (selectedTournament.ThreeOutOf4)
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
                        if      (lowestGame == 1) g1Checked = false;
                        else if (lowestGame == 2) g2Checked = false;
                        else if (lowestGame == 3) g3Checked = false;
                        else if (lowestGame == 4) g4Checked = false;
                    }
                }

                int scratch      = (g1Checked ? (orig.Game1 ?? 0) : 0) + (g2Checked ? (orig.Game2 ?? 0) : 0)
                                 + (g3Checked ? (orig.Game3 ?? 0) : 0) + (g4Checked ? (orig.Game4 ?? 0) : 0);
                int checkedGames = (g1Checked ? 1 : 0) + (g2Checked ? 1 : 0)
                                 + (g3Checked ? 1 : 0) + (g4Checked ? 1 : 0);
                int hdcpTotal    = scratch + (checkedGames * (m.Handicap + m.Bonus));
                int entryAvg     = checkedGames > 0 ? scratch / checkedGames : 0;

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
                    0,     // ADJ AVG — user editable, defaults to 0
                    false,
                    null,  // Squad
                    m.Handicap,
                    m.Bonus,
                    m.SidePot,
                    null   // Notes
                );
                dgvTournament.Rows[rowIdx].Tag = m.GameId;
                ApplySandbaggingHighlight(rowIdx, orig.LeagueAverage);
            }
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
                    SidePot      = b.SidePot,
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
                new DataGridViewTextBoxColumn { Name = "colDetailProPot",      HeaderText = "Pro\nPot",      Width = 50  },
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
                UpdateGameUseFlags(e.RowIndex);
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
            foreach (WinnerListMemberViewModel b in _currentTournamentBowlers.Where(b => b.MemberNumber == memberNumber))
            {
                int g1    = Convert.ToInt32(b.Game1);
                int g2    = Convert.ToInt32(b.Game2);
                int g3    = Convert.ToInt32(b.Game3);
                int g4    = Convert.ToInt32(b.Game4);
                int hdcp  = Convert.ToInt32(b.Handicap);
                int bonus = Convert.ToInt32(b.Bonus);

                int validGames = (g1 > 0 ? 1 : 0) + (g2 > 0 ? 1 : 0) + (g3 > 0 ? 1 : 0) + (g4 > 0 ? 1 : 0);
                int scratch    = g1 + g2 + g3 + g4;
                int wHdcp      = scratch + (validGames * (hdcp + bonus));
                int entry      = validGames > 0 ? scratch / validGames : 0;

                int rowIdx = dgvDetail.Rows.Add(
                    validGames,
                    selectedTournament.Date.ToShortDateString(),
                    g1 > 0 ? (object)g1 : null,
                    g2 > 0 ? (object)g2 : null,
                    g3 > 0 ? (object)g3 : null,
                    g4 > 0 ? (object)g4 : null,
                    scratch,
                    wHdcp,
                    entry,
                    null,                                        // 30 AVG — not yet computed
                    null,                                        // Adjusted AVG
                    hdcp,
                    bonus,
                    b.SidePot > 0 ? (object)b.SidePot : null,
                    null,                                        // Place — not yet finalized
                    b.MoneyWon > 0 ? (object)b.MoneyWon : null,
                    null                                         // Notes
                );
                dgvDetail.Rows[rowIdx].DefaultCellStyle.BackColor = Color.LightBlue;
            }

            // --- Finalized historical rows (ordered by date descending from DB) ---
            for (int i = 0; i < history.Count; i++)
            {
                PlayerHistoryViewModel h = history[i];
                int wHdcp = h.TotalScore + (h.GamesPlayed * (h.HandiCap + h.Bonus));
                int entry = h.GamesPlayed > 0 ? h.TotalScore / h.GamesPlayed : 0;

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
                    !string.IsNullOrEmpty(h.ProPot) && h.ProPot != "0" ? (object)h.ProPot : null,
                    !string.IsNullOrEmpty(h.PPHG) ? (object)h.PPHG : null,
                    h.MoneyWon > 0 ? (object)h.MoneyWon : null,
                    h.Notes
                );

                // Highlight the 30 AVG cell for entries within the rolling 30-game window
                if (i < thirtyGameWindow)
                    dgvDetail.Rows[rowIdx].Cells["colDetail30Avg"].Style.BackColor = Color.LightGreen;
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
