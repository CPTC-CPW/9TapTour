#nullable disable
using ClosedXML.Excel;
using NineTapTour.Core.Models;
using NineTapTour.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NineTapTour.Core.Export;

/// <summary>
/// Headless series report Excel writer. Workbook logic was moved verbatim from
/// FrmTournamentResults.ExportToExcel (M7.2); the form keeps the file dialogs,
/// DB saves, and message boxes.
/// </summary>
public class SeriesReportExcelExporter : ISeriesReportExcelExporter
{
    public List<TemplateEarningsRow> ReadEarningsAndPots(string templatePath, int rowCount)
    {
        var result = new List<TemplateEarningsRow>();

        using var readWb = new XLWorkbook(templatePath);
        var readWs = readWb.Worksheet(1);
        int excelRow = 4;
        for (int idx = 0; idx < rowCount; idx++)
        {
            // Earnings are always in col I of the bowler row.
            // The cell may be a plain number or a currency-formatted string ("$1,100"),
            // so strip "$" and "," before parsing as a fallback.
            var earningsVal = readWs.Cell(excelRow, 9).Value;
            decimal earnings;
            if (earningsVal.IsNumber)
            {
                earnings = (decimal)earningsVal.GetNumber();
            }
            else
            {
                string raw = readWs.Cell(excelRow, 9).GetString()
                    .Replace("$", "").Replace(",", "").Trim();
                decimal.TryParse(raw, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture, out earnings);
            }

            // The merged F/G/H cell on the next row contains "Progressive Pot" when
            // a pot row follows this bowler (always for places 1-3, and sometimes
            // beyond that when there are ties in the top 3).
            bool hasPotRow = readWs.Cell(excelRow + 1, 6).GetString()
                .Contains("Progressive Pot", StringComparison.OrdinalIgnoreCase);

            decimal pot = 0m;
            if (hasPotRow)
            {
                var potVal = readWs.Cell(excelRow + 1, 9).Value;
                pot = potVal.IsNumber ? (decimal)potVal.GetNumber() : 0m;
            }

            result.Add(new TemplateEarningsRow(earnings, pot));
            excelRow += hasPotRow ? 2 : 1;
        }

        return result;
    }

    public void Export(string templatePath, string destinationPath, SeriesReportExportRequest request)
    {
        IReadOnlyList<SeriesReportRow> rows = request.Rows;

        using var workbook = new XLWorkbook(templatePath);
        var ws = workbook.Worksheet(1);
        ws.Cell(1, 1).Value = request.TournamentLocation + request.TournamentEvent;
        ws.Cell(2, 1).Value = request.TournamentDate;

        // For 2-Day tournaments, we need to change "Total Score" header
        if (request.IsTwoDay)
            ws.Cell(3, 7).Value = "Qualifying Score";

        var resultIdxToExcelRow         = new Dictionary<int, int>();
        var excelRowsWithProgressivePot = new HashSet<int>();
        int excelRow = 4;
        int i = 0;
        while (i < rows.Count)
        {
            var row = rows[i];
            int currentPlace = 0;
            string placeDisplay = row.PlaceDisplay ?? "";
            if (request.IsTwoDay)
            {
                if (WinnersService.TryParsePlaceStartFromText(row.PlaceGroupLabel, out int parsedFromLabel))
                    currentPlace = parsedFromLabel;
                else if (WinnersService.TryParsePlaceStartFromText(placeDisplay, out int parsedFromDisplay))
                    currentPlace = parsedFromDisplay;
            }
            else
            {
                int.TryParse(placeDisplay.TrimEnd('T'), out currentPlace);
            }

            // Check for tie: if previous or next row has the same place
            bool isTie = false;
            if (i > 0)
            {
                int prevPlace = 0;
                int.TryParse(rows[i - 1].PlaceDisplay?.TrimEnd('T'), out prevPlace);
                if (prevPlace == currentPlace) isTie = true;
            }
            if (i < rows.Count - 1)
            {
                int nextPlace = 0;
                int.TryParse(rows[i + 1].PlaceDisplay?.TrimEnd('T'), out nextPlace);
                if (nextPlace == currentPlace) isTie = true;
            }
            if (request.IsTwoDay)
            {
                string groupedLabel = row.PlaceGroupLabel;
                ws.Cell(excelRow, 1).Value = string.IsNullOrWhiteSpace(groupedLabel)
                    ? placeDisplay
                    : groupedLabel;
            }
            else
            {
                // Use ordinal with tie for place standing
                ws.Cell(excelRow, 1).Value = WinnersService.GetOrdinalWithTie(currentPlace, isTie);
            }
            ws.Cell(excelRow, 2).Value = row.FullName;
            ws.Cell(excelRow, 6).Value = row.HandicapDisplay;
            ws.Cell(excelRow, 7).Value = row.TotalScoreDisplay;
            resultIdxToExcelRow[i] = excelRow;

            ws.Cell(excelRow, 11).Value = currentPlace;
            ws.Cell(excelRow, 12).Value = row.MemberNumberText;

            // Always explicitly set or clear the Membership$ (Column M) background so that
            // pre-existing orange from a previous export never bleeds into a current member's row.
            bool membershipNotCurrent = int.TryParse(row.MemberNumberText, out int memberNumber)
                && request.IsMembershipCurrentByMemberNumber.TryGetValue(memberNumber, out bool isCurrent)
                && !isCurrent;
            ws.Cell(excelRow, 13).Style.Fill.BackgroundColor =
                membershipNotCurrent ? XLColor.Orange : XLColor.NoColor;

            // Detect a pot row in the template by checking col F of the next row.
            if (ws.Cell(excelRow + 1, 6).GetString()
                    .Contains("Progressive Pot", StringComparison.OrdinalIgnoreCase))
            {
                excelRowsWithProgressivePot.Add(excelRow);
                excelRow++;
            }

            i++;
            excelRow++;
        }

        if (request.ApplyDoublesCheckConsolidation)
        {
            // For doubles: detect bowlers who placed multiple times and skip their duplicate checks.
            var consolidation = BuildDoublesConsolidation(rows);
            if (consolidation.SecondaryRowIndices.Count > 0)
            {
                UpdateCheckSheetsForDoubles(workbook, resultIdxToExcelRow, consolidation.SecondaryRowIndices,
                    excelRowsWithProgressivePot, consolidation.PlaceLabelsForMemo, rows);
            }
        }

        workbook.SaveAs(destinationPath);
        // ClosedXML corrupts check-signature image anchors on save when a sheet has
        // multiple same-named pictures; restore the template's drawings untouched.
        FileHelper.RestoreTemplateDrawings(templatePath, destinationPath);
    }

