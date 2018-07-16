using System.Drawing.Text;
using System.Windows.Forms;
using Bogus.Extensions.UnitedStates;
using NineTapTour.Database;
using NineTapTour.Models;

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
            // DO NOT CHANGE TO TRUE!!
            AutomaticMigrationsEnabled = false;
        }
        //This is a seed method using the bogus library for generating fake data https://github.com/bchavez/Bogus
        //
        //This method will seed the database for Regions and Members as of the current day 7/16/18
        //
        //Logic: checks to see if any data exists in the respective tables, it no data exists
        //it will create 2 regions and from there create as many members as you like randomizing what region they are in.
        //by putting in the starting and ending region ids it will pick regions falling between these ids
        private readonly int _startingRegionId = 1;
        private readonly int _endingRegionId = 2;
        private readonly int _numOfMembersToGenerate = 100;
        private readonly int _lowestBowlScore = 100;
        private readonly int _highestBowlScore = 299;
        private readonly int _lowestAverage = 100;
        private readonly int _highestAverage = 299;
        protected override void Seed(NineTapTour.Database.NineTapDb context)
        {
            
            if (!context.NineTapRegion.Any())
            {
                context.NineTapRegion.AddOrUpdate(r => r.NineTapRegionID,
                    new NineTapRegion {NineTapRegionID = 1, NineTapRegionName = "Washington"},
                    new NineTapRegion {NineTapRegionID = 2, NineTapRegionName = "Hawaii"}
                );
            }
                if (!context.Members.Any())
                {

//                    var regiondSeed = new Bogus.Faker<NineTapRegion>().RuleFor(r => r.NineTapRegionName, f => f.Address.State()).Generate(2);
                    var memberSeed = new Bogus.Faker<Member>().RuleFor(m => m.FirstName, f => f.Name.FirstName())
                        .RuleFor(m => m.LastName, f => f.Name.LastName())
                        .RuleFor(m => m.StartAvg, f => f.Random.Number(_lowestAverage, _highestAverage))
                        .RuleFor(m => m.Average, f => f.Random.Number(_lowestAverage, _highestAverage))
                        .RuleFor(m => m.City, f => f.Address.City())
                        .RuleFor(m => m.Street, f => f.Address.StreetAddress())
                        .RuleFor(m => m.State, f => f.Address.State())
                        .RuleFor(m => m.PostalCode, f => f.Address.ZipCode())
                        .RuleFor(m => m.DateOfBirth, f => f.Person.DateOfBirth)
                        .RuleFor(m => m.Email, f => f.Person.Email)
                        .RuleFor(m => m.IsActive, f => f.Random.Bool())
                        .RuleFor(m => m.IsLifetimeMember, f => f.Random.Bool())
                        .RuleFor(m => m.IsSenior, f => f.Random.Bool())
                        .RuleFor(m => m.JoinDate, f => f.Date.Between(DateTime.Now.AddYears(-10), DateTime.Now))
                        .RuleFor(m => m.SSN, f => f.Person.Ssn())
                        .RuleFor(m => m.PrimaryPhone, f => f.Person.Phone)
                        .RuleFor(m => m.NineTapRegionID, f => f.Random.Number(_startingRegionId, _endingRegionId))
                        .RuleFor(m => m.Number, f => f.UniqueIndex)
                        .Generate(_numOfMembersToGenerate);
               
                //TODO: columns that may need to be added
                // Bonus, Double Check averages are correct,
                //TODO: bonus, recheck average, gender is a enum so we need to use a random num generator, handicap.
                //todo: going to leave it out for now
                    //is starting avg adj average
                    // or is it avg is adjusted avg
                context.Members.AddRange(memberSeed);
                }
                context.SaveChanges();
            }
    }
}
