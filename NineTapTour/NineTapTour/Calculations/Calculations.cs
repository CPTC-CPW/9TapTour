﻿using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using NineTapTour.Models;

namespace NineTapTour.Calculations
{
    public static class Calculations
    {
        /// <summary>
        /// Constants are based on 9-Tap Rules for adding and deducting pins based on type
        /// of tournament and member placement, handicap calculations, and calculating the
        /// number of participants that can place in a tournament.  
        /// </summary>
       
        const int MAX_BONUS_PINS_ALLOWED = 5;
        const int NO_PINS_TO_DEDUCT = 0;
        const double DEDUCT_HALF = .5;
        const int DEDUCT_1 = 1;
        const int DEDUCT_2 = 2;
        const int DEDUCT_3 = 3;
        const int FIRST_PLACE = 1;
        const int MIN_PLACEMENT_DEDUCT_2_PINS = 6;
        const int MAX_PLACEMENT_DEDUCT_2_PINS = 10;
        const int MIN_PLACEMENT_DEDUCT_3_PINS = 2;
        const int MAX_PLACEMENT_DEDUCT_3_PINS = 5;
        const int MAX_HANDICAP_PINS = 70;
        const int BASE_AVERAGE_HANDICAP_CALCULATOR = 220; //This is a "magic number" used to calculate Bonus.
        const double PERCENTAGE_TO_CALCULATE_HANDICAP = .9;

        /// <summary>
        /// Calculates handicap pins to be 90% of the difference between 220 and a bowler's 9 tap tour average.
        /// The maximum a handicap can be is 70 pins.
        /// </summary>
        /// <param name="currentAverage"></param>
        /// <returns></returns>
        public static int CalculateHandicapPins(int currentAverage)
        {
            return Math.Min(MAX_HANDICAP_PINS, (BASE_AVERAGE_HANDICAP_CALCULATOR - currentAverage) * 9 / 10);
        }

        /// <summary>
        /// Returns the adjusted bonus pins after a tournament depending on if a bowler placed
        /// and what ranking a bowler placed.
        /// </summary>
        /// <param name="memberPlaced">Ranking a bowler placed. 0 if not placed</param>
        /// <param name="currentBonusPins">Bonus pins the participant had before this tournament</param>
        /// <param name="memNum">Member number that used to identify bowler by user</param>
        /// <param name="RegionID">RegionId from where the tournament is played</param>
        /// <param name="currTournamentDate">Date when the current tournament is taking place</param>
        /// <returns>Adjusted bonus pins after current tournament</returns>
        public static int GetAdjustedBonusPins(int memberPlaced, int currentBonusPins, int memNum, int RegionID, DateTime currTournamentDate)
        {
            if (memberPlaced > 0)
            {
                return  DeductFromBonusPins(memberPlaced, currentBonusPins);
            }
            return AddToBonusPins(currentBonusPins, memNum, RegionID, currTournamentDate, PlayerHistoryDB.GetLastFiveTournaments(memNum, RegionID));
        }

        /// <summary>
        /// Adds to bonus pins if necessary and returns the new total bonus pins for a member
        /// </summary>
        /// <param name="currentBonusPins">Bonus pins before calculating new bonus pins</param>
        /// <param name="memberNum">Member number for the member to calculate bonus pins</param>
        /// <param name="RegionID">Region the current tournament is taking place</param>
        /// <param name="currTournamentDate">The date the tournament took place</param>
        /// <param name="last2Tournaments">The last two distinct tournaments</param>
        /// <returns></returns>
        public static int AddToBonusPins(int currentBonusPins, int memberNum, int RegionID, DateTime currTournamentDate, List<PlayerHistory> last2Tournaments)
        {
            if (currentBonusPins == MAX_BONUS_PINS_ALLOWED)
            {
                return MAX_BONUS_PINS_ALLOWED;
            }

            if (last2Tournaments.Count >= 2)
            {
                // Filtering history where they had bowled in a different squad on the same day
                if (last2Tournaments[0].TournamentDate != last2Tournaments[1].TournamentDate &&
                   last2Tournaments[1].TournamentDate != currTournamentDate && 
                   currTournamentDate != last2Tournaments[0].TournamentDate)
                {
                    // Checks to see if the last 2 bowling history is the same as it is currently, 
                    // after 3 times not placing, they gain a bonus point
                    if (last2Tournaments[0].Bonus == last2Tournaments[1].Bonus &&
                       last2Tournaments[1].Bonus == currentBonusPins)
                    {
                        return ++currentBonusPins;
                    }
                }
                return currentBonusPins;
            }
            else
            {
                return currentBonusPins;
            }
        }

