namespace NineTapTour.Core.Import;

/// <summary>
/// Outcome of an Excel member import. The forms build their user-visible
/// messages from these values; Core never shows UI.
/// </summary>
public sealed record ImportResult
{
    /// <summary>
    /// Number of records (game rows) imported.
    /// </summary>
    public int Added { get; init; }

    /// <summary>
    /// Number of records updated in place.
    /// </summary>
    public int Updated { get; init; }

    /// <summary>
    /// Number of records skipped (for the single-member history import this is 1
    /// when the member's history was already imported and nothing was done).
    /// </summary>
    public int Skipped { get; init; }

    /// <summary>
    /// Human-readable warnings gathered while importing.
    /// </summary>
    public IReadOnlyList<string> Warnings { get; init; } = [];

    /// <summary>
    /// The trueAVG of the most recent imported tournament (single-member history
    /// import only); the member form uses it to refresh the 30-game average box.
    /// Null when no history exists after the import.
    /// </summary>
    public double? MostRecentTrueAverage { get; init; }
}
