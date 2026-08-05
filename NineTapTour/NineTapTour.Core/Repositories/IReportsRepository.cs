#nullable disable
using NineTapTour.Core.Models;
using System.Collections.Generic;

namespace NineTapTour.Core.Repositories;

/// <summary>
/// Data access for reports. Instance replacement for the old static ReportsDB;
/// method names and behavior are unchanged.
/// </summary>
public interface IReportsRepository
{
    List<ReportGameEntry> GetReportEntries(int? startYear, int? endYear, int? memberNumber = null);
    List<int> GetTournamentYears();
}
