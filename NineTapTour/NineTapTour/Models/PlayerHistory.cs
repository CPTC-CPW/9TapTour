using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NineTapTour.Models
{
    /// <summary>
    /// [DEPRECATED] This class has been refactored to use PlayerHistoryViewModel.
    /// Use PlayerHistoryViewModel instead. This class exists for backward compatibility only.
    /// 
    /// Historical Note: PlayerHistory was a duplicate storage of Game data.
    /// All data now lives in the Game entity, and this ViewModel provides a compatible interface.
    /// </summary>
    [Obsolete("PlayerHistory entity is deprecated. All data is stored in Game entity. Use PlayerHistoryViewModel for read operations.")]
    public class PlayerHistory : PlayerHistoryViewModel
    {
        // Navigation properties (kept for EF Core compatibility during transition)
        
        /// <summary>
        /// Navigation property to Game (EF Core relationship)
        /// </summary>
        [ForeignKey(nameof(GameID))]
        public Game Game { get; set; }

        /// <summary>
        /// Navigation property to NineTapRegion (EF Core relationship)
        /// </summary>
        [ForeignKey(nameof(regionID))]
        public NineTapRegion NineTapRegion { get; set; }

        /// <summary>
        /// Default constructor for backward compatibility
        /// </summary>
        public PlayerHistory() : base()
        {
        }

        /// <summary>
        /// Constructor from Game entity
        /// </summary>
        public PlayerHistory(Game game, int memberNumber, DateTime tournamentDate, int regionId)
            : base(game, memberNumber, tournamentDate, regionId)
        {
        }
    }
}
