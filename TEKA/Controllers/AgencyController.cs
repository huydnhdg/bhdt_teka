using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;

namespace TEKA.Controllers
{
    
    public class AgencyController : Controller
    {
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
        // GET: Agency
        public ActionResult Index([Bind(Include = "id,StartPage,name,serial,status,tungay,denngay,city,county,thanhtoan,phone")] KhachhangDaily data)
        {
            //int id = 0, int StartPage = 0, string name = "", string serial = "", string phone = "", string status = "", string tungay = "", string denngay = "", string thanhtoan = ""
            data.phone = Utils.ConvertTo.FormatPhoneStartWith84(data.phone);

            int Page = data.id;
            int Perpage = 10;
            int Row = Page * Perpage;
            int count = db.Products.Count();
            int DisplayPage = 10;
            ViewBag.Row = Row;
            ViewBag.SectionTitle = @"<a href=""/Home"">Home</a>";

            /**
                 * hien thi 1 luc 5 page
                 */
            int TotalPage = count / Perpage + (count % Perpage == 0 ? 0 : 1);
            String PagingHtml = String.Empty;
            if (Page == data.StartPage + DisplayPage - 1)
            {
                //tang giai page vd: 0-4 --> 4-8
                data.StartPage += DisplayPage - 1;
            }
            else if (Page == data.StartPage)
            {
                if (data.StartPage > DisplayPage - 1)
                {
                    data.StartPage -= DisplayPage - 1;
                }
                else
                {
                    data.StartPage = 0;
                }
            }

            for (int i = data.StartPage; i < DisplayPage + data.StartPage; i++)
            {
                if (TotalPage <= i)
                    break;
                String url = Url.Action("Index", new { Id = i, StartPage = data.StartPage });
                if (i == Page)
                {
                    PagingHtml += String.Format(@"<li><a href=""{0}""><strong>{1}</strong></a></li>", url, (i + 1));
                }
                else
                {
                    PagingHtml += String.Format(@"<li><a href=""{0}"">{1}</a></li>", url, (i + 1));
                }
            }

            IEnumerable<Product> product = db.Products.Where(a => a.UserName == User.Identity.Name).OrderByDescending(f => f.ActiveDate).Skip(Row).Take(10);
            IEnumerable<Customer> customer = db.Customers.OrderBy(o => o.Status);
            if (!string.IsNullOrEmpty(data.thanhtoan))
            {
                int sta = int.Parse(data.thanhtoan);
                if (sta == 1)//khach hang tu kich hoat
                {
                    customer = customer.Where(a => a.Status != 1 || a.Status != 11 || a.Status != 0);
                    product = db.Products.Where(p => p.Status == 1 && p.UserName == User.Identity.Name).OrderByDescending(c => c.CreateDate).Skip(Row).Take(10);
                    
                }
                else if (sta == 0)//da thanh toan
                {
                    customer = customer.Where(a => a.Status == 10);
                    product = db.Products.Where(p => p.Status == 1 && p.UserName == User.Identity.Name).OrderByDescending(c => c.CreateDate).Skip(Row).Take(10);
                    
                }
                else if (sta == 2)//chua thanh toan
                {
                    customer = customer.Where(a => a.Status == 11);
                    product = db.Products.Where(p => p.Status == 1 && p.UserName == User.Identity.Name).OrderByDescending(c => c.CreateDate).Skip(Row).Take(10);
                    
                }
            }
            if (!string.IsNullOrEmpty(data.name))
            {
                customer = db.Customers.Where(n => n.Name.Contains(data.name));
                product = db.Products.Where(s => s.ActiveDate != null && s.UserName == User.Identity.Name).OrderByDescending(c => c.CreateDate).Skip(Row).Take(10);
                
            }
            if (!string.IsNullOrEmpty(data.tungay))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(data.tungay, MyCultureInfo);
                customer = customer.Where(a => a.ActiveDate >= s);
            }
            if (!string.IsNullOrEmpty(data.denngay))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(data.denngay, MyCultureInfo);
                customer = customer.Where(a => a.ActiveDate <= s);
            }
            if (!string.IsNullOrEmpty(data.serial))
            {
                product = db.Products.Where(s => s.Serial == data.serial && s.UserName == User.Identity.Name).OrderByDescending(c => c.CreateDate).Skip(Row).Take(10);
                customer = customer.Where(s => s.Serial == data.serial);
            }
            if (!string.IsNullOrEmpty(data.status))
            {
                product = db.Products.Where(s => s.Status.ToString() == data.status && s.UserName == User.Identity.Name).OrderByDescending(f => f.ActiveDate).Skip(Row).Take(10);
            }
            if (!string.IsNullOrEmpty(data.phone))
            {
                product = db.Products.Where(s => s.PhoneActive == data.phone && s.UserName == User.Identity.Name).OrderByDescending(c => c.CreateDate).Skip(Row).Take(10);
                customer = customer.Where(s => s.Phone == data.phone);
            }

            List<ProAgenModel> model = new List<ProAgenModel>();
            foreach (var p in product)
            {
                if (p.ActiveDate != null)
                {
                    foreach (var c in customer)
                    {
                        if (p.Serial == c.Serial)
                        {
                            ProAgenModel i = new ProAgenModel();
                            i.Serial = p.Serial;
                            i.Phone = c.Phone;
                            i.Name = c.Name;
                            i.Address = c.Address + " " + c.County + " " + c.City;
                            i.ThanhToan = p.ThanhToan;
                            i.ActiveDate = p.ActiveDate;
                            i.EndDate = p.EndDate;
                            i.ExportDate = p.ExportDate;
                            i.ProductCode = p.Code;
                            if (c.Status == 10)
                            {
                                i.ThanhToan = "đã thanh toán";
                            }
                            else if (c.Status == 11)
                            {
                                i.ThanhToan = "chưa thanh toán";
                            }

                            model.Add(i);
                        }
                    }
                }
                else
                {
                    ProAgenModel i = new ProAgenModel();
                    i.Serial = p.Serial;
                    //i.Phone = c.Phone;
                    //i.Name = c.Name;
                    //i.Address = c.Address + "" + c.County + "" + c.City;
                    //i.ThanhToan = p.ThanhToan;
                    //i.ActiveDate = p.ActiveDate.ToString();
                    //i.EndDate = p.EndDate.ToString();
                    i.ProductCode = p.Code;
                    i.ExportDate = p.ExportDate;
                    //if (p.Status == 1)
                    //{
                    //    i.ThanhToan = "đã thanh toán";
                    //}
                    model.Add(i);
                }


            }

            ViewBag.PagingHtml = PagingHtml;
            ViewBag.Page = data.id;
            //var model = Products.Skip(Row).Take(10).ToList();
            data.KhachhangDailys = model.ToList();
            return View(data);
        }

        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.Product p = db.Products.Find(id);
            TEKA.Models.Customer c = db.Customers.Find(id);
            ProAgenModel i = new ProAgenModel();
            i.Serial = p.Serial;
            i.Phone = c.Phone;
            i.Name = c.Name;
            i.Address = c.Address + " " + c.County + " " + c.City;
            i.ThanhToan = p.ThanhToan;
            i.ActiveDate = p.ActiveDate;
            i.EndDate = p.EndDate;
            if (i == null)
            {
                return HttpNotFound();
            }
            return View(i);
        }
    }
}