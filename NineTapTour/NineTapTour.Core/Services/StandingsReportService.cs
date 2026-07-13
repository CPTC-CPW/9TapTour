using System;
using System.Collections.Generic;
using System.Linq;
using NineTapTour.Abstractions;
using NineTapTour.Calculations;
using NineTapTour.Models;
using NineTapTour.Models.ViewModels;

namespace NineTapTour.Services
{
    /// <summary>
    /// Builds and orders the Member-Scores report view models. Extracted verbatim from
    /// FrmMemberScores.GetTournamentPlacings so behavior is preserved but UI-free and testable.
    /// </summary>
    public sealed class StandingsReportService : IStandingsReportService
    {
        public (List<ParticipantsGameViewModel> GameScores, List<TopParticipantGameViewModel> SeriesScores) BuildReport(
            IReadOnlyList<Participant> participants, bool isThreeOfFour, ReportType reportType)
        {
            var gameScores = new List<ParticipantsGameViewModel>();
            var seriesScores = new List<TopParticipantGameViewModel>();

            // Per-game (high game) view models — one top game per person per squad.
            foreach (Participant p in participants)
            {
                gameScores.Add(new ParticipantsGameViewModel(
                    p.Member.Number,
                    p.Member.FirstName,
                    p.Member.LastName,
                    p.Squad,
                    p.Game.AllGameScores().Max(),
                    p.Member.Handicap,
                    p.Member.Bonus));
            }

            // Per-series (high series) view models.
            foreach (Participant p in participants)
            {
                var validScores = p.Game.AllGameScores().Where(g => g.HasValue).ToList();
                List<int> top3Games = TournamentStatsCalculator.GetTop3OutOf4(p.Game.Game1, p.Game.Game2, p.Game.Game3, p.Game.Game4);
                int numberOfGames = validScores.Count;

                var vm = new TopParticipantGameViewModel(
                    p.Member.Number,
                    p.Member.FirstName,
                    p.Member.LastName,
                    0,
                    p.Game.AllGameScores().Sum().Value,
                    top3Games.Sum(),
                    top3Games.Sum()
                        + (Math.Min(3, numberOfGames) * p.Member.Handicap)
                        + (Math.Min(3, numberOfGames) * p.Game.Bonus),
                    p.Game.Game1,
                    p.Game.Game2,
                    p.Game.Game3,
                    p.Game.Game4,
                    p.Game.Handicap,
                    p.Game.Bonus.Value,
                    p.Game.Id,
                    p.Squad)
                {
                    IsThreeOutOf4 = isThreeOfFour
                };

                seriesScores.Add(vm);
            }

            switch (reportType)
            {
                case ReportType.HighGameHandicapGameSenior:
                    gameScores = [.. gameScores.OrderByDescending(t => t.HighScore + t.Handicap + t.Bonus)];
                    break;
                case ReportType.HighGame:
                    gameScores = [.. gameScores.OrderByDescending(t => t.HighScore)];
                    break;
                case ReportType.HighSeriesScratch when isThreeOfFour:
                    seriesScores = [.. seriesScores.OrderByDescending(t => t.Top3ScratchScore)];
                    break;
                case ReportType.HighSeriesScratch:
                    seriesScores = [.. seriesScores.OrderByDescending(t => t.ScratchTotal)];
                    break;
                case ReportType.HighSeriesHandicap when isThreeOfFour:
                    seriesScores = [.. seriesScores.OrderByDescending(t => t.Top3HandiScores)];
                    break;
                case ReportType.HighSeriesHandicap:
                    seriesScores = [.. seriesScores.OrderByDescending(t => t.HandicapScore)];
                    break;
            }

            return (gameScores, seriesScores);
        }
    }
}
