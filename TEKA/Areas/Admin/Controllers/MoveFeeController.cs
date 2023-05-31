using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;

namespace TEKA.Areas.Admin.Controllers
{
    [Authorize(Roles = "Partner,Admin")]
    public class MoveFeeController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        public ActionResult Index(string s1, string s2, string s3, string s4, string s5, string s6, string c1, string c2,
            string c3, string c4, string c5, string c6, int? page)
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
            var model = db.MoveFees.OrderBy(a => a.Price);
            var data = model as IEnumerable<MoveFee>;
            if (!String.IsNullOrEmpty(s1))
            {
                data = model.Where(a => a.Cate.Contains(s1));
            }


            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(data.OrderBy(a => a.Cate).ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Product/Edit/5
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "")] TEKA.Models.MoveFee move)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {

                    db.MoveFees.Add(move);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(move);
            }
            catch
            {
                return View(move);
            }
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.MoveFee move = db.MoveFees.Find(id);
            if (move == null)
            {
                return HttpNotFound();
            }
            return View(move);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] TEKA.Models.MoveFee move)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {

                    db.Entry(move).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(move);
            }
            catch
            {
                return View(move);
            }
        }

        // GET: Admin/Product/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.MoveFee move = db.MoveFees.Find(id);
            if (move == null)
            {
                return HttpNotFound();
            }
            return View(move);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                // TODO: Add delete logic here
                TEKA.Models.MoveFee move = db.MoveFees.Find(id);
                db.MoveFees.Remove(move);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}