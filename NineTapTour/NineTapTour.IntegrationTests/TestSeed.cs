using System;
using System.Threading;
using NineTapTour.Database;
using NineTapTour.Models;

namespace NineTapTour.IntegrationTests
{
    /// <summary>
    /// Fluent-ish helpers for seeding tournament data into a test <see cref="NineTapDb"/>. Member
    /// numbers auto-increment (globally unique across the run) unless one is supplied, so tests that
    /// share the run-wide database do not collide.
    /// </summary>
    internal static class TestSeed
    {
        private static int _seq = 5000;

        /// <summary>A process-unique member number.</summary>
        public static int NextNumber() => Interlocked.Increment(ref _seq);

        public static Tournament AddTournament(NineTapDb db, bool threeOfFour = false, int squads = 4,
            bool doubles = false, DateTime? date = null, string location = "Test Tournament")
        {
            var t = new Tournament
            {
                Date = date ?? new DateTime(2026, 1, 1),
                Location = location,
                Event = "Test",
                Squads = squads,
                ThreeOutOf4 = threeOfFour,
                Doubles = doubles,
            };
            db.Tournaments.Add(t);
            return t;
        }

        public static Member AddMember(NineTapDb db, int? number = null, string first = "First",
            string last = "Last", bool active = true, bool senior = false, DateTime? lastPayment = null,
            bool lifetime = false, int? average = null, int? handicap = null, int bonus = 0)
        {
            var m = new Member
            {
                Number = number ?? NextNumber(),
                FirstName = first,
                LastName = last,
                IsActive = active,
                IsSenior = senior,
                LastPayment = lastPayment ?? new DateTime(2026, 1, 1),
                IsLifetimeMember = lifetime,
                Average = average,
                Handicap = handicap,
                Bonus = bonus,
            };
            db.Members.Add(m);
            return m;
        }

        public static Game AddGame(NineTapDb db, int? g1 = null, int? g2 = null, int? g3 = null,
            int? g4 = null, int handicap = 0, int bonus = 0, bool finalized = false,
            decimal? moneyWon = null, int adjustedAvg = 0, int? placeStanding = null)
        {
            var g = new Game
            {
                Game1 = g1,
                Game2 = g2,
                Game3 = g3,
                Game4 = g4,
                Handicap = handicap,
                Bonus = bonus,
                IsFinalized = finalized,
                MoneyWon = moneyWon,
                AdjustedAvg = adjustedAvg,
                PlaceStanding = placeStanding,
                UseGame1 = true,
                UseGame2 = true,
                UseGame3 = true,
                UseGame4 = true,
            };
            db.Games.Add(g);
            return g;
        }

        public static Participant AddParticipant(NineTapDb db, Tournament tournament, Member member,
            Game game, int squad = 1)
        {
            var p = new Participant { Tournament = tournament, Member = member, Game = game, Squad = squad };
            db.Participants.Add(p);
            return p;
        }

        /// <summary>Adds a member + game + participant in one call and returns the participant.</summary>
        public static Participant AddEntry(NineTapDb db, Tournament tournament, int squad,
            int? g1, int? g2, int? g3, int? g4, int handicap = 0, int bonus = 0, bool finalized = false,
            decimal? moneyWon = null, int adjustedAvg = 0, string first = "First", string last = "Last",
            int? memberNumber = null, bool senior = false)
        {
            Member member = AddMember(db, memberNumber, first, last, senior: senior);
            Game game = AddGame(db, g1, g2, g3, g4, handicap, bonus, finalized, moneyWon, adjustedAvg);
            return AddParticipant(db, tournament, member, game, squad);
        }
    }
}
