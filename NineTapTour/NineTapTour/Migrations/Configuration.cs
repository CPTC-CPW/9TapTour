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
            // Do not turn on automatic migrations. Leaving auto migrations off is recommended for team environments
            //https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/automatic#recommendation-for-team-environments
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
        }
    }
}
