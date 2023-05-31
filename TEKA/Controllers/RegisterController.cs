using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TEKA.Models;
using TEKA.Utils;

namespace TEKA.Controllers
{
    public class RegisterController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();

        [HttpGet]
        //khach hang active
        public Response Register(string msisdn, string serial, string message)
        {
            msisdn = Utils.ConvertTo.FormatPhoneStartWith84(msisdn);
            logger.Info(String.Format("RegisterController-Register-msisdn-{0}-serial-{1}-message-{2}", msisdn, serial, message)); ;
            //tra ve thong tin active
            Response respone = new Response();
            try
            {
                Product product = new Product();
                var products = db.Products.Where(s => s.Serial.ToUpper() == serial.ToUpper()).OrderByDescending(d => d.ExportDate);//check thong tin san pham
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
                    if (product.Status == -2)
                    {
                        respone.message = "Serial sản phẩm bị khóa kích hoạt.";
                        respone.status = -2;
                    }
                    else if (product.ActiveDate != null)
                    {
                        respone.message = "Serial da duoc kich hoat truoc do.";
                        respone.status = 1;
                        return respone;
                    }
                    else
                    {
                        product.ActiveDate = DateTime.Now;
                        if (product.Limited != null)
                        {
                            product.EndDate = DateTime.Now.AddMonths(product.Limited ?? default(int));//convert int? to int
                        }
                        product.PhoneActive = msisdn;
                        product.PhoneSend = msisdn;
                        product.Status = 1;//1: active 0: chua active 2: het han
                        db.Entry(product).State = System.Data.Entity.EntityState.Modified;//active to product

                        var customer = new Customer();
                        customer.Serial = serial.ToUpper();
                        customer.Message = message;
                        customer.Phone = msisdn;
                        customer.ActiveDate = DateTime.Now;
                        if (product.Limited != null)
                        {
                            customer.EndDate = DateTime.Now.AddMonths(product.Limited ?? default(int));//convert int? to int
                        }
                        customer.CodeProduct = product.Code;
                        customer.NameAgency = product.NameAgency;
                        customer.CreateBy = msisdn;
                        customer.CreateDate = DateTime.Now;
                        customer.Status = 111;
                        customer.ActiveWho = 2;
                        customer.Type = 0;
                        db.Customers.Add(customer);//luu thong tin khach hang

                        db.SaveChanges();
                        //tra them ma du thuong vao trong response
                        //QuayThuong quayThuong = new QuayThuong()
                        //{
                        //    ProductSerial = product.Serial,
                        //    ProductName = product.Name,
                        //    CustomerPhone = customer.Phone,
                        //    CustomerAddress = customer.Address,
                        //};
                        //tra thong tin bao hanh ve response
                        string ac = customer.ActiveDate?.ToString("dd/MM/yyy");
                        string en = customer.EndDate?.ToString("dd/MM/yyy");

                        respone.activedate = ac;
                        respone.enddate = en;
                        respone.limited = product.Limited.ToString();
                        respone.name = customer.Name;
                        respone.productname = product.Name;
                        respone.message = "success";
                        respone.status = 0;
                        //dong chuong trinh 31/12/2019
                        /*respone.maduthuong = Utils.SendCodeQuayThuong.SendCode(quayThuong, "SMS");*///tra ma du thuong cho api
                    }
                }
                else
                {
                    respone.message = "Serial khong ton tai.";
                    respone.status = -1;
                }
                logger.Info(String.Format("RegisterController-Register-msisdn-{0}-serial-{1}-message-{2}-respone-{3}", msisdn, serial, message, respone.message)); ;
            }
            catch (Exception e)
            {
                respone.message = "" + e.Message;
                respone.status = 100;
                logger.Info(e);
            }
            return respone;
        }

        //tra cuu thong tin bao hanh
        [HttpGet]
        public Response Search(string serial)
        {
            //tra ve thong tin tra cuu
            var respone = new Response();
            try
            {
                if (serial != null)
                {
                    Product product = new Product();
                    var products = db.Products.Where(s => s.Serial.ToUpper() == serial.ToUpper()).OrderByDescending(d => d.ExportDate);//check thong tin san pham
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
                        if (product.Status == -2)
                        {
                            respone.message = "Serial sản phẩm bị khóa kích hoạt.";
                            respone.status = -2;
                        }
                        else if (product.ActiveDate == null)
                        {
                            respone.message = "serial nay chua duoc kich hoat";
                            respone.status = 0;
                        }
                        else
                        {
                            var customer = db.Customers.SingleOrDefault(c => c.Serial.ToUpper() == serial.ToUpper());
                            //tra ve thong tin khach hang vao respone
                            string ac = customer.ActiveDate?.ToString("dd/MM/yyy");
                            string en = customer.EndDate?.ToString("dd/MM/yyy");

                            respone.activedate = ac;
                            respone.enddate = en;
                            respone.limited = product.Limited.ToString();
                            respone.name = customer.Name;
                            respone.productname = product.Name;

                            respone.message = "success";
                            respone.status = 1;
                        }

                    }
                    else
                    {
                        respone.message = "Serial khong ton tai.";
                        respone.status = -1;
                    }
                }
                logger.Info(String.Format("RegisterController-Search-serial-{0}-message-{1}", serial, respone.message));

            }
            catch (Exception e)
            {
                respone.message = "" + e.Message;
                respone.status = 100;
                logger.Info(e);
            }
            return respone;
        }

        //khach hang bao loi san pham
        [HttpGet]
        public Response Error(string msisdn)
        {
            msisdn = Utils.ConvertTo.FormatPhoneStartWith84(msisdn);
            var respone = new Response();
            try
            {
                var customer = db.Customers;
                if (customer.Count() > 0)
                {
                    var warranti = new Warranti()
                    {
                        Phone = msisdn,
                        Createdate = DateTime.Now,
                        Chanel = "SMS",
                        Status = 0,
                    };
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
                        Detail = string.Format(Common.warranti_create, User.Identity.Name,warranti.Chanel),
                    };
                    db.LogWarantis.Add(log);
                    db.SaveChanges();

                    respone.message = "So dien thoai da duoc ghi nhan.";
                    respone.status = 1;
                }
                else
                {
                    respone.status = 0;
                    respone.message = "So dien thoai khong ton tai.";
                }
                logger.Info(String.Format("RegisterController-Error-serial-{0}-message-{1}", msisdn, respone.message));
            }
            catch (Exception e)
            {
                respone.message = "Có lỗi xảy ra. Liên hệ nhà cung cấp.";
                respone.status = 100;
                logger.Info(e.Message);
            }
            return respone;
        }
    }
}
