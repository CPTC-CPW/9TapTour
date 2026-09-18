using NineTapTour.Core.Calculations;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.Services;
using static NineTapTour.Core.Calculations.ReportHelper;

namespace NineTapTour.Web.Components.Pages.Standings;

/// <summary>
/// The standings report choices (from FrmMemberScores' Senior / Game / Series
/// buttons and FrmMemberScoresReports) carried between the setup page, the
/// print page, and the Excel download as query-string values.
/// </summary>
public sealed class StandingsQuery
{
    public const string TypeSeries = "series";
    public const string TypeGame = "game";
    public const string TypeSenior = "senior";

    public string Type { get; set; } = TypeSeries;

    /// <summary>Series only: rank by handicap (true) or scratch (false) series.</summary>
    public bool Handicap { get; set; } = true;

    /// <summary>Squad the standings qualify by; 0 means all squads.</summary>
    public int QualifySquad { get; set; }

    /// <summary>Series only: squads included in the standings; empty means all.</summary>
    public List<int> Squads { get; set; } = [];

    /// <summary>How many bowlers (or teams for doubles) to include.</summary>
    public int Count { get; set; }

    public bool PrintDues { get; set; }

    public int? ManualCutoff { get; set; }

    public ReportType ReportType => Type switch
    {
        TypeSenior => ReportType.HighGameHandicapGameSenior,
        TypeGame => ReportType.HighGame,
        _ => ReportType.HighSeriesScratch,
    };

    /// <summary>Squad list in the shape the Core standings queries expect: [0] means all squads.</summary>
    public List<int> SquadListForQuery => Squads.Count == 0 ? [0] : [.. Squads.OrderBy(s => s)];

    /// <summary>Loads the full (un-truncated) standings the report is built from.</summary>
    public List<MemberScores> LoadAll(Tournament tournament, IScoresService scoresService, IParticipantRepository participantRepository)
    {
        return Type switch
        {
            TypeSenior => participantRepository.GetSeniorMemberScores(tournament.Id),
            TypeGame => scoresService.GetGameScores(tournament.Id),
            _ => scoresService.GetSeriesStandings(tournament.Id, tournament.ThreeOutOf4, tournament.Doubles, Handicap, !Handicap, SquadListForQuery),
        };
    }

    /// <summary>Top N by placement, as the desktop applied before printing or exporting.</summary>
    public List<MemberScores> LoadTop(Tournament tournament, IScoresService scoresService, IParticipantRepository participantRepository)
    {
        List<MemberScores> all = LoadAll(tournament, scoresService, participantRepository);
        return TournamentCalculations.MakeTopMembersByPlacementList(all, Count, tournament.Doubles);
    }

    public string ToQueryString()
    {
        List<string> parts =
        [
            $"type={Type}",
            $"handicap={(Handicap ? "true" : "false")}",
            $"qualify={QualifySquad}",
            $"count={Count}",
            $"dues={(PrintDues ? "true" : "false")}",
        ];
        if (Squads.Count > 0)
        {
            parts.Add("squads=" + string.Join(",", Squads));
        }
        if (ManualCutoff.HasValue)
        {
            parts.Add($"cutoff={ManualCutoff.Value}");
        }
        return string.Join("&", parts);
    }

    public static StandingsQuery FromQuery(string? type, bool? handicap, int? qualify, string? squads, int? count, bool? dues, int? cutoff)
    {
        return new StandingsQuery
        {
            Type = type ?? TypeSeries,
            Handicap = handicap ?? true,
            QualifySquad = qualify ?? 0,
            Squads = [.. (squads ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)],
            Count = count ?? 0,
            PrintDues = dues ?? false,
            ManualCutoff = cutoff,
        };
    }
}
