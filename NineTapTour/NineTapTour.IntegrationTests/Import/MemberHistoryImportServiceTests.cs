using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Import;
using NineTapTour.Core.Models;
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

        private static void WriteGameRow(IXLWorksheet ws, int row, DateTime date, int g1, int g2, int g3, int? g4,
            string finPPHG = "1st", int? total = null)
        {
            int gameSum = g1 + g2 + g3 + (g4 ?? 0);

            ws.Cell(row, 1).Value = 4;                          // GameTotal
            ws.Cell(row, 2).Value = date;                       // Date
            ws.Cell(row, 3).Value = g1;                         // Game1
            ws.Cell(row, 4).Value = g2;                         // Game2
            ws.Cell(row, 5).Value = g3;                         // Game3
            if (g4.HasValue)
            {
                ws.Cell(row, 6).Value = g4.Value;               // Game4 (blank = not bowled)
            }
            ws.Cell(row, 7).Value = total ?? gameSum;           // Total (book total; best-3 for 3-of-4)
            ws.Cell(row, 8).Value = gameSum / 4.0;              // AverageOfRow
            ws.Cell(row, 9).Value = 185.5;                      // TrueAverage
            ws.Cell(row, 10).Value = 186;                       // AVG
            ws.Cell(row, 11).Value = 18;                        // Handicap
            ws.Cell(row, 12).Value = 2;                         // Bonus
            if (!string.IsNullOrEmpty(finPPHG))
            {
                ws.Cell(row, 14).Value = finPPHG;               // FinPPHG (place standing)
            }
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
                    Assert.IsTrue(importedTournaments.All(t => t.IsImported),
                        "history-import tournaments are flagged IsImported");
                    Assert.IsTrue(importedTournaments.All(t => !t.IsOnlyThreeGames && !t.ThreeOutOf4),
                        "4-game rows totaling all four games are a regular format");

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
                    Assert.AreEqual(1, secondDate.Game.PlaceStanding, "\"1st\" in the book parses to place 1");
                    Assert.IsFalse(secondDate.Game.IsComp,
                        "comp is a case-by-case designation, never derived from placing");
                    // 200+201+202+203 scratch + (18 handicap + 2 bonus) per counted game
                    Assert.AreEqual(806 + 20 * 4, secondDate.Game.HandicapTotal);

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
                    Assert.AreEqual(1, secondDate.Game.PlaceStanding,
                        "the re-run duplicate path rewrites PlaceStanding too");
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
        public void ImportFolder_DetectsTournamentFormats_ParsesPlaces_AndReportsCanExcludeImported()
        {
            MemberHistoryImportService service = CreateService();
            MemberRepository memberRepository = new(TestDatabase.DbFactory);
            ReportsRepository reportsRepository = new(TestDatabase.DbFactory);

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

            string folder = Path.Combine(Path.GetTempPath(), $"NineTapImport_{Guid.NewGuid():N}");
            Directory.CreateDirectory(folder);
            using (XLWorkbook workbook = new())
            {
                IXLWorksheet ws = workbook.AddWorksheet("Sheet1");
                ws.Cell(1, 2).Value = "Formats, Fred";
                ws.Cell(1, 10).Value = 150;
                ws.Cell(1, 14).Value = "Mem #901";

                // Three-game tournament: no 4th game anywhere; book total is the 3-game sum.
                WriteGameRow(ws, 3, new DateTime(2002, 3, 3), 180, 190, 200, null, finPPHG: "17th tie");
                WriteGameRow(ws, 4, new DateTime(2002, 3, 3), 150, 160, 170, null, finPPHG: "");
                // 3-of-4: four games recorded, book total counts only the best three (drops the 150).
                WriteGameRow(ws, 5, new DateTime(2002, 4, 4), 150, 200, 210, 220, finPPHG: "9thHM", total: 630);

                workbook.SaveAs(Path.Combine(folder, "member901.xlsx"));
            }

            try
            {
                memberRepository.AddOrUpdateMember(new Member
                {
                    Number = 901,
                    IsActive = true,
                    FirstName = "Fred",
                    LastName = "Formats",
                    MiddleInitial = "",
                    Gender = MemberGenders.Male,
                    Street = "2 Import St",
                    City = "Testville",
                    State = "WA",
                    PostalCode = "98000",
                    PrimaryPhone = "555-0901",
                    Average = 150,
                });

                ListProgress progress = new();
                ImportResult result = service.ImportFolder(folder, progress);

                Assert.AreEqual(3, result.Added);
                Assert.AreEqual(0, result.Warnings.Count);
                CollectionAssert.Contains(progress.Messages,
                    "Tournament formats detected: 1 three-game, 1 3-of-4.\r\n");

                using (NineTapDb db = TestDatabase.DbFactory.CreateDbContext())
                {
                    int memberId = db.Members.Single(m => m.Number == 901).Id;

                    Tournament threeGame = db.Tournaments.Single(t => t.Id > maxTournamentId && t.Date == new DateTime(2002, 3, 3));
                    Assert.IsTrue(threeGame.IsImported);
                    Assert.IsTrue(threeGame.IsOnlyThreeGames, "no 4th game in any entry means a 3-game tournament");
                    Assert.IsFalse(threeGame.ThreeOutOf4);

                    Tournament threeOf4 = db.Tournaments.Single(t => t.Id > maxTournamentId && t.Date == new DateTime(2002, 4, 4));
                    Assert.IsTrue(threeOf4.IsImported);
                    Assert.IsFalse(threeOf4.IsOnlyThreeGames);
                    Assert.IsTrue(threeOf4.ThreeOutOf4, "a book total of the best three games means 3-of-4");

                    List<Participant> participants = db.Participants
                        .Include(p => p.Game)
                        .Include(p => p.Tournament)
                        .Where(p => p.Member.Id == memberId)
                        .ToList();

                    Game placed17 = participants.Single(p => p.Tournament.Id == threeGame.Id && p.Squad == 1).Game;
                    Assert.AreEqual(17, placed17.PlaceStanding, "\"17th tie\" parses to place 17");

                    Game unplaced = participants.Single(p => p.Tournament.Id == threeGame.Id && p.Squad == 2).Game;
                    Assert.IsNull(unplaced.PlaceStanding, "a blank place cell stays null");
                    Assert.AreEqual(25.5m, unplaced.MoneyWon, "cash imports even without a recorded place");

                    Game bestThree = participants.Single(p => p.Tournament.Id == threeOf4.Id).Game;
                    // 630 best-3 scratch + (18 handicap + 2 bonus) per counted game (3 after the drop)
                    Assert.AreEqual(630 + 20 * 3, bestThree.HandicapTotal);
                    Assert.AreEqual(9, bestThree.PlaceStanding, "\"9thHM\" parses to place 9");
                    Assert.IsFalse(bestThree.UseGame1 ?? true, "the dropped lowest game is marked unused");
                    Assert.IsTrue(bestThree.UseGame2 ?? false);
                    Assert.IsTrue(bestThree.UseGame3 ?? false);
                    Assert.IsTrue(bestThree.UseGame4 ?? false);
                    Assert.AreEqual(630, bestThree.ScratchTotal, "ScratchTotal matches the book's best-3 total");
                }

                // Imported entries appear in reports by default and can be excluded.
                List<ReportGameEntry> withImported = reportsRepository.GetReportEntries(null, null, 901);
                Assert.AreEqual(3, withImported.Count);
                Assert.AreEqual(17, withImported.Single(e => e.TournamentDate == new DateTime(2002, 3, 3) && e.PlaceStanding != null).PlaceStanding);

                List<ReportGameEntry> withoutImported = reportsRepository.GetReportEntries(null, null, 901, includeImported: false);
                Assert.AreEqual(0, withoutImported.Count, "excluding imported history removes all legacy entries");
            }
            finally
            {
                using (NineTapDb db = TestDatabase.DbFactory.CreateDbContext())
                {
                    db.Participants.Where(p => p.Id > maxParticipantId).ExecuteDelete();
                    db.Games.Where(g => g.Id > maxGameId).ExecuteDelete();
                    db.Tournaments.Where(t => t.Id > maxTournamentId).ExecuteDelete();
                    db.Members.Where(m => m.Id > maxMemberId).ExecuteDelete();
                }
                Directory.Delete(folder, recursive: true);
            }
        }

        [TestMethod]
        public void ImportFolder_BadDatesAndJunkFile_WarnsAndContinues()
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

            string folder = Path.Combine(Path.GetTempPath(), $"NineTapImport_{Guid.NewGuid():N}");
            Directory.CreateDirectory(folder);

            // A file with the right extension that is not a workbook at all: it must
            // produce an error warning without aborting the rest of the folder.
            File.WriteAllText(Path.Combine(folder, "corrupt.xlsx"), "not an excel file");

            using (XLWorkbook workbook = new())
            {
                IXLWorksheet ws = workbook.AddWorksheet("Sheet1");
                ws.Cell(1, 2).Value = "Dates, Dana";
                ws.Cell(1, 10).Value = 150;
                ws.Cell(1, 14).Value = "Mem #902";

                WriteGameRow(ws, 3, new DateTime(2003, 1, 1), 180, 190, 200, 210);
                // Excel's rendering of an empty date-formatted cell (serial 0)
                WriteGameRow(ws, 4, new DateTime(1899, 12, 31), 150, 160, 170, 180);
                // Unreadable text date
                WriteGameRow(ws, 5, new DateTime(2003, 2, 2), 150, 160, 170, 180);
                ws.Cell(5, 2).Value = "garbage";

                workbook.SaveAs(Path.Combine(folder, "member902.xlsx"));
            }

            try
            {
                memberRepository.AddOrUpdateMember(new Member
                {
                    Number = 902,
                    IsActive = true,
                    FirstName = "Dana",
                    LastName = "Dates",
                    MiddleInitial = "",
                    Gender = MemberGenders.Female,
                    Street = "3 Import St",
                    City = "Testville",
                    State = "WA",
                    PostalCode = "98000",
                    PrimaryPhone = "555-0902",
                    Average = 150,
                });

                ListProgress progress = new();
                ImportResult result = service.ImportFolder(folder, progress);

                Assert.AreEqual(1, result.Added, "only the row with a plausible date imports");
                Assert.IsTrue(result.Warnings.Any(w => w.Contains("Failed to import corrupt.xlsx")),
                    "the unreadable workbook produces an error warning");
                Assert.IsTrue(result.Warnings.Any(w =>
                        w.Contains("member902.xlsx sheet 'Sheet1' row 4")
                        && w.Contains("implausible tournament date 12/31/1899")),
                    "the epoch date row is skipped with a warning");
                Assert.IsTrue(result.Warnings.Any(w =>
                        w.Contains("member902.xlsx sheet 'Sheet1' row 5")
                        && w.Contains("missing or unreadable date")),
                    "the text date row is skipped with a warning");

                using (NineTapDb db = TestDatabase.DbFactory.CreateDbContext())
                {
                    List<Tournament> importedTournaments = db.Tournaments
                        .Where(t => t.Id > maxTournamentId)
                        .ToList();
                    Assert.AreEqual(1, importedTournaments.Count,
                        "no tournaments are created for implausible or unreadable dates");
                    Assert.AreEqual(new DateTime(2003, 1, 1), importedTournaments[0].Date);
                }
            }
            finally
            {
                using (NineTapDb db = TestDatabase.DbFactory.CreateDbContext())
                {
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
