using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineTapTour.Models;

namespace NineTapTour.Database
{
    class FinalizeTempDB
    {
        /***************************************************************
        * LEAGUE AVERAGE
        ****************************************************************/
        public static double LeagueAverage(Member mem)
        {
            double sum = 0;
            double average = 0;
            var db = new NineTapDb();
            var temp = (

                        from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        join g in db.Games on p.Game.Id equals g.Id
                        join t in db.Tournaments on p.Tournament.Id equals t.Id
                        where mem.Id == m.Id
                        orderby t.Date descending
                        select new
                        {
                            t.Date,
                            g.Game1,
                            g.Game2,
                            g.Game3,
                            g.Game4,
                            Average = (g.Game1 + g.Game2 + g.Game3 + g.Game4) / 4

                        }).Take(30).ToList();
            if (temp.Count > 0)
            {
                foreach (var item in temp)
                {
                    sum += Convert.ToDouble(item.Average);
                }
                return (average = sum / temp.Count());
            }
            return 0;
        }

        //calculates league average for member based off last 30 games or total games played if less than 30.
        public static double LeagueAverage(int memID)
        {
            double sum = 0;
            double average = 0;
            var db = new NineTapDb();
            var temp = (
                        from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        join g in db.Games on p.Game.Id equals g.Id
                        join t in db.Tournaments on p.Tournament.Id equals t.Id
                        where memID == m.Id
                        orderby t.Date descending
                        select new
                        {
                            t.Date,
                            g.Game1,
                            g.Game2,
                            g.Game3,
                            g.Game4,
                            Average = (g.Game1 + g.Game2 + g.Game3 + g.Game4) / 4
                        }).Take(30).ToList();

            if (temp.Count > 0)
            {
                foreach (var item in temp)
                {
                    sum += Convert.ToDouble(item.Average);
                }
                return (average = sum / temp.Count());
            }
            return 0;
        }

        public static double LeagueAvgFromPlayerHistory(int mem, int howmany, int regionid)
        {
            var db = new NineTapDb();
            // Calculates the Sum as the query instead of grabing all the data
            double sum = (from p in db.PlayerHistory
                          where p.MemberNumber == mem && p.regionID == regionid
                          orderby p.TournamentDate descending
                          select p.AverageForGame).Take(howmany).Sum();
            return sum;
            /*
            var temp = (from p in db.PlayerHistory
                        where p.MemberNumber == mem && p.regionID == regionid
                        orderby p.TournamentDate descending
                        select new
                        {
                            p.TournamentDate,
                            p.Game1,
                            p.Game2,
                            p.Game3,
                            p.Game4,
                            p.trueAVG,
                            p.AverageForGame
                        }).Take(howmany).ToList();
            if (temp.Count > 0)
            {

                foreach (var item in temp)
                {
                    sum += Convert.ToDouble(item.AverageForGame);
                }
                return sum;
            }
            return 0;
            */

        }

        /***************************************************
         * FINALIZETEMP
         * ************************************************/

        public static void AddFinalizeTemp(FinalizeTemp temp)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    //checks if tournament is new or already existing in db
                    if (!db.FinalizeTemp.Any(f => f.GameId == temp.GameId))
                    {
                        db.Entry(temp).State = EntityState.Added;
                        /*************************************************************************
                        updates the handicap of a member that participated in the tournament in the database 
                        ***There is a problem in the database's member's average, so it was not 
                           used, but I believe it should be
                           -The problem might be when a tournament record is added, it is not 
                           updating the member's average in the database.
                        *************************************************************************/
                        db.Members.First(x => x.Id == temp.MemberId).Handicap = Calculations.Calculations.CalculateHandicapPins(Convert.ToInt16(LeagueAverage(db.Members.First(x => x.Id == temp.MemberId))));
                        /************************************************************************/
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(temp).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error Number : " + ex.Number + " - " + ex.Message);
            }
        }

