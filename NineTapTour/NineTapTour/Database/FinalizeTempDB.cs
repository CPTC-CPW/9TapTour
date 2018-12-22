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

        /***************************************************************
        calculates the average
        ***note I saw this method twice now and this is the third one
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

        public static void DeleteFinilizeTemp (FinalizeTemp ft)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(ft).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch
            {

            }



        }

        public static Participant getParticipantbyGameID (int gameID)
        {
            Participant p = new Participant();
            var db = new NineTapDb();
            var temp = (

                from g in db.Participants
                where g.Game.Id == gameID
                select new
                {
                    g.Game,
                    g.Id,
                    g.Member,
                    g.Squad,
                    g.Tournament,

                });
            foreach (var g in temp)
            {
                p.Game = g.Game;
                p.Id = g.Id;
                p.Member = g.Member;
                p.Squad = g.Squad;
                p.Tournament = g.Tournament;

            }
            return p;

        }
        public static void deleteParticipant (Participant p)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(p).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch
            {

            }

        }

        public static List<FinalizeTemp> GetFinalizeList(int RegionID)
        {
            using (var db = new NineTapDb())
            {
                return (from f in db.FinalizeTemp
                        where f.FinalizeRegionID == RegionID
                        select f).ToList();
            }
        }

        public static List<Participant> GetparticpantList(int RegionID)
        {
            using (var db = new NineTapDb())
            {
                return (from p in db.Participants
                        where p.ParticipantRegionID == RegionID
                        select p).ToList();
            }
        }

        public static List<Game> GetGameList(int RegionID)
        {
            using (var db = new NineTapDb())
            {
                return (from g in db.Games
                        where g.gameRegionID == RegionID
                        select g).ToList();
            }
        }

        public static int GetCompEntryQtyByTourney(int tourneyId)
        {
            var db = new NineTapDb();
            return db.FinalizeTemp
                    .Join(db.Games,
                        ft => ft.GameId,
                        g => g.Id,
                        (ft, g) => new { g.IsComp, ft.TournamentID })
                    .Where(ftg => ftg.TournamentID == tourneyId && ftg.IsComp)
                    .Count();


                    //.Include(t => t.Participant.Select(p => p.Game).Where(g => g.IsComp))
                    //.Where(t => t.Id == tourneyId)
                    //.Count();
        }
    }





}

