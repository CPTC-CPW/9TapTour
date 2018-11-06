using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NineTapTour.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        //[Required]
        //[Index("IX_MemberNumber", IsUnique = )]
        public int Number { get; set; }
        //[Required]
        public bool IsActive { get; set; }

        #region Personal Info
        //[Required]
        public string FirstName { get; set; }
        //[Required]
        public string LastName { get; set; }

        public string MiddleInitial { get; set; }
        //[Required]
        [DataType(DataType.Date)]       
        public DateTime? DateOfBirth { get; set; }

        [Index("IX_MemberSSN")]
        [StringLength(11), Column(TypeName = "char")]
        public string SSN { get; set; }
        //[Required]
        public MemberGenders Gender { get; set; }
        #endregion

        #region Postal Address
        //[Required]
        public string Street { get; set; }
        //[Required]
        public string City { get; set; }
        //[Required]
        public string State { get; set; }
        //[Required]
        public string PostalCode { get; set; }
        #endregion

        #region Contact Info
        public string Email { get; set; }
        //[Required]
        public string PrimaryPhone { get; set; }
        public string SecondaryPhone { get; set; }
        #endregion

        #region Score Info
        //average for tournament games, used for calculated averages
        public int? Average { get; set; }
        //average for league games, not used for calculated averages, just a stored value
        public int? StartAvg { get; set; }
        public int? Handicap { get; set; }
        [DefaultValue(0)]
        public int Bonus { get; set; }
        #endregion

        #region Misc. Info
        
        [DataType(DataType.Date)]
        public DateTime? JoinDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? RejoinDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastBowled { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastPayment { get; set; }

        public bool IsLifetimeMember { get; set; }
        public string Notes { get; set; }
        public int? Referrals { get; set; }
        //[Required]
        public bool IsSenior { get; set; }
        public decimal MoneyEarned { get; set; }
        public int NineTapRegionID { get; set; }
        #endregion

        public override string ToString()
        {
            return "Member ID: " + Number + " Name: " + LastName + ", " + FirstName;
        }
    }

    public class MemberScores
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int placing { get; set; }
        public int? Score { get; set; }
        public int MemberId { get; set; } // Renamed MemberNo to MemberId because that is the actual info being assigned to this property
        public string LastPaymentYear { get; set; }
        public bool Paid { get; set; }
    }

    public enum MemberGenders
    {
        Female,
        Male
    }
}
