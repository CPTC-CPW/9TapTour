using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Data.Repositories;
using NineTapTour.Data.Services;
using NineTapTour.Models;

namespace NineTapTour.IntegrationTests
{
    /// <summary>
    /// Full-database tests for the finalize orchestration extracted from FrmFinalizeTournament.
    /// </summary>
    [TestClass]
    [TestCategory("Integration")]
    public class FinalizationServiceTests
    {
        private static FinalizationService NewService(Microsoft.EntityFrameworkCore.IDbContextFactory<NineTapTour.Database.NineTapDb> factory)
            => new FinalizationService(factory, new FinalizeRepository(factory));

        [TestMethod]
        public void FinalizeTournament_Singles_PersistsGameMemberAndTournament()
        {
            var factory = IntegrationEnvironment.Require();
            int tournamentId, gameId, memberNumber = TestSeed.NextNumber();
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);
                var p = TestSeed.AddEntry(db, t, 1, 200, 180, 190, 210, memberNumber: memberNumber);
                db.SaveChanges();
                tournamentId = t.Id; gameId = p.Game.Id;
            }

            NewService(factory).FinalizeTournament(tournamentId, new List<FinalizeGameInput>
            {
                new() { GameId = gameId, IsDoublesMember = false, MemberNumber = memberNumber, AdjustedAvg = 185, Handicap = 30, BonusFromGrid = 3 }
            });

            using (var db = factory.CreateDbContext())
            {
                Game game = db.Games.Find(gameId);
                Assert.IsTrue(game.IsFinalized);
                Assert.AreEqual(185, game.AdjustedAvg);
                Assert.AreEqual(30, game.Handicap);
                Assert.AreEqual(3, game.Bonus);

                Member member = db.Members.Single(m => m.Number == memberNumber);
                Assert.AreEqual(185, member.Average);
                Assert.AreEqual(3, member.Bonus);
                // CalculateHandicapPins(185) = (220-185)*90/100 = 31
                Assert.AreEqual(31, member.Handicap);

                Assert.IsTrue(db.Tournaments.Find(tournamentId).IsTournamentFinalized);
            }
        }

        [TestMethod]
        public void FinalizeTournament_CashingSingles_KeepsOriginalBonusOnGame_GridBonusOnMember()
        {
            var factory = IntegrationEnvironment.Require();
            int tournamentId, gameId, memberNumber = TestSeed.NextNumber();
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);
                var p = TestSeed.AddEntry(db, t, 1, 210, 200, 220, 205, memberNumber: memberNumber);
                db.SaveChanges();
                tournamentId = t.Id; gameId = p.Game.Id;
            }

            NewService(factory).FinalizeTournament(tournamentId, new List<FinalizeGameInput>
            {
                new() { GameId = gameId, IsDoublesMember = false, MemberNumber = memberNumber,
                        AdjustedAvg = 190, Handicap = 27, IsCashing = true, BonusFromGrid = 1, OriginalBaseBonus = 5 }
            });

            using (var db = factory.CreateDbContext())
            {
                Game game = db.Games.Find(gameId);
                Assert.AreEqual(5, game.Bonus, "Cashing game keeps the original pre-deduction bonus.");
                Member member = db.Members.Single(m => m.Number == memberNumber);
                Assert.AreEqual(1, member.Bonus, "Member record takes the (deducted) grid bonus.");
            }
        }

        [TestMethod]
        public void FinalizeTournament_DoublesMember_SplitsEarnings_HalfRateBonus_AndPlace()
        {
            var factory = IntegrationEnvironment.Require();
            int tournamentId, gameId, memberNumber = TestSeed.NextNumber();
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db, doubles: true);
                var p = TestSeed.AddEntry(db, t, 1, 200, 180, null, null, memberNumber: memberNumber);
                db.SaveChanges();
                tournamentId = t.Id; gameId = p.Game.Id;
            }

            NewService(factory).FinalizeTournament(tournamentId, new List<FinalizeGameInput>
            {
                new() { GameId = gameId, IsDoublesMember = true, MemberNumber = memberNumber,
                        Game1 = 200, Game2 = 180, UseGame1 = true, UseGame2 = true,
                        AdjustedAvg = 180, Handicap = 36, PlaceStanding = 1, Earnings = 100m,
                        IsCashing = true, OriginalBaseBonus = 4 }
            });

            using (var db = factory.CreateDbContext())
            {
                Game game = db.Games.Find(gameId);
                Assert.AreEqual(50m, game.MoneyWon, "Doubles earnings are the 50% share of the place prize.");
                Assert.AreEqual(2, game.Bonus, "Half-rate bonus for a 1st-place casher with base 4.");
                Assert.AreEqual(1, game.PlaceStanding);
                Assert.IsTrue(game.IsFinalized);
            }
        }
    }
}
