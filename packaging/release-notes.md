## Installing

9-Tap Tour and 9-Tap Member Import keep themselves up to date. You only download them once, and PCs that had the ClickOnce version move over on their own the next time the app opens.

To install on a new PC, download the Setup file from **Assets** below and double-click it:

- [`NineTapTour-Setup.exe`](https://github.com/CPTC-CPW/9TapTour/releases/download/{{VERSION}}/NineTapTour-Setup.exe) for 9-Tap Tour.
- [`NineTapMemberImport-Setup.exe`](https://github.com/CPTC-CPW/9TapTour/releases/download/{{VERSION}}/NineTapMemberImport-Setup.exe) for 9-Tap Member Import.

The installers aren't code-signed, so if Windows says "Windows protected your PC", click **More info**, then **Run anyway**. They install for your Windows account (no administrator password needed). The apps need Microsoft SQL Server Express LocalDB and offer to install it the first time they open if it's missing.

The other files in **Assets** (`.nupkg`, `.json`, `Portable.zip`) are what installed apps download to update, or copies that run from any folder. You don't need them.
