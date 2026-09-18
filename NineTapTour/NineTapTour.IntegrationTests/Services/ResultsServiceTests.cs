using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Export;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.IntegrationTests.Services
{
    /// <summary>
    /// Golden masters for the results workflow extracted from FrmTournamentResults.
    /// </summary>
    [TestClass]
    public class ResultsServiceTests
    {
        private static ResultsService CreateService()
        {
            var dbf = TestDatabase.DbFactory;
            TournamentRepository tournaments = new(dbf);
            WinnersService winners = new(tournaments, new DoublesTeamRepository(dbf), new MemberRepository(dbf), dbf);
            return new ResultsService(tournaments, winners, new SeriesReportExcelExporter(), dbf);
        }

        [TestMethod]
        public void BuildResults_RegularTournament_TopThree()
        {
            ResultsGridModel grid = CreateService().BuildResults(TestDatabase.RegularTournamentId, 3);

            Assert.AreEqual(4, grid.TotalEntries);
            Assert.AreEqual(0, grid.CompEntries);
            Assert.IsFalse(grid.IsDoubles);
            CollectionAssert.AreEqual(
                new[] { "1|Alice Anderson|63 + 2|920|101", "2|Frank Fox|54 + 0|732|106", "3|Eve Evans|70 + 3|536|105" },
                grid.Rows.Select(r => $"{r.PlaceDisplay}|{r.FullName}|{r.HandicapDisplay}|{r.TotalScoreDisplay}|{r.MemberNumber}").ToArray());
            Assert.IsTrue(grid.Rows.All(r => !r.IsFiller && r.GameId > 0 && r.Earnings == 0 && r.ProgressivePotText == "0.00"));
        }

        [TestMethod]
        public void BuildResults_ThreeOfFour_DropsLowestGame()
        {
            ResultsGridModel grid = CreateService().BuildResults(TestDatabase.ThreeOf4TournamentId, 3);

            Assert.AreEqual(6, grid.TotalEntries);
            CollectionAssert.AreEqual(
                new[] { "1|Carol Chen|18 + 5|706", "2|Alice Anderson|63 + 2|705", "3|Dave Diaz|0 + 1|693" },
                grid.Rows.Select(r => $"{r.PlaceDisplay}|{r.FullName}|{r.HandicapDisplay}|{r.TotalScoreDisplay}").ToArray());
        }

        [TestMethod]
        public void BuildResults_MorePlacesThanEntries_AddsFillerRows()
        {
            ResultsGridModel grid = CreateService().BuildResults(TestDatabase.RegularTournamentId, 6);

            Assert.AreEqual(6, grid.Rows.Count);
            Assert.AreEqual(4, grid.Rows.Count(r => !r.IsFiller));
            CollectionAssert.AreEqual(new[] { "5", "6" }, grid.Rows.Where(r => r.IsFiller).Select(r => r.PlaceDisplay).ToArray());
        }

        [TestMethod]
        public void SaveResults_WritesPlaceEarningsAndPot_SkipsFillersAndDuplicates()
        {
            using ScratchTournament scratch = ScratchTournament.Create("Results Lanes", squads: 1);
            Member dan = scratch.AddMember(9401, "Dan", "Delta", 150, 63, 2);
            Member eve = scratch.AddMember(9402, "Eva", "Epsilon", 160, 54, 0);
            int danGame = scratch.AddEntry(dan, 1, 200, 200, 200, 200, 63, 2);
            int eveGame = scratch.AddEntry(eve, 1, 150, 150, 150, 150, 54, 0);

            ResultsSaveOutcome outcome = CreateService().SaveResults(scratch.Tournament.Id,
            [
                new ResultRowSave(danGame, "1T", 75m, "12.50"),
                new ResultRowSave(danGame, "9", 1m, "0"),          // duplicate game: first row wins
                new ResultRowSave(eveGame, "2", 25m, "twenty"),    // non-numeric pot lands in Notes
                new ResultRowSave(0, "3", 0m, "0.00"),             // filler row
            ]);

            Assert.AreEqual(2, outcome.SavedCount);
            Assert.AreEqual(0, outcome.Errors.Count);

            Game danSaved = scratch.GetGame(danGame);
            Assert.AreEqual(1, danSaved.PlaceStanding);
            Assert.IsNull(danSaved.PlaceStandingLabel);
            Assert.AreEqual(75m, danSaved.MoneyWon);
            Assert.AreEqual(12.50m, danSaved.SidePot);

            Game eveSaved = scratch.GetGame(eveGame);
            Assert.AreEqual(2, eveSaved.PlaceStanding);
            Assert.AreEqual(25m, eveSaved.MoneyWon);
            Assert.IsNull(eveSaved.SidePot);
            Assert.AreEqual("Progressive Pot was entered as: twenty", eveSaved.Notes);
        }

        [TestMethod]
        public void AddTwoDayRound_BuildsOneRowPerPlace()
        {
            List<TwoDayRow> rows = CreateService().AddTwoDayRound(46, 48, 50m);

            Assert.AreEqual(3, rows.Count);
            Assert.IsTrue(rows.All(r => r.PlaceLabel == "46th - 48th" && r.PlaceSortStart == 46 && r.Earnings == 50m));
            Assert.ThrowsExactly<ArgumentException>(() => CreateService().AddTwoDayRound(5, 4, 0m));
        }

        [TestMethod]
        public void TwoDay_AutoFill_Save_Load_RoundTrip()
        {
            using ScratchTournament scratch = ScratchTournament.Create("Two Day Lanes", squads: 1, isTwoDay: true);
            Member fay = scratch.AddMember(9403, "Fay", "Zeta", 170, 45, 4);
            int fayGame = scratch.AddEntry(fay, 1, 180, 190, 200, 210, 45, 4);
            ResultsService service = CreateService();

            TwoDayRow row = service.AddTwoDayRound(3, 3, 40m).Single();
            row.MemberNumber = 9403;
            service.AutoFillTwoDayRow(scratch.Tournament.Id, row);
            Assert.IsNull(row.StatusMessage);
            Assert.AreEqual("Fay Zeta", row.FullName);
            Assert.AreEqual(fayGame, row.GameId);
            Assert.IsTrue(row.TotalScore > 0);

            TwoDayRow unknown = service.AddTwoDayRound(4, 4, 0m).Single();
            unknown.MemberNumber = 999999;
            service.AutoFillTwoDayRow(scratch.Tournament.Id, unknown);
            StringAssert.Contains(unknown.StatusMessage, "not found");

            ResultsSaveOutcome outcome = service.SaveTwoDay(scratch.Tournament.Id,
            [
                new TwoDayRowSave(row.GameId, row.PlaceLabel, 40m, "5"),
                new TwoDayRowSave(row.GameId, "not a place", 1m, "0"),
            ]);
            Assert.AreEqual(1, outcome.SavedCount);
            Assert.AreEqual(1, outcome.Errors.Count);
            StringAssert.Contains(outcome.Errors[0], "row 2");

            Game saved = scratch.GetGame(fayGame);
            Assert.AreEqual(3, saved.PlaceStanding);
            Assert.AreEqual("3rd - 3rd", saved.PlaceStandingLabel);
            Assert.AreEqual(40m, saved.MoneyWon);
            Assert.AreEqual(5m, saved.SidePot);

            List<TwoDayRow> loaded = service.LoadTwoDay(scratch.Tournament.Id);
            TwoDayRow reloaded = loaded.Single();
            Assert.AreEqual("3rd - 3rd", reloaded.PlaceLabel);
            Assert.AreEqual(9403, reloaded.MemberNumber);
            Assert.AreEqual("Fay Zeta", reloaded.FullName);
            Assert.AreEqual(40m, reloaded.Earnings);
            Assert.AreEqual("5.00", reloaded.ProgressivePotText);
        }

        [TestMethod]
        public void BuildExportFileName_KeepsMacroExtension()
        {
            ResultsService service = CreateService();

            string xlsx = service.BuildExportFileName(TestDatabase.RegularTournamentId, @"C:\templates\checks.xlsx");
            string xlsm = service.BuildExportFileName(TestDatabase.RegularTournamentId, @"C:\templates\checks.XLSM");

            Assert.AreEqual("Golden Master Lanes Regular Golden Master 02-14-2026.xlsx", xlsx);
            Assert.AreEqual("Golden Master Lanes Regular Golden Master 02-14-2026.xlsm", xlsm);
        }
    }
}
