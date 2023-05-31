using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace TEKA.Controllers
{
    [Authorize(Roles ="Agency, PG")]
    public class InfoAgencyController : Controller
    {
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
        // GET: InfoAgency
        public ActionResult Index()
        {
            string id = User.Identity.GetUserId();
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

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnable,LockoutEndDateUtc,LockoutEnable,AccessFailed,UserName,FullName,NameAgency,Birthday,City,County,CodeAgency,TaxCode,ReplaceName,Type")] TEKA.Models.AspNetUser user)
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

        //ajax lay Quan/ Huyen tu Thanh pho
        [HttpPost]
        public ActionResult GetCityList(string name)
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
    }
}