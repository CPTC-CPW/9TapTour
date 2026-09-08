using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Calculations;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.IntegrationTests.Services
{
    /// <summary>
    /// Golden masters for the finalize grid extracted from FrmFinalizeTournament.
    /// The seeded members have no finalized history, so every computed column
    /// follows from the seed scores alone; the expectations were captured from
    /// the extracted service and cross-checked against the form's rules
    /// (handicap + bonus per counted game, 3-of-4 drops the lowest game, the
    /// first place casher in a 6-entry field loses all bonus pins).
    /// </summary>
    [TestClass]
    public class FinalizeGridServiceTests
    {
        private static FinalizeGridService CreateService()
        {
            var dbf = TestDatabase.DbFactory;
            return new FinalizeGridService(
                new TournamentRepository(dbf),
                new MemberRepository(dbf),
                new GameRepository(dbf),
                new FinalizeTempRepository(dbf),
                new PlayerHistoryRepository(dbf),
                new DoublesTeamRepository(dbf),
                dbf,
                new FinalizeCalculationService());
        }

        private record ExpectedRow(int Standing, int Member, bool[] Use, int Scratch, int Hdcp, int EntryAvg, double Avg30,
            int AdjAvg, int Handicap, int Bonus, int NewBonus, int Squad, bool Cashing);

        private static void AssertRows(FinalizeGridModel grid, ExpectedRow[] expected)
        {
            Assert.AreEqual(expected.Length, grid.Rows.Count, "row count");
            for (int i = 0; i < expected.Length; i++)
            {
                FinalizeGridRow row = grid.Rows[i];
                ExpectedRow e = expected[i];
                string where = $"row {i} (member {e.Member})";
                Assert.AreEqual(e.Standing, row.Standing, where + " standing");
                Assert.AreEqual(e.Member, row.MemberNumber, where + " member");
                CollectionAssert.AreEqual(e.Use, row.UseGames, where + " use flags");
                Assert.AreEqual(e.Scratch, row.ScratchTotal, where + " scratch");
                Assert.AreEqual(e.Hdcp, row.HdcpTotal, where + " hdcp total");
                Assert.AreEqual(e.EntryAvg, row.EntryAvg, where + " entry avg");
                Assert.AreEqual(e.Avg30, row.ThirtyEntryAvg ?? 0, 0.05, where + " 30 avg");
                Assert.AreEqual(e.AdjAvg, row.AdjAvg, where + " adj avg");
                Assert.AreEqual(e.Handicap, row.Hdcp, where + " handicap");
                Assert.AreEqual(e.Handicap, row.NewHdcp, where + " new handicap preview equals handicap when adj avg is the member average");
                Assert.AreEqual(e.Bonus, row.Bonus, where + " bonus");
                Assert.AreEqual(e.NewBonus, row.NewBonus, where + " new bonus");
                Assert.AreEqual(e.Squad, row.Squad, where + " squad");
                Assert.AreEqual(e.Cashing, row.IsCashing, where + " cashing");
                Assert.IsTrue(row.IsPlaced, where + " every seeded member has one entry, so every row is the placed entry");
                Assert.IsFalse(row.DirectorCheck, where + " director check defaults off");
                Assert.IsFalse(row.IsValid, where + " rows are invalid until the director checks them");
                Assert.IsNull(row.Earnings, where + " no earnings seeded");
            }
        }

        [TestMethod]
        public void BuildGrid_RegularTournament_GoldenMaster()
        {
            FinalizeGridModel grid = CreateService().BuildGrid(TestDatabase.RegularTournamentId);

            Assert.IsFalse(grid.IsFinalized);
            Assert.IsFalse(grid.IsDoubles);
            Assert.IsTrue(grid.ShowGame3);
            Assert.IsTrue(grid.ShowGame4);
            // Four entries is below the five needed for anyone to cash.
            Assert.AreEqual(0, grid.CashLine);

            bool[] all = [true, true, true, true];
            AssertRows(grid,
            [
                new(1, 101, all, 660, 920, 165, 165, 150, 63, 2, 2, 1, false),
                new(2, 106, [true, true, true, false], 570, 732, 190, 190, 160, 54, 0, 0, 1, false),
                new(3, 105, [true, true, false, false], 390, 536, 195, 195, 140, 70, 3, 3, 1, false),
                new(4, 107, [true, false, false, false], 210, 233, 210, 210, 200, 18, 5, 5, 1, false),
            ]);
        }

        [TestMethod]
        public void BuildGrid_ThreeOfFourTournament_GoldenMaster()
        {
            FinalizeGridModel grid = CreateService().BuildGrid(TestDatabase.ThreeOf4TournamentId);

            Assert.IsTrue(grid.ThreeOutOf4);
            Assert.AreEqual(1, grid.CashLine, "six entries: only first place cashes");

            AssertRows(grid,
            [
                // Lowest of four games is dropped; the casher loses every bonus pin.
                new(1, 103, [false, true, true, true], 637, 706, 212, 212.3, 200, 18, 5, 0, 1, true),
                new(2, 101, [false, true, true, true], 510, 705, 170, 170, 150, 63, 2, 2, 1, false),
                new(3, 104, [true, false, true, true], 690, 693, 230, 230, 220, 0, 1, 1, 2, false),
                new(4, 102, [true, true, true, false], 570, 678, 190, 190, 180, 36, 0, 0, 1, false),
                new(5, 105, [false, true, true, true], 330, 549, 110, 110, 140, 70, 3, 3, 2, false),
                new(6, 106, [false, true, true, true], 369, 531, 123, 123, 160, 54, 0, 0, 2, false),
            ]);
        }

        [TestMethod]
        public void BuildDetail_SeededMember_ShowsLiveEntryAndNoHistory()
        {
            FinalizeGridService service = CreateService();
            FinalizeGridModel grid = service.BuildGrid(TestDatabase.RegularTournamentId);

            FinalizeDetailModel detail = service.BuildDetail(grid, 107);

            Assert.AreEqual(107, detail.MemberNumber);
            Assert.AreEqual("Grace Gill", detail.MemberName);
            Assert.AreEqual(1, detail.Rows.Count, "one live entry, no finalized history");
            FinalizeDetailRow row = detail.Rows[0];
            Assert.IsTrue(row.IsCurrent);
            Assert.AreEqual(1, row.Games);
            Assert.AreEqual(210, row.Scratch);
            Assert.AreEqual(233, row.WithHandicap);
            Assert.AreEqual("4", row.Place);
            Assert.AreEqual(210, row.ThirtyAverage);
            Assert.AreEqual("Earnings (0.00)", detail.ColumnHeaders["Earnings"]);
        }

        [TestMethod]
        public void ApplyRowEdit_MirrorsMemberFieldsAndPersists_ThenFinalizeUpdatesMembers()
        {
            using ScratchTournament scratch = ScratchTournament.Create("Finalize Lanes", squads: 2);
            Member alice = scratch.AddMember(9301, "Ada", "Alpha", average: 150, handicap: 63, bonus: 2);
            Member bob = scratch.AddMember(9302, "Ben", "Beta", average: 180, handicap: 36, bonus: 0);
            int aliceSquad1 = scratch.AddEntry(alice, 1, 150, 160, 170, 180, 63, 2);
            int aliceSquad2 = scratch.AddEntry(alice, 2, 200, 200, 200, 200, 63, 2);
            int bobSquad1 = scratch.AddEntry(bob, 1, 180, 190, 200, 210, 36, 0);

            FinalizeGridService service = CreateService();
            FinalizeGridModel grid = service.BuildGrid(scratch.Tournament.Id);
            Assert.AreEqual(3, grid.Rows.Count);
            FinalizeGridRow aliceRow = grid.Rows.Single(r => r.GameId == aliceSquad1);

            // Director changes Ada's adjusted average on one entry: it mirrors to her other entry.
            grid = service.ApplyRowEdit(scratch.Tournament.Id, EditFrom(aliceRow, FinalizeEditField.AdjAvg) with { AdjAvg = 160 });
            Assert.IsTrue(grid.Rows.Where(r => r.MemberNumber == 9301).All(r => r.AdjAvg == 160), "adj avg mirrored to both entries");
            Assert.AreEqual(160, scratch.GetGame(aliceSquad1).AdjustedAvg);
            Assert.AreEqual(160, scratch.GetGame(aliceSquad2).AdjustedAvg);
            Assert.AreEqual(TournamentCalculations.CalculateHandicapPins(160), grid.Rows.First(r => r.MemberNumber == 9301).NewHdcp);
            Assert.IsTrue(grid.Rows.Single(r => r.GameId == bobSquad1).AdjAvg == 180, "other members untouched");

            // A game score edit persists and recomputes only that row.
            FinalizeGridRow bobRow = grid.Rows.Single(r => r.GameId == bobSquad1);
            grid = service.ApplyRowEdit(scratch.Tournament.Id, EditFrom(bobRow, FinalizeEditField.Game4) with { Game4 = 250 });
            bobRow = grid.Rows.Single(r => r.GameId == bobSquad1);
            Assert.AreEqual(250, scratch.GetGame(bobSquad1).Game4);
            Assert.AreEqual(180 + 190 + 200 + 250, bobRow.ScratchTotal);

            // Finalize refuses while director checks are missing, listing the offending games.
            FinalizeOutcome refused = service.Finalize(scratch.Tournament.Id);
            Assert.IsFalse(refused.Success);
            Assert.AreEqual(3, refused.InvalidGameIds.Count);

            // Director check mirrors to the member's other entries and persists.
            aliceRow = grid.Rows.Single(r => r.GameId == aliceSquad1);
            grid = service.ApplyRowEdit(scratch.Tournament.Id, EditFrom(aliceRow, FinalizeEditField.DirectorCheck) with { DirectorCheck = true });
            Assert.IsTrue(grid.Rows.Where(r => r.MemberNumber == 9301).All(r => r.DirectorCheck && r.IsValid));
            Assert.IsTrue(scratch.GetGame(aliceSquad2).KeepAdjustedAvg);
            bobRow = grid.Rows.Single(r => r.GameId == bobSquad1);
            grid = service.ApplyRowEdit(scratch.Tournament.Id, EditFrom(bobRow, FinalizeEditField.DirectorCheck) with { DirectorCheck = true });
            Assert.IsTrue(grid.Rows.All(r => r.IsValid));

            FinalizeOutcome finalized = service.Finalize(scratch.Tournament.Id);
            Assert.IsTrue(finalized.Success, finalized.Message);

            Member adaAfter = scratch.ReloadMember(9301);
            Assert.AreEqual(160, adaAfter.Average);
            Assert.AreEqual(TournamentCalculations.CalculateHandicapPins(160), adaAfter.Handicap);
            Assert.IsTrue(scratch.GetGame(aliceSquad1).IsFinalized);
            Assert.IsTrue(scratch.GetGame(aliceSquad2).IsFinalized);
            Assert.IsTrue(scratch.GetGame(bobSquad1).IsFinalized);
            Assert.AreEqual(180, scratch.ReloadMember(9302).Average);

            grid = service.BuildGrid(scratch.Tournament.Id);
            Assert.IsTrue(grid.IsFinalized);
            Assert.IsFalse(service.Finalize(scratch.Tournament.Id).Success, "second finalize is refused");

            // Edits are ignored once finalized.
            bobRow = grid.Rows.Single(r => r.GameId == bobSquad1);
            service.ApplyRowEdit(scratch.Tournament.Id, EditFrom(bobRow, FinalizeEditField.Game1) with { Game1 = 100 });
            Assert.AreEqual(180, scratch.GetGame(bobSquad1).Game1);
        }

        [TestMethod]
        public void BuildGrid_TwoDay_UsesStoredPlacesAndOverrides()
        {
            using ScratchTournament scratch = ScratchTournament.Create("Championship Lanes", squads: 1, isTwoDay: true);
            Member cara = scratch.AddMember(9303, "Cara", "Gamma", average: 170, handicap: 45, bonus: 4);
            int gameId = scratch.AddEntry(cara, 1, 180, 190, 200, 210, 45, 4, moneyWon: 100m);
            using (var db = TestDatabase.DbFactory.CreateDbContext())
            {
                Game game = db.Games.Single(g => g.Id == gameId);
                game.PlaceStanding = 3;
                game.PlaceStandingLabel = "3rd - 5th";
                db.SaveChanges();
            }

            FinalizeGridService service = CreateService();
            FinalizeGridModel grid = service.BuildGrid(scratch.Tournament.Id);
            FinalizeGridRow row = grid.Rows.Single();
            Assert.AreEqual(3, row.Standing, "2-day standings come from the results screen");
            Assert.IsTrue(row.IsCashing, "seeded from money won");
            Assert.AreEqual(4, row.NewBonus, "2-day never auto-deducts");

            grid = service.BuildGrid(scratch.Tournament.Id, new Dictionary<int, int> { [9303] = 1 });
            Assert.AreEqual(1, grid.Rows.Single().NewBonus, "director override wins");
        }

        private static FinalizeRowEdit EditFrom(FinalizeGridRow row, FinalizeEditField field)
        {
            return new FinalizeRowEdit(row.GameId, field, row.Game1, row.Game2, row.Game3, row.Game4,
                row.UseGame1, row.UseGame2, row.UseGame3, row.UseGame4, row.AdjAvg, row.DirectorCheck, row.Bonus,
                row.Earnings, row.Notes);
        }
    }
}
