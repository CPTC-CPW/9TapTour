# 9-Tap Tour
This app is the 9-Tap Tour Replacement Application, that keeps track of 
the 9 Tap Tour. This information includes member data, tournament 
information, games, monies earned, player stats, etc. The app will 
replace the current program being used by the client to run future 9-Tap 
tournaments.

## Getting Started With 9TapTour Step-By-Step Instructions
We are using Visual Studio 2026 and SQL Server. This will also work using Visual Studio 2017+.

1. Clone the project and then build in Visual Studio

2. Run `Update-Database` in the Package Manager Console

3. Run the member import program to import data (must have sample files from client)

### Prerequisites
The current build is being built on Windows machines through Visual Studio 2026 and .NET 10.

* [Download Office 365](https://www.microsoft.com/en-us/education/products/office) Free for students (required in import program to transform old Excel format).
* [Download Visual Studio 2026](https://visualstudio.microsoft.com/downloads/)
* [.NET 10 SDK](https://dotnet.microsoft.com/download/visual-studio-sdks) - Comes with VS2026

## Architecture

The solution is split so that all business logic is headless and reusable (for example by a future website):

| Project | Role |
| --- | --- |
| `NineTapTour.Core` (`net10.0`) | Everything headless: EF Core entities + `NineTapDb` + migrations (`Data/`), repositories (`Repositories/`), business services (`Services/` — finalize math, winners/payouts, score entry, doubles pairing, stats, session state, DB maintenance), Excel import (`Import/`) and export (`Export/`), print content (`Printing/`), pure calculations (`Calculations/`), and the `IMessageService`/`IFileDialogService` UI abstractions (`Abstractions/`). Core never references WinForms — a unit test enforces this. |
| `NineTapTour` (WinExe) | The WinForms shell. `Program.cs` is the DI composition root (configuration from `appsettings.json`, `ServiceCollection`, migrations run at startup). Forms are resolved from the container (`IFormNavigator` for MDI singletons, `IFormFactory` for dialogs) and receive repositories/services via constructor injection. Code-behind keeps only control wiring, grid read/write, dialogs, and GDI+ drawing. |
| `NineTapTour.Web` (Blazor Web App, `net10.0`) | The website that replaces the desktop app: Razor components with global Interactive Server rendering, ASP.NET Core Identity (local accounts, one `Admin` role) on its own `ApplicationDbContext` in the same database, and one page per desktop screen (members, tournaments, score entry, standings, results, finalize, doubles pairings, reports, regions, users). Print pages are statically rendered; Excel files stream to the browser. References Core only. |
| `MemberImportTest` (WinExe) | Thin shell over the shared Core import services. |
| `NineTapTourTests` | Unit tests (MSTest): Core services, calculations, characterization golden masters. |
| `NineTapTour.IntegrationTests` | Real LocalDB tests: a unique catalog is created, migrated, and seeded per run, then dropped. Includes golden masters for the standings SQL, the finalize grid, the results grid, the region repository, the AddRegions backfill, and a backup/restore round trip. |
| `NineTapTour.Web.Tests` | Hosts the website with `WebApplicationFactory` against its own LocalDB catalog: container validation, component architecture rules (authorization on every page, Admin policy on admin pages, static print pages), and HTTP smoke tests of every page. |

### Regions

Every member has a home region and every tournament belongs to a region (`Regions` table, `RegionId` on `Members` and `Tournaments`). The `AddRegions` migration seeds one region named `Default` and assigns all existing rows to it. Regions are managed on the website (Admin > Regions). The desktop app has no region UI: `MemberRepository.AddOrUpdateMember` and `TournamentRepository.AddTournament`/`UpdateTournament` apply the default region on insert and keep the stored region on update, so desktop edits never wipe a region chosen on the web.

### Running the website

1. Set the connection string (`ConnectionStrings:NineTapDb` in `NineTapTour.Web/appsettings.json`, or the `ConnectionStrings__NineTapDb` environment variable).
2. Set the first admin password once: `dotnet user-secrets set "Identity:Admin:Password" "<password>" --project NineTapTour/NineTapTour.Web` (or the `Identity__Admin__Password` environment variable). On the first start with an empty user table the site creates the `admin` user in the `Admin` role.
3. `dotnet run --project NineTapTour/NineTapTour.Web`. Startup applies pending migrations for both the tour database and the Identity tables (history table `__IdentityMigrationsHistory`), then seeds the admin. Set `Database:MigrateOnStartup` to `false` to skip this.
4. Hosting needs WebSockets enabled and a single instance (or sticky sessions) because Interactive Server keeps a SignalR circuit per user.

Conventions:
* New business logic goes in `NineTapTour.Core` services, injected into forms and Razor components via constructor parameters / `@inject` (registered in `CoreServiceConfiguration.AddNineTapTourCore`; UI-specific registrations live in `ServiceConfiguration.AddNineTapTourServices` for WinForms and `WebServiceConfiguration.AddNineTapTourWeb` for the website).
* Data access goes through the repository interfaces; repositories take `IDbContextFactory<NineTapDb>` and create short-lived contexts.
* Web pages take the tournament or member id from the route; nothing on the web may inject `ITournamentSession` (an architecture test enforces this).
* The connection string lives in `appsettings.json` (`ConnectionStrings:NineTapDb`).
* `dotnet ef` commands for the tour database target `NineTapTour.Core` (a design-time factory supplies the connection string): `dotnet ef migrations add <Name> --project NineTapTour.Core --startup-project NineTapTour.Core --output-dir Data/Migrations`. Identity migrations target the web project: `dotnet ef migrations add <Name> --project NineTapTour.Web --startup-project NineTapTour.Web --context ApplicationDbContext --output-dir Data/Migrations`.

### Coding Style Requirements
Reference the [code style requirements](CodingStyle.md) for more information.

## Authors 
Reference the list of [contributors](https://github.com/CPTC-CPW/9TapTour/graphs/contributors) who participated in this project.