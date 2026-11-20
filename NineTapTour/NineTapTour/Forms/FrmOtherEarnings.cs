using NineTapTour.Core.Entities;
using NineTapTour.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace NineTapTour.Forms;

/// <summary>
/// Lists and edits a member's other earnings: cash not tied to a tournament
/// entry, such as satellites, Yakima, Hawaii and career totals from before the
/// legacy books. The earnings reports can add these to a member's earnings.
/// </summary>
public class FrmOtherEarnings : Form
{
    private const string ColId = "colId";
    private const string ColDate = "colDate";
    private const string ColDescription = "colDescription";
    private const string ColAmount = "colAmount";
    private const string ColSource = "colSource";

    private readonly Member member;
    private readonly IOtherEarningRepository otherEarningRepository;

    private DataGridView dgvEarnings;
    private Label lblTotal;

    public FrmOtherEarnings(Member member, IOtherEarningRepository otherEarningRepository)
    {
        this.member = member;
        this.otherEarningRepository = otherEarningRepository;
        InitializeControls();
        LoadRows();
    }

    private void InitializeControls()
    {
        SuspendLayout();

        Text = $"Other Earnings — {member.LastName}, {member.FirstName} (#{member.Number})";
        Size = new Size(820, 480);
        MinimumSize = new Size(640, 320);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.Sizable;

        Label lblInfo = new()
        {
            Dock = DockStyle.Top,
            Height = 52,
            Padding = new Padding(8, 6, 8, 0),
            Text = "Cash won with no tournament entry, such as satellites, Yakima, Hawaii and career totals " +
                   "from before the books. It counts toward earnings on the reports, never as an entry. " +
                   "Leave Date blank when it is unknown. Re-running the history import replaces rows that came from a book."
        };

        dgvEarnings = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = true,
            AllowUserToDeleteRows = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersWidth = 25
        };
        dgvEarnings.Columns.AddRange(
            new DataGridViewTextBoxColumn { Name = ColId, Visible = false },
            new DataGridViewTextBoxColumn { Name = ColDate, HeaderText = "Date", Width = 90 },
            new DataGridViewTextBoxColumn
            {
                Name = ColDescription,
                HeaderText = "Description",
                Width = 290,
                MaxInputLength = OtherEarning.DescriptionMaxLength
            },
            new DataGridViewTextBoxColumn
            {
                Name = ColAmount,
                HeaderText = "Amount",
                Width = 90,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
            },
            new DataGridViewTextBoxColumn
            {
                Name = ColSource,
                HeaderText = "From Book",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = { ForeColor = SystemColors.GrayText }
            });
        dgvEarnings.CellEndEdit += (_, _) => UpdateTotal();
        dgvEarnings.UserDeletedRow += (_, _) => UpdateTotal();

        Panel pnlBottom = new() { Dock = DockStyle.Bottom, Height = 44 };

        Button btnDelete = new() { Text = "Delete Row", Size = new Size(100, 28), Location = new Point(8, 8) };
        btnDelete.Click += BtnDelete_Click;

        lblTotal = new Label { AutoSize = true, Location = new Point(124, 14) };

        Button btnSave = new()
        {
            Text = "Save",
            Size = new Size(90, 28),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnSave.Click += BtnSave_Click;

        Button btnCancel = new()
        {
            Text = "Cancel",
            Size = new Size(90, 28),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            DialogResult = DialogResult.Cancel
        };

        pnlBottom.Controls.AddRange([btnDelete, lblTotal, btnSave, btnCancel]);
        pnlBottom.Resize += (_, _) =>
        {
            btnCancel.Location = new Point(pnlBottom.ClientSize.Width - btnCancel.Width - 8, 8);
            btnSave.Location = new Point(btnCancel.Left - btnSave.Width - 8, 8);
        };

        // Fill first, then the docked edges, so the grid sits between them
        Controls.Add(dgvEarnings);
        Controls.Add(pnlBottom);
        Controls.Add(lblInfo);

        AcceptButton = btnSave;
        CancelButton = btnCancel;

        ResumeLayout(true);
    }

    private void LoadRows()
    {
        foreach (OtherEarning row in otherEarningRepository.GetForMember(member.Number))
        {
            dgvEarnings.Rows.Add(
                row.Id,
                row.Date?.ToString("M/d/yyyy") ?? "",
                row.Description,
                row.Amount.ToString("N2"),
                row.IsImported ? row.Source : "Entered by hand");
        }
        UpdateTotal();
    }

    private void UpdateTotal()
    {
        decimal total = GridRows()
            .Select(r => TryParseAmount(r.Cells[ColAmount].Value, out decimal amount) ? amount : 0)
            .Sum();
        lblTotal.Text = $"Total: {total:C2}";
    }

    private IEnumerable<DataGridViewRow> GridRows()
    {
        return dgvEarnings.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow);
    }

    private void BtnDelete_Click(object sender, EventArgs e)
    {
        foreach (DataGridViewRow row in dgvEarnings.SelectedRows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow).ToList())
        {
            dgvEarnings.Rows.Remove(row);
        }
        UpdateTotal();
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        dgvEarnings.EndEdit();

        List<OtherEarning> rows = [];
        foreach (DataGridViewRow row in GridRows())
        {
            string description = Convert.ToString(row.Cells[ColDescription].Value)?.Trim() ?? "";
            string dateText = Convert.ToString(row.Cells[ColDate].Value)?.Trim() ?? "";
            object amountValue = row.Cells[ColAmount].Value;

            // Skip rows the user started but left completely blank
            if (description.Length == 0 && dateText.Length == 0 && string.IsNullOrWhiteSpace(Convert.ToString(amountValue)))
            {
                continue;
            }

            if (!TryParseAmount(amountValue, out decimal amount) || amount == 0)
            {
                ShowRowError(row, ColAmount, "Enter a non-zero amount, such as 125 or $125.50.");
                return;
            }

            DateTime? date = null;
            if (dateText.Length > 0)
            {
                if (!DateTime.TryParse(dateText, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime parsed))
                {
                    ShowRowError(row, ColDate, "Enter a date such as 10/4/2014, or leave it blank when unknown.");
                    return;
                }
                date = parsed.Date;
            }

            int id = row.Cells[ColId].Value is int existingId ? existingId : 0;
            rows.Add(new OtherEarning { Id = id, Date = date, Description = description, Amount = amount });
        }

        try
        {
            otherEarningRepository.SaveForMember(member.Number, rows);
        }
        catch (Exception ex)
        {
            MessageBox.Show("The other earnings could not be saved:\n" + ex.Message,
                "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void ShowRowError(DataGridViewRow row, string column, string message)
    {
        dgvEarnings.CurrentCell = row.Cells[column];
        MessageBox.Show(message, "Check This Row", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private static bool TryParseAmount(object value, out decimal amount)
    {
        return decimal.TryParse(Convert.ToString(value), NumberStyles.Currency, CultureInfo.CurrentCulture, out amount);
    }
}
