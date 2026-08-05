#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Core.Calculations
{
    /// <summary>
    /// Pure input validation and squad list helpers, split out of the
    /// WinForms FormHelper so they can be tested and reused headlessly.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Checks a string for numeric values
        /// true if all are numeric
        /// </summary>
        public static bool IsNumeric(string str)
        {
            bool isNum = int.TryParse(str, out _);
            return isNum;
        }

        /// <summary>
        /// Tests a string input for being an integer
        /// between 0-300 inclusive. Returns true if valid.
        /// </summary>
        public static bool IsAverageValid(string stringAverage)
        {
            Int32.TryParse(stringAverage, out int test);
            return test >= 0 && test <= 300;
        }

        /// <summary>
        /// Validates string input for being a date later
        /// than 1900. Returns true if valid.
        /// </summary>
        public static bool IsDateTimeValid(string stringDate)
        {
            DateTime century = new(1900, 01, 01);
            if (DateTime.TryParse(stringDate, out DateTime dateTime))
            {
                if (dateTime >= century)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Validates string input for being a US state.
        /// Returns true if valid.
        /// </summary>
        public static bool IsStateValid(string state)
        {
            string uppercaseState = state.ToUpper().Trim();
            string[] USstates =
                [
                    "AL","AK","AS","AZ","AR","CA","CO",
                    "CT","DE","DC","FM","FL","GA","GU",
                    "HI","ID","IL","IN","IA","KS","KY",
                    "LA","ME","MH","MD","MA","MI","MN",
                    "MS","MO","MT","NE","NV","NH","NJ",
                    "NM","NY","NC","ND","MP","OH","OK",
                    "OR","PW","PA","PR","RI","SC","SD",
                    "TN","TX","UT","VT","VI","VA","WA",
                    "WV","WI","WY"
                ];
            return uppercaseState.Length == 2 &&
                USstates.Contains(uppercaseState);
        }

        /// <summary>
        /// This method takes the list from GetFilterSeriesList()
        /// and returns a list of the chosen squads.
        /// 0 is all squads
        /// </summary>
        /// <param name="filterSeries">A list of 9 booleans determined by GRPQBS1n on FrmMemberScores</param>
        public static List<int> SquadNumList(List<bool> filterSeries)
        {
            List<int> squadList = [];
            for (int i = 0; i <= filterSeries.Count - 1; i++)
            {
                if (filterSeries[i] == true)
                {
                    squadList.Add(i);
                }
            }
            return squadList;
        }

        /// <summary>
        /// This method looks at the squad list and determine if it doesn't 'skip' squads
        /// e.g. if the list is 1,2,3 it's true. if 1,2,4 it's false.
        /// </summary>
        /// <param name="squadList">A list of squads selected in Filter series</param>
        public static bool IsContinuous(List<int> squadList) {
            for (int i = 1; i < squadList.Count(); i++) {
                if (squadList[i] - squadList[i - 1] != 1) {
                    return false;
                }
            }
            return true;
        }
    }
}
