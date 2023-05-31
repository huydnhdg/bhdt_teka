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
    public class HelpController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        // GET: Admin/Help
        // GET: Admin/Help/Edit/5
        public ActionResult Index()
        {
            TEKA.Models.Article model = db.Articles.SingleOrDefault(a => a.Type == 1);
            return View(model);
        }

        // POST: Admin/Help/Edit/5
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Id,Title,Description,Image,Url,Alt,Text,CreateDate,CreateBy,Tag,Type,Status")] TEKA.Models.Article model)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    model.CreateDate = DateTime.Now;//thoi gian hien tai
                    model.CreateBy = User.Identity.Name;//user identity
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch
            {
                return View(model);
            }
        }
        
    }
}
