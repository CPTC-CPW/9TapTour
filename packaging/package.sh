#!/usr/bin/env bash
# Builds 9-Tap Tour and 9-Tap Member Import and packages each with Velopack, so installed copies update
# themselves from the GitHub releases.
#
#   VERSION=1.2.3 packaging/package.sh
#
# Run from the repository root on Windows (Git Bash), with vpk (dotnet tool install -g vpk) on the PATH at the
# same version as the Velopack package in NineTapTour.csproj. Each app is its own Velopack channel, named after
# the app in lower case, so an installed app only ever updates to its own package. Writes to dist/:
#
#   NineTapTour-Setup.exe, NineTapMemberImport-Setup.exe          installers (current user, no admin needed)
#   NineTapTour-Portable.zip, NineTapMemberImport-Portable.zip    run from any folder and still update
#   <App>-<version>-<app>-full.nupkg, releases.<app>.json         what installed apps download to update
#
# The pack ids and titles must match NineTapTour/NineTapTour/Deployment/DeployedApp.cs. The titles are the
# names ClickOnce used, so the shortcuts keep their names.
set -euo pipefail

version=${VERSION:?set VERSION, e.g. VERSION=1.2.3}
mkdir -p dist
dist=$(cd dist && pwd)

# package <project> <exe it builds> <pack id> <title>
package() {
  local project=$1 exe=$2 id=$3 title=$4
  local out="out/$id"
  rm -rf "$out"

  # Self-contained, so the PC doesn't need the .NET runtime.
  dotnet publish "$project" --configuration Release --runtime win-x64 --self-contained --output "$out" \
    -p:DebugType=none -p:Version="$version"

  # The installed .exe is named after the pack id. Renaming it is fine: it still finds the .dll it was built for.
  if [[ $exe != "$id.exe" ]]; then
    mv "$out/$exe" "$out/$id.exe"
  fi

  # vpk lower-cases channel names, so this spells it the way the file names will.
  local channel=${id,,}

  # No delta packages: each update downloads the whole app, which keeps the release simple.
  vpk pack --packId "$id" --packTitle "$title" --packAuthors "Clover Park Technical College" \
    --packVersion "$version" --channel "$channel" --runtime win-x64 --delta None \
    --packDir "$out" --mainExe "$id.exe" --icon NineTapTour/NineTapTour/Icon/9tap_white.ico --outputDir "$dist"

  # vpk names the installers after the channel too; the shorter names are the ones the apps and README use.
  mv "$dist/$id-$channel-Setup.exe" "$dist/$id-Setup.exe"
  mv "$dist/$id-$channel-Portable.zip" "$dist/$id-Portable.zip"
}

package NineTapTour/NineTapTour/NineTapTour.csproj NineTapTour.exe NineTapTour "9-Tap Tour 2021 - Manager"
package NineTapTour/MemberImportTest/MemberImportTest/MemberImportData.csproj MemberImportTest.exe NineTapMemberImport "9-Tap Member Import 2021"

# vpk also writes assets.<channel>.json (its own list of what it built) and RELEASES-<channel> (an older update
# list). The apps read neither, only releases.<channel>.json and the .nupkg, so they'd only clutter the release.
rm -f "$dist"/assets.*.json "$dist"/RELEASES-*
