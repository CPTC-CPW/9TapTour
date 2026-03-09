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
            List<WinnerListMemberViewModel> bowlers = TournamentDB.GetWinnerListMemberData(selectedTournament.Id);
            List<ExcelMember> members = BuildExcelMemberList(bowlers);

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

            // Display order: group all entries for the same player together by name,
            // placed entry first within each group, then unplaced duplicates by score
            members.Sort((x, y) =>
            {
                int nameCompare = string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
                if (nameCompare != 0) return nameCompare;

                bool xPlaced = x.PlaceStanding > 0;
                bool yPlaced = y.PlaceStanding > 0;
                if (xPlaced && !yPlaced) return -1;
                if (!xPlaced && yPlaced) return 1;
                if (xPlaced)             return x.PlaceStanding.CompareTo(y.PlaceStanding);
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
    }
}
