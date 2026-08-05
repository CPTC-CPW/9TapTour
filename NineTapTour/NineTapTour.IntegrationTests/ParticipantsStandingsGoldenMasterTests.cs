using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Entities;
using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.IntegrationTests
{
    /// <summary>
    /// Golden-master tests that freeze the CURRENT behavior of the standings
    /// queries in ParticipantsDB (raw T-SQL and EF paths) before they are moved
    /// into repositories. These assert observed behavior, including known
    /// quirks (the raw SQL Paid classification differs from the EF one, and
    /// GetStandingsForTournamentByScratch returns Score = 0 for every row).
    /// Do NOT "fix" expectations here without an intentional behavior change.
    /// </summary>
    [TestClass]
    public class ParticipantsStandingsGoldenMasterTests
    {
        private static string ThisYear => DateTime.Today.Year.ToString();
        private static string TwoYearsAgo => DateTime.Today.AddYears(-2).Year.ToString();

        /// <summary>
        /// The raw SQL standings return the database identity Members.Id in the
        /// MemberId column (NOT Member.Number, which the EF paths return).
        /// </summary>
        private static int DbId(int memberNumber) => TestDatabase.DbIdByNumber[memberNumber];

        [TestMethod]
        public void GetStandingsForThreeOf4ByScratch_ReturnsDropLowestTotalsOrderedDescending()
        {
            List<MemberScores> result = ParticipantsDB.GetStandingsForThreeOf4ByScratch(TestDatabase.ThreeOf4TournamentId);

            AssertRows(result,
            [
                // dbId, score, lastPaymentYear, paid
                // Note the raw SQL Paid classification: 'true' only when the last
                // payment is at least a year old — the opposite of the EF paths.
                (DbId(104), 690, ThisYear, false),
                (DbId(103), 637, ThisYear, false),
                (DbId(102), 570, TwoYearsAgo, true),
                (DbId(101), 510, ThisYear, false),
                (DbId(106), 369, "", false),
                (DbId(105), 330, ThisYear, false),
            ]);
        }

        [TestMethod]
        public void GetStandingsForThreeOf4ByScratch_TournamentWithMissingGamesOrLifetimeMembers_Throws()
        {
            // Current behavior on tournaments outside the happy path: a
            // participant with any null game makes the SQL Score column NULL,
            // which Convert.ToInt32 rejects (and separately, the SQL CASE mixes
            // 'life' varchar with YEAR() int, so a lifetime member with full
            // games raises a SqlException). The EF-based standings queries
            // handle both situations.
            Assert.ThrowsExactly<InvalidCastException>(() =>
                ParticipantsDB.GetStandingsForThreeOf4ByScratch(TestDatabase.RegularTournamentId));
        }

        [TestMethod]
        public void GetStandingsForThreeOutOf4ByFilterSeriesByHandicap_AllSquads()
        {
            List<MemberScores> result = ParticipantsDB.GetStandingsForThreeOutOf4ByFilterSeriesByHandicap(
                [1, 2], TestDatabase.ThreeOf4TournamentId);

            AssertRows(result,
            [
                (DbId(103), 706, ThisYear, false),
                (DbId(101), 705, ThisYear, false),
                (DbId(104), 693, ThisYear, false),
                (DbId(102), 678, TwoYearsAgo, true),
                (DbId(105), 549, ThisYear, false),
                (DbId(106), 531, "", false),
            ]);
        }

        [TestMethod]
        public void GetStandingsForThreeOutOf4ByFilterSeriesByHandicap_SingleSquadFilters()
        {
            List<MemberScores> squad1 = ParticipantsDB.GetStandingsForThreeOutOf4ByFilterSeriesByHandicap(
                [1], TestDatabase.ThreeOf4TournamentId);
            List<MemberScores> squad2 = ParticipantsDB.GetStandingsForThreeOutOf4ByFilterSeriesByHandicap(
                [2], TestDatabase.ThreeOf4TournamentId);

            CollectionAssert.AreEqual(new[] { DbId(103), DbId(101), DbId(102) }, squad1.Select(r => r.MemberId).ToArray());
            CollectionAssert.AreEqual(new[] { 706, 705, 678 }, squad1.Select(r => r.Score.Value).ToArray());

            CollectionAssert.AreEqual(new[] { DbId(104), DbId(105), DbId(106) }, squad2.Select(r => r.MemberId).ToArray());
            CollectionAssert.AreEqual(new[] { 693, 549, 531 }, squad2.Select(r => r.Score.Value).ToArray());
        }

        [TestMethod]
        public void GetStandingsForThreeOf4ByFilterSeriesByScratch_AllSquads()
        {
            List<MemberScores> result = ParticipantsDB.GetStandingsForThreeOf4ByFilterSeriesByScratch(
                [1, 2], TestDatabase.ThreeOf4TournamentId);

            CollectionAssert.AreEqual(new[] { DbId(104), DbId(103), DbId(102), DbId(101), DbId(106), DbId(105) }, result.Select(r => r.MemberId).ToArray());
            CollectionAssert.AreEqual(new[] { 690, 637, 570, 510, 369, 330 }, result.Select(r => r.Score.Value).ToArray());
        }

        [TestMethod]
        public void GetStandingsForTournamentByHandicap_ThreeOf4_MatchesRawSqlNumbers()
        {
            List<MemberScores> result = ParticipantsDB.GetStandingsForTournamentByHandicap(
                TestDatabase.ThreeOf4TournamentId, isThreeOfFourTournament: true);

            Dictionary<int, int?> scoreByMember = result.ToDictionary(r => r.MemberId, r => r.Score);
            Assert.AreEqual(705, scoreByMember[101]);
            Assert.AreEqual(678, scoreByMember[102]);
            Assert.AreEqual(706, scoreByMember[103]);
            Assert.AreEqual(693, scoreByMember[104]);
            Assert.AreEqual(549, scoreByMember[105]);
            Assert.AreEqual(531, scoreByMember[106]);

            // EF Paid rule: paid when the last payment is within a year (opposite
            // of the raw SQL classification captured above)
            Assert.IsTrue(result.Single(r => r.MemberId == 101).Paid);
            Assert.IsFalse(result.Single(r => r.MemberId == 102).Paid);
        }

        [TestMethod]
        public void GetStandingsForTournamentByHandicap_Regular_HandlesMissingGames()
        {
            List<MemberScores> result = ParticipantsDB.GetStandingsForTournamentByHandicap(
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
        public void GetStandingsForTournamentByScratch_CurrentBehavior_ScoreIsAlwaysZero()
        {
            // The interim projection never populates the per-game scores, so the
            // score summation loop adds nothing. Only the SQL-side ordering is
            // meaningful. This is current production behavior; freeze it.
            List<MemberScores> result = ParticipantsDB.GetStandingsForTournamentByScratch(
                TestDatabase.ThreeOf4TournamentId);

            Assert.HasCount(6, result);
            Assert.IsTrue(result.All(r => r.Score == 0));
        }

        [TestMethod]
        public void GetParticipants_ReturnsAllWithMemberAndGameLoaded()
        {
            List<Participant> participants = ParticipantsDB.GetParticipants(TestDatabase.ThreeOf4TournamentId);

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
