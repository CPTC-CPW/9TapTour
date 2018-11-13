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
        [DataRow(5, 1)]
        [DataRow(0, 0)]
        [DataRow(6, 2)]
        public void CalculateNumberOfMembersThatCanPlaceInATournament_ReturnsExpectedAmount(int numParticipants, int expectedNumThatCanPlace)
        {
            decimal resultNumThatCanPlace = Calculations.CalculateNumberOfMembersThatCanPlaceInATournament(numParticipants);
            Assert.AreEqual(expectedNumThatCanPlace, resultNumThatCanPlace);
        }
    }
}