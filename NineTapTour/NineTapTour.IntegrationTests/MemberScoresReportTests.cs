using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Data.Repositories;   // StandingsRepository
using NineTapTour.Models;

namespace NineTapTour.IntegrationTests
{
    /// <summary>
    /// Integration coverage for the per-game "high game" and senior reports produced by
    /// <see cref="StandingsRepository.GetGameMemberScores"/> and
    /// <see cref="StandingsRepository.GetSeniorMemberScores"/>. Both expand each participant into one
    /// <see cref="MemberScores"/> per game column (4 per participant).
    /// </summary>
    [TestClass]
    [TestCategory("Integration")]
    public class MemberScoresReportTests
    {
        [TestMethod]
        public void GetGameMemberScores_ReturnsFourEntriesPerParticipant()
        {
            var factory = IntegrationEnvironment.Require();

            int aNum = TestSeed.NextNumber();
            int bNum = TestSeed.NextNumber();
            int tournamentId;

            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);
                TestSeed.AddEntry(db, t, 1, 200, 180, 190, 210, memberNumber: aNum);
                TestSeed.AddEntry(db, t, 1, 150, 160, 170, 140, memberNumber: bNum);
                db.SaveChanges();
                tournamentId = t.Id;
            }

            var repo = new StandingsRepository(factory);
            List<MemberScores> result = repo.GetGameMemberScores(tournamentId);

            // 2 participants * 4 game columns = 8 entries.
            Assert.AreEqual(8, result.Count, "Expected four entries per participant.");
            Assert.AreEqual(4, result.Count(s => s.MemberId == aNum), "Member A should have four entries.");
            Assert.AreEqual(4, result.Count(s => s.MemberId == bNum), "Member B should have four entries.");

            var aScores = result.Where(s => s.MemberId == aNum).Select(s => s.Score).ToList();
            CollectionAssert.AreEquivalent(new int?[] { 200, 180, 190, 210 }, aScores,
                "Member A's four game scores should be present.");
        }

        [TestMethod]
        public void GetGameMemberScores_NullGame_ProducesNullScoreEntry_NotCrash()
        {
            var factory = IntegrationEnvironment.Require();

            int memberNum = TestSeed.NextNumber();
            int tournamentId;

            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);
                // Only three games bowled; the fourth is null.
                TestSeed.AddEntry(db, t, 1, 200, 180, 190, null, memberNumber: memberNum);
                db.SaveChanges();
                tournamentId = t.Id;
            }

            var repo = new StandingsRepository(factory);
            List<MemberScores> result = repo.GetGameMemberScores(tournamentId);

            var memberEntries = result.Where(s => s.MemberId == memberNum).ToList();
            Assert.AreEqual(4, memberEntries.Count, "A partially-bowled participant still yields four entries.");
            Assert.AreEqual(1, memberEntries.Count(s => s.Score == null),
                "Exactly one entry should carry a null score for the unbowled game.");
        }

        [TestMethod]
        public void GetGameMemberScores_EmptyTournament_ReturnsEmpty()
        {
            var factory = IntegrationEnvironment.Require();

            int tournamentId;
            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);
                db.SaveChanges();
                tournamentId = t.Id;
            }

            var repo = new StandingsRepository(factory);
            List<MemberScores> result = repo.GetGameMemberScores(tournamentId);

            Assert.AreEqual(0, result.Count, "A tournament with no participants should return no scores.");
        }

        [TestMethod]
        public void GetSeniorMemberScores_OnlyIncludesSeniors()
        {
            var factory = IntegrationEnvironment.Require();

            int seniorNum = TestSeed.NextNumber();
            int nonSeniorNum = TestSeed.NextNumber();
            int tournamentId;

            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);
                TestSeed.AddEntry(db, t, 1, 200, 180, 190, 210, memberNumber: seniorNum, senior: true);
                TestSeed.AddEntry(db, t, 1, 150, 160, 170, 140, memberNumber: nonSeniorNum, senior: false);
                db.SaveChanges();
                tournamentId = t.Id;
            }

            var repo = new StandingsRepository(factory);
            List<MemberScores> result = repo.GetSeniorMemberScores(tournamentId);

            Assert.AreEqual(4, result.Count, "Only the single senior participant's four entries should appear.");
            Assert.IsTrue(result.All(s => s.MemberId == seniorNum),
                "Every returned entry must belong to the senior member.");
            Assert.IsFalse(result.Any(s => s.MemberId == nonSeniorNum),
                "The non-senior member must be excluded.");
        }

        [TestMethod]
        public void GetSeniorMemberScores_SortedByScoreDescending()
        {
            var factory = IntegrationEnvironment.Require();

            int seniorNum = TestSeed.NextNumber();
            int tournamentId;

            using (var db = factory.CreateDbContext())
            {
                var t = TestSeed.AddTournament(db);
                TestSeed.AddEntry(db, t, 1, 200, 180, 190, 210, memberNumber: seniorNum, senior: true);
                db.SaveChanges();
                tournamentId = t.Id;
            }

            var repo = new StandingsRepository(factory);
            List<MemberScores> result = repo.GetSeniorMemberScores(tournamentId);

            Assert.AreEqual(4, result.Count, "The senior participant should yield four entries.");

            // Scores must be non-increasing; treat null as the lowest possible value.
            for (int i = 0; i < result.Count - 1; i++)
            {
                int current = result[i].Score ?? int.MinValue;
                int next = result[i + 1].Score ?? int.MinValue;
                Assert.IsTrue(current >= next,
                    $"Scores must be sorted descending; index {i} ({current}) was less than index {i + 1} ({next}).");
            }
        }
    }
}
