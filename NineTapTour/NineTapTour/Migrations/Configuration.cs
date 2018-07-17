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
        //This method will seed the database for Regions and Members as of the current database implementation 7/16/18
        //
        #region Description
        //Description Checks to see if a database currently exists
        //If one does NOT exist build regions and then builds fake member data to replicate real world ninetap data that is configurable
        //see instruction region below.


        #endregion

        #region Instructions
        //To rebuild/reconfigure the database go to the sql server object explorer then
        //find the ninetap database right click and check close connection then hit ok
        //and run UPDATE-DATABASE in the packamanger console
        //
        //
        //Use any of the read only fields to set and configure the data being created and follow the instructions of rebuild
        //if you want to configure it further Look at the lambda expression chain in the Seed method and refer to https://github.com/bchavez/Bogus
        //


        #endregion

        #region Known Issues **READ THIS IF YOU ARE HAVING ISSUES**
        //TODO: KNOWN ISSUES -> isLifeTime must be false until an error pertaining to the checking and unchecking of it is fixed.Refer to issue #
        //TODO:RegionId is set to only 1 region since an error occuring in sifting through members occurs when picking a region refer to issue #
        //TODO: 
        #endregion

        //If you want to configure 
        private readonly int _numOfMembersToGenerate = 300;

        private readonly int _startingRegionId = 1;
        private readonly int _endingRegionId = 2;

        private readonly int _lowestBowlScore = 100;
        private readonly int _highestBowlScore = 299;

        private readonly int _lowestAverage = 100;
        private readonly int _highestAverage = 299;

        private readonly int _lowestBonusPin = 0;
        private readonly int _highestBonusPin = 0;

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
                var memberSeed = new Bogus.Faker<Member>().RuleFor(m => m.FirstName, f => f.Name.FirstName())
                    .RuleFor(m => m.LastName, f => f.Name.LastName())
                    .RuleFor(m => m.MiddleInitial, f => "")
                    .RuleFor(m => m.StartAvg, f => f.Random.Number(_lowestAverage, _highestAverage))
                    .RuleFor(m => m.Average, f => f.Random.Number(_lowestAverage, _highestAverage))
                    .RuleFor(m => m.City, f => f.Address.City())
                    .RuleFor(m => m.Street, f => f.Address.StreetAddress())
                    .RuleFor(m => m.State, f => f.Address.State())
                    .RuleFor(m => m.PostalCode, f => f.Address.ZipCode())
                    .RuleFor(m => m.DateOfBirth, f => f.Person.DateOfBirth)
                    .RuleFor(m => m.Email, f => f.Person.Email)
                    .RuleFor(m => m.IsActive, f => true)
                    .RuleFor(m => m.IsLifetimeMember, f => false)
                    .RuleFor(m => m.IsSenior, f => f.Random.Bool())
                    .RuleFor(m => m.JoinDate, f => f.Date.Between(DateTime.Now.AddYears(-10), DateTime.Now).Date)
                    .RuleFor(m => m.SSN, f => f.Person.Ssn())
                    .RuleFor(m => m.PrimaryPhone, f => f.Person.Phone)
                    .RuleFor(m => m.NineTapRegionID, f => _startingRegionId)
                    .RuleFor(m => m.Number, f => f.IndexGlobal + 1)
                    .RuleFor(m => m.Bonus, f => f.Random.Number(_lowestBonusPin, _highestBonusPin))
                    .Generate(_numOfMembersToGenerate);
            context.Members.AddRange(memberSeed);
            }
            context.SaveChanges();
        }
    }
}
