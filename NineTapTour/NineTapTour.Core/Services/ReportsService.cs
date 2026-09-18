using NineTapTour.Core.Calculations;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
using System.Collections;
using System.Reflection;

namespace NineTapTour.Core.Services;

public class ReportsService : IReportsService
{
    public const string CategorySummary = "Summary";
    public const string CategoryHighSeries = "High Series";
    public const string CategoryHighGames = "High Games";

    private static readonly Dictionary<string, TourReportCategory> tourCategories = new()
    {
        { CategoryHighSeries, TourReportCategory.HighSeries },
        { CategoryHighGames, TourReportCategory.HighGames },
        { "High Averages", TourReportCategory.HighAverages },
        { "Total Entries", TourReportCategory.TotalEntries },
        { "Earnings", TourReportCategory.Earnings },
        { "1st Place Finishes", TourReportCategory.FirstPlaceFinishes },
        { "Top 5 Finishes", TourReportCategory.Top5Finishes },
        { "Top 10 Finishes", TourReportCategory.Top10Finishes },
    };

    private static readonly string[] individualCategories =
    [
        CategorySummary, CategoryHighSeries, CategoryHighGames,
    ];

    /// <summary>Friendly header text for row properties whose names are not display ready.</summary>
    private static readonly Dictionary<string, string> columnHeaders = new()
    {
        { "Member", "Member #" },
        { "SeriesWithHdcp", "Series w/HDCP" },
        { "HighSeries", "High Series" },
        { "HighGame", "High Game" },
        { "FirstPlace", "1st Place" },
        { "SecondPlace", "2nd Place" },
        { "ThirdPlace", "3rd Place" },
        { "Top5", "Top 5" },
        { "Top10", "Top 10" },
    };

    private readonly IMemberRepository memberRepository;
    private readonly IReportsRepository reportsRepository;

    public ReportsService(IMemberRepository memberRepository, IReportsRepository reportsRepository)
    {
        this.memberRepository = memberRepository;
        this.reportsRepository = reportsRepository;
    }

    public IReadOnlyList<string> IndividualCategories => individualCategories;

    public IReadOnlyList<string> TourCategories => [.. tourCategories.Keys];

    public List<int> GetTournamentYears()
    {
        return reportsRepository.GetTournamentYears();
    }

    public ReportTable Run(ReportRequest request)
    {
        (int? startYear, int? endYear, string periodLabel) = NormalizePeriod(request.StartYear, request.EndYear);

        Member? member = null;
        if (request.Scope == ReportScope.Individual)
        {
            if (request.MemberNumber == null)
            {
                throw new ArgumentException("An individual report needs a member.", nameof(request));
            }

            member = memberRepository.GetMember(request.MemberNumber.Value);
            if (member.Id == 0)
            {
                throw new ArgumentException($"Member {request.MemberNumber} was not found.", nameof(request));
            }

            if (!individualCategories.Contains(request.Category))
            {
                throw new ArgumentException($"Unknown individual report category '{request.Category}'.", nameof(request));
            }
        }
        else if (!tourCategories.ContainsKey(request.Category))
        {
            throw new ArgumentException($"Unknown tour report category '{request.Category}'.", nameof(request));
        }

        string title = BuildTitle(member, request.Category, periodLabel, request.IncludeSidePots, request.IncludeImported);

        List<ReportGameEntry> entries = reportsRepository.GetReportEntries(startYear, endYear, member?.Number, request.IncludeImported);
        if (entries.Count == 0)
        {
            return new ReportTable { Title = title };
        }

        IList rows;
        if (request.Scope == ReportScope.Individual)
        {
            rows = request.Category switch
            {
                CategoryHighSeries => ReportCalculations.GetHighSeries(entries, request.TopN),
                CategoryHighGames => ReportCalculations.GetHighGames(entries, request.TopN),
                _ => ReportCalculations.BuildIndividualSummary(entries, request.IncludeSidePots),
            };
        }
        else
        {
            rows = tourCategories[request.Category] switch
            {
                TourReportCategory.HighSeries => ReportCalculations.GetHighSeries(entries, request.TopN),
                TourReportCategory.HighGames => ReportCalculations.GetHighGames(entries, request.TopN),
                var tourCategory => ReportCalculations.GetTourReport(entries, tourCategory, request.TopN, request.IncludeSidePots),
            };
        }

        return BuildTable(title, rows);
    }

    /// <summary>
    /// Orders a year range and produces the label used in report titles:
    /// "Career", "2025", or "2024 to 2025".
    /// </summary>
    public static (int? StartYear, int? EndYear, string PeriodLabel) NormalizePeriod(int? startYear, int? endYear)
    {
        if (startYear == null && endYear == null)
        {
            return (null, null, "Career");
        }

        int start = startYear ?? endYear!.Value;
        int end = endYear ?? start;
        if (start > end)
        {
            (start, end) = (end, start);
        }

        string label = start == end ? start.ToString() : $"{start} to {end}";
        return (start, end, label);
    }

    /// <summary>Title shown above the report and written into the Excel export.</summary>
    public static string BuildTitle(Member? member, string category, string periodLabel, bool includeSidePots, bool includeImported)
    {
        string scopeLabel = member?.ToString() ?? "Tour-Wide";
        string title = $"{scopeLabel} — {category} — {periodLabel}";
        if (includeSidePots)
        {
            title += " (incl. side pots)";
        }
        if (!includeImported)
        {
            title += " (excl. imported history)";
        }
        return title;
    }

    /// <summary>
    /// Converts a list of report row objects into a typed table using the row
    /// type's public properties, the same way the DataGridView auto-generated
    /// its columns.
    /// </summary>
    public static ReportTable BuildTable(string title, IList rows)
    {
        Type rowType = rows.GetType().IsGenericType
            ? rows.GetType().GetGenericArguments()[0]
            : rows.Count > 0 ? rows[0]!.GetType() : typeof(object);

        PropertyInfo[] properties = rowType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        List<ReportColumn> columns = [.. properties.Select(p => new ReportColumn(
            p.Name,
            columnHeaders.TryGetValue(p.Name, out string? header) ? header : p.Name,
            KindFor(p.PropertyType)))];

        List<object?[]> values = [];
        foreach (object? row in rows)
        {
            values.Add([.. properties.Select(p => p.GetValue(row))]);
        }

        return new ReportTable { Title = title, Columns = columns, Rows = values };
    }

    private static ReportColumnKind KindFor(Type type)
    {
        Type underlying = Nullable.GetUnderlyingType(type) ?? type;
        if (underlying == typeof(decimal))
        {
            return ReportColumnKind.Currency;
        }
        if (underlying == typeof(double) || underlying == typeof(float))
        {
            return ReportColumnKind.Number;
        }
        if (underlying == typeof(DateTime))
        {
            return ReportColumnKind.Date;
        }
        if (underlying == typeof(int) || underlying == typeof(long) || underlying == typeof(short))
        {
            return ReportColumnKind.Integer;
        }
        return ReportColumnKind.Text;
    }
}
