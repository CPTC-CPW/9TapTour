using NineTapTour.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Database
{
   public class NineTapRegionDB
    {
        public static List<NineTapRegion> GetRegionList()
        {
            using (NineTapDb db = new NineTapDb())
            {
                return (from t in db.NineTapRegion
                        select t).ToList();
            }
        }

        public static int getNumberOfRegions()
        {

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
        }


        public static void AddRegion(NineTapRegion temp)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(temp).State = db.NineTapRegion.Any(his => his.NineTapRegionID == temp.NineTapRegionID) ?
                         EntityState.Modified :
                         EntityState.Added;
                    db.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                throw new PlayerHistoryTableException("Error Number : " + ex.Number + " - " + ex.Message);
            }
        }

        public static void deleteRegion(NineTapRegion t)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(t).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch
            {

            }

        }

        public static NineTapRegion getRegionByID ( int regionID)
        {
            NineTapRegion NTR = new NineTapRegion();
            using (var db = new NineTapDb())
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
