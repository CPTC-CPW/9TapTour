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

        // BASIS_SCORE used to calculate handicap, 
        // usually slightly higher than bowler with highest average in the league 
        const int BASIS_SCORE = 220;

        // BASIS_SCORE_PERCENTAGE
        // used to calculate handicap, amount of difference to be used
        const int BASIS_SCORE_PERCENTAGE = 90;

       

        /// <summary>
        /// Calculates handicap pins:
        /// 90% of the difference between 220 and a bowler's 9 tap tour average. 
        /// or the maximum of 70 pins, whichever is lowest.
        /// </summary>
        /// <param name="currentAverage"></param>
        /// <returns>Number of calculated Handicap Pins</returns>
        public static int CalculateHandicapPins(int currentAverage)
        {
            /// 90% is the current BASIS_SCORE_PERCENTAGE, 220 is BASIS_SCORE
            /// both available for easy modification
            // int division ensures any fractional handicap is thrown out
            int averageBasedHandicapPins= ( (BASIS_SCORE - currentAverage) *  BASIS_SCORE_PERCENTAGE / 100 );

            int lowestHandicap = Math.Min(MAX_HANDICAP_PINS, averageBasedHandicapPins);
                 
            return lowestHandicap;
        }

        /// <summary>
        /// Returns the adjusted bonus pins after a tournament depending on if a bowler placed
        /// and what ranking a bowler placed.
        /// </summary>
        /// <param name="memberPlacement">Ranking a bowler placed. 0 if not placed</param>
        /// <param name="totalEntries">Total entries for the tournament by all members</param>
        /// <param name="compEntries">Entries that do not have to pay entry fee</param>
        /// <param name="currentBonusPins">Bonus pins the participant had before this tournament</param>
        /// <param name="memNum">Member number that used to identify bowler by user</param>
        /// <param name="RegionID">RegionId from where the tournament is played</param>
        /// <param name="currTournamentId">Id of the current tournament</param>
        /// <returns>Adjusted bonus pins after current tournament</returns>
        public static int GetAdjustedBonusPins(byte memberPlacement, int totalEntries, int compEntries, int currentBonusPins, 
                                                int memNum, int RegionID, int currTournamentId)
        {
            int lowestPlacementToCash = GetQtyOfMembersThatCanPlace(totalEntries, compEntries);

            if (memberPlacement <= lowestPlacementToCash)
            {
                return  DeductFromBonusPins(memberPlacement, currentBonusPins);
            }

            // Gets the amount of entries the member has for the tournament
            int membersGameEntryCount = FinalizeTempDB.GetMembersGameEntryCount(currTournamentId, memNum);
            List<PlayerHistory> latestGames = PlayerHistoryDB.GetLastQtyGamesMoneyWon(memNum, RegionID, 15);

            return AddToBonusPins(currentBonusPins, latestGames, membersGameEntryCount);
        }


        /// <summary>
        /// Adds to bonus pins if necessary and returns the new total bonus pins for a member
        /// </summary>
        /// <param name="currentBonusPins">Bonus pins before calculating new bonus pins</param>
        /// <param name="latestGames">a member's player history</param>
        /// <param name="currTourneyEntryCount">this is the number of losses in the current game</param>
        /// <returns></returns>
        public static int AddToBonusPins(int currentBonusPins, List<PlayerHistory> latestGames, int currTourneyEntryCount)
        {
            int additionalBonus = 0; 

            // if has 2 or less losses and no previous games or bonus pins are maxed out
            if (latestGames == null || currentBonusPins == MAX_BONUS_PINS_ALLOWED)
            {
                // additionalBonus = 0; but already initialized to 0
            }

            // if has lost 4 entries this tournament and has 2 losses in history not 
            // yet used for gaining a bonus pin
            else if (currTourneyEntryCount == 4 && DoesGetBonus(latestGames, currTourneyEntryCount, 6))
            {
                additionalBonus = 2;
            }

            // if has at least 3 losses in latestGames or 3 losses in current tournament
            else if (DoesGetBonus(latestGames, currTourneyEntryCount, 3) || currTourneyEntryCount >= 3)
            {
                additionalBonus = 1;
            }

            additionalBonus = ValidateAdditionalBonus(currentBonusPins, additionalBonus);

            return currentBonusPins + additionalBonus;
        }


        public static int ValidateAdditionalBonus(int currentBonusPins, int additionalBonus)
        {
            // Max bonus pins is 5
            // Only case would be 4 bonus to start and then 2 to be added
            if (currentBonusPins + additionalBonus > MAX_BONUS_PINS_ALLOWED)
            {
                additionalBonus -= 1;
            }
            return additionalBonus; 
        }

        /// <summary>
        /// Determines if a player's game history when added to current game losses 
        /// qualifies them to get bonus pins. Used in AddToBonusPins method. 
        /// </summary>
        /// <param name="latestGames">a member's player history</param>
        /// <param name="currTourneyEntryCount">this is the number of losses in the current game</param>
        /// <param name="minLosses">minimum number of losses to determine if bonus is earned</param>
        /// <returns></returns>
        private static bool DoesGetBonus(List<PlayerHistory> latestGames, int currTourneyEntryCount, int minLosses)
        {
            // find first index of a tournament with a cashed game
            int lastCashedTourneyIndex = FindLastCashedTourneyIndex(latestGames);

            if (lastCashedTourneyIndex == -1)
            {
                // did not lose any of the latest games with a 3rd loss in a row
                return latestGames.Count % 3 + currTourneyEntryCount >= minLosses;

            }

            // is the multiple of a 3rd loss in a row after a win
            return lastCashedTourneyIndex % 3 + currTourneyEntryCount >= minLosses;
        }

        /// <summary>
        /// Finds the first index of the last tournament where the player cashed. Returns -1 if not found.
        /// </summary>
        /// <param name="latestGames">Games to find last cashed tourney in</param>
        /// <returns>First index of last cashed tourney. -1 if not found</returns>
        private static int FindLastCashedTourneyIndex(List<PlayerHistory> latestGames)
        {
            for (int i = 0; i < latestGames.Count; i++)
            {
                if (PlayerDidCash(latestGames[i]))
                {

                    // move to first index of winning tournament where member cashed
                    while (i - 1 >= 0 && latestGames[i].TournamentDate == latestGames[i - 1].TournamentDate)
                    {
                        i--;
                    }
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// If a PlayerHistory's MoneyWon is greater than 0 than this method returns true
        /// </summary>
        /// <param name="playerHistory"></param>
        /// <returns>true if MoneyWon is greater than 0</returns>
        private static bool PlayerDidCash(PlayerHistory playerHistory)
        {
            return playerHistory.MoneyWon > 0;
        }

        public static int DeductFromBonusPins(int memberPlaced, int currentBonusPins)
        {
            int bonusPinsAfterDeduction = currentBonusPins;

            if (memberPlaced == FIRST_PLACE)
            {
                bonusPinsAfterDeduction -= currentBonusPins;
            }
            else if (memberPlaced <= MAX_PLACEMENT_DEDUCT_3_PINS)
            {
                bonusPinsAfterDeduction -= DEDUCT_3;
            }
            else if (memberPlaced <= MAX_PLACEMENT_DEDUCT_2_PINS)
            {
                bonusPinsAfterDeduction -= DEDUCT_2;
            }
            else
            {
                bonusPinsAfterDeduction -= DEDUCT_1;
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
        /// Calculate place standings of bowlers. Ties between bowlers result in the same placestanding. Duplicate entries are removed.
        /// returned list is sorted by highest score
        /// </summary>
        /// <param name="tempListOfMemberScores">list of MemberScores sorted by Score with placestanding, without duplicate members</param>
        /// 
        //My Change is renaming temp to tempListOfMemberScores
        public static List<MemberScores> CalculatePlaceStandings(List<MemberScores> tempListOfMemberScores)
        {
            if (tempListOfMemberScores.Count == 0)
            {
                return tempListOfMemberScores;
            }

            // Makes copy so original list won't be affected
            tempListOfMemberScores = tempListOfMemberScores.ToList();

            //remove duplicates
            RemoveDuplicateBowlers(tempListOfMemberScores);

            //ensure bowlers are sorted by score
            tempListOfMemberScores.Sort(new MemberScoresComparer());

            int place = 1;
            tempListOfMemberScores[0].placing = place++;

            for (int currPosition = 1; currPosition < tempListOfMemberScores.Count; currPosition++)
            {
                if (tempListOfMemberScores[currPosition].Score == tempListOfMemberScores[currPosition - 1].Score)
                {
                    tempListOfMemberScores[currPosition].placing = tempListOfMemberScores[currPosition - 1].placing;
                }
                else
                {
                    tempListOfMemberScores[currPosition].placing = place;
                }
                place++;
            }

            return tempListOfMemberScores;
        }   

        /// <summary>
        /// Calculates place standings of bowlers and returns a Dictionary of placestandings mapped to
        /// a bowlers FinalizeTemp. Sorted by Placement order but with 0s (duplicate entries) last.
        /// </summary>
        /// <param name="members"></param>
        /// <returns>Dictionary of FinalizeTemps and ints where ints are placings. Sorted by highest score with duplicate members last</returns>
        public static Dictionary<FinalizeTemp, int> CalculatePlaceStandings(List<FinalizeTemp> members)
        {
            if (members.Count == 0)
            {
                return new Dictionary<FinalizeTemp, int>();
            }

            // original members list won't be affected
            members = members.ToList();

            // Sort the list by the total score, including handicap, in descending order.
            members.Sort((a, b) => b.HandicapTotal.CompareTo(a.HandicapTotal));

            // only non duplicates used for placing
            List<FinalizeTemp> removals = RemoveDuplicateBowlers(members);


            // links FinalizeTemp to an integer used for placing
            var membersPlacingMap = new Dictionary<FinalizeTemp, int>();
            foreach (var member in members)
            {
                membersPlacingMap.Add(member, 0);
            }

            int place = 1;
            membersPlacingMap[members[0]] = place++;

            // Calculate each members placing
            for (int currPosition = 1; currPosition < members.Count; currPosition++)
            {
                FinalizeTemp currMember = members[currPosition];
                FinalizeTemp prevMember = members[currPosition - 1];

                if (currMember.HandicapTotal == prevMember.HandicapTotal)
                {
                    membersPlacingMap[currMember] = membersPlacingMap[prevMember];
                }
                else
                {
                    membersPlacingMap[currMember] = place;
                }
                place++;
            }

            // Add duplicate entries to end of list
            foreach(var member in removals)
            {
                membersPlacingMap.Add(member, 0);
            }

            return membersPlacingMap;
        }

        /// <summary>
        /// Removes duplicate members by lowestHandicap based on memberNumber. 
        /// </summary>
        /// <param name="members"></param>
        /// <returns>list of removed FinalizeTemps</returns>
        private static List<FinalizeTemp> RemoveDuplicateBowlers(List<FinalizeTemp> members)
        {
            List<FinalizeTemp> removal = new List<FinalizeTemp>();
            for (int i = 0; i < members.Count; i++)
            {
                bool isCurrIndexRemoved = false;
                for (int j = i + 1; j < members.Count; j++)
                {
                    if (members[i].MemberNumber == members[j].MemberNumber)
                    {
                        if (members[i].HandicapTotal >= members[j].HandicapTotal)
                            removal.Add(members[j]);
                        else
                        {
                            removal.Add(members[i]);
                            isCurrIndexRemoved = true;
                        }
                    }
                }

                foreach (FinalizeTemp deleteMember in removal)
                {
                    members.Remove(deleteMember);
                }

                if (isCurrIndexRemoved)
                {
                    i--;
                }
            }
            return removal;
        }

        /// <summary>
        /// Calculate place standings of bowlers. Ties between bowlers result in the same placestanding
        /// </summary>
        /// <param name="members"></param>
        /// <returns>List of ExcelMembers with duplicate members removed ordered by TotalScore with assigned placement</returns>
        public static List<ExcelMember> CalculatePlaceStandings(List<ExcelMember> members)
        {
            if (members.Count == 0)
            {
                return members;
            }

            // Makes copy so original list won't be affected
            members = members.ToList();

            //remove duplicates
            RemoveDuplicateBowlers(members);

            //ensure bowlers are sorted by score
            members.Sort((x, y) => y.TotalScore.CompareTo(x.TotalScore));

            int place = 1;
            members[0].PlaceStanding = place++;

            for (int currPosition = 1; currPosition < members.Count; currPosition++)
            {
                if (members[currPosition].TotalScore == members[currPosition - 1].TotalScore)
                {
                    members[currPosition].PlaceStanding = members[currPosition - 1].PlaceStanding;
                }
                else
                {
                    members[currPosition].PlaceStanding = place;
                }
                place++;
            }

            return members;
        }

        /// <summary>
        /// Removes the lower scores of duplicate bowlers by MemberId
        /// </summary>
        /// <param name="temp"></param>
        private static void RemoveDuplicateBowlers(List<MemberScores> temp)
        {
            List<MemberScores> removal = new List<MemberScores>();
            for (int i = 0; i < temp.Count; i++)
            {
                bool isCurrIndexRemoved = false;
                for (int j = i + 1; j < temp.Count; j++)
                {
                    if(temp[i].MemberId == temp[j].MemberId)
                    {
                        if (temp[i].Score >= temp[j].Score)
                            removal.Add(temp[j]);
                        else
                        {
                            removal.Add(temp[i]);
                            isCurrIndexRemoved = true;
                        }
                    }
                }

                foreach (MemberScores deleteMember in removal)
                {
                    temp.Remove(deleteMember);
                }

                if (isCurrIndexRemoved)
                {
                    i--;
                }
            }
        }

        /// <summary>
        /// Finds all duplicate bowlers and removes all duplicates that aren't that bowler's
        /// highest score
        /// </summary>
        /// <param name="members"></param>
        private static void RemoveDuplicateBowlers(List<ExcelMember> members)
        {
            List<ExcelMember> removal = new List<ExcelMember>();
            for (int i = 0; i < members.Count; i++)
            {
                bool isCurrIndexRemoved = false;
                for (int j = i + 1; j < members.Count; j++)
                {
                    if (members[i].MemberNumber == members[j].MemberNumber)
                    {
                        if (members[i].TotalScore >= members[j].TotalScore)
                            removal.Add(members[j]);
                        else
                        {
                            removal.Add(members[i]);
                            isCurrIndexRemoved = true;
                        }
                    }
                }

                foreach (ExcelMember deleteMember in removal)
                {
                    members.Remove(deleteMember);
                }

                // prevents from skipping over current index
                if (isCurrIndexRemoved)
                {
                    i--;
                }
            }
        }

        /// <summary>
        /// Makes a list of Members ordered by placement, keeping only the highest score for each member. Only players that
        /// can cash in the tournament are included in the new list.
        /// </summary>
        /// <param name="members">list of members to copy and process</param>
        /// <param name="totalEntries">total amount of tournament entries</param>
        /// <param name="compEntries">comp entry amount in a tournament</param>
        /// <returns>List of ExcelMembers ordered by placement without duplicate ExcelMembers MemberNumbers to the placement that should cash</returns>
        public static List<ExcelMember> MakeTopMembersByPlacementList(List<ExcelMember> members, int totalEntries, int compEntries)
        {
            return MakeTopMembersByPlacementList(members, GetQtyOfMembersThatCanPlace(totalEntries, compEntries));
        }


        /// <summary>
        /// Makes a list of Members ordered by placement, keeping only the highest score for each member. Players
        /// below the lowestPlacement (1st is highest) threshold are not included in the new list.
        /// </summary>
        /// <param name="members">list of members to copy and process</param>
        /// <param name="lowestPlacement">The lowest placement to accept (1st is highest)</param>
        /// <returns>List of ExcelMembers ordered by placement without duplicate ExcelMembers MemberNumbers to the lowest selected placement</returns>
        public static List<ExcelMember> MakeTopMembersByPlacementList(List<ExcelMember> members, int lowestPlacement)
        {
            members = CalculatePlaceStandings(members);

            // takes only top place members above or at lowest placement threshold
            return members.Where(m => m.PlaceStanding <= lowestPlacement).ToList();
        }

        /// <summary>
        /// Makes a list of Members ordered by placement, keeping only the highest score for each member. Players
        /// below the lowestPlacement (1st is highest) threshold are not included in the new list.
        /// </summary>
        /// <param name="members">list of members to copy and process</param>
        /// <param name="lowestPlacement">The lowest placement to accept (1st is highest)</param>
        /// <returns>List of MemberScores ordered by placement without duplicate MemberScore MemberIds to the lowest selected placement</returns>
        public static List<MemberScores> MakeTopMembersByPlacementList(List<MemberScores> members, int lowestPlacement)
        {
            members = CalculatePlaceStandings(members);

            // takes only top place members above lowest placement threshold
            return members.Where(m => m.placing <= lowestPlacement).ToList();
        }
    }

    /// <summary>
    /// Sorts member scores in descending order
    /// </summary>
    public class MemberScoresComparer : IComparer<MemberScores>
    {
        int IComparer<MemberScores>.Compare(MemberScores x, MemberScores y)
        {
            int score1 = x.Score.HasValue ? (int)x.Score : 0;
            int score2 = y.Score.HasValue ? (int)y.Score : 0;
            return score2.CompareTo(score1);
        }
    }
}