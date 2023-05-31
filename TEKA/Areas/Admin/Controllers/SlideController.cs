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
    public class SlideController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        // GET: Admin/Question
        public ActionResult Index(int id = 0, int StartPage = 0)
        {
            IEnumerable<TEKA.Models.Article> Articles = db.Articles.Where(t => t.Type == 3);
            int Page = id;
            int Perpage = 10;
            int Row = Page * Perpage;
            int count = Articles.Count();
            int DisplayPage = 10;
            ViewBag.Row = Row;
            ViewBag.SectionTitle = @"<a href=""/Home"">Home</a>";

            /**
                 * hien thi 1 luc 5 page
                 */
            int TotalPage = count / Perpage + (count % Perpage == 0 ? 0 : 1);
            String PagingHtml = String.Empty;
            if (Page == StartPage + DisplayPage - 1)
            {
                //tang giai page vd: 0-4 --> 4-8
                StartPage += DisplayPage - 1;
            }
            else if (Page == StartPage)
            {
                if (StartPage > DisplayPage - 1)
                {
                    StartPage -= DisplayPage - 1;
                }
                else
                {
                    StartPage = 0;
                }
            }

            for (int i = StartPage; i < DisplayPage + StartPage; i++)
            {
                if (TotalPage <= i)
                    break;
                String url = Url.Action("Index", new { Id = i, StartPage = StartPage });
                if (i == Page)
                {
                    PagingHtml += String.Format(@"<li><a href=""{0}""><strong>{1}</strong></a></li>", url, (i + 1));
                }
                else
                {
                    PagingHtml += String.Format(@"<li><a href=""{0}"">{1}</a></li>", url, (i + 1));
                }
            }

            ViewBag.PagingHtml = PagingHtml;
            ViewBag.Page = id;
            var model = Articles.Skip(Row).Take(10).ToList();
            return View(model);
        }

        // GET: Admin/Question/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Question/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Question/Create
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Image,Url,Alt,Text,CreateDate,CreateBy,Tag,Type,Status")] TEKA.Models.Article model)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    model.CreateDate = DateTime.Now;//thoi gian hien tai
                    model.CreateBy = User.Identity.Name;//user identity
                    model.Type = 3;
                    db.Articles.Add(model);
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

        // GET: Admin/Question/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.Article model = db.Articles.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
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
                    model.Type = 3;
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

        // GET: Admin/Article/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.Article user = db.Articles.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Article/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long? id)
        {
            try
            {
                // TODO: Add delete logic here
                TEKA.Models.Article user = db.Articles.Find(id);
                db.Articles.Remove(user);
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
