using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Import;
using System;

namespace NineTapTourTests.Import
{
    /// <summary>
    /// Unit tests for the pure parsing/detection helpers used by the legacy
    /// member-history import (the end-to-end import path is covered by the
    /// integration tests).
    /// </summary>
    [TestClass]
    public class MemberHistoryImportParsingTests
    {
        [DataTestMethod]
        [DataRow("1st", 1)]
        [DataRow("4th", 4)]
        [DataRow("17th tie", 17)]
        [DataRow("9thHM", 9)]
        [DataRow("22nd", 22)]
        [DataRow(" 3rd ", 3)]
        public void ParseLegacyPlaceStanding_TakesLeadingDigits(string cell, int expected)
        {
            Assert.AreEqual(expected, MemberHistoryImportService.ParseLegacyPlaceStanding(cell));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("HM")]
        [DataRow("tie 4th")]
        [DataRow("0")]
        public void ParseLegacyPlaceStanding_NonPlacingValues_ReturnNull(string cell)
        {
            Assert.IsNull(MemberHistoryImportService.ParseLegacyPlaceStanding(cell));
        }

        [TestMethod]
        public void IsPlausibleTournamentDate_AcceptsRealTournamentDates()
        {
            Assert.IsTrue(MemberHistoryImportService.IsPlausibleTournamentDate(new DateTime(1980, 1, 1)));
            Assert.IsTrue(MemberHistoryImportService.IsPlausibleTournamentDate(new DateTime(2001, 5, 5)));
            Assert.IsTrue(MemberHistoryImportService.IsPlausibleTournamentDate(DateTime.Today));
        }

        [TestMethod]
        public void IsPlausibleTournamentDate_RejectsEpochAndFutureDates()
        {
            // Excel's zero serial ("1/0/1900") loads as 1899-12-31; serial 1 is 1900-01-01.
            Assert.IsFalse(MemberHistoryImportService.IsPlausibleTournamentDate(new DateTime(1899, 12, 31)));
            Assert.IsFalse(MemberHistoryImportService.IsPlausibleTournamentDate(new DateTime(1900, 1, 1)));
            Assert.IsFalse(MemberHistoryImportService.IsPlausibleTournamentDate(new DateTime(1979, 12, 31)));
            Assert.IsFalse(MemberHistoryImportService.IsPlausibleTournamentDate(DateTime.Today.AddDays(1)));
        }

        private static Game NewGame(int? g1, int? g2, int? g3, int? g4)
        {
            return new Game
            {
                Game1 = g1,
                Game2 = g2,
                Game3 = g3,
                Game4 = g4,
                UseGame1 = g1.HasValue,
                UseGame2 = g2.HasValue,
                UseGame3 = g3.HasValue,
                UseGame4 = g4.HasValue,
            };
        }

        [TestMethod]
        public void ApplyBestThreeOfFourDrop_BookTotalIsBestThree_MarksLowestUnused()
        {
            Game game = NewGame(150, 200, 210, 220);

            MemberHistoryImportService.ApplyBestThreeOfFourDropIfDetected(game, 630);

            Assert.IsFalse(game.UseGame1 ?? true);
            Assert.IsTrue(game.UseGame2 ?? false);
            Assert.IsTrue(game.UseGame3 ?? false);
            Assert.IsTrue(game.UseGame4 ?? false);
            Assert.AreEqual(630, game.ScratchTotal);
        }

        [TestMethod]
        public void ApplyBestThreeOfFourDrop_TieForLowest_DropsOnlyTheFirst()
        {
            Game game = NewGame(180, 150, 150, 200);

            MemberHistoryImportService.ApplyBestThreeOfFourDropIfDetected(game, 530);

            Assert.IsTrue(game.UseGame1 ?? false);
            Assert.IsFalse(game.UseGame2 ?? true);
            Assert.IsTrue(game.UseGame3 ?? false);
            Assert.IsTrue(game.UseGame4 ?? false);
        }

        [TestMethod]
        public void ApplyBestThreeOfFourDrop_BookTotalIsAllFourGames_LeavesEveryGameUsed()
        {
            Game game = NewGame(150, 200, 210, 220);

            MemberHistoryImportService.ApplyBestThreeOfFourDropIfDetected(game, 780);

            Assert.IsTrue(game.UseGame1 ?? false);
            Assert.IsTrue(game.UseGame2 ?? false);
            Assert.IsTrue(game.UseGame3 ?? false);
            Assert.IsTrue(game.UseGame4 ?? false);
        }

        [TestMethod]
        public void ApplyBestThreeOfFourDrop_MissingGameOrMissingTotal_DoesNothing()
        {
            Game threeGames = NewGame(180, 190, 200, null);
            MemberHistoryImportService.ApplyBestThreeOfFourDropIfDetected(threeGames, 570);
            Assert.IsTrue(threeGames.UseGame1 ?? false);
            Assert.IsTrue(threeGames.UseGame2 ?? false);
            Assert.IsTrue(threeGames.UseGame3 ?? false);

            Game noTotal = NewGame(150, 200, 210, 220);
            MemberHistoryImportService.ApplyBestThreeOfFourDropIfDetected(noTotal, -1);
            Assert.IsTrue(noTotal.UseGame1 ?? false);
        }

        [TestMethod]
        public void ApplyBestThreeOfFourDrop_LowestGameIsZero_IsUndetectableAndDoesNothing()
        {
            // With a 0 low game the best-3 and 4-game sums are identical, so the
            // row gives no 3-of-4 evidence and all games stay used.
            Game game = NewGame(0, 200, 210, 220);

            MemberHistoryImportService.ApplyBestThreeOfFourDropIfDetected(game, 630);

            Assert.IsTrue(game.UseGame1 ?? false);
            Assert.IsTrue(game.UseGame2 ?? false);
            Assert.IsTrue(game.UseGame3 ?? false);
            Assert.IsTrue(game.UseGame4 ?? false);
        }
    }
}
