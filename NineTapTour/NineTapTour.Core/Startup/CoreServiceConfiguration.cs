using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NineTapTour.Core.Data;
using NineTapTour.Core.Export;
using NineTapTour.Core.Import;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.Services;

namespace NineTapTour.Core.Startup;

public static class CoreServiceConfiguration
{
    /// <summary>
    /// Registers the DbContext factory and all repositories. Shared by the
    /// WinForms app, the import tool, tests, and (later) the website.
    /// </summary>
    public static IServiceCollection AddNineTapTourCore(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextFactory<NineTapDb>(options => options.UseSqlServer(connectionString));

        services.AddSingleton<IMemberRepository, MemberRepository>();
        services.AddSingleton<IGameRepository, GameRepository>();
        services.AddSingleton<ITournamentRepository, TournamentRepository>();
        services.AddSingleton<ITournamentStatsRepository, TournamentStatsRepository>();
        services.AddSingleton<IParticipantRepository, ParticipantRepository>();
        services.AddSingleton<IPlayerHistoryRepository, PlayerHistoryRepository>();
        services.AddSingleton<IReportsRepository, ReportsRepository>();
        services.AddSingleton<IFinalizeTempRepository, FinalizeTempRepository>();
        services.AddSingleton<IDoublesTeamRepository, DoublesTeamRepository>();
        services.AddSingleton<IDoublesPartnerPlanRepository, DoublesPartnerPlanRepository>();
        services.AddSingleton<IDoublesPartnerClaimRepository, DoublesPartnerClaimRepository>();

        services.AddSingleton<ITournamentSession, TournamentSession>();
        services.AddSingleton<IDatabaseMaintenanceService, DatabaseMaintenanceService>();
        services.AddSingleton<IFinalizeCalculationService, FinalizeCalculationService>();
        services.AddSingleton<IWinnersService, WinnersService>();
        services.AddSingleton<IScoresService, ScoresService>();
        services.AddSingleton<IStatsService, StatsService>();
        services.AddSingleton<IDoublesPairingService, DoublesPairingService>();
        services.AddSingleton<ISeriesReportExcelExporter, SeriesReportExcelExporter>();

        services.AddSingleton<IMemberImportService, MemberImportService>();
        services.AddSingleton<IMemberHistoryImportService, MemberHistoryImportService>();

        return services;
    }
}
