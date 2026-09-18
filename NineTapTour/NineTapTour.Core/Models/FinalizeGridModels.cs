namespace NineTapTour.Core.Models;

/// <summary>
/// One row of the finalize tournament grid: an entry (game) for a member in a
/// squad, with the live computed columns. Doubles rows carry the partner's
/// game id and a team index so the two rows can be banded together.
/// </summary>
public sealed class FinalizeGridRow
{
    public int GameId { get; set; }

    public int? PartnerGameId { get; set; }

    public bool IsDoublesRow => PartnerGameId.HasValue;

    /// <summary>Zero-based team index for doubles rows (used for alternating row colours), -1 for singles.</summary>
    public int TeamIndex { get; set; } = -1;

    public int? Standing { get; set; }

    public int MemberNumber { get; set; }

    public string Name { get; set; } = string.Empty;

    public int? Game1 { get; set; }
    public int? Game2 { get; set; }
    public int? Game3 { get; set; }
    public int? Game4 { get; set; }

    public bool UseGame1 { get; set; }
    public bool UseGame2 { get; set; }
    public bool UseGame3 { get; set; }
    public bool UseGame4 { get; set; }

    public int ScratchTotal { get; set; }

    /// <summary>For doubles rows this is the team's combined handicap total.</summary>
    public int HdcpTotal { get; set; }

    public int EntryAvg { get; set; }

    public double? ThirtyEntryAvg { get; set; }

    public int AdjAvg { get; set; }

    public bool DirectorCheck { get; set; }

    public int Squad { get; set; }

    public int Hdcp { get; set; }

    public int? NewHdcp { get; set; }

    /// <summary>Carry-in bonus the entry is scored with.</summary>
    public int Bonus { get; set; }

    /// <summary>Bonus pins the member carries out of the tournament.</summary>
    public int NewBonus { get; set; }

    /// <summary>Place money, plus the side pot on a placed singles entry.</summary>
    public decimal? Earnings { get; set; }

    public string? Notes { get; set; }

    public bool IsValid { get; set; }

    public bool IsCashing { get; set; }

    public bool IsPlaced { get; set; }

    public double LeagueAverage { get; set; }

    /// <summary>Per game (index 0-3): true when the score is 40+ pins under the league average.</summary>
    public bool[] Sandbagging { get; } = new bool[4];

    /// <summary>Best place standing of the member (drives the bonus deduction preview).</summary>
    public int Placing { get; set; }

    public int HistoricalEntries { get; set; }

    public int CurrentEntries { get; set; }

    /// <summary>Whole-dollar side pot stored on the game; folded into Earnings on placed singles rows.</summary>
    public int SidePot { get; set; }

    public bool[] UseGames => [UseGame1, UseGame2, UseGame3, UseGame4];

    public int?[] Games => [Game1, Game2, Game3, Game4];
}

/// <summary>The finalize grid for one tournament.</summary>
public sealed class FinalizeGridModel
{
    public int TournamentId { get; init; }

    public string TournamentName { get; init; } = string.Empty;

    public DateTime TournamentDate { get; init; }

    public bool IsFinalized { get; init; }

    public bool IsDoubles { get; init; }

    public bool IsTwoDay { get; init; }

    public bool ThreeOutOf4 { get; init; }

    /// <summary>False for doubles (two games each) and three-game formats.</summary>
    public bool ShowGame3 { get; init; } = true;

    public bool ShowGame4 { get; init; } = true;

    /// <summary>Lowest placement that cashes.</summary>
    public int CashLine { get; init; }

    public List<FinalizeGridRow> Rows { get; init; } = [];

    public IReadOnlyDictionary<int, int> BestStandingByMember { get; init; } = new Dictionary<int, int>();
}

/// <summary>Which grid cell the director edited; decides what is mirrored to the member's other entries.</summary>
public enum FinalizeEditField
{
    Game1,
    Game2,
    Game3,
    Game4,
    UseGame1,
    UseGame2,
    UseGame3,
    UseGame4,
    AdjAvg,
    DirectorCheck,
    Bonus,
    Earnings,
    Notes,
}

/// <summary>All editable values of one grid row after an edit, plus which field changed.</summary>
public sealed record FinalizeRowEdit(
    int GameId,
    FinalizeEditField Field,
    int? Game1,
    int? Game2,
    int? Game3,
    int? Game4,
    bool UseGame1,
    bool UseGame2,
    bool UseGame3,
    bool UseGame4,
    int AdjAvg,
    bool DirectorCheck,
    int Bonus,
    decimal? Earnings,
    string? Notes);

/// <summary>One row of the member detail panel under the finalize grid.</summary>
public sealed class FinalizeDetailRow
{
    public int Games { get; set; }

    public DateTime Date { get; set; }

    public int? Game1 { get; set; }
    public int? Game2 { get; set; }
    public int? Game3 { get; set; }
    public int? Game4 { get; set; }

    public int Scratch { get; set; }

    public int WithHandicap { get; set; }

    public int Entry { get; set; }

    public double? ThirtyAverage { get; set; }

    public int? AdjustedAvg { get; set; }

    public int Handicap { get; set; }

    public int Bonus { get; set; }

    public string? Place { get; set; }

    public decimal? Earnings { get; set; }

    public string? Notes { get; set; }

    /// <summary>True for this tournament's live entries (shown in blue on the desktop).</summary>
    public bool IsCurrent { get; set; }

    /// <summary>Bonus cell highlight: the entry cashed.</summary>
    public bool HighlightBonus { get; set; }

    /// <summary>30 AVG cell highlight: the entry is inside the rolling 30-entry window.</summary>
    public bool HighlightThirtyAverage { get; set; }
}

/// <summary>The member detail panel: header line values, column header totals, and rows.</summary>
public sealed class FinalizeDetailModel
{
    public int MemberNumber { get; init; }

    public string MemberName { get; init; } = string.Empty;

    /// <summary>Most recent league (true 30-game) average, rounded for the header.</summary>
    public int LeagueAverage { get; init; }

    /// <summary>Column header text keyed by column: Games, Game1..Game4, Scratch, WHdcp, Entry, ThirtyAvg, Earnings.</summary>
    public IReadOnlyDictionary<string, string> ColumnHeaders { get; init; } = new Dictionary<string, string>();

    public List<FinalizeDetailRow> Rows { get; init; } = [];
}

/// <summary>Doubles team summary row for the finalize Team View.</summary>
public sealed record FinalizeTeamRow(
    int? Place,
    string Member1,
    string Member2,
    int Scratch1,
    int Scratch2,
    int HdcpTotal,
    int Bonus1,
    int Bonus2,
    decimal? Earnings1,
    decimal? Earnings2);

/// <summary>Result of a finalize attempt.</summary>
public sealed record FinalizeOutcome(bool Success, string Message, IReadOnlyList<int> InvalidGameIds);
