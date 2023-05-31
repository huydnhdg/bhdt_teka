using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TEKA.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManagerRoleController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        // GET: Admin/ManagerRole
        public ActionResult Index()
        {
            var model = context.Roles.AsEnumerable();
            return View(model);
        }

        // GET: Admin/ManagerRole/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/ManagerRole/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ManagerRole/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IdentityRole role)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    context.Roles.Add(role);
                    context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(role);
        }

        // GET: Admin/ManagerRole/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/ManagerRole/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/ManagerRole/Delete/5
        public ActionResult Delete(string id)
        {
            var model = context.Roles.Find(id);
            return View(model);
        }

        // POST: Admin/ManagerRole/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            IdentityRole model = null;
            try
            {
                model = context.Roles.Find(id);
                context.Roles.Remove(model);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }
    }
}
