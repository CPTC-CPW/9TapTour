#nullable disable
using NineTapTour.Core.Entities;
namespace NineTapTour.Core.Models
{
    /// <summary>
    /// A wrapper class that extends MemberScores to represent a doubles team's combined standings.
    /// Each instance represents a paired team with combined score and both partners' data.
    /// </summary>
    public class TeamMemberScores : MemberScores
    {
        /// <summary>
        /// Gets or sets the first team member's ID.
        /// </summary>
        public int Partner1MemberId { get; set; }

        /// <summary>
        /// Gets or sets the first team member's first name.
        /// </summary>
        public string Partner1FirstName { get; set; }

        /// <summary>
        /// Gets or sets the first team member's last name.
        /// </summary>
        public string Partner1LastName { get; set; }

        /// <summary>
        /// Gets or sets the second team member's ID.
        /// </summary>
        public int Partner2MemberId { get; set; }

        /// <summary>
        /// Gets or sets the second team member's first name.
        /// </summary>
        public string Partner2FirstName { get; set; }

        /// <summary>
        /// Gets or sets the second team member's last name.
        /// </summary>
        public string Partner2LastName { get; set; }

        /// <summary>
        /// Gets or sets the first partner's individual score.
        /// </summary>
        public int? Partner1Score { get; set; }

        /// <summary>
        /// Gets or sets the second team member's last membership payment year.
        /// </summary>
        public string Partner2LastPaymentYear { get; set; }

        /// <summary>
        /// Gets or sets the second partner's individual score.
        /// </summary>
        public int? Partner2Score { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a team record (as opposed to an individual).
        /// </summary>
        public bool IsTeam { get; set; } = true;
    }
}
