using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NineTapTour.Database
{
    public class Member
    {
        public int Id { get; set; }

        [Index("IX_MemberNumber", 1, IsUnique = true)]
        public int Number { get; set; }

 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string SSN { get; set; }

        // TODO: Ask Joseph for enum suggestions
        public MemberGenders Gender { get; set; }

        #region Postal Address
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        #endregion

        #region Contact Info
        public string Email { get; set; }
        public string PrimaryPhone { get; set; }
        public string SecondaryPhone { get; set; }
        #endregion

        #region Score Info
        public int? Average { get; set; }
        public int? Handicap { get; set; }
        public int? Bonus { get; set; }
        #endregion

        #region Misc. Info
        public DateTime JoinDate { get; set; }
        public DateTime? RejoinDate { get; set; }
        public DateTime? LastBowled { get; set; }
        [Column(TypeName = "Money")]
        public decimal MoneyEarned { get; set; }
        [Column(TypeName = "ntext")]
        public string Notes { get; set; }
        public int? Referrals { get; set; }
        public bool IsSenior { get; set; }
        public bool IsActive { get; set; }
        #endregion
    }

    public enum MemberGenders
    {
        Female,
        Male
    }
}
