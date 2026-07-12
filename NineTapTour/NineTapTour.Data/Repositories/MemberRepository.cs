using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Abstractions;
using NineTapTour.Calculations;
using NineTapTour.Database;
using NineTapTour.Models;

namespace NineTapTour.Data.Repositories
{
    /// <summary>EF Core implementation of <see cref="IMemberRepository"/> (formerly <c>MemberDB</c>).</summary>
    public sealed class MemberRepository : IMemberRepository
    {
        private readonly IDbContextFactory<NineTapDb> _factory;

        public MemberRepository(IDbContextFactory<NineTapDb> factory) => _factory = factory;

        public void AddOrUpdateMember(Member member)
        {
            using var db = _factory.CreateDbContext();
            bool exists = db.Members.Any(m => m.Id == member.Id);
            if (exists)
            {
                db.Entry(member).State = EntityState.Modified;
            }
            else
            {
                member.Id = 0;
                db.Entry(member).State = EntityState.Added;
            }

            if (member.Average != null)
            {
                member.Handicap = TournamentCalculations.CalculateHandicapPins(member.Average.Value);
            }

            db.SaveChanges();
        }

        public bool MemberExists(Member member)
        {
            using var db = _factory.CreateDbContext();
            return db.Members.Any(m => m.Number == member.Number);
        }

        public List<Member> GetMemberList()
        {
            using var db = _factory.CreateDbContext();
            return [.. db.Members.AsNoTracking().OrderBy(m => m.Number)];
        }

        public Member GetMember(int memberNumber)
        {
            using var db = _factory.CreateDbContext();
            return db.Members.AsNoTracking().SingleOrDefault(m => m.Number == memberNumber) ?? new Member();
        }

        public Member GetMemberByGameId(int gameId)
        {
            using var db = _factory.CreateDbContext();
            return db.Participants
                .AsNoTracking()
                .Include(p => p.Game)
                .Include(p => p.Member)
                .First(p => p.Game.Id == gameId)
                .Member;
        }

        public int GetMemberIdByNumber(int memberNumber)
        {
            using var db = _factory.CreateDbContext();
            return db.Members
                .Where(m => m.Number == memberNumber)
                .Select(m => m.Id)
                .SingleOrDefault();
        }

        public int GetMemberNumberById(int memberId)
        {
            using var db = _factory.CreateDbContext();
            return db.Members
                .Where(m => m.Id == memberId)
                .Select(m => m.Number)
                .SingleOrDefault();
        }

        public int GetLastMemberNumber()
        {
            using var db = _factory.CreateDbContext();
            return db.Members.Max(m => (int?)m.Number) ?? 0;
        }

        public int GetFirstMemberNumber()
        {
            using var db = _factory.CreateDbContext();
            return db.Members.Min(m => (int?)m.Number) ?? 0;
        }
    }
}