    /// <summary>
    /// Groups results rows by MemberNumber and identifies members who placed more than once
    /// (possible in doubles when the same person bowled in multiple squads on different teams).
    /// Returns: combined earnings for each multi-placer, the secondary row indices to zero out,
    /// and the ordered place labels to use in the combined check's memo line.
    /// </summary>
    internal static (Dictionary<int, decimal> CombinedEarnings,
             HashSet<int> SecondaryRowIndices,
             Dictionary<int, List<string>> PlaceLabelsForMemo)
        BuildDoublesConsolidation(IReadOnlyList<SeriesReportRow> rows)
    {
        var seen        = new Dictionary<int, int>();           // memberNum → first row index
        var earningsMap = new Dictionary<int, decimal>();
        var labelsMap   = new Dictionary<int, List<string>>();
        var secondary   = new HashSet<int>();

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            if (!int.TryParse(row.MemberNumberText, out int memberNum) || memberNum <= 0)
                continue;

            decimal.TryParse(row.EarningsText, out decimal earn);
            string placeStr = row.PlaceDisplay ?? "";
            int.TryParse(placeStr.TrimEnd('T'), out int placeNum);
            string placeLabel = placeNum > 0 ? WinnersService.GetOrdinalWithTie(placeNum, placeStr.EndsWith("T")) : "";

            if (seen.TryAdd(memberNum, i))
            {
                earningsMap[memberNum] = earn;
                labelsMap[memberNum]   = [placeLabel];
            }
            else
            {
                earningsMap[memberNum] += earn;
                labelsMap[memberNum].Add(placeLabel);
                secondary.Add(i);
            }
        }

