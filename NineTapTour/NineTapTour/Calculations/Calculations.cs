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

        /* 
         * With the current rules posed on 9Tap's website,
         * If you win money and are in the place standings: 
         */

        // 1st place: All bonus pins removed
        const int FIRST_PLACE = 1;

        // 2ed-5th place: 3 bonus pins removed
        const int MAX_PLACEMENT_DEDUCT_3_PINS = 5;
        const int DEDUCT_3 = 3;

        // 6th-10th place: 2 bonus pins removed
        const int MAX_PLACEMENT_DEDUCT_2_PINS = 10;
        const int DEDUCT_2 = 2;

        // 11th+ place: 1 bonus pin removed
        const int DEDUCT_1 = 1;

        // Members may not have more then 5 bonus pins
        // And no less then 0
        const int MAX_BONUS_PINS_ALLOWED = 5;
        const int MIN_BONUS_PINS_ALLOWED = 0;

        // Members may not have more then 70 Handicap pins
        const int MAX_HANDICAP_PINS = 70;

        // Handicap pins are calculated as
        // 90% of 220 minus the average
        const int BASIS_SCORE = 220;
        const int BASIS_SCORE_PERCENTAGE = 90;

        /// <summary>
        /// Calculates and returns the members handicap pins amount
        /// based on their average
        /// </summary>
        public static int CalculateHandicapPins(int currentAverage)
        {
            // Handicap pins are calculated as 90% of 220 minus the average
            // Int is used for this caluclation to remove all decimal points
            int averageBasedHandicapPins = (
                (BASIS_SCORE - currentAverage) *
                (BASIS_SCORE_PERCENTAGE / 100));

            // Member cannot have more then 70 handicap pins
            return Math.Min(MAX_HANDICAP_PINS, averageBasedHandicapPins);
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
        public static int GetAdjustedBonusPins(int memberPlacement, int totalEntries, int compEntries, int currentBonusPins, 
                                                int memNum, int RegionID, int currTournamentId)
        {
            // Calculates if the member is on the place standing
            int lowestPlacementToCash = GetQtyOfMembersThatCanPlace(totalEntries, compEntries);

            // If player won money and are in the place standing, bonus pins are reduced
            if (memberPlacement <= lowestPlacementToCash)
                return DeductFromBonusPins(memberPlacement, currentBonusPins);

            // Gets the amount of entries the member has for the tournament
            int membersGameEntryCount = FinalizeTempDB.GetMembersGameEntryCount(currTournamentId, memNum);
            List<PlayerHistory> latestGames = PlayerHistoryDB.GetLastQtyGamesMoneyWon(memNum, RegionID, 15);

            // If a player didnt win money, they might gain bonus pins
            return AddToBonusPins(currentBonusPins, latestGames, membersGameEntryCount);
        }


        /// <summary>
        /// Adds to bonus pins if necessary and returns the new total bonus pins for a member
        /// </summary>
        /// <param name="currentBonusPins">Bonus pins before calculating new bonus pins</param>
        /// <param name="latestGames">a member's player history</param>
        /// <param name="currTourneyEntryCount">this is the number of losses in the current game</param>
        public static int AddToBonusPins(int currentBonusPins, List<PlayerHistory> latestGames, int currTourneyEntryCount)
        {
            int additionalBonus = 0;

            // If a player does not have a latest game,
            // Or has the max amount of bonus pins already, that player will not gain any more pins
            if (latestGames == null || currentBonusPins == MAX_BONUS_PINS_ALLOWED)
                return currentBonusPins;

            // if has lost 4 entries this tournament and has 2 losses in history not 
            // yet used for gaining a bonus pin
            else if (currTourneyEntryCount == 4 && DoesGetBonus(latestGames, currTourneyEntryCount, 6))
                additionalBonus = 2;

            // if has at least 3 losses in latestGames or 3 losses in current tournament
            else if (DoesGetBonus(latestGames, currTourneyEntryCount, 3) || currTourneyEntryCount >= 3)
                additionalBonus = 1;

            int newBonusPins = currentBonusPins + additionalBonus;
            return ValidateBonusPins(newBonusPins);
        }

        /// <summary>
        /// Calculates the amount of bonus pins the member would have after scoring 
        /// in the place given if they won money
        /// </summary>
        /// <param name="memberPlaced">The position the member scored in</param>
        /// <param name="currentBonusPins">That member's current bonus pins</param>
        /// <returns>That members new number of bonus pins</returns>
        public static int DeductFromBonusPins(int memberPlaced, int currentBonusPins)
        {
            int newBonusPinAmount = currentBonusPins;

            // 1st place, All bonus pins removed
            if (memberPlaced == FIRST_PLACE)
                newBonusPinAmount = MIN_BONUS_PINS_ALLOWED;

            // 2ed-5th place, 3 bonus pins removed
            else if (memberPlaced <= MAX_PLACEMENT_DEDUCT_3_PINS)
                newBonusPinAmount -= DEDUCT_3;

            // 6th-10th place, 2 bonus pins removed
            else if (memberPlaced <= MAX_PLACEMENT_DEDUCT_2_PINS)
                newBonusPinAmount -= DEDUCT_2;

            // 11th+ place, 1 bonus pin removed
            else newBonusPinAmount -= DEDUCT_1;

            // Checks if bonus pins are less then 0 before returning;
            return ValidateBonusPins(newBonusPinAmount);
        }

        /// <summary>
        /// Checks if bonus pins is within the upper and lower limits
        /// </summary>
        /// <returns>
        /// The new amount of bonus pins that is within the bonus pins limits,
        /// or returns the original bonus pins if nothing is wrong with it
        /// </returns>
        public static int ValidateBonusPins(int bonusPins)
        {
            // Bonus pins cannot be less then 0
            if (bonusPins < MIN_BONUS_PINS_ALLOWED)
                return MIN_BONUS_PINS_ALLOWED;

            // Bonus pins cannot be greater then 5
            else if (bonusPins > MAX_BONUS_PINS_ALLOWED)
                return MAX_BONUS_PINS_ALLOWED;

            return bonusPins;
        }

        /// <summary>
        /// Determines if a player's game history when added to current game losses 
        /// qualifies them to get bonus pins. Used in AddToBonusPins method. 
        /// </summary>
        /// <param name="latestGames">a member's player history</param>
        /// <param name="currTourneyEntryCount">this is the number of losses in the current game</param>
        /// <param name="minLosses">minimum number of losses to determine if bonus is earned</param>
        private static bool DoesGetBonus(List<PlayerHistory> latestGames, int currTourneyEntryCount, int minLosses)
        {
            // find first index of a tournament with a cashed game
            int lastCashedTourneyIndex = FindLastCashedTourneyIndex(latestGames);

            // did not lose any of the latest games with a 3rd loss in a row
            if (lastCashedTourneyIndex == -1)
                return latestGames.Count % 3 + currTourneyEntryCount >= minLosses;

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
                    // Move to first index of winning tournament where member cashed
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
        /// Returns true if the player won money, returns false otherwise
        /// </summary>
        private static bool PlayerDidCash(PlayerHistory playerHistory)
        {
            return playerHistory.MoneyWon > 0;
        }

        /// <summary>
        /// Returns the number of members that can place in the tournament
        /// </summary>
        /// <param name="totalEntries">all tournament participants including comp entries</param>
        /// <param name="compEntries">tournament participants that also work at tournament</param>
        /// <returns>
        /// The quantity of members that can place in a tournament, 
        /// which is also the max placing in the tournament
        /// </returns>
        public static int GetQtyOfMembersThatCanPlace(int totalEntries, int compEntries)
        {
            // Only 1/5th of the members can place
            return (totalEntries - compEntries) / 5;
        }

        /// <summary>
        /// Calculate place standings of bowlers. Ties between bowlers result in the same placestanding. Duplicate entries are removed.
        /// returned list is sorted by highest score
        /// </summary>
        /// <param name="tempListOfMemberScores">list of MemberScores sorted by Score with placestanding, without duplicate members</param>
        public static List<MemberScores> CalculatePlaceStandings(List<MemberScores> tempListOfMemberScores)
        {
            // A list of no members needs nothing done to it
            if (tempListOfMemberScores.Count == 0)
                return tempListOfMemberScores;

            // Makes copy so original list won't be affected
            tempListOfMemberScores = tempListOfMemberScores.ToList();

            // Removes duplicate members
            RemoveDuplicateBowlers(tempListOfMemberScores);

            // The first member ordered by score will will always score 1st place
            int place = 1;
            tempListOfMemberScores.Sort(new MemberScoresComparer());
            tempListOfMemberScores[0].placing = place++;

            // Gives each member a place standing
            for (int currPosition = 1; currPosition < tempListOfMemberScores.Count; currPosition++)
            {
                // Members with identical scores will tie
                if (tempListOfMemberScores[currPosition].Score == tempListOfMemberScores[currPosition - 1].Score)
                    tempListOfMemberScores[currPosition].placing = tempListOfMemberScores[currPosition - 1].placing;

                // Otherwise, each member gets a unique placing
                else
                    tempListOfMemberScores[currPosition].placing = place;

                // Place is still counted if members tie
                place++;
            }

            // Returns the edited list of members
            return tempListOfMemberScores;
        }

        /// <summary>
        /// Calculates place standings of bowlers and returns a Dictionary of placestandings mapped to
        /// a bowlers FinalizeTemp. Sorted by Placement order but with 0s (duplicate entries) last.
        /// </summary>
        /// <param name="members"></param>
        /// <returns>Dictionary of FinalizeTemps and ints where ints are placings. Sorted by highest score with duplicate members last</returns>
        public static Dictionary<FinalizeTemp, int> CalculatePlaceStandings(List<FinalizeTemp> members, Tournament tournament)
        {
            // A list of no members will return an empty list
            if (members.Count == 0)
                return new Dictionary<FinalizeTemp, int>();

            // Makes copy so original list won't be affected
            members = members.ToList();

            // Sort the list by the total score, including handicap, in descending order.
            members.Sort((a, b) => b.HandicapTotal.CompareTo(a.HandicapTotal));

            // Removes duplicate members
            List<FinalizeTemp> removals = RemoveDuplicateBowlers(members);


            // links FinalizeTemp to an integer used for placing
            var membersPlacingMap = new Dictionary<FinalizeTemp, int>();
            foreach (var member in members)
            {
                membersPlacingMap.Add(member, 0);
            }

            int place = 1;
            membersPlacingMap[members[0]] = place++;

            if (tournament.ThreeOutOf4)
            {
                //The variable to tell AlterHandicapTotalAccordingToMinimumGameScore to subtract the lowest scored game from handicaptotal
                bool isPositive = false;
                //subtracts the lowest scored game from handicap total so that the rankings are calculated correctly for a 
                AlterHandicapTotalAccordingToMinimumGameScore(members, isPositive);
            }

            // Calculate each member's placing
            for (int currPosition = 1; currPosition < members.Count; currPosition++)
            {
                FinalizeTemp currMember = members[currPosition];
                FinalizeTemp prevMember = members[currPosition - 1];

                // Tied scores will have the same place standing
                if (currMember.HandicapTotal == prevMember.HandicapTotal)
                    membersPlacingMap[currMember] = membersPlacingMap[prevMember];

                // Otherwise members get a unique placing
                else
                    membersPlacingMap[currMember] = place;

                place++;
            }

            // Add duplicate entries to end of list
            foreach (FinalizeTemp member in removals)
            {
                membersPlacingMap.Add(member, 0);
            }

            if (tournament.ThreeOutOf4)
            {
                //The variable to tell AlterHandicapTotalAccordingToMinimumGameScore to add the lowest scored game to handicaptotal
                bool isPositive = true;
                //adds the lowest scored game back to handicap total so that the handicap total is calculated with all four of the games still.
                AlterHandicapTotalAccordingToMinimumGameScore(members, isPositive);
            }

            return membersPlacingMap;
        }

        /// <summary>
        /// Either substracts or adds to the handicap total by the lowest scored game if the tournament is a three out of four tournament.
        /// </summary>
        /// <param name="members">The list of bowlers</param>
        /// <param name="isPositive">Wether or not to add or substract from handicap total</param>
        private static void AlterHandicapTotalAccordingToMinimumGameScore(List<FinalizeTemp> members, bool isPositive)
        {
            foreach (FinalizeTemp currMember in members)
            {
                //puts the four games of the current member into a list so that the minimum value can be found easier.
                List<int?> games = new List<int?>()
                {
                    currMember.Game1,
                    currMember.Game2,
                    currMember.Game3,
                    currMember.Game4
                };

                // If positive, the lowest scored game is added to the handicap total. All handicaps and bonuses are accounted for.
                if (isPositive)
                    currMember.HandicapTotal = currMember.HandicapTotal + (games.Min().Value + currMember.Handicap + currMember.Bonus);

                // If not positive, the lowest scored game is subtracted from the handicap total. All handicaps and bonuses are accounted for.
                else
                    currMember.HandicapTotal = currMember.HandicapTotal - (games.Min().Value + currMember.Handicap + currMember.Bonus);
            }
        }

        /// <summary>
        /// Calculate place standings of bowlers. Ties between bowlers result in the same placestanding
        /// </summary>
        /// <param name="members"></param>
        /// <returns>List of ExcelMembers with duplicate members removed ordered by TotalScore with assigned placement</returns>
        public static List<ExcelMember> CalculatePlaceStandings(List<ExcelMember> members)
        {
            // No members, no place standings
            if (members.Count == 0)
                return members;

            // Makes copy so original list won't be affected
            members = members.ToList();

            //remove duplicates
            RemoveDuplicateBowlers(members);

            // The first member in the list when sorted by score will be in first place
            int place = 1;
            members.Sort((x, y) => y.TotalScore.CompareTo(x.TotalScore));
            members[0].PlaceStanding = place++;

            for (int currPosition = 1; currPosition < members.Count; currPosition++)
            {
                // Tied members get the same position
                if (members[currPosition].TotalScore == members[currPosition - 1].TotalScore)
                    members[currPosition].PlaceStanding = members[currPosition - 1].PlaceStanding;

                // Otherwise they get a unique placing
                else
                    members[currPosition].PlaceStanding = place;

                // Placing is still added if members tied
                place++;
            }
            return members;
        }

        /// <summary>
        /// Removes the lower scores of duplicate bowlers in the list
        /// </summary>
        /// <returns>list of removed FinalizeTemps</returns>
        private static List<FinalizeTemp> RemoveDuplicateBowlers(List<FinalizeTemp> members)
        {
            List<FinalizeTemp> removal = new List<FinalizeTemp>();
            for (int i = 0; i < members.Count; i++)
            {
                bool isCurrIndexRemoved = false;
                for (int j = i + 1; j < members.Count; j++)
                {
                    // If any two members have the same number
                    if (members[i].MemberNumber == members[j].MemberNumber)
                    {
                        // Remove the inferior clone
                        if (members[i].HandicapTotal >= members[j].HandicapTotal)
                            removal.Add(members[j]);
                        else
                        {
                            removal.Add(members[i]);
                            isCurrIndexRemoved = true;
                        }
                    }
                }

                // Removes all members who are in removal list
                foreach (FinalizeTemp deleteMember in removal)
                {
                    members.Remove(deleteMember);
                }

                if (isCurrIndexRemoved)
                    i--;
            }
            return removal;
        }

        /// <summary>
        /// Removes the lower scores of duplicate bowlers in the list
        /// </summary>
        private static void RemoveDuplicateBowlers(List<MemberScores> temp)
        {
            List<MemberScores> removal = new List<MemberScores>();
            for (int i = 0; i < temp.Count; i++)
            {
                bool isCurrIndexRemoved = false;
                for (int j = i + 1; j < temp.Count; j++)
                {
                    // If any two members have the same Id
                    if (temp[i].MemberId == temp[j].MemberId)
                    {
                        // Removes the inferior clone
                        if (temp[i].Score >= temp[j].Score)
                            removal.Add(temp[j]);
                        else
                        {
                            removal.Add(temp[i]);
                            isCurrIndexRemoved = true;
                        }
                    }
                }

                // Removes all members that are in the removal list
                foreach (MemberScores deleteMember in removal)
                {
                    temp.Remove(deleteMember);
                }

                if (isCurrIndexRemoved)
                    i--;
            }
        }

        /// <summary>
        /// Removes the lower scores of duplicate bowlers in the list
        /// </summary>
        private static void RemoveDuplicateBowlers(List<ExcelMember> members)
        {
            List<ExcelMember> removal = new List<ExcelMember>();
            for (int i = 0; i < members.Count; i++)
            {
                bool isCurrIndexRemoved = false;
                for (int j = i + 1; j < members.Count; j++)
                {
                    // If any two members have the same Id
                    if (members[i].MemberNumber == members[j].MemberNumber)
                    {
                        // Removes the inferior clone
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
                    i--;
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