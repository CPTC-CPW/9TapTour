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
                m => new { m.Id, m.Number, m.SSN},
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
                        Street = "8510 Idlewood Dr. SW",
                        City = "Lakewood",
                        State = "WA",
                        PostalCode = "98498",
                        PrimaryPhone = "(253) 353-2451",
                        Email = "dahl8677@student.cptc.edu"
                    }
                );
        }
    }
}
