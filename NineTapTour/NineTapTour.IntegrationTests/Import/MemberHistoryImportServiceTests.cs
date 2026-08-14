using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Import;
using NineTapTour.Core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NineTapTour.IntegrationTests.Import
{
    /// <summary>
    /// End-to-end test of the bulk history import against the seeded LocalDB
    /// catalog. Workbooks are generated on the fly (no binary fixtures) and use
    /// member numbers >= 900 plus tournament dates that do not collide with the
    /// seeded golden-master data. Every row this test inserts is removed again
    /// in the finally block so the whole-database assertions in other
    /// integration tests (e.g. the backup/restore round trip) keep holding.
    /// </summary>
    [TestClass]
    public class MemberHistoryImportServiceTests
    {
        private sealed class ListProgress : IProgress<string>
        {
            public List<string> Messages { get; } = [];

            public void Report(string value)
            {
                Messages.Add(value);
            }
        }

        private static MemberHistoryImportService CreateService()
        {
            return new MemberHistoryImportService(
                new MemberRepository(TestDatabase.DbFactory),
                new GameRepository(TestDatabase.DbFactory),
                new TournamentRepository(TestDatabase.DbFactory),
                TestDatabase.DbFactory);
        }

        private static string CreateWorkbookFolder(string fileName, int memberNumber)
        {
            string folder = Path.Combine(Path.GetTempPath(), $"NineTapImport_{Guid.NewGuid():N}");
            Directory.CreateDirectory(folder);

            using XLWorkbook workbook = new();
            IXLWorksheet ws = workbook.AddWorksheet("Sheet1");
            ws.Cell(1, 2).Value = "Importer, Ivy B";
            ws.Cell(1, 10).Value = 150;
            ws.Cell(1, 14).Value = $"Mem #{memberNumber}";

            WriteGameRow(ws, 3, new DateTime(2001, 5, 5), 180, 190, 200, 210);
            WriteGameRow(ws, 4, new DateTime(2001, 5, 5), 150, 160, 170, 180);
            WriteGameRow(ws, 5, new DateTime(2001, 6, 6), 200, 201, 202, 203);

            workbook.SaveAs(Path.Combine(folder, fileName));
            return folder;
        }

        private static void WriteGameRow(IXLWorksheet ws, int row, DateTime date, int g1, int g2, int g3, int g4)
        {
            ws.Cell(row, 1).Value = 4;                          // GameTotal
            ws.Cell(row, 2).Value = date;                       // Date
            ws.Cell(row, 3).Value = g1;                         // Game1
            ws.Cell(row, 4).Value = g2;                         // Game2
            ws.Cell(row, 5).Value = g3;                         // Game3
            ws.Cell(row, 6).Value = g4;                         // Game4
            ws.Cell(row, 7).Value = g1 + g2 + g3 + g4;          // Total
            ws.Cell(row, 8).Value = (g1 + g2 + g3 + g4) / 4.0;  // AverageOfRow
            ws.Cell(row, 9).Value = 185.5;                      // TrueAverage
            ws.Cell(row, 10).Value = 186;                       // AVG
            ws.Cell(row, 11).Value = 18;                        // Handicap
            ws.Cell(row, 12).Value = 2;                         // Bonus
            ws.Cell(row, 14).Value = "1st";                     // FinPPHG
            ws.Cell(row, 15).Value = 25.5;                      // Cash
            ws.Cell(row, 16).Value = "imported";                // Notes
        }

        [TestMethod]
        public void ImportFolder_CreatesHistory_AndReRunIsIdempotent()
        {
            MemberHistoryImportService service = CreateService();
            MemberRepository memberRepository = new(TestDatabase.DbFactory);

            int maxMemberId;
            int maxTournamentId;
            int maxGameId;
            int maxParticipantId;
            using (NineTapDb db = TestDatabase.DbFactory.CreateDbContext())
            {
                maxMemberId = db.Members.Max(m => (int?)m.Id) ?? 0;
                maxTournamentId = db.Tournaments.Max(t => (int?)t.Id) ?? 0;
                maxGameId = db.Games.Max(g => (int?)g.Id) ?? 0;
                maxParticipantId = db.Participants.Max(p => (int?)p.Id) ?? 0;
            }

            string folder = CreateWorkbookFolder("member900.xlsx", 900);
            try
            {
                memberRepository.AddOrUpdateMember(new Member
                {
                    Number = 900,
                    IsActive = true,
                    FirstName = "Ivy",
                    LastName = "Importer",
                    MiddleInitial = "",
                    Gender = MemberGenders.Female,
                    Street = "1 Import St",
                    City = "Testville",
                    State = "WA",
                    PostalCode = "98000",
                    PrimaryPhone = "555-0900",
                    Average = 150,
                });

                ListProgress progress = new();
                ImportResult first = service.ImportFolder(folder, progress);

                Assert.AreEqual(3, first.Added);
                Assert.AreEqual(0, first.Warnings.Count);
                CollectionAssert.Contains(progress.Messages, "Processing: member900.xlsx\r\n");
                CollectionAssert.Contains(progress.Messages, "Current File Being Processed: member900.xlsx\r\n");
                CollectionAssert.Contains(progress.Messages, "  File complete: 3 records saved.\r\n");

                int gamesAfterFirst;
                using (NineTapDb db = TestDatabase.DbFactory.CreateDbContext())
                {
                    int memberId = db.Members.Single(m => m.Number == 900).Id;

                    List<Tournament> importedTournaments = db.Tournaments
                        .Where(t => t.Id > maxTournamentId)
                        .OrderBy(t => t.Date)
                        .ToList();
                    Assert.AreEqual(2, importedTournaments.Count, "one tournament per unique date");
                    Assert.AreEqual(new DateTime(2001, 5, 5), importedTournaments[0].Date);
                    Assert.AreEqual(new DateTime(2001, 6, 6), importedTournaments[1].Date);
                    Assert.AreEqual("Imported", importedTournaments[0].Location);

                    List<Participant> participants = db.Participants
                        .Include(p => p.Game)
                        .Include(p => p.Tournament)
                        .Where(p => p.Member.Id == memberId)
                        .ToList();
                    Assert.AreEqual(3, participants.Count);

                    List<int> squadsFirstDate = participants
                        .Where(p => p.Tournament.Id == importedTournaments[0].Id)
                        .Select(p => p.Squad)
                        .OrderBy(s => s)
                        .ToList();
                    CollectionAssert.AreEqual(new List<int> { 1, 2 }, squadsFirstDate,
                        "two entries on the same date get squads 1 and 2");

                    // The squad counter is keyed by the Tournament instance, so
                    // numbering restarts at 1 for each tournament even when the
                    // tournaments are brand new (all Id 0) in the same file. The
                    // single entry on the second date therefore gets squad 1.
                    Participant secondDate = participants.Single(p => p.Tournament.Id == importedTournaments[1].Id);
                    Assert.AreEqual(1, secondDate.Squad);
                    Assert.AreEqual(200, secondDate.Game.Game1);
                    Assert.AreEqual(203, secondDate.Game.Game4);
                    Assert.IsTrue(secondDate.Game.IsFinalized);

                    gamesAfterFirst = db.Games.Count(g => g.Id > maxGameId);
                    Assert.AreEqual(3, gamesAfterFirst);
                }

                // Re-run the same import: it must be idempotent. Squad numbers
                // are computed the same way on both runs (per-tournament 1..n),
                // so every row matches an existing participant, the existing
                // games get their scores rewritten in place, and no new
                // tournaments, games, or participants are inserted. Rows are
                // still parsed and reported, so Added and the progress messages
                // are unchanged from the first run.
                ListProgress secondProgress = new();
                ImportResult second = service.ImportFolder(folder, secondProgress);
                Assert.AreEqual(3, second.Added, "rows are still parsed and reported on a re-run");
                Assert.AreEqual(0, second.Warnings.Count);
                CollectionAssert.Contains(secondProgress.Messages, "  File complete: 3 records saved.\r\n");

                using (NineTapDb db = TestDatabase.DbFactory.CreateDbContext())
                {
                    int memberId = db.Members.Single(m => m.Number == 900).Id;
                    Assert.AreEqual(2, db.Tournaments.Count(t => t.Id > maxTournamentId),
                        "re-run reuses the existing tournaments");
                    Assert.AreEqual(3, db.Participants.Count(p => p.Member.Id == memberId),
                        "re-run must not insert duplicate participants");
                    Assert.AreEqual(gamesAfterFirst, db.Games.Count(g => g.Id > maxGameId),
                        "re-run must not insert duplicate games");

                    // The skipped rows update the existing games in place, so the
                    // scores and squad numbers are unchanged after the re-run.
                    List<Tournament> importedTournaments = db.Tournaments
                        .Where(t => t.Id > maxTournamentId)
                        .OrderBy(t => t.Date)
                        .ToList();
                    Participant secondDate = db.Participants
                        .Include(p => p.Game)
                        .Single(p => p.Member.Id == memberId && p.Tournament.Id == importedTournaments[1].Id);
                    Assert.AreEqual(1, secondDate.Squad);
                    Assert.AreEqual(200, secondDate.Game.Game1);
                    Assert.AreEqual(203, secondDate.Game.Game4);
                }
            }
            finally
            {
                using (NineTapDb db = TestDatabase.DbFactory.CreateDbContext())
                {
                    // Remove everything this test inserted so whole-database
                    // assertions in the other integration tests keep holding.
                    db.Participants.Where(p => p.Id > maxParticipantId).ExecuteDelete();
                    db.Games.Where(g => g.Id > maxGameId).ExecuteDelete();
                    db.Tournaments.Where(t => t.Id > maxTournamentId).ExecuteDelete();
                    db.Members.Where(m => m.Id > maxMemberId).ExecuteDelete();
                }
                Directory.Delete(folder, recursive: true);
            }
        }

        [TestMethod]
        public void ImportFolder_UnknownMember_ReportsWarningAndImportsNothing()
        {
            MemberHistoryImportService service = CreateService();
            string folder = CreateWorkbookFolder("member998.xlsx", 998);
            try
            {
                int tournamentsBefore;
                using (NineTapDb db = TestDatabase.DbFactory.CreateDbContext())
                {
                    tournamentsBefore = db.Tournaments.Count();
                }

                ListProgress progress = new();
                ImportResult result = service.ImportFolder(folder, progress);

                Assert.AreEqual(0, result.Added);
                Assert.AreEqual(1, result.Warnings.Count);
                Assert.AreEqual("  WARNING: Member #998 not found or inactive. Skipping file.\r\n", result.Warnings[0]);
                CollectionAssert.Contains(progress.Messages, result.Warnings[0]);

                using (NineTapDb db = TestDatabase.DbFactory.CreateDbContext())
                {
                    Assert.AreEqual(tournamentsBefore, db.Tournaments.Count(),
                        "a skipped file must not create tournaments");
                }
            }
            finally
            {
                Directory.Delete(folder, recursive: true);
            }
        }
    }
}
