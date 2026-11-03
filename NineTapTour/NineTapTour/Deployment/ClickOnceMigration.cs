using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NineTapTour.Deployment;

/// <summary>
/// Moves a ClickOnce install over to Velopack. The last ClickOnce release installs the Velopack copy
/// and opens it, and the Velopack copy then removes the ClickOnce one.
/// </summary>
public static class ClickOnceMigration
{
    /// <summary>
    /// Passed to the Velopack copy when the ClickOnce copy opens it, so it knows to remove the ClickOnce copy.
    /// </summary>
    public const string MovedFromClickOnceArgument = "--moved-from-clickonce";

    /// <summary>
    /// When this is the ClickOnce copy, installs the Velopack copy if needed and opens it. Returns true when
    /// the Velopack copy is open and this one should exit. Returns false when this isn't the ClickOnce copy,
    /// or the install failed (offline); the ClickOnce copy then opens as usual and tries again next time.
    /// </summary>
    public static bool MoveToVelopack(DeployedApp app)
    {
        if (!IsClickOnceCopy(app))
        {
            return false;
        }

        if (!File.Exists(app.InstalledExePath) && !InstallVelopackCopy(app))
        {
            return false;
        }

        ProcessStartInfo velopackCopy = new(app.InstalledExePath)
        {
            ArgumentList = { MovedFromClickOnceArgument },
            UseShellExecute = false,
        };
        RemoveClickOnceVariables(velopackCopy);
        Process.Start(velopackCopy);
        return true;
    }

    /// <summary>
    /// In the Velopack copy, when the ClickOnce copy opened it: removes the ClickOnce copy's shortcuts and
    /// asks Windows to uninstall it.
    /// </summary>
    public static void RemoveClickOnceCopy(DeployedApp app, string[] args)
    {
        if (!args.Contains(MovedFromClickOnceArgument))
        {
            return;
        }

        try
        {
            DeleteClickOnceShortcuts(app);

            string uninstallCommand = FindClickOnceUninstallCommand(app);
            if (uninstallCommand == null)
            {
                return;
            }

            MessageBox.Show(
                $"{app.ProductName} now installs and updates itself a new way. Your data hasn't changed." +
                $"{Environment.NewLine}{Environment.NewLine}The old copy is still installed. Windows asks what to do with it next:" +
                " choose \"Remove the application from this computer\" and click OK.",
                app.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

            (string fileName, string arguments) = SplitCommand(uninstallCommand);
            Process.Start(new ProcessStartInfo(fileName, arguments) { UseShellExecute = false });
        }
        catch (Exception ex)
        {
            // The old copy staying installed does no harm: without its shortcuts nothing opens it.
            Trace.WriteLine($"Removing the ClickOnce copy failed: {ex}");
        }
    }

    /// <summary>
    /// ClickOnce sets ClickOnce_IsNetworkDeployed for the apps it starts. Its app cache folder is checked
    /// too, in case the app is started some other way.
    /// </summary>
    public static bool IsClickOnceCopy(DeployedApp app)
    {
        // The Velopack copy never is, even if it was started with ClickOnce's variables.
        string appFolder = Path.TrimEndingDirectorySeparator(AppContext.BaseDirectory);
        if (string.Equals(appFolder, app.InstalledFolder, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (bool.TryParse(Environment.GetEnvironmentVariable("ClickOnce_IsNetworkDeployed"), out bool isDeployed) && isDeployed)
        {
            return true;
        }

        return appFolder.Contains(@"\AppData\Local\Apps\2.0\", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Splits an uninstall command such as "rundll32.exe dfshim.dll,ShArpMaintain ..." into the program and its arguments.
    /// </summary>
    public static (string FileName, string Arguments) SplitCommand(string command)
    {
        command = command.Trim();
        if (command.StartsWith('"'))
        {
            int closingQuote = command.IndexOf('"', 1);
            if (closingQuote > 0)
            {
                return (command[1..closingQuote], command[(closingQuote + 1)..].Trim());
            }
        }

        int space = command.IndexOf(' ');
        return space < 0 ? (command, "") : (command[..space], command[(space + 1)..].Trim());
    }

    private static bool InstallVelopackCopy(DeployedApp app)
    {
        Exception failure = ProgressWindow.Run(app.ProductName,
            $"Installing the new version of {app.ProductName}. This only happens once and takes a minute or two.", async () =>
        {
            string setupPath = await FileDownload.ToTempAsync(DeployedApp.UpdateSource, app.SetupFileName);

            // The download comes from this app rather than a browser, so Windows doesn't block the unsigned installer.
            ProcessStartInfo setupStart = new(setupPath)
            {
                ArgumentList = { "--silent" },
                UseShellExecute = false,
            };
            RemoveClickOnceVariables(setupStart);
            using Process setup = Process.Start(setupStart);
            await setup.WaitForExitAsync();
            if (setup.ExitCode != 0)
            {
                throw new InvalidOperationException($"{app.SetupFileName} exited with code {setup.ExitCode}.");
            }
        });

        if (failure == null && File.Exists(app.InstalledExePath))
        {
            return true;
        }

        Trace.WriteLine($"Installing the Velopack copy failed: {failure}");
        MessageBox.Show(
            $"The new version of {app.ProductName} couldn't be installed, so this one opens as usual." +
            " It tries again the next time you open it. Check that the PC is connected to the internet.",
            app.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
    }

    /// <summary>
    /// Started programs inherit ClickOnce's variables, which would make the Velopack copy look like the ClickOnce one.
    /// </summary>
    private static void RemoveClickOnceVariables(ProcessStartInfo startInfo)
    {
        foreach (string name in startInfo.Environment.Keys.Where(key => key.StartsWith("ClickOnce_", StringComparison.OrdinalIgnoreCase)).ToList())
        {
            startInfo.Environment.Remove(name);
        }
    }

    private static void DeleteClickOnceShortcuts(DeployedApp app)
    {
        string shortcutName = app.ProductName + ".appref-ms";

        string desktopShortcut = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), shortcutName);
        File.Delete(desktopShortcut);

        // ClickOnce puts the Start menu shortcut in folders named after the publisher and suite.
        string startMenu = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        foreach (string shortcut in Directory.EnumerateFiles(startMenu, shortcutName, SearchOption.AllDirectories).ToList())
        {
            File.Delete(shortcut);

            DirectoryInfo folder = Directory.GetParent(shortcut);
            while (folder != null && !string.Equals(folder.FullName.TrimEnd('\\'), startMenu.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase)
                && !folder.EnumerateFileSystemInfos().Any())
            {
                folder.Delete();
                folder = folder.Parent;
            }
        }
    }

    private static string FindClickOnceUninstallCommand(DeployedApp app)
    {
        using RegistryKey uninstall = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
        if (uninstall == null)
        {
            return null;
        }

        foreach (string keyName in uninstall.GetSubKeyNames())
        {
            using RegistryKey entry = uninstall.OpenSubKey(keyName);
            string command = entry?.GetValue("UninstallString") as string;
            if (entry?.GetValue("DisplayName") as string == app.ProductName
                && command != null && command.Contains("dfshim", StringComparison.OrdinalIgnoreCase))
            {
                return command;
            }
        }
        return null;
    }
}
