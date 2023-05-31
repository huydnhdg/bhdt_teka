using ImageResizer;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TEKA.Areas.Admin.Models;
using TEKA.FCM;
using TEKA.Models;
using TEKA.Utils;

namespace TEKA.Controllers
{
    [Authorize]
    public class WarrantiController : BaseController
    {
        
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
        public ActionResult Index(int? page, int? status, string start_date, string end_date, string textString, string textString1, string textString2, string channel,string cr_date , string sdate, string edate)
        {           

            var m = from a in db.Warrantis
                    join b in db.Products on a.IdProduct equals b.Id into tempp
                    from tp in tempp.DefaultIfEmpty()
                    join c in db.AddKeys on a.Id equals c.IdWarranti into temp
                    from t in temp.DefaultIfEmpty()

                    join k in db.AspNetUsers on t.IdKey equals k.Id into temp2
                    from t2 in temp2.DefaultIfEmpty()

                    join dev in db.AspNetUsers on t.IdKTV equals dev.Id into temp1
                    from t1 in temp1.DefaultIfEmpty()
                    select new WarrantiModel()
                    {
                        Id = a.Id,
                        CodeWarr = a.CodeWarr,
                        Name = a.Name,
                        Address = a.Address,
                        District = a.District,
                        Province = a.Province,
                        Phone = a.Phone,
                        PhoneWarr = a.PhoneWarr,
                        Note = a.Note,
                        Status = a.Status,
                        Outdate = a.Outdate,
                        Createdate = a.Createdate,
                        Chanel = a.Chanel,
                        Createby = a.Createby,
                        Extra = a.Extra,
                        ProductCate = a.ProductCate,
                        Category = a.Category,

                        Product = a.Product,
                        Serial = a.Serial,
                        Model = a.Model,
                        Agent = a.Agent,

                        Exportdate = tp.ExportDate,
                        Activedate = tp.ActiveDate,
                        Limited = tp.Limited,
                        Buydate = a.Buydate,
                        Amount = t.Amount,

                        //
                        IdKey = t.Id,
                        Senddate = t.Createdate,
                        Deadline = t.Deadline,
                        Key = t2.UserName,

                        //
                        Successdate = t.Successdate,
                        KTVNote = t.Note,
                        Cost = t.Cost,
                        Comback = t.Comeback,
                        KTV = t1.UserName,
                        //
                        Devices = db.Phutung_Warranti.Where(a => a.IdKey == t.Id).ToList(),
                        Image = db.ImageWarrs.Where(a => a.IDKey == t.Id).Select(a => a.Image).ToList(),

                        KM = t.KM + " " + t.MoveFee,
                        Sign = t.Sign

                    };
            DateTime today = DateTime.Now.AddDays(-1);
            var outdate = m.Where(s => s.Deadline != null && s.Deadline < today && s.Outdate == false && (s.Status == 2 || s.Status == 5 || s.Status == 6 || s.Status == 7 || s.Status == 8));
            if (outdate.Count() > 0)
            {
                foreach (var item in outdate)
                {
                    var model = db.Warrantis.Find(item.Id);
                    model.Outdate = true;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }

            string _user = User.Identity.Name;
            if (User.IsInRole("Key"))
            {
                m = m.Where(a => a.Key == _user);
            }
            if (User.IsInRole("KTV"))
            {
                m = m.Where(a => a.KTV == _user);
            }
            if (User.IsInRole("Agency"))
            {
                m = m.Where(a => a.Createby == _user);
            }


            ViewBag.all = m.Count();

            ViewBag.moi = m.Where(s => s.Status == 0 && s.Outdate == false).Count();
            ViewBag.chuyen = m.Where(s => s.Status == 2 && s.Outdate == false).Count();
            ViewBag.tuchoi = m.Where(s => s.Status == 3 && s.Outdate == false).Count();
            ViewBag.xuly = m.Where(s => s.Status == 5 && s.Outdate == false).Count();
            ViewBag.demve = m.Where(s => s.Status == 6 && s.Outdate == false).Count();
            ViewBag.doilinhkien = m.Where(s => s.Status == 7 && s.Outdate == false).Count();
            ViewBag.chophanhoi = m.Where(s => s.Status == 8 && s.Outdate == false).Count();
            ViewBag.hoanthanh = m.Where(s => s.Status == 1 && s.Outdate == false).Count();

            ViewBag.quahan = m.Where(s => s.Outdate == true).Count();
            ViewBag.huybo = m.Where(s => s.Status == 9 && s.Outdate == false).Count();

            if (!String.IsNullOrEmpty(textString))
            {
                m = m.Where(a => a.CodeWarr.Contains(textString)
                || a.Comback.Contains(textString)
                || a.Extra.Contains(textString)
                || a.Note.Contains(textString)
                || a.Key == textString
                || a.KTV == textString
                || a.Createby == textString
                );
                ViewBag.textString = textString;
            }
            if (!String.IsNullOrEmpty(textString1))
            {
                m = m.Where(a => a.Phone.Contains(textString1)
                || a.PhoneWarr.Contains(textString1)
                || a.Address.Contains(textString1)
                || a.District.Contains(textString1)
                || a.Province.Contains(textString1)
                );
                ViewBag.textString1 = textString1;
            }
            if (!String.IsNullOrEmpty(textString2))
            {
                m = m.Where(a => a.Product.Contains(textString2)
                || a.Serial.Contains(textString2)
                || a.Model.Contains(textString2)
                || a.ProductCate.Contains(textString2)
                );
                ViewBag.textString2 = textString2;
            }
            if (!String.IsNullOrEmpty(channel))
            {
                m = m.Where(a => a.Category == channel);
                ViewBag.channel = channel;
            }
            if (!String.IsNullOrEmpty(start_date))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(start_date, MyCultureInfo);
                m = m.Where(a => a.Createdate >= s);
                ViewBag.start_date = start_date;
            }
            if (!String.IsNullOrEmpty(end_date))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(end_date, MyCultureInfo);
                m = m.Where(a => a.Createdate <= s);
                ViewBag.end_date = end_date;
            }
            if (!String.IsNullOrEmpty(sdate))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(sdate, MyCultureInfo);
                m = m.Where(a => a.Buydate >= s);
                ViewBag.sdate = sdate;
            }
            if (!String.IsNullOrEmpty(edate))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(edate, MyCultureInfo);
                m = m.Where(a => a.Buydate <= s);
                ViewBag.edate = edate;
            }
            if (status != null)
            {
                if (status == 10)
                {
                    m = m.Where(a => a.Outdate == true);                    
                    ViewBag.status = status;
                }
                else
                {
                    m = m.Where(a => a.Status == status && a.Outdate == false);
                    ViewBag.status = status;
                }

            }
            if (!String.IsNullOrEmpty(cr_date))
            {
                int _s3 = int.Parse(cr_date);
                m = m.Where(a => a.Outdate == true && a.Status == _s3);

                ViewBag.cr_date = cr_date;
            }

            var cr_url = new CurrentURL()
            {
                UserName = User.Identity.Name,
                URL = Request.Url.AbsoluteUri
            };
            db.CurrentURLs.Add(cr_url);
            db.SaveChanges();

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(m.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Success(long Id)
        {
            var model = db.AddKeys.Find(Id);
            var warr = db.Warrantis.Find(model.IdWarranti);
            ViewBag.code = warr.CodeWarr;
            var note = new NoteModel()
            {
                status = warr.Status,
                Id = model.Id,
                IdWarranti = model.IdWarranti,
                IdKey = model.IdKey,
                Createdate = model.Createdate,
                Deadline = model.Deadline,
                Successdate = model.Successdate,
                Note = model.Note,
                Cost = model.Cost ?? 0,
                Comeback = model.Comeback,
                Device = model.Device,
                IdKTV = model.IdKTV,
                Image1 = model.Image1,
                Image2 = model.Image2,
                Image3 = model.Image3,
                Image4 = model.Image4,
                Image5 = model.Image5,
                Image6 = model.Image6,
                KM = model.KM,
                MoveFee = model.MoveFee ?? 0,
                Price = model.Price ?? 0,
                Amount = model.Amount ?? 0,
                CateError = model.CateError,
                Lat = model.Lat,
                Long = model.Long,
                Name_price = model.Name_price,
                Service_price = model.Service_price,
                Distance = model.Distance,
                DistanceFee = model.DistanceFee,
                TravelFee = model.TravelFee,
                AccessaryFee = model.AccessaryFee,
                Category = warr.Category,
                CharFee = model.CharFee,
                PubFee = model.PubFee,
                ListPhutung = db.Phutung_Warranti.Where(a => a.IdKey == model.Id).ToList(),
                Checkin = db.ImageWarrs.Where(a => a.IDKey == model.Id && a.Type == 1).ToList(),
                Checkout = db.ImageWarrs.Where(a => a.IDKey == model.Id && a.Type == 2).ToList(),
            };
            TempData["km"] = db.MoveFees.ToList();
            TempData["device"] = db.Devices.ToList();
            return View(note);
        }
        [HttpPost]
        public ActionResult Product(long id)
        {

            var model = db.Warrantis.Find(id);
            return PartialView("~/Views/Warranti/_Product.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Product([Bind(Include = "")] Warranti warranti)
        {
            if (ModelState.IsValid)
            {

                db.Entry(warranti).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Product", "Warranti");

        }
        [HttpPost]
        public ActionResult Details(long id)
        {
            var model = db.LogWarantis.Where(a => a.IdWarranti == id);
            return PartialView("~/Views/Warranti/_Details.cshtml", model);
        }
        [HttpPost]
        public ActionResult DisplayImage(long IdAddkey)
        {
            var listImage = db.ImageWarrs.Where(a => a.IDKey == IdAddkey).ToList();
            return PartialView("~/Views/Warranti/_DisplayImage.cshtml", listImage);
        }
        public ActionResult viewimage_newtab(long id)
        {
            var listImage = db.ImageWarrs.Where(a => a.IDKey == id).ToList();
            //return PartialView("~/Views/Warranti/_DisplayImage.cshtml", listImage);
            return View(listImage);
        }
        [HttpPost]
        public ActionResult Notes(long id)
        {

            var model = db.AddKeys.Find(id);
            var warr = db.Warrantis.Find(model.IdWarranti);

            var note = new NoteModel()
            {
                status = warr.Status,
                Id = model.Id,
                IdWarranti = model.IdWarranti,
                IdKey = model.IdKey,
                Createdate = model.Createdate,
                Deadline = model.Deadline,
                Successdate = model.Successdate,
                Note = model.Note,
                Cost = model.Cost,
                Comeback = model.Comeback,
                Device = model.Device,
                IdKTV = model.IdKTV,
                Image1 = model.Image1,
                Image2 = model.Image2,
                Image3 = model.Image3,
                Image4 = model.Image4,
                Image5 = model.Image5,
                Image6 = model.Image6,
                KM = model.KM,
                MoveFee = model.MoveFee ?? 0,
                Price = model.Price ?? 0,
                Amount = model.Amount ?? 0,
                CateError = model.CateError,
                Category = warr.Category,
                CharFee = model.CharFee,
                PubFee = model.PubFee,
                ListPhutung = db.Phutung_Warranti.Where(a => a.IdKey == model.Id).ToList(),
                Checkin = db.ImageWarrs.Where(a => a.IDKey == model.Id && a.Type == 1).ToList(),
                Checkout = db.ImageWarrs.Where(a => a.IDKey == model.Id && a.Type == 2).ToList(),
            };
            return PartialView("~/Views/Warranti/_Update.cshtml", note);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Success([Bind(Include = "")] NoteModel noteModel,
            IEnumerable<HttpPostedFileBase> checkin, IEnumerable<HttpPostedFileBase> checkout)
        {

            try
            {
                
                var addkey = db.AddKeys.Find(noteModel.Id);
                var warr = db.Warrantis.Find(addkey.IdWarranti);

                addkey.Successdate = noteModel.Successdate;
                if (noteModel.Successdate != null && warr.Createdate == null)
                {
                    warr.CountDay = (noteModel.Successdate.Value.Date - warr.Createdate.Value.Date).Days;
                }
                addkey.Note = noteModel.Note;
                addkey.Cost = noteModel.Cost;
                addkey.Comeback = noteModel.Comeback;
                addkey.Device = noteModel.Device;
                addkey.KM = noteModel.KM;
                addkey.MoveFee = noteModel.MoveFee;
                addkey.Price = noteModel.Price;
                addkey.Amount = noteModel.Amount;
                addkey.CateError = noteModel.CateError;
                addkey.Deadline = noteModel.Deadline;
                warr.Category = noteModel.Category;
                addkey.CateError = noteModel.CateError;
                addkey.Name_price = noteModel.Name_price;
                addkey.Service_price = noteModel.Service_price;
                addkey.AccessaryFee = noteModel.AccessaryFee;
                addkey.TravelFee = noteModel.TravelFee;
                addkey.Distance = noteModel.Distance;
                addkey.DistanceFee = noteModel.DistanceFee;
                addkey.CharFee = noteModel.CharFee;
                addkey.PubFee = noteModel.PubFee;
                if (noteModel.ListPhutung.Count() > 0)
                {
                    var phutungold = db.Phutung_Warranti.Where(a => a.IdKey == addkey.Id);
                    //có phụ tùng mới
                    if (phutungold.Count() < noteModel.ListPhutung.Count())
                    {
                        if (phutungold.Count() > 0)
                        {
                            db.Phutung_Warranti.RemoveRange(phutungold);
                        }

                        foreach (var item in noteModel.ListPhutung)
                        {
                            var phutung = new Phutung_Warranti()
                            {
                                IdKey = addkey.Id,
                                Name = item.Name,
                                Price = item.Price
                            };
                            db.Phutung_Warranti.Add(phutung);
                        }
                    }
                }

                //luu anh vao phieu
                var listimage = new List<ImageWarr>();

                foreach (var file in checkin)
                {
                    if (file != null)
                    {
                        string strrand = Guid.NewGuid().ToString();
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/ImageWarr/"), strrand + "_" + fileName);
                        file.SaveAs(path);
                        ResizeSettings resizeSetting = new ResizeSettings
                        {
                            MaxWidth = 800,
                            MaxHeight = 800,
                        };
                        ImageBuilder.Current.Build(path, path, resizeSetting);
                        string link = "/ImageWarr/" + strrand + "_" + fileName;
                        var image = new ImageWarr()
                        {
                            Image = link,
                            IDKey = addkey.Id,
                            Type = 1
                        };
                        listimage.Add(image);
                    }
                }
                db.ImageWarrs.AddRange(listimage);
                foreach (var file in checkout)
                {
                    if (file != null)
                    {
                        string strrand = Guid.NewGuid().ToString();
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/ImageWarr"), strrand + "_" + fileName);
                        file.SaveAs(path);
                        ResizeSettings resizeSetting = new ResizeSettings
                        {
                            MaxWidth = 800,
                            MaxHeight = 800,
                        };
                        ImageBuilder.Current.Build(path, path, resizeSetting);
                        string link = "/ImageWarr/" + strrand + "_" + fileName;
                        var image = new ImageWarr()
                        {
                            Image = link,
                            IDKey = addkey.Id,
                            Type = 2
                        };
                        listimage.Add(image);
                    }
                }
                db.ImageWarrs.AddRange(listimage);


                //cap nhat chi tiet phieu
                db.Entry(addkey).State = System.Data.Entity.EntityState.Modified;

                

                //cap nhat trang thai phieu
                if (noteModel.status == 1 || noteModel.status == 9)
                {
                    warr.Outdate = false;
                    warr.Status = noteModel.status;
                    db.Entry(warr).State = System.Data.Entity.EntityState.Modified;
                    var log = new LogWaranti()
                    {
                        Createdate = DateTime.Now,
                        IdWarranti = addkey.IdWarranti,
                        Detail = string.Format(Common.warranti_complete, User.Identity.Name, Utils.Robot.getnamestatus(noteModel.status))
                    };
                    db.LogWarantis.Add(log);
                    //luu log trang thai phieu
                }
                else if (warr.Status != noteModel.status)
                {

                    warr.Status = noteModel.status;
                    db.Entry(warr).State = System.Data.Entity.EntityState.Modified;
                    var log = new LogWaranti()
                    {
                        Createdate = DateTime.Now,
                        IdWarranti = addkey.IdWarranti,
                        Detail = string.Format(Common.warranti_changestatus, User.Identity.Name, Utils.Robot.getnamestatus(noteModel.status), noteModel.Deadline.Value.ToString("dd/MM/yyyy"))
                    };
                    db.LogWarantis.Add(log);
                    //luu log trang thai phieu
                }
                else if (warr.Status == noteModel.status)
                {
                    //nếu có linh kiện thì chuyển luôn sang chờ phản hồi
                    if (noteModel.ListPhutung.Count() > 0 && warr.Status < 8)
                    {
                        warr.Status = 8;
                    }
                    bool ischange = false;
                    //lưu thông tin nếu đổi ngày deadline
                    if (addkey.Deadline.Value.Date != noteModel.Deadline.Value.Date)
                    {
                        ischange = true;
                    }
                    string changes = string.Format(Common.change_deadline, User.Identity.Name, noteModel.Deadline.Value.ToString("dd/MM/yyyy"));
                    string edit = string.Format(Common.warranti_edit, User.Identity.Name);
                    var log = new LogWaranti()
                    {
                        Createdate = DateTime.Now,
                        IdWarranti = addkey.IdWarranti,
                        Detail = ischange == true ? changes : edit

                    };
                    db.LogWarantis.Add(log);
                }

                db.SaveChanges();
                var crurl = db.CurrentURLs.OrderByDescending(a => a.ID).FirstOrDefault(a => a.UserName == User.Identity.Name);
                return Redirect(crurl.URL);
            }
            catch (Exception ex)
            {
                return View(noteModel);
            }

        }

        public ActionResult EditNew(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warranti warranti = db.Warrantis.Find(id);
            if (warranti == null)
            {
                return HttpNotFound();
            }
            TempData["province"] = db.Provinces.ToList();
            var ag = from a in db.AspNetUsers
                     from b in a.AspNetUserRoles
                     where b.RoleId == "Agency"
                     select a;
            TempData["agent"] = ag.Select(a => a.UserName).ToList();
            //drop product cate
            var cate = db.Models.Select(a => a.ProductCate);
            var model = cate.GroupBy(a => a).Where(b => b.Count() > 0).Select(g => g.Key);
            var list = new List<String>();
            list.Add("Bổ sung sau");
            list.AddRange(model.ToList());
            TempData["cate"] = list;
            //drop model
            var modelitem = db.Models.Where(a => a.ProductCate == warranti.ProductCate);
            var listmodel = new List<String>();
            listmodel.Add("Bổ sung sau");
            if (modelitem.Count() > 0)
            {
                listmodel = modelitem.Select(a => a.Model1).ToList();
            }
            TempData["model"] = listmodel;
            return View(warranti);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNew([Bind(Include = "")] Warranti warranti)
        {
            if (ModelState.IsValid)
            {
                var log = new LogWaranti()
                {
                    Createdate = DateTime.Now,
                    IdWarranti = warranti.Id,
                    Detail = string.Format(Common.warranti_edit, User.Identity.Name)
                };
                db.LogWarantis.Add(log);

                db.Entry(warranti).State = EntityState.Modified;
                db.SaveChanges();
                var crurl = db.CurrentURLs.OrderByDescending(a => a.ID).FirstOrDefault(a => a.UserName == User.Identity.Name);
                return Redirect(crurl.URL);
            }
            TempData["province"] = db.Provinces.ToList();
            var ag = from a in db.AspNetUsers
                     from b in a.AspNetUserRoles
                     where b.RoleId == "Agency"
                     select a;
            TempData["agent"] = ag.Select(a => a.UserName).ToList();
            //drop product cate
            var cate = db.Models.Select(a => a.ProductCate);
            var model = cate.GroupBy(a => a).Where(b => b.Count() > 0).Select(g => g.Key);
            var list = new List<String>();
            list.Add("Bổ sung sau");
            list.AddRange(model.ToList());
            TempData["cate"] = list;
            //drop model
            var modelitem = db.Models.Where(a => a.ProductCate == warranti.ProductCate);
            var listmodel = new List<String>();
            listmodel.Add("Bổ sung sau");
            if (modelitem.Count() > 0)
            {
                listmodel = modelitem.Select(a => a.Model1).ToList();
            }
            TempData["model"] = listmodel;
            return View(warranti);
        }
        [HttpPost]
        public JsonResult AutoCompleteCountry(string term)
        {
            var result = (from r in db.Devices
                          where r.Name.ToLower().Contains(term.ToLower()) || r.Code.ToLower().Contains(term.ToLower())
                          select new { r.Name }).Distinct();
            return Json(result.OrderBy(a => a.Name).Take(20), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AutoCompleteSerFee(string term, string cateer)
        {
            var result = (from r in db.Service_Fee
                          where r.Name.ToLower().Contains(term.ToLower()) && r.Status == cateer
                          select new { r.Name, r.Warfee, r.Charfee, r.Pubfee }).Distinct();
            return Json(result.OrderBy(a => a.Name).Take(20), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DistancePrice(string distance)
        {
            if (!string.IsNullOrEmpty(distance))
            {
                int model = int.Parse(distance);
                if (model <= 15)
                {
                    return Json(0);
                }
                else if (model >= 50)
                {
                    return Json(-1);
                }
                else
                {
                    int temp = model - 15;
                    int total = temp * 10000;
                    return Json(total);
                }
            }
            return Json(0);
        }
        [HttpPost]
        public ActionResult GetKMPrice(string km)
        {
            var listPT = db.MoveFees;
            var model = listPT.Where(a => a.Cate == km).SingleOrDefault();
            return Json(model.Price);
        }
        [HttpPost]
        public ActionResult Getprice(string cate)
        {
            var model = from a in db.Devices
                        select a;

            var device = model.SingleOrDefault(a => a.Name.Contains(cate) || a.Code.Contains(cate));

            return Json(device.Price, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteImage(long Id)
        {
            var image = db.ImageWarrs.Find(Id);
            db.ImageWarrs.Remove(image);
            db.SaveChanges();
            return Redirect("/Warranti/Success/" + image.IDKey);
        }
        [HttpPost]
        public ActionResult AddKTV(long id)
        {
            string userId = User.Identity.GetUserId();
            var key = from a in db.AspNetUsers
                      from b in a.AspNetUserRoles
                      where b.RoleId == "KTV" && a.Createby == userId

                      select a;
            TempData["ktv"] = key.ToList();
            var warranti = db.AddKeys.Find(id);
            return PartialView("~/Views/Warranti/_AddKTV.cshtml", warranti);
        }

        public ActionResult AddKTV_newtab(long id)
        {
            string userId = User.Identity.GetUserId();
            var key = from a in db.AspNetUsers
                      from b in a.AspNetUserRoles
                      where b.RoleId == "KTV" && a.Createby == userId

                      select a;
            TempData["ktv"] = key.ToList();
            var addkey = db.AddKeys.Find(id);

            return View(addkey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddKTVConfirm([Bind(Include = "")] AddKey addKey)
        {
            if (ModelState.IsValid)
            {
                //add thông tin thợ vào phiếu
                var model = db.AddKeys.Find(addKey.Id);
                model.IdKTV = addKey.IdKTV;
                model.Deadline = addKey.Deadline;

                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                //thay đổi trạng thái tự động khi chuyến phiếu cho thợ
                var warranti = db.Warrantis.Find(model.IdWarranti);
                warranti.Status = 5;//dang xu ly
                db.Entry(warranti).State = EntityState.Modified;

                var user = db.AspNetUsers.Find(addKey.IdKTV);
                var log = new LogWaranti()
                {
                    Createdate = DateTime.Now,
                    IdWarranti = model.IdWarranti,
                    Detail = string.Format(Common.warranti_add_ktv, User.Identity.Name, user.UserName, model.Deadline.Value.ToString("dd/MM/yyyy"))
                };
                db.LogWarantis.Add(log);
                //gửi 1 thông báo cho thợ khi nhận được phiếu
                var userKTV = db.AspNetUsers.Find(addKey.IdKTV);
                var sent = new SentNotify();
                sent.Sent(userKTV.UserName, string.Format("Bạn nhận được 1 yêu cầu xử lý cho phiếu có mã {0}", warranti.CodeWarr));

                db.SaveChanges();

                var crurl = db.CurrentURLs.OrderByDescending(a => a.ID).FirstOrDefault(a => a.UserName == User.Identity.Name);
                return Redirect(crurl.URL);
            }

            return View("addktv_newtab");

        }

        public void DatlichExport(int? status, string start_date, string end_date, string textString, string textString1, string textString2, string channel, string cr_date)
        {
            var m = from a in db.Warrantis
                    join b in db.Products on a.IdProduct equals b.Id into tempp
                    from tp in tempp.DefaultIfEmpty()

                    join c in db.AddKeys on a.Id equals c.IdWarranti into temp
                    from t in temp.DefaultIfEmpty()

                    join k in db.AspNetUsers on t.IdKey equals k.Id into temp2
                    from t2 in temp2.DefaultIfEmpty()

                    join dev in db.AspNetUsers on t.IdKTV equals dev.Id into temp1
                    from t1 in temp1.DefaultIfEmpty()
                    select new WarrantiModel()
                    {
                        Id = a.Id,
                        CodeWarr = a.CodeWarr,
                        Name = a.Name,
                        Address = a.Address,
                        District = a.District,
                        Province = a.Province,
                        Phone = a.Phone,
                        PhoneWarr = a.PhoneWarr,
                        Note = a.Note,
                        Status = a.Status,
                        Outdate = a.Outdate,
                        Createdate = a.Createdate,
                        Chanel = a.Chanel,
                        Createby = a.Createby,
                        Extra = a.Extra,
                        ProductCate = a.ProductCate,
                        Category = a.Category,

                        Product = a.Product,
                        Serial = a.Serial,
                        Model = a.Model,
                        Agent = a.Agent,

                        Exportdate = tp.ExportDate,
                        Activedate = tp.ActiveDate,
                        Limited = tp.Limited,
                        Buydate = a.Buydate,


                        //
                        IdKey = t.Id,
                        Senddate = t.Createdate,
                        Deadline = t.Deadline,
                        Key = t2.UserName,

                        //
                        Successdate = t.Successdate,
                        KTVNote = t.Note,
                        Cost = t.Cost,
                        Comback = t.Comeback,
                        KTV = t1.UserName,
                        //
                        Devices = db.Phutung_Warranti.Where(a => a.IdKey == t.Id).ToList(),
                        Image = db.ImageWarrs.Where(a => a.IDKey == t.Id).Select(a => a.Image).ToList(),

                        KM = t.KM + " " + t.MoveFee,
                        Sign = t.Sign

                    };
            if (!String.IsNullOrEmpty(textString))
            {
                m = m.Where(a => a.CodeWarr.Contains(textString)
                || a.Comback.Contains(textString)
                || a.Extra.Contains(textString)
                || a.Note.Contains(textString)
                || a.Key == textString
                || a.KTV == textString
                || a.Createby == textString
                );
                ViewBag.textString = textString;
            }
            if (!String.IsNullOrEmpty(textString1))
            {
                m = m.Where(a => a.Phone.Contains(textString1)
                || a.PhoneWarr.Contains(textString1)
                || a.Address.Contains(textString1)
                || a.District.Contains(textString1)
                || a.Province.Contains(textString1)
                );
                ViewBag.textString1 = textString1;
            }
            if (!String.IsNullOrEmpty(textString2))
            {
                m = m.Where(a => a.Product.Contains(textString2)
                || a.Serial.Contains(textString2)
                || a.Model.Contains(textString2)
                || a.ProductCate.Contains(textString2)
                );
                ViewBag.textString2 = textString2;
            }
            if (!String.IsNullOrEmpty(channel))
            {
                m = m.Where(a => a.Category == channel);
                ViewBag.channel = channel;
            }
            if (!String.IsNullOrEmpty(start_date))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(start_date, MyCultureInfo);
                m = m.Where(a => a.Createdate >= s);
                ViewBag.start_date = start_date;
            }
            if (!String.IsNullOrEmpty(end_date))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(end_date, MyCultureInfo);
                m = m.Where(a => a.Createdate <= s);
                ViewBag.end_date = end_date;
            }
            if (status != null)
            {
                if (status == 10)
                {
                    m = m.Where(a => a.Outdate == true);
                    
                    ViewBag.status = status;
                }
                else
                {
                    m = m.Where(a => a.Status == status && a.Outdate == false);
                    ViewBag.status = status;
                }

            }
            if (!String.IsNullOrEmpty(cr_date))
            {
                int _s3 = int.Parse(cr_date);
                m = m.Where(a => a.Outdate == true && a.Status == _s3);

                ViewBag.cr_date = cr_date;
            }
            string _user = User.Identity.Name;
            if (User.IsInRole("Key"))
            {
                m = m.Where(a => a.Key == _user);
            }
            if (User.IsInRole("KTV"))
            {
                m = m.Where(a => a.KTV == _user);
            }
            if (User.IsInRole("Agency"))
            {
                m = m.Where(a => a.Createby == _user);
            }

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "No./Số lịch";
            Sheet.Cells["B1"].Value = "Ngày nhận lịch";
            Sheet.Cells["C1"].Value = "Tên khách hàng";
            Sheet.Cells["D1"].Value = "Địa chỉ";
            Sheet.Cells["E1"].Value = "Quận/Huyện";
            Sheet.Cells["F1"].Value = "Tỉnh/Thành phố";
            Sheet.Cells["G1"].Value = "ĐT";
            Sheet.Cells["H1"].Value = "Nhóm sản phẩm";
            Sheet.Cells["I1"].Value = "Model";
            Sheet.Cells["J1"].Value = "Serial";
            Sheet.Cells["K1"].Value = "Code";
            Sheet.Cells["L1"].Value = "Ngày mua";
            Sheet.Cells["M1"].Value = "Đại lý";
            Sheet.Cells["N1"].Value = "Tình trạng nhận";
            Sheet.Cells["O1"].Value = "Loại dịch vụ";
            Sheet.Cells["P1"].Value = "Ngày yêu cầu xử lý";
            Sheet.Cells["Q1"].Value = "Trạm thực hiện";
            Sheet.Cells["R1"].Value = "Kỹ thuật";
            Sheet.Cells["S1"].Value = "Kết quả xử lý";
            Sheet.Cells["T1"].Value = "Yêu cầu đặt biệt";
            Sheet.Cells["U1"].Value = "Code phụ tùng";
            Sheet.Cells["V1"].Value = "Trạng thái lịch";


            int row = 2;
            foreach (var item in m)
            {
                string cate = (string.IsNullOrEmpty(item.ProductCate)) ? ("Bổ sung sau") : (item.ProductCate);
                string model = (item.Model == null) ? ("Bổ sung sau") : (item.Model);
                string agent = (item.Agent == null) ? ("Bổ sung sau") : (item.Agent);

                string buydate = (item.Buydate == null) ? ("") : (item.Buydate.GetValueOrDefault().ToString("dd/MM/yyyy"));
                string deadline = (item.Deadline == null) ? ("") : (item.Deadline.GetValueOrDefault().ToString("dd/MM/yyyy"));

                string code = "";
                var product = db.Products.FirstOrDefault(a => a.Serial == item.Serial);
                if (product != null)
                {
                    code = product.Code;
                }
                Sheet.Cells[string.Format("A{0}", row)].Value = item.CodeWarr;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Createdate.GetValueOrDefault().ToString("dd/MM/yyyy");
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Name;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Address;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.District;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Province;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Phone + " " + item.PhoneWarr;
                Sheet.Cells[string.Format("H{0}", row)].Value = cate;
                Sheet.Cells[string.Format("I{0}", row)].Value = model;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.Serial;
                Sheet.Cells[string.Format("K{0}", row)].Value = code;
                Sheet.Cells[string.Format("L{0}", row)].Value = buydate;
                Sheet.Cells[string.Format("M{0}", row)].Value = agent;
                Sheet.Cells[string.Format("N{0}", row)].Value = item.Note;
                Sheet.Cells[string.Format("O{0}", row)].Value = item.Category;
                Sheet.Cells[string.Format("P{0}", row)].Value = deadline;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.Key;
                Sheet.Cells[string.Format("R{0}", row)].Value = item.KTV;
                Sheet.Cells[string.Format("S{0}", row)].Value = item.Comback;
                Sheet.Cells[string.Format("T{0}", row)].Value = item.Extra;
                //Sheet.Cells[string.Format("U{0}", row)].Value = item.Device;
                Sheet.Cells[string.Format("V{0}", row)].Value = getStatus(item.Status);
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        string getStatus(int? status)
        {
            string str = "";
            switch (status)
            {
                case (0):
                    str = "Mới tạo";
                    break;
                case (1):
                    str = "Hoàn thành";
                    break;
                case (2):
                    str = "Chuyển trạm";
                    break;
                case (3):
                    str = "Trạm từ chối";
                    break;
                case (4):
                    str = "";
                    break;
                case (5):
                    str = "Đang xử lý";
                    break;
                case (6):
                    str = "Đem về trạm";
                    break;
                case (7):
                    str = "Đợi linh kiện";
                    break;
                case (8):
                    str = "Chờ phản hồi";
                    break;
                case (9):
                    str = "Hủy bỏ";
                    break;
                default:
                    return str;
            }
            return str;
        }

    }
}