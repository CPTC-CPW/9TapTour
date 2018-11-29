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
        // database implementation.
        // Initial Work Alex Ramirez: 7/16/18
        // Updated Work Anthony McCann: 11/16/18
        //
        #region Description
        // Checks to see if a database currently exists. If one does NOT exist, builds 
        // regions and then builds fake member data to replicate real world ninetap 
        // data that is configurable see Instructions region below.
        #endregion

        #region Instructions
        // To rebuild/reconfigure the database go to the sql server object explorer then
        // find the ninetap database right click and check close connection and make sure
        // delete backup is also checked (is by default) then hit ok, make sure project
        // has been recently built and run UPDATE-DATABASE in the Package Manager Console.
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

        // Configurable values below. Understanding each variable before editing is recommended.
        // Variables organized by how the Seed method cascades.

        // Variable _memberStartingNumber is used for the index global in Bogus so we can 
        // start our members at a specific number and then increment by one. For example, 
        // if we set this to 0 the first member created will start at 0 then the second 
        // will be 1 and so on.
        private readonly int _memberStartingNumber = 1;

        // Currently only 1 region enabled
        private readonly int _startingRegionId = 1;
        private readonly int _endingRegionId = 1;

        // For initial Game Seed randomization
        private readonly int _lowestBowlScore = 100;
        private readonly int _highestBowlScore = 280;

        // To adjust games 2, 3, and 4's scores of initial randomizing for differentiation
        private readonly int _scoreAdjuster = 15;

        // For new Tournament Seed
        private readonly int _numberOfCurrentTournamentsToCreate = 2;
        private readonly int _furthestBackTournamentDatesInDays = 56;
        private readonly int _mostCurrentTournamentDatesInDays = 7;
        private readonly int _maxSquads = 4;

        // For Member Info randomization
        private readonly int _lowestAverage = 100;
        private readonly int _highestAverage = 299;
        private readonly int _lowestBonusPin = 0;
        private readonly int _highestBonusPin = 5;
        private readonly int _joinDatesInYearsAgo = 10;
        private readonly string _lastPossibleJoinDate = "08/01/2017";

        // For Participant Seed .Generate of Members
        private readonly int _numOfMembersToGenerate = 200;

        // For PlayerHistory Seed
        private readonly int _tournamentDatesInYearsAgo = 1;
        private readonly string _lastTournamentDate = "08/01/2018";

        // Review the documentation before changing anything directly in the Seed method
