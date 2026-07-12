using System.Collections.Generic;

namespace NineTapTour.Calculations
{
    public static class TournamentStatsCalculator
    {
        /// <summary>
        /// This method sorts scores and removes the lowest if 4 scores are present
        /// It returns  a list with the 3 highest scores listOfValidScores
        /// </summary>
        /// <param name="scores"></param>
        public static List<int> GetTop3OutOf4(int? game1, int? game2, int? game3, int? game4)
        {
            List<int?> scores = [game1, game2, game3, game4];
            List<int> listOfValidScores = [];
            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i].HasValue)
                    listOfValidScores.Add(scores[i].Value);
            }

            //after sorting I want to get rid of lowest score
            listOfValidScores.Sort();
            if (listOfValidScores.Count == 4)
                listOfValidScores.Remove(listOfValidScores[0]);

            listOfValidScores.Reverse();
            return listOfValidScores;
        }
    }
}
