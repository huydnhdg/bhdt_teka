using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;

namespace TEKA.Areas.Admin.Controllers
{
    [Authorize(Roles = "Partner,Admin")]
    public class ManageKeyController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        // GET: Admin/ManagePG
        public ActionResult Index(string s1, string s2, string s3, string s4, string s5, string s6, string c1, string c2, string c3, string c4, string c5, string c6, int? page)
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
            var model = from a in db.AspNetUsers
                        from b in a.AspNetUserRoles
                        where b.RoleId == "Key"
                        select a;
            if (!String.IsNullOrEmpty(s1))
            {
                model = model.Where(s => s.FullName.Contains(s1));
            }
            if (!String.IsNullOrEmpty(s1))
            {
                model = model.Where(s => s.PhoneNumber.Contains(s2));
            }
            if (!String.IsNullOrEmpty(s1))
            {
                model = model.Where(s => s.Email.Contains(s3));
            }
            if (!String.IsNullOrEmpty(s1))
            {
                model = model.Where(s => s.UserName.Contains(s4));
            }
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(model.OrderBy(s => s.UserName).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.AspNetUsers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                // TODO: Add delete logic here
                var user = db.AspNetUsers.Find(id);
                db.AspNetUsers.Remove(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.AspNetUsers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] AspNetUser user)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(user);
            }
            catch
            {
                return View(user);
            }
        }
    }
}