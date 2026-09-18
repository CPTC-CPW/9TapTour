namespace NineTapTour.Core.Models;

/// <summary>
/// One row of the tournament results grid (FrmTournamentResults): a cashed
/// winner, or an empty filler row below the winners so the director can type
/// additional places. Earnings and the progressive pot are the editable cells.
/// </summary>
public sealed class ResultRow
{
    /// <summary>Place with a trailing "T" when tied with a neighbour, e.g. "3T".</summary>
    public string PlaceDisplay { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    /// <summary>"handicap + bonus" as shown in the H/B* column.</summary>
    public string HandicapDisplay { get; set; } = string.Empty;

    public string TotalScoreDisplay { get; set; } = string.Empty;

    public int? MemberNumber { get; set; }

    /// <summary>0 for filler rows, which are never saved.</summary>
    public int GameId { get; set; }

    public bool IsFiller => GameId <= 0;

    public decimal Earnings { get; set; }

    /// <summary>Free text: a decimal is saved as the side pot, anything else lands in the game notes.</summary>
    public string ProgressivePotText { get; set; } = "0.00";
}

/// <summary>The built results grid plus the winners list it came from (needed for team view and export).</summary>
public sealed class ResultsGridModel
{
    public int TournamentId { get; init; }

    public bool IsDoubles { get; init; }

    /// <summary>Winners (singles) or teams (doubles) the director asked to place.</summary>
    public int RequestedCount { get; init; }

    public int TotalEntries { get; init; }

    public int CompEntries { get; init; }

    public List<ResultRow> Rows { get; init; } = [];

    /// <summary>Full winners list from IWinnersService, in the consecutive-pair order for doubles.</summary>
    public IReadOnlyList<ExcelMember> Winners { get; init; } = [];
}

/// <summary>Editable values of one results row, posted back to be saved.</summary>
public sealed record ResultRowSave(int GameId, string PlaceDisplay, decimal Earnings, string ProgressivePotText);

/// <summary>One doubles team collapsed from two individual result rows.</summary>
public sealed record TeamResultRow(
    int Place,
    bool IsTie,
    string Member1Name,
    string Member1HandicapDisplay,
    string Member2Name,
    string Member2HandicapDisplay,
    int CombinedTotal,
    decimal Earnings1,
    decimal Earnings2,
    decimal SidePot1,
    decimal SidePot2)
{
    public string PlaceDisplay => IsTie ? $"{Place}T" : Place.ToString();

    public decimal CombinedEarnings => Earnings1 + Earnings2;

    public decimal CombinedSidePot => SidePot1 + SidePot2;
}

/// <summary>
/// One row of the 2-day championship results grid: a place group ("46th - 59th")
/// with a member number the director types, which auto-fills the rest.
/// </summary>
public sealed class TwoDayRow
{
    public string PlaceLabel { get; set; } = string.Empty;

    /// <summary>First place of the group, used to order rows and saved as Game.PlaceStanding.</summary>
    public int PlaceSortStart { get; set; }

    public int? MemberNumber { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string HandicapDisplay { get; set; } = string.Empty;

    public int? TotalScore { get; set; }

    public decimal Earnings { get; set; }

    public string ProgressivePotText { get; set; } = "0.00";

    public int? GameId { get; set; }

    /// <summary>Message from the last auto-fill attempt (member not found, no game), or null.</summary>
    public string? StatusMessage { get; set; }
}

/// <summary>Editable values of one 2-day row, posted back to be saved.</summary>
public sealed record TwoDayRowSave(int? GameId, string PlaceLabel, decimal Earnings, string ProgressivePotText);

/// <summary>How many rows were written and which rows were skipped and why.</summary>
public sealed record ResultsSaveOutcome(int SavedCount, IReadOnlyList<string> Errors);

/// <summary>The exported workbook's suggested file name and the save outcome that preceded it.</summary>
public sealed record ResultsExportOutcome(string FileName, ResultsSaveOutcome Save);
