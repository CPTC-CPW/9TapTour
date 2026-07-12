using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NineTapTour.Abstractions;
using NineTapTour.Data.Repositories;
using NineTapTour.Database;

namespace NineTapTour.Data
{
    /// <summary>
    /// Registers the EF Core <see cref="NineTapDb"/> context factory and every repository so the
    /// WinForms apps (and integration tests) share one composition root.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNineTapData(this IServiceCollection services, string connectionString)
        {
            services.AddDbContextFactory<NineTapDb>(options => options.UseSqlServer(connectionString));

            services.AddSingleton<IMemberRepository, MemberRepository>();
            services.AddSingleton<IGameRepository, GameRepository>();
            services.AddSingleton<ITournamentRepository, TournamentRepository>();
            services.AddSingleton<IParticipantRepository, ParticipantRepository>();
            services.AddSingleton<IStandingsRepository, StandingsRepository>();
            services.AddSingleton<IPlayerHistoryRepository, PlayerHistoryRepository>();
            services.AddSingleton<IDoublesRepository, DoublesRepository>();
            services.AddSingleton<ITournamentStatsRepository, TournamentStatsRepository>();
            services.AddSingleton<IFinalizeRepository, FinalizeRepository>();
            services.AddSingleton<IDatabaseAdminService, DatabaseAdminService>();

            return services;
        }
    }
}
