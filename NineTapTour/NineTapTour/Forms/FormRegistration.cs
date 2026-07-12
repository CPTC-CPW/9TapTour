using Microsoft.Extensions.DependencyInjection;

namespace NineTapTour.Forms
{
    /// <summary>
    /// Registers the WinForms forms with the DI container. No-argument forms are registered as
    /// transient services (resolved via <c>GetRequiredService</c> / <see cref="ActivatorUtilities"/>).
    /// Forms that require a runtime argument (e.g. a Tournament) are NOT registered here; callers
    /// create them with <c>ActivatorUtilities.CreateInstance&lt;T&gt;(serviceProvider, args...)</c>,
    /// which fills the service parameters from DI and takes the runtime arguments from the caller.
    /// </summary>
    internal static class FormRegistration
    {
        public static IServiceCollection AddForms(IServiceCollection services)
        {
            services.AddTransient<FrmMain>();
            services.AddTransient<FrmMainMenu>();
            services.AddTransient<FrmAbout>();
            services.AddTransient<FrmMemberData>();
            services.AddTransient<FrmMemberScores>();
            services.AddTransient<FrmUpdateActiveMem>();
            services.AddTransient<FrmLabelPrint>();
            services.AddTransient<FrmNewTournament>();
            services.AddTransient<FrmPrintByDate>();
            services.AddTransient<FrmSelection>();
            services.AddTransient<FrmSearch>();
            services.AddTransient<FrmTournamentStats>();
            services.AddTransient<FrmTournamentResults>();
            services.AddTransient<FrmTournamentsByYear>();
            services.AddTransient<FrmTourSearch>();

            return services;
        }
    }
}
