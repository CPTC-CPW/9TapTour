using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.Services;
using System;
using System.Linq;

namespace NineTapTour.IntegrationTests
{
    /// <summary>
    /// Score entry against the seeded LocalDB catalog. Uses a member number >= 950 and
    /// tournament dates that do not collide with the golden-master data, and removes
    /// every row it inserts so the whole-database assertions in the other integration
    /// tests keep holding.
    /// </summary>
    [TestClass]
    public class ScoresServiceIntegrationTests
    {
        private const int MemberNumber = 950;

        private static ScoresService CreateService()
        {
            return new ScoresService(
                new MemberRepository(TestDatabase.DbFactory),
                new GameRepository(TestDatabase.DbFactory),
                new TournamentRepository(TestDatabase.DbFactory),
                new ParticipantRepository(TestDatabase.DbFactory),
                new DoublesTeamRepository(TestDatabase.DbFactory),
                TestDatabase.DbFactory);
        }

        /// <summary>
        /// Issue #1291. After a tournament is finalized the director raises the member's
        /// average on the member form. The member's next entry must be scored with the
        /// handicap of that edited average, not with a handicap re-derived from the last
        /// finalized tournament's adjusted average.
        /// </summary>
        [TestMethod]
        public void SaveScoreEntry_NewEntry_UsesMemberHandicap_NotLastFinalizedAdjustedAverage()
        {
            MemberRepository memberRepository = new(TestDatabase.DbFactory);
            int finalizedTournamentId = 0;
            int openTournamentId = 0;

            try
            {
                Member member = new()
                {
                    Number = MemberNumber,
                    IsActive = true,
                    FirstName = "Hal",
                    LastName = "Handicap",
                    MiddleInitial = "",
                    Gender = MemberGenders.Male,
                    Street = "1 Handicap St",
                    City = "Testville",
                    State = "WA",
                    PostalCode = "98000",
                    PrimaryPhone = "555-0950",
                    Average = 190,
                };
                memberRepository.AddOrUpdateMember(member);   // handicap (220-190)*90/100 = 27

                using (NineTapDb db = TestDatabase.DbFactory.CreateDbContext())
                {
                    Tournament finalized = new()
                    {
                        Date = new DateTime(2026, 3, 1),
                        Location = "Issue 1291 Lanes",
                        Event = "Finalized before the average edit",
                        Sponsors = "",
                        Notes = "",
                        Squads = 1,
                        IsTournamentFinalized = true,
                    };
                    Tournament open = new()
                    {
                        Date = new DateTime(2026, 3, 8),
                        Location = "Issue 1291 Lanes",
                        Event = "Open after the average edit",
                        Sponsors = "",
                        Notes = "",
                        Squads = 1,
                    };
                    db.Tournaments.AddRange(finalized, open);

                    Game finalizedGame = new()
                    {
                        Game1 = 180, Game2 = 190, Game3 = 200, Game4 = 210,
                        Handicap = 27,
                        Bonus = 0,
                        MoneyWon = 0,
                        AdjustedAvg = 190,
                        IsFinalized = true,
                    };
                    db.Games.Add(finalizedGame);
                    db.Participants.Add(new Participant
                    {
                        Tournament = finalized,
                        Member = db.Members.Single(m => m.Number == MemberNumber),
                        Squad = 1,
                        Game = finalizedGame,
                    });
                    db.SaveChanges();

                    finalizedTournamentId = finalized.Id;
                    openTournamentId = open.Id;
                }

                // The director raises the average on the member form after the finalize.
                Member edited = memberRepository.GetMember(MemberNumber);
                edited.Average = 200;
                memberRepository.AddOrUpdateMember(edited);   // handicap (220-200)*90/100 = 18
                Assert.AreEqual(18, memberRepository.GetMember(MemberNumber).Handicap);

                ScoreEntryResult result = CreateService().SaveScoreEntry(
                    new ScoreEntryRequest(openTournamentId, MemberNumber, Squad: 1, 170, 180, 190, 200, MoneyWon: 0, IsComp: false));
                Assert.IsTrue(result.Success, result.ErrorMessage);

                using NineTapDb verify = TestDatabase.DbFactory.CreateDbContext();
                Game entry = verify.Participants
                    .Where(p => p.Tournament.Id == openTournamentId && p.Member.Number == MemberNumber)
                    .Select(p => p.Game)
                    .Single();
                Assert.AreEqual(18, entry.Handicap,
                    "the new entry must carry the handicap of the edited average (200 -> 18), " +
                    "not one re-derived from the last finalized adjusted average (190 -> 27)");
            }
            finally
            {
                using NineTapDb db = TestDatabase.DbFactory.CreateDbContext();
                int[] tournamentIds = [finalizedTournamentId, openTournamentId];
                var participants = db.Participants
                    .Include(p => p.Game)
                    .Where(p => tournamentIds.Contains(p.Tournament.Id) || p.Member.Number == MemberNumber)
                    .ToList();
                db.Games.RemoveRange(participants.Select(p => p.Game));
                db.Participants.RemoveRange(participants);
                db.Tournaments.RemoveRange(db.Tournaments.Where(t => tournamentIds.Contains(t.Id)));
                db.Members.RemoveRange(db.Members.Where(m => m.Number == MemberNumber));
                db.SaveChanges();
            }
        }
    }
}