        public static FinalizeTemp getFinalizeID(Game currentG)
        {
            FinalizeTemp ft = new FinalizeTemp();
            var db = new NineTapDb();
            var temp = (

                from par in db.FinalizeTemp
                where par.GameId == currentG.Id
                select new
                {

                    par.AdjustedAvg,
                    par.Bonus,
                    par.FinalizeID,
                    par.FirstName,
                    par.Game1,
                    par.Game2,
                    par.Game3,
                    par.Game4,
                    par.GameAvg,
                    par.GameId,
                    par.Handicap,
                    par.KeepAdjustedAvg,
                    par.LastName,
                    par.LeagueAverage,
                    par.MemberId,
                    par.Notes,
                    par.ScratchTotal,
                    par.Squad,
                    par.TournamentID,
                    par.UseGame1,
                    par.UseGame2,
                    par.UseGame3,
                    par.UseGame4
                });
            foreach (var i in temp)
            {
                ft.AdjustedAvg = i.AdjustedAvg;
                ft.Bonus = i.Bonus;
                ft.FinalizeID = i.FinalizeID;
                ft.FirstName = i.FirstName;
                ft.Game1 = i.Game1;
                ft.Game2 = i.Game2;
                ft.Game3 = i.Game3;
                ft.Game4 = i.Game4;
                ft.GameAvg = i.GameAvg;
                ft.GameId = i.GameId;
                ft.Handicap = i.Handicap;
                ft.KeepAdjustedAvg = i.KeepAdjustedAvg;
                ft.LastName = i.LastName;
                ft.LeagueAverage = i.LeagueAverage;
                ft.MemberId = i.MemberId;
                ft.Notes = i.Notes;
                ft.ScratchTotal = i.ScratchTotal;
                ft.Squad = i.Squad;
                ft.TournamentID = i.TournamentID;
                ft.UseGame1 = i.UseGame1;
                ft.UseGame2 = i.UseGame2;
                ft.UseGame3 = i.UseGame3;
                ft.UseGame4 = i.UseGame4;
               
            }
            return ft;
        }

        /// <summary>
        /// THis method Gets a list of all participant objects for the tournament passed into method.
        /// </summary>
        /// <param name="tourn"> represent the tournament you want list of particpants from</param>
        /// <returns>List of Participants for specific tournament</returns>
        public static List<FinalizeTemp> GetAllInitialParticipantGameList(Tournament tourn)
        {
            var db = new NineTapDb();
            List<FinalizeTemp> ParticipantList = new List<FinalizeTemp>();
            var temp = (from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        join g in db.Games on p.Game.Id equals g.Id
                        join t in db.Tournaments on p.Tournament.Id equals t.Id
                        where tourn.Id == p.Tournament.Id
                        orderby m.FirstName descending
                        select new
                        {
                            g.Id,
                            m.FirstName,
                            m.LastName,
                            MemberId = m.Id,
                            p.Squad,
                            g.Game1,
                            g.Game2,
                            g.Game3,
                            g.Game4,
                            g.UseGame1,
                            g.UseGame2,
                            g.UseGame3,
                            g.UseGame4,
                            g.Notes,
                            g.Handicap,
                            g.Bonus,
                            m.Number,
                            t.TourneyRegion
                        }).ToList();
            foreach (var item in temp)
            {
                int gplayed = 0;
                FinalizeTemp NewParticipant = new FinalizeTemp();
                NewParticipant.GameId = item.Id;
                NewParticipant.MemberId = item.MemberId;
                NewParticipant.FirstName = item.FirstName;
                NewParticipant.LastName = item.LastName;
                NewParticipant.Game1 = item.Game1;
                NewParticipant.Game2 = item.Game2;
                NewParticipant.Game3 = item.Game3;
                NewParticipant.Game4 = item.Game4;

                NewParticipant.UseGame1 = item.UseGame1 ?? item.Game1.HasValue;
                NewParticipant.UseGame2 = item.UseGame2 ?? item.Game2.HasValue;
                NewParticipant.UseGame3 = item.UseGame3 ?? item.Game3.HasValue;
                NewParticipant.UseGame4 = item.UseGame4 ?? item.Game4.HasValue;

                if (item.Game1.HasValue)
                {
                    gplayed++;
                }

                if (item.Game2.HasValue)
                {
                    gplayed++;
                }

                if (item.Game3.HasValue)
                {
                    gplayed++;
                }

                if (item.Game4.HasValue)
                {
                    gplayed++;
                }

                NewParticipant.Notes = item.Notes;
                NewParticipant.ScratchTotal = (item.Game1 ?? 0) + (item.Game2 ?? 0) + (item.Game3 ?? 0) + (item.Game4 ?? 0);
                NewParticipant.Squad = item.Squad;
                NewParticipant.GameAvg = ((item.Game1 ?? 0) + (item.Game2 ?? 0) + (item.Game3 ?? 0) + (item.Game4 ?? 0)) / gplayed;
                NewParticipant.Handicap = item.Handicap ?? 0;
                NewParticipant.Bonus = item.Bonus ?? 0;

                int hTotal = (item.Game1 != null) ? ((item.Game1 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0)) : 0;
                hTotal += (item.Game2 != null) ? ((item.Game2 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0)) : 0;
                hTotal += (item.Game3 != null) ? ((item.Game3 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0)) : 0;
                hTotal += (item.Game4 != null) ? ((item.Game4 ?? 0) + (item.Handicap ?? 0) + (item.Bonus ?? 0)) : 0;

                NewParticipant.HandicapTotal = hTotal;

                NewParticipant.memberNumber = item.Number;
                NewParticipant.FinalizeRegionID = item.TourneyRegion;

                ParticipantList.Add(NewParticipant);
            }

            return ParticipantList;
        }

