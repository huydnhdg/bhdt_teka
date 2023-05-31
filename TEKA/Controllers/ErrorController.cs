using Microsoft.AspNet.Identity;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TEKA.Models;
using TEKA.Utils;

namespace TEKA.Controllers
{
    [RoutePrefix("bao-loi-san-pham")]
    public class ErrorController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();

        [Route]
        public ActionResult Index()
        {
            TempData["province"] = db.Provinces.ToList();
            var cate = db.Models.Select(a => a.ProductCate);
            var model = cate.GroupBy(a => a).Where(b => b.Count() > 0).Select(g => g.Key);
            var list = new List<String>();
            list.Add("Bổ sung sau");
            list.AddRange(model.ToList());
            TempData["cate"] = list;
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "")] TEKA.Models.Warranti warranti)
        {
            try
            {
                try
                {
                    DateTime today = DateTime.Now;
                    var old = db.Warrantis.Where(a => a.Phone == warranti.Phone && a.Serial == warranti.Serial && a.Model == warranti.Model).OrderByDescending(a => a.Createdate).FirstOrDefault();
                    if (old != null && old.Createdate.Value.Date == today.Date)
                    {
                        TempData["province"] = db.Provinces.ToList();
                        var cate2 = db.Models.Select(a => a.ProductCate);
                        var model2 = cate2.GroupBy(a => a).Where(b => b.Count() > 0).Select(g => g.Key);
                        var list2 = new List<String>();
                        list2.Add("Bổ sung sau");
                        list2.AddRange(model2.ToList());
                        TempData["cate"] = list2;
                        return Json("Không gửi yêu cầu trùng nhau.");
                    }
                }
                catch
                {

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
                            CodeAgency = warranti.Agent
                        };
                        db.Customers.Add(customer);
                    }
                }

                warranti.Createdate = DateTime.Now;
                warranti.Status = 0;
                warranti.Createby = User.Identity.Name;
                warranti.Outdate = false;
                db.Warrantis.Add(warranti);
                db.SaveChanges();

                warranti.CodeWarr = Utils.Robot.Get_Code_Warranti(warranti.Id.ToString());//tao code
                db.Entry(warranti).State = EntityState.Modified;
                db.SaveChanges();
                //update lai phieu ghi

                //neu la tai khoan tram tao thi chuyen luon cho tram nay de co the chuyen ktv
                if (User.IsInRole("Key"))
                {
                    string id = User.Identity.GetUserId();
                    var addkey = new AddKey();
                    addkey.IdWarranti = warranti.Id;
                    addkey.IdKey = id;
                    addkey.Createdate = DateTime.Now;
                    addkey.Deadline = DateTime.Now.AddDays(5);
                    db.AddKeys.Add(addkey);
                    db.SaveChanges();
                    //doi trang thai thanh chuyen tram
                    warranti.Status = 2;
                    db.Entry(warranti).State = EntityState.Modified;
                    db.SaveChanges();
                }


                var log = new LogWaranti()
                {
                    Createdate = DateTime.Now,
                    IdWarranti = warranti.Id,
                    Detail = string.Format(Common.warranti_create,User.Identity.Name, warranti.Chanel)
                };
                db.LogWarantis.Add(log);
                db.SaveChanges();

                TempData["province"] = db.Provinces.ToList();
                var cate = db.Models.Select(a => a.ProductCate);
                var model = cate.GroupBy(a => a).Where(b => b.Count() > 0).Select(g => g.Key);
                var list = new List<String>();
                list.Add("Bổ sung sau");
                list.AddRange(model.ToList());
                TempData["cate"] = list;
                return Json("TEKA đã tiếp nhận yêu cầu bảo hành thành công.");
            }
            catch
            {
                TempData["province"] = db.Provinces.ToList();
                var cate = db.Models.Select(a => a.ProductCate);
                var model = cate.GroupBy(a => a).Where(b => b.Count() > 0).Select(g => g.Key);
                var list = new List<String>();
                list.Add("Bổ sung sau");
                list.AddRange(model.ToList());
                TempData["cate"] = list;
                return Json("Đã có lỗi xảy ra. Liên hệ tổng đài để được giải quyết.");
            }
        }
        [HttpPost]
        public JsonResult GetAgent(string text)
        {
            var cate = (from a in db.AspNetUsers
                        where a.AspNetUserRoles.FirstOrDefault().RoleId == "Agency"
                        where a.UserName.Contains(text)
                        select new { a.UserName });
            return Json(cate, JsonRequestBehavior.AllowGet);
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
            var models = db.Models.Where(a => a.ProductCate.Contains(cate));
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(models.ToList());
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetIdProduct(string serial)
        {
            var product = db.Products.Where(a => a.Serial == serial).SingleOrDefault();
            if (product != null && product.ActiveDate != null)
            {
                var prodres = new ProdRes();
                prodres.Id = product.Id;
                prodres.Name = product.Name;
                prodres.Code = product.Code;
                prodres.Model = product.Model;
                prodres.Limited = product.Limited;
                prodres.Buydate = product.Buydate;
                prodres.activedate = product.ActiveDate.GetValueOrDefault().ToString("dd/MM/yyyy");
                prodres.enddate = product.EndDate.GetValueOrDefault().ToString("dd/MM/yyyy");


                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string result = javaScriptSerializer.Serialize(prodres);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (product == null)
            {
                return Json("Sản phẩm này chưa kích hoạt.", JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);

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
        }
    }
}
