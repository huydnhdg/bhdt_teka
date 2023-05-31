using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;

namespace TEKA.Areas.Admin.Controllers
{
    public class LOG_MOController : Controller
    {
        private TEKAEntities db = new TEKAEntities();
        public static List<LOG_MO> ls = new List<LOG_MO>();
        // GET: Admin/LOG_MO
        public ActionResult Index(string s1, string s2, string s3, string s4, string s5, string s6, string c1, string c2, string c3, string c4, string c5, string c6, int? page)
        {
            s6 = Utils.ConvertTo.FormatPhoneStartWith84(s6);
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
            var m = from a in db.LOG_MO.OrderByDescending(a => a.Createdate)
                    select a;
            if (!String.IsNullOrEmpty(s4))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s4, MyCultureInfo);
                m = m.Where(a => a.Createdate >= s);
            }
            if (!String.IsNullOrEmpty(s5))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s5, MyCultureInfo);
                m = m.Where(a => a.Createdate <= s);
            }
            if (!String.IsNullOrEmpty(s6))
            {
                m = m.Where(a => a.User_Id.Contains(s6));
            }
            IEnumerable<LOG_MO> data = m as IEnumerable<LOG_MO>;
            ls = m.ToList();
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/LOG_MO/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOG_MO lOG_MO = db.LOG_MO.Find(id);
            if (lOG_MO == null)
            {
                return HttpNotFound();
            }
            return View(lOG_MO);
        }

        // GET: Admin/LOG_MO/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/LOG_MO/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,User_Id,Service_Id,Request_Id,Message,Createdate,Status,Response")] LOG_MO lOG_MO)
        {
            if (ModelState.IsValid)
            {
                db.LOG_MO.Add(lOG_MO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(lOG_MO);
        }

        // GET: Admin/LOG_MO/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOG_MO lOG_MO = db.LOG_MO.Find(id);
            if (lOG_MO == null)
            {
                return HttpNotFound();
            }
            return View(lOG_MO);
        }

        // POST: Admin/LOG_MO/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,User_Id,Service_Id,Request_Id,Message,Createdate,Status,Response")] LOG_MO lOG_MO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lOG_MO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lOG_MO);
        }

        // GET: Admin/LOG_MO/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOG_MO lOG_MO = db.LOG_MO.Find(id);
            if (lOG_MO == null)
            {
                return HttpNotFound();
            }
            return View(lOG_MO);
        }

        // POST: Admin/LOG_MO/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            LOG_MO lOG_MO = db.LOG_MO.Find(id);
            db.LOG_MO.Remove(lOG_MO);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
