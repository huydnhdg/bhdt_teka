using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using ImageResizer;
using NLog;
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
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using TEKA.Areas.Admin.Models;
using TEKA.FCM;
using TEKA.Models;
using TEKA.Utils;

namespace TEKA.Areas.Admin.Controllers
{
    [Authorize(Roles = "Partner,Admin,Mod")]
    public class ErrorController : BaseController
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        public ActionResult Index(int? page, int? status, string start_date, string end_date, string textString, string textString1, string textString2, string channel, string cr_date, string outdate)
        {
            
            var m = from a in db.Warrantis
                    //join b in db.Products on a.Serial equals b.Serial into tempp
                    //from tp in tempp.DefaultIfEmpty()
                    join c in db.AddKeys on a.Id equals c.IdWarranti into temp
                    from t in temp.DefaultIfEmpty()
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

                        //Exportdate = tp.ExportDate,
                        //Activedate = tp.ActiveDate,
                        //Limited = tp.Limited,
                        Buydate = a.Buydate,
                        Checked = a.Checked,

                        //
                        IdKey = t.Id,
                        Senddate = t.Createdate,
                        Deadline = t.Deadline,
                        Key = db.AspNetUsers.FirstOrDefault(x => x.Id == t.IdKey).UserName,

                        //
                        Successdate = t.Successdate,
                        KTVNote = t.Note,
                        Amount = t.Amount,
                        Cost = t.Cost,
                        Comback = t.Comeback,
                        KTV = db.AspNetUsers.FirstOrDefault(x => x.Id == t.IdKTV).UserName,
                        //
                        //Devices = db.Phutung_Warranti.Where(a => a.IdKey == t.Id).ToList(),
                        Image = db.ImageWarrs.Where(a => a.IDKey == t.Id).Select(a => a.Image).ToList(),

                        KM = t.KM + " " + t.MoveFee,
                        Sign = t.Sign,
                        txtSearch1 = a.CodeWarr + a.Extra + a.Createby + a.Note + t.Comeback + db.AspNetUsers.FirstOrDefault(x => x.Id == t.IdKey).UserName + db.AspNetUsers.FirstOrDefault(x => x.Id == t.IdKTV).UserName,
                        txtSearch2 = a.Phone + a.PhoneWarr + a.Address + a.District + a.Province + a.Name,
                        txtSearch3 = a.Product + a.ProductCate + a.Serial + a.Model + a.ProductCate,
                        CountDay = a.CountDay
                    };
            //var query = db.Warrantis.Where(a => db.Warrantis.OrderByDescending(b => b.Id).Select(y => y.Id).Skip(0).Take(10).Contains(a.Id)).ToList();
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
            if (!string.IsNullOrEmpty(outdate))
            {

                DateTime? _now = DateTime.Now.Date;
                int _outdate = int.Parse(outdate);
                m = m.Where(a => a.Status == 1 && a.CountDay == _outdate);
                ViewBag.outdate = outdate;
            }

            if (!String.IsNullOrEmpty(textString))
            {
                m = m.Where(a => a.txtSearch1.Contains(textString));
                ViewBag.textString = textString;
            }
            if (!String.IsNullOrEmpty(textString1))
            {
                m = m.Where(a => a.txtSearch2.Contains(textString1));
                ViewBag.textString1 = textString1;
            }
            if (!String.IsNullOrEmpty(textString2))
            {
                m = m.Where(a => a.txtSearch3.Contains(textString2));
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

            var cr_url = new CurrentURL()
            {
                UserName = User.Identity.Name,
                URL = Request.Url.AbsoluteUri
            };
            db.CurrentURLs.Add(cr_url);
            db.SaveChanges();
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            int start = (int)(pageNumber - 1) * pageSize;

            ViewBag.pageCurrent = pageNumber;
            int totalPage = m.Count();
            float totalNumsize = (totalPage / (float)pageSize);
            int numSize = (int)Math.Ceiling(totalNumsize);
            if (numSize > 10)
            {
                numSize = 10;
            }
            ViewBag.numSize = numSize;
            return View(m.OrderByDescending(x => x.Id).Skip(start).Take(pageSize).ToList());
        }


