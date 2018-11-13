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
        /// method uses integer division based on client's calculation 
        /// </summary>
        /// <param name="currentAverage"></param>
        /// <returns></returns>
        public static int CalculateHandicapPins(int currentAverage)
        {

            double calculateHandicap = Convert.ToDouble(BASE_AVERAGE_HANDICAP_CALCULATOR - currentAverage) * PERCENTAGE_TO_CALCULATE_HANDICAP;
            if (calculateHandicap > MAX_HANDICAP_PINS)
            {
                return MAX_HANDICAP_PINS;
            }
            else
            {
                return (int)(calculateHandicap);
            }
        }

        public static int AdjustBonusPins(int memberPlaced, int currentBonusPins, int memNum, int RegionID, DateTime currentT)
        {
            if (memberPlaced > 0)
            {
                return  DeductFromBonusPins(memberPlaced, currentBonusPins);
            }
            return AddToBonusPins(currentBonusPins, memNum, RegionID, currentT);
        }

        public static int AddToBonusPins(int currentBonusPins, int MemNum, int RegionID, DateTime currenT)
        {
            if (currentBonusPins == MAX_BONUS_PINS_ALLOWED)
            {
                return MAX_BONUS_PINS_ALLOWED;
            }
            List<PlayerHistory> latestTournaments = PlayerHistoryDB.getLastFiveTournaments(MemNum, RegionID);

            if (latestTournaments.Count >= 2)
            {
                // Filtering history where they had bowled in a diffrent sqaud on the same day
                if (latestTournaments[0].TournamentDate != latestTournaments[1].TournamentDate &&
                   latestTournaments[1].TournamentDate != currenT && 
                   currenT != latestTournaments[0].TournamentDate)
                {
                    // Checks to see if the last 2 bowling history is the same as it is currently, 
                    // after 3 times not placing, they gain a bonus point
                    if (latestTournaments[0].Bonus == latestTournaments[1].Bonus &&
                       latestTournaments[1].Bonus == currentBonusPins)
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
        /// method rounds half pin deduction to the nearest whole number based
        /// on client's calculation
        /// </summary>
        /// <param name="halfPinDeduction"></param>
        /// <returns></returns>
        public static int DoublesTournamentRoundPinValue(double halfPinDeduction)
        {
            int doublesPinDeduction = (int)Math.Round(halfPinDeduction, MidpointRounding.AwayFromZero);
            return doublesPinDeduction;
        }

        /// <summary>
        /// method uses integer division based on client's calculation 1 in 5 participants
        /// place in a tournament
        /// </summary>
        /// <param name="numberOfParticipantsInTournament"></param>
        /// <returns></returns>
        public static decimal CalculateNumberOfMembersThatCanPlaceInATournament(int numberOfParticipantsInTournament)
        {
            //grabs the ceiling of the double when divided by 5
            decimal numberOfPlacementsBasedOnParticipants =(Math.Round(Convert.ToDecimal(numberOfParticipantsInTournament / 5)));
            return numberOfPlacementsBasedOnParticipants;
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