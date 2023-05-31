using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TEKA.Models;
using PagedList;

namespace TEKA.Areas.Admin.Controllers
{
    [Authorize(Roles = "Partner,Admin")]
    public class AgencyController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        // GET: Admin/Agency
        //public ActionResult Index([Bind(Include = "id,StartPage,name,city,county")] Daily daily)
        //{
        //    IEnumerable<TEKA.Models.AspNetUser> User = db.AspNetUsers.Where(t => t.Type == null);

        //    if (!string.IsNullOrEmpty(daily.name))
        //    {
        //        User = User.Where(n => n.NameAgency != null);
        //        User = User.Where(n => Utils.TiengViet.Convert(n.NameAgency).Contains(Utils.TiengViet.Convert(daily.name.ToUpper())));
        //        if (User.Count() == 0)
        //        {
        //            User = User.Where(n => Utils.TiengViet.Convert(n.UserName).Contains(Utils.TiengViet.Convert(daily.name.ToLower())));
        //        }

        //    }
        //    if (!string.IsNullOrEmpty(daily.city) || !string.IsNullOrEmpty(daily.county))
        //    {
        //        User = User.Where(a => a.City == daily.city).Where(b => b.County == daily.county);
        //    }
        //    int Page = daily.id;
        //    int Perpage = 10;
        //    int Row = Page * Perpage;
        //    int count = User.Count();
        //    int DisplayPage = 10;
        //    ViewBag.Row = Row;
        //    ViewBag.SectionTitle = @"<a href=""/Home"">Home</a>";

        //    /**
        //         * hien thi 1 luc 5 page
        //         */
        //    int TotalPage = count / Perpage + (count % Perpage == 0 ? 0 : 1);
        //    String PagingHtml = String.Empty;
        //    if (Page == daily.StartPage + DisplayPage - 1)
        //    {
        //        //tang giai page vd: 0-4 --> 4-8
        //        daily.StartPage += DisplayPage - 1;
        //    }
        //    else if (Page == daily.StartPage)
        //    {
        //        if (daily.StartPage > DisplayPage - 1)
        //        {
        //            daily.StartPage -= DisplayPage - 1;
        //        }
        //        else
        //        {
        //            daily.StartPage = 0;
        //        }
        //    }

        //    for (int i = daily.StartPage; i < DisplayPage + daily.StartPage; i++)
        //    {
        //        if (TotalPage <= i)
        //            break;
        //        String url = Url.Action("Index", new { Id = i, StartPage = daily.StartPage });
        //        if (i == Page)
        //        {
        //            PagingHtml += String.Format(@"<li><a href=""{0}""><strong>{1}</strong></a></li>", url, (i + 1));
        //        }
        //        else
        //        {
        //            PagingHtml += String.Format(@"<li><a href=""{0}"">{1}</a></li>", url, (i + 1));
        //        }
        //    }

        //    ViewBag.PagingHtml = PagingHtml;
        //    ViewBag.Page = daily.id;
        //    daily.AspNetUsers = User.Skip(Row).Take(10).ToList();//danh sach agency
        //    var province = db.Provinces.ToList();//danh sach Tinh/ Thanh pho
        //    ViewBag.province = province;
        //    return View(daily);
        //}

        public ActionResult Index(string s1, string s2, string s3, string c1, string c2, string c3, string s4, string c4, int? page)
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

            //IEnumerable<AspNetUser> m = db.AspNetUsers.Where(x => x.Type == null).OrderBy(x => x.NameAgency);
            var m = from a in db.AspNetUsers
                    where a.Type == null
                    from b in a.AspNetUserRoles
                    where b.RoleId == "Agency"
                    select a;
            if (!String.IsNullOrEmpty(s1))
            {
                m = m.Where(s => s.NameAgency != null && s.NameAgency.ToLower().Contains(s1.ToLower())
                || s.UserName.Contains(s1) || s.PhoneNumber.Contains(s1));
            }
            if (!String.IsNullOrEmpty(s2))
            {
                m = m.Where(s => s.City == s2);
            }
            if (!String.IsNullOrEmpty(s3))
            {
                m = m.Where(s => s.County == s3);
            }
            if (!String.IsNullOrEmpty(s4))
            {
                m = m.Where(s => s.UserName == s4);
            }

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            var province = db.Provinces.ToList();//danh sach Tinh/ Thanh pho
            ViewBag.province = province;
            return View(m.OrderBy(s => s.UserName).ToPagedList(pageNumber, pageSize));
        }

        //ajax lay Quan/ Huyen tu Thanh pho
        [HttpPost]
        public ActionResult GetCity(string name)
        {
            List<TEKA.Models.District> lstcity = new List<TEKA.Models.District>();
            var id = db.Provinces.Where(s => s.Name == name).SingleOrDefault();//get id theo ten

            var provi = db.Districts.Where(x => x.ProvinceId == id.Id).ToList();//get ds quan huyen
            var ress = new List<Ressponse>();//add data vao response
            foreach (var i in provi)
            {
                var res = new Ressponse();
                res.Id = i.Id;
                res.Name = i.Name;
                ress.Add(res);
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(ress);//convert to json
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public class Ressponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // GET: Admin/Agency/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.AspNetUser user = db.AspNetUsers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Admin/Agency/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.AspNetUser user = db.AspNetUsers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.province = db.Provinces.ToList();
            return View(user);
        }

        // POST: Admin/Agency/Edit/5
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnable,LockoutEndDateUtc,LockoutEnable,AccessFailed,UserName,FullName,NameAgency,Birthday,City,County,CodeAgency,TaxCode,ReplaceName,Type,Address")] TEKA.Models.AspNetUser user)
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

        // GET: Admin/Agency/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.AspNetUser user = db.AspNetUsers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Agency/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                // TODO: Add delete logic here
                TEKA.Models.AspNetUser user = db.AspNetUsers.Find(id);
                db.AspNetUsers.Remove(user);
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
