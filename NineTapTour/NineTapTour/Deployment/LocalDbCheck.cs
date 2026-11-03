using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace NineTapTour.Deployment;

/// <summary>
/// The apps keep their data in SQL Server Express LocalDB. ClickOnce installed it with the app, but the
/// Velopack installer can't, so the apps check for it when they open and offer to install it.
/// </summary>
public static class LocalDbCheck
{
    // SQL Server 2019 Express LocalDB, the version the ClickOnce installer set up, so a backup made on one
    // of the client's PCs restores on another.
    private const string InstallerFolderUrl = "https://download.microsoft.com/download/7/c/1/7c14e92e-bdcb-4f89-b7cf-93543e7112d1/";
    private const string InstallerFileName = "SqlLocalDB.msi";

    /// <summary>
    /// Returns true when the app can go on opening: the database isn't on LocalDB, LocalDB is installed,
    /// or the user just installed it. Returns false, after telling the user, when it's still missing.
    /// </summary>
    public static bool EnsureInstalled(string connectionString, string productName)
    {
        if (!UsesLocalDb(connectionString) || IsInstalled())
        {
            return true;
        }

        DialogResult answer = MessageBox.Show(
            $"{productName} keeps its data in Microsoft SQL Server Express LocalDB, which isn't installed on this PC." +
            $"{Environment.NewLine}{Environment.NewLine}Download and install it now? It's free from Microsoft (about 55 MB)." +
            " Windows asks for permission to install it, and its installer asks you to accept Microsoft's license.",
            "SQL Server LocalDB needed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (answer == DialogResult.No)
        {
            ShowStillMissing(productName);
            return false;
        }

        Exception failure = ProgressWindow.Run(productName, "Downloading SQL Server Express LocalDB from Microsoft...", async () =>
        {
            string installerPath = await FileDownload.ToTempAsync(InstallerFolderUrl, InstallerFileName);

            // The installer's own window shows Microsoft's license, so the user accepts it there.
            using Process installer = Process.Start(new ProcessStartInfo("msiexec.exe")
            {
                ArgumentList = { "/i", installerPath },
                UseShellExecute = false,
            });
            await installer.WaitForExitAsync();
        });

        if (IsInstalled())
        {
            return true;
        }

        if (failure != null)
        {
            Trace.WriteLine($"LocalDB install failed: {failure}");
        }
        ShowStillMissing(productName);
        return false;
    }

    /// <summary>
    /// True when the connection string points at a LocalDB instance, such as (localdb)\MSSQLLocalDB.
    /// </summary>
    public static bool UsesLocalDb(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return false;
        }

        string dataSource = new SqlConnectionStringBuilder(connectionString).DataSource;
        return dataSource.Trim().StartsWith("(localdb)", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// True when any version of LocalDB is installed. Each one adds itself to this registry key.
    /// </summary>
    public static bool IsInstalled()
    {
        using RegistryKey machine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        using RegistryKey versions = machine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions");
        return versions != null && versions.SubKeyCount > 0;
    }

    private static void ShowStillMissing(string productName)
    {
        MessageBox.Show(
            $"{productName} can't open without SQL Server Express LocalDB. Open {productName} again to install it," +
            " or ask the developer team for help.",
            "SQL Server LocalDB needed", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
