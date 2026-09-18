using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Web.Components;
using NineTapTour.Web.Data;
using NineTapTour.Web.Startup;

// The desktop app formats currency and dates for en-US; pin the same culture so
// reports and exports match regardless of the host's locale.
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNineTapTourWeb(builder.Configuration, builder.Environment.IsDevelopment());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseRequestLocalization("en-US");

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Apply pending migrations for both contexts and seed the first admin before
// serving requests, so a schema problem fails fast. Skipped by the dotnet ef
// tooling, which builds the host without running it.
if (!EF.IsDesignTime && app.Configuration.GetValue("Database:MigrateOnStartup", true))
{
    await DatabaseStartup.MigrateAndSeedAsync(app.Services, app.Logger);
}

app.Run();

/// <summary>Exposed so the test project can host the app with WebApplicationFactory.</summary>
public partial class Program { }
