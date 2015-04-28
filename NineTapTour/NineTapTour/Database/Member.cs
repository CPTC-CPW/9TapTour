using System;
using System.ComponentModel.DataAnnotations;

namespace NineTapTour.Database
{
    public class Member
    {
        [Key]
        public int MemberNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public MemberGenders Gender { get; set; }
        public bool IsSenior { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoinDate { get; set; }
        public int Referals { get; set; }
        public string StreetAddress { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string PrimaryPhone { get; set; }
        public string SecondaryPhone { get; set; }
        public string Notes { get; set; }

    }

    public enum MemberGenders
    {
        Female,
        Male
    }
}
