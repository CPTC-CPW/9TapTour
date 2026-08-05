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
| `MemberImportTest` (WinExe) | Thin shell over the shared Core import services. |
| `NineTapTourTests` | Unit tests (MSTest): Core services, calculations, characterization golden masters. |
| `NineTapTour.IntegrationTests` | Real LocalDB tests: a unique catalog is created, migrated, and seeded per run, then dropped. Includes golden masters for the standings SQL and a backup/restore round trip. |

Conventions:
* New business logic goes in `NineTapTour.Core` services, injected into forms via constructor parameters (registered in `CoreServiceConfiguration.AddNineTapTourCore` / `ServiceConfiguration.AddNineTapTourServices`).
* Data access goes through the repository interfaces; repositories take `IDbContextFactory<NineTapDb>` and create short-lived contexts.
* The connection string lives in `appsettings.json` (`ConnectionStrings:NineTapDb`).
* `dotnet ef` commands target `NineTapTour.Core` (a design-time factory supplies the connection string).

### Coding Style Requirements
Reference the [code style requirements](CodingStyle.md) for more information.

## Authors 
Reference the list of [contributors](https://github.com/CPTC-CPW/9TapTour/graphs/contributors) who participated in this project.