using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;

namespace TEKA.Controllers
{
    [RoutePrefix("tra-cuu-bao-hanh")]
    public class SearchWarrantiController : BaseController
    {
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
        [Route]
        public ActionResult Index(string textSearch)
        {
            if (!string.IsNullOrEmpty(textSearch))
            {
                ViewBag.textSearch = textSearch;
                var model = from a in db.Warrantis
                            join b in db.AddKeys on a.Id equals b.IdWarranti into temp
                            from bt in temp.DefaultIfEmpty()

                            join k in db.AspNetUsers on bt.IdKey equals k.Id into temp2
                            from t2 in temp2.DefaultIfEmpty()

                            join kt in db.AspNetUsers on bt.IdKTV equals kt.Id into temp3
                            from t3 in temp3.DefaultIfEmpty()

                            select new WarrantiSearch()
                            {
                                Phone = a.Phone,
                                Status = a.Status,
                                CodeWarr = a.CodeWarr,
                                ProductCate = a.ProductCate,
                                Category = a.Category,
                                Serial = a.Serial,
                                Model = a.Model,
                                Createdate = a.Createdate,
                                Note = a.Note,
                                Activedate = a.Activedate,
                                Limited = a.Limited,
                                KeyWarranti = t2.UserName,
                                KTV = t3.UserName,
                                Comback = bt.Comeback,
                            };

                WarrantiSearch warranti = new WarrantiSearch();
                warranti = model.OrderByDescending(a => a.Createdate).FirstOrDefault(a => a.Phone == textSearch || a.Serial == textSearch || a.CodeWarr == textSearch);
                if (warranti == null)
                {
                    ViewBag.message = "NOT";
                    return View();
                }
                return View(warranti);
            }
            return View();
        }
    }
}