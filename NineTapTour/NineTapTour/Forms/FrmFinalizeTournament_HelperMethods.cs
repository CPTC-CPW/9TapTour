using NineTapTour.Database;
using NineTapTour.Models;
using System;
using System.Windows.Forms;

namespace NineTapTour.Forms
{
    public partial class FrmFinalizeTournament
    {
        /// <summary>
        /// This function takes the averages from the current games being finalized, grabs the appropriate amount of player history, and calculates the 30 game average for the player.
        /// </summary>
        /// <param name="memberNum">The Member Number of the player whose averages we are calculating.</param>
        /// <returns></returns>
        private double CalcThirtyLeagueAverage(int memberNum)
        {
            return FinalizeTempDB.GetLeagueAverage(memberNum, RegionID, currTournament.Id);            
        }

        /// <summary>
        /// Gets the row of data grid by the Game Id value stored in that row.
        /// Returns -1 if not found.
        /// </summary>
        /// <param name="currGameId"></param>
        /// <returns>The row index of the Game Id</returns>
        private int FindDataGridRowIndex(int currGameId)
        {
            foreach (DataGridViewRow row in TournamentEntriesGrid.Rows)
            {
                DataGridViewCell gameIdCell = row.Cells[GAME_ID_COLUMN];
                if (Convert.ToInt32(gameIdCell.Value) == currGameId)
                {
                    return gameIdCell.RowIndex;
                }
            }
            return -1;
        }

        /// <summary>
        /// Adjusts bonus pins for Member and PlayerHistory parameters if Game is not in PlayerHistory
        /// </summary>
        /// <param name="totalEntriesQty">Total entries for the tournament</param>
        /// <param name="compEntriesQty">Total comp entries for the tournament</param>
        /// <param name="ph">PlayerHistory to adjust bonus pins</param>
        /// <param name="currGame">Current Game of tournament</param>
        /// <param name="currMember">Current Member to adjust bonus pins of</param>
        /// <param name="placeStanding">Placestanding in tournament for current member's entry</param>
        private void AdjustBonusPins(int totalEntriesQty, int compEntriesQty, PlayerHistory ph, Game currGame, Member currMember, int placeStanding)
        {
            // Adjust bonus pins only if game has not been finalized previously
            if (!PlayerHistoryDB.PlayerHistoryExists(currGame.Id))
            {
                currMember.Bonus = Calculations.Calculations.GetAdjustedBonusPins(placeStanding, totalEntriesQty, compEntriesQty,
                                                                            currMember.Bonus, currMember.Number, RegionID, currTournament.Id);
                ph.Bonus = currMember.Bonus;
            }
        }
    }
}
