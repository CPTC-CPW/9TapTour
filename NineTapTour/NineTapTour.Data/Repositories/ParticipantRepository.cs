using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Abstractions;
using NineTapTour.Database;
using NineTapTour.Models;

namespace NineTapTour.Data.Repositories
{
    /// <summary>EF Core implementation of <see cref="IParticipantRepository"/> (part of former <c>ParticipantsDB</c>).</summary>
    public sealed class ParticipantRepository : IParticipantRepository
    {
        private readonly IDbContextFactory<NineTapDb> _factory;

        public ParticipantRepository(IDbContextFactory<NineTapDb> factory) => _factory = factory;

        public bool EnsureParticipantExists(int tournamentId, int memberId, int squad)
        {
            using var db = _factory.CreateDbContext();

            bool exists = db.Participants.Any(p =>
                p.Tournament.Id == tournamentId &&
                p.Member.Id == memberId &&
                p.Squad == squad);
            if (exists)
                return true;

            var tournament = db.Tournaments.Find(tournamentId);
            var member = db.Members.Find(memberId);
            if (tournament == null || member == null)
                return false;

            var game = new Game
            {
                Bonus = member.Bonus,
                Handicap = member.Handicap,
                IsComp = false,
                MoneyWon = 0
            };

            db.Games.Add(game);

            var participant = new Participant
            {
                Tournament = tournament,
                Member = member,
                Squad = squad,
                Game = game
            };

            db.Participants.Add(participant);
            db.SaveChanges();
            return true;
        }

        public (int Total, Dictionary<int, int> BySquad) GetParticipantNoScoreCounts(int tournamentId)
        {
            using var db = _factory.CreateDbContext();
            var rows = db.Participants
                .AsNoTracking()
                .Include(p => p.Game)
                .Where(p => p.Tournament.Id == tournamentId && p.Game.Game1 == null)
                .Select(p => p.Squad)
                .ToList();

            var bySquad = rows
                .GroupBy(squad => squad)
                .ToDictionary(g => g.Key, g => g.Count());

            return (rows.Count, bySquad);
        }

        public List<Participant> GetParticipants(int tournamentId)
        {
            using var db = _factory.CreateDbContext();
            return [.. db.Participants.AsNoTracking().Include("Member").Include("Game").Include("Tournament")
                .Where(p => p.Tournament.Id == tournamentId)
                .OrderBy(p => p.Member.Id)];
        }

        public int GetParticipantID(int memberId, int tournyId, int squad)
        {
            using var db = _factory.CreateDbContext();
            return (from p in db.Participants
                    where p.Member.Id == memberId
                        && p.Tournament.Id == tournyId
                        && p.Squad == squad
                    select p.Id).FirstOrDefault();
        }
    }
}
