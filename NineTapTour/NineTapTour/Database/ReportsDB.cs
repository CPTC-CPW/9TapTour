using NineTapTour.Core.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;

namespace NineTapTour.Database;

public class ReportsDB
{
    /// <summary>
    /// Returns all finalized tournament entries flattened for report calculations,
    /// optionally filtered by tournament year range and/or a single member.
    /// </summary>
    /// <param name="startYear">Earliest tournament year to include, or null for no lower bound</param>
    /// <param name="endYear">Latest tournament year to include, or null for no upper bound</param>
    /// <param name="memberNumber">If set, only entries for this member number are returned</param>
    public static List<ReportGameEntry> GetReportEntries(int? startYear, int? endYear, int? memberNumber = null)
    {
        using var db = new NineTapDb();

        var query = db.Games
            .Include(g => g.Participant)
                .ThenInclude(p => p.Member)
            .Include(g => g.Participant.Tournament)
            .Where(g => g.IsFinalized
                     && g.Participant != null
                     && g.Participant.Member != null
                     && g.Participant.Tournament != null);

        if (startYear.HasValue)
        {
            query = query.Where(g => g.Participant.Tournament.Date.Year >= startYear.Value);
        }
        if (endYear.HasValue)
        {
            query = query.Where(g => g.Participant.Tournament.Date.Year <= endYear.Value);
        }
        if (memberNumber.HasValue)
        {
            query = query.Where(g => g.Participant.Member.Number == memberNumber.Value);
        }

        var games = query.ToList();

        return games.Select(g => new ReportGameEntry
        {
            MemberNumber = g.Participant.Member.Number,
            FirstName = g.Participant.Member.FirstName,
            LastName = g.Participant.Member.LastName,
            TournamentId = g.Participant.Tournament.Id,
            TournamentDate = g.Participant.Tournament.Date,
            Location = g.Participant.Tournament.Location,
            Event = g.Participant.Tournament.Event,
            GameScores = GetUsedGameScores(g),
            ScratchSeries = g.ScratchTotal,
            HandicapSeries = g.HandicapTotal,
            GamesPlayed = g.GamesPlayed,
            MoneyWon = g.MoneyWon ?? 0,
            SidePot = g.SidePot ?? 0,
            PlaceStanding = g.PlaceStanding
        }).ToList();
    }

    /// <summary>
    /// Returns the distinct years that have at least one tournament, newest first.
    /// Used to populate the year dropdowns on the reports form.
    /// </summary>
    public static List<int> GetTournamentYears()
    {
        using var db = new NineTapDb();
        return db.Tournaments
            .Select(t => t.Date.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToList();
    }

    /// <summary>
    /// Returns the individual game scores that are marked as "used" (UseGameX = true).
    /// Null UseGame flags default to true, matching Game.ScratchTotal.
    /// </summary>
    private static List<int> GetUsedGameScores(Game g)
    {
        List<int> scores = [];

        if ((g.UseGame1 ?? true) && g.Game1.HasValue)
            scores.Add(g.Game1.Value);

        if ((g.UseGame2 ?? true) && g.Game2.HasValue)
            scores.Add(g.Game2.Value);

        if ((g.UseGame3 ?? true) && g.Game3.HasValue)
            scores.Add(g.Game3.Value);

        if ((g.UseGame4 ?? true) && g.Game4.HasValue)
            scores.Add(g.Game4.Value);

        return scores;
    }
}