        public static int DeductFromBonusPins(int memberPlaced, int currentBonusPins)
        {
            int bonusPinsAfterDeduction;

            if (memberPlaced == FIRST_PLACE)
            {
                bonusPinsAfterDeduction = currentBonusPins - currentBonusPins;
            }
            else if (memberPlaced <= MAX_PLACEMENT_DEDUCT_3_PINS)
            {
                bonusPinsAfterDeduction = currentBonusPins - DEDUCT_3;
            }
            else if (memberPlaced <= MAX_PLACEMENT_DEDUCT_2_PINS)
            {
                bonusPinsAfterDeduction = currentBonusPins - DEDUCT_2;
            }
            else
            {
                bonusPinsAfterDeduction = currentBonusPins - DEDUCT_1;
            }

            //ensure that new bonus pins are 0 or greater
            if (bonusPinsAfterDeduction <= 0)
            {
                return NO_PINS_TO_DEDUCT;
            }
            else
            {
                return bonusPinsAfterDeduction;
            }
        }

        /// <summary>
        /// Number of participants that can place in tournament. Total entries minus comp entries (tournament 
        /// workers that do not have to pay entry fees) and divides by 5 using integer division. 
        /// </summary>
        /// <param name="totalEntries">all tournament participants including comp entries</param>
        /// <param name="compEntries">tournament participants that also work at tournament</param>
        /// <returns>the quantity of members that can place in a tournament</returns>
        public static int GetQtyOfMembersThatCanPlace(int totalEntries, int compEntries)
        {
            return (totalEntries - compEntries) / 5;
        }

        /// <summary>
        /// Calculate place standings of bowlers. Ties between bowlers result in the same placestanding
        /// </summary>
        /// <param name="temp"></param>
        public static void CalculatePlaceStandings(List<MemberScores> temp)
        {
            //remove duplicates
            RemoveDuplicateBowers(temp);

            //ensure bowlers are sorted by score
            temp.Sort(new MemberScoresComparer());
            temp.Reverse();


            int place = 1;
            for (int currPosition = 0; currPosition < temp.Count; currPosition++)
            {
                if (currPosition > 0 && temp[currPosition].Score == temp[currPosition - 1].Score)
                {
                    temp[currPosition].placing = temp[currPosition - 1].placing;
                }
                else
                {
                    temp[currPosition].placing = place;
                }
                place++;
            }
        }

        /// <summary>
        /// Removes the lower scores of duplicate bowlers by MemberId
        /// </summary>
        /// <param name="temp"></param>
        private static void RemoveDuplicateBowers(List<MemberScores> temp)
        {
            for (int i = 0; i < temp.Count; i++)
            {
                List<MemberScores> removal = new List<MemberScores>();
                for (int j = i + 1; j < temp.Count; j++)
                {
                    if(temp[i].MemberId == temp[j].MemberId)
                    {
                        if (temp[i].Score >= temp[j].Score)
                            removal.Add(temp[j]);
                        else
                            removal.Add(temp[i]);
                    }
                }

                foreach (MemberScores deleteMember in removal)
                {
                    temp.Remove(deleteMember);
                }
            }
        }
    }

    /// <summary>
    /// Currently sorts member scores in ASCENDING order
    /// </summary>
    public class MemberScoresComparer : IComparer<MemberScores>
    {
        int IComparer<MemberScores>.Compare(MemberScores x, MemberScores y)
        {
            int score1 = x.Score.HasValue ? (int)x.Score : 0;
            int score2 = y.Score.HasValue ? (int)y.Score : 0;
            return score1.CompareTo(score2);
        }
    }
}