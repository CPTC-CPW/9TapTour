#nullable disable
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Calculations;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static NineTapTour.Core.Calculations.ReportHelper;

namespace NineTapTour.Core.Services;

/// <summary>
/// Headless score-entry hub logic. Leaderboard building, series standings selection,
/// doubles team combination, and score-entry persistence were moved verbatim from
/// FrmMemberScores (M7.3); the pure computations are public statics so they can be
/// characterization-tested without a database.
/// </summary>
public class ScoresService : IScoresService
{
    private static readonly IComparer<MemberScores> scoreComparer = new MemberScoresComparer();

    private readonly IMemberRepository memberRepository;
    private readonly IGameRepository gameRepository;
    private readonly ITournamentRepository tournamentRepository;
    private readonly IParticipantRepository participantRepository;
    private readonly IPlayerHistoryRepository playerHistoryRepository;
    private readonly IDoublesTeamRepository doublesTeamRepository;
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public ScoresService(
        IMemberRepository memberRepository,
        IGameRepository gameRepository,
        ITournamentRepository tournamentRepository,
        IParticipantRepository participantRepository,
        IPlayerHistoryRepository playerHistoryRepository,
        IDoublesTeamRepository doublesTeamRepository,
        IDbContextFactory<NineTapDb> dbFactory)
    {
        this.memberRepository = memberRepository;
        this.gameRepository = gameRepository;
        this.tournamentRepository = tournamentRepository;
        this.participantRepository = participantRepository;
        this.playerHistoryRepository = playerHistoryRepository;
        this.doublesTeamRepository = doublesTeamRepository;
        this.dbFactory = dbFactory;
    }

    public LeaderboardResult GetTournamentLeaderboards(int tournamentId, bool isThreeOfFourTournament,
        ReportType reportType, int qualifyBySquadNumber, IReadOnlyList<int> filterSquads)
    {
        List<Participant> listOfParticipants = participantRepository.GetParticipants(tournamentId);
        listOfParticipants = FilterParticipantsBySquad(listOfParticipants, qualifyBySquadNumber, filterSquads);
        return BuildLeaderboards(listOfParticipants, isThreeOfFourTournament, reportType);
    }

    /// <summary>
    /// Takes a participant list and squad selection and filters for a list of participants.
    /// A single squad number (1-8) keeps only that squad; 9 applies the multi-squad
    /// filter list; anything else leaves the list unchanged.
    /// </summary>
    public static List<Participant> FilterParticipantsBySquad(List<Participant> listOfParticipants,
        int qualifyBySquadNumber, IReadOnlyList<int> filterSquads)
    {
        if (qualifyBySquadNumber > 0 && qualifyBySquadNumber <= 8)
            listOfParticipants = [.. listOfParticipants.Where(p => p.Squad == qualifyBySquadNumber)];

        else if (filterSquads.Count > 0 && qualifyBySquadNumber == 9)
            // Filters out each squad
            // take the list of participants where => if the squad number equals to any of the filtered numbers.
            listOfParticipants = [.. listOfParticipants.Where(p => filterSquads.Any(h => h == p.Squad))];
        return listOfParticipants;
    }

