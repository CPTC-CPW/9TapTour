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

## Installing and Updates

The client's two apps, **9-Tap Tour** (`NineTapTour`) and **9-Tap Member Import** (`MemberImportTest`), install
with [Velopack](https://velopack.io) from this repository's [GitHub releases](https://github.com/CPTC-CPW/9TapTour/releases/latest).
Download `NineTapTour-Setup.exe` or `NineTapMemberImport-Setup.exe` once; it installs for the current Windows user
(no administrator password needed). After that, each time an app opens it downloads any newer release in the
background, and the update installs the next time it opens (9-Tap Tour also offers to restart right away from the
menu bar).

The apps need SQL Server Express LocalDB. When the connection string points at `(localdb)` and LocalDB isn't
installed, they offer to download and install SQL Server 2019 Express LocalDB from Microsoft before opening.

The deployment code is in `NineTapTour/Deployment/` and is shared by both apps.

### Releases

Run the **Release** workflow from the repository's Actions tab, on `main`. Choose whether the release has bug fixes,
new features or breaking changes, and it picks the next version from the latest release tag (tags are bare versions
such as `3.2.3`), or type a version to use instead. The optional summary goes in the release title, such as
`3.3.1 - Birthdate fixes`. It builds both apps and publishes a GitHub release with their packages, which installed
copies update from.

To build the packages locally, from the repository root in Git Bash with `vpk` installed
(`dotnet tool install -g vpk --version 1.2.161`):

```bash
VERSION=3.3.0 bash packaging/package.sh
```

Setting `NINETAP_UPDATE_SOURCE` to the resulting `dist` folder makes an installed app update from it instead of
GitHub, to try out a release before publishing it.

### Moving from ClickOnce

Before Velopack, the apps were ClickOnce installs updated from `ninetap.azurewebsites.net`. Version 3.3.0 is the
last ClickOnce release and moves each PC over by itself:

1. Publish 3.3.0 with the **Release** workflow first, so the Velopack installers are on GitHub.
2. Then publish 3.3.0 of both apps with ClickOnce from Visual Studio as before (the profiles are already at 3.3.0.0).
3. The next time the client opens an app, ClickOnce updates it to 3.3.0, which downloads and silently installs the
   Velopack copy, opens it, and deletes the ClickOnce shortcuts. The Velopack copy then asks Windows to uninstall the
   ClickOnce copy; the client chooses **Remove the application from this computer**. If the download fails (offline),
   the ClickOnce copy opens as usual and tries again next time.
4. Keep the ClickOnce site up until every PC has moved over. Don't publish ClickOnce again.

The data isn't affected: it's in the LocalDB database, not the app's folder.

### Coding Style Requirements
Reference the [code style requirements](CodingStyle.md) for more information.

## Authors 
Reference the list of [contributors](https://github.com/CPTC-CPW/9TapTour/graphs/contributors) who participated in this project.