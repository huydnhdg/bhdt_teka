using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;
using TEKA.Models;

namespace TEKA.Controllers
{
    [RoutePrefix("api")]
    public class ValuesController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        TEKA.Models.TEKAEntities db = new TEKAEntities();

        [Route("insert")]
        [HttpPost]
        // POST api/<controller>
        public XmlDocument InsertSeri(HttpRequestMessage request)
        {

            var doc = new XmlDocument();
            doc.Load(request.Content.ReadAsStreamAsync().Result);
            //deserialize xml
            bxrq requestdata = new bxrq();
            rply response = new rply();
            XmlSerializer serializer = new XmlSerializer(typeof(bxrq));
            using (XmlReader reader = new XmlNodeReader(doc))
            {
                requestdata = (bxrq)serializer.Deserialize(reader);
            }
            try
            {
                var p = db.Products.Where(s => s.Serial.ToUpper() == requestdata.serial.ToUpper() && s.Code == requestdata.code && s.Name == requestdata.name);//kiem tra san pham trong db
                var checkuser = db.AspNetUsers.SingleOrDefault(a => a.UserName == requestdata.username);//kiem tra username
                if (checkuser == null)
                {
                    response.error = "-1";
                    response.error_desc = "error";
                    response.msg = "username khong hop le.";
                }
                else if (p.Count() > 0)
                {
                    response.error = "-2";
                    response.error_desc = "error";
                    response.msg = "Serial bi trung.";

                }
                else
                {
                    Product product = new Product();
                    product.Name = requestdata.name;
                    product.Code = requestdata.code;
                    product.Serial = requestdata.serial.Replace(" ", String.Empty);
                    try
                    {
                        product.ExportDate = DateTime.FromOADate(long.Parse(requestdata.export_date));
                    }
                    catch
                    {
                        product.ExportDate = Convert.ToDateTime(requestdata.export_date);
                    }
                    product.Limited = int.Parse(requestdata.limited);
                    product.Quantity = int.Parse(requestdata.quantity);
                    product.NameAgency = checkuser.NameAgency;
                    product.UserName = requestdata.username;
                    product.CreateDate = DateTime.Now;
                    product.CreateBy = "geodis";

                    db.Products.Add(product);
                    db.SaveChanges();
                    response.error = "0";
                    response.error_desc = "success";
                    response.msg = "Update thanh cong san pham.";
                    logger.Info(String.Format("response.success_desc: {0} {1} {2} {3} {4} {5} {6} {7}", response.error_desc, requestdata.name, requestdata.code, requestdata.serial, requestdata.export_date, requestdata.limited, requestdata.quantity, requestdata.username));
                }


            }
            catch (Exception ex)
            {
                response.error = "-1";
                response.error_desc = "error";
                response.msg = ex.Message;
            }
            logger.Info(String.Format("response.error_desc: {0} {1} {2} {3} {4} {5} {6} {7} {8}", response.error_desc, requestdata.name, requestdata.code, requestdata.serial, requestdata.export_date, requestdata.limited, requestdata.quantity, requestdata.username, response.msg));
            //response xml serialize
            XmlDocument resxml = null;
            XmlSerializer res = new XmlSerializer(response.GetType());
            using (MemoryStream memStm = new MemoryStream())
            {
                res.Serialize(memStm, response);

                memStm.Position = 0;

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;

                using (var xtr = XmlReader.Create(memStm, settings))
                {
                    resxml = new XmlDocument();
                    resxml.Load(xtr);
                }
            }

            return resxml;
        }
        [Route("active")]
        [HttpPost]
        // POST api/<controller>
        public XmlDocument ActiveSeri(HttpRequestMessage request)
        {
            var doc = new XmlDocument();
            doc.Load(request.Content.ReadAsStreamAsync().Result);
            //deserialize xml
            khrq requestdata = new khrq();
            rply response = new rply();
            XmlSerializer serializer = new XmlSerializer(typeof(khrq));
            using (XmlReader reader = new XmlNodeReader(doc))
            {
                requestdata = (khrq)serializer.Deserialize(reader);
            }
            //insert db
            if (requestdata == null)
            {
                response.error = "-1";
                response.error_desc = "error";
                response.msg = "null value xml.";
            }
            try
            {
                Product product = new Product();
                Customer customer = new Customer();
                var check = db.Products.Where(s => s.Serial.ToUpper() == requestdata.serial.ToUpper() && s.Name == requestdata.namesp).OrderByDescending(d => d.ExportDate);
                if (check.Count() > 1)
                {
                    product = check.FirstOrDefault();
                }
                else
                {
                    product = check.SingleOrDefault();
                }
                if (product != null)
                {
                    if (product.ActiveDate != null)
                    {
                        response.error = "-1";
                        response.error_desc = "error";
                        response.msg = "San pham da duoc kich hoat truoc do";
                    }
                    else
                    {
                        //update thong tin bảng san pham

                        requestdata.phone = Utils.ConvertTo.FormatPhoneStartWith84(requestdata.phone);
                        product.ActiveDate = DateTime.Now;
                        if (product.Limited != null)
                        {
                            product.EndDate = DateTime.Now.AddMonths(product.Limited ?? default(int));//convert int? to int
                        }
                        product.PhoneSend = requestdata.phone;
                        product.Status = 1;//1: active 0: chua active 2: het han
                        db.Entry(product).State = System.Data.Entity.EntityState.Modified;//active to product

                        //them thong tin vao bang khach hang

                        customer.Name = requestdata.namekh;
                        customer.Serial = requestdata.serial.ToUpper();
                        customer.Phone = requestdata.phone;
                        customer.ActiveDate = DateTime.Now;
                        customer.Address = requestdata.diachi;
                        customer.City = requestdata.city;
                        customer.County = requestdata.county;
                        customer.Email = requestdata.email;
                        DateTime birthday;
                        if (requestdata.birthday != null)
                        {
                            try
                            {
                                birthday = DateTime.FromOADate(long.Parse(requestdata.birthday));
                                customer.Birthday = birthday;
                            }
                            catch
                            {
                                birthday = Convert.ToDateTime(requestdata.birthday);
                                customer.Birthday = birthday;
                            }
                        }
                        customer.CreateBy = User.Identity.Name;//dai ly kich hoat cho khach hang
                        customer.CreateDate = DateTime.Now;
                        if (product.Limited != null)
                        {
                            customer.EndDate = DateTime.Now.AddMonths(product.Limited ?? default(int));//convert int? to int
                        }
                        customer.CodeProduct = product.Code;

                        customer.NameAgency = product.NameAgency;
                        customer.CodeAgency = product.UserName;
                        customer.ActiveWho = 1;
                        customer.Type = 0;
                        customer.Status = 11;

                        db.Customers.Add(customer);//luu thong tin khach hang
                        db.SaveChanges();

                        //gui tin nhan den so dien thoai da kich hoat
                        int sendresult = Utils.SMS.SentSMS(customer.Phone, String.Format("San pham {0} da kich hoat thanh cong tu ngay {1} den ngay {2}. LH 18007227(mien phi) khi can ho tro them. Cam on quy khách.", customer.Serial, customer.ActiveDate, customer.EndDate));
                        if (sendresult != -1)
                        {
                            logger.Info(String.Format("API-Active-sendMT-{0}", customer.Phone));
                        }
                        else
                        {
                            logger.Info(String.Format("API-Active-sendMT-{0}", "error"));
                        }

                        response.error = "0";
                        response.error_desc = "success";
                        response.msg = "Kich hoat san pham thanh cong.";
                    }
                }
                else
                {
                    response.error = "-1";
                    response.error_desc = "error";
                    response.msg = "San pham khong ton tai trong he thong.";
                }
            }
            catch (Exception ex)
            {
                response.error = "-1";
                response.error_desc = "error";
                response.msg = ex.Message;
            }
            //response xml serialize
            XmlDocument resxml = null;
            XmlSerializer res = new XmlSerializer(response.GetType());
            using (MemoryStream memStm = new MemoryStream())
            {
                res.Serialize(memStm, response);

                memStm.Position = 0;

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;

                using (var xtr = XmlReader.Create(memStm, settings))
                {
                    resxml = new XmlDocument();
                    resxml.Load(xtr);
                }
            }

            return resxml;
        }
        public class rply
        {
            public string error { get; set; }
            public string error_desc { get; set; }
            public string msg { get; set; }
        }
        [XmlRoot("bxrq")]
        public class bxrq
        {
            public string name { get; set; }
            public string code { get; set; }
            public string serial { get; set; }
            public string export_date { get; set; }
            public string limited { get; set; }
            public string quantity { get; set; }
            //public string category { get; set; }
            public string name_agency { get; set; }
            public string username { get; set; }
        }
        [XmlRoot("khrq")]
        public class khrq
        {
            public string namesp { get; set; }
            public string serial { get; set; }
            public string phone { get; set; }
            public string namekh { get; set; }
            public string diachi { get; set; }
            public string county { get; set; }
            public string city { get; set; }
            public string birthday { get; set; }
            public string email { get; set; }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}