#if DEBUG
        protected override void Seed(NineTapTour.Database.NineTapDb context)
        {
            // System.Diagnostics is a Debug tool for Seed Data (Uncomment statement below Reference if needed)
            // Reference: http://blog.theodybrothers.com/2015/09/debugging-your-seed-method-when-running.html
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            if (!context.Members.Any())
            {
                // Increment local variables to prevent data mismatch in areas of the database 
                // that are not related correctly.
                // **DO NOT TOUCH THESE UNLESS THERE IS A REAL GOOD REASON, LIKE A DATABASE CHANGE.**

                #region Info about these **DO NOT TOUCH THESE UNLESS THERE IS A REAL GOOD REASON, LIKE A DATABASE CHANGE.**
                // Since tinkering with Bogus, which is meant for relational databases and the 
                // current state of the database does not use this as it should there are some 
                // work arounds that I had to implement to avoid data mismatch. Index variables
                // are used to keep track of what index we are currently at in the list to 
                // replicate the exact same data in other tables.
                //
                // The indexes are currently handicap, bonus, averages. -> Basically when a member is 
                // created their handicap, bonus, and averages are added to a list then when a game is 
                // created. The lists are tapped into and take a parallel index for values.
                #endregion

                int initialSeedIndexForLists = 0;
                int playerHistoryIndexForLists = 0;
                List<int> handicapList = new List<int>();
                List<int> bonusList = new List<int>();
                List<int> avgList = new List<int>();

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
                // We create however many tournaments are inputted into the variable _numberOfCurrentTournamentsToCreate
                // that need to be finalized on the GUI with money won amounts adjusted manually. Each member has a 
                // PlayerHistory of one tournament, half of which have money earned previously. 
                #endregion

                // Creates tournaments
                var tournamentSeed = new Bogus.Faker<Tournament>().Rules((f, t) =>
                {
                    t.Date = DateTime.Now.AddDays(f.Random.Int(-_furthestBackTournamentDatesInDays, -_mostCurrentTournamentDatesInDays));
                    t.Location = f.Address.City();
                    t.Event = $"SomeTournament {initialSeedIndexForLists}";
                    t.Notes = f.Lorem.Sentence();
                    t.Sponsors = f.Company.CompanyName();
                    t.Squads = _maxSquads;
                    t.Doubles = false;
                    t.ThreeOutOf4 = false;
                    t.TourneyRegion = 1;
                }).Generate(_numberOfCurrentTournamentsToCreate);

                // Creates Members Information
                var memberSeed = new Bogus.Faker<Member>().Rules((f, m) =>
                {
                    m.FirstName = f.Name.FirstName();
                    m.LastName = f.Name.LastName();
                    m.MiddleInitial = "";
                    m.StartAvg = f.Random.Number(_lowestAverage, _highestAverage);
                    m.Average = m.StartAvg + f.Random.Number(-_scoreAdjuster, _scoreAdjuster);
                    m.City = f.Address.City();
                    m.Street = f.Address.StreetAddress();
                    m.State = f.Address.StateAbbr();
                    m.PostalCode = f.Address.ZipCode();
                    m.DateOfBirth = f.Person.DateOfBirth;
                    m.Email = f.Person.Email;
                    m.IsActive = true;
                    m.IsLifetimeMember = false;
                    m.IsSenior = f.Random.Bool();
                    m.JoinDate = f.Date.Past(_joinDatesInYearsAgo, refDate: DateTime.Parse(_lastPossibleJoinDate)); ;
                    m.SSN = f.Person.Ssn();
                    m.PrimaryPhone = f.Person.Phone;
                    m.NineTapRegionID = f.Random.Number(_startingRegionId, _endingRegionId);
                    m.Number = f.IndexVariable++ + _memberStartingNumber;
                    m.Bonus = f.Random.Number(_lowestBonusPin, _highestBonusPin);
                    m.Handicap = Calculations.Calculations.CalculateHandicapPins(m.StartAvg.Value);
                    bonusList.Add(m.Bonus);
                    handicapList.Add(m.Handicap.Value);
                    avgList.Add(m.StartAvg.Value);
                    m.MoneyEarned = f.Random.Decimal(0, 300);
                });

                // Creates four games for each Member based off their information
                var gameSeed = new Bogus.Faker<Game>().Rules((f, g) =>
                {
                    g.Game1 = f.Random.Number(_lowestBowlScore, _highestBowlScore);
                    // These values off set the scores they bowled
                    g.Game2 = g.Game1 - f.Random.Number(-_scoreAdjuster, _scoreAdjuster);
                    g.Game3 = g.Game1 - f.Random.Number(-_scoreAdjuster, _scoreAdjuster);
                    g.Game4 = g.Game1 - f.Random.Number(-_scoreAdjuster, _scoreAdjuster);
                    g.Notes = null;
                    g.Handicap = handicapList[initialSeedIndexForLists];
                    g.TotalScore = g.Game1 + g.Game2 + g.Game3 + g.Game4;
                    g.gameRegionID = 1;
                    g.Bonus = bonusList[initialSeedIndexForLists++];
                    g.InputtedAvg = g.TotalScore / 4;
                    g.MoneyWon = f.Random.Decimal(0, 0);
                });

                // ParticipantSeed uses all other seeds besides the PlayerHistory seed 
                // to create members, their games, squad placement, region, and a random
                // tournament.
                var participantSeed = new Bogus.Faker<Participant>().Rules((f, p) =>
                {
                    p.Member = memberSeed;
                    p.Game = gameSeed;
                    p.Squad = f.Random.Number(1, _maxSquads);
                    p.ParticipantRegionID = 1;
                    p.Tournament = tournamentSeed[f.Random.Number(0, (_numberOfCurrentTournamentsToCreate - 1))];
                })
                .Generate(_numOfMembersToGenerate);

                // Add all above seeds
                context.Participants.AddRange(participantSeed);

                // Create random to use in MoneyWon formula
                Random rand = new Random();
                // Iterate through all participants created in the participantSeed
                foreach (var p in participantSeed)
                {
                    // Creates PlayerHistory for each Member
                    var playerHistorySeed = new Bogus.Faker<PlayerHistory>().Rules((f, ph) =>
                    {
                        ph.MemberNumber = playerHistoryIndexForLists + 1;
                        ph.GamesPlayed = 4;
                        ph.TournamentDate = f.Date.Past(_tournamentDatesInYearsAgo, refDate: DateTime.Parse(_lastTournamentDate));
                        ph.Game1 = avgList[playerHistoryIndexForLists];
                        // These values off set the scores they bowled
                        ph.Game2 = ph.Game1 - f.Random.Number(-_scoreAdjuster, _scoreAdjuster);
                        ph.Game3 = ph.Game1 - f.Random.Number(-_scoreAdjuster, _scoreAdjuster);
                        ph.Game4 = ph.Game1 - f.Random.Number(-_scoreAdjuster, _scoreAdjuster);
                        ph.TotalScore = ph.Game1.Value + ph.Game2.Value + ph.Game3.Value + ph.Game4.Value;
                        ph.HandiCap = handicapList[playerHistoryIndexForLists];                        
                        ph.MoneyWon = f.Random.Decimal(0, 1);
                        ph.MoneyWon *= (playerHistoryIndexForLists % 2) * (rand.Next(0, 1000));
                        ph.Notes = null;
                        ph.AverageForGame = ph.TotalScore / 4;
                        ph.AVG = avgList[playerHistoryIndexForLists];
                        ph.trueAVG = avgList[playerHistoryIndexForLists];
                        ph.Bonus = bonusList[playerHistoryIndexForLists];
                        ph.ProPot = null;
                        ph.PPHG = $"{Math.Max(ph.Game1.Value, Math.Max(ph.Game2.Value, Math.Max(ph.Game3.Value, ph.Game4.Value)))}";
                        ph.regionID = 1;
                        playerHistoryIndexForLists++;
                    });

                    // Add PlayerHistory seed
                    context.PlayerHistory.Add(playerHistorySeed);
                }
            }
            // Save All
            context.SaveChanges();
        }
#endif
    }
}
