using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NineTapTour.Database
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Index("IX_MemberNumber", IsUnique = true)]
        public int Number { get; set; }
        [Required]
        public bool IsActive { get; set; }

        #region Personal Info
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string MiddleInitial { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [Index("IX_MemberSSN", IsUnique = true)]
        [StringLength(9), Column(TypeName = "char")]
        public string SSN { get; set; }
        [Required]
        public MemberGenders Gender { get; set; }
        #endregion

        #region Postal Address
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
        #endregion

        #region Contact Info
        [Required]
        public string Email { get; set; }
        [Required]
        public string PrimaryPhone { get; set; }
        public string SecondaryPhone { get; set; }
        #endregion

        #region Score Info
        public int? Average { get; set; }
        public int? Handicap { get; set; }
        public int? Bonus { get; set; }
        #endregion

        #region Misc. Info
        [Required]
        public DateTime JoinDate { get; set; }
        public DateTime? RejoinDate { get; set; }
        public DateTime? LastBowled { get; set; }
        [Column(TypeName = "Money")]
        public decimal MoneyEarned { get; set; }
        public string Notes { get; set; }
        public int? Referrals { get; set; }
        [Required]
        public bool IsSenior { get; set; }
        #endregion
    }

    public enum MemberGenders
    {
        Female,
        Male
    }
}
