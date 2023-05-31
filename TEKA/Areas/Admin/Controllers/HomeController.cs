using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEKA.Areas.Admin.Models;
using TEKA.Models;

namespace TEKA.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Partner,Mod,Sale")]
    public class HomeController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        public ActionResult Index(string start_date = "", string end_date = "", string cate = "key", string cate_error = "")
        {
            if (User.IsInRole("Sale"))
            {
                return RedirectToAction("Index","Kichhoat");
            }
            var WARRANTI = from a in db.Warrantis
                           select a;
            if (!string.IsNullOrEmpty(start_date))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(start_date, MyCultureInfo);
                WARRANTI = WARRANTI.Where(a => a.Createdate >= s);
                ViewBag.start_date = start_date;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(end_date, MyCultureInfo);
                WARRANTI = WARRANTI.Where(a => a.Createdate <= s);
                ViewBag.end_date = end_date;
            }
            if (cate == "key")
            {
                DateTime timenow = DateTime.Now.AddDays(-4);
                var key = from a in db.AspNetUsers
                          from b in a.AspNetUserRoles
                          where b.RoleId == "Key"
                          select new HomePage()
                          {
                              Id = a.Id,
                              Key = a.UserName,
                              money = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where y.IdKey == a.Id select y).Sum(am => am.Amount) ?? 0,
                              all = (from x in db.AddKeys where x.IdKey == a.Id select x).Count(),
                              success = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Status == 1 && y.IdKey == a.Id select y).Count(),
                              cancel = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Status == 9 && y.IdKey == a.Id select y).Count(),
                              deadline = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Outdate == true && y.IdKey == a.Id select y).Count(),
                          };
                ViewBag.HomePage = key;
            }
            else if (cate == "cate_service")
            {
                DateTime timenow = DateTime.Now.AddDays(-4);
                var key = from a in WARRANTI.GroupBy(a => a.Category).Select(a => a.FirstOrDefault())
                          select new HomePage()
                          {
                              Id = "",
                              Key = a.Category,
                              money = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Category == a.Category select y).Sum(am => am.Amount) ?? 0,
                              all = (from x in WARRANTI where x.Category == a.Category select x).Count(),
                              success = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Status == 1 && x.Category == a.Category select y).Count(),
                              cancel = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Status == 9 && x.Category == a.Category select y).Count(),
                              deadline = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Outdate == true && x.Category == a.Category select y).Count(),
                          };
                ViewBag.HomePage = key;
            }
            else if (cate == "product_cate")
            {
                DateTime timenow = DateTime.Now.AddDays(-4);
                var key = from a in WARRANTI.GroupBy(a => a.ProductCate).Select(a => a.FirstOrDefault())
                          select new HomePage()
                          {
                              Id = "",
                              Key = a.ProductCate,
                              money = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.ProductCate == a.ProductCate select y).Sum(am => am.Amount) ?? 0,
                              all = (from x in WARRANTI where x.ProductCate == a.ProductCate select x).Count(),
                              success = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Status == 1 && x.ProductCate == a.ProductCate select y).Count(),
                              cancel = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Status == 9 && x.ProductCate == a.ProductCate select y).Count(),
                              deadline = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Outdate == true && x.ProductCate == a.ProductCate select y).Count(),
                          };
                ViewBag.HomePage = key;
            }
            else if (cate == "model")
            {
                if (!string.IsNullOrEmpty(cate_error))
                {
                    DateTime timenow = DateTime.Now.AddDays(-4);
                    var key = from a in WARRANTI.GroupBy(a => a.Model).Select(a => a.FirstOrDefault())
                              select new HomePage()
                              {
                                  Id = "",
                                  Key = a.Model,
                                  money = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Model == a.Model select y).Sum(am => am.Amount) ?? 0,
                                  all = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Model == a.Model && y.CateError == cate_error select x).Count(),
                                  success = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Status == 1 && x.Model == a.Model && y.CateError == cate_error select y).Count(),
                                  cancel = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Status == 9 && x.Model == a.Model && y.CateError == cate_error select y).Count(),
                                  deadline = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Outdate == true && x.Model == a.Model && y.CateError == cate_error select y).Count(),
                              };
                    ViewBag.HomePage = key;
                }
                else
                {
                    DateTime timenow = DateTime.Now.AddDays(-4);
                    var key = from a in WARRANTI.GroupBy(a => a.Model).Select(a => a.FirstOrDefault())
                              select new HomePage()
                              {
                                  Id = "",
                                  Key = a.Model,
                                  money = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Model == a.Model select y).Sum(am => am.Amount) ?? 0,
                                  all = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Model == a.Model select x).Count(),
                                  success = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Status == 1 && x.Model == a.Model select y).Count(),
                                  cancel = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Status == 9 && x.Model == a.Model select y).Count(),
                                  deadline = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where y.Deadline < timenow && (x.Status == 6 || x.Status == 7 || x.Status == 8) && x.Model == a.Model select y).Count(),
                              };
                    ViewBag.HomePage = key;
                }

            }
            else if (cate == "city")
            {
                DateTime timenow = DateTime.Now.AddDays(-4);
                var key = from a in WARRANTI.GroupBy(a => a.Province).Select(a => a.FirstOrDefault())
                          select new HomePage()
                          {
                              Id = "",
                              Key = a.Province,
                              money = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Province == a.Province select y).Sum(am => am.Amount) ?? 0,
                              all = (from x in WARRANTI where x.Province == a.Province select x).Count(),
                              success = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Status == 1 && x.Province == a.Province select y).Count(),
                              cancel = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Status == 9 && x.Province == a.Province select y).Count(),
                              deadline = (from x in WARRANTI join y in db.AddKeys on x.Id equals y.IdWarranti where x.Outdate ==true && x.Province == a.Province select y).Count(),
                          };
                ViewBag.HomePage = key;
            }

            return View();
        }
        public class HomePage
        {
            public string Id { get; set; }
            public string Key { get; set; }
            public int all { get; set; }
            public int success { get; set; }
            public int cancel { get; set; }
            public int deadline { get; set; }
            public int money { get; set; }
        }
        public PartialViewResult LoadNoti()
        {
            var error = db.Warrantis;

            var active = db.Customers.Where(e => e.Status == 11 || e.Status == 111 || e.Status == 1);//active chua xem
            ViewBag.error = error.Count();
            ViewBag.active = active.Count();
            return PartialView();
        }

        public PartialViewResult LoadMenu()
        {
            var m = from a in db.Warrantis
                    join c in db.AddKeys on a.Id equals c.IdWarranti into temp
                    from t in temp.DefaultIfEmpty()
                    select new WarrantiModel()
                    {
                        Id = a.Id,
                        Status = a.Status,
                        Deadline = t.Deadline,
                        Outdate = a.Outdate
                    };
            DateTime today = DateTime.Now.AddDays(-1);
            ViewBag.quahan = m.Where(s => s.Outdate == true).Count();

            return PartialView();
        }
    }
}