    /// <summary>
    /// Builds the leaderboard view models for the given participants and orders them
    /// according to the report type. High-game lists allow one top game per person per
    /// squad; series lists use full or top-3-of-4 totals depending on the ruleset.
    /// </summary>
    public static LeaderboardResult BuildLeaderboards(List<Participant> listOfParticipants,
        bool isThreeOfFourTournament, ReportType reportType)
    {
        var participantsGameViewModels = new List<ParticipantsGameViewModel>();
        var topParticipantGameViewModels = new List<TopParticipantGameViewModel>();

        // makes list of ParticipantsGameViewModel which will be used to populate scratch game and handicap game
        // listboxes which only allow 1 top game per person per squad
        foreach (Participant currParticipant in listOfParticipants)
        {
            // creates temp variable for PaticipantsGameViewModel to store necessary info for each person
            ParticipantsGameViewModel currTopScoreViewModel =
                new(
                /* MemberNo  */ currParticipant.Member.Number,
                /* FirstName */ currParticipant.Member.FirstName,
                /* LastName  */ currParticipant.Member.LastName,
                /* Squad */ currParticipant.Squad,
                /* HighScore */ currParticipant.Game.AllGameScores().Max(),
                /* Handicap  */ currParticipant.Member.Handicap,
                /* Bonus */ currParticipant.Member.Bonus
                );

            // adds person to list<ParticipantsGameViewModel>
            participantsGameViewModels.Add(currTopScoreViewModel);
        }

        foreach (Participant currParticipant in listOfParticipants)
        {
            //Gets all of the game scores that are valid (that have a value)
            var allScoresWithOutNullGames = currParticipant.Game.AllGameScores().Where(g => g.HasValue).ToList();

            //totals all games with out nulls/valid score
            int? totalScore = allScoresWithOutNullGames.Sum();

            //Sets a collection of all the games to a new variable.
            var top4Games = allScoresWithOutNullGames;

            //Sets a collection of all the games using the 3 out of 4 ruleset
            var top3Games = GetTop3OutOf4([.. top4Games]);

            int numberOfGames = top4Games.Count;

            TopParticipantGameViewModel currTopScoreViewModel =
                new(
                /* MemberNo  */ currParticipant.Member.Number,
                /* FirstName */ currParticipant.Member.FirstName,
                /* LastName  */ currParticipant.Member.LastName,
                /* Placeing  */ 0,
                /* ScratchTotal */ currParticipant.Game.AllGameScores().Sum().Value,
                /* top3ScratchScore  */ top3Games.Sum(),
                /* top3HandicapScore */ top3Games.Sum() +
                                        (Math.Min(3, numberOfGames) * currParticipant.Member.Handicap) +
                                        (Math.Min(3, numberOfGames) * currParticipant.Game.Bonus),
                /* Game1 */ currParticipant.Game.Game1,
                /* Game2 */ currParticipant.Game.Game2,
                /* Game3 */ currParticipant.Game.Game3,
                /* Game4 */ currParticipant.Game.Game4,
                /* Handicap */ currParticipant.Game.Handicap,
                /* Bonus  */ currParticipant.Game.Bonus.Value,
                /* gameID */ currParticipant.Game.Id,
                /* squad  */ currParticipant.Squad,
                /* threeOutOf4 */ isThreeOfFourTournament
                );

            topParticipantGameViewModels.Add(currTopScoreViewModel);
        }


        if (reportType == ReportType.HighGameHandicapGameSenior)
        {
            // display data in the list boxes
            // orders list by highest handicap score game to lowest
            participantsGameViewModels = [.. participantsGameViewModels.OrderByDescending(t => t.HighScore + t.Handicap + t.Bonus)];
        }
        else if (reportType == ReportType.HighGame)
        {
            // orders list by highest scratch score game to lowest
            participantsGameViewModels = [.. participantsGameViewModels.OrderByDescending(t => t.HighScore)];
        }
        else if (reportType == ReportType.HighSeriesScratch && isThreeOfFourTournament)
        {
            topParticipantGameViewModels = [.. topParticipantGameViewModels.OrderByDescending(t => t.Top3ScratchScore)];
        }
        else if (reportType == ReportType.HighSeriesScratch && !isThreeOfFourTournament)
        {
            topParticipantGameViewModels = [.. topParticipantGameViewModels.OrderByDescending(t => t.ScratchTotal)];
        }
        else if (reportType == ReportType.HighSeriesHandicap && isThreeOfFourTournament)
        {
            topParticipantGameViewModels = [.. topParticipantGameViewModels.OrderByDescending(t => t.Top3HandiScores)];
        }
        else if (reportType == ReportType.HighSeriesHandicap && !isThreeOfFourTournament)
        {
            topParticipantGameViewModels = [.. topParticipantGameViewModels.OrderByDescending(t => t.HandicapScore)];
        }

        return new LeaderboardResult(participantsGameViewModels, topParticipantGameViewModels);
    }

