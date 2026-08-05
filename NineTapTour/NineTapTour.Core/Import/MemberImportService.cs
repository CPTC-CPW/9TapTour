#nullable disable
using ClosedXML.Excel;
using NineTapTour.Core.Calculations;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;

namespace NineTapTour.Core.Import;

/// <summary>
/// Single-member Excel history import moved verbatim from
/// FrmMemberData.BtnImportData_Click / ProcessExcelFile. The form is now a thin
/// shell that shows the messages this service reports.
/// </summary>
public class MemberImportService : IMemberImportService
{
    /// <summary>
    /// Warning reported when the member already has imported player history.
    /// </summary>
    public const string HistoryAlreadyImportedWarning = "Member history has already been imported";

    private readonly IMemberRepository memberRepository;
    private readonly ITournamentRepository tournamentRepository;
    private readonly IPlayerHistoryRepository playerHistoryRepository;

    public MemberImportService(IMemberRepository memberRepository, ITournamentRepository tournamentRepository,
        IPlayerHistoryRepository playerHistoryRepository)
    {
        this.memberRepository = memberRepository;
        this.tournamentRepository = tournamentRepository;
        this.playerHistoryRepository = playerHistoryRepository;
    }

    /// <inheritdoc />
    public ImportResult ImportMemberHistory(string pathAndFileName, Member member)
    {
        List<PlayerHistoryViewModel> alreadyImportedPH =
            playerHistoryRepository.GetMemberPlayerHistory(member.Number);

        if (alreadyImportedPH.Count > 0)
        {
            return new ImportResult
            {
                Skipped = 1,
                Warnings = [HistoryAlreadyImportedWarning]
            };
        }

        // Process the Excel file and create tournaments/participants
        List<ExcelRow> rows = ProcessExcelFile(pathAndFileName, member);

        // Update member's averages after import
        double? mostRecentTrueAverage = null;
        PlayerHistoryViewModel reset = playerHistoryRepository.GetMostRecentTournament(member.Number);
        if (reset != null)
        {
            member.Average = reset.AVG;
            member.Handicap = TournamentCalculations
                .CalculateHandicapPins(Convert.ToInt32(member.Average));

            member.Bonus = reset.Bonus;
            mostRecentTrueAverage = reset.trueAVG;
        }

        // Grabs the total money won by the member
        decimal moneySum = playerHistoryRepository.GetTotalMoneyWon(member.Number);

        member.MoneyEarned += moneySum;

        memberRepository.AddOrUpdateMember(member);

        return new ImportResult
        {
            Added = rows.Count,
            MostRecentTrueAverage = mostRecentTrueAverage
        };
    }

    /// <summary>
    /// Processes excel file for member data import
    /// Creates tournaments for each unique date and links games to member through participants
    /// </summary>
    private List<ExcelRow> ProcessExcelFile(string pathAndFileName, Member member)
    {
        List<ExcelRow> returnMe = [];
        // Dictionary to track tournaments by date
        Dictionary<DateTime, Tournament> tournamentsCache = [];

        using (var workbook = new XLWorkbook(pathAndFileName))
        {
            var ws = workbook.Worksheet(1);
            ExcelPlayerHeader player = ExcelWorkbookReader.ReadMemberDataHeader(ws);

            int rowNum = 3;
            int lastRow = ws.LastRowUsed().RowNumber();

            for (int row = rowNum; row <= lastRow; row++)
            {
                ExcelRow temp = ExcelWorkbookReader.ReadMemberDataRow(ws, row, player);

                // Create or get tournament for this date
                Tournament tournament;
                DateTime tournamentDate = temp.Date.Date; // Normalize to date only

                if (!tournamentsCache.ContainsKey(tournamentDate))
                {
                    // Check if tournament already exists in database
                    List<Tournament> existingTournaments = tournamentRepository.GetTournamentList()
                        .Where(t => t.Date.Date == tournamentDate).ToList();

                    if (existingTournaments.Count > 0)
                    {
                        tournament = existingTournaments[0];
                    }
                    else
                    {
                        // Create new tournament for this date
                        tournament = new Tournament
                        {
                            Date = tournamentDate,
                            Location = $"Imported - {tournamentDate:yyyy-MM-dd}",
                            Event = "Legacy Data Import",
                            Notes = "Tournament created from legacy data import",
                            Squads = 1,
                            Doubles = false,
                            ThreeOutOf4 = false,
                            IsOnlyThreeGames = false,
                            IsTournamentFinalized = false
                        };

                        // Add tournament to database
                        tournamentRepository.AddTournament(tournament);
                    }

                    tournamentsCache[tournamentDate] = tournament;
                }
                else
                {
                    tournament = tournamentsCache[tournamentDate];
                }

                // Create Game entity
                Game game = new()
                {
                    Game1 = temp.Game1 >= 0 ? temp.Game1 : null,
                    Game2 = temp.Game2 >= 0 ? temp.Game2 : null,
                    Game3 = temp.Game3 >= 0 ? temp.Game3 : null,
                    Game4 = temp.Game4 >= 0 ? temp.Game4 : null,
                    Handicap = temp.HandyCap >= 0 ? temp.HandyCap : null,
                    Bonus = temp.Bonus >= 0 ? temp.Bonus : null,
                    MoneyWon = temp.Cash > 0 ? Convert.ToDecimal(temp.Cash) : null,
                    Notes = temp.Notes,
                    IsFinalized = true, // Mark as finalized since it's legacy data
                    AdjustedAvg = temp.AVG,
                    LeagueAverage = temp.TrueAverage,
                    UseGame1 = temp.Game1 >= 0,
                    UseGame2 = temp.Game2 >= 0,
                    UseGame3 = temp.Game3 >= 0,
                    UseGame4 = temp.Game4 >= 0
                };

                // Create Participant linking member, game, and tournament
                Participant participant = new()
                {
                    Member = member,
                    Game = game,
                    Tournament = tournament,
                    Squad = 1 // Default squad for imported data
                };

                // Add participant (which will also save the game)
                tournamentRepository.AddMemberToTournament(participant);

                returnMe.Add(temp);
            }
        }
        return returnMe;
    }
}
