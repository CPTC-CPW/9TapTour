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

            Calculations.CalculatePlaceStandings(members);

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
            Calculations.CalculatePlaceStandings(members);
            
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
        [DataRow(270, -45)]
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

        [TestMethod]                   // Scenarios:
        [DataRow(0, 17, 12, 31, 0, 1)] // - Did not cash last 3
        [DataRow(5, 17, 12, 31, 1, 5)] // - Max bonus pins should be 5 even while not cashing last 3
        [DataRow(4, 17, 12, 31, 1, 4)] // - Cashed 2nd tournament ago
        [DataRow(3, 17, 12, 31, 2, 3)] // - Cashed 3rd tournament ago
        [DataRow(0, 17, 12, 31, 3, 1)] // - Cashed 4th tournament ago
        [DataRow(0, 17, 12, 31, 4, 0)] // - Cashed 3rd tournament ago but not last 3 entries (multiple tournament entry)
        [DataRow(0, 17, 12, 31, 5, 0)] // - Cashed 2nd tournament ago as multiple entry but not the other 2
        [DataRow(0, 17, 12, 31, 7, 0)] // - Cashed 3rd tournament ago as multiple entry but not the other 2
        [DataRow(0, 17, 12, 31, 6, 1)] // - Did not cash last 3 with a multiple entry
        public void AddToBonusPins_ReturnsExpectedBonusPins(int currentBonusPins,int currTournYear, int currTournMonth, int currTournDay, int playerHistoryListNum, int expectedBonusPins)
        {
            DateTime currTournamentDate = new DateTime(currTournYear, currTournMonth, currTournDay);
            List<PlayerHistory> last2Tournaments = GetPlayerHistoryTestData(playerHistoryListNum);

            int resultBonusPins = Calculations.AddToBonusPins(currentBonusPins, currTournamentDate, last2Tournaments);

            Assert.AreEqual(expectedBonusPins, resultBonusPins);
        }

        private static List<PlayerHistory> GetPlayerHistoryTestData(int listNum)
        {
            switch (listNum)
            {
                case 0:
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), Bonus = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), Bonus = 0
                        }
                    };
                case 1:
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), Bonus = 5
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), Bonus = 5
                        }
                    };
                case 2:
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), Bonus = 3
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), Bonus = 4
                        }
                    };
                case 3:
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), Bonus = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), Bonus = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,28), Bonus = 1
                        }
                    };
                case 4:
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), Bonus = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), Bonus = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), Bonus = 1
                        }
                    };
                case 5:
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), Bonus = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), Bonus = 1
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), Bonus = 0
                        }
                    };
                case 6:
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), Bonus = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), Bonus = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), Bonus = 0
                        }
                    };
                case 7:
                    return new List<PlayerHistory>()
                    {
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,30), Bonus = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), Bonus = 0
                        },
                        new PlayerHistory()
                        {
                            TournamentDate = new DateTime(17,12,29), Bonus = 1
                        }
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}