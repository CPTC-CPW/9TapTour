using Microsoft.EntityFrameworkCore;
using NineTapTour.Models;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Database
{
    public static class DoublesTeamDB
    {
        /// <summary>
        /// Returns all DoublesTeam records for the given tournament,
        /// with Member1 and Member2 navigation properties loaded.
        /// </summary>
        public static List<DoublesTeam> GetTeamsByTournament(int tournamentId)
        {
            using var db = new NineTapDb();
            return [.. db.DoublesTeams
                .Include(dt => dt.Member1)
                .Include(dt => dt.Member2)
                .Include(dt => dt.Tournament)
                .Where(dt => dt.Tournament.Id == tournamentId)
                .OrderBy(dt => dt.Id)];
        }

        /// <summary>
        /// Returns true if a pairing between memberId1 and memberId2 already exists in
        /// the tournament (order-independent: (A,B) == (B,A)).
        /// </summary>
        public static bool TeamExists(int tournamentId, int memberId1, int memberId2)
        {
            using var db = new NineTapDb();
            return db.DoublesTeams.Any(dt =>
                dt.Tournament.Id == tournamentId &&
                ((dt.Member1.Id == memberId1 && dt.Member2.Id == memberId2) ||
                 (dt.Member1.Id == memberId2 && dt.Member2.Id == memberId1)));
        }

        /// <summary>
        /// Creates a new doubles pairing for the tournament.
        /// Returns false (without saving) if the pairing already exists or if
        /// both member IDs are the same.
        /// </summary>
        public static bool AddTeam(int tournamentId, int memberId1, int memberId2)
        {
            if (memberId1 == memberId2)
                return false;

            if (TeamExists(tournamentId, memberId1, memberId2))
                return false;

            using var db = new NineTapDb();

            var tournament = db.Tournaments.Find(tournamentId);
            var member1 = db.Members.Find(memberId1);
            var member2 = db.Members.Find(memberId2);

            if (tournament == null || member1 == null || member2 == null)
                return false;

            var team = new DoublesTeam
            {
                Tournament = tournament,
                Member1 = member1,
                Member2 = member2
            };

            db.DoublesTeams.Add(team);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// Removes the DoublesTeam record with the given ID.
        /// </summary>
        public static void RemoveTeam(int teamId)
        {
            using var db = new NineTapDb();
            var team = db.DoublesTeams.Find(teamId);
            if (team != null)
            {
                db.DoublesTeams.Remove(team);
                db.SaveChanges();
            }
        }
    }
}
