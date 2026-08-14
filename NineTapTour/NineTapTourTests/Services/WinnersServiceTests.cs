using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Services;
using NineTapTour.Core.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTourTests.Services
{
    /// <summary>
    /// Characterization tests for the pure winners-list logic extracted from
    /// FrmTournamentResults (M7.2). Expectations were hand-computed from the
    /// original form code and must not change.
    /// </summary>
    [TestClass]
    public class WinnersServiceOrdinalTests
    {
        [DataTestMethod]
        [DataRow(1, false, "1st")]
        [DataRow(2, false, "2nd")]
        [DataRow(3, false, "3rd")]
        [DataRow(4, false, "4th")]
        [DataRow(10, false, "10th")]
        // Teens always take "th"
        [DataRow(11, false, "11th")]
        [DataRow(12, false, "12th")]
        [DataRow(13, false, "13th")]
        [DataRow(21, false, "21st")]
        [DataRow(22, false, "22nd")]
        [DataRow(23, false, "23rd")]
        [DataRow(101, false, "101st")]
        [DataRow(111, false, "111th")]
        // Tie flag appends "T"
        [DataRow(1, true, "1stT")]
        [DataRow(12, true, "12thT")]
        public void GetOrdinalWithTie_MatchesCurrentBehavior(int place, bool isTie, string expected)
        {
            Assert.AreEqual(expected, WinnersService.GetOrdinalWithTie(place, isTie));
        }

        [DataTestMethod]
        [DataRow(46, 59, "46th - 59th")]
        [DataRow(1, 1, "1st - 1st")]
        [DataRow(2, 13, "2nd - 13th")]
        public void Build2DayPlaceGroupLabel_MatchesCurrentBehavior(int start, int end, string expected)
        {
            Assert.AreEqual(expected, WinnersService.Build2DayPlaceGroupLabel(start, end));
        }
    }

    [TestClass]
    public class WinnersServicePlaceParsingTests
    {
        [DataTestMethod]
        [DataRow("46th - 59th", true, 46)]
        [DataRow("46 - 59", true, 46)]
        [DataRow("5", true, 5)]
        [DataRow("3rd", true, 3)]
        [DataRow("12T", true, 12)]
        [DataRow(" 7th ", true, 7)]
        [DataRow("", false, 0)]
        [DataRow(null, false, 0)]
        [DataRow("abc", false, 0)]
        [DataRow("0", false, 0)]
        public void TryParsePlaceStartFromText_MatchesCurrentBehavior(string text, bool expectedSuccess, int expectedStart)
        {
            bool success = WinnersService.TryParsePlaceStartFromText(text, out int start);
            Assert.AreEqual(expectedSuccess, success);
            Assert.AreEqual(expectedStart, start);
        }

        [DataTestMethod]
        [DataRow("5T", (byte)5)]
        [DataRow("12", (byte)12)]
        [DataRow(null, (byte)0)]
        [DataRow("abc", (byte)0)]
        // Quirk: values over byte.MaxValue fail the byte parse and collapse to 0
        [DataRow("300", (byte)0)]
        public void ParsePlaceStanding_MatchesCurrentBehavior(string value, byte expected)
        {
            Assert.AreEqual(expected, WinnersService.ParsePlaceStanding(value));
        }
    }

    [TestClass]
    public class WinnersServiceTieMarkerTests
    {
        [TestMethod]
        public void ApplyTieMarkers_MarksAdjacentEqualPlaces()
        {
            List<string> result = WinnersService.ApplyTieMarkers(["1", "2", "2", "4"]);
            CollectionAssert.AreEqual(new[] { "1", "2T", "2T", "4" }, result);
        }

        [TestMethod]
        public void ApplyTieMarkers_MarksTieAtStart()
        {
            List<string> result = WinnersService.ApplyTieMarkers(["1", "1", "2"]);
            CollectionAssert.AreEqual(new[] { "1T", "1T", "2" }, result);
        }

        [TestMethod]
        public void ApplyTieMarkers_SkipsZeroAndNonNumericPlaces()
        {
            List<string> result = WinnersService.ApplyTieMarkers(["", "3", "3", "0", "0"]);
            CollectionAssert.AreEqual(new[] { "", "3T", "3T", "0", "0" }, result);
        }

        [TestMethod]
        public void ApplyTieMarkers_SingleRowUnchanged()
        {
            List<string> result = WinnersService.ApplyTieMarkers(["1"]);
            CollectionAssert.AreEqual(new[] { "1" }, result);
        }

        [TestMethod]
        public void ApplyTieMarkers_ThreeWayTieAllMarked()
        {
            List<string> result = WinnersService.ApplyTieMarkers(["2", "2", "2", "5"]);
            CollectionAssert.AreEqual(new[] { "2T", "2T", "2T", "5" }, result);
        }
    }

    [TestClass]
    public class WinnersServiceFillerPlaceTests
    {
        [DataTestMethod]
        // Singles: filler place is simply the row index + 1
        [DataRow(false, 5, 5, 6)]
        [DataRow(false, 5, 7, 8)]
        // Doubles: 2 filled rows per team, consecutive filler rows share a team place
        [DataRow(true, 4, 4, 3)]
        [DataRow(true, 4, 5, 3)]
        [DataRow(true, 4, 6, 4)]
        [DataRow(true, 4, 7, 4)]
        public void ComputeFillerPlace_MatchesCurrentBehavior(bool isDoubles, int filledRowCount, int fillerRowIndex, int expected)
        {
            Assert.AreEqual(expected, WinnersService.ComputeFillerPlace(isDoubles, filledRowCount, fillerRowIndex));
        }
    }

    [TestClass]
    public class WinnersServiceComputeWinnersRowsTests
    {
        private static WinnerListMemberViewModel Bowler(int memberNumber, int? handicap, int memberBonus,
            int? g1, int? g2, int? g3, int? g4, bool isComp = false)
        {
            return new WinnerListMemberViewModel
            {
                MemberNumber = memberNumber,
                BowlerName = $"Bowler {memberNumber}",
                Handicap = handicap,
                MemberBonus = memberBonus,
                Game1 = g1,
                Game2 = g2,
                Game3 = g3,
                Game4 = g4,
                IsComp = isComp,
                GameId = memberNumber * 100
            };
        }

        [TestMethod]
        public void PreviousHandicap_OverridesStoredHandicap()
        {
            var bowlers = new List<WinnerListMemberViewModel> { Bowler(10, 20, 2, 100, 120, null, null) };
            var prevHdcp = new Dictionary<int, int> { [10] = 15 };

            WinnersListResult result = WinnersService.ComputeWinnersRows(bowlers, prevHdcp, threeOutOf4: false);

            Assert.AreEqual(15, result.Winners[0].Handicap);
            // 100 + 120 + 2 valid games * (15 hdcp + 2 bonus) = 254
            Assert.AreEqual(254, result.Winners[0].TotalScore);
        }

        [TestMethod]
        public void MissingPreviousHandicap_FallsBackToStoredHandicap()
        {
            var bowlers = new List<WinnerListMemberViewModel> { Bowler(10, 20, 2, 100, 120, null, null) };

            WinnersListResult result = WinnersService.ComputeWinnersRows(bowlers, new Dictionary<int, int>(), threeOutOf4: false);

            Assert.AreEqual(20, result.Winners[0].Handicap);
            // 100 + 120 + 2 * (20 + 2) = 264
            Assert.AreEqual(264, result.Winners[0].TotalScore);
        }

        [TestMethod]
        public void ZeroPreviousHandicap_FallsBackToStoredHandicap()
        {
            var bowlers = new List<WinnerListMemberViewModel> { Bowler(10, 20, 0, 100, null, null, null) };
            var prevHdcp = new Dictionary<int, int> { [10] = 0 };

            WinnersListResult result = WinnersService.ComputeWinnersRows(bowlers, prevHdcp, threeOutOf4: false);

            Assert.AreEqual(20, result.Winners[0].Handicap);
            // 100 + 1 * 20 = 120
            Assert.AreEqual(120, result.Winners[0].TotalScore);
        }

        [TestMethod]
        public void ThreeOutOf4_FourGames_DropsLowestAndZeroesIt()
        {
            var bowlers = new List<WinnerListMemberViewModel> { Bowler(10, 10, 1, 100, 90, 110, 120) };

            WinnersListResult result = WinnersService.ComputeWinnersRows(bowlers, new Dictionary<int, int>(), threeOutOf4: true);

            // Lowest game (90) is dropped: 100 + 110 + 120 + 3 * (10 + 1) = 363
            Assert.AreEqual(363, result.Winners[0].TotalScore);
            Assert.AreEqual(0, result.Winners[0].Game2Score);
            Assert.AreEqual(100, result.Winners[0].Game1Score);
        }

        [TestMethod]
        public void ThreeOutOf4_ThreeGames_KeepsAllThree()
        {
            var bowlers = new List<WinnerListMemberViewModel> { Bowler(10, 10, 1, 100, null, 110, 120) };

            WinnersListResult result = WinnersService.ComputeWinnersRows(bowlers, new Dictionary<int, int>(), threeOutOf4: true);

            // Only 3 games bowled, nothing dropped: 100 + 110 + 120 + 3 * 11 = 363
            Assert.AreEqual(363, result.Winners[0].TotalScore);
        }

        [TestMethod]
        public void CompEntriesAndTotalEntries_AreCounted()
        {
            var bowlers = new List<WinnerListMemberViewModel>
            {
                Bowler(1, 5, 0, 100, null, null, null, isComp: true),
                Bowler(2, 5, 0, 100, null, null, null),
                Bowler(3, 5, 0, 100, null, null, null, isComp: true)
            };

            WinnersListResult result = WinnersService.ComputeWinnersRows(bowlers, new Dictionary<int, int>(), threeOutOf4: false);

            Assert.AreEqual(3, result.TotalEntries);
            Assert.AreEqual(2, result.CompEntries);
        }
    }

    [TestClass]
    public class WinnersServiceComputeDoublesWinnersRowsTests
    {
        private static WinnerListMemberViewModel DoublesBowler(int memberId, int memberNumber, int? handicap,
            int memberBonus, int? g1, int? g2, int squad, bool isComp = false)
        {
            return new WinnerListMemberViewModel
            {
                MemberId = memberId,
                MemberNumber = memberNumber,
                BowlerName = $"Bowler {memberNumber}",
                Handicap = handicap,
                MemberBonus = memberBonus,
                Game1 = g1,
                Game2 = g2,
                Squad = squad,
                IsComp = isComp,
                GameId = memberNumber * 100
            };
        }

        private static DoublesTeam Team(int member1Id, int member2Id, int squad)
        {
            return new DoublesTeam
            {
                Member1 = new Member { Id = member1Id },
                Member2 = new Member { Id = member2Id },
                Squad = squad
            };
        }

        [TestMethod]
        public void Teams_AreOrderedByCombinedTotal_WithSharedPlacePerPair()
        {
            var bowlers = new List<WinnerListMemberViewModel>
            {
                DoublesBowler(1, 11, 10, 1, 100, 110, squad: 1),
                DoublesBowler(2, 22, 20, 0, 90, 95, squad: 1),
                DoublesBowler(3, 33, 5, 2, 150, 140, squad: 1),
                DoublesBowler(4, 44, 15, 1, 120, 130, squad: 1)
            };
            var teams = new List<DoublesTeam> { Team(1, 2, 1), Team(3, 4, 1) };

            WinnersListResult result = WinnersService.ComputeDoublesWinnersRows(bowlers, teams, new Dictionary<int, int>());

            // Team A (members 11, 22): 100+110+90+95 + 2*(10+1) + 2*(20+0) = 457
            // Team B (members 33, 44): 150+140+120+130 + 2*(5+2) + 2*(15+1) = 586
            Assert.AreEqual(4, result.Winners.Count);
            // Higher total first, pairs stay consecutive
            Assert.AreEqual(33, result.Winners[0].MemberNumber);
            Assert.AreEqual(44, result.Winners[1].MemberNumber);
            Assert.AreEqual(586, result.Winners[0].TotalScore);
            Assert.AreEqual(586, result.Winners[1].TotalScore);
            Assert.AreEqual(1, result.Winners[0].PlaceStanding);
            Assert.AreEqual(1, result.Winners[1].PlaceStanding);
            Assert.AreEqual(11, result.Winners[2].MemberNumber);
            Assert.AreEqual(457, result.Winners[2].TotalScore);
            Assert.AreEqual(2, result.Winners[2].PlaceStanding);
            Assert.AreEqual(2, result.Winners[3].PlaceStanding);
        }

        [TestMethod]
        public void TiedTeams_ShareAPlace_AndNextTeamSkips()
        {
            var bowlers = new List<WinnerListMemberViewModel>
            {
                DoublesBowler(1, 11, 0, 0, 100, 100, squad: 1),
                DoublesBowler(2, 22, 0, 0, 100, 100, squad: 1),
                DoublesBowler(3, 33, 0, 0, 100, 100, squad: 1),
                DoublesBowler(4, 44, 0, 0, 100, 100, squad: 1),
                DoublesBowler(5, 55, 0, 0, 90, 90, squad: 1),
                DoublesBowler(6, 66, 0, 0, 90, 90, squad: 1)
            };
            var teams = new List<DoublesTeam> { Team(1, 2, 1), Team(3, 4, 1), Team(5, 6, 1) };

            WinnersListResult result = WinnersService.ComputeDoublesWinnersRows(bowlers, teams, new Dictionary<int, int>());

            // Two teams tied at 400 share place 1; third team (360) takes place 3
            int[] places = result.Winners.Select(w => w.PlaceStanding).ToArray();
            CollectionAssert.AreEqual(new[] { 1, 1, 1, 1, 3, 3 }, places);
        }

        [TestMethod]
        public void PreviousHandicap_IsUsedInCombinedTotal()
        {
            var bowlers = new List<WinnerListMemberViewModel>
            {
                DoublesBowler(1, 11, 10, 0, 100, 100, squad: 1),
                DoublesBowler(2, 22, 10, 0, 100, 100, squad: 1)
            };
            var teams = new List<DoublesTeam> { Team(1, 2, 1) };
            var prevHdcp = new Dictionary<int, int> { [11] = 25 };

            WinnersListResult result = WinnersService.ComputeDoublesWinnersRows(bowlers, teams, prevHdcp);

            // 400 scratch + 2*(25+0) + 2*(10+0) = 470
            Assert.AreEqual(470, result.Winners[0].TotalScore);
        }

        [TestMethod]
        public void TeamMissingAnEntry_IsSkipped()
        {
            var bowlers = new List<WinnerListMemberViewModel>
            {
                DoublesBowler(1, 11, 0, 0, 100, 100, squad: 1)
            };
            var teams = new List<DoublesTeam> { Team(1, 2, 1) };

            WinnersListResult result = WinnersService.ComputeDoublesWinnersRows(bowlers, teams, new Dictionary<int, int>());

            Assert.AreEqual(0, result.Winners.Count);
            Assert.AreEqual(1, result.TotalEntries);
        }
    }

    [TestClass]
    public class WinnersServiceTeamPairingTests
    {
        private static ExcelMember Member(int memberNumber, int place)
        {
            return new ExcelMember { MemberNumber = memberNumber, PlaceStanding = place, TotalScore = 500 - place };
        }

        [TestMethod]
        public void Pairings_AreBuiltFromConsecutivePairs_SortedByPlace()
        {
            List<ExcelMember> winners =
            [
                Member(1, 2), Member(2, 2),
                Member(3, 1), Member(4, 1)
            ];

            List<DoublesTeamPairing> pairings = WinnersService.BuildTeamPairings(winners, maxPlace: 2);

            Assert.AreEqual(2, pairings.Count);
            Assert.AreEqual(1, pairings[0].Place);
            Assert.AreEqual(3, pairings[0].Member1.MemberNumber);
            Assert.AreEqual(4, pairings[0].Member2.MemberNumber);
            Assert.AreEqual(2, pairings[1].Place);
            Assert.IsFalse(pairings[0].IsTie);
            Assert.IsFalse(pairings[1].IsTie);
        }

        [TestMethod]
        public void TiedTeams_AreFlaggedAsTies()
        {
            List<ExcelMember> winners =
            [
                Member(1, 1), Member(2, 1),
                Member(3, 1), Member(4, 1),
                Member(5, 3), Member(6, 3)
            ];

            List<DoublesTeamPairing> pairings = WinnersService.BuildTeamPairings(winners, maxPlace: 3);

            Assert.AreEqual(3, pairings.Count);
            Assert.IsTrue(pairings[0].IsTie);
            Assert.IsTrue(pairings[1].IsTie);
            Assert.IsFalse(pairings[2].IsTie);
        }

        [TestMethod]
        public void TeamsBeyondMaxPlace_AreExcluded()
        {
            List<ExcelMember> winners =
            [
                Member(1, 1), Member(2, 1),
                Member(3, 2), Member(4, 2),
                Member(5, 3), Member(6, 3)
            ];

            List<DoublesTeamPairing> pairings = WinnersService.BuildTeamPairings(winners, maxPlace: 2);

            Assert.AreEqual(2, pairings.Count);
            Assert.AreEqual(1, pairings[0].Place);
            Assert.AreEqual(2, pairings[1].Place);
        }
    }
}
