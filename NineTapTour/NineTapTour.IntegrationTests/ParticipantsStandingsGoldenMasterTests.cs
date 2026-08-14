using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.IntegrationTests
{
    /// <summary>
    /// Golden-master tests for the standings queries in ParticipantRepository.
    /// These originally froze the pre-refactor behavior of the raw T-SQL
    /// standings paths, including their known defects. Those defects were fixed
    /// on 2026-08-14 (the raw SQL was replaced with EF/LINQ implementations that
    /// mirror the EF siblings), so these tests now pin the FIXED behavior:
    /// MemberId is Member.Number, Paid uses the EF rule (paid when the last
    /// payment is within a year, or lifetime), lifetime members and null games
    /// are handled gracefully, and the scratch standings return real totals.
    /// </summary>
    [TestClass]
    public class ParticipantsStandingsGoldenMasterTests
    {
        private static string ThisYear => DateTime.Today.Year.ToString();
        private static string TwoYearsAgo => DateTime.Today.AddYears(-2).Year.ToString();

        private static ParticipantRepository Repo => new(TestDatabase.DbFactory);

        [TestMethod]
        public void GetStandingsForThreeOf4ByScratch_ReturnsDropLowestTotalsOrderedDescending()
        {
            List<MemberScores> result = Repo.GetStandingsForThreeOf4ByScratch(TestDatabase.ThreeOf4TournamentId);

            AssertRows(result,
            [
                // memberNumber, score, lastPaymentYear, paid
                // Fixed 2026-08-14: MemberId is now Member.Number (was Members.Id),
                // Paid follows the EF rule (true when paid within a year or
                // lifetime; the raw SQL had it inverted), and a null LastPayment
                // yields "" (EF translates ToString() with COALESCE(..., '')).
                (104, 690, ThisYear, true),
                (103, 637, ThisYear, true),
                (102, 570, TwoYearsAgo, false),
                (101, 510, ThisYear, true),
                (106, 369, "", true),
                (105, 330, ThisYear, true),
            ]);
        }

        [TestMethod]
        public void GetStandingsForThreeOf4ByScratch_HandlesMissingGamesAndLifetimeMembers()
        {
            // Fixed 2026-08-14: the raw SQL predecessor crashed on this tournament
            // (a null game made Score NULL, which Convert.ToInt32 rejected, and the
            // lifetime member's CASE mixed 'life' varchar with YEAR() int). The EF
            // implementation sums the played games and drops the lowest game only
            // when all four are present.
            List<MemberScores> result = Repo.GetStandingsForThreeOf4ByScratch(TestDatabase.RegularTournamentId);

            AssertRows(result,
            [
                // memberNumber, score, lastPaymentYear, paid
                (106, 570, "", true),       // 180+190+200 (only 3 games: nothing dropped)
                (101, 510, ThisYear, true), // 150+160+170+180 - 150 (all 4 games: lowest dropped)
                (105, 390, ThisYear, true), // 190+200
                (107, 210, "life ", true),  // 210; lifetime member handled gracefully
            ]);
        }

        [TestMethod]
        public void GetStandingsForThreeOutOf4ByFilterSeriesByHandicap_AllSquads()
        {
            List<MemberScores> result = Repo.GetStandingsForThreeOutOf4ByFilterSeriesByHandicap(
                [1, 2], TestDatabase.ThreeOf4TournamentId);

            // Scores are unchanged from the raw SQL masters (the per-game EF
            // handicap math is numerically identical to sum + H*3 + B*3 - lowest),
            // but MemberId/Paid/LastPaymentYear follow the fixed EF semantics.
            AssertRows(result,
            [
                (103, 706, ThisYear, true),
                (101, 705, ThisYear, true),
                (104, 693, ThisYear, true),
                (102, 678, TwoYearsAgo, false),
                (105, 549, ThisYear, true),
                (106, 531, "", true),
            ]);
        }

        [TestMethod]
        public void GetStandingsForThreeOutOf4ByFilterSeriesByHandicap_SingleSquadFilters()
        {
            List<MemberScores> squad1 = Repo.GetStandingsForThreeOutOf4ByFilterSeriesByHandicap(
                [1], TestDatabase.ThreeOf4TournamentId);
            List<MemberScores> squad2 = Repo.GetStandingsForThreeOutOf4ByFilterSeriesByHandicap(
                [2], TestDatabase.ThreeOf4TournamentId);

            CollectionAssert.AreEqual(new[] { 103, 101, 102 }, squad1.Select(r => r.MemberId).ToArray());
            CollectionAssert.AreEqual(new[] { 706, 705, 678 }, squad1.Select(r => r.Score.Value).ToArray());

            CollectionAssert.AreEqual(new[] { 104, 105, 106 }, squad2.Select(r => r.MemberId).ToArray());
            CollectionAssert.AreEqual(new[] { 693, 549, 531 }, squad2.Select(r => r.Score.Value).ToArray());
        }

        [TestMethod]
        public void GetStandingsForThreeOf4ByFilterSeriesByScratch_AllSquads()
        {
            List<MemberScores> result = Repo.GetStandingsForThreeOf4ByFilterSeriesByScratch(
                [1, 2], TestDatabase.ThreeOf4TournamentId);

            // Like the raw SQL it replaced, this filters all squads at once and
            // orders globally by score descending.
            CollectionAssert.AreEqual(new[] { 104, 103, 102, 101, 106, 105 }, result.Select(r => r.MemberId).ToArray());
            CollectionAssert.AreEqual(new[] { 690, 637, 570, 510, 369, 330 }, result.Select(r => r.Score.Value).ToArray());
        }

        [TestMethod]
        public void GetStandingsForTournamentByHandicap_ThreeOf4_MatchesRawSqlNumbers()
        {
            List<MemberScores> result = Repo.GetStandingsForTournamentByHandicap(
                TestDatabase.ThreeOf4TournamentId, isThreeOfFourTournament: true);

            Dictionary<int, int?> scoreByMember = result.ToDictionary(r => r.MemberId, r => r.Score);
            Assert.AreEqual(705, scoreByMember[101]);
            Assert.AreEqual(678, scoreByMember[102]);
            Assert.AreEqual(706, scoreByMember[103]);
            Assert.AreEqual(693, scoreByMember[104]);
            Assert.AreEqual(549, scoreByMember[105]);
            Assert.AreEqual(531, scoreByMember[106]);

            // EF Paid rule: paid when the last payment is within a year
            Assert.IsTrue(result.Single(r => r.MemberId == 101).Paid);
            Assert.IsFalse(result.Single(r => r.MemberId == 102).Paid);
        }

        [TestMethod]
        public void GetStandingsForTournamentByHandicap_Regular_HandlesMissingGames()
        {
            List<MemberScores> result = Repo.GetStandingsForTournamentByHandicap(
                TestDatabase.RegularTournamentId);

            Assert.HasCount(4, result);
            // Only the participant with all four games has a non-null ordering key,
            // so they sort first; per-member scores add handicap+bonus per played game.
            Assert.AreEqual(101, result[0].MemberId);

            Dictionary<int, int?> scoreByMember = result.ToDictionary(r => r.MemberId, r => r.Score);
            Assert.AreEqual(920, scoreByMember[101]); // 660 + 4*(63+2)
            Assert.AreEqual(536, scoreByMember[105]); // 390 + 2*(70+3)
            Assert.AreEqual(732, scoreByMember[106]); // 570 + 3*(54+0)
            Assert.AreEqual(233, scoreByMember[107]); // 210 + 1*(18+5)

            // EF path formats the lifetime marker with a trailing space
            MemberScores grace = result.Single(r => r.MemberId == 107);
            Assert.AreEqual("life ", grace.LastPaymentYear);
            Assert.IsTrue(grace.Paid);
        }

        [TestMethod]
        public void GetStandingsForTournamentByScratch_ReturnsPlainGameSumsOrderedDescending()
        {
            // Fixed 2026-08-14: the interim projection previously never populated
            // the per-game scores, so every row's Score was 0. Scratch totals are
            // now the plain 4-game sums (no handicap/bonus, no drop-lowest since
            // isThreeOfFourTournament defaults to false).
            List<MemberScores> result = Repo.GetStandingsForTournamentByScratch(
                TestDatabase.ThreeOf4TournamentId);

            CollectionAssert.AreEqual(new[] { 103, 104, 102, 101, 106, 105 }, result.Select(r => r.MemberId).ToArray());
            CollectionAssert.AreEqual(new[] { 847, 800, 740, 660, 490, 430 }, result.Select(r => r.Score.Value).ToArray());
        }

        [TestMethod]
        public void GetStandingsForTournamentByFilterSeriesByScratch_ReturnsPlainGameSumsPerSquad()
        {
            // Fixed 2026-08-14: the scratch summation previously added the null
            // HandicapValue/BonusPinValue, nulling out the totals. Like its
            // handicap sibling, this method appends each requested squad's rows in
            // turn, ordered by game sum descending within the squad.
            List<MemberScores> result = Repo.GetStandingsForTournamentByFilterSeriesByScratch(
                [1, 2], TestDatabase.ThreeOf4TournamentId);

            CollectionAssert.AreEqual(new[] { 103, 102, 101, 104, 106, 105 }, result.Select(r => r.MemberId).ToArray());
            CollectionAssert.AreEqual(new[] { 847, 740, 660, 800, 490, 430 }, result.Select(r => r.Score.Value).ToArray());
        }

        [TestMethod]
        public void GetParticipants_ReturnsAllWithMemberAndGameLoaded()
        {
            List<Participant> participants = Repo.GetParticipants(TestDatabase.ThreeOf4TournamentId);

            Assert.HasCount(6, participants);
            Assert.IsTrue(participants.All(p => p.Member != null && p.Game != null && p.Tournament != null));
        }

        private static void AssertRows(List<MemberScores> actual,
            (int MemberId, int Score, string LastPaymentYear, bool Paid)[] expected)
        {
            CollectionAssert.AreEqual(expected.Select(e => e.MemberId).ToArray(),
                actual.Select(r => r.MemberId).ToArray(), "member ordering");
            CollectionAssert.AreEqual(expected.Select(e => e.Score).ToArray(),
                actual.Select(r => r.Score.Value).ToArray(), "scores");
            CollectionAssert.AreEqual(expected.Select(e => e.LastPaymentYear).ToArray(),
                actual.Select(r => r.LastPaymentYear).ToArray(), "last payment year");
            CollectionAssert.AreEqual(expected.Select(e => e.Paid).ToArray(),
                actual.Select(r => r.Paid).ToArray(), "paid flags");
        }
    }
}
