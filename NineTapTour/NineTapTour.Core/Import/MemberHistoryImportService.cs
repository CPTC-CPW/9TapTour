#nullable disable
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Calculations;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Export;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
using System.Text.RegularExpressions;

namespace NineTapTour.Core.Import;

/// <summary>
/// Bulk legacy history import moved verbatim from MemberImportTest.FrmMain
/// (GetAllExcelData / ProcessExcelFile / ExtractPlayerInfoFromWorksheet). Each
/// file is imported through a single DbContext created from the factory, using
/// the repositories' shared-context overloads, and saved once per file.
/// </summary>
public class MemberHistoryImportService : IMemberHistoryImportService
{
    private readonly IMemberRepository memberRepository;
    private readonly IGameRepository gameRepository;
    private readonly ITournamentRepository tournamentRepository;
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public MemberHistoryImportService(IMemberRepository memberRepository, IGameRepository gameRepository,
        ITournamentRepository tournamentRepository, IDbContextFactory<NineTapDb> dbFactory)
    {
        this.memberRepository = memberRepository;
        this.gameRepository = gameRepository;
        this.tournamentRepository = tournamentRepository;
        this.dbFactory = dbFactory;
    }

    /// <inheritdoc />
    public ImportResult ImportFolder(string folderPath, IProgress<string> progress)
    {
        string[] files = Directory.GetFiles(folderPath);
        return ImportFiles(files, progress);
    }

    /// <inheritdoc />
    public ImportResult ImportFiles(string[] files, IProgress<string> progress)
    {
        List<ExcelRow> rows = [];
        List<string> warnings = [];
        for (int i = 0; i < files.Length; i++)
        {
            // If the file is not an excel file, skip it
            if (!FileHelper.IsValidExcelExtension(Path.GetExtension(files[i])))
            {
                continue;
            }
            progress?.Report($"Processing: {Path.GetFileName(files[i])}\r\n");
            try
            {
                rows.AddRange(ProcessExcelFile(files[i], progress, warnings));
            }
            catch (Exception ex)
            {
                // One unreadable workbook must not abort the rest of the folder.
                string warning = $"  ERROR: Failed to import {Path.GetFileName(files[i])}: {ex.Message}\r\n";
                progress?.Report(warning);
                warnings.Add(warning);
            }
        }

        DetectImportedTournamentFormats(progress);

        return new ImportResult
        {
            Added = rows.Count,
            Warnings = warnings
        };
    }

    /// <summary>
    /// Sanity window for imported tournament dates. Legacy books predate the
    /// tour software but not 1980, and imported tournaments are always in the
    /// past. Dates outside this window are almost always artifacts of bad
    /// cells: Excel's zero serial ("1/0/1900") loads as 1899-12-31, and a
    /// time-only cell lands on the same epoch.
    /// </summary>
    internal static bool IsPlausibleTournamentDate(DateTime date)
    {
        return date >= new DateTime(1980, 1, 1) && date.Date <= DateTime.Today;
    }

    /// <summary>
    /// Largest number read as a place. Bigger values are year labels the books
    /// write in the place column next to carried-forward totals ("2005/06").
    /// </summary>
    private const int MaxLegacyPlace = 999;

    /// <summary>
    /// Parses a legacy place-standing cell into a numeric place. The books use
    /// many variations ("4th", "17th tie", "9thHM", ...), so only the leading
    /// digits are taken. Blank cells (the bowler did not place) and year labels
    /// ("2005", "2005/06") return null.
    /// </summary>
    internal static int? ParseLegacyPlaceStanding(string finPPHG)
    {
        if (string.IsNullOrWhiteSpace(finPPHG))
            return null;

        Match match = RegexHelpers.LeadingDigits().Match(finPPHG);
        return match.Success && int.TryParse(match.Groups[1].Value, out int place)
                && place > 0 && place <= MaxLegacyPlace
            ? place
            : null;
    }

