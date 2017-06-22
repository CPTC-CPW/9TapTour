using NineTapTour.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
namespace NineTapTour.Database
{
    class PlayerHistoryDB
    {
        public static void AddPlayerHistory(PlayerHistory temp)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(temp).State = db.PlayerHistory.Any(his => his.hisID == temp.hisID) ?
                         EntityState.Modified :
                         EntityState.Added;
                    db.SaveChanges();
                }
            }
            catch(SqlException ex)
            {
                throw new PlayerHistoryTableException ("Error Number : " + ex.Number + " - " + ex.Message);
            }
        }
    }
}
