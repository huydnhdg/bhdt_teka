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
    public class DescriptionHomeController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        // GET: Admin/Question
        // GET: Admin/Question/Edit/5
        public ActionResult Edit()
        {
            Article model = db.Articles.SingleOrDefault(t => t.Type == 4);
            return View(model);
        }

        // POST: Admin/Question/Edit/5
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Image,Url,Alt,Text,CreateDate,CreateBy,Tag,Type,Status")] TEKA.Models.Article model)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    model.CreateDate = DateTime.Now;//thoi gian hien tai
                    model.CreateBy = User.Identity.Name;//user identity
                    model.Type = 4;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return View(model);
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
