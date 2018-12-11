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
        public static int GetAdjustedBonusPins(byte memberPlaced, int currentBonusPins, int memNum, int RegionID, DateTime currTournamentDate)
        {
            if (memberPlaced > 0)
            {
                return  DeductFromBonusPins(memberPlaced, currentBonusPins);
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
        /// <param name="latestTournaments">The last two distinct tournaments</param>
        /// <returns></returns>
        public static int AddToBonusPins(int currentBonusPins, DateTime currTournamentDate, List<PlayerHistory> latestTournaments)
        {
            if (latestTournaments == null || latestTournaments.Count < 2 || currentBonusPins == MAX_BONUS_PINS_ALLOWED)
            {
                return currentBonusPins;
            }

            #region Check for wins as multiple entries and get distinct tournaments by date

            PlayerHistory lastTourney = latestTournaments[0];
            int i = 1;
            while (i < latestTournaments.Count && lastTourney.TournamentDate == latestTournaments[i].TournamentDate)
            {
                // if won the last tournament on a different squad
                if (lastTourney.Bonus != latestTournaments[i].Bonus)
                {
                    return currentBonusPins;
                }
                i++;
            }

            PlayerHistory secondToLast = latestTournaments[i];
            if (secondToLast == null)
            {
                return currentBonusPins;
            }
            
            while (i < latestTournaments.Count && secondToLast.TournamentDate == latestTournaments[i].TournamentDate)
            {
                // if won the second to last tournament on a different squad
                if (lastTourney.Bonus != latestTournaments[i].Bonus)
                {
                    return currentBonusPins;
                }
                i++;
            }
            #endregion

            // After 3 games not placing add a bonus pin
            if (currentBonusPins == lastTourney.Bonus && currentBonusPins == secondToLast.Bonus)
            {
                return currentBonusPins + 1;
            }
            return currentBonusPins;
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
            if (temp.Count == 0)
            {
                return;
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
        }

        /// <summary>
        /// Calculate place standings of bowlers. Ties between bowlers result in the same placestanding
        /// </summary>
        /// <param name="games"></param>
        public static void CalculatePlaceStandings(List<Game> games)
        {
            if (games.Count == 0)
            {
                return;
            }

            // Makes copy so original list won't be affected
            games = games.ToList();

            int compEntries = games.Where(g => g.IsComp).Count();
            int totalEntries = games.Count();

            int lowestPlacement = GetQtyOfMembersThatCanPlace(totalEntries, compEntries);

            //ensure bowlers are sorted by highest scoring game
            games.Sort(new GameComparer());

            byte place = 1;
            games[0].PlaceStanding = place++;
            for (int currPosition = 1; currPosition < games.Count; currPosition++)
            {
                Game currGame = games[currPosition];
                Game prevGame = games[currPosition - 1];

                int currTourneyTotal = GetTourneyTotal(currGame);
                int prevTourneyTotal = GetTourneyTotal(prevGame);

                if (currTourneyTotal == prevTourneyTotal)
                {
                    currGame.PlaceStanding = prevGame.PlaceStanding;
                }
                else if (place <= lowestPlacement)
                {
                    currGame.PlaceStanding = place;
                }
                else // if place > lowestPlacement. Those who don't place get 0s as place standing
                {
                    currGame.PlaceStanding = 0;
                }
                place++;
            }
        }

        /// <summary>
        /// Finds the highest game in a tournament by a player
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private static int GetTourneyTotal(Game game)
        {
            int game1 = game.Game1 ?? 0;
            int game2 = game.Game2 ?? 0;
            int game3 = game.Game3 ?? 0;
            int game4 = game.Game4 ?? 0;

            return game1 + game2 + game3 + game4;
        }

        /// <summary>
        /// Calculate place standings of bowlers. Ties between bowlers result in the same placestanding
        /// </summary>
        /// <param name="members"></param>
        public static void CalculatePlaceStandings(List<ExcelMember> members)
        {
            if (members.Count == 0)
            {
                return;
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
        /// <returns></returns>
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
        /// <returns></returns>
        public static List<ExcelMember> MakeTopMembersByPlacementList(List<ExcelMember> members, int lowestPlacement)
        {
            CalculatePlaceStandings(members);

            // takes only top place members above lowest placement threshold
            return members.Where(m => m.PlaceStanding <= lowestPlacement).ToList();
        }

        /// <summary>
        /// Makes a list of Members ordered by placement, keeping only the highest score for each member. Players
        /// below the lowestPlacement (1st is highest) threshold are not included in the new list.
        /// </summary>
        /// <param name="members">list of members to copy and process</param>
        /// <param name="lowestPlacement">The lowest placement to accept (1st is highest)</param>
        /// <returns></returns>
        public static List<MemberScores> MakeTopMembersByPlacementList(List<MemberScores> members, int lowestPlacement)
        {
            CalculatePlaceStandings(members);

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

    /// <summary>
    /// Sorts Games by highest scored Game in descending order
    /// </summary>
    public class GameComparer : IComparer<Game>
    {
        int IComparer<Game>.Compare(Game x, Game y)
        {
            int xGame1 = x.Game1 ?? 0;
            int xGame2 = x.Game2 ?? 0;
            int xGame3 = x.Game3 ?? 0;
            int xGame4 = x.Game4 ?? 0;

            int yGame1 = y.Game1 ?? 0;
            int yGame2 = y.Game2 ?? 0;
            int yGame3 = y.Game3 ?? 0;
            int yGame4 = y.Game4 ?? 0;

            int xGameTotal = xGame1 + xGame2 + xGame3 + xGame4;
            int yGameTotal = yGame1 + yGame2 + yGame3 + yGame4;

            return yGameTotal.CompareTo(xGameTotal);
        }
    }
}