        //makes a list from the finalizetemp table to be used in dataview source
        public static List<FinalizeTemp> GetListFromTable(Tournament tourn)
        {
            var db = new NineTapDb();
            //get list of participants by tournament
            return db.FinalizeTemp
                            .Where(p => p.TournamentID == tourn.Id)
                            .OrderBy(p => p.FirstName)
                            .ThenBy(p => p.Squad)
                            .ToList();
        }

        public static List<FinalizeTemp> GetFinalizeListByRegionID(int RegionID)
        {
            using (var db = new NineTapDb())
            {
                return (from f in db.FinalizeTemp
                        where f.FinalizeRegionID == RegionID
                        select f).ToList();
            }
        }

        public static void DeleteFinalizeTemp(FinalizeTemp ft)
        {
            using (var db = new NineTapDb())
            {
                db.Entry(ft).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }


        /**********************************************************
         * PARTICIPANT
         * *******************************************************/

        public static List<Participant> getGameParticipantList(int id)
        {
            List<Participant> p = new List<Participant>();
            var db = new NineTapDb();
            var temp = (

                from par in db.Participants
                where par.Tournament.Id == id
                select new
                {
                    par.Id,
                    par.Game,
                    par.Member,
                    par.Squad,
                    par.Tournament
                }).ToList();


            return p;
        }

        /// <summary>
        /// Retrieves a single participant from a tournament based on given gameID.
        /// Return null if no participant is found
        /// </summary>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public static Participant GetParticipantByGameId (int gameID)
        {
            var db = new NineTapDb();
            return (from par in db.Participants
                   where par.Game.Id == gameID
                   select par).Include(nameof(Member)).SingleOrDefault();
        }

        public static List<Participant> GetparticpantListByRegionID(int RegionID)
        {
            using (var db = new NineTapDb())
            {
                return (from p in db.Participants
                        where p.ParticipantRegionID == RegionID
                        select p).Include(nameof(Participant.Member)).ToList();
            }
        }

        public static void deleteParticipant(Participant p)
        {
            using (var db = new NineTapDb())
            {
                db.Entry(p).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        public static List<Game> GetGameListByRegionID(int RegionID)
        {
            using (var db = new NineTapDb())
            {
                return (from g in db.Games
                        where g.gameRegionID == RegionID
                        select g).ToList();
            }
        }

        /// <summary>
        /// Returns the total amount of comp entries for the tournament
        /// </summary>
        /// <param name="tourneyId">The id property for the tournament</param>
        /// <returns>Qty of comp entries</returns>
        public static int GetCompEntryQtyByTourneyID(int tourneyId)
        {
            var db = new NineTapDb();
            return db.FinalizeTemp
                    .Join(db.Games,
                        ft => ft.GameId,
                        g => g.Id,
                        (ft, g) => new { g.IsComp, ft.TournamentID })
                    .Where(ftg => ftg.TournamentID == tourneyId && ftg.IsComp)
                    .Count();
        }

        /// <summary>
        /// Gets the entry quantity for a member in a tournament
        /// </summary>
        /// <returns>the amount of enties in tournament by a member</returns>
        public static int GetMembersGameEntryCount(int tourneyId, int memberNum)
        {
            var db = new NineTapDb();
            return db.FinalizeTemp
                    .Where(ft => ft.TournamentID == tourneyId && ft.memberNumber == memberNum)
                    .Count();
        }

        public static bool GameExists(PlayerHistory Temp)
        {

            using (var db = new NineTapDb())
            {

                if (db.FinalizeTemp.Any(m => m.GameId == Temp.GameID))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }        
    }
}

