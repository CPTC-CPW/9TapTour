#nullable disable
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Export;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;

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
            rows.AddRange(ProcessExcelFile(files[i], progress, warnings));
        }

        return new ImportResult
        {
            Added = rows.Count,
            Warnings = warnings
        };
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

        // Create a single DbContext for the entire file import
        using (var db = dbFactory.CreateDbContext())
        {
            using (var workbook = new XLWorkbook(pathAndFileName))
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

                // If we couldn't extract player info, abort
                if (!playerInfoExtracted || playerNumberAsInt <= 0)
                {
                    throw new ArgumentException($"  ERROR: Could not extract valid player information from {Path.GetFileName(pathAndFileName)}\r\n");
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

                // Now process each worksheet with the extracted player info
                foreach (var ws in workbook.Worksheets)
                {
                    const int GameDataLastRow = 46;
                    const int GameDataStartRow = 3;

                    // PERFORMANCE: Track participant counts per tournament to avoid repeated DB queries.
                    // Keyed by the Tournament instance (reference equality), NOT by Id: tournaments
                    // newly added in this file all share Id 0 until SaveChanges runs, so an Id-keyed
                    // counter would number squads sequentially across different new tournaments.
                    Dictionary<Tournament, int> tournamentSquadCounts =
                        new Dictionary<Tournament, int>(ReferenceEqualityComparer.Instance);

                    for (int row = GameDataStartRow; row <= GameDataLastRow; row++)
                    {
                        ExcelRow temp = ExcelWorkbookReader.ReadHistoryRow(ws, row, playerFinalFirstAndMiddle,
                            playerLastName, playerOrgAVG, playerNumberAsInt);

                        if (temp == null)
                        {
                            // No game data in this row; skip it
                            continue;
                        }

                        DateTime rowDate = temp.Date.Date;
                        if (rowDate == DateTime.MinValue)
                        {
                            // Invalid date; skip this row
                            continue;
                        }

                        Tournament tourn = existingTournaments.FirstOrDefault(t => t.Date.Date == rowDate);
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
                            };

                            tournamentRepository.AddTournament(tourn, db);
                            existingTournaments.Add(tourn);
                        }

                        // Squad numbering is 1-based per player per tournament within this import run.
                        // Always start from 1 for the first entry read, regardless of any existing DB records.
                        int squadNumber;
                        if (!tournamentSquadCounts.ContainsKey(tourn))
                        {
                            tournamentSquadCounts[tourn] = 0;
                        }
                        tournamentSquadCounts[tourn]++;
                        squadNumber = tournamentSquadCounts[tourn];

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
                            IsComp = !string.IsNullOrWhiteSpace(temp.FinPPHG),
                            IsFinalized = true,
                            UseGame1 = temp.Game1 > -1 ? true : false,
                            UseGame2 = temp.Game2 > -1 ? true : false,
                            UseGame3 = temp.Game3 > -1 ? true : false,
                            UseGame4 = temp.Game4 > -1 ? true : false,

                            AdjustedAvg = temp.AVG,
                            KeepAdjustedAvg = true,
                            LeagueAverage = temp.TrueAverage,
                            HandicapTotal = temp.HandyCap,
                            // Place standing has many variations like (4th, 17th tie, 9thHM, and more)
                            // PlaceStanding = Convert.ToInt32(temp.FinPPHG),
                        };

                        // Only add the new Game to the context when this participant does not
                        // already exist. For duplicates, AddMemberToTournament updates the
                        // existing participant's game scores in place and this Game instance is
                        // discarded (never tracked), so re-running an import does not leave
                        // orphaned duplicate game rows.
                        bool participantExists = db.Participants
                            .AsNoTracking()
                            .Any(p => p.Member.Id == member.Id
                                   && p.Tournament.Id == tourn.Id
                                   && p.Squad == squadNumber);

                        if (!participantExists)
                        {
                            gameRepository.AddOrUpdateGame(game, db);
                        }

                        Participant participant = new Participant()
                        {
                            Squad = squadNumber,
                            Member = member,
                            Game = game,
                            Tournament = tourn
                        };

                        tournamentRepository.AddMemberToTournament(participant, db);

                        returnMe.Add(temp);
                    }
                }

                db.SaveChanges();
                progress?.Report($"  File complete: {returnMe.Count} records saved.\r\n");
                return returnMe;
            }
        }
    }
}
