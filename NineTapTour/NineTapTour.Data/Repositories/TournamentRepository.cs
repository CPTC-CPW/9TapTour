using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Abstractions;
using NineTapTour.Database;
using NineTapTour.Models;
using NineTapTour.Models.ViewModels;

namespace NineTapTour.Data.Repositories
{
    /// <summary>EF Core implementation of <see cref="ITournamentRepository"/> (formerly <c>TournamentDB</c>).</summary>
    public sealed class TournamentRepository : ITournamentRepository
    {
        private readonly IDbContextFactory<NineTapDb> _factory;

        public TournamentRepository(IDbContextFactory<NineTapDb> factory) => _factory = factory;

        public void AddTournament(Tournament tourn)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(tourn).State = db.Tournaments.Any(t => t.Id == tourn.Id)
                ? EntityState.Modified
                : EntityState.Added;
            db.SaveChanges();
        }

        public bool UpdateTournament(Tournament tourn)
        {
            using var db = _factory.CreateDbContext();
            Tournament original = db.Tournaments.Find(tourn.Id);
            if (original != null)
            {
                db.Entry(original).CurrentValues.SetValues(tourn);
                db.SaveChanges();
            }
            else
            {
                throw new ArgumentException("The original data could not be found.");
            }
            return true;
        }

        public List<Tournament> GetTournamentList()
        {
            using var db = _factory.CreateDbContext();
            return [.. (from t in db.Tournaments.AsNoTracking()
                        orderby t.Date descending
                        select t)];
        }

        public List<Participant> GetTournamentMemberList(Tournament tourn)
        {
            using var db = _factory.CreateDbContext();
            return [.. (from p in db.Participants.AsNoTracking()
                        join m in db.Members on p.Member.Id equals m.Id
                        orderby p.Id
                        where p.Tournament.Id == tourn.Id
                        select p).Include(m => m.Member)];
        }

        public List<Participant> GetTournamentMemberListInOrder(Tournament tourn)
        {
            using var db = _factory.CreateDbContext();
            return [.. (from p in db.Participants.AsNoTracking()
                        join m in db.Members on p.Member.Id equals m.Id
                        orderby p.Member.Id
                        where p.Tournament.Id == tourn.Id
                        select p).Include(m => m.Member)];
        }

        public int GetTotalNumberParticipantsInTournament(Tournament tourn)
        {
            using var db = _factory.CreateDbContext();
            return db.Participants
                .Where(p => p.Tournament.Id == tourn.Id)
                .Count();
        }

        public List<Member> GetUniqueTourMembers(Tournament tourn)
        {
            using var db = _factory.CreateDbContext();
            return [.. (from p in db.Participants.AsNoTracking()
                        join m in db.Members on p.Member.Id equals m.Id
                        where p.Tournament.Id == tourn.Id
                        select m).Distinct()];
        }

        public List<Member> GetUniqueTourMembersByDate(DateTime start, DateTime end)
        {
            using var db = _factory.CreateDbContext();
            return [.. (from p in db.Participants.AsNoTracking()
                        join m in db.Members on p.Member.Id equals m.Id
                        where p.Tournament.Date >= start && p.Tournament.Date <= end
                        select m).Distinct()];
        }

        public void AddMemberToTournament(Participant player)
        {
            using var db = _factory.CreateDbContext();

            // Use AsNoTracking to avoid tracking entities in the duplicate check query
            bool isMemberInTournament = db.Participants
                .AsNoTracking()
                .Any(p => p.Member.Id == player.Member.Id
                       && p.Tournament.Id == player.Tournament.Id
                       && p.Squad == player.Squad);

            if (!isMemberInTournament)
            {
                player.Id = 0; // New participants will get an auto generated id

                // Attach related entities to this context before adding the participant
                db.Attach(player.Member);
                db.Attach(player.Tournament);
                db.Attach(player.Game);

                db.Participants.Add(player);

                // Set states to Unchanged so the existing member/tournament are not re-inserted
                db.Entry(player.Tournament).State = EntityState.Unchanged;
                db.Entry(player.Member).State = EntityState.Unchanged;

                db.SaveChanges();
            }
            else
            {
                Game result = db.Games.SingleOrDefault(g => g.Id == player.Game.Id);
                Participant squadResult = db.Participants.SingleOrDefault(p => p.Id == player.Id);
                Participant memberQuery = db.Participants.Include(m => m.Member)
                    .Where(m => m.Member.Id == player.Member.Id).FirstOrDefault();
                result.Game1 = player.Game.Game1;
                result.Game2 = player.Game.Game2;
                result.Game3 = player.Game.Game3;
                result.Game4 = player.Game.Game4;
                result.MoneyWon = player.Game.MoneyWon;
                result.IsComp = player.Game.IsComp;

                if (squadResult == null)
                {
                    squadResult = new Participant();
                }
                squadResult.Squad = player.Squad;
                squadResult.Member = memberQuery.Member;
                db.SaveChanges();
            }
        }

        public Tournament GetTourneyByID(int tournID)
        {
            using var db = _factory.CreateDbContext();
            return (from g in db.Tournaments.AsNoTracking()
                    where g.Id == tournID
                    select g).SingleOrDefault();
        }

        public List<Member> GetAllActiveMembers()
        {
            using var db = _factory.CreateDbContext();
            return [.. (from active in db.Members.AsNoTracking()
                        where active.IsActive == true
                        select active)];
        }

        public void DeleteTournament(Tournament tourn)
        {
            using var db = _factory.CreateDbContext();

            // Delete games
            var gamesToDelete = db.Games.Where(g => g.Participant.Tournament.Id == tourn.Id).ToList();
            db.Games.RemoveRange(gamesToDelete);

            // Delete participants
            var participantsToDelete = db.Participants.Where(p => p.Tournament.Id == tourn.Id).ToList();
            db.Participants.RemoveRange(participantsToDelete);

            db.Entry(tourn).State = EntityState.Deleted;
            db.SaveChanges();
        }

        public List<WinnerListMemberViewModel> GetWinnerListMemberData(int tournamentId)
        {
            using var db = _factory.CreateDbContext();
            return [.. (from p in db.Participants.AsNoTracking()
                        join m in db.Members on p.Member.Id equals m.Id
                        join g in db.Games on p.Game.Id equals g.Id
                        join t in db.Tournaments on p.Tournament.Id equals t.Id
                        let memberNumber = m.Number
                        let name = m.FirstName + " " + m.LastName
                        where tournamentId == p.Tournament.Id
                        select new WinnerListMemberViewModel
                        {
                            PlaceStanding = g.PlaceStanding,
                            PlaceStandingLabel = g.PlaceStandingLabel,
                            MemberId = m.Id,
                            MemberNumber = memberNumber,
                            BowlerName = name,
                            Handicap = g.Handicap,
                            Bonus = g.Bonus,
                            MemberBonus = m.Bonus,
                            MoneyWon = g.MoneyWon,
                            SidePot = g.SidePot,
                            GameId = g.Id,
                            Game1 = g.Game1,
                            Game2 = g.Game2,
                            Game3 = g.Game3,
                            Game4 = g.Game4,
                            IsComp = g.IsComp,
                            LeagueAverage = (double)(m.Average ?? 0),
                            AdjustedAvg = g.AdjustedAvg,
                            UseGame1 = g.UseGame1,
                            UseGame2 = g.UseGame2,
                            UseGame3 = g.UseGame3,
                            UseGame4 = g.UseGame4,
                            KeepAdjustedAvg = g.KeepAdjustedAvg,
                            Squad = p.Squad
                        })];
        }
    }
}
