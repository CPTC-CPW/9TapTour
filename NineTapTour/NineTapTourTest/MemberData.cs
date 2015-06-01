using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour;
using NineTapTour.Database;


namespace NineTapTourTest
{
    [TestClass]
    public class MemberData
    {
        Member test = new Member();
        [TestMethod]
        public void SaveShouldWork()
        {
            //arrange
            NineTapDb db = new NineTapDb();
             test = new Member
                    {
                        Number = 1,
                        IsActive = true,
                        JoinDate = DateTime.Today,
                        IsSenior = false,
                        FirstName = "Matthew",
                        LastName = "Dahl",
                        MiddleInitial = "S",
                        DateOfBirth = DateTime.Parse("04/04/1985"),
                        Gender = MemberGenders.Male,
                        SSN = "123456789",
                        Street = "Matt Dr. SW",
                        City = "Mattville",
                        State = "MD",
                        PostalCode = "54321",
                        PrimaryPhone = "(555) 555-5555",
                        Email = "matt-mail@mail-matt.com",
                    };
            //act
            MemberDb.AddMember(test);

            //assert 



        }
    }
}
