using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Data;
using NineTapTour.Core.Services;
using System;
using System.IO;
using System.Linq;

namespace NineTapTour.IntegrationTests
{
    [TestClass]
    public class DatabaseMaintenanceServiceTests
    {
        [TestMethod]
        public void BackupAndRestore_RoundTrip_PreservesData()
        {
            DatabaseMaintenanceService service = new(TestDatabase.DbFactory);
            string backupPath = Path.Combine(Path.GetTempPath(), $"{TestDatabase.CatalogName}_roundtrip.bak");

            try
            {
                service.BackupTo(backupPath);
                Assert.IsTrue(File.Exists(backupPath), "backup file should exist");

                service.RestoreFrom(backupPath);

                using NineTapDb db = TestDatabase.DbFactory.CreateDbContext();
                Assert.AreEqual(7, db.Members.Count(), "all seeded members should survive the round trip");
                Assert.AreEqual(2, db.Tournaments.Count(), "both seeded tournaments should survive the round trip");
            }
            finally
            {
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
            }
        }

        [TestMethod]
        public void CreateBackupName_UsesCatalogNameAndBakExtension()
        {
            DatabaseMaintenanceService service = new(TestDatabase.DbFactory);
            string name = service.CreateBackupName();

            StringAssert.StartsWith(name, TestDatabase.CatalogName + "_");
            StringAssert.EndsWith(name, ".bak");
        }
    }
}