    /// <summary>
    /// This method sorts scores and removes the lowest if 4 scores are present
    /// It returns  a list with the 3 highest scores listOfValidScores
    /// </summary>
    /// <param name="scores"></param>
    public static List<int> GetTop3OutOf4(List<int?> scores)
    {
        List<int> listOfValidScores = [];
        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i].HasValue)
                listOfValidScores.Add(scores[i].Value);
        }

        //after sorting I want to get rid of lowest score
        listOfValidScores.Sort();
        if (listOfValidScores.Count == 4)
            listOfValidScores.Remove(listOfValidScores[0]);

        listOfValidScores.Reverse();
        return listOfValidScores;
    }

    /// <summary>
    /// Computes the 3-of-4 adjusted series totals: the lowest scratch game and the
    /// lowest handicap game are each dropped from their respective four-game totals.
    /// Both lists must contain exactly the four entered game values.
    /// </summary>
    public static ScoreTotals ComputeThreeOfFourAdjustedTotals(IReadOnlyList<int> scratchScores,
        IReadOnlyList<int> handicapScores)
    {
        int scratchTotal = scratchScores.Sum() - scratchScores.Min();
        int handicapTotal = handicapScores.Sum() - handicapScores.Min();
        return new ScoreTotals(scratchTotal, handicapTotal);
    }

    /// <summary>
    /// Pure rule from FrmMemberScores.IsValid: given the tournament type and which
    /// scratch score boxes are empty, returns true when a required score is missing
    /// (doubles tournaments only require games 1 and 2; 3-game tournaments require
    /// the first three games).
    /// </summary>
    public static bool AreRequiredScoresMissing(bool isDoubles, bool isOnlyThreeGames,
        bool game1Empty, bool game2Empty, bool game3Empty, bool game4Empty)
    {
        bool areAnyGamesScoresEmpty = isDoubles
            ? game1Empty || game2Empty
            : game1Empty || game2Empty || game3Empty || game4Empty;
        bool areAnyFirst3BoxesEmptyForThreeGameTournament = isOnlyThreeGames && (game1Empty || game2Empty || game3Empty);
        return (areAnyGamesScoresEmpty && !isOnlyThreeGames) || areAnyFirst3BoxesEmptyForThreeGameTournament;
    }

    public List<MemberScores> GetGameScores(int tournamentId)
    {
        List<MemberScores> temp = participantRepository.GetGameMemberScores(tournamentId);
        temp.Sort(scoreComparer);
        return temp;
    }

    public List<MemberScores> GetSeriesStandings(int tournamentId, bool isThreeOfFourTournament,
        bool isDoubles, bool useHandicap, bool useScratch, List<int> squadList)
    {
        var temp = new List<MemberScores>();

        if (useHandicap)
        {
            if (isThreeOfFourTournament && squadList.Contains(0))
            {
                temp = participantRepository.GetStandingsForTournamentByHandicap(tournamentId, true);
            }
            else if (isThreeOfFourTournament && !squadList.Contains(0))
            {
                temp = participantRepository.GetStandingsForTournamentByFilterSeriesByHandicap(squadList, tournamentId, true);
            }
            else if (!isThreeOfFourTournament && squadList.Contains(0))
            {
                temp = participantRepository.GetStandingsForTournamentByHandicap(tournamentId);
            }
            else if (!isThreeOfFourTournament && !squadList.Contains(0))
            {
                temp = participantRepository.GetStandingsForTournamentByFilterSeriesByHandicap(squadList, tournamentId);
            }
        }
        else if (useScratch)
        {
            if (isThreeOfFourTournament && squadList.Contains(0))
            {
                temp = participantRepository.GetStandingsForTournamentByScratch(tournamentId, true);
            }
            else if (isThreeOfFourTournament && !squadList.Contains(0))
            {
                temp = participantRepository.GetStandingsForTournamentByFilterSeriesByScratch(squadList, tournamentId, true);
            }
            else if (!isThreeOfFourTournament && squadList.Contains(0))
            {
                temp = participantRepository.GetStandingsForTournamentByScratch(tournamentId);
            }
            else if (!isThreeOfFourTournament && !squadList.Contains(0))
            {
                temp = participantRepository.GetStandingsForTournamentByFilterSeriesByScratch(squadList, tournamentId);
            }
        }

        // For doubles tournaments, combine partner scores into team standings
        if (isDoubles)
        {
            List<DoublesTeam> allTeams = doublesTeamRepository.GetTeamsByTournament(tournamentId);
            temp = CombineDoublesSeriesToTeams(temp, allTeams, squadList.Contains(0) ? null : squadList);
        }

        temp.Sort(scoreComparer);
        return temp;
    }

    /// <summary>
    /// Combines individual doubles player standings into team standings.
    /// Each DoublesTeam's two members' scores are summed into a single team entry.
    /// Partners are matched by name as "FirstName1 LastName1 &amp; FirstName2 LastName2".
    /// </summary>
    /// <param name="individualScores">The list of individual player standings.</param>
    /// <param name="allTeams">The doubles team pairings for the tournament.</param>
    /// <param name="squadFilter">Optional list of squad numbers to filter teams by. Null means all squads.</param>
    /// <returns>A list of TeamMemberScores representing team standings.</returns>
    public static List<MemberScores> CombineDoublesSeriesToTeams(List<MemberScores> individualScores,
        List<DoublesTeam> allTeams, List<int> squadFilter)
    {
        var combinedTeams = new List<MemberScores>();

        // Filter teams by squad list if provided
        List<DoublesTeam> teams = squadFilter == null
            ? allTeams
            : allTeams.Where(t => squadFilter.Contains(t.Squad)).ToList();

        // Process each team pairing
        foreach (var team in teams)
        {
            // Find both members' scores in the individual standings
            var member1Scores = individualScores.FirstOrDefault(s => s.MemberId == team.Member1.Number && s.Squad == team.Squad);
            var member2Scores = individualScores.FirstOrDefault(s => s.MemberId == team.Member2.Number && s.Squad == team.Squad);

            // Skip this team if either member is not found in the standings
            if (member1Scores == null || member2Scores == null)
                continue;

            // Create a combined team entry
            var teamScores = new TeamMemberScores
            {
                // FirstName/LastName intentionally left empty; reports use Partner1*/Partner2* fields
                FirstName = string.Empty,
                LastName = string.Empty,

                // Sum both partners' scores
                Score = (member1Scores.Score ?? 0) + (member2Scores.Score ?? 0),

                // Use first partner's payment info as the team's overall status
                Paid = member1Scores.Paid && member2Scores.Paid,
                LastPaymentYear = member1Scores.LastPaymentYear,

                // Store individual partner data
                Partner1MemberId = team.Member1.Number,
                Partner1FirstName = team.Member1.FirstName,
                Partner1LastName = team.Member1.LastName,
                Partner1Score = member1Scores.Score,

                Partner2MemberId = team.Member2.Number,
                Partner2FirstName = team.Member2.FirstName,
                Partner2LastName = team.Member2.LastName,
                Partner2Score = member2Scores.Score,
                Partner2LastPaymentYear = member2Scores.LastPaymentYear,

                // Use first partner's member ID as a reference
                MemberId = team.Member1.Number,
                IsTeam = true
            };

            combinedTeams.Add(teamScores);
        }

        return combinedTeams;
    }

    public ScoreEntryResult SaveScoreEntry(ScoreEntryRequest request)
    {
        // Gets the current tournament from the database
        Tournament currTourney = tournamentRepository.GetTourneyByID(request.TournamentId);

        // Get the member from the database using the captured member number
        Member currentMem = memberRepository.GetMember(request.MemberNumber);

        Participant player = new()
        {
            Member = currentMem,
            Game = new Game(),
            Tournament = currTourney,
            Squad = request.Squad
        };

        using var db = dbFactory.CreateDbContext();

        int gameId = gameRepository.GetGameID(db, currentMem.Id, currTourney.Id, request.Squad);
        int parID = participantRepository.GetParticipantID(db, currentMem.Id, currTourney.Id, request.Squad);

        if (parID != 0)
        {
            player.Id = parID;
        }

        player.Game.Id = gameId;
        player.Game.MoneyWon = request.MoneyWon;
        player.Game.Game1 = request.Game1;
        player.Game.Game2 = request.Game2;
        player.Game.Game3 = request.Game3;
        player.Game.Game4 = request.Game4;

        Game currentGame = GetGameOrNull(currentMem.Id, currTourney.Id, request.Squad);

        if (currentGame == null)
        {
            int? mostRecentAdjAvg = playerHistoryRepository.GetMostRecentAverage(currentMem.Number);
            player.Game.Handicap = mostRecentAdjAvg != null
                ? TournamentCalculations.CalculateHandicapPins(mostRecentAdjAvg.Value)
                : currentMem.Handicap;
            player.Game.Bonus = currentMem.Bonus;
        }
        else
        {
            player.Game.Bonus = currentGame.Bonus;
            player.Game.Handicap = currentGame.Handicap;
        }

        // If comp entry was checked, set IsComp to true in game table
        if (request.IsComp)
        {
            player.Game.IsComp = true;
        }

        db.SaveChanges();

        bool success = true;
        string errorMessage = "";
        try
        {
            tournamentRepository.AddMemberToTournament(player);
        }
        catch (MemberAccessException ex)
        {
            success = false;
            errorMessage = ex.Message;
        }

        // UPDATE LASTBOWLED DATE
        // Sets last bowled to now and updates DB record.
        // Runs even when the add failed, matching the original form flow.
        if (DateTime.Now > currentMem.LastBowled || currentMem.LastBowled == null)
        {
            currentMem.LastBowled = DateTime.Now;
            memberRepository.AddOrUpdateMember(currentMem);
        }

        return new ScoreEntryResult(success, errorMessage, currentMem, currTourney);
    }

    /// <summary>
    /// Gets the bowler's game in the tournament squad, or null when it does not exist.
    /// Mirrors FrmMemberScores.GetScoresById which swallows the duplicate-row error.
    /// </summary>
    private Game GetGameOrNull(int memberId, int tournamentId, int squad)
    {
        try
        {
            return gameRepository.GetGameInTournament(memberId, tournamentId, squad);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine("Error Number : " + ex.Message);
            return null;
        }
    }
}
