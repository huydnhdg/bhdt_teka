using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;

namespace TEKA.Controllers
{
    [RoutePrefix("historyactive")]
    [Authorize]
    public class HistoryActiveController : BaseController
    {
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
        // GET: HistoryActive
        [Route]
        public ActionResult Index()
        {
            var pg = User.Identity.Name;
            var model = db.Customers.Where(s => s.CreateBy == pg);
            var cus = new Khachhang()
            {
                Customers = model.OrderBy(s => s.ActiveDate).ToList()
            };
            return View(cus);
        }
    }
}