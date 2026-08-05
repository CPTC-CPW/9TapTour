using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NineTapTour.Database;
using NineTapTour.Core.Data;
using NineTapTour.Forms;
using NineTapTour.Services;
using System;

namespace NineTapTour.Startup;

public static class ServiceConfiguration
{
    /// <summary>
    /// Registers all application services and the MDI child forms that are
    /// opened through IFormNavigator. Dialog forms with runtime constructor
    /// arguments are created through IFormFactory and need no registration.
    /// </summary>
    public static IServiceCollection AddNineTapTourServices(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("NineTapDb")
            ?? throw new InvalidOperationException("Connection string 'NineTapDb' is missing from appsettings.json.");

        services.AddDbContextFactory<NineTapDb>(options => options.UseSqlServer(connectionString));

        services.AddSingleton<IFormNavigator, FormNavigator>();
        services.AddSingleton<IFormFactory, FormFactory>();

        // MDI shell and singleton child forms opened through IFormNavigator
        services.AddTransient<FrmMain>();
        services.AddTransient<FrmMainMenu>();
        services.AddTransient<FrmAbout>();
        services.AddTransient<FrmMemberData>();
        services.AddTransient<FrmMemberScores>();
        services.AddTransient<FrmReports>();
        services.AddTransient<FrmNewTournament>();

        return services;
    }
}
