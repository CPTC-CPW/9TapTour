using NineTapTour.Database;

namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<NineTapTour.Database.NineTapDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(NineTapTour.Database.NineTapDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.Members.AddOrUpdate(
                m => new { m.Number, m.SSN },
                    new Member
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
                        Notes = "Some notes because we can."
                    },
                    new Member
                    {
                        Number = 2,
                        IsActive = true,
                        JoinDate = DateTime.Today,
                        IsSenior = false,
                        FirstName = "Joshua",
                        LastName = "Bachman",
                        MiddleInitial = "M",
                        DateOfBirth = DateTime.Parse("05/05/1990"),
                        Gender = MemberGenders.Male,
                        SSN = "987654321",
                        Street = "Josh St. NW",
                        City = "Josh Towne",
                        State = "JB",
                        PostalCode = "12345",
                        PrimaryPhone = "(555) 555-5555",
                        Email = "bachmanMailer@joshua.com",
                        Notes = "Notes here too."
                    },
                    new Member
                    {
                        Number = 3,
                        IsActive = false,
                        JoinDate = DateTime.Today,
                        IsSenior = true,
                        FirstName= "Strikes",
                        LastName= "McGee",
                        MiddleInitial = "X",
                        DateOfBirth = DateTime.Parse("6/07/1956"),
                        Gender = MemberGenders.Male,
                        SSN = "039451295",
                        Street = "McGuckit Lane",
                        City = "Gravity Falls",
                        State = "GF",
                        PostalCode = "89654",
                        PrimaryPhone = "(190) 564-4518",
                        Email = "OMMcGuckit@GoldChainsQuarterly.com",
                        Notes = "Here we go again! \"Banjo Noises\" "
                    }
                );
        }
    }
}
