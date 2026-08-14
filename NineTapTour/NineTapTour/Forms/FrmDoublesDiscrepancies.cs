using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NineTapTour.Forms;

/// <summary>
/// Lists all pairing discrepancies for the tournament and provides quick-fix actions.
/// Two types of discrepancy are shown:
///   1. Missing Reciprocal — Claim A→B exists but B→A does not.
///   2. Count Mismatch     — A bowler's planned partner count differs from their actual claim count.
/// </summary>
public class FrmDoublesDiscrepancies : Form
{
    private readonly Tournament _tournament;
    private readonly IDoublesPairingService doublesPairingService;

    private Label          lblTitle;
    private DataGridView   dgvIssues;
    private Label          lblStatus;
    private Button         btnFixAllReciprocals;
    private Button         btnClose;

    // ----------------------------------------------------------------
    // Construction
    // ----------------------------------------------------------------

    public FrmDoublesDiscrepancies(Tournament tournament, IDoublesPairingService doublesPairingService)
    {
        _tournament = tournament;
        this.doublesPairingService = doublesPairingService;
        InitializeControls();
        LoadDiscrepancies();
    }

    private void InitializeControls()
    {
        SuspendLayout();

        Text            = "Fix Pairing Discrepancies";
        Size            = new Size(880, 520);
        MinimumSize     = new Size(680, 380);
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.Sizable;

        // --- Title ---
        lblTitle = new Label
        {
            Text      = $"Pairing Discrepancies \u2014 {_tournament.TourneyNameDate}",
            Font      = new Font("Arial", 11, FontStyle.Bold),
            Dock      = DockStyle.Top,
            Height    = 34,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(8, 0, 0, 0)
        };

        // --- Bottom bar ---
        var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 44 };

        btnFixAllReciprocals = new Button
        {
            Text = "Fix All Reciprocals",
            Size = new Size(138, 28),
            Location = new Point(8, 8)
        };
        btnFixAllReciprocals.Click += BtnFixAllReciprocals_Click;

        lblStatus = new Label
        {
            Location  = new Point(272, 14),
            AutoSize  = true,
            Text      = string.Empty,
            ForeColor = Color.DarkGreen
        };

