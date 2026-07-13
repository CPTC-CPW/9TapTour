using System.Collections.Generic;
using NineTapTour.Models;
using NineTapTour.Models.ViewModels;

namespace NineTapTour.Abstractions
{
    /// <summary>
    /// Builds the Member-Scores report view models (per-game "high game" list and the "high series"
    /// list) from a tournament's participants, applying the ordering for the selected report type.
    /// Pure logic extracted from FrmMemberScores so it can be unit tested.
    /// </summary>
    public interface IStandingsReportService
    {
        (List<ParticipantsGameViewModel> GameScores, List<TopParticipantGameViewModel> SeriesScores) BuildReport(
            IReadOnlyList<Participant> participants, bool isThreeOfFour, ReportType reportType);
    }
}
