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
    public class GameDB
    {
        public static void AddGame(Game temp)
        {
            try
            {
                using (var db = new NineTapDb())
                {
                    db.Entry(temp).State = db.Participants.Any(GAME => GAME.Id == temp.Id) ?
                        EntityState.Modified :
                        EntityState.Added;
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                throw new GameTableException("Error Number : " + " - " + ex.Message);
            }
        }
    }
}
