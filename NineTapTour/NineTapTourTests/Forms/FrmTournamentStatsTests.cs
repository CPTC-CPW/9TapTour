using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Forms.Tests
{
    [TestClass()]
    public class FrmTournamentStatsTests
    {
        [TestMethod()]
        public void GetTop3OutOf4_ListOf4Scores_DropsLowestAndReturns3ScoresInDescendingOrder()
        {
            List<int?> testScores = new List<int?>()
            {
                200, 100, 150, 175
            };
            int minScore = testScores.Min().Value;

            List<int> top3Scores = FrmTournamentStats.GetTop3OutOf4(testScores);
            //returned scores are sorted in descending order
            List<int> expectedScores = new List<int>() { 200, 175, 150 };
            bool isLowestScorePresent = top3Scores.Any(score => score == minScore);

            Assert.AreEqual(expectedScores.Count, top3Scores.Count); //ensure 3 scores are returned when given 4
            Assert.IsFalse(isLowestScorePresent);                    //ensure lowest is dropped

            for (int i = 0; i < expectedScores.Count; i++)
            {
                //Test fails if the expected score is not found in the same spot in the top3Scores
                if (expectedScores[i] != top3Scores[i])
                    Assert.Fail("Expected score not found in the correct position. Scores should be in descending order");
            }
        }

        [TestMethod]
        public void GetTop3OutOf4_ListOf3OrLessScores_DoesNotDropLowest()
        {
            List<int?> testScores = new List<int?>()
            {
                100, 120, 110
            };

            List<int> result = FrmTournamentStats.GetTop3OutOf4(testScores);
            List<int> result2 = FrmTournamentStats.GetTop3OutOf4(testScores.GetRange(0, 2));

            //ensure list of scores with less than 4 games do not drop any
            Assert.AreEqual(result.Count, 3);
            Assert.AreEqual(result2.Count, 2);
        }
    }
}