    /// <summary>
    /// Finds the entry a re-run should update instead of inserting: an entry for
    /// the same member and tournament with the same four game scores that no other
    /// row of this file has already claimed. Matching on scores rather than squad
    /// number keeps re-runs correct for books whose entries were imported with
    /// colliding squad numbers (older imports restarted numbering on every sheet).
    /// </summary>
    internal static Participant FindMatchingImportedEntry(IEnumerable<Participant> existingEntries,
        Game game, ISet<int> claimedParticipantIds)
    {
        return existingEntries
            .Where(p => !claimedParticipantIds.Contains(p.Id))
            .OrderBy(p => p.Squad)
            .FirstOrDefault(p => p.Game != null
                && p.Game.Game1 == game.Game1
                && p.Game.Game2 == game.Game2
                && p.Game.Game3 == game.Game3
                && p.Game.Game4 == game.Game4);
    }

    /// <summary>
    /// Legacy 3-of-4 books record four game scores but total only the best three.
    /// When the row's book total equals the best-3 sum (and differs from the
    /// 4-game sum), the lowest game is marked unused so ScratchTotal matches the
    /// book. On a tie for lowest, only the first is dropped.
    /// </summary>
    internal static void ApplyBestThreeOfFourDropIfDetected(Game game, int bookTotal)
    {
        if (bookTotal < 0 || !game.Game1.HasValue || !game.Game2.HasValue
            || !game.Game3.HasValue || !game.Game4.HasValue)
        {
            return;
        }

        int[] scores = [game.Game1.Value, game.Game2.Value, game.Game3.Value, game.Game4.Value];
        int lowest = scores.Min();

        // A lowest game of 0 makes the best-3 and 4-game sums identical, so a
        // drop is undetectable (and dropping would not change the total anyway).
        if (lowest == 0 || bookTotal != scores.Sum() - lowest)
        {
            return;
        }

        switch (Array.IndexOf(scores, lowest))
        {
            case 0: game.UseGame1 = false; break;
            case 1: game.UseGame2 = false; break;
            case 2: game.UseGame3 = false; break;
            case 3: game.UseGame4 = false; break;
        }
    }

