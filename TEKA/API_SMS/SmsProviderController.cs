using ImageResizer.Plugins.Basic;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using TEKA.Models;
using static CKFinder.Connector.CKFinderEvent;

namespace TEKA.API_SMS
{
    [RoutePrefix("api/sms")]
    public class SmsProviderController : ApiController
    {
        ////api/sms/receive?command_code=TEKA&user_id=0399570279&request_id=xx&service_id=xx&message=TEKA%20NS232600080
        private static Logger logger = LogManager.GetCurrentClassLogger();
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
        [Route("receive")]
        [HttpGet]
        public HttpResponseMessage Recevice(string Command_Code, string User_ID, string Service_ID, string Request_ID, string Message)
        {
            string MT = saicuphap;
            int status = -1;
            logger.Info(string.Format("[MO] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, Message));
            try
            {
                string pattern = @"^(0[1-9]|84[1-9])(\d{8})$";

                // Xác thực số điện thoại
                bool isValid = Regex.IsMatch(User_ID, pattern);

                if (isValid)
                {
                    User_ID = Utils.ConvertTo.FormatPhoneStartWith84(User_ID);

                }
                else
                {
                    MT = teka_saisdt;
                    status = 0;
                }
                string[] words = Message.ToUpper().Trim().Split(' ');
                if (words.Length == 2)
                {
                    if (words[1] == "BH")//bao hanh
                    {
                        var customer = new Customer();
                        var old_customer = db.Customers.SingleOrDefault(a => a.Phone == User_ID);
                        if (customer == null)
                        {
                            var new_customer = new Customer()
                            {
                                Phone = User_ID,
                                CreateDate = DateTime.Now,

                            };
                            db.Customers.Add(new_customer);
                            db.SaveChanges();

                            customer = new_customer;
                        }
                        else
                        {
                            customer = old_customer;
                        }
                        CreateWarranti(customer);
                        MT = teka_bhtc;
                        status = 1;

                    }
                    else
                    {
                        string serial = words[1];
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
                        if (product != null )
                        {
                            if (product.Status == -2)
                            {
                                MT = teka_tcspbikhoa;
                                status = -2;
                            }
                            else if (product.ActiveDate != null)
                            {
                                MT = string.Format(teka_dakh, product.Serial, product.ActiveDate.Value.ToString("dd/MM/yyyy"), product.EndDate.Value.ToString("dd/MM/yyyy"));
                                status = 1;
                            } 
                              else
                            {



                                product.ActiveDate = DateTime.Now;
                                if (product.Limited != null)
                                {
                                    product.EndDate = DateTime.Now.AddMonths(product.Limited ?? default(int));//convert int? to int
                                }
                                product.PhoneActive = User_ID;
                                product.PhoneSend = User_ID;
                                product.Status = 1;//1: active 0: chua active 2: het han
                                db.Entry(product).State = System.Data.Entity.EntityState.Modified;//active to product

                                var customer = new Customer();
                                customer.Serial = serial.ToUpper();
                                customer.Message = Message;
                                customer.Phone = User_ID;
                                customer.ActiveDate = DateTime.Now;
                                if (product.Limited != null)
                                {
                                    customer.EndDate = DateTime.Now.AddMonths(product.Limited ?? default(int));//convert int? to int
                                }
                                customer.CodeProduct = product.Code;
                                customer.NameAgency = product.NameAgency;
                                customer.CreateBy = User_ID;
                                customer.CreateDate = DateTime.Now;
                                customer.Status = 111;
                                customer.ActiveWho = 2;
                                customer.Type = 0;
                                db.Customers.Add(customer);//luu thong tin khach hang

                                db.SaveChanges();
                                MT = string.Format(teka_khtc, product.Serial, product.ActiveDate.Value.ToString("dd/MM/yyyy"), product.EndDate.Value.ToString("dd/MM/yyyy"));
                                status = 0;
                            }    

                        }   
                        else
                        {
                            MT = teka_khsaima;
                            status = -1;
                        }
                    }
                }
                else if (words.Length == 3)
                {
                    if (words[1] == "TRACUU")//tra cuu bao hanh
                    {
                        string code = words[2];
                        var product = db.Products.SingleOrDefault(a => a.Serial == code);
                        var today = DateTime.Now;
                        today = today.AddDays(1);
                        if (product == null)
                        {
                            //khong ton tai serial
                            MT = teka_tcsaima;
                            status = -1;
                        }
                        if (product.ActiveDate == null)
                        {
                            //chua kich hoat
                            MT = string.Format(teka_tcchuakichhoat,product.Serial);
                            status = 0;
                        }
                        else if (product.EndDate < today)
                        {
                            //qua han bao hanh
                            MT = string.Format(teka_tchethanbh, product.Serial, product.EndDate.Value.ToString("dd/MM/yyyy"));
                        }
                        else if (product.ActiveDate != null)
                        {
                            MT = string.Format(teka_tcdakichhoat, product.Serial, product.ActiveDate.Value.ToString("dd/MM/yyyy"), product.EndDate.Value.ToString("dd/MM/yyyy"));
                            status = 1;
                        }
                        else if (product.Status == -2)
                        {
                            MT = teka_tcspbikhoa;
                            status = -2;
                        }
                    }
                    else//khong dung cu phap
                    {
                        MT = saicuphap;
                        status = 100;
                    }
                  
                }
            }
            catch (Exception ex)
            {
               
                MT = saicuphap;
                status = 100;
                logger.Error(ex.Message);
            }
            var log_mo = new LOG_MO()
            {
                User_Id = User_ID,
                Service_Id = Service_ID,
                Request_Id = Request_ID,
                Message = Message,
                Createdate = DateTime.Now,
                Status = status,
                Response = MT
            };
            db.LOG_MO.Add(log_mo);
            db.SaveChanges();
            //var response = new HttpResponseMessage();
            //response.Content = new StringContent("0|" + MT);
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            logger.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, MT));
            var response = new HttpResponseMessage();
            response.Content = new StringContent("0|" + MT);
            return response;
        }
        void CreateWarranti(Customer customer)
        {
            var warranti = new Warranti()
            {
                Status = 0,
                Createdate = DateTime.Now,
                Chanel = "SMS",
                Phone = customer.Phone,


            };
            db.Warrantis.Add(warranti);
            db.SaveChanges();
            //tao ma phieu bao hanh 
            warranti.CodeWarr = Utils.Robot.Get_Code_Warranti(warranti.Id.ToString());
            db.Entry(warranti).State = EntityState.Modified;
            db.SaveChanges();
        }

