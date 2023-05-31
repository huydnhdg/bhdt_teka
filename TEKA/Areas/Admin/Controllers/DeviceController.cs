using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TEKA.Areas.Admin.Models;
using TEKA.Models;

namespace TEKA.Areas.Admin.Controllers
{
    public class DeviceController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
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
            var m = db.Devices.Take(10000);
            if (!String.IsNullOrEmpty(s1))
            {
                m = db.Devices.Where(s => s.Name == s1);
            }
            if (!String.IsNullOrEmpty(s2))
            {
                m = db.Devices.Where(s => s.Code.Equals(s2.ToUpper()));
            }
            
            if (!String.IsNullOrEmpty(s4))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s4, MyCultureInfo);
                m = m.Where(a => a.Createdate >= s);
            }
            if (!String.IsNullOrEmpty(s5))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s5, MyCultureInfo);
                m = m.Where(a => a.Createdate <= s);
            }
           
            IEnumerable<Device> data = m as IEnumerable<Device>;
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(data.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DeviceSendKey(string s1, string s2, string s3, string s4, string s5, string s6, string c1, string c2, string c3, string c4, string c5, string c6, int? page)
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
            var m = from a in db.DeviceSendKeys
                    join b in db.Devices on a.IdDevice equals b.Id
                    join c in db.AspNetUsers on a.IdKey equals c.Id
                    select new DeviceSendKeyModel()
                    {
                        ID = a.ID,
                        IdDevice = a.IdDevice,
                        IdKey = a.IdKey,
                        Quantity = a.Quantity,
                        Price = a.Price,
                        Amount = a.Amount,
                        Note = a.Note,
                        Code = b.Code,
                        Name = b.Name,
                        Key = c.UserName,
                        Createdate = a.Createdate,
                        Createby = a.Createby
                    };
            if (!String.IsNullOrEmpty(s1))
            {
                m = m.Where(a => a.Name == s1 || a.Code == s1 || a.Key == s1|| a.Note == s1);
            }

            if (!String.IsNullOrEmpty(s4))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s4, MyCultureInfo);
                m = m.Where(a => a.Createdate >= s);
            }
            if (!String.IsNullOrEmpty(s5))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s5, MyCultureInfo);
                m = m.Where(a => a.Createdate <= s);
            }

            IEnumerable<DeviceSendKeyModel> data = m as IEnumerable<DeviceSendKeyModel>;
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(data.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult SendKey()
        {
            var device = db.Devices;
            var key = from a in db.AspNetUsers from b in a.AspNetUserRoles where b.RoleId == "Key" select a;
            TempData["device"] = device.ToList();
            TempData["key"] = key.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendKey([Bind(Include = "")] DeviceSendKey deviceSendKey)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    deviceSendKey.Createdate = DateTime.Now;
                    deviceSendKey.Createby = User.Identity.Name;
                    db.DeviceSendKeys.Add(deviceSendKey);
                    db.SaveChanges();
                    return RedirectToAction("DeviceSendKey","Device");
                }
                return View(deviceSendKey);
            }
            catch
            {
                return View(deviceSendKey);
            }
        }
        [HttpPost]
        public JsonResult getprice(long? id)
        {
            var device = db.Devices.Find(id);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(device);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteSendKey(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceSendKey deviceSendKey = db.DeviceSendKeys.Find(id);
            if (deviceSendKey == null)
            {
                return HttpNotFound();
            }
            return View(deviceSendKey);
        }

        // POST: Admin/Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSendKeyConfirmed(long id)
        {
            DeviceSendKey deviceSendKey = db.DeviceSendKeys.Find(id);
            db.DeviceSendKeys.Remove(deviceSendKey);
            db.SaveChanges();
            return RedirectToAction("DeviceSendKey","Device");
        }

        public ActionResult EditSendKey(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceSendKey deviceSendKey = db.DeviceSendKeys.Find(id);
            if (deviceSendKey == null)
            {
                return HttpNotFound();
            }
            return View(deviceSendKey);
        }

        // POST: Admin/Devices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSendKey([Bind(Include = "")] DeviceSendKey deviceSendKey)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deviceSendKey).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DeviceSendKey","Device");
            }
            return View(deviceSendKey);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "")] Device device)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    device.Createdate = DateTime.Now;
                    device.Createby = User.Identity.Name;
                    db.Devices.Add(device);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(device);
            }
            catch
            {
                return View(device);
            }
        }

        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        // POST: Admin/Devices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] Device device)
        {
            if (ModelState.IsValid)
            {
                db.Entry(device).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(device);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        // POST: Admin/Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Device device = db.Devices.Find(id);
            db.Devices.Remove(device);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}