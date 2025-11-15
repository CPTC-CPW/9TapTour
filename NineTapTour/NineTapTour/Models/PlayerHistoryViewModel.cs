using System;

namespace NineTapTour.Models
{
    /// <summary>
    /// View Model for player tournament history.
    /// Combines data from Game, Member (via Participant), and Tournament entities.
    /// This is NOT a database entity - it's a Data Transfer Object (DTO) for historical data display.
    /// 
    /// This ViewModel provides the same structure as the legacy PlayerHistory table,
    /// but pulls data from the Games table which is the single source of truth.
    /// </summary>
    public class PlayerHistoryViewModel
    {
        /// <summary>
        /// History ID (maps to GameID for compatibility)
        /// </summary>
        public int hisID { get; set; }

        /// <summary>
        /// Member number (from Member via Participant)
        /// </summary>
        public int MemberNumber { get; set; }

        /// <summary>
        /// Number of games played in this tournament entry
        /// </summary>
        public int GamesPlayed { get; set; }

        /// <summary>
        /// Tournament date (from Tournament via Participant)
        /// </summary>
        public DateTime TournamentDate { get; set; }

        /// <summary>
        /// Game ID reference
        /// </summary>
        public int GameID { get; set; }

        /// <summary>
        /// Individual game scores
        /// </summary>
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }

        /// <summary>
        /// Total scratch score for this entry
        /// </summary>
        public int TotalScore { get; set; }

        /// <summary>
        /// Handicap pins for this entry
        /// </summary>
        public int HandiCap { get; set; }

        /// <summary>
        /// Bonus pins for this entry
        /// </summary>
        public int Bonus { get; set; }

        /// <summary>
        /// Money won in this tournament
        /// </summary>
        public decimal MoneyWon { get; set; }

        /// <summary>
        /// Notes for this game
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Average for this specific entry (game average)
        /// </summary>
        public double AverageForEntry { get; set; }

        /// <summary>
        /// True 30-game league average at time of tournament
        /// </summary>
        public double trueAVG { get; set; }

        /// <summary>
        /// Adjusted average (director-approved)
        /// </summary>
        public int AVG { get; set; }

        /// <summary>
        /// Pro pot / side pot (stored as string for compatibility)
        /// </summary>
        public string ProPot { get; set; }

        /// <summary>
        /// Progressive Post High Game - Place standing (stored as string for compatibility)
        /// </summary>
        public string PPHG { get; set; }

        /// <summary>
        /// Region ID
        /// </summary>
        public int regionID { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PlayerHistoryViewModel()
        {
        }

        /// <summary>
        /// Constructor to create ViewModel from Game entity and related data
        /// </summary>
        public PlayerHistoryViewModel(Game game, int memberNumber, DateTime tournamentDate, int regionId)
        {
            hisID = game.Id;
            GameID = game.Id;
            MemberNumber = memberNumber;
            TournamentDate = tournamentDate;
            regionID = regionId;

            // Game scores
            Game1 = game.Game1;
            Game2 = game.Game2;
            Game3 = game.Game3;
            Game4 = game.Game4;

            // Calculated/stored values
            GamesPlayed = game.GamesPlayed;
            TotalScore = game.ScratchTotal;
            HandiCap = game.Handicap ?? 0;
            Bonus = game.Bonus ?? 0;
            MoneyWon = game.MoneyWon ?? 0;
            Notes = game.Notes;

            // Averages
            AverageForEntry = game.GameAvg;
            trueAVG = game.LeagueAverage;
            AVG = game.AdjustedAvg;

            // String conversions
            ProPot = game.SidePot?.ToString() ?? "0";
            PPHG = game.PlaceStanding?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Constructor with all parameters for maximum flexibility
        /// </summary>
        public PlayerHistoryViewModel(
            int historyId,
            int memberNumber,
            int gamesPlayed,
            DateTime tournamentDate,
            int gameId,
            int? game1,
            int? game2,
            int? game3,
            int? game4,
            int totalScore,
            int handicap,
            int bonus,
            decimal moneyWon,
            string notes,
            double averageForEntry,
            double trueAvg,
            int adjustedAvg,
            string proPot,
            string pphg,
            int regionId)
        {
            hisID = historyId;
            MemberNumber = memberNumber;
            GamesPlayed = gamesPlayed;
            TournamentDate = tournamentDate;
            GameID = gameId;
            Game1 = game1;
            Game2 = game2;
            Game3 = game3;
            Game4 = game4;
            TotalScore = totalScore;
            HandiCap = handicap;
            Bonus = bonus;
            MoneyWon = moneyWon;
            Notes = notes;
            AverageForEntry = averageForEntry;
            trueAVG = trueAvg;
            AVG = adjustedAvg;
            ProPot = proPot;
            PPHG = pphg;
            regionID = regionId;
        }
    }
}
