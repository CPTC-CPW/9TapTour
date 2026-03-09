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
        private int regionID;

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

        public FrmFinalizeTournament(Tournament selectedTournament, int regionID)
        {
            this.selectedTournament = selectedTournament;
            this.regionID = regionID;

            InitializeComponent();
        }

        private void FrmFinalizeTournament_Load(object sender, EventArgs e)
        {
            BuildGrids();
            LoadTournamentGrid();
            dgvTournament.SelectionChanged += DgvTournament_SelectionChanged;
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
            btnFinalizeTournament.Location = new Point(ClientSize.Width - btnFinalizeTournament.Width - 12, 7);

            pnlToolbar.Controls.AddRange([chkDirCheck, chkAdjAvg, btnFinalizeTournament]);

            // --- SplitContainer (top grid / bottom grid) ---
            splitMain = new SplitContainer
            {
                Dock        = DockStyle.Fill,
                Orientation = Orientation.Horizontal
            };

            // Top grid
            dgvTournament = CreateTournamentGrid();
            dgvTournament.CurrentCellDirtyStateChanged += DgvTournament_CurrentCellDirtyStateChanged;
            dgvTournament.CellValueChanged             += DgvTournament_CellValueChanged;
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
            splitMain.Panel2.Controls.Add(dgvDetail);
            splitMain.Panel2.Controls.Add(pnlPlayerInfo);

            // Add to form — Fill first, Top-docked toolbar second so toolbar is laid out first
            Controls.Add(splitMain);
            Controls.Add(pnlToolbar);

            ResumeLayout(true);

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

            dgvTournament.Rows.Clear();
            foreach (ExcelMember m in members)
            {
                // Default: a game is checked when its score is non-zero
                bool g1Checked = m.Game1Score > 0;
                bool g2Checked = m.Game2Score > 0;
                bool g3Checked = m.Game3Score > 0;
                bool g4Checked = m.Game4Score > 0;

                // 3-of-4: uncheck the lowest-scoring game when all 4 are present
                if (selectedTournament.ThreeOutOf4)
                {
                    var validScores = new List<(int score, int game)>
                    {
                        (m.Game1Score, 1), (m.Game2Score, 2), (m.Game3Score, 3), (m.Game4Score, 4)
                    }.Where(x => x.score > 0).ToList();

                    if (validScores.Count == 4)
                    {
                        int lowestGame = validScores.MinBy(x => x.score).game;
                        if      (lowestGame == 1) g1Checked = false;
                        else if (lowestGame == 2) g2Checked = false;
                        else if (lowestGame == 3) g3Checked = false;
                        else if (lowestGame == 4) g4Checked = false;
                    }
                }

                int scratch      = (g1Checked ? m.Game1Score : 0) + (g2Checked ? m.Game2Score : 0)
                                 + (g3Checked ? m.Game3Score : 0) + (g4Checked ? m.Game4Score : 0);
                int checkedGames = (g1Checked ? 1 : 0) + (g2Checked ? 1 : 0)
                                 + (g3Checked ? 1 : 0) + (g4Checked ? 1 : 0);
                int hdcpTotal    = scratch + (checkedGames * (m.Handicap + m.Bonus));
                int entryAvg     = checkedGames > 0 ? scratch / checkedGames : 0;

                dgvTournament.Rows.Add(
                    m.PlaceStanding > 0 ? (object)m.PlaceStanding : null,  // blank for duplicate entries
                    m.MemberNumber,
                    m.Name,
                    m.Game1Score > 0 ? (object)m.Game1Score : null,
                    g1Checked,
                    m.Game2Score > 0 ? (object)m.Game2Score : null,
                    g2Checked,
                    m.Game3Score > 0 ? (object)m.Game3Score : null,
                    g3Checked,
                    m.Game4Score > 0 ? (object)m.Game4Score : null,
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
                    // Compute TotalScore from the top 3 valid games without modifying game scores,
                    // so all 4 raw scores remain available for display in the grid.
                    List<int> scores = new[] { m.Game1Score, m.Game2Score, m.Game3Score, m.Game4Score }
                        .Where(x => x > 0).ToList();
                    if (scores.Count == 4)
                        scores.Remove(scores.Min());
                    m.TotalScore = scores.Sum() + (scores.Count * (m.Handicap + m.Bonus));
                }
                else
                {
                    int validGames = 0;
                    if (m.Game1Score > 0) validGames++;
                    if (m.Game2Score > 0) validGames++;
                    if (m.Game3Score > 0) validGames++;
                    if (m.Game4Score > 0) validGames++;
                    m.TotalScore = m.Game1Score + m.Game2Score + m.Game3Score + m.Game4Score
                                 + (validGames * (m.Handicap + m.Bonus));
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

            int g1 = GetCheckedScore("colGame1", "colGame1Check");
            int g2 = GetCheckedScore("colGame2", "colGame2Check");
            int g3 = GetCheckedScore("colGame3", "colGame3Check");
            int g4 = GetCheckedScore("colGame4", "colGame4Check");

            int scratch      = g1 + g2 + g3 + g4;
            int checkedGames = (g1 > 0 ? 1 : 0) + (g2 > 0 ? 1 : 0) + (g3 > 0 ? 1 : 0) + (g4 > 0 ? 1 : 0);
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
                RecalculateTournamentRow(e.RowIndex);
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
        /// Populates <see cref="dgvDetail"/> with all tournament entries for the given member.
        /// Current-tournament rows appear at the top with a blue highlight.
        /// The 30 AVG cell is highlighted light green for the most recent 30 finalized entries.
        /// </summary>
        private void LoadDetailGrid(int memberNumber, string memberName)
        {
            const int thirtyGameWindow = 30;

            List<PlayerHistoryViewModel> history =
                PlayerHistoryDB.GetMemberPlayerHistory(memberNumber, regionID);

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
                    h.Game1 > 0 ? (object)h.Game1 : null,
                    h.Game2 > 0 ? (object)h.Game2 : null,
                    h.Game3 > 0 ? (object)h.Game3 : null,
                    h.Game4 > 0 ? (object)h.Game4 : null,
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

            UpdateDetailGridHeaders(history.Take(thirtyGameWindow).ToList());

            double leagueAvg = history.FirstOrDefault()?.trueAVG ?? 0;
            lblPlayerInfo.Text = $"Mem#   {memberNumber}        {memberName,-35}AVG   {(int)Math.Round(leagueAvg)}";
        }

        /// <summary>
        /// Updates the column headers of <see cref="dgvDetail"/> to show the sum (or computed
        /// average) of the provided entries — typically the last 30 finalized games.
        /// </summary>
        private void UpdateDetailGridHeaders(List<PlayerHistoryViewModel> last30)
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
                dgvDetail.Columns["colDetailEarnings"].HeaderText = "Earnings";
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
            decimal earnings   = last30.Sum(h => h.MoneyWon);

            dgvDetail.Columns["colDetailGames"].HeaderText    = $"Games\n({totalGames})";
            dgvDetail.Columns["colDetailGame1"].HeaderText    = $"Game1\n({game1Sum})";
            dgvDetail.Columns["colDetailGame2"].HeaderText    = $"Game2\n({game2Sum})";
            dgvDetail.Columns["colDetailGame3"].HeaderText    = $"Game3\n({game3Sum})";
            dgvDetail.Columns["colDetailGame4"].HeaderText    = $"Game4\n({game4Sum})";
            dgvDetail.Columns["colDetailScratch"].HeaderText  = $"Scratch\n({scratch})";
            dgvDetail.Columns["colDetailWHdcp"].HeaderText    = $"w/HDCP\n({wHdcpSum})";
            dgvDetail.Columns["colDetailEntry"].HeaderText    = $"Entry\n({entryAvg})";
            dgvDetail.Columns["colDetail30Avg"].HeaderText    = $"30 AVG\n({avg30:0.#})";
            dgvDetail.Columns["colDetailEarnings"].HeaderText = $"Earnings\n({earnings:0.00})";
        }
    }
}