        string saicuphap = "Tin nhan cua Quy khach sai cu phap. Vui long soan: TEKA(dau cach)(so serial) gui 8077. LH 18007227(mien phi) hoac truy cap https://onlinewarranty.tekavietnam.com";

        string teka_tcsaima = "Serial san pham cua ban khong ton tai. Vui long kiem tra lai so serial va lien he 18007227 hoac truy cap https://onlinewarranty.tekavietnam.com de duoc ho tro";
        string teka_tcdakichhoat = "San pham {0} da duoc kich hoat tu ngay {1} den ngay {2}. Vui long lien he 18007227(mien phi) de ho tro. Cam on quy khach.";
        string teka_tcchuakichhoat = "San pham {0} chua duoc kich hoat, moi thac mac lien he 18007227(mien phi). Cam on QK.";
        string teka_tchethanbh = "San pham {0} da het han bao hanh ({1}), moi thac mac lien he 18007227(mien phi). Cam on QK.";
        string teka_tcspbikhoa = "San pham cua ban khong nam trong danh sach kich hoat bao hanh dien tu. Vui long lien he 18007227 (mienphi). Cam on QK";

        string teka_bhtc = "Yeu cau bao hanh cua ban da duoc ghi nhan, chung toi se lien he lai trong vong 24h. Moi thac mac lien he 18007227 (mienphi). Cam on QK";
        string teka_saisdt = "So dien thoai Yeu cau bao hanh cua ban khong ton tai. Moi thac mac lien he 18007227 (mienphi). Cam on QK";

        string teka_khsaima = "Serial san pham cua ban khong ton tai. Vui long kiem tra lai so serial va lien he 18007227(mien phi) hoac truy cap https://onlinewarranty.tekavietnam.com";
        string teka_chuakh = "Serial san pham cua ban chua kich hoat duoc. Vui long lien he 18007227(mien phi) hoac truy cap http://onlinewarranty.tekavietnam.com de duoc ho tro kich hoat.";
        string teka_dakh = "San pham {0} da duoc kich hoat tu ngay {1} den ngay {2}. Vui long lien he 18007227(mien phi) de ho tro. Cam on quy khach.";
        string teka_khtc = "San pham {0} da kich hoat thanh cong tu ngay {1} den ngay {2}. LH 18007227(mien phi) khi can ho tro them. Cam on quy khach.";
       


    }
}
