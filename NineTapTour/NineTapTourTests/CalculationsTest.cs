using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Forms;
using NineTapTour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Calculations.Test
{
    [TestClass()]
    public class CalculationsTest
    {
        [TestMethod()]
        public void CalculatePlaceStandings_RemovesDuplicateBowlers()
        {
            List<MemberScores> members = GetMemberScoreTestData();
            int numBowlersWithDuplicates = members.Count;

            members = Calculations.CalculatePlaceStandings(members);

            int numBowlersAfterPlaceStandingsCalculation = members.Count;

            Assert.AreNotEqual(numBowlersWithDuplicates, numBowlersAfterPlaceStandingsCalculation);
        }

        private static List<MemberScores> GetMemberScoreTestData()
        {
            return new List<MemberScores>()
            {
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
            };
        }

        [TestMethod]
        public void CalculatePlaceStandings_DisplaysTiesForBowlersWithSameScores()
        {
            List<MemberScores> members = GetMemberScoreTestData();
            members = Calculations.CalculatePlaceStandings(members);
            
            //check first bowler is A with 1000; lesser score (800) is removed
            Assert.AreEqual(members[0].MemberId, 1);
            Assert.AreEqual(members[0].Score, 1000);
            Assert.AreEqual(members[0].placing, 1);

            //check three way tie (next bowlers all have score of 800)
            Assert.AreEqual(members[1].placing, 2);
            Assert.AreEqual(members[2].placing, 2);
            Assert.AreEqual(members[3].placing, 2);

            //next two members tie (next two bowlers tie with 650
            Assert.AreEqual(members[4].placing, 5);
            Assert.AreEqual(members[5].placing, 5);

            //last member in list (score of 500)
            Assert.AreEqual(members[6].placing, 7);
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
            int handicapResult = Calculations.CalculateHandicapPins(avg);
            Assert.AreEqual(handicapResult, expectedHandicap);
        }

        [TestMethod]
        [DataRow(106, 5, 20)]
        [DataRow(101, 0, 20)]
        [DataRow(100, 0, 20)]
        [DataRow(100, 5, 19)]
        [DataRow(100, 4, 19)]
        public void CalculateNumberOfMembersThatCanPlaceInATournament_ReturnsExpectedAmount(int totalParticipants, int compParticipants, int expectedNumThatCanPlace)
        {
            decimal resultNumThatCanPlace = Calculations.GetQtyOfMembersThatCanPlace(totalParticipants, compParticipants);
            Assert.AreEqual(expectedNumThatCanPlace, resultNumThatCanPlace);
        }

        [TestMethod]                      // Scenarios:
        [DataRow(0, 1, 10, 0)] // - Lost current game with null player history
        [DataRow(0, 2, 11, 0)] // - Lost current game with empty list player history
        [DataRow(0, 1, 0, 1)] // - Did not cash last 3 enties in different tournaments
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
            List<PlayerHistory> latestTournaments = GetPlayerHistoryTestData(playerHistoryListNum);

            int resultBonusPins = Calculations.AddToBonusPins(currentBonusPins, latestTournaments, currTourneyEntryCount);

            Assert.AreEqual(expectedBonusPins, resultBonusPins);
        }

        private static List<PlayerHistory> GetPlayerHistoryTestData(int listNum)
        {
            switch (listNum)
            {
                case 0: // Did not cash last 3
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0, Bonus = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0, Bonus = 0
                        }
                    };
                case 1: // Cashed 2nd tournament ago
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 1
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0
                        }
                    };
                case 2: // Cashed 3rd tournament ago
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 1
                        }
                    };
                case 3: // Cashed 4th tournament ago
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,28), MoneyWon = 1
                        }
                    };
                case 4: // Cashed 3rd tournament ago but not last 3 entries (multiple tournament entry)
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 1
                        }
                    };
                case 5: // Cashed 2nd tournament ago as multiple entry but not the other 2
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 1
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0
                        }
                    };
                case 6: // Did not cash last 3 with a multiple entry
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0
                        }
                    };
                case 7: // Cashed 3rd tournament ago as multiple entry but not the other 2
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 1
                        }
                    };
                case 8:
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0, Bonus = 5
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0, Bonus = 5
                        }
                    };
                case 9: // Gained a bonus pin last tournament
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), MoneyWon = 0, Bonus = 1
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), MoneyWon = 0, Bonus = 0
                        }
                    };
                case 10:
                    return null;
                case 11:
                    return new List<PlayerHistory>();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


    }
}