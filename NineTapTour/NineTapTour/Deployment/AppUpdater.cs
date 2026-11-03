using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Velopack;

namespace NineTapTour.Deployment;

/// <summary>
/// Updates the app from the GitHub releases with Velopack. Each app is packaged on its own channel,
/// so it only looks at its own release list (releases.&lt;channel&gt;.json).
/// </summary>
public sealed class AppUpdater
{
    // Velopack reads a URL from the web and anything else as a folder.
    private readonly UpdateManager manager = new(DeployedApp.UpdateSource);
    private UpdateInfo update;

    /// <summary>
    /// Downloads the newest release if it's newer than this one and returns its version. Returns null when
    /// the app is up to date, wasn't installed by Velopack (ClickOnce or a debug build), or the check
    /// failed (offline). A downloaded update installs the next time the app opens.
    /// </summary>
    public async Task<string> DownloadUpdateAsync()
    {
        if (!manager.IsInstalled)
        {
            return null;
        }

        try
        {
            update = await manager.CheckForUpdatesAsync();
            if (update == null)
            {
                return null;
            }

            await manager.DownloadUpdatesAsync(update);
            return update.TargetFullRelease.Version.ToString();
        }
        catch (Exception ex)
        {
            // Not worth bothering the user with; the app checks again next time it opens.
            Trace.WriteLine($"Update check failed: {ex}");
            update = null;
            return null;
        }
    }

    /// <summary>
    /// Closes the app straight away, installs the downloaded update and opens the new version.
    /// </summary>
    public void RestartToUpdate()
    {
        if (update != null)
        {
            manager.ApplyUpdatesAndRestart(update.TargetFullRelease);
        }
    }
}
