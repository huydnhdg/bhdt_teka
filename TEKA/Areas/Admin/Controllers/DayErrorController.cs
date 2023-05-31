using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;

namespace TEKA.Areas.Admin.Controllers
{
    public class DayErrorController : Controller
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        // GET: Admin/DayError
        public ActionResult Index()
        {
            List<Teka> agen = new List<Teka>();
            agen = ReadFile();
            for (int i = 0; i < agen.Count(); i++)
            {
                Update(agen[i]);
            }
            try
            {
                ViewBag.mess = "ok";
            }
            catch (Exception e)
            {
                ViewBag.mess = e;
            }
            return View();
        }
        public List<Teka> ReadFile()
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(@"D:\HN\bhdt.teka\activeteka.txt", Encoding.UTF8))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    string[] arrListStr = line.Split('\r');
                    List<Teka> Customer = new List<Teka>();
                    List<Teka> Agency = new List<Teka>();
                    for (int i = 0; i < 383; i++)
                    {
                        Teka teka = new Teka();
                        teka.time = arrListStr[i].Split('-')[0];
                        teka.type = arrListStr[i].Split('-')[1];
                        teka.seri = arrListStr[i].Split('-')[2];
                        teka.phone = arrListStr[i].Split('-')[3];
                        if (teka.type.Contains("AgencyActive"))
                        {
                            Agency.Add(teka);
                        }
                        else
                        {
                            Customer.Add(teka);
                        }
                    }
                    return Agency;
                }

            }
            catch (Exception e)
            {
            }
            return null;
        }
        public void Update(Teka teka)
        {
            var cus = db.Customers.Where(s => s.Serial == teka.seri).FirstOrDefault();
            cus.Status = 11;
            cus.Type = 0;
            db.Entry(cus).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
        public void AgenActive(Teka teka)
        {
            var pro = db.Products.Where(s => s.Serial == teka.seri).FirstOrDefault();
            var cus = db.Customers.Where(s => s.Phone == teka.phone).FirstOrDefault();
            if (pro != null && pro.ActiveDate == null)//du dieu kien kich hoat
            {
                pro.ActiveDate = DateTime.Parse(teka.time);
                pro.EndDate = DateTime.Parse(teka.time).AddMonths(pro.Limited ?? default(int));
                pro.PhoneSend = teka.phone;
                pro.Status = 1;
                db.Entry(pro).State = System.Data.Entity.EntityState.Modified;
                if (cus != null)//khach hang da co thong tin
                {
                    Customer customer = new Customer();
                    customer.Name = cus.Name;
                    customer.Serial = teka.seri.ToUpper();
                    customer.Phone = teka.phone;
                    customer.ActiveDate = DateTime.Parse(teka.time);
                    customer.Address = cus.Address;
                    customer.County = cus.County;
                    customer.City = cus.City;
                    customer.Birthday = cus.Birthday;
                    customer.Email = cus.Email;
                    customer.CreateDate = DateTime.Parse(teka.time);
                    customer.EndDate = DateTime.Parse(teka.time).AddMonths(pro.Limited ?? default(int));
                    customer.CodeProduct = pro.Code;
                    customer.CreateBy = pro.UserName;
                    customer.NameAgency = pro.NameAgency;
                    customer.ActiveWho = 1;
                    customer.Status = 11;
                    customer.Type = 0;
                    db.Customers.Add(customer);
                }
                else
                {
                    Customer customer = new Customer();
                    customer.Serial = teka.seri.ToUpper();
                    customer.Phone = teka.phone;
                    customer.ActiveDate = DateTime.Parse(teka.time);
                    customer.CreateDate = DateTime.Parse(teka.time);
                    customer.EndDate = DateTime.Parse(teka.time).AddMonths(pro.Limited ?? default(int));
                    customer.CodeProduct = pro.Code;
                    customer.CreateBy = pro.UserName;
                    customer.NameAgency = pro.NameAgency;
                    customer.ActiveWho = 1;
                    customer.Status = 11;
                    customer.Type = 0;
                    db.Customers.Add(customer);
                }
                db.SaveChanges();
            }
        }
        public void CusActive(Teka teka)
        {
            var pro = db.Products.Where(s => s.Serial == teka.seri).FirstOrDefault();
            var cus = db.Customers.Where(s => s.Phone == teka.phone).FirstOrDefault();
            if (pro != null && pro.ActiveDate == null)//du dieu kien kich hoat
            {
                pro.ActiveDate = DateTime.Parse(teka.time);
                pro.EndDate = DateTime.Parse(teka.time).AddMonths(pro.Limited ?? default(int));
                pro.PhoneSend = teka.phone;
                pro.Status = 1;
                db.Entry(pro).State = System.Data.Entity.EntityState.Modified;
                if (cus != null)//khach hang da co thong tin
                {
                    Customer customer = new Customer();
                    customer.Name = cus.Name;
                    customer.Serial = teka.seri.ToUpper();
                    customer.Phone = teka.phone;
                    customer.ActiveDate = DateTime.Parse(teka.time);
                    customer.Address = cus.Address;
                    customer.County = cus.County;
                    customer.City = cus.City;
                    customer.Birthday = cus.Birthday;
                    customer.Email = cus.Email;
                    customer.CreateBy = cus.CreateBy;
                    customer.CreateDate = DateTime.Parse(teka.time);
                    customer.EndDate = DateTime.Parse(teka.time).AddMonths(pro.Limited ?? default(int));
                    customer.CodeProduct = pro.Code;
                    customer.ActiveWho = 0;
                    customer.Status = 1;
                    customer.Type = 0;
                    db.Customers.Add(customer);
                }
                else
                {
                    Customer customer = new Customer();
                    customer.Serial = teka.seri.ToUpper();
                    customer.Phone = teka.phone;
                    customer.ActiveDate = DateTime.Parse(teka.time);
                    customer.CreateDate = DateTime.Parse(teka.time);
                    customer.EndDate = DateTime.Parse(teka.time).AddMonths(pro.Limited ?? default(int));
                    customer.CreateBy = teka.phone;
                    customer.ActiveWho = 0;
                    customer.Status = 1;
                    customer.Type = 0;
                    db.Customers.Add(customer);
                }
                db.SaveChanges();

            }
        }
        public class Teka
        {
            public string time { get; set; }
            public string seri { get; set; }
            public string phone { get; set; }
            public string type { get; set; }
        }
    }
}