using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.IntegrationTests.Services
{
    /// <summary>
    /// A throwaway tournament with its own members, so tests that write place
    /// standings, earnings, or finalization never disturb the seeded golden
    /// master data. Disposing deletes the games, participants, tournament, and
    /// members.
    /// </summary>
    internal sealed class ScratchTournament : IDisposable
    {
        public Tournament Tournament { get; }

        public List<Member> Members { get; } = [];

        private ScratchTournament(Tournament tournament)
        {
            Tournament = tournament;
        }

        public static ScratchTournament Create(string location, int squads, bool threeOutOf4 = false, bool isTwoDay = false, bool doubles = false)
        {
            Tournament tournament = new()
            {
                Date = new DateTime(2026, 9, 12),
                Location = location,
                Event = "Scratch Event",
                Sponsors = "",
                Notes = "",
                Squads = squads,
                ThreeOutOf4 = threeOutOf4,
                IsTwoDay = isTwoDay,
                Doubles = doubles,
                RegionId = TestDatabase.DefaultRegionId,
            };

            using NineTapDb db = TestDatabase.DbFactory.CreateDbContext();
            db.Tournaments.Add(tournament);
            db.SaveChanges();
            return new ScratchTournament(tournament);
        }

        public Member AddMember(int number, string firstName, string lastName, int average, int handicap, int bonus)
        {
            Member member = new()
            {
                Number = number,
                IsActive = true,
                FirstName = firstName,
                LastName = lastName,
                MiddleInitial = "",
                Average = average,
                Handicap = handicap,
                Bonus = bonus,
                RegionId = TestDatabase.DefaultRegionId,
            };

            using NineTapDb db = TestDatabase.DbFactory.CreateDbContext();
            db.Members.Add(member);
            db.SaveChanges();
            Members.Add(member);
            return member;
        }

        /// <summary>Adds one entry (participant + game) and returns the game id.</summary>
        public int AddEntry(Member member, int squad, int? game1, int? game2, int? game3, int? game4, int handicap, int bonus,
            decimal? moneyWon = null, decimal? sidePot = null)
        {
            using NineTapDb db = TestDatabase.DbFactory.CreateDbContext();
            db.Attach(member);
            db.Attach(Tournament);
            Game game = new()
            {
                Game1 = game1,
                Game2 = game2,
                Game3 = game3,
                Game4 = game4,
                Handicap = handicap,
                Bonus = bonus,
                MoneyWon = moneyWon,
                SidePot = sidePot,
                IsComp = false,
            };
            db.Games.Add(game);
            db.Participants.Add(new Participant { Tournament = Tournament, Member = member, Squad = squad, Game = game });
            db.SaveChanges();
            return game.Id;
        }

        public Game GetGame(int gameId)
        {
            using NineTapDb db = TestDatabase.DbFactory.CreateDbContext();
            return db.Games.Single(g => g.Id == gameId);
        }

        public Member ReloadMember(int number)
        {
            using NineTapDb db = TestDatabase.DbFactory.CreateDbContext();
            return db.Members.Single(m => m.Number == number);
        }

        public void Dispose()
        {
            new TournamentRepository(TestDatabase.DbFactory).DeleteTournament(Tournament);

            using NineTapDb db = TestDatabase.DbFactory.CreateDbContext();
            List<int> ids = [.. Members.Select(m => m.Id)];
            db.Members.RemoveRange(db.Members.Where(m => ids.Contains(m.Id)));
            db.SaveChanges();
        }
    }
}
