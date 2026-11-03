using System;
using System.IO;
using System.Windows.Forms;

namespace NineTapTour.Deployment;

/// <summary>
/// One of the client's apps as Velopack installs it from the GitHub releases, and as ClickOnce
/// installed it before that. The names must match packaging/package.sh.
/// </summary>
public sealed class DeployedApp
{
    public static readonly DeployedApp Tour = new("NineTapTour", "9-Tap Tour 2021 - Manager");

    public static readonly DeployedApp MemberImport = new("NineTapMemberImport", "9-Tap Member Import 2021");

    // "latest/download" always serves the newest release's files.
    private const string ReleaseFilesUrl = "https://github.com/CPTC-CPW/9TapTour/releases/latest/download/";

    private DeployedApp(string packId, string productName)
    {
        PackId = packId;
        ProductName = productName;
    }

    /// <summary>
    /// Velopack's id for the app, which also names its install folder, its .exe and its Setup file.
    /// </summary>
    public string PackId { get; }

    /// <summary>
    /// The name on the app's shortcuts. ClickOnce used the same name, so the client sees no change.
    /// </summary>
    public string ProductName { get; }

    public string SetupFileName => PackId + "-Setup.exe";

    /// <summary>
    /// The running app's version, such as 3.3.0, without the commit the SDK adds after a "+".
    /// </summary>
    public static string CurrentVersion => Application.ProductVersion.Split('+')[0];

    /// <summary>
    /// Where Velopack installs the app for the current Windows user. Its shortcuts open this .exe,
    /// and updates replace the "current" folder's contents.
    /// </summary>
    public string InstalledFolder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PackId, "current");

    public string InstalledExePath => Path.Combine(InstalledFolder, PackId + ".exe");

    /// <summary>
    /// The folder or URL holding the packaged releases. NINETAP_UPDATE_SOURCE, a folder or URL,
    /// tries out a release before it's published.
    /// </summary>
    public static string UpdateSource
    {
        get
        {
            string source = Environment.GetEnvironmentVariable("NINETAP_UPDATE_SOURCE");
            return string.IsNullOrEmpty(source) ? ReleaseFilesUrl : source;
        }
    }
}
