using NineTapTour.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineTapTour.Models;
using Microsoft.EntityFrameworkCore;

namespace NineTapTour.Database
{
   public class NineTapRegionDB
    {
        /// <summary>
        /// Returns a lit of all the NineTapRegions in the database
        /// </summary>
        public static List<NineTapRegion> GetRegionList()
        {
            using (NineTapDb db = new NineTapDb())
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
            using (NineTapDb db = new NineTapDb())
            {
                return (from t in db.NineTapRegion
                        select t).Count();
            }
        }

        /// <summary>
        /// Adds the NineTapRegion given to the database
        /// </summary>
        public static void AddRegion(NineTapRegion temp)
        {
            using (var db = new NineTapDb())
            {
				db.NineTapRegion.Add(temp);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes the NineTapRegion given from the database
        /// </summary>
        /// <param name="t"></param>
        public static void DeleteRegion(NineTapRegion t)
        {
            // This code was wraped in a try/catch block, but the catch did nothing
            using (var db = new NineTapDb())
            {
                db.Entry(t).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Returns a NineTapRegion with the same regionID as the one given
        /// </summary>
        public static NineTapRegion GetRegionByID (int regionID)
        {
            using (var db = new NineTapDb())
            {
                return (from region in db.NineTapRegion
                        where region.NineTapRegionID == regionID
                        select region).Single();
            }
        }
    }
}
