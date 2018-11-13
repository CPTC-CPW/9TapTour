using System.Collections;
using System.Collections.Generic;
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

        // This is a seed method using the Bogus library for generating fake data: 
        // https://github.com/bchavez/Bogus
        //
        // This method will seed the database for Regions and Members as of the current 
        // database implementation 7/16/18
        //
        #region Description
        // Description Checks to see if a database currently exists
        // If one does NOT exist build regions and then builds fake member data to 
        // replicate real world ninetap data that is configurable
        // see instruction region below.
        #endregion

        #region Instructions
        // To rebuild/reconfigure the database go to the sql server object explorer then
        // find the ninetap database right click and check close connection then hit ok
        // and run UPDATE-DATABASE in the packamanger console
        //
        //
        // Use any of the read only fields to set and configure the data being created 
        // and follow the instructions of rebuild if you want to configure it further.
        // Look at the lambda expression chain in the Seed method and refer to:
        // https://github.com/bchavez/Bogus
        #endregion

        #region Known Issues **READ THIS IF YOU ARE HAVING ISSUES**
        // TODO: "FIXED IN ISSUE #311: https://github.com/CPTC-CPW/9TapTour/issues/304" 
        //       -> KNOWN ISSUES -> isLifeTime must be false until an error pertaining to 
        //       the checking and unchecking of it is fixed. Refer to issue #304 <- FIXED
        //
        // TODO: RegionId is set to only one region since an error occuring in sifting 
        //       through members occurs when picking a region refer to issue #305 && #303:
        //       https://github.com/CPTC-CPW/9TapTour/issues?q=is%3Aissue+305+is%3Aopen
        #endregion

        // If you want to configure 
        private readonly int _numOfMembersToGenerate = 200;

        private readonly int _startingRegionId = 1;
        private readonly int _endingRegionId = 1;

        private readonly int _lowestBowlScore = 100;
        private readonly int _highestBowlScore = 299;

        private readonly int _lowestAverage = 100;
        private readonly int _highestAverage = 299;

        private readonly int _lowestBonusPin = 0;
        private readonly int _highestBonusPin = 5;

        // Variable _memberStartingNumber is used for the index global in bogus so we can 
        // start our members at a specific number and then increment by 1
        // For example if we set this to 0 the first member created will start at 0 then 
        // the second will be 1 and so on.
        private readonly int _memberStartingNumber = 1;

        private readonly int _scoreAdjuster = 15;

        // Variable _earliestJoinDate SHOULD ALWAYS BE A NEGATIVE NUMBER.
        // This will set the earliest year possible (in comparison to the date when running 
        // Update-Database) when creating fake members and their joined dates.
        private readonly DateTime _earliestJoinDate = DateTime.Now.AddYears(-10); // <-- Negative Num Required

        private readonly DateTime _latestJoinDate = DateTime.Now;
        private readonly int _maxSquads = 4;

        // Review the documentation before changing anything directly in the Seed method
