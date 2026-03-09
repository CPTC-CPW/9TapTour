using NineTapTour.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
                new DataGridViewTextBoxColumn  { Name = "colAdjAvg",       HeaderText = "ADJ\nAVG",        Width = 50,  ReadOnly = true },
                new DataGridViewCheckBoxColumn { Name = "colDirCheck",     HeaderText = "Director\nCheck", Width = 58  },
                new DataGridViewTextBoxColumn  { Name = "colSquad",        HeaderText = "Squad",           Width = 45,  ReadOnly = true },
                new DataGridViewTextBoxColumn  { Name = "colHdcp",         HeaderText = "HDCP",            Width = 45,  ReadOnly = true },
                new DataGridViewTextBoxColumn  { Name = "colBonus",        HeaderText = "Bonus",           Width = 45  },
                new DataGridViewTextBoxColumn  { Name = "colProPot",       HeaderText = "Pro\nPot",        Width = 45  },
                new DataGridViewTextBoxColumn  { Name = "colNotes",        HeaderText = "Notes",           AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill }
            );

            return dgv;
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
    }
}
