using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;

namespace TEKA.Controllers
{
    [Authorize]
    [RoutePrefix("tho-bao-hanh")]
    public class KTVController : BaseController
    {
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
        [Route]
        public ActionResult Index(string s1, string s2, string s3, string s4, string s5, string s6, string s7, string c1, string c2, string c3, string c4, string c5, string c6, string c7, int? page)
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
            if (s7 != null)
            {
                page = 1;
            }
            else
            {
                s7 = c7;
            }
            ViewBag.c7 = s7;
            string userId = User.Identity.GetUserId();
            var linq = from a in db.AspNetUsers
                       from b in a.AspNetUserRoles
                       where b.RoleId == "KTV"
                       where a.Createby == userId
                       select a;
            IEnumerable<AspNetUser> m = linq.OrderBy(x => x.UserName) as IEnumerable<AspNetUser>;
            if (!String.IsNullOrEmpty(s1))
            {
                m = m.Where(s => s.PhoneNumber == s1);
            }
            if (!String.IsNullOrEmpty(s2))
            {
                m = m.Where(s => s.UserName.ToUpper() == s2.ToUpper());
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(m.ToPagedList(pageNumber, pageSize));
        }

        [Route("edit")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.AspNetUser product = db.AspNetUsers.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] TEKA.Models.AspNetUser user)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {

                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;

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
        [Route("delete")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(user);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}