#if DEBUG
        protected override void Seed(NineTapTour.Database.NineTapDb context)
        {
            if (!context.Members.Any())
            {
                // Incrementer local variables to prevent data mismatch in areas of the database 
                // that are not related correctly.
                // **DO NOT TOUCH THESE UNLESS THERE IS A REAL GOOD REASON, LIKE A DATABASE CHANGE.**

                #region Info about these **DO NOT TOUCH THESE UNLESS THERE IS A REAL GOOD REASON, LIKE A DATABASE CHANGE.**
                // Since tinkering with Bogus, which is meant for relational databases and the 
                // current state of the database does not use this as it should there are some 
                // work arounds that I had to implement to avoid data mismatch. Index is use to 
                // keep track of what index we are currently at in the list to replicate the exact 
                // same data in other tables.
                //
                // The two indexes currently are handicap and bonus -> Basically when a member is 
                // created their handicap and bonus are added to a list then when a game is created 
                // the handicap and bonus list are tapped into and take the parallel index for values.
                #endregion

                int index = 0;
                List<int> handicapList = new List<int>();
                List<int> bonusList = new List<int>();

                // Create Regions
                if (!context.NineTapRegion.Any())
                {
                    context.NineTapRegion.AddOrUpdate(r => r.NineTapRegionID,
                        new NineTapRegion { NineTapRegionID = 1, NineTapRegionName = "Washington" },
                        new NineTapRegion { NineTapRegionID = 2, NineTapRegionName = "Hawaii" }
                    );
                }

                // Create Tournament, this only generates one change generation to allow for more (you 
                // will have to change participants below and generate them into random tournaments if 
                // you want them to be spread out).

                #region Additional info about tournament 
                // We only create one tournament as that's all we need for basic testing for most things.
                // This can be expanded upon but you will need to correctly think about how the calculations 
                // would progress with each new tournament, an alternative is to create more and then go 
                // through each tournament and finalize the scores. If you really wanted to test entries over 
                // time calculation.
                #endregion
                
                var tournamentSeed = new Bogus.Faker<Tournament>().Rules((f, t) =>
                    {
                        t.Date = DateTime.Now;
                        t.Location = f.Address.City();
                        t.Event = $"SomeTournament {index}";
                        t.Notes = f.Lorem.Sentence();
                        t.Sponsors = f.Company.CompanyName();
                        t.Squads = _maxSquads;
                        t.Doubles = false;
                        t.ThreeOutOf4 = false;
                        t.TourneyRegion = 1;
                    }).Generate(1);

                // Creates members and seeds in all important information, and some extra information 
                // to simulate.
                var memberSeed = new Bogus.Faker<Member>()
                    .Rules((f, m) =>
                    {
                        m.FirstName = f.Name.FirstName();
                        m.LastName = f.Name.LastName();
                        m.MiddleInitial = "";
                        m.StartAvg = f.Random.Number(_lowestAverage, _highestAverage);
                        m.Average = f.Random.Number(_lowestAverage, _highestAverage);
                        m.City = f.Address.City();
                        m.Street = f.Address.StreetAddress();
                        m.State = f.Address.State();
                        m.PostalCode = f.Address.ZipCode();
                        m.DateOfBirth = f.Person.DateOfBirth;
                        m.Email = f.Person.Email;
                        m.IsActive = true;
                        m.IsLifetimeMember = false;
                        m.IsSenior = f.Random.Bool();
                        m.JoinDate = f.Date.Between(_earliestJoinDate, _latestJoinDate);
                        m.SSN = f.Person.Ssn();
                        m.PrimaryPhone = f.Person.Phone;
                        m.NineTapRegionID = f.Random.Number(_startingRegionId, _endingRegionId);
                        m.Number = f.IndexVariable++ + _memberStartingNumber;
                        m.Bonus = f.Random.Number(_lowestBonusPin, _highestBonusPin);
                        m.Handicap = Calculations.Calculations.CalculateHandicapPins(m.StartAvg.Value);
                        bonusList.Add(m.Bonus);
                        handicapList.Add(m.Handicap.Value);
                        m.MoneyEarned = f.Random.Decimal(0, 300);
                    });

                var gameSeed = new Bogus.Faker<Game>().Rules((f, g) => {
                    g.Game1 = f.Random.Number(100, 280);

                    // These values off set the scores they bowled
                    g.Game2 = g.Game1 - f.Random.Number(-_scoreAdjuster, _scoreAdjuster);
                    g.Game3 = g.Game1 - f.Random.Number(-_scoreAdjuster, _scoreAdjuster);
                    g.Game4 = g.Game1 - f.Random.Number(-_scoreAdjuster, _scoreAdjuster);

                    g.Notes = null;
                    g.Handicap = handicapList[index];
                    g.TotalScore = g.Game1 + g.Game2 + g.Game3 + g.Game4;
                    g.gameRegionID = 1;
                    g.Bonus = bonusList[index++];
                    g.InputtedAvg = g.TotalScore / 4;
                    g.MoneyWon = f.Random.Decimal(0, 0);
                });
                
                // Original
                var participantSeed = new Bogus.Faker<Participant>().Rules((f, p) =>
                {
                    p.Member = memberSeed;
                    p.Game = gameSeed;
                    p.Squad = f.Random.Number(1, _maxSquads);
                    p.ParticipantRegionID = 1;
                    p.Tournament = tournamentSeed[0];
                })
                .Generate(_numOfMembersToGenerate);

                // At this point you will generate one member per participant, that will also have 
                // a game related to a single tournament.
                context.Participants.AddRange(participantSeed);
            }
            context.SaveChanges();
        }
#endif
    }
}
