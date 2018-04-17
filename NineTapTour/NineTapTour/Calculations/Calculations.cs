﻿using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Calculations
{
    public class Calculations
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

        public static int CalculateBonusPins(bool didMemberCash, int memberPlaced, int currentBonusPins, bool isDoublesTournament, int memNum,int RegionID)
        {
            
            int RETURN = 0;
            if (didMemberCash)
            {
              RETURN =  DeductBonusPins(memberPlaced, currentBonusPins, isDoublesTournament);
            }
            else
            {
               RETURN = AddBonusPins(currentBonusPins, isDoublesTournament, memNum, RegionID);
            }
            return RETURN;
          
        }

        public static int AddBonusPins(int currentBonusPins, bool isDoublesTournament, int MemNum, int RegionID)
        {
            if (currentBonusPins == MAX_BONUS_PINS_ALLOWED)
            {
                return MAX_BONUS_PINS_ALLOWED;
            }
            else if(PlayerHistoryDB.getLastFiveFromPlayerhistory(MemNum,RegionID).Count >= 3)
            {
                List<PlayerHistory> latest = PlayerHistoryDB.getLastFiveFromPlayerhistory(MemNum,RegionID);
                if(latest[0].TournamentDate != latest[1].TournamentDate &&
                   latest[1].TournamentDate != latest[2].TournamentDate && //filtering history where they had bowled in a diffrent sqaud on the same day
                   latest[2].TournamentDate != latest[0].TournamentDate)
                {
                    if(latest[0].Bonus == latest[1].Bonus &&
                       latest[1].Bonus == latest[2].Bonus &&  //checks to see if there last 3 history were the same, after 3 times not placing, they gain a bonus point
                       latest[2].Bonus == latest[0].Bonus)
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
        public static decimal CalculateNumberOfMembersThatCanPlaceInATournament(int numberOfParticipantsInTournament)
        {
            //grabs the ceiling of the double when divided by 5
            decimal numberOfPlacementsBasedOnParticipants =(Math.Round(Convert.ToDecimal(numberOfParticipantsInTournament / 5)));
            return numberOfPlacementsBasedOnParticipants;
        }
    }
}