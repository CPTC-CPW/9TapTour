using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Calculations;
using NineTapTour.Forms;
using NineTapTour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTourTests
{
    [TestClass()]
    public class CalculationsTest
    {
        [TestMethod()]
        public void CalculatePlaceStandings_RemovesDuplicateBowlers()
        {
            List<MemberScores> members = GetMemberScoreTestData();
            int numBowlersWithDuplicates = members.Count;

            members = TournamentCalculations.CalculatePlaceStandings(members);

            int numBowlersAfterPlaceStandingsCalculation = members.Count;

            Assert.AreNotEqual(numBowlersWithDuplicates, numBowlersAfterPlaceStandingsCalculation);
        }

        private static List<MemberScores> GetMemberScoreTestData()
        {
            return
            [
                new MemberScores()
                {
                    FirstName = "A", LastName = "A",  MemberId= 1, Score = 800
                },
                new MemberScores()
                {
                    FirstName = "B", LastName = "B",  MemberId= 2, Score = 800
                },
                new MemberScores()
                {
                    FirstName = "C", LastName = "C",  MemberId= 3, Score = 800
                },
                new MemberScores()
                {
                    FirstName = "D", LastName = "D",  MemberId= 4, Score = 800
                },
                new MemberScores()
                {
                    FirstName = "E", LastName = "E",  MemberId= 5, Score = 650
                },
                new MemberScores()
                {
                    FirstName = "F", LastName = "F",  MemberId= 6, Score = 650
                },
                new MemberScores()
                {
                    FirstName = "G", LastName = "G",  MemberId= 7, Score = 500
                },
                new MemberScores()
                {
                    FirstName = "A", LastName = "A",  MemberId= 1, Score = 1000
                }
            ];
        }

        [TestMethod]
        public void CalculatePlaceStandings_DisplaysTiesForBowlersWithSameScores()
        {
            List<MemberScores> members = GetMemberScoreTestData();
            members = TournamentCalculations.CalculatePlaceStandings(members);

            //check first bowler is A with 1000; lesser score (800) is removed
            Assert.AreEqual(1, members[0].MemberId);
            Assert.AreEqual(1000, members[0].Score);
            Assert.AreEqual(1, members[0].placing);

            //check three way tie (next bowlers all have score of 800)
            Assert.AreEqual(2, members[1].placing);
            Assert.AreEqual(2, members[2].placing);
            Assert.AreEqual(2, members[3].placing);

            //next two members tie (next two bowlers tie with 650
            Assert.AreEqual(5, members[4].placing);
            Assert.AreEqual(5, members[5].placing);

            //last member in list (score of 500)
            Assert.AreEqual(7, members[6].placing);
        }

        [TestMethod]
        [DataRow(230, -9)]
        [DataRow(220, 0)]
        [DataRow(207, 11)]
        [DataRow(227, -6)] // verifies proper truncating
        [DataRow(270, -45)] // verifies proper truncating
        [DataRow(170, 45)]
        [DataRow(120, 70)] // max handicap should be 70
        public void CalculateHandicapPins_ReturnsExpectedAmount(int avg, int expectedHandicap)
        {
            int handicapResult = TournamentCalculations.CalculateHandicapPins(avg);
            Assert.AreEqual(expectedHandicap, handicapResult);
        }

        [TestMethod]
        [DataRow(106, 5, 20)]
        [DataRow(101, 0, 20)]
        [DataRow(100, 0, 20)]
        [DataRow(100, 5, 19)]
        [DataRow(100, 4, 19)]
        public void CalculateNumberOfMembersThatCanPlaceInATournament_ReturnsExpectedAmount(int totalParticipants, int compParticipants, int expectedNumThatCanPlace)
        {
            decimal resultNumThatCanPlace = TournamentCalculations.GetQtyOfMembersThatCanPlace(totalParticipants, compParticipants);
            Assert.AreEqual(expectedNumThatCanPlace, resultNumThatCanPlace);
        }

        [TestMethod]                      // Scenarios:
        [DataRow(0, 1, 10, 0)] // - Lost current game with null player history
        [DataRow(0, 2, 11, 0)] // - Lost current game with empty list player history
        [DataRow(0, 1, 0, 1)] // - Did not cash last 3 entries in different tournaments
        [DataRow(5, 1, 8, 5)] // - Max bonus pins should be 5 even while not cashing last 3 entries
        [DataRow(0, 1, 1, 0)] // - Cashed 2nd entry ago, 2nd tournament ago
        [DataRow(0, 1, 2, 0)] // - Cashed 3rd entry ago, 3rd tournament ago
        [DataRow(0, 1, 3, 1)] // - Cashed 4th entry ago, 4th tournament ago
        [DataRow(0, 1, 4, 1)] // - Cashed 3rd tournament ago but not last 3 entries (multiple tournament entry)
        [DataRow(0, 1, 5, 0)] // - Cashed 2nd tournament ago as multiple entry
        [DataRow(0, 1, 7, 0)] // - Cashed 3rd tournament ago as multiple entry
        [DataRow(0, 1, 6, 0)] // - Did not cash last 4 entries with a with 3 losses already applied
        [DataRow(0, 4, 0, 2)] // - 6 losses in a row not yet applied
        [DataRow(0, 4, 6, 1)] // - Has 6 losses in a row with 3 losses applied
        [DataRow(0, 4, 7, 1)] // - Has 6 losses in a row but won 2nd to last tournament that includes one of the wins
        [DataRow(0, 2, 2, 1)] // - Lost twice in current tourney, once in previous, but had cashed game 2 tourneys ago
        [DataRow(0, 2, 3, 1)] // - Lost twice in current tourney and twice in last two tournaments
        [DataRow(1, 4, 0, 3)] // - 6 losses in a row not yet applied, 1 initial
        [DataRow(2, 4, 0, 4)] // - 6 losses in a row not yet applied, 2 initial
        [DataRow(3, 4, 0, 5)] // - 6 losses in a row not yet applied, 3 initial
        [DataRow(4, 4, 0, 5)] // - 6 losses in a row not yet applied, 4 initial
        [DataRow(5, 4, 0, 5)] // - 6 losses in a row not yet applied, 4 initial
        public void AddToBonusPins_ReturnsExpectedBonusPins(int currentBonusPins, int currTourneyEntryCount, int playerHistoryListNum,
                                                            int expectedBonusPins)
        {
            List<PlayerHistoryViewModel> latestTournaments = GetPlayerHistoryTestData(playerHistoryListNum);

            int resultBonusPins = TournamentCalculations.AddToBonusPins(currentBonusPins, latestTournaments, currTourneyEntryCount);

            Assert.AreEqual(expectedBonusPins, resultBonusPins);
        }

        [TestMethod]
        public void MakeTopMembersByPlacementList_Doubles_DoesNotDeduplicateSameBowlerDifferentPartner()
        {
            // Bowler 1 appears twice — once paired with bowler 2, once with bowler 3.
            // Both entries are valid and must survive deduplication.
            List<MemberScores> doublesEntries =
            [
                new TeamMemberScores { MemberId = 1, Score = 1500, Partner1MemberId = 1, Partner2MemberId = 2 },
                new TeamMemberScores { MemberId = 1, Score = 1400, Partner1MemberId = 1, Partner2MemberId = 3 },
                new TeamMemberScores { MemberId = 4, Score = 1200, Partner1MemberId = 4, Partner2MemberId = 5 },
            ];

            List<MemberScores> result = TournamentCalculations.MakeTopMembersByPlacementList(doublesEntries, 3, isDoubles: true);

            Assert.HasCount(3, result, "All three doubles entries should be kept");
        }

        [TestMethod]
        public void MakeTopMembersByPlacementList_Doubles_AssignsCorrectPlacements()
        {
            List<MemberScores> doublesEntries =
            [
                new TeamMemberScores { MemberId = 1, Score = 1500, Partner1MemberId = 1, Partner2MemberId = 2 },
                new TeamMemberScores { MemberId = 1, Score = 1400, Partner1MemberId = 1, Partner2MemberId = 3 },
                new TeamMemberScores { MemberId = 4, Score = 1200, Partner1MemberId = 4, Partner2MemberId = 5 },
            ];

            List<MemberScores> result = TournamentCalculations.MakeTopMembersByPlacementList(doublesEntries, 3, isDoubles: true);

            Assert.AreEqual(1, result[0].placing);
            Assert.AreEqual(2, result[1].placing);
            Assert.AreEqual(3, result[2].placing);
        }

        [TestMethod]
        public void MakeTopMembersByPlacementList_NonDoubles_StillDeduplicatesSameBowler()
        {
            // Bowler 1 appears twice (different squads). Non-doubles should keep only the higher score.
            List<MemberScores> members =
            [
                new MemberScores { MemberId = 1, Score = 1000 },
                new MemberScores { MemberId = 1, Score = 800 },
                new MemberScores { MemberId = 2, Score = 900 },
            ];

            List<MemberScores> result = TournamentCalculations.MakeTopMembersByPlacementList(members, 3, isDoubles: false);

            Assert.HasCount(2, result, "Duplicate bowler 1 should be reduced to one entry");
            Assert.IsTrue(result.All(m => m.MemberId != 1 || m.Score == 1000), "Only the higher score for bowler 1 should remain");
        }

        private static List<PlayerHistoryViewModel> GetPlayerHistoryTestData(int listNum)
        {
            return listNum switch
            {
                // Did not cash last 3
                0 => [
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0, Bonus = 0
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0, Bonus = 0
                        }
                ],
                // Cashed 2nd tournament ago
                1 => [
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 1
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0
                        }
                ],
                // Cashed 3rd tournament ago
                2 => [
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 1
                        }
                ],
                // Cashed 4th tournament ago
                3 => [
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,28), MoneyWon = 1
                        }
                ],
                // Cashed 3rd tournament ago but not last 3 entries (multiple tournament entry)
                4 => [
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 1
                        }
                ],
                // Cashed 2nd tournament ago as multiple entry but not the other 2
                5 => [
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 1
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0
                        }
                ],
                // Did not cash last 3 with a multiple entry
                6 => [
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0
                        }
                ],
                // Cashed 3rd tournament ago as multiple entry but not the other 2
                7 => [
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 1
                        }
                ],
                8 => [
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0, Bonus = 5
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0, Bonus = 5
                        }
                ],
                // Gained a bonus pin last tournament
                9 => [
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0, Bonus = 1
                        },
                        new PlayerHistoryViewModel()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0, Bonus = 0
                        }
                ],
                10 => null,
                11 => [],
                _ => throw new ArgumentOutOfRangeException(),
            };
        }


    }
}