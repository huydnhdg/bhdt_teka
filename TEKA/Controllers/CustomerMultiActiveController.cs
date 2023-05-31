using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TEKA.Models;
using TEKA.Utils;

namespace TEKA.Controllers
{
    [RoutePrefix("kich-hoat-bao-hanh")]
    public class CustomerMultiActiveController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
        [Route]
        // GET: CustomerMultiActive
        public ActionResult Index()
        {
            var province = db.Provinces.OrderBy(a => a.Name).ToList();//danh sach Tinh/ Thanh pho
            ViewBag.province = province;
            string message = "";
            ViewBag.message = message;
            return View();
        }
        [HttpPost]
        public ActionResult MultiActive(string serial, string namesp, string namekh, string city, string county, string address, string phone, string email, string birthday,string buydate,string installdate)
        {
            RessActive ress = new RessActive();
            if (string.IsNullOrEmpty(serial) || string.IsNullOrEmpty(namesp) || string.IsNullOrEmpty(namekh) || string.IsNullOrEmpty(city) || string.IsNullOrEmpty(county) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phone))
            {
                ress.message = "Thông tin có (*) là bắt buộc.";
            }
            else
            {
                Product product = new Product();
                var products = db.Products.Where(s => s.Serial.ToUpper() == serial.ToUpper() && s.Name == namesp).OrderByDescending(d => d.ExportDate);//check thong tin san pham
                if (products.Count() > 1)
                {
                    product = products.FirstOrDefault();
                }
                else
                {
                    product = products.SingleOrDefault();
                }
                if (product != null)
                {
                    if (product.EndDate != null)
                    {
                        ress.message = "Serial sản phẩm đã được kích hoạt trước đó.";
                    }
                    else if (product.Status == -2)
                    {
                        int sendresult = Utils.SMS.SentSMS(phone, "San pham cua ban khong nam trong danh sach kich hoat bao hanh dien tu. Vui long lien he 18007227 (mienphi). Cam on QK");
                        ress.message = "Sản phẩm của bạn không nằm trong danh sách kích hoạt bảo hành điện tử. Vui lòng liên hệ 18007227 (miễn phí). Cảm ơn QK";
                    }
                    else
                    {
                        phone = Utils.ConvertTo.FormatPhoneStartWith84(phone);
                        product.ActiveDate = DateTime.Now;
                        if (product.Limited != null)
                        {
                            product.EndDate = DateTime.Now.AddMonths(product.Limited ?? default(int));//convert int? to int
                        }
                        product.PhoneActive = phone;
                        product.PhoneSend = phone;
                        product.Status = 1;//1: active 0: chua active 2: het han
                        product.Buydate = ConvertTo.TryParse(buydate);
                        product.Installdate = ConvertTo.TryParse(installdate);
                        db.Entry(product).State = System.Data.Entity.EntityState.Modified;//active to product

                        Customer customer = new Customer();
                        customer.Name = namekh;
                        customer.Serial = serial.ToUpper();
                        //customer.Message = message;
                        customer.Phone = phone;
                        customer.ActiveDate = DateTime.Now;
                        customer.Address = address;
                        customer.County = county;
                        customer.City = city;
                        customer.Birthday = ConvertTo.TryParse(birthday);
                        customer.Buydate = ConvertTo.TryParse(buydate);
                        customer.Installdate = ConvertTo.TryParse(installdate);
                        customer.Email = email;
                        customer.CreateBy = namekh;
                        customer.CreateDate = DateTime.Now;
                        if (product.Limited != null)
                        {
                            customer.EndDate = DateTime.Now.AddMonths(product.Limited ?? default(int));//convert int? to int
                        }
                        customer.CodeProduct = product.Code;
                        customer.ActiveWho = 0;
                        customer.Status = 1;
                        customer.Type = 0;
                        db.Customers.Add(customer);//luu thong tin khach hang

                        db.SaveChanges();
                        ress.message = "Kích hoạt sản phẩm thành công.";
                        ress.customer = product;
                        logger.Info(String.Format("CustomerActive-Active-serial-{0}-message-{1}", serial, ress.message));

                        //gui tin nhan den so dien thoai da kich hoat
                        string ac = customer.ActiveDate?.ToString("dd/MM/yyy");
                        string en = customer.EndDate?.ToString("dd/MM/yyy");
                        int sendresult = Utils.SMS.SentSMS(phone, String.Format("San pham {0} da kich hoat thanh cong tu ngay {1} den ngay {2}. LH 18007227(mien phi) khi can ho tro them. Cam on quy khach.", customer.Serial, ac, en));
                        if (sendresult != -1)
                        {
                            logger.Info(String.Format("CustomerActive-Active-sendMT-{0}", customer.Phone));
                        }
                        else
                        {
                            logger.Info(String.Format("CustomerActive-Active-sendMT-{0}", "error"));
                        }
                        //gui ma du thuong
                        //QuayThuong quayThuong = new QuayThuong()
                        //{
                        //    ProductSerial = product.Serial,
                        //    ProductName = product.Name,
                        //    CustomerPhone = customer.Phone,
                        //    CustomerAddress = customer.Address + " " + customer.County + " " + customer.City,
                        //};
                        //dong chuong trinh 31/12/2019
                        //string maduthuong = Utils.SendCodeQuayThuong.SendCode(quayThuong);//tra lai ma du thuong
                    }
                }
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(ress);//convert to json
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public class RessActive
        {
            public string message { get; set; }
            public Product customer { get; set; }
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
        [HttpPost]
        public ActionResult CheckSerial(string serial)
        {
            string mess = "";
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            if (!string.IsNullOrEmpty(serial))
            {
                var product = db.Products.FirstOrDefault(n => n.Serial.ToUpper() == serial.ToUpper());
                if (product == null)
                {
                    mess = "Serial không tồn tại.";
                    return Json(javaScriptSerializer.Serialize(mess), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (product.PhoneActive != null)
                    {
                        mess = "Serial đã được kích hoạt trước đó.";
                        return Json(javaScriptSerializer.Serialize(mess), JsonRequestBehavior.AllowGet);
                    }
                    if (product.UserName != User.Identity.Name)
                    {
                        mess = "Bạn không có quyền kích hoạt sản phẩm này bằng User " + User.Identity.Name;
                        return Json(javaScriptSerializer.Serialize(mess), JsonRequestBehavior.AllowGet);
                    }

                }
            }
            else
            {
                mess = "Serial là bắt buộc.";
            }

            return Json(javaScriptSerializer.Serialize(mess), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetListProduct(string serial)
        {
            var product = db.Products.Where(s => s.Serial.ToUpper() == serial.ToUpper()).ToList();//lay danh sach serial trung nhau chua kich hoat
            List<string> productname = new List<string>();
            foreach (var i in product)
            {
                string s = i.Name;
                productname.Add(s);
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(productname);//convert to json
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CheckPhone(string phone)
        {
            RessActive ress = new RessActive();
            Customer cus = new Customer();
            if (!string.IsNullOrEmpty(phone))
            {
                phone = Utils.ConvertTo.FormatPhoneStartWith84(phone);
                var customer = db.Customers.Where(p => p.Phone == phone).OrderByDescending(p => p.ActiveDate);
                if (customer.Count() > 1)
                {
                    cus = customer.FirstOrDefault();
                }
                else
                {
                    cus = customer.SingleOrDefault();
                }

            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            return Json(javaScriptSerializer.Serialize(cus), JsonRequestBehavior.AllowGet);
        }
    }
}