    /// <summary>
    /// Infers the format of every imported tournament from the games recorded
    /// across all imported books: a tournament with scores but no 4th game
    /// anywhere is a 3-game tournament, and one where any complete row dropped
    /// its lowest game (see <see cref="ApplyBestThreeOfFourDropIfDetected"/>) is
    /// 3-of-4. Recomputed over all imported tournaments after each import run so
    /// later files (other players' books) and re-runs keep refining the flags.
    /// Doubles cannot be detected from single-player books and stays false.
    /// </summary>
    private void DetectImportedTournamentFormats(IProgress<string> progress)
    {
        using var db = dbFactory.CreateDbContext();

        var rowsByTournament = db.Participants
            .Where(p => p.Tournament.IsImported)
            .Select(p => new
            {
                TournamentId = p.Tournament.Id,
                p.Game.Game1,
                p.Game.Game2,
                p.Game.Game3,
                p.Game.Game4,
                p.Game.UseGame1,
                p.Game.UseGame2,
                p.Game.UseGame3,
                p.Game.UseGame4,
            })
            .ToList()
            .GroupBy(r => r.TournamentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        int threeGameCount = 0;
        int threeOutOf4Count = 0;
        foreach (Tournament tourn in db.Tournaments.Where(t => t.IsImported))
        {
            if (!rowsByTournament.TryGetValue(tourn.Id, out var games))
            {
                continue;
            }

            bool anyScores = games.Any(g =>
                g.Game1.HasValue || g.Game2.HasValue || g.Game3.HasValue || g.Game4.HasValue);

            tourn.IsOnlyThreeGames = anyScores && !games.Any(g => g.Game4.HasValue);
            tourn.ThreeOutOf4 = games.Any(g =>
                g.Game1.HasValue && g.Game2.HasValue && g.Game3.HasValue && g.Game4.HasValue
                && (g.UseGame1 == false || g.UseGame2 == false || g.UseGame3 == false || g.UseGame4 == false));

            if (tourn.IsOnlyThreeGames)
            {
                threeGameCount++;
            }
            if (tourn.ThreeOutOf4)
            {
                threeOutOf4Count++;
            }
        }

        db.SaveChanges();
        progress?.Report($"Tournament formats detected: {threeGameCount} three-game, {threeOutOf4Count} 3-of-4.\r\n");
    }

    /// <summary>
    /// This will process the actual excel files and impport the info needed from the files to the program
    /// NOTE: This is currently set up for the old format. New format has not yet been implemented.
    /// </summary>
    private List<ExcelRow> ProcessExcelFile(string pathAndFileName, IProgress<string> progress, List<string> warnings)
    {
        progress?.Report($"Current File Being Processed: {Path.GetFileName(pathAndFileName)}\r\n");

        List<ExcelRow> returnMe = new List<ExcelRow>();
        char[] splitters = new[] { '/', '-' };

        // Create a single DbContext for the entire file import.
        // The file stream is opened here rather than letting XLWorkbook open the
        // path: when the workbook constructor throws on a corrupt file, ClosedXML
        // leaks its internal stream and the file stays locked on disk.
        using (var db = dbFactory.CreateDbContext())
        using (var fileStream = File.OpenRead(pathAndFileName))
        {
            using (var workbook = new XLWorkbook(fileStream))
            {
                // Extract player information ONCE from the first worksheet that has it
                string[] playerFinalFirstAndMiddle = new[] { "", "" };
                string playerLastName = "";
                int playerOrgAVG = -1;
                int playerNumberAsInt = 0;

                bool playerInfoExtracted = false;

                // Find and extract player information from first worksheet with data
                foreach (var ws in workbook.Worksheets)
                {
                    if (!playerInfoExtracted)
                    {
                        ExcelWorkbookReader.ExtractHistoryPlayerInfo(ws, ref playerFinalFirstAndMiddle,
                            ref playerLastName, ref playerOrgAVG, ref playerNumberAsInt, splitters);

                        if (playerNumberAsInt > 0)
                        {
                            playerInfoExtracted = true;
                        }
                    }
                }

                // If we couldn't extract player info, skip this file (warn, don't throw:
                // the rest of the folder should still import)
                if (!playerInfoExtracted || playerNumberAsInt <= 0)
                {
                    string headerWarning = $"  ERROR: Could not extract valid player information from {Path.GetFileName(pathAndFileName)}; file skipped.\r\n";
                    progress?.Report(headerWarning);
                    warnings.Add(headerWarning);
                    return returnMe;
                }

                // Load existing tournaments once for the entire workbook
                List<Tournament> existingTournaments = tournamentRepository.GetTournamentList(db);

                // PERFORMANCE: Look up member once per file instead of per row
                var member = memberRepository.GetMember(playerNumberAsInt, db);
                if (member == null || member.IsActive != true)
                {
                    string warning = $"  WARNING: Member #{playerNumberAsInt} not found or inactive. Skipping file.\r\n";
                    progress?.Report(warning);
                    warnings.Add(warning);
                    return returnMe;
                }

                // Last squad number used per tournament for this member, across every
                // page of the book: a tournament can span two pages, and numbering
                // restarted on each page used to give both entries squad 1. Keyed by
                // the Tournament instance (reference equality), NOT by Id: tournaments
                // newly added in this file all share Id 0 until SaveChanges runs.
                Dictionary<Tournament, int> lastSquadByTournament =
                    new Dictionary<Tournament, int>(ReferenceEqualityComparer.Instance);

                // Existing entries already updated by an earlier row of this file, so two
                // rows with the same scores on the same date never update the same entry.
                HashSet<int> claimedParticipantIds = [];

                // Pages run newest (first tab) to oldest (last tab).
                List<IXLWorksheet> pages = [.. workbook.Worksheets];

                // Cash on rows with no entry is replaced on every run: drop this book's
                // previously imported rows (rows added by hand are kept). Saved with the
                // rest of the file, so a failed import leaves the old rows in place.
                string fileName = Path.GetFileName(pathAndFileName);
                string sourcePrefix = fileName + ", ";
                db.OtherEarnings.RemoveRange(db.OtherEarnings
                    .Where(o => o.Member.Id == member.Id && o.IsImported && o.Source.StartsWith(sourcePrefix)));
                int otherEarningsCount = 0;
                decimal otherEarningsTotal = 0;

                // Now process each worksheet with the extracted player info
                for (int pageIndex = 0; pageIndex < pages.Count; pageIndex++)
                {
                    const int GameDataLastRow = 46;
                    const int GameDataStartRow = 3;

                    IXLWorksheet ws = pages[pageIndex];

                    // Every page but the oldest starts with the career cash carried
                    // forward from the page before it; that balance is not winnings.
                    bool isOldestPage = pageIndex == pages.Count - 1;
                    int? carryForwardRow = isOldestPage ? null : ExcelWorkbookReader.FindCashCarryForwardRow(ws);

                    // Without the page's CASH total the carried-forward balance cannot be
                    // told apart from real cash, so no cash-only rows are read from it.
                    bool canReadOtherEarnings = isOldestPage || carryForwardRow.HasValue;
                    if (!canReadOtherEarnings)
                    {
                        string totalWarning = $"  WARNING: {fileName} sheet '{ws.Name}': no CASH total formula found; cash on rows without an entry was not imported.\r\n";
                        progress?.Report(totalWarning);
                        warnings.Add(totalWarning);
                    }

                    for (int row = GameDataStartRow; row <= GameDataLastRow; row++)
                    {
                        ExcelRow temp = ExcelWorkbookReader.ReadHistoryRow(ws, row, playerFinalFirstAndMiddle,
                            playerLastName, playerOrgAVG, playerNumberAsInt);

                        // A game can share the carry-forward row; keep the game, drop the balance.
                        if (temp != null && row == carryForwardRow)
                        {
                            temp.Cash = 0;
                        }

                        if (temp == null)
                        {
                            // Cash with no entry (career totals from before the book,
                            // satellites, Yakima, Hawaii, ...) counts toward lifetime cash.
                            OtherEarningRow other = canReadOtherEarnings && row != carryForwardRow
                                ? ExcelWorkbookReader.ReadOtherEarningRow(ws, row)
                                : null;
                            if (other != null)
                            {
                                db.OtherEarnings.Add(new OtherEarning
                                {
                                    Member = member,
                                    Amount = other.Amount,
                                    Description = other.Description,
                                    Date = other.Date.HasValue && IsPlausibleTournamentDate(other.Date.Value)
                                        ? other.Date
                                        : null,
                                    IsImported = true,
                                    Source = $"{sourcePrefix}{ws.Name}, row {row}",
                                });
                                otherEarningsCount++;
                                otherEarningsTotal += other.Amount;
                            }

                            // Blank filler rows are normal; only warn when the row has a
                            // date (so it looks like a data row) but the games-played
                            // cell (column 1) could not be read.
                            if (!ws.Cell(row, 1).IsEmpty() && !ws.Cell(row, 2).IsEmpty())
                            {
                                string rowWarning = $"  WARNING: {Path.GetFileName(pathAndFileName)} sheet '{ws.Name}' row {row}: unreadable games-played value; entry skipped.\r\n";
                                progress?.Report(rowWarning);
                                warnings.Add(rowWarning);
                            }
                            continue;
                        }

                        DateTime rowDate = temp.Date.Date;
                        if (rowDate == DateTime.MinValue)
                        {
                            string dateWarning = $"  WARNING: {Path.GetFileName(pathAndFileName)} sheet '{ws.Name}' row {row}: missing or unreadable date; entry skipped.\r\n";
                            progress?.Report(dateWarning);
                            warnings.Add(dateWarning);
                            continue;
                        }

                        if (!IsPlausibleTournamentDate(rowDate))
                        {
                            // Excel renders an empty date-formatted cell as serial 0
                            // ("1/0/1900"), which loads as 1899-12-31 and would
                            // otherwise create a junk tournament on that date.
                            string dateWarning = $"  WARNING: {Path.GetFileName(pathAndFileName)} sheet '{ws.Name}' row {row}: implausible tournament date {rowDate:M/d/yyyy}; entry skipped.\r\n";
                            progress?.Report(dateWarning);
                            warnings.Add(dateWarning);
                            continue;
                        }

                        Tournament tourn = existingTournaments.FirstOrDefault(t => t.IsImported && t.Date.Date == rowDate);
                        if (tourn == null)
                        {
                            tourn = new Tournament()
                            {
                                Date = rowDate,
                                Location = "Imported",
                                Event = $"Imported Tourney - {rowDate}",
                                Notes = string.Empty,
                                Sponsors = string.Empty,
                                Squads = 4,
                                Doubles = false,
                                ThreeOutOf4 = false,
                                IsOnlyThreeGames = false,
                                IsImported = true,
                            };

                            tournamentRepository.AddTournament(tourn, db);
                            existingTournaments.Add(tourn);
                        }

                        // There are some cases where an entire entry will be all null games
                        // this is due to tournament conditions such as invalid lane oilings.
                        // The tournament is valid but none of the scores are counted due to inflated numbers.
                        Game game = new Game()
                        {
                            Game1 = temp.Game1 > -1 ? temp.Game1 : null,
                            Game2 = temp.Game2 > -1 ? temp.Game2 : null,
                            Game3 = temp.Game3 > -1 ? temp.Game3 : null,
                            Game4 = temp.Game4 > -1 ? temp.Game4 : null,
                            TotalScore = temp.Total > -1 ? temp.Total : null,
                            Handicap = temp.HandyCap > -1 ? temp.HandyCap : 0,
                            Bonus = temp.Bonus > -1 ? temp.Bonus : 0,
                            MoneyWon = Convert.ToDecimal(temp.Cash),
                            Notes = temp.Notes,
                            // Comp (bowling free as tournament help) is a case-by-case
                            // designation that the legacy books do not record.
                            IsComp = false,
                            IsFinalized = true,
                            UseGame1 = temp.Game1 > -1 ? true : false,
                            UseGame2 = temp.Game2 > -1 ? true : false,
                            UseGame3 = temp.Game3 > -1 ? true : false,
                            UseGame4 = temp.Game4 > -1 ? true : false,

                            AdjustedAvg = temp.AVG,
                            KeepAdjustedAvg = true,
                            LeagueAverage = temp.TrueAverage,
                            // Place standing has many variations like (4th, 17th tie, 9thHM, and more),
                            // so only the leading digits are taken; blank cells (did not place) stay null.
                            PlaceStanding = ParseLegacyPlaceStanding(temp.FinPPHG),
                        };

                        ApplyBestThreeOfFourDropIfDetected(game, temp.Total);

                        // Handicap and bonus apply per counted game, so this must be
                        // computed after any 3-of-4 drop reduces GamesPlayed.
                        game.HandicapTotal = game.ScratchTotal
                            + ((game.Handicap ?? 0) + (game.Bonus ?? 0)) * game.GamesPlayed;

                        // Entries already saved for this member and tournament (by an earlier
                        // run). Rows added earlier in this file are not saved yet, so they
                        // are never in this list.
                        List<Participant> existingEntries = db.Participants
                            .Include(p => p.Game)
                            .Where(p => p.Member.Id == member.Id && p.Tournament.Id == tourn.Id)
                            .ToList();

                        Participant existing = FindMatchingImportedEntry(existingEntries, game, claimedParticipantIds);
                        if (existing != null)
                        {
                            // Re-run: update the entry in place so corrected parsing (cash,
                            // place, 3-of-4 drops) reaches rows imported by an older version,
                            // and this Game instance is discarded (never tracked).
                            claimedParticipantIds.Add(existing.Id);
                            Game existingGame = existing.Game;
                            existingGame.MoneyWon = game.MoneyWon;
                            existingGame.IsComp = game.IsComp;
                            existingGame.PlaceStanding = game.PlaceStanding;
                            existingGame.UseGame1 = game.UseGame1;
                            existingGame.UseGame2 = game.UseGame2;
                            existingGame.UseGame3 = game.UseGame3;
                            existingGame.UseGame4 = game.UseGame4;
                            existingGame.HandicapTotal = game.HandicapTotal;
                        }
                        else
                        {
                            // New entry: next squad number after any already used for this
                            // member in this tournament, whether saved or added in this file.
                            if (!lastSquadByTournament.TryGetValue(tourn, out int lastSquad))
                            {
                                lastSquad = existingEntries.Count == 0 ? 0 : existingEntries.Max(p => p.Squad);
                            }
                            lastSquad++;
                            lastSquadByTournament[tourn] = lastSquad;

                            gameRepository.AddOrUpdateGame(game, db);

                            Participant participant = new Participant()
                            {
                                Squad = lastSquad,
                                Member = member,
                                Game = game,
                                Tournament = tourn
                            };

                            tournamentRepository.AddMemberToTournament(participant, db);
                        }

                        returnMe.Add(temp);
                    }
                }

                db.SaveChanges();
                if (otherEarningsCount > 0)
                {
                    progress?.Report($"  Other earnings (cash with no entry): {otherEarningsCount} rows, {otherEarningsTotal:C}.\r\n");
                }
                progress?.Report($"  File complete: {returnMe.Count} records saved.\r\n");
                return returnMe;
            }
        }
    }
}
