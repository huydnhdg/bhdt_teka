using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TEKA.Areas.Admin;
using TEKA.Models;

namespace TEKA.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManagerUserController : BaseController
    {
        
        ApplicationDbContext context = new ApplicationDbContext();
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        // GET: Admin/ManagerUser
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
            IEnumerable<AspNetUser> m = db.AspNetUsers;
            if (!String.IsNullOrEmpty(s1))
            {
                m = db.AspNetUsers.Where(s => s.UserName.Contains(s1));
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(m.OrderBy(a => a.UserName).ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/ManagerUser/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/ManagerUser/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ManagerUser/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/ManagerUser/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.AspNetUser model = db.AspNetUsers.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Admin/ManagerUser/Edit/5
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnable,LockoutEndDateUtc,LockoutEnable,AccessFailed,UserName,FullName,NameAgency,Birthday,City,County,CodeAgency,TaxCode,ReplaceName,Type")] TEKA.Models.AspNetUser user)
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

        public ActionResult EditRole(string id)
        {
            ApplicationUser model = context.Users.Find(id);
            ViewBag.RoleId = new SelectList(context.Roles.ToList().Where(item => model.Roles.FirstOrDefault(r => r.RoleId == item.Id) == null).ToList(), "Id", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToRole(string UserId,string[] RoleId)
        {
            ApplicationUser model = context.Users.Find(UserId);
            if (RoleId != null && RoleId.Count() > 0)
            {
                foreach (string item in RoleId)
                {
                    IdentityRole role = context.Roles.Find(RoleId);
                    model.Roles.Add(new IdentityUserRole() { UserId = UserId, RoleId = item });
                }
                context.SaveChanges();
            }
            ViewBag.RoleId = new SelectList(context.Roles.ToList().Where(item => model.Roles.FirstOrDefault(r => r.RoleId == item.Id) == null).ToList(), "Id", "Name");
            return RedirectToAction("EditRole", new { Id = UserId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleFromUser(string UserId,string RoleId)
        {
            ApplicationUser model = context.Users.Find(UserId);
            model.Roles.Remove(model.Roles.Single(m => m.RoleId == RoleId));
            context.SaveChanges();
            ViewBag.RoleId = new SelectList(context.Roles.ToList().Where(item => model.Roles.FirstOrDefault(r => r.RoleId == item.Id) == null).ToList(), "Id", "Name");
            return RedirectToAction("EditRole", new { Id = UserId });
        }

        // GET: Admin/ManagerUser/Delete/5
        public ActionResult Delete(string id)
        {
            var model = db.AspNetUsers.Find(id);
            return View(model);
        }

        // POST: Admin/ManagerUser/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser model = null;
            try
            {
                model = context.Users.Find(id);
                context.Users.Remove(model);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Delete", model);
            }
        }
    }
}
