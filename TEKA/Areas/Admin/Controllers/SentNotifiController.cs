using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEKA.FCM;
using TEKA.Models;

namespace TEKA.Areas.Admin.Controllers
{
    public class SentNotifiController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        public ActionResult Index(string s1, string s2, string s3, string s4,
            string s5, string s6, string c1, string c2, string c3, string c4, string c5, string c6, int? page)
        {
            if (s1 != null)
            {
                page = 1;
            }
            else
            {
                s1 = c1;
            }
            ViewBag.c1 = s1;
            if (s2 != null)
            {
                page = 1;
            }
            else
            {
                s2 = c2;
            }
            ViewBag.c2 = s2;
            if (s3 != null)
            {
                page = 1;
            }
            else
            {
                s3 = c3;
            }
            ViewBag.c3 = s3;
            if (s4 != null)
            {
                page = 1;
            }
            else
            {
                s4 = c4;
            }
            ViewBag.c4 = s4;
            if (s5 != null)
            {
                page = 1;
            }
            else
            {
                s5 = c5;
            }
            ViewBag.c5 = s5;
            if (s6 != null)
            {
                page = 1;
            }
            else
            {
                s6 = c6;
            }
            ViewBag.c6 = s6;
            var model = from a in db.User_Device
                        select a;
            if (!String.IsNullOrEmpty(s1))
            {
                model = model.Where(a => a.Provider == s1 || a.UserName == s1);
            }
            if (!String.IsNullOrEmpty(s2))
            {
                model = model.Where(a => a.Provider == s2);
            }
            if (!String.IsNullOrEmpty(s4))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s4, MyCultureInfo);
                model = model.Where(a => a.Createdate >= s);
            }
            if (!String.IsNullOrEmpty(s5))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s5, MyCultureInfo);
                model = model.Where(a => a.Createdate <= s);
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Sent()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Sent(SentNotifi sent)
        {
            try
            {
                var list_user = db.User_Device;
                foreach(var item in list_user)
                {
                    var item_send = new SentNotify();
                    item_send.Sent(item.UserName, sent.Message);
                }
                ViewBag.msg = "Đã gửi thành công đến thiết bị";
                return View();
            }catch(Exception ex)
            {

            }
            return View(sent);
        }
    }
}