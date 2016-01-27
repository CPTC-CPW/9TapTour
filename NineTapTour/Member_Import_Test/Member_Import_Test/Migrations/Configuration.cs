namespace Member_Import_Test.Migrations
{
    using Classes;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Member_Import_Test.MembersDB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Member_Import_Test.MembersDB context)
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
                m => new { m.Number },
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
                        Notes = "Some notes because we can.",
                        Average = 150,
                        Handicap = 10,
                        Bonus = 3
                    }
                    );
        }
    }
}
