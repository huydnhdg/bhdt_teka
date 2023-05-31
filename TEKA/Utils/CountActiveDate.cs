using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.Utils
{
    public class CountActiveDate
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
        public void CountDate()
        {
            try
            {
                foreach (var p in db.Products)
                {

                    if (p.EndDate > DateTime.Now)
                    {

                        //product
                        p.Status = 2;
                        db.Entry(p).State = System.Data.Entity.EntityState.Modified;
                        //customer
                        var c = db.Customers.SingleOrDefault(s => s.Serial == p.Serial);
                        c.Type = 1;
                        db.Entry(c).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {
                logger.Info(e);

            }
            
        }
    }
}