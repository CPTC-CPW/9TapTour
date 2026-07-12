using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Abstractions;
using NineTapTour.Calculations;
using NineTapTour.Database;
using NineTapTour.Models;

namespace NineTapTour.Data.Repositories
{
    /// <summary>EF Core implementation of <see cref="IFinalizeRepository"/> (formerly <c>FinalizeTempDB</c>).</summary>
    public sealed class FinalizeRepository : IFinalizeRepository
    {
        private readonly IDbContextFactory<NineTapDb> _factory;

        public FinalizeRepository(IDbContextFactory<NineTapDb> factory) => _factory = factory;

        /// <summary>
        /// Calculates league average for given member based off last 30 games
        /// or total games played if less than 30.
        /// </summary>
        public double Get30GameAverage(Member mem)
        {
            // Represents the number of past games for the player
            int howManyGames = 30;
            using var db = _factory.CreateDbContext();
            {
                var getGames = (from p in db.Participants
                           join m in db.Members on p.Member.Id equals m.Id
                           join g in db.Games on p.Game.Id equals g.Id
                           join t in db.Tournaments on p.Tournament.Id equals t.Id
                           where mem.Id == m.Id
                           orderby t.Date descending
                           select new { g.Game1, g.Game2, g.Game3, g.Game4, g.UseGame1, g.UseGame2, g.UseGame3, g.UseGame4 })
                              .AsNoTracking().Take(howManyGames).ToList();

                List<double> allAverages = [];
                foreach (var avg in getGames)
                {
                    allAverages.Add(Convert.ToDouble(avg.Game1 + avg.Game2 + avg.Game3 + avg.Game4) /
                        LeagueAverageHelper(avg.UseGame1, avg.UseGame2, avg.UseGame3, avg.UseGame4));
                }

                // Guard against dividing by zero when the member has no qualifying games.
                if (allAverages.Count == 0)
                {
                    return 0;
                }

                return allAverages.Sum() / allAverages.Count;
            }
        }

        /// <summary>
        /// This is a helper method for <see cref="Get30GameAverage(Member)"/>
        /// that takes the useGames from the database to see if they are true or null.
        /// We are saying that null is true, because they are optional booleans in the database.
        /// (Assuming that during the import process these are not being updated)
        /// </summary>
        /// <param name="g1">UseGame1 From GameDB</param>
        /// <param name="g2">UseGame2 From GameDB</param>
        /// <param name="g3">UseGame3 From GameDB</param>
        /// <param name="g4">UseGame4 From GameDB</param>
        public int LeagueAverageHelper(bool? g1, bool? g2,
            bool? g3, bool? g4)
        {
            // Total games played (Max: 4)
            int totalGamesPlayed = 0;
            if (g1 == null || g1.Value)
            {
                totalGamesPlayed++;
            }

            if (g2 == null || g2.Value)
            {
                totalGamesPlayed++;
            }

            if (g3 == null || g3.Value)
            {
                totalGamesPlayed++;
            }

            if (g4 == null || g4.Value)
            {
                totalGamesPlayed++;
            }
            return totalGamesPlayed;
        }

        /// <summary>
        /// Get's the leauge average based off the formula
        /// (Total of Scratch Score / Total of games played)
        /// from the last 30 entries
        /// </summary>
        /// <param name="memberNumber">The member number NOT id</param>
        /// <param name="tournamentId">The id of the tournament</param>
        public double Get30GameAverage(int memberNumber, int tournamentId)
        {
            int allGamesPlayed = 0;
            int totalScratchTotal = 0;

            // Going through the database to get all of a player's ScratchTotals and the 4 UseGames
            // to help with calculating the league Average
            List<CurrentHistory> currentHistory = GetCurrentHistory(memberNumber, tournamentId);
            // Going through the database to get all of a player's GamePlayed and ScratchTotals Excluding
            // those from the current game.
            List<PreviousHistory> previousHistory = GetPreviousHistory(memberNumber, currentHistory);

            // Looping through the current player history adding up there scratch totals & games played
            foreach (var c in currentHistory)
            {
                allGamesPlayed += LeagueAverageHelper(c.UseGame1, c.UseGame2, c.UseGame3, c.UseGame4);
                totalScratchTotal += c.ScratchTotal;
            }

            //Looping through the previous player history adding up there scratch totals & games played
            foreach (var ph in previousHistory)
            {
                allGamesPlayed += ph.GamesPlayed;
                totalScratchTotal += ph.TotalScore;
            }

            // Guard against dividing by zero when the member has no counted games.
            if (allGamesPlayed == 0)
            {
                return 0;
            }

            // Truncate to one decimal place
            return Math.Truncate(((double)totalScratchTotal / allGamesPlayed) * 10) / 10;
        }

