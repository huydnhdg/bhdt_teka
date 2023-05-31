using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEKA.Areas.Admin.Models;
using TEKA.Models;
using TEKA.Utils;

namespace TEKA.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        protected override void ExecuteCore()
        {
            int culture = 0;

            //user changer lang
            if (this.Session == null || this.Session["CurrentCulture"] == null)
            {
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Culture"], out culture);
                this.Session["CurrentCulture"] = culture;
            }
            else
            {
                culture = (int)this.Session["CurrentCulture"];
            }
            //
            CheckOutdate();
            SessionManager.CurrentCulture = culture;

            base.ExecuteCore();
        }
        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }


        public ActionResult ThrowException()
        {
            throw new NotImplementedException();
        }

        private void CheckOutdate()
        {
            var war = from a in db.Warrantis
                      join b in db.AddKeys on a.Id equals b.IdWarranti into temp
                      from t in temp.DefaultIfEmpty()
                      select new WarrantiModel()
                      {
                          Id = a.Id,
                          Deadline = t.Deadline,
                          Status = a.Status,
                          Outdate = a.Outdate
                      };
            //var sswar = war.Where(a => a.Outdate == true && a.Status == 1);

            DateTime today = DateTime.Now.AddDays(-1);
            var outdate = war.Where(s => s.Deadline != null && s.Deadline < today && s.Outdate == false && (s.Status == 2 || s.Status == 5 || s.Status == 6 || s.Status == 7 || s.Status == 8));

            foreach (var item in outdate)
            {
                var model = db.Warrantis.Find(item.Id);
                model.Outdate = true;
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
        }
    }
}