﻿using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Calculations
{
    static class Calculations
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
        const int FIRST_PLACE_DEDUCTION = 1;
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

        public static int CalculateBonusPins(bool didMemberCash, int memberPlaced, int currentBonusPins, bool isDoublesTournament)
        {
            if (didMemberCash)
            {
                DeductBonusPins(memberPlaced, currentBonusPins, isDoublesTournament);
            }
            else
            {
                AddBonusPins(currentBonusPins, isDoublesTournament);
            }
            throw new NotImplementedException();
        }

        public static int AddBonusPins(int currentBonusPins, bool isDoublesTournament)
        {
            if (currentBonusPins == MAX_BONUS_PINS_ALLOWED)
            {
                return MAX_BONUS_PINS_ALLOWED;
            }
            else
            {
                return currentBonusPins++;
            }
        }

        public static int DeductBonusPins(int memberPlaced, int currentBonusPins, bool isDoublesTournament)
        {
            int bonusPinsAfterDeduction;

            if (memberPlaced == FIRST_PLACE_DEDUCTION)
            {
                if (isDoublesTournament)
                {
                    bonusPinsAfterDeduction = currentBonusPins - DoublesTournamentRoundPinValue(DEDUCT_HALF);
                }
                else
                {
                    bonusPinsAfterDeduction = currentBonusPins - currentBonusPins;
                }
            }
            else if (memberPlaced >= MIN_PLACEMENT_DEDUCT_3_PINS && memberPlaced <= MAX_PLACEMENT_DEDUCT_3_PINS)
            {
                if (isDoublesTournament)
                {
                    bonusPinsAfterDeduction = currentBonusPins - DoublesTournamentRoundPinValue(DEDUCT_HALF);
                }
                else
                {
                    bonusPinsAfterDeduction = currentBonusPins - DEDUCT_3;
                }
            }
            else if (memberPlaced >= MIN_PLACEMENT_DEDUCT_2_PINS && memberPlaced <= MAX_PLACEMENT_DEDUCT_2_PINS)
            {
                if (isDoublesTournament)
                {
                    bonusPinsAfterDeduction = currentBonusPins - DoublesTournamentRoundPinValue(DEDUCT_HALF);
                }
                else
                {
                    bonusPinsAfterDeduction = currentBonusPins - DEDUCT_2;
                }
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
        public static int CalculateNumberOfMembersThatCanPlaceInATournament(int numberOfParticipantsInTournament)
        {
            int numberOfPlacementsBasedOnParticipants = numberOfParticipantsInTournament / 5;
            return numberOfPlacementsBasedOnParticipants;
        }
    }
}