using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Abstractions;
using NineTapTour.Calculations;
using NineTapTour.Database;
using NineTapTour.Models;

namespace NineTapTour.Data.Services
{
    /// <summary>
    /// Applies finalized rows to games and members in one transaction. Extracted verbatim from
    /// FrmFinalizeTournament.FinalizeAllGames so the behavior is preserved but testable.
    /// </summary>
    public sealed class FinalizationService : IFinalizationService
    {
        private readonly IDbContextFactory<NineTapDb> _factory;
        private readonly IFinalizeRepository _finalizeRepo;

        public FinalizationService(IDbContextFactory<NineTapDb> factory, IFinalizeRepository finalizeRepo)
        {
            _factory = factory;
            _finalizeRepo = finalizeRepo;
        }

        public void FinalizeTournament(int tournamentId, IReadOnlyList<FinalizeGameInput> rows)
        {
            using var db = _factory.CreateDbContext();

            // Update each member's record once per member.
            var updatedMembers = new HashSet<int>();

            // The 30-game league average depends only on the member number (tournament is fixed for this
            // pass), yet a member can appear on many rows — cache so the history queries run once each.
            var leagueAvgByMember = new Dictionary<int, double>();
            double LeagueAverage(int memberNum)
            {
                if (!leagueAvgByMember.TryGetValue(memberNum, out double avg))
                {
                    avg = _finalizeRepo.Get30GameAverage(memberNum, tournamentId);
                    leagueAvgByMember[memberNum] = avg;
                }
                return avg;
            }

            foreach (FinalizeGameInput row in rows)
            {
                Game game = db.Games.Find(row.GameId);
                if (game == null) continue;

                if (row.IsDoublesMember)
                {
                    game.Game1 = row.Game1;
                    game.Game2 = row.Game2;
                    game.UseGame1 = row.UseGame1;
                    game.UseGame2 = row.UseGame2;
                    game.IsFinalized = true;
                    game.AdjustedAvg = row.AdjustedAvg;
                    game.KeepAdjustedAvg = row.DirectorCheck;
                    game.Handicap = row.Handicap;
                    game.LeagueAverage = LeagueAverage(row.MemberNumber);
                    game.Bonus = TournamentCalculations.ComputeHalfRateBonus(row.OriginalBaseBonus, row.PlaceStanding, row.IsCashing);

                    // Director entered the full place prize; save each member's 50% share.
                    game.MoneyWon = row.Earnings > 0 ? row.Earnings / 2m : (decimal?)null;
                    game.PlaceStanding = row.PlaceStanding > 0 ? (int?)row.PlaceStanding : null;

                    if (row.MemberNumber > 0 && updatedMembers.Add(row.MemberNumber))
                    {
                        Member member = db.Members.FirstOrDefault(m => m.Number == row.MemberNumber);
                        if (member != null && member.Id > 0)
                        {
                            member.Average = row.AdjustedAvg;
                            member.Handicap = TournamentCalculations.CalculateHandicapPins(row.AdjustedAvg);
                            member.Bonus = game.Bonus ?? 0;
                        }
                    }
                    continue;
                }

                // Singles row.
                game.IsFinalized = true;
                game.AdjustedAvg = row.AdjustedAvg;
                game.Handicap = row.Handicap;

                // Preserve the original pre-deduction bonus for cashing/third-entry members so the
                // Game record is not corrupted across sessions.
                game.Bonus = (row.IsCashing || row.IsThirdEntryBonus) ? row.OriginalBaseBonus : row.BonusFromGrid;
                game.LeagueAverage = LeagueAverage(row.MemberNumber);

                if (updatedMembers.Add(row.MemberNumber))
                {
                    Member member = db.Members.FirstOrDefault(m => m.Number == row.MemberNumber);
                    if (member != null && member.Id > 0)
                    {
                        member.Average = row.AdjustedAvg;
                        member.Handicap = TournamentCalculations.CalculateHandicapPins(row.AdjustedAvg);
                        member.Bonus = row.BonusFromGrid;
                    }
                }
            }

            Tournament tourn = db.Tournaments.Find(tournamentId);
            if (tourn != null)
            {
                tourn.IsTournamentFinalized = true;
            }

            db.SaveChanges();
        }
    }
}
