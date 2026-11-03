using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Deployment;
using System;
using System.IO;

namespace NineTapTourTests.Deployment
{
    [TestClass]
    public class DeploymentTests
    {
        [TestMethod]
        [DataRow(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NineTapDb2025;Integrated Security=True", true)]
        [DataRow(@"Server=(LocalDB)\MSSQLLocalDB;Database=NineTapDb2025;Trusted_Connection=True", true)]
        [DataRow(@"Data Source=.\SQLEXPRESS;Initial Catalog=NineTapDb2025;Integrated Security=True", false)]
        [DataRow(@"Server=tcp:example.database.windows.net;Database=NineTapDb2025", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        public void UsesLocalDb_DetectsLocalDbDataSources(string connectionString, bool expected)
        {
            Assert.AreEqual(expected, LocalDbCheck.UsesLocalDb(connectionString));
        }

        [TestMethod]
        public void SplitCommand_ClickOnceUninstallString()
        {
            (string fileName, string arguments) = ClickOnceMigration.SplitCommand(
                "rundll32.exe dfshim.dll,ShArpMaintain NineTapTour.application, Culture=neutral, PublicKeyToken=0000000000000000, processorArchitecture=amd64");

            Assert.AreEqual("rundll32.exe", fileName);
            Assert.AreEqual("dfshim.dll,ShArpMaintain NineTapTour.application, Culture=neutral, PublicKeyToken=0000000000000000, processorArchitecture=amd64", arguments);
        }

        [TestMethod]
        public void SplitCommand_QuotedProgram()
        {
            (string fileName, string arguments) = ClickOnceMigration.SplitCommand(@"""C:\Program Files\Tool\remove.exe"" /quiet");

            Assert.AreEqual(@"C:\Program Files\Tool\remove.exe", fileName);
            Assert.AreEqual("/quiet", arguments);
        }

        [TestMethod]
        public void SplitCommand_ProgramOnly()
        {
            Assert.AreEqual(("remove.exe", ""), ClickOnceMigration.SplitCommand("remove.exe"));
        }

        [TestMethod]
        public void IsClickOnceCopy_NotForTestRun()
        {
            Assert.IsFalse(ClickOnceMigration.IsClickOnceCopy(DeployedApp.Tour));
        }

        [TestMethod]
        public void InstalledExePath_IsInVelopacksCurrentFolder()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            Assert.AreEqual(Path.Combine(localAppData, "NineTapTour", "current", "NineTapTour.exe"), DeployedApp.Tour.InstalledExePath);
            Assert.AreEqual(Path.Combine(localAppData, "NineTapMemberImport", "current", "NineTapMemberImport.exe"), DeployedApp.MemberImport.InstalledExePath);
        }

        /// <summary>
        /// The ClickOnce copy downloads the Setup file by name and the apps' shortcuts are named after the
        /// titles, so the packaging script has to use the same names as DeployedApp.
        /// </summary>
        [TestMethod]
        [DataRow("Tour")]
        [DataRow("MemberImport")]
        public void PackageScript_MatchesDeployedApp(string appName)
        {
            DeployedApp app = appName == "Tour" ? DeployedApp.Tour : DeployedApp.MemberImport;
            string script = File.ReadAllText(FindRepositoryFile(Path.Combine("packaging", "package.sh")));

            StringAssert.Contains(script, $" {app.PackId} \"{app.ProductName}\"");
            StringAssert.Contains(script, "\"$dist/$id-Setup.exe\"");
            Assert.AreEqual(app.PackId + "-Setup.exe", app.SetupFileName);
        }

        private static string FindRepositoryFile(string relativePath)
        {
            for (DirectoryInfo folder = new(AppContext.BaseDirectory); folder != null; folder = folder.Parent)
            {
                string path = Path.Combine(folder.FullName, relativePath);
                if (File.Exists(path))
                {
                    return path;
                }
            }
            throw new FileNotFoundException($"Couldn't find {relativePath} above {AppContext.BaseDirectory}.");
        }
    }
}