        btnClose = new Button
        {
            Text = "Close",
            Size = new Size(88, 28)
        };
        // Right-align close button; re-position on resize
        pnlBottom.Resize += (s, e) =>
            btnClose.Location = new Point(pnlBottom.Width - btnClose.Width - 8,
                                         (pnlBottom.Height - btnClose.Height) / 2);
        btnClose.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };

        pnlBottom.Controls.Add(btnFixAllReciprocals);
        pnlBottom.Controls.Add(lblStatus);
        pnlBottom.Controls.Add(btnClose);

        // --- Grid ---
        dgvIssues = new DataGridView
        {
            Dock                  = DockStyle.Fill,
            ReadOnly              = true,
            AllowUserToAddRows    = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible     = false,
            SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        };

        // Hidden data columns
        dgvIssues.Columns.Add(new DataGridViewTextBoxColumn { Name = "colType",       HeaderText = "colType",       Visible = false });
        dgvIssues.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSrcId",       HeaderText = "colSrcId",       Visible = false });
        dgvIssues.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPartId",      HeaderText = "colPartId",      Visible = false });
        dgvIssues.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSquadData",   HeaderText = "colSquadData",   Visible = false });
        dgvIssues.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPlanned",     HeaderText = "colPlanned",     Visible = false });
        dgvIssues.Columns.Add(new DataGridViewTextBoxColumn { Name = "colActual",      HeaderText = "colActual",      Visible = false });

        // Visible columns
        dgvIssues.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTypeDisplay", HeaderText = "Type",           FillWeight = 22 });
        dgvIssues.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSquad",       HeaderText = "Squad",          FillWeight = 8  });
        dgvIssues.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBowlerNum",   HeaderText = "Bowler #",       FillWeight = 10 });
        dgvIssues.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBowlerName",  HeaderText = "Bowler Name",    FillWeight = 22 });
        dgvIssues.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDetails",     HeaderText = "Details",        FillWeight = 38 });

        var colFix = new DataGridViewButtonColumn
        {
            Name                        = "colFix",
            HeaderText                  = "Fix",
            Text                        = "Fix",
            UseColumnTextForButtonValue = false,
            FillWeight                  = 14
        };
        dgvIssues.Columns.Add(colFix);

        var colRemove = new DataGridViewButtonColumn
        {
            Name                        = "colRemove",
            HeaderText                  = string.Empty,
            Text                        = "Remove Claim",
            UseColumnTextForButtonValue = true,
            FillWeight                  = 16
        };
        dgvIssues.Columns.Add(colRemove);

        dgvIssues.CellFormatting += DgvIssues_CellFormatting;
        dgvIssues.CellClick      += DgvIssues_CellClick;

        Controls.Add(dgvIssues);
        Controls.Add(pnlBottom);
        Controls.Add(lblTitle);

        ResumeLayout(false);
    }

    // ----------------------------------------------------------------
    // Data loading
    // ----------------------------------------------------------------

    private void LoadDiscrepancies()
    {
        dgvIssues.Rows.Clear();
        lblStatus.Text = string.Empty;

        foreach (DoublesDiscrepancy item in doublesPairingService.GetDiscrepancies(_tournament.Id))
            AddRow(item);

        UpdateBulkButtons();
    }

    private void AddRow(DoublesDiscrepancy item)
    {
        string typeDisplay, details, fixText;
        if (item.Type == DoublesDiscrepancyType.MissingReciprocal)
        {
            typeDisplay = "Missing Reciprocal";
            details     = $"#{item.SourceMemberNumber} claims #{item.PartnerMemberNumber} ({item.PartnerMemberName}), but #{item.PartnerMemberNumber} doesn't claim back";
            fixText     = "Add Reciprocal";
        }
        else
        {
            typeDisplay = "Count Mismatch";
            details     = $"Planned {item.PlannedCount}, Entered {item.ActualCount}";
            fixText     = string.Empty;
        }

        int rowIndex = dgvIssues.Rows.Add(
            (int)item.Type,           // colType (hidden)
            item.SourceMemberId,      // colSrcId (hidden)
            item.PartnerMemberId,     // colPartId (hidden)
            item.Squad,               // colSquadData (hidden)
            item.PlannedCount,        // colPlanned (hidden)
            item.ActualCount,         // colActual (hidden)
            typeDisplay,              // colTypeDisplay
            item.Squad,               // colSquad
            item.SourceMemberNumber,  // colBowlerNum
            item.SourceMemberName,    // colBowlerName
            details                   // colDetails
        );

        // Button text is set via CellFormatting below
        var row = dgvIssues.Rows[rowIndex];
        row.Cells["colFix"].Value    = fixText;
        row.Cells["colRemove"].Value = item.Type == DoublesDiscrepancyType.MissingReciprocal
            ? "Remove Claim"
            : string.Empty;
    }

    // ----------------------------------------------------------------
    // Grid formatting
    // ----------------------------------------------------------------

    private void DgvIssues_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgvIssues.Rows[e.RowIndex];

        // Colour code row by type
        var type = (DoublesDiscrepancyType)(int)row.Cells["colType"].Value;
        if (e.ColumnIndex == dgvIssues.Columns["colTypeDisplay"].Index)
        {
            e.CellStyle.ForeColor = type == DoublesDiscrepancyType.MissingReciprocal
                ? Color.DarkOrange
                : Color.DarkBlue;
            e.FormattingApplied = true;
        }

        // Hide Remove button cell for CountMismatch rows
        if (e.ColumnIndex == dgvIssues.Columns["colRemove"].Index)
        {
            if (type == DoublesDiscrepancyType.CountMismatch)
            {
                e.Value             = string.Empty;
                e.FormattingApplied = true;
            }
        }
    }

    // ----------------------------------------------------------------
    // Cell click — per-row fix actions
    // ----------------------------------------------------------------

    private void DgvIssues_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        var row  = dgvIssues.Rows[e.RowIndex];
        var type = (DoublesDiscrepancyType)(int)row.Cells["colType"].Value;
        int srcId   = (int)row.Cells["colSrcId"].Value;
        int partId  = (int)row.Cells["colPartId"].Value;
        int squad   = (int)row.Cells["colSquadData"].Value;

        int colFixIdx    = dgvIssues.Columns["colFix"].Index;
        int colRemoveIdx = dgvIssues.Columns["colRemove"].Index;

        if (e.ColumnIndex == colFixIdx)
        {
            if (type == DoublesDiscrepancyType.MissingReciprocal)
            {
                doublesPairingService.FixReciprocal(_tournament.Id, srcId, partId, squad);
                LoadDiscrepancies();
            }
        }
        else if (e.ColumnIndex == colRemoveIdx && type == DoublesDiscrepancyType.MissingReciprocal)
        {
            doublesPairingService.RemoveClaimAndTeam(_tournament.Id, srcId, partId, squad);
            LoadDiscrepancies();
        }
    }

    // ----------------------------------------------------------------
    // Bulk actions
    // ----------------------------------------------------------------

    private void BtnFixAllReciprocals_Click(object sender, EventArgs e)
    {
        int fixed_ = doublesPairingService.FixAllMissingReciprocals(_tournament.Id);

        ShowStatus($"{fixed_} reciprocal(s) added.");
        LoadDiscrepancies();
    }

    // ----------------------------------------------------------------
    // Helpers
    // ----------------------------------------------------------------

    private void UpdateBulkButtons()
    {
        bool hasReciprocal = false;
        foreach (DataGridViewRow row in dgvIssues.Rows)
        {
            var type = (DoublesDiscrepancyType)(int)row.Cells["colType"].Value;
            if (type == DoublesDiscrepancyType.MissingReciprocal) hasReciprocal = true;
        }

        btnFixAllReciprocals.Enabled = hasReciprocal;
    }

    private void ShowStatus(string message)
    {
        lblStatus.Text      = message;
        lblStatus.ForeColor = Color.DarkGreen;
    }
}