        // Only include members who placed more than once
        var combined = earningsMap
            .Where(kv => labelsMap[kv.Key].Count > 1)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        var memoLabels = labelsMap
            .Where(kv => kv.Value.Count > 1)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        return (combined, secondary, memoLabels);
    }

    /// <summary>
    /// For each non-Results worksheet, remaps formulas that reference Results rows belonging to
    /// secondary (duplicate) bowler placements to the correct unique bowler row, and updates
    /// progressive-pot earnings formulas accordingly.  Also writes the combined place-label memo
    /// into the place cell of any single-check sheet that belongs to a multi-placer.
    /// </summary>
    private static void UpdateCheckSheetsForDoubles(
        XLWorkbook workbook,
        Dictionary<int, int> resultIdxToExcelRow,
        HashSet<int> secondaryResultIndices,
        HashSet<int> excelRowsWithProgressivePot,
        Dictionary<int, List<string>> multiPlacerPlaceLabels,
        IReadOnlyList<SeriesReportRow> rows)
    {
        // All bowler excel rows in ascending order (including secondary)
        var allBowlerRowsSorted = resultIdxToExcelRow.Values.OrderBy(r => r).ToList();

        // Secondary excel rows
        var secondaryExcelRows = secondaryResultIndices
            .Where(i => resultIdxToExcelRow.ContainsKey(i))
            .Select(i => resultIdxToExcelRow[i])
            .ToHashSet();

        // Unique sequence: bowler rows excluding secondary, in order
        var uniqueSequence = allBowlerRowsSorted.Where(r => !secondaryExcelRows.Contains(r)).ToList();

        // Map memberNum → primary excel row (first/best placement)
        var memberToPrimaryRow = new Dictionary<int, int>();
        for (int i = 0; i < rows.Count; i++)
        {
            if (!resultIdxToExcelRow.TryGetValue(i, out int exRow)) continue;
            if (secondaryResultIndices.Contains(i)) continue;
            if (!int.TryParse(rows[i].MemberNumberText, out int mn) || mn <= 0) continue;
            memberToPrimaryRow.TryAdd(mn, exRow);
        }

        // Build memo text keyed by primary excel row
        var memoByPrimaryRow = new Dictionary<int, string>();
        foreach (var (mn, labels) in multiPlacerPlaceLabels)
        {
            if (memberToPrimaryRow.TryGetValue(mn, out int exRow))
                memoByPrimaryRow[exRow] = string.Join(", ", labels);
        }

        var allBowlerRowsSet = new HashSet<int>(allBowlerRowsSorted);
        var rowNumPattern    = new Regex(@"Results!([A-Z]+)(\d+)", RegexOptions.IgnoreCase);

        foreach (var ws in workbook.Worksheets)
        {
            if (ws.Name.Equals("Results", StringComparison.OrdinalIgnoreCase)) continue;

            var formulaCells = ws.CellsUsed().Where(c => c.HasFormula).ToList();
            if (formulaCells.Count == 0) continue;

            // Collect all Results bowler-row numbers this sheet references
            var sheetRefRows = new HashSet<int>();
            foreach (var cell in formulaCells)
            {
                foreach (Match m in rowNumPattern.Matches(cell.FormulaA1))
                {
                    if (int.TryParse(m.Groups[2].Value, out int rowNum) && allBowlerRowsSet.Contains(rowNum))
                        sheetRefRows.Add(rowNum);
                }
            }
            if (sheetRefRows.Count == 0) continue;

            // Build old→new remap: position k in allBowlerRowsSorted → uniqueSequence[k]
            var rowRemap = new Dictionary<int, int>();
            foreach (int checkRow in sheetRefRows)
            {
                int pos = allBowlerRowsSorted.IndexOf(checkRow);
                if (pos < 0 || pos >= uniqueSequence.Count) continue;
                int newRow = uniqueSequence[pos];
                if (newRow != checkRow)
                    rowRemap[checkRow] = newRow;
            }

            // Apply formula remaps (process longer row numbers first to avoid partial matches)
            if (rowRemap.Count > 0)
            {
                foreach (var cell in formulaCells)
                {
                    string formula = cell.FormulaA1;
                    string updated = formula;
                    foreach (var (oldRow, newRow) in rowRemap.OrderByDescending(kv => kv.Key))
                    {
                        bool newHasPot = excelRowsWithProgressivePot.Contains(newRow);
                        updated = ApplyRowRemap(updated, oldRow, newRow, newHasPot);
                    }
                    if (!ReferenceEquals(updated, formula) && updated != formula)
                        cell.FormulaA1 = updated;
                }
            }

            // Write the combined-places memo into the place label cell of the primary check.
            // Each check's place is in the C column (e.g. C3, C20, C37) with formula
            // =Results!A{n}. Primary rows are never remapped, so that formula is still
            // intact here — we override the cell value to show all cashed places (e.g. "3rd, 7th").
            foreach (var (primaryRow, memoText) in memoByPrimaryRow)
            {
                if (!sheetRefRows.Contains(primaryRow)) continue;
                foreach (var cell in formulaCells)
                {
                    string f = cell.FormulaA1.Trim();
                    if (f.Equals($"Results!A{primaryRow}", StringComparison.OrdinalIgnoreCase))
                    {
                        cell.Value = memoText;
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Rewrites all Results!{col}{oldRow} references in <paramref name="formula"/> to point to
    /// <paramref name="newRow"/>.  Column I is handled specially: if the new row has a
    /// progressive-pot row below it the earnings formula expands to I{n}+I{n+1}, otherwise
    /// it collapses to just I{n}.  A placeholder character prevents double-substitution.
    /// </summary>
    public static string ApplyRowRemap(string formula, int oldRow, int newRow, bool newHasPot)
    {
        const string ph = "\x01";

        // Neutralize column I (handle progressive-pot combo and single ref uniformly)
        string potCombo = $@"Results!I{oldRow}\s*\+\s*Results!I{oldRow + 1}";
        formula = Regex.Replace(formula, potCombo, ph, RegexOptions.IgnoreCase);
        formula = Regex.Replace(formula, $@"Results!I{oldRow}(?!\d)", ph, RegexOptions.IgnoreCase);

        // Restore I with correct progressive-pot handling
        string iValue = newHasPot
            ? $"Results!I{newRow}+Results!I{newRow + 1}"
            : $"Results!I{newRow}";
        formula = formula.Replace(ph, iValue);

        // Replace all remaining column references for oldRow → newRow (I already resolved above)
        formula = Regex.Replace(
            formula,
            $@"Results!([A-Z]+){oldRow}(?!\d)",
            m => m.Groups[1].Value.Equals("I", StringComparison.OrdinalIgnoreCase)
                ? m.Value
                : $"Results!{m.Groups[1].Value}{newRow}",
            RegexOptions.IgnoreCase);

        return formula;
    }
}
