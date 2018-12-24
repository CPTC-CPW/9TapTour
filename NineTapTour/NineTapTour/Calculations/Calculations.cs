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
        /// <param name="memberPlacement">Ranking a bowler placed. 0 if not placed</param>
        /// <param name="totalEntries">Total entries for the tournament</param>
        /// <param name="compEntries">Entries that do not have to pay entry fee</param>
        /// <param name="currentBonusPins">Bonus pins the participant had before this tournament</param>
        /// <param name="memNum">Member number that used to identify bowler by user</param>
        /// <param name="RegionID">RegionId from where the tournament is played</param>
        /// <param name="currTournamentDate">Date when the current tournament is taking place</param>
        /// <returns>Adjusted bonus pins after current tournament</returns>
        public static int GetAdjustedBonusPins(byte memberPlacement, int totalEntries, int compEntries, int currentBonusPins, 
                                                int memNum, int RegionID, DateTime currTournamentDate)
        {
            int lowestPlacementToCash = GetQtyOfMembersThatCanPlace(totalEntries, compEntries);

            if (memberPlacement <= lowestPlacementToCash)
            {
                return  DeductFromBonusPins(memberPlacement, currentBonusPins);
            }
            return AddToBonusPins(currentBonusPins, currTournamentDate, PlayerHistoryDB.GetLastEightTournaments(memNum, RegionID));
        }

        /// <summary>
        /// Adds to bonus pins if necessary and returns the new total bonus pins for a member
        /// </summary>
        /// <param name="currentBonusPins">Bonus pins before calculating new bonus pins</param>
        /// <param name="memberNum">Member number for the member to calculate bonus pins</param>
        /// <param name="RegionID">Region the current tournament is taking place</param>
        /// <param name="currTournamentDate">The date the tournament took place</param>
        /// <param name="latestGames">The last two distinct tournaments</param>
        /// <returns></returns>
        public static int AddToBonusPins(int currentBonusPins, DateTime currTournamentDate, List<PlayerHistory> latestGames)
        {
            if (latestGames == null || latestGames.Count < 2 || currentBonusPins == MAX_BONUS_PINS_ALLOWED)
            {
                return currentBonusPins;
            }

            PlayerHistory lastTourney = latestGames[0];

            if (PlayerDidCash(lastTourney))
            {
                return currentBonusPins;
            }

            // if won the last tournament on a different squad
            int i = 1;
            while (i < latestGames.Count && lastTourney.TournamentDate == latestGames[i].TournamentDate)
            { 
                if (PlayerDidCash(latestGames[i]))
                {
                    return currentBonusPins;
                }
                i++;
            }

            PlayerHistory secondToLast = latestGames[i];

            // if a second to last tournament doesn't exist or player cashed
            if (secondToLast == null || PlayerDidCash(secondToLast))
            {
                return currentBonusPins;
            }
            i++;

            // if won the second to last tournament on a different squad
            while (i < latestGames.Count && secondToLast.TournamentDate == latestGames[i].TournamentDate)
            {
                if (PlayerDidCash(latestGames[i]))
                {
                    return currentBonusPins;
                }
                i++;
            }

            // Add one if did not cash last 3 tournaments including the current
            return currentBonusPins + 1;
        }

        private static bool PlayerDidCash(PlayerHistory playerHistory)
        {
            return playerHistory.MoneyWon > 0;
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
        /// Calculate place standings of bowlers. Ties between bowlers result in the same placestanding. Duplicate entries are removed.
        /// returned list is sorted by highest score
        /// </summary>
        /// <param name="temp">list of MemberScores sorted by Score with placestanding, without duplicate members</param>
        public static List<MemberScores> CalculatePlaceStandings(List<MemberScores> temp)
        {
            if (temp.Count == 0)
            {
                return temp;
            }

            // Makes copy so original list won't be affected
            temp = temp.ToList();

            //remove duplicates
            RemoveDuplicateBowlers(temp);

            //ensure bowlers are sorted by score
            temp.Sort(new MemberScoresComparer());

            int place = 1;
            temp[0].placing = place++;
            for (int currPosition = 1; currPosition < temp.Count; currPosition++)
            {
                if (temp[currPosition].Score == temp[currPosition - 1].Score)
                {
                    temp[currPosition].placing = temp[currPosition - 1].placing;
                }
                else
                {
                    temp[currPosition].placing = place;
                }
                place++;
            }

            return temp;
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
                for (int j = i + 1; j < members.Count; j++)
                {
                    if (members[i].memberNumber == members[j].memberNumber)
                    {
                        if (members[i].HandicapTotal >= members[j].HandicapTotal)
                            removal.Add(members[j]);
                        else
                            removal.Add(members[i]);
                    }
                }

                foreach (FinalizeTemp deleteMember in removal)
                {
                    members.Remove(deleteMember);
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
                for (int j = i + 1; j < members.Count; j++)
                {
                    if (members[i].MemberNumber == members[j].MemberNumber)
                    {
                        if (members[i].TotalScore >= members[j].TotalScore)
                            removal.Add(members[j]);
                        else
                            removal.Add(members[i]);
                    }
                }

                foreach (ExcelMember deleteMember in removal)
                {
                    members.Remove(deleteMember);
                }
            }
        }

        /// <summary>
        /// Makes a list of Members ordered by placement, keeping only the highest score for each member. Only players that
        /// can cash in the tournament are included in the new list.
        /// </summary>
        /// <param name="members">list of members to copy and process</param>
        /// <param name="totalEntries">total amount of tournament entries</param>
        /// <param name="compEntries">comp entry amoun in a tournament</param>
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