using NineTapTour.Core.Models;

namespace NineTapTour.Core.Services;

/// <summary>
/// Tournament results workflow extracted from FrmTournamentResults: build the
/// winners grid for the number of places the director wants to pay, save
/// place/earnings/side pot back to the games, collapse doubles into team rows,
/// run the 2-day championship round entry, and export the client check
/// template. All methods are stateless; the tournament id travels with each call.
/// </summary>
public interface IResultsService
{
    /// <summary>Builds the results grid for the top <paramref name="requestedCount"/> winners (teams for doubles).</summary>
    ResultsGridModel BuildResults(int tournamentId, int requestedCount);

    /// <summary>Writes place standing, earnings, and side pot (or a note) to each row's game. Filler and duplicate rows are skipped.</summary>
    ResultsSaveOutcome SaveResults(int tournamentId, IReadOnlyList<ResultRowSave> rows);

    /// <summary>Doubles only: one row per team reconstructed from the consecutive-pair winners list.</summary>
    List<TeamResultRow> BuildTeamView(ResultsGridModel grid);

    /// <summary>2-day: previously saved place groups, auto-filled from the members' game entries.</summary>
    List<TwoDayRow> LoadTwoDay(int tournamentId);

    /// <summary>2-day: one empty row per place from start to end, pre-filled with the round's earnings.</summary>
    /// <exception cref="ArgumentException">End place before start place.</exception>
    List<TwoDayRow> AddTwoDayRound(int startPlace, int endPlace, decimal earnings);

    /// <summary>2-day: fills name, H/B, total score, and game id for the member number typed on a row.</summary>
    void AutoFillTwoDayRow(int tournamentId, TwoDayRow row);

    /// <summary>2-day: writes place group, earnings, and side pot (or a note) to each row's game.</summary>
    ResultsSaveOutcome SaveTwoDay(int tournamentId, IReadOnlyList<TwoDayRowSave> rows);

    /// <summary>
    /// Standard export: reads earnings/pots from the pre-filled template when
    /// possible, saves the rows, then fills the template and writes the workbook
    /// to <paramref name="destination"/>. The workbook keeps the template's
    /// extension (.xlsm keeps macros).
    /// </summary>
    ResultsExportOutcome Export(int tournamentId, string templatePath, Stream destination,
        IReadOnlyList<ResultRowSave> rows, bool teamView);

    /// <summary>2-day export: saves the rows, then fills the template ordered by place group.</summary>
    ResultsExportOutcome ExportTwoDay(int tournamentId, string templatePath, Stream destination,
        IReadOnlyList<TwoDayRow> rows);

    /// <summary>"{Location} {Event} {MM-dd-yyyy}" plus the template's extension.</summary>
    string BuildExportFileName(int tournamentId, string templatePath);
}
