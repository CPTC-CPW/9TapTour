using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Calculations
{
    class Calculations
    {
        public static int CalculateHandicapPins(int currentAverage)
        {
            int MAX_HANDICAP_PINS = 70;
            int BASE_AVERAGE_HANDICAP_CALCULATOR = 220;
            double PERCENTAGE_TO_CALCULATE_HANDICAP = .9;
            double calculateHandicap = Convert.ToDouble(BASE_AVERAGE_HANDICAP_CALCULATOR - currentAverage) * PERCENTAGE_TO_CALCULATE_HANDICAP;
            if(calculateHandicap > MAX_HANDICAP_PINS)
            {
                return MAX_HANDICAP_PINS;
            }
            else
            {
                return Convert.ToInt32(calculateHandicap);
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
            int MAX_BONUS_PINS_ALLOWED = 5;
            if(currentBonusPins == MAX_BONUS_PINS_ALLOWED)
            {
                return MAX_BONUS_PINS_ALLOWED;
            }
            else
            {
               return ++currentBonusPins;
            }

            throw new NotImplementedException();
        }

        public static int DeductBonusPins(int memberPlaced, int currentBonusPins, bool isDoublesTournament)
        {
            int bonusPinsAfterDeduction = 0;

            int NO_PINS_TO_DEDUCT = 0;
            int DEDUCT_HALF = 2;
            int DEDUCT_1 = 1;
            int DEDUCT_2 = 2;
            int DEDUCT_3 = 3;
            
            int PLACEMENT_DEDUCT_ALL_PINS = 1;
            int MIN_PLACEMENT_DEDUCT_2_PINS = 6;
            int MAX_PLACEMENT_DEDUCT_2_PINS = 10;
            int MIN_PLACEMENT_DEDUCT_3_PINS = 2;
            int MAX_PLACEMENT_DEDUCT_3_PINS = 5;

            if(memberPlaced == PLACEMENT_DEDUCT_ALL_PINS)
            {
                if (isDoublesTournament)
                {
                    bonusPinsAfterDeduction = currentBonusPins - (currentBonusPins / DEDUCT_HALF);
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
                    bonusPinsAfterDeduction = currentBonusPins - (DEDUCT_3 / DEDUCT_HALF);
                }
                else
                {
                    bonusPinsAfterDeduction = currentBonusPins - currentBonusPins;
                }
            }
            else if(memberPlaced >= MIN_PLACEMENT_DEDUCT_2_PINS && memberPlaced <= MAX_PLACEMENT_DEDUCT_2_PINS)
            {
                if (isDoublesTournament)
                {
                    bonusPinsAfterDeduction = currentBonusPins - (DEDUCT_2 / DEDUCT_HALF);
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
            if(bonusPinsAfterDeduction <= 0)
            {
                return NO_PINS_TO_DEDUCT;
            }
            else
            {
                return bonusPinsAfterDeduction;
            }
        }

        public static int CalculateNumberOfMembersThatCanPlaceInATournament(Participant p)
        {
          
            throw new NotImplementedException();
        }
    }
}