        public List<CurrentHistory> GetCurrentHistory(int memberNumber, int tournamentId)
        {
            using var db = _factory.CreateDbContext();
            {
                // Phase 6: Use Tournament.TourneyRegion.NineTapRegionID for proper FK relationship
                var curHistory = (from p in db.Participants
                                  join m in db.Members on p.Member.Id equals m.Id
                                  join g in db.Games on p.Game.Id equals g.Id
                                  join t in db.Tournaments on p.Tournament.Id equals t.Id
                                  where m.Number == memberNumber &&
                                        t.Id == tournamentId
                                  select new CurrentHistory
                                  {
                                      ScratchTotal = (g.Game1 ?? 0) + (g.Game2 ?? 0) + (g.Game3 ?? 0) + (g.Game4 ?? 0),
                                      UseGame1 = g.UseGame1 ?? true,
                                      UseGame2 = g.UseGame2 ?? true,
                                      UseGame3 = g.UseGame3 ?? true,
                                      UseGame4 = g.UseGame4 ?? true
                                  }).AsNoTracking().ToList();

                return curHistory;
            }
        }

        public List<PreviousHistory> GetPreviousHistory(int memberNumber, List<CurrentHistory> curHistory)
        {
            using var db = _factory.CreateDbContext();
            {
                var prevHistory = (from p in db.Participants
                                  join m in db.Members on p.Member.Id equals m.Id
                                  join g in db.Games on p.Game.Id equals g.Id
                                  join t in db.Tournaments on p.Tournament.Id equals t.Id
                                  where m.Number == memberNumber
                                     && g.IsFinalized
                                  orderby t.Date descending, g.MoneyWon descending
                                   select new PreviousHistory
                                  {
                                      TournamentDate = t.Date,
                                      GamesPlayed = g.GamesPlayed,
                                      TotalScore = g.ScratchTotal
                                  }).AsNoTracking().Take(30 - curHistory.Count).ToList();
                return prevHistory;
            }
        }

        /// <summary>
        /// [Phase 4 REFACTORED] Updates Game entity with finalization properties.
        /// FinalizeTemp writes are deprecated - only Games table is updated.
        /// </summary>
        public void AddFinalizeTemp(GameViewModel temp)
        {
            using var db = _factory.CreateDbContext();
            {
                try
                {
                    // Phase 4: Only update Games table, skip FinalizeTemp entirely
                    Game game = db.Games.FirstOrDefault(g => g.Id == temp.GameId);
                    if (game != null)
                    {
                        // Phase 4: Removed game.TournamentID (redundant - stored in Participant)
                        // Update finalization properties on Game entity
                        game.LeagueAverage = temp.LeagueAverage;
                        game.AdjustedAvg = temp.AdjustedAvg;
                        game.KeepAdjustedAvg = temp.KeepAdjustedAvg;
                        game.HandicapTotal = temp.HandicapTotal;

                        // Update member handicap
                        var member = db.Members.FirstOrDefault(x => x.Id == temp.MemberId);
                        if (member != null)
                        {
                            member.Handicap = TournamentCalculations.CalculateHandicapPins(
                                Convert.ToInt16(Get30GameAverage(member))
                            );
                        }

                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error updating game finalization data: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Retrieves a single participant from a tournament based on given gameID.
        /// Return null if no participant is found
        /// </summary>
        public Participant GetParticipantByGameId(int gameID)
        {
            using var db = _factory.CreateDbContext();
            return (from par in db.Participants
                   where par.Game.Id == gameID
                   // No tracking prevents EF from monitoring changes. This means
                   // that we will have to manually update/delete entities
                   select par).Include(nameof(Member)).AsNoTracking().SingleOrDefault();
        }

        /// <summary>
        /// Deletes the Participant given from the database
        /// </summary>
        public void DeleteParticipant(Participant p)
        {
            using var db = _factory.CreateDbContext();
            {
                db.Entry(p).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the entry quantity for a member in a tournament (Phase 3: reads from Games only).
        /// </summary>
        /// <returns>the amount of enties in tournament by a member</returns>
        public int GetMembersGameEntryCount(int tourneyId, int memberNum)
        {
            using var db = _factory.CreateDbContext();
            {
                // Phase 3: Read from Games via Participants join
                return (from p in db.Participants
                       join m in db.Members on p.Member.Id equals m.Id
                       join g in db.Games on p.Game.Id equals g.Id
                       where p.Tournament.Id == tourneyId && m.Number == memberNum
                       select g).AsNoTracking().Count();
            }
        }
    }
}
