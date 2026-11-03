#!/usr/bin/env bash
# Prints the version for the next release.
#
#   next-version.sh <change> [version]
#
# <change> is "Bug fixes", "New features" or "Breaking changes", which bump the patch, minor or major number
# of the latest release tag. Release tags are bare versions such as 3.2.3 (older ones may start with a v);
# 0.0.0 is used when there are none. A [version] such as 3.4.0 is used as is instead, as long as it's newer
# than the latest tag.
set -euo pipefail

change=${1:?usage: next-version.sh <change> [version]}
requested=${2:-}

latest=$(git tag --list | grep -E '^v?[0-9]+\.[0-9]+\.[0-9]+$' | sed 's/^v//' | sort -V | tail -n 1 || true)
latest=${latest:-0.0.0}

if [ -n "$requested" ]; then
  version=${requested#v}
  if ! [[ $version =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
    echo "Version '$requested' isn't of the form 1.2.3." >&2
    exit 1
  fi
  if [ "$version" = "$latest" ] || [ "$(printf '%s\n%s\n' "$latest" "$version" | sort -V | tail -n 1)" != "$version" ]; then
    echo "Version $version isn't newer than the latest release, $latest." >&2
    exit 1
  fi
  echo "$version"
  exit 0
fi

IFS=. read -r major minor patch <<< "$latest"
case "$change" in
  "Bug fixes")        patch=$((10#$patch + 1)) ;;
  "New features")     minor=$((10#$minor + 1)); patch=0 ;;
  "Breaking changes") major=$((10#$major + 1)); minor=0; patch=0 ;;
  *)
    echo "Unknown change '$change'. Use 'Bug fixes', 'New features' or 'Breaking changes'." >&2
    exit 1
    ;;
esac
echo "$major.$minor.$patch"
