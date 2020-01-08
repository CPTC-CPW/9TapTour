using NineTapTour.Exceptions;
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
   public class NineTapRegionDB
    {
        /// <summary>
        /// Returns a lit of all the NineTapRegions in the database
        /// </summary>
        public static List<NineTapRegion> GetRegionList()
        {
            using (NineTapDB db = new NineTapDB())
            {
                return (from t in db.NineTapRegion
                        select t).ToList();
            }
        }

        /// <summary>
        /// Returns the number of NineTapRegions in the database
        /// </summary>
        public static int GetNumberOfRegions()
        {
            using (NineTapDB db = new NineTapDB())
            {
                return (from t in db.NineTapRegion
                        select t).Count();
            }
            #region Refactored Code
            /*
            List<NineTapRegion> RegionList = new List<NineTapRegion>();
            NineTapRegion current = new NineTapRegion();
            using (var db = new NineTapDb())
            {
                var temp = (from g in db.NineTapRegion
                            select new
                            {
                                g.NineTapRegionID
                            });
                foreach (var v in temp)
                {
                    current.NineTapRegionID = v.NineTapRegionID;
                    RegionList.Add(current);

                }
                return RegionList.Count;
            }
            */
            #endregion
        }

        /// <summary>
        /// Adds the NineTapRegion given to the database
        /// </summary>
        public static void AddRegion(NineTapRegion temp)
        {
            try
            {
                using (var db = new NineTapDB())
                {
					db.NineTapRegion.Add(temp);
                    db.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                throw new PlayerHistoryTableException("Error Number : " + ex.Number + " - " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes the NineTapRegion given from the database
        /// </summary>
        /// <param name="t"></param>
        public static void DeleteRegion(NineTapRegion t)
        {
            // This code was wraped in a try/catch block, but the catch did nothing
            using (var db = new NineTapDB())
            {
                db.Entry(t).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Returns a NineTapRegion with the same regionID as the one given
        /// </summary>
        public static NineTapRegion GetRegionByID ( int regionID)
        {
            NineTapRegion NTR = new NineTapRegion();
            using (var db = new NineTapDB())
            {
                var temp = (from g in db.NineTapRegion
                            select new
                            {
                                g.NineTapRegionID,
                                g.NineTapRegionName
                            });
                foreach (var v in temp)
                {
                    NTR.NineTapRegionName = v.NineTapRegionName;
                    NTR.NineTapRegionID = v.NineTapRegionID;
                }

            }
            return NTR;  
        }
    }
}
