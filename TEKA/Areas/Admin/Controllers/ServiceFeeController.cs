using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;

namespace TEKA.Areas.Admin.Controllers
{
    public class ServiceFeeController : Controller
    {
        private TEKAEntities db = new TEKAEntities();
        public static List<Service_Fee> ls = new List<Service_Fee>();
        public static IEnumerable<Service_Fee> data = null;
        // GET: Admin/ServiceFee
        public ActionResult Index(string s1, int? page , string s3 , string c1, string c3)
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

            if (s3 != null)
            {
                page = 1;
            }
            else
            {
                s3 = c3;
            }
            ViewBag.c3 = s3;
            var m = db.Service_Fee.OrderByDescending(a => a.Name).Take(100);
            if (!String.IsNullOrEmpty(s1))
            {
                m = db.Service_Fee.Where(s => s.Name.Contains(s1));
            }
            if (!String.IsNullOrEmpty(s3))
            {
                m = m.Where(x => x.Status.ToString() == s3);
            }

            data = m as IEnumerable<Service_Fee>;
            ls = m.ToList();
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(m.OrderByDescending(a => a.Status).ToPagedList(pageNumber, pageSize));
    
        }

        // GET: Admin/ServiceFee/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service_Fee service_Fee = db.Service_Fee.Find(id);
            if (service_Fee == null)
            {
                return HttpNotFound();
            }
            return View(service_Fee);
        }

        // GET: Admin/ServiceFee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ServiceFee/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Status,Warfee,Charfee,Pubfee")] Service_Fee service_Fee)
        {
            if (ModelState.IsValid)
            {
                db.Service_Fee.Add(service_Fee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(service_Fee);
        }

        // GET: Admin/ServiceFee/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service_Fee service_Fee = db.Service_Fee.Find(id);
            if (service_Fee == null)
            {
                return HttpNotFound();
            }
            return View(service_Fee);
        }

        // POST: Admin/ServiceFee/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Status,Warfee,Charfee,Pubfee")] Service_Fee service_Fee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(service_Fee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(service_Fee);
        }

        // GET: Admin/ServiceFee/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service_Fee service_Fee = db.Service_Fee.Find(id);
            if (service_Fee == null)
            {
                return HttpNotFound();
            }
            return View(service_Fee);
        }

        // POST: Admin/ServiceFee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Service_Fee service_Fee = db.Service_Fee.Find(id);
            db.Service_Fee.Remove(service_Fee);
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