        [HttpPost]
        public ActionResult ShowImage(long id, string type)
        {
            var model = db.AddKeys.Find(id);
            string result;
            if (type == "img2")
            {
                result = model.Image2;
            }
            else if (type == "img3")
            {
                result = model.Image3;
            }
            else if (type == "img4")
            {
                result = model.Image4;
            }
            else if (type == "img5")
            {
                result = model.Image5;
            }
            else if (type == "img6")
            {
                result = model.Image6;
            }
            else
            {
                result = model.Image1;
            }
            return PartialView("~/Areas/Admin/Views/Error/_ShowImage.cshtml", result);
        }
        [HttpPost]
        public ActionResult Details(long id)
        {
            var model = db.LogWarantis.Where(a => a.IdWarranti == id);
            return PartialView("~/Areas/Admin/Views/Error/_Details.cshtml", model);
        }
        [HttpPost]
        public ActionResult ChangeStatus(long id)
        {
            var model = db.Warrantis.Find(id);
            return PartialView("~/Areas/Admin/Views/Error/_ChangeStatus.cshtml", model);
        }

        [HttpPost]
        public ActionResult DisplayImage(long IdAddkey)
        {
            var listImage = db.ImageWarrs.Where(a => a.IDKey == IdAddkey).ToList();
            return PartialView("~/Areas/Admin/Views/Error/_DisplayImage.cshtml", listImage);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Change([Bind(Include = "")] Warranti warranti)
        {
            if (ModelState.IsValid)
            {
                db.Entry(warranti).State = System.Data.Entity.EntityState.Modified;
                //db.SaveChanges();
                var log = new LogWaranti()
                {
                    Createdate = DateTime.Now,
                    IdWarranti = warranti.Id,
                    Detail = string.Format(Common.warranti_changestatus, User.Identity.Name, Utils.Robot.getnamestatus(warranti.Status))
                };
                db.LogWarantis.Add(log);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("ChangeStatus", "Error");

        }        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeSs(long id)
        {
            if (ModelState.IsValid)
            {
                var _warr = db.Warrantis.Find(id);
                _warr.Checked = true;
                db.Entry(_warr).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
               
            }
            return View();

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
                Service_price = model.Service_price?? 0,
                Distance = model.Distance,
                DistanceFee = model.DistanceFee ?? 0,
                TravelFee = model.TravelFee ?? 0,
                AccessaryFee = model.AccessaryFee ?? 0,
                Category = warr.Category,
                CharFee = model.CharFee ?? 0,
                PubFee = model.PubFee ?? 0,
                ListPhutung = db.Phutung_Warranti.Where(a => a.IdKey == model.Id).ToList(),
                Checkin = db.ImageWarrs.Where(a => a.IDKey == model.Id && a.Type == 1).ToList(),
                Checkout = db.ImageWarrs.Where(a => a.IDKey == model.Id && a.Type == 2).ToList(),
            };

            TempData["km"] = db.MoveFees.ToList();
            TempData["device"] = db.Devices.ToList();
            //urlss = Request.Url.AbsoluteUri;
            return View(note);
        }

        [HttpPost]
        public ActionResult Addnewdevice()
        {
            var model = from a in db.Devices
                        select new Device()
                        {
                            Name = a.Name + " " + a.Code + " " + a.Price
                        };
            //ViewBag.device = model.ToList();
            return PartialView("~/Areas/Admin/Views/Error/_Addnewdevice.cshtml", null);
        }
        [HttpPost]
        public JsonResult AutoCompleteCountry(string term)
        {
            var result = (from r in db.Devices
                          where r.Name.ToLower().Contains(term.ToLower()) || r.Code.ToLower().Contains(term.ToLower())
                          select new { r.Name , r.Price }).Distinct();
            return Json(result.OrderBy(a => a.Name).Take(20), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AutoCompleteSerFee(string term,string cateer)
        {
            var result = (from r in db.Service_Fee
                          where r.Name.ToLower().Contains(term.ToLower()) && r.Status == cateer
                          select new { r.Name, r.Warfee, r.Charfee, r.Pubfee }).Distinct();
            return Json(result.OrderBy(a => a.Name).Take(20), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Getprice(string cate)
        {
            var model = from a in db.Devices
                        select a;

            var device = model.SingleOrDefault(a => a.Name == cate);

            return Json(device.Price, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteImage(long Id)
        {
            var image = db.ImageWarrs.Find(Id);
            db.ImageWarrs.Remove(image);
            db.SaveChanges();
            return Redirect("/Admin/Error/Success/" + image.IDKey);
        }
        [HttpPost]
        public ActionResult Notes(long id)
        {
            var model = db.AddKeys.Find(id);
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
                CharFee = model.CharFee ?? 0,
                PubFee = model.PubFee ?? 0,
                ListPhutung = db.Phutung_Warranti.Where(a => a.IdKey == model.Id).ToList(),
                Checkin = db.ImageWarrs.Where(a => a.IDKey == model.Id && a.Type == 1).ToList(),
                Checkout = db.ImageWarrs.Where(a => a.IDKey == model.Id && a.Type == 2).ToList(),
            };
            TempData["km"] = db.MoveFees.ToList();
            TempData["device"] = db.Devices.ToList();
            return PartialView("~/Areas/Admin/Views/Error/_Notes.cshtml", note);
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
                if (noteModel.Successdate != null && warr.Createdate !=null)
                {
                    warr.CountDay = (noteModel.Successdate.Value.Date - warr.Createdate.Value.Date).Days;
                }
                addkey.Note = noteModel.Note;
                addkey.Cost = noteModel.Cost;
                addkey.Comeback = noteModel.Comeback;
                addkey.Device = noteModel.Device;
                warr.Category = noteModel.Category;
                addkey.CharFee = noteModel.CharFee;
                addkey.PubFee = noteModel.PubFee;
                addkey.KM = noteModel.KM;
                addkey.MoveFee = noteModel.MoveFee;
                addkey.Price = noteModel.Price;
                addkey.Amount = noteModel.Amount;
                addkey.CateError = noteModel.CateError;
                addkey.Name_price = noteModel.Name_price;
                addkey.Service_price = noteModel.Service_price;
                addkey.AccessaryFee = noteModel.AccessaryFee;
                addkey.TravelFee = noteModel.TravelFee;
                addkey.Distance = noteModel.Distance;
                addkey.DistanceFee = noteModel.DistanceFee;
                //chỉnh sửa deadline
                if (addkey.Deadline != null && addkey.Deadline.Value.Date != noteModel.Deadline.Value.Date)
                {
                    string cr_date = "";
                    string nw_date = "";
                    if (addkey.Deadline != null)
                    {
                        cr_date = addkey.Deadline.Value.ToString("dd/MM/yyyy");
                    }
                    if (noteModel.Deadline != null)
                    {
                        nw_date = noteModel.Deadline.Value.ToString("dd/MM/yyyy");
                    }
                    var log = new LogWaranti()
                    {
                        Createdate = DateTime.Now,
                        IdWarranti = addkey.IdWarranti,
                        Detail = User.Identity.Name + " thay đổi ngày dự kiến : " + cr_date + " -> " + nw_date

                    };
                    db.LogWarantis.Add(log);
                    addkey.Deadline = noteModel.Deadline;
                    warr.Outdate = false;
                    db.Entry(warr).State = System.Data.Entity.EntityState.Modified;
                }
                else if (addkey.Deadline == null)
                {
                    if (noteModel.Deadline != null)
                    {
                        string nw_date = "";
                        nw_date = noteModel.Deadline.Value.ToString("dd/MM/yyyy");
                        var log = new LogWaranti()
                        {
                            Createdate = DateTime.Now,
                            IdWarranti = addkey.IdWarranti,
                            Detail = User.Identity.Name + " bổ sung ngày dự kiến : " + nw_date

                        };
                        db.LogWarantis.Add(log);
                    }
                    addkey.Deadline = noteModel.Deadline;
                    warr.Outdate = false;
                    db.Entry(warr).State = System.Data.Entity.EntityState.Modified;
                }

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
                    // nếu hoàn thành thì k còn quá hạn nữa
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
                noteModel.Checkin = new List<ImageWarr>();
                noteModel.Checkout = new List<ImageWarr>();
                TempData["km"] = db.MoveFees.ToList();
                return View(noteModel);
            }

        }


        [HttpPost]
        public ActionResult AddKey(long id)
        {
            var key = from a in db.AspNetUsers
                      from b in a.AspNetUserRoles
                      where b.RoleId == "Key"
                      select a;
            TempData["key"] = key.ToList();
            var model = new AddKey()
            {
                IdWarranti = id,
            };
            return PartialView("~/Areas/Admin/Views/Error/_AddKey.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "")] AddKey addKey)
        {
            if (ModelState.IsValid)
            {
                var check = db.AddKeys.Where(a => a.IdWarranti == addKey.IdWarranti).SingleOrDefault();
                var crurl = db.CurrentURLs.OrderByDescending(a => a.ID).FirstOrDefault(a => a.UserName == User.Identity.Name);

                if (check != null)
                {
                    return Redirect(crurl.URL);
                }
                addKey.Createdate = DateTime.Now;
                db.AddKeys.Add(addKey);
                //db.SaveChanges();

                var key = db.AspNetUsers.Find(addKey.IdKey);

                var warranti = db.Warrantis.Find(addKey.IdWarranti);
                warranti.Status = 2;//chuyen tram
                db.Entry(warranti).State = System.Data.Entity.EntityState.Modified;

                var sent = new SentNotify();
                sent.Sent(key.UserName, string.Format("Bạn nhận được 1 yêu cầu xử lý cho phiếu có mã {0}", warranti.CodeWarr));

                var log = new LogWaranti()
                {
                    Createdate = DateTime.Now,
                    IdWarranti = warranti.Id,
                    Detail = string.Format(Common.wrranti_addkey, User.Identity.Name, key.UserName, addKey.Deadline.Value.ToString("dd/MM/yyyy"))
                };
                db.LogWarantis.Add(log);
                db.SaveChanges();

                return Redirect(crurl.URL);
            }
            return RedirectToAction("AddKey", "Error");

        }

        [HttpPost]
        public JsonResult ReturnKey(long id)
        {
            var warranti = db.Warrantis.Find(id);
            warranti.Status = 0;
            db.Entry(warranti).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var key = db.AddKeys.Where(a => a.IdWarranti == id).SingleOrDefault();
            db.AddKeys.Remove(key);

            var user = db.AspNetUsers.Find(key.IdKey);

            var log = new LogWaranti()
            {
                Createdate = DateTime.Now,
                IdWarranti = warranti.Id,
                Detail = string.Format(Common.warranti_return, User.Identity.Name, user.UserName)
            };
            db.LogWarantis.Add(log);

            db.SaveChanges();
            return Json(new
            {
                status = 0
            });
        }
        [HttpPost]
        public JsonResult GetIdProduct(string serial)
        {
            if (!string.IsNullOrEmpty(serial))
            {
                serial = serial.Trim();
            }
            var product = db.Products.OrderByDescending(a => a.CreateDate).FirstOrDefault(a => a.Serial == serial);
            var warranti = db.Warrantis.OrderByDescending(a => a.Createdate).FirstOrDefault(a => a.Serial == serial);
            var prodres = new ProdRes();

            if (warranti != null)
            {
                prodres.Id = product.Id;
                prodres.Model = warranti.Model != null ? warranti.Model : "";
                prodres.productcate = warranti.ProductCate != null ? warranti.ProductCate : "";
                prodres.Name = warranti.Name;
                prodres.Code = product.Code;
                prodres.Limited = warranti.Limited;
                prodres.buydate = warranti.Buydate != null ? (warranti.Buydate.GetValueOrDefault().ToString("yyyy-MM-dd")) : ("");
                prodres.activedate = product.ActiveDate != null ? (product.ActiveDate.GetValueOrDefault().ToString("yyyy-MM-dd")) : ("");
                prodres.enddate = product.EndDate != null ? (product.EndDate.GetValueOrDefault().ToString("yyyy-MM-dd")) : ("");

                prodres.phone = warranti.Phone != null ? warranti.Phone : "";
                prodres.cusname = warranti.Name != null ? warranti.Name : "";
                prodres.phone2 = warranti.PhoneWarr != null ? warranti.PhoneWarr : "";
                prodres.province = warranti.Province != null ? warranti.Province : "";
                prodres.district = warranti.District != null ? warranti.District : "";
                prodres.address = warranti.Address != null ? warranti.Address : "";

                prodres.NameAgency = warranti.Agent;
            }
            else if (product != null)
            {
                var model = db.Models.FirstOrDefault(a => a.Code == product.Code);
                var customer = db.Warrantis.OrderByDescending(a => a.Createdate).FirstOrDefault(a => a.Serial == product.Serial);
                prodres.Id = product.Id;
                prodres.Model = model != null ? model.Model1 : "";
                prodres.productcate = model != null ? model.ProductCate : "";
                prodres.Name = product.Name;
                prodres.Code = product.Code;
                prodres.Limited = product.Limited;
                prodres.buydate = product.Buydate != null ? (product.Buydate.GetValueOrDefault().ToString("yyyy-MM-dd")) : ("");
                prodres.activedate = product.ActiveDate != null ? (product.ActiveDate.GetValueOrDefault().ToString("yyyy-MM-dd")) : ("");
                prodres.enddate = product.EndDate != null ? (product.EndDate.GetValueOrDefault().ToString("yyyy-MM-dd")) : ("");

                prodres.phone = customer != null ? customer.Phone : "";
                prodres.cusname = customer != null ? customer.Name : "";
                prodres.phone2 = customer != null ? customer.PhoneWarr : "";
                prodres.province = customer != null ? customer.Province : "";
                prodres.district = customer != null ? customer.District : "";
                prodres.address = customer != null ? customer.Address : "";

                prodres.NameAgency = product.NameAgency;
            }
            else
            {
                return Json("Sản phẩm này không tồn tại.", JsonRequestBehavior.AllowGet);
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(prodres);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCustomer(string phone)
        {
            var customer = db.Customers.Where(a => a.Phone == phone);
            var cusres = new Customer();
            if (customer.Count() > 0)
            {
                foreach (var item in customer)
                {
                    if (item.Name != null)
                    {
                        cusres.Id = item.Id;
                        cusres.Name = item.Name;
                        cusres.Address = item.Address;
                        cusres.County = item.County;
                        cusres.City = item.City;
                        cusres.Phone = item.Phone;
                        break;
                    }
                }
            }


            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(cusres);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public class ProdRes : Product
        {
            public string activedate { get; set; }
            public string enddate { get; set; }
            public string buydate { get; set; }
            public string productcate { get; set; }

            public string phone { get; set; }
            public string phone2 { get; set; }
            public string cusname { get; set; }
            public string province { get; set; }
            public string district { get; set; }
            public string address { get; set; }
        }

        public ActionResult CreateNew(string phone)
        {
            TempData["province"] = db.Provinces.ToList();
            var ag = from a in db.AspNetUsers
                     from b in a.AspNetUserRoles
                     where b.RoleId == "Agency"
                     select a;
            TempData["agent"] = ag.ToList();

            var cate = db.Models.Select(a => a.ProductCate);
            var model = cate.GroupBy(a => a).Where(b => b.Count() > 0).Select(g => g.Key);
            var list = new List<String>();
            list.Add("Bổ sung sau");
            list.AddRange(model.ToList());
            TempData["cate"] = list;
            var warranti = new Warranti();
            warranti.Phone = phone;

            return View(warranti);
        }

        public ActionResult Create2Prod()
        {
            TempData["province"] = db.Provinces.ToList();
            var ag = from a in db.AspNetUsers
                     from b in a.AspNetUserRoles
                     where b.RoleId == "Agency"
                     select a;
            TempData["agent"] = ag.ToList();

            var cate = db.Models.Select(a => a.ProductCate);
            var model = cate.GroupBy(a => a).Where(b => b.Count() > 0).Select(g => g.Key);
            var list = new List<String>();
            list.Add("Bổ sung sau");
            list.AddRange(model.ToList());
            TempData["cate"] = list;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2Prod([Bind(Include = "")] TEKA.Areas.Admin.Models.CreateWithProduct warranti)
        {
            if (ModelState.IsValid)
            {
                if (warranti.Products.Count() > 0 && !string.IsNullOrEmpty(warranti.Cus.Phone) && !string.IsNullOrEmpty(warranti.War.Note))
                {

                    foreach (var item in warranti.Products)
                    {
                        var model = new Warranti();
                        model.Phone = warranti.Cus.Phone;
                        model.PhoneWarr = warranti.Cus.Phone2;
                        model.Name = warranti.Cus.Name;
                        model.Address = warranti.Cus.Address;
                        model.District = warranti.Cus.District;
                        model.Province = warranti.Cus.Province;
                        if (!string.IsNullOrEmpty(warranti.Cus.Phone))
                        {
                            var cus = db.Customers.Where(a => a.Phone == warranti.Cus.Phone && a.Serial == item.Serial);
                            if (cus.Count() == 0)
                            {
                                //tao moi thong tin khach hang
                                var customer = new Customer()
                                {
                                    Phone = warranti.Cus.Phone,
                                    Name = warranti.Cus.Name,
                                    Address = warranti.Cus.Address,
                                    Ward = warranti.Cus.Ward,
                                    County = warranti.Cus.District,
                                    City = warranti.Cus.Province,
                                    Serial = item.Serial,
                                    ActiveDate = item.Activedate,
                                    CodeAgency = item.Agent,
                                    CreateDate = DateTime.Now,
                                };
                                db.Customers.Add(customer);
                            }
                        }

                        model.Status = 0;
                        model.Category = warranti.War.Category;
                        model.Chanel = (warranti.War.Chanel == null) ? "WEB" : warranti.War.Chanel;
                        model.Note = warranti.War.Note;
                        model.Extra = warranti.War.Extra;
                        model.Createby = User.Identity.Name;
                        model.Createdate = DateTime.Now;

                        model.ProductCate = item.ProductCate;
                        model.Product = item.Name;
                        model.Serial = item.Serial;
                        model.Model = item.Model;
                        model.Agent = item.Agent;
                        model.Buydate = item.Buydate;
                        model.Activedate = item.Activedate;
                        model.Limited = item.Limited;
                        model.Outdate = false;
                        //model.Checked = 0;
                        db.Warrantis.Add(model);
                        db.SaveChanges();

                        //update code

                        model.CodeWarr = Utils.Robot.Get_Code_Warranti(model.Id.ToString());//tao code
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                    return RedirectToAction("Index");
                }
            }
            TempData["province"] = db.Provinces.ToList();
            var ag = from a in db.AspNetUsers
                     from b in a.AspNetUserRoles
                     where b.RoleId == "Agency"
                     select a;
            TempData["agent"] = ag.ToList();

            var cate = db.Models.Select(a => a.ProductCate);
            var model2 = cate.GroupBy(a => a).Where(b => b.Count() > 0).Select(g => g.Key);
            var list = new List<String>();
            list.Add("Bổ sung sau");
            list.AddRange(model2.ToList());
            TempData["cate"] = list;
            return View(warranti);
        }

        // POST: Admin/Article/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNew([Bind(Include = "")] TEKA.Models.Warranti warranti)
        {
            try
            {

                if (warranti.Chanel == null)
                {
                    warranti.Chanel = "WEB";
                }
                if (!string.IsNullOrEmpty(warranti.Phone))
                {
                    warranti.Phone = Utils.FormatString.formatUserId(warranti.Phone, 2);
                    var cus = db.Customers.Where(a => a.Phone == warranti.Phone && a.Serial == warranti.Serial);
                    if (cus.Count() == 0)
                    {
                        //tao moi thong tin khach hang
                        var customer = new Customer()
                        {
                            Phone = warranti.Phone,
                            Name = warranti.Name,
                            Address = warranti.Address,
                            Ward = warranti.Ward,
                            County = warranti.District,
                            City = warranti.Province,
                            Serial = warranti.Serial,
                            ActiveDate = warranti.Activedate,
                            CodeAgency = warranti.Agent,
                            CreateDate = DateTime.Now,
                        };
                        db.Customers.Add(customer);
                    }
                }
                warranti.Outdate = false;
                warranti.Createdate = DateTime.Now;
                warranti.Status = 0;
                warranti.Createby = User.Identity.Name;
                //warranti.Checked = 0;
                db.Warrantis.Add(warranti);

                db.SaveChanges();
                warranti.CodeWarr = Utils.Robot.Get_Code_Warranti(warranti.Id.ToString());//tao code
                db.Entry(warranti).State = EntityState.Modified;
                db.SaveChanges();
                //update lai phieu ghi

                var log = new LogWaranti()
                {
                    Createdate = DateTime.Now,
                    IdWarranti = warranti.Id,
                    Detail = string.Format(Common.warranti_create, User.Identity.Name, warranti.Chanel)
                };
                db.LogWarantis.Add(log);
                db.SaveChanges();


                return RedirectToAction("Index");
            }
            catch
            {
                TempData["province"] = db.Provinces.ToList();
                var ag = from a in db.AspNetUsers
                         from b in a.AspNetUserRoles
                         where b.RoleId == "Agency"
                         select a;
                TempData["agent"] = ag.ToList();

                var cate = db.Models.Select(a => a.ProductCate);
                var model = cate.GroupBy(a => a).Where(b => b.Count() > 0).Select(g => g.Key);
                var list = new List<String>();
                list.Add("Bổ sung sau");
                list.AddRange(model.ToList());
                TempData["cate"] = list;
                return View(warranti);
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
            var cate = db.Models.Select(a => a.ProductCate  );
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
        public ActionResult GetModel(string cate)
        {
            var modelitem = db.Models.Where(a => a.ProductCate == cate);
            var listmodel = new List<String>();
            listmodel.Add("Bổ sung sau");
            if (modelitem.Count() > 0)
            {
                listmodel = modelitem.Select(a => a.Model1).ToList();
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(listmodel);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCity(string City)
        {
            District city = new District();
            var id = db.Provinces.Where(s => s.Name == City).SingleOrDefault();
            var provi = db.Districts.Where(x => x.ProvinceId == id.Id).ToList();
            var ress = new List<String>();
            foreach (var i in provi)
            {
                ress.Add(i.Name);
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(ress);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetProductCate(string cate)
        {
            var models = db.Models.Where(a => a.ProductCate == cate);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(models.ToList());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCate()
        {
            var cate = db.Models.Select(a => a.ProductCate);
            var model = cate.GroupBy(a => a).Where(b => b.Count() > 0).Select(g => g.Key);
            var list = new List<String>();
            list.Add("Bổ sung sau");
            list.AddRange(model.ToList());
            return Json(list);
        }

        [HttpPost]
        public ActionResult GetAgent()
        {
            var agent = from a in db.AspNetUsers
                        from b in a.AspNetUserRoles
                        where b.RoleId == "Agency"
                        select a.UserName;
            var list = new List<String>();
            list.AddRange(agent.ToList());
            return Json(list);
        }
        [HttpPost]
        public ActionResult GetKMPrice(string km)
        {
            var listPT = db.MoveFees;
            var model = listPT.Where(a => a.Cate == km).SingleOrDefault();
            return Json(model.Price);
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
        public ActionResult GetDevicePrice(string cate)
        {
            var listPT = db.Devices;
            var model = listPT.Where(a => a.Code.Contains(cate) || a.Name.Contains(cate)).SingleOrDefault();
            return Json(model.Price);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id">id phiếu bảo hành</param>
        /// <returns></returns>
        public ActionResult Delete(long Id)
        {

            try
            {
                //sửa chỗ này xóa phiếu chưa chuyển trạm không được 
                var warranti = db.Warrantis.Find(Id);
                var addkey = db.AddKeys.FirstOrDefault(a => a.IdWarranti == warranti.Id);
                if (addkey != null)
                {
                    db.AddKeys.Remove(addkey);
                    var img = db.ImageWarrs.Where(a => a.IDKey == addkey.Id);
                    var device = db.Phutung_Warranti.Where(a => a.IdKey == addkey.Id);
                    if (img.Count() > 0)
                    {
                        db.ImageWarrs.RemoveRange(img);
                    }
                    if (device.Count() > 0)
                    {
                        db.Phutung_Warranti.RemoveRange(device);
                    }
                }
                if (warranti != null)
                {
                    db.Warrantis.Remove(warranti);
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index");
        }
        public void DatlichExport(int? status, string start_date, string end_date, string textString, string textString1, string textString2, string channel, string cr_date)
        {
            var m = from a in db.Warrantis
                    join b in db.Products on a.IdProduct equals b.Id into tempp
                    from tp in tempp.DefaultIfEmpty()
                        //tram
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
                        Sign = t.Sign,
                        txtSearch1 = a.CodeWarr + a.Extra + a.Createby + a.Note + t.Comeback + t2.UserName + t1.UserName,
                        txtSearch2 = a.Phone + a.PhoneWarr + a.Address + a.District + a.Province + a.Name,
                        txtSearch3 = a.Product + a.ProductCate + a.Serial + a.Model + a.ProductCate

                    };
            if (!String.IsNullOrEmpty(textString))
            {
                m = m.Where(a => a.txtSearch1.Contains(textString));
                //m = m.Where(a => a.CodeWarr.Contains(textString)
                //|| a.Comback.Contains(textString)
                //|| a.Extra.Contains(textString)
                //|| a.Note.Contains(textString)
                //|| a.Key == textString
                //|| a.KTV == textString
                //|| a.Createby == textString
                //);
                ViewBag.textString = textString;
            }
            if (!String.IsNullOrEmpty(textString1))
            {
                //m = m.Where(a => a.Phone.Contains(textString1)
                //|| a.PhoneWarr.Contains(textString1)
                //|| a.Address.Contains(textString1)
                //|| a.District.Contains(textString1)
                //|| a.Province.Contains(textString1)
                //);
                m = m.Where(a => a.txtSearch2.Contains(textString1));
                ViewBag.textString1 = textString1;
            }
            if (!String.IsNullOrEmpty(textString2))
            {
                m = m.Where(a => a.txtSearch3.Contains(textString2));
                //m = m.Where(a => a.Product.Contains(textString2)
                //|| a.Serial.Contains(textString2)
                //|| a.Model.Contains(textString2)
                //|| a.ProductCate.Contains(textString2)
                //);
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
        public ActionResult UploadModel()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadModel(FormCollection formCollection)
        {
            if (Request != null)
            {
                try
                {
                    HttpPostedFileBase file = Request.Files["UploadedFile"];
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));


                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            List<Warranti> listresult = new List<Warranti>();
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;
                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {
                                string code = "";
                                string cate = "";
                                string model = "";
                                code = workSheet.Cells[rowIterator, 1].Value.ToString();
                                try { cate = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch { cate = ""; }
                                try { model = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch { model = ""; }


                                var _warr = new Warranti()
                                {
                                    CodeWarr = code,
                                    ProductCate = cate,
                                    Model = model
                                };
                                var warranti = db.Warrantis.SingleOrDefault(a => a.CodeWarr == code);
                                if (warranti != null)
                                {
                                    if (!string.IsNullOrEmpty(cate))
                                    {
                                        warranti.ProductCate = cate;
                                    }
                                    warranti.Model = model;
                                    db.Entry(warranti).State = EntityState.Modified;

                                    _warr.Status = 0;
                                }
                                else
                                {
                                    _warr.Status = -1;
                                }
                                listresult.Add(_warr);
                            }
                            db.SaveChanges();
                            return View(listresult);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Info(string.Format("[UpdateModel] {0}", ex));
                    ViewBag.text = ex.Message;
                }
            }
            return View();
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
