using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;
using TEKA.Utils;

namespace TEKA.Controllers
{
    public class HomeController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();

        public ActionResult Index()
        {
            ViewBag.des = db.Articles.SingleOrDefault(t => t.Type == 4).Text;
            ViewBag.setup = db.Setups.FirstOrDefault(a => a.Cate == "Hotline").Title;
            return View();
        }

        public ActionResult ChangeCurrentCulture(int id)
        {
            SessionManager.CurrentCulture = id;
            Session["CurrentCulture"] = id;
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public JsonResult GetProduct(string serial)
        {
            if (serial.Length != 0)
            {
                var model = db.Products.Where(a => a.Serial == serial).OrderByDescending(s => s.ExportDate).FirstOrDefault();
                string user = User.Identity.Name;
                if (model == null)
                {
                    return Json(new
                    {
                        success = true,
                        status = false

                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(user))
                    {
                        return Json(new
                        {
                            success = true,
                            status = true,
                            author = true,
                            data = model

                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = true,
                            status = true,
                            author = false,
                            data = model

                        });
                    }

                }
            }
            else
            {
                return Json(new
                {
                    success = false

                });
            }

        }

        public PartialViewResult LoadSlide()
        {
            List<Article> model = db.Articles.Where(t => t.Type == 3).ToList();
            return PartialView(model);
        }
        public PartialViewResult LoadFooter()
        {
            List<Setup> model = db.Setups.ToList();
            return PartialView(model);
        }
        public PartialViewResult LoadLogo()
        {
            List<Setup> model = db.Setups.ToList();
            return PartialView(model);
        }
    }
}