using Microsoft.EntityFrameworkCore;
using NineTapTour.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Database
{
    /// <summary>
    /// Helper class for Phase 2 refactoring - migrating from FinalizeTemp to Game entity.
    /// Provides utilities for validation, cleanup, and migration status checking.
    /// </summary>
    public static class FinalizeTempMigrationHelper
    {
        /// <summary>
        /// Gets statistics about FinalizeTemp usage in the database.
        /// </summary>
        public static MigrationStatistics GetMigrationStatistics()
        {
            using (var db = new NineTapDb())
            {
                var stats = new MigrationStatistics
                {
                    TotalFinalizeRecords = db.FinalizeTemp.Count(),
                    FinalizedTournaments = db.Tournaments.Count(t => t.IsTournamentFinalized),
                    UnfinalizedTournaments = db.Tournaments.Count(t => !t.IsTournamentFinalized),
                    FinalizedGames = db.Games.Count(g => g.IsFinalized),
                    UnfinalizedGames = db.Games.Count(g => !g.IsFinalized)
                };

                // Count FinalizeTemp records for finalized vs unfinalized tournaments
                var finalizeRecordsGrouped = (from ft in db.FinalizeTemp
                                             join t in db.Tournaments on ft.TournamentID equals t.Id
                                             group ft by t.IsTournamentFinalized into g
                                             select new { IsFinalized = g.Key, Count = g.Count() })
                                             .ToList();

                stats.FinalizeRecordsForFinalizedTournaments = 
                    finalizeRecordsGrouped.FirstOrDefault(x => x.IsFinalized)?.Count ?? 0;
                stats.FinalizeRecordsForUnfinalizedTournaments = 
                    finalizeRecordsGrouped.FirstOrDefault(x => !x.IsFinalized)?.Count ?? 0;

                return stats;
            }
        }

        /// <summary>
        /// Lists all finalized tournaments that still have FinalizeTemp records (candidates for cleanup).
        /// </summary>
        public static List<TournamentCleanupInfo> GetCleanupCandidates()
        {
            using (var db = new NineTapDb())
            {
                return [.. (from ft in db.FinalizeTemp
                           join t in db.Tournaments on ft.TournamentID equals t.Id
                           where t.IsTournamentFinalized
                           group ft by new { t.Id, t.Date, t.Location } into g
                           select new TournamentCleanupInfo
                           {
                               TournamentId = g.Key.Id,
                               TournamentDate = g.Key.Date,
                               Location = g.Key.Location,
                               FinalizeRecordCount = g.Count()
                           })
                           .OrderBy(t => t.TournamentDate)];
            }
        }

        /// <summary>
        /// Validates and optionally fixes data inconsistencies for a finalized tournament.
        /// </summary>
        public static ValidationResult ValidateAndFixTournament(int tournamentId, bool autoFix = false)
        {
            var result = new ValidationResult { TournamentId = tournamentId };

            using (var db = new NineTapDb())
            {
                var tournament = db.Tournaments.Find(tournamentId);
                if (tournament == null)
                {
                    result.Errors.Add("Tournament not found");
                    return result;
                }

                if (!tournament.IsTournamentFinalized)
                {
                    result.Errors.Add("Tournament is not finalized");
                    return result;
                }

                var finalizeRecords = db.FinalizeTemp.Where(ft => ft.TournamentID == tournamentId).ToList();
                result.TotalRecords = finalizeRecords.Count;

                foreach (var ft in finalizeRecords)
                {
                    var game = db.Games.Find(ft.GameId);
                    if (game == null)
                    {
                        result.Errors.Add($"Game {ft.GameId} not found for FinalizeTemp {ft.FinalizeID}");
                        continue;
                    }

                    if (!game.IsFinalized)
                    {
                        result.Warnings.Add($"Game {ft.GameId} is not marked as finalized");
                        if (autoFix)
                        {
                            game.IsFinalized = true;
                            result.FixesApplied++;
                        }
                    }

                    // Check for data mismatches
                    if (game.TournamentID != ft.TournamentID && game.TournamentID != null)
                    {
                        result.Warnings.Add($"Game {ft.GameId} TournamentID mismatch: Game={game.TournamentID}, FinalizeTemp={ft.TournamentID}");
                    }

                    if (Math.Abs(game.LeagueAverage - ft.LeagueAverage) > 0.1)
                    {
                        result.Warnings.Add($"Game {ft.GameId} LeagueAverage mismatch: Game={game.LeagueAverage}, FinalizeTemp={ft.LeagueAverage}");
                    }

                    result.ValidRecords++;
                }

                if (autoFix && result.FixesApplied > 0)
                {
                    db.SaveChanges();
                }

                result.IsValid = result.Errors.Count == 0;
            }

            return result;
        }
    }

    /// <summary>
    /// Statistics about the FinalizeTemp to Game migration progress.
    /// </summary>
    public class MigrationStatistics
    {
        public int TotalFinalizeRecords { get; set; }
        public int FinalizedTournaments { get; set; }
        public int UnfinalizedTournaments { get; set; }
        public int FinalizedGames { get; set; }
        public int UnfinalizedGames { get; set; }
        public int FinalizeRecordsForFinalizedTournaments { get; set; }
        public int FinalizeRecordsForUnfinalizedTournaments { get; set; }

        public double MigrationProgress => FinalizedTournaments > 0 
            ? (double)(FinalizedTournaments - (FinalizeRecordsForFinalizedTournaments > 0 ? FinalizeRecordsForFinalizedTournaments : 0)) / FinalizedTournaments * 100 
            : 0;
    }

    /// <summary>
    /// Information about a tournament that has FinalizeTemp records ready for cleanup.
    /// </summary>
    public class TournamentCleanupInfo
    {
        public int TournamentId { get; set; }
        public DateTime TournamentDate { get; set; }
        public string Location { get; set; }
        public int FinalizeRecordCount { get; set; }
    }

    /// <summary>
    /// Result of validating a tournament's data migration.
    /// </summary>
    public class ValidationResult
    {
        public int TournamentId { get; set; }
        public bool IsValid { get; set; }
        public int TotalRecords { get; set; }
        public int ValidRecords { get; set; }
        public int FixesApplied { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
