using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Calculations;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Export;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.ViewModels;

namespace NineTapTour.Core.Services;

public class ResultsService : IResultsService
{
    private readonly ITournamentRepository tournamentRepository;
    private readonly IWinnersService winnersService;
    private readonly ISeriesReportExcelExporter seriesReportExporter;
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public ResultsService(
        ITournamentRepository tournamentRepository,
        IWinnersService winnersService,
        ISeriesReportExcelExporter seriesReportExporter,
        IDbContextFactory<NineTapDb> dbFactory)
    {
        this.tournamentRepository = tournamentRepository;
        this.winnersService = winnersService;
        this.seriesReportExporter = seriesReportExporter;
        this.dbFactory = dbFactory;
    }

    public ResultsGridModel BuildResults(int tournamentId, int requestedCount)
    {
        Tournament tournament = GetTournament(tournamentId);
        WinnersListResult winnersResult = winnersService.BuildWinnersList(
            new WinnersListRequest(tournament.Id, tournament.Doubles, tournament.ThreeOutOf4));
        List<ExcelMember> winners = winnersResult.Winners;

        // For doubles, requestedCount = number of teams. Both members of each team share
        // the same PlaceStanding, so filter directly instead of re-ranking by TotalScore,
        // and keep the consecutive-pair order [T1M1, T1M2, T2M1, T2M2, ...].
        List<ExcelMember> clientRequested = tournament.Doubles
            ? [.. winners.Where(m => m.PlaceStanding <= requestedCount)]
            : TournamentCalculations.MakeTopMembersByPlacementList(winners, requestedCount);

        int gridRowCount = tournament.Doubles ? requestedCount * 2 : requestedCount;

        List<ResultRow> rows = [];
        foreach (ExcelMember winner in clientRequested)
        {
            rows.Add(new ResultRow
            {
                PlaceDisplay = winner.PlaceStanding.ToString(),
                FullName = winner.Name,
                HandicapDisplay = winner.Handicap + " + " + winner.Bonus,
                TotalScoreDisplay = winner.TotalScore.ToString(),
                MemberNumber = winner.MemberNumber,
                GameId = winner.GameId,
                // The desktop grid showed earnings as whole dollars.
                Earnings = Convert.ToInt32(winner.MoneyWon ?? 0),
                ProgressivePotText = winner.SidePot?.ToString() ?? "0.00",
            });
        }

        for (int fillerIndex = clientRequested.Count; fillerIndex < gridRowCount; fillerIndex++)
        {
            rows.Add(new ResultRow
            {
                PlaceDisplay = WinnersService.ComputeFillerPlace(tournament.Doubles, clientRequested.Count, fillerIndex).ToString(),
                Earnings = 0,
                ProgressivePotText = "0.00",
            });
        }

        List<string> markedPlaces = WinnersService.ApplyTieMarkers([.. rows.Select(r => r.PlaceDisplay)]);
        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].PlaceDisplay = markedPlaces[i];
        }

        return new ResultsGridModel
        {
            TournamentId = tournament.Id,
            IsDoubles = tournament.Doubles,
            RequestedCount = requestedCount,
            TotalEntries = winnersResult.TotalEntries,
            CompEntries = winnersResult.CompEntries,
            Rows = rows,
            Winners = winners,
        };
    }

    public ResultsSaveOutcome SaveResults(int tournamentId, IReadOnlyList<ResultRowSave> rows)
    {
        List<string> errors = [];
        int saved = 0;
        HashSet<int> savedGameIds = [];

        using NineTapDb db = dbFactory.CreateDbContext();
        foreach (ResultRowSave row in rows)
        {
            // A member on multiple teams in the same squad shares one Game record; the first row wins.
            if (row.GameId <= 0 || !savedGameIds.Add(row.GameId))
            {
                continue;
            }

            Game? game = db.Games.Find(row.GameId);
            if (game == null)
            {
                errors.Add($"Game {row.GameId} was not found.");
                continue;
            }

            game.PlaceStanding = WinnersService.ParsePlaceStanding(row.PlaceDisplay);
            game.PlaceStandingLabel = null;
            game.MoneyWon = row.Earnings;
            ApplyProgressivePot(game, row.ProgressivePotText);
            saved++;
        }

        db.SaveChanges();
        return new ResultsSaveOutcome(saved, errors);
    }

    public List<TeamResultRow> BuildTeamView(ResultsGridModel grid)
    {
        List<DoublesTeamPairing> pairs = WinnersService.BuildTeamPairings(grid.Winners, grid.RequestedCount);
        return [.. pairs.Select(pair => new TeamResultRow(
            pair.Place,
            pair.IsTie,
            pair.Member1.Name,
            $"{pair.Member1.Handicap} + {pair.Member1.Bonus}",
            pair.Member2.Name,
            $"{pair.Member2.Handicap} + {pair.Member2.Bonus}",
            pair.Member1.TotalScore,   // combined total is the same for both members
            pair.Member1.MoneyWon ?? 0m,
            pair.Member2.MoneyWon ?? 0m,
            pair.Member1.SidePot ?? 0m,
            pair.Member2.SidePot ?? 0m))];
    }

    public List<TwoDayRow> LoadTwoDay(int tournamentId)
    {
        List<WinnerListMemberViewModel> saved = [.. tournamentRepository.GetWinnerListMemberData(tournamentId)
            .Where(b => b.PlaceStanding > 0)
            .OrderBy(b => b.PlaceStanding)];

        List<TwoDayRow> rows = [];
        foreach (WinnerListMemberViewModel bowler in saved)
        {
            int placeStart = bowler.PlaceStanding ?? 0;
            string placeLabel = string.IsNullOrWhiteSpace(bowler.PlaceStandingLabel)
                ? (placeStart > 0 ? WinnersService.GetOrdinalWithTie(placeStart, false) : string.Empty)
                : bowler.PlaceStandingLabel;

            TwoDayRow row = new()
            {
                PlaceLabel = placeLabel,
                PlaceSortStart = placeStart,
                MemberNumber = bowler.MemberNumber,
                Earnings = bowler.MoneyWon ?? 0m,
                ProgressivePotText = bowler.SidePot?.ToString("F2") ?? "0.00",
                GameId = bowler.GameId,
            };
            AutoFillTwoDayRow(tournamentId, row);
            rows.Add(row);
        }

        return rows;
    }

    public List<TwoDayRow> AddTwoDayRound(int startPlace, int endPlace, decimal earnings)
    {
        if (endPlace < startPlace)
        {
            throw new ArgumentException("End place must be greater than or equal to start place.", nameof(endPlace));
        }

        string label = WinnersService.Build2DayPlaceGroupLabel(startPlace, endPlace);
        List<TwoDayRow> rows = [];
        for (int place = startPlace; place <= endPlace; place++)
        {
            rows.Add(new TwoDayRow
            {
                PlaceLabel = label,
                PlaceSortStart = startPlace,
                Earnings = earnings,
                ProgressivePotText = "0.00",
            });
        }
        return rows;
    }

    public void AutoFillTwoDayRow(int tournamentId, TwoDayRow row)
    {
        row.StatusMessage = null;
        if (row.MemberNumber is not int memberNumber || memberNumber <= 0)
        {
            return;
        }

        TwoDayAutoFillResult fill = winnersService.AutoFillTwoDayMember(memberNumber, tournamentId);
        switch (fill.Status)
        {
            case TwoDayAutoFillStatus.MemberNotFound:
                row.StatusMessage = $"Member number {memberNumber} not found.";
                ClearAutoFill(row);
                return;
            case TwoDayAutoFillStatus.GameNotFound:
                row.StatusMessage = $"No game entry found for member {memberNumber} in this 2-day tournament. " +
                    "Make sure scores have been entered in Member Scores first.";
                ClearAutoFill(row);
                return;
        }

        row.FullName = fill.FullName;
        row.HandicapDisplay = fill.HandicapDisplay;
        row.TotalScore = fill.TotalScore;
        row.GameId = fill.GameId;
    }

    public ResultsSaveOutcome SaveTwoDay(int tournamentId, IReadOnlyList<TwoDayRowSave> rows)
    {
        List<string> errors = [];
        int saved = 0;

        using NineTapDb db = dbFactory.CreateDbContext();
        for (int i = 0; i < rows.Count; i++)
        {
            TwoDayRowSave row = rows[i];
            if (row.GameId is not int gameId || gameId <= 0)
            {
                continue;
            }

            if (!WinnersService.TryParsePlaceStartFromText(row.PlaceLabel, out int placeStart))
            {
                errors.Add($"Invalid place grouping on row {i + 1}. Use a numeric place or a range like 46th - 59th.");
                continue;
            }

            Game? game = db.Games.Find(gameId);
            if (game == null)
            {
                continue;
            }

            game.PlaceStanding = placeStart;
            game.PlaceStandingLabel = row.PlaceLabel.Trim();
            game.MoneyWon = row.Earnings;
            ApplyProgressivePot(game, row.ProgressivePotText);
            saved++;
        }

        db.SaveChanges();
        return new ResultsSaveOutcome(saved, errors);
    }

    public ResultsExportOutcome Export(int tournamentId, string templatePath, Stream destination,
        IReadOnlyList<ResultRowSave> rows, bool teamView)
    {
        Tournament tournament = GetTournament(tournamentId);
        List<ResultRowSave> working = [.. rows];

        // Read earnings and progressive pot from the pre-filled template so the
        // database records what the client typed into the check sheet.
        try
        {
            List<TemplateEarningsRow> templateRows = seriesReportExporter.ReadEarningsAndPots(templatePath, working.Count);
            for (int i = 0; i < templateRows.Count && i < working.Count; i++)
            {
                working[i] = working[i] with
                {
                    Earnings = templateRows[i].Earnings,
                    ProgressivePotText = templateRows[i].ProgressivePot.ToString(),
                };
            }
        }
        catch
        {
            // If the template cannot be read, proceed with the posted values as-is.
        }

        ResultsSaveOutcome save = SaveResults(tournamentId, working);

        // Rebuild from the database so team rows reflect the earnings just saved.
        int requestedCount = tournament.Doubles ? Math.Max(1, working.Count / 2) : Math.Max(1, working.Count);
        ResultsGridModel grid = BuildResults(tournamentId, requestedCount);
        Dictionary<int, ResultRowSave> savedByGameId = working.Where(r => r.GameId > 0).GroupBy(r => r.GameId)
            .ToDictionary(g => g.Key, g => g.First());

        List<SeriesReportRow> exportRows = [];
        if (tournament.Doubles && teamView)
        {
            foreach (TeamResultRow team in BuildTeamView(grid))
            {
                exportRows.Add(new SeriesReportRow(
                    team.Place.ToString(),
                    null,
                    $"{team.Member1Name} & {team.Member2Name}",
                    string.Empty, // handicap intentionally blank in team export
                    team.CombinedTotal.ToString(),
                    string.Empty,
                    team.CombinedEarnings.ToString()));
            }
        }
        else
        {
            foreach (ResultRow row in grid.Rows)
            {
                decimal earnings = row.GameId > 0 && savedByGameId.TryGetValue(row.GameId, out ResultRowSave? posted)
                    ? posted.Earnings
                    : row.Earnings;
                exportRows.Add(new SeriesReportRow(
                    row.PlaceDisplay,
                    null,
                    row.FullName,
                    row.HandicapDisplay,
                    row.TotalScoreDisplay,
                    row.MemberNumber?.ToString() ?? string.Empty,
                    earnings.ToString()));
            }
        }

        WriteWorkbook(tournament, templatePath, destination, exportRows,
            applyDoublesCheckConsolidation: tournament.Doubles && !teamView && !tournament.IsTwoDay);

        return new ResultsExportOutcome(BuildExportFileName(tournament, templatePath), save);
    }

    public ResultsExportOutcome ExportTwoDay(int tournamentId, string templatePath, Stream destination,
        IReadOnlyList<TwoDayRow> rows)
    {
        Tournament tournament = GetTournament(tournamentId);

        ResultsSaveOutcome save = SaveTwoDay(tournamentId,
            [.. rows.Select(r => new TwoDayRowSave(r.GameId, r.PlaceLabel, r.Earnings, r.ProgressivePotText))]);

        List<SeriesReportRow> exportRows = [.. rows
            .OrderBy(r => r.PlaceSortStart)
            .ThenByDescending(r => r.TotalScore ?? 0)
            .Select(r => new SeriesReportRow(
                r.PlaceLabel,
                r.PlaceLabel,
                r.FullName,
                r.HandicapDisplay,
                r.TotalScore?.ToString() ?? string.Empty,
                r.MemberNumber?.ToString() ?? string.Empty,
                r.Earnings.ToString()))];

        WriteWorkbook(tournament, templatePath, destination, exportRows, applyDoublesCheckConsolidation: false);
        return new ResultsExportOutcome(BuildExportFileName(tournament, templatePath), save);
    }

    public string BuildExportFileName(int tournamentId, string templatePath)
    {
        return BuildExportFileName(GetTournament(tournamentId), templatePath);
    }

    private static string BuildExportFileName(Tournament tournament, string templatePath)
    {
        string templateExtension = Path.GetExtension(templatePath).ToLowerInvariant();
        string outputExtension = templateExtension == ".xlsm" ? ".xlsm" : ".xlsx";
        string date = tournament.Date.ToString("MM-dd-yyyy");
        return tournament.Location + " " + tournament.Event + " " + date + outputExtension;
    }

    private void WriteWorkbook(Tournament tournament, string templatePath, Stream destination,
        List<SeriesReportRow> exportRows, bool applyDoublesCheckConsolidation)
    {
        List<int> memberNumbers = [.. exportRows
            .Select(r => r.MemberNumberText)
            .Where(s => int.TryParse(s, out _))
            .Select(int.Parse)
            .Distinct()];
        Dictionary<int, bool> membershipCurrent = winnersService.GetMembershipCurrentByMemberNumber(memberNumbers);

        SeriesReportExportRequest request = new(
            tournament.Location,
            tournament.Event,
            tournament.Date,
            tournament.IsTwoDay,
            applyDoublesCheckConsolidation,
            exportRows,
            membershipCurrent);

        // The exporter works file-to-file (it re-injects template drawings by path),
        // so write to a temp file and copy it into the caller's stream.
        string outputPath = Path.Combine(Path.GetTempPath(), $"9tap-results-{Guid.NewGuid():N}{Path.GetExtension(templatePath)}");
        try
        {
            seriesReportExporter.Export(templatePath, outputPath, request);
            using FileStream output = File.OpenRead(outputPath);
            output.CopyTo(destination);
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }

    private static void ApplyProgressivePot(Game game, string progressivePotText)
    {
        if (decimal.TryParse(progressivePotText, out decimal sidePot))
        {
            game.SidePot = sidePot;
        }
        else
        {
            game.Notes = $"Progressive Pot was entered as: {progressivePotText}";
        }
    }

    private static void ClearAutoFill(TwoDayRow row)
    {
        row.FullName = string.Empty;
        row.HandicapDisplay = string.Empty;
        row.TotalScore = null;
        row.GameId = null;
    }

    private Tournament GetTournament(int tournamentId)
    {
        return tournamentRepository.GetTourneyByID(tournamentId)
            ?? throw new ArgumentException($"Tournament {tournamentId} was not found.", nameof(tournamentId));
    }
}
