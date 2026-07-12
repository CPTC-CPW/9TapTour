using System;
using System.ComponentModel;

namespace NineTapTour.Models
{
    /// <summary>
    /// View Model for tournament finalization UI.
    /// Combines data from Game, Member, and Participant entities for efficient DataGridView binding.
    /// This is NOT a database entity - it's a Data Transfer Object (DTO) for UI purposes.
    /// </summary>
    public class GameViewModel
    {
        public int GameId { get; set; }
        public int TournamentID { get; set; }
        public int MemberId { get; set; }
        public int MemberNumber { get; set; }
        public int FinalizeRegionID { get; set; }
        
        // Member Information (denormalized from Member table)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        // Participant Information (from Participant table)
        public int Squad { get; set; }
        
        // Game Scores (from Game table)
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }
        
        // Game Validity Flags (from Game table)
        [DefaultValue(true)]
        public bool UseGame1 { get; set; }
        [DefaultValue(true)]
        public bool UseGame2 { get; set; }
        [DefaultValue(true)]
        public bool UseGame3 { get; set; }
        [DefaultValue(true)]
        public bool UseGame4 { get; set; }
        
        // Finalization Data (from Game table)
        public double LeagueAverage { get; set; }
        public int AdjustedAvg { get; set; }
        [DefaultValue(false)]
        public bool KeepAdjustedAvg { get; set; }
        public int GameAvg { get; set; }
        
        // Calculated Values (from Game table)
        public int ScratchTotal { get; set; }
        public int HandicapTotal { get; set; }
        public int Handicap { get; set; }
        public int Bonus { get; set; }
        
        // Notes
        public string Notes { get; set; }
        
        /// <summary>
        /// Full name of the member (computed for UI display)
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";
    }
}
