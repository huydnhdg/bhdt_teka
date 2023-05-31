using NLog;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using TEKA.Models;

namespace TEKA.Areas.Admin.Controllers
{
    public class QuayThuongController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        public static List<QuayThuong> ls = new List<QuayThuong>();
        // GET: Admin/QuayThuong
        
        public ActionResult Index(string s1, string s2, string s3, string s4, string s5, string s6, string c1, string c2, string c3, string c4, string c5, string c6, int? page)
        {
            var dsquaythuong = db.QuayThuongs.Where(a => !String.IsNullOrEmpty(a.CustomerPhone)).ToList();
            foreach (var item in dsquaythuong)
            {
                var model = db.Customers.Where(a => a.Phone == item.CustomerPhone).OrderByDescending(a => a.CreateDate).FirstOrDefault();
                if (model != null)
                {
                    if (!String.IsNullOrEmpty(model.Address) && !String.IsNullOrEmpty(model.County) && !String.IsNullOrEmpty(model.City))
                    {
                        item.CustomerAddress = model.Address + " " + model.County + " " + model.City;
                    }
                }
                
                              
            }
            //db.Entry(dsquaythuong).State = System.Data.Entity.EntityState.Modified;
            //db.SaveChanges();
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

            IEnumerable<TEKA.Models.QuayThuong> customers = db.QuayThuongs.OrderByDescending(a => a.CreateDate);

            if (!String.IsNullOrEmpty(s1))
            {
                customers = customers.Where(a => !string.IsNullOrEmpty(a.ProductSerial) && a.ProductSerial.Contains(s1));
            }
            if (!String.IsNullOrEmpty(s2))
            {
                customers = customers.Where(a => !string.IsNullOrEmpty(a.ProductName) && a.ProductName == s2);
            }
            if (!String.IsNullOrEmpty(s3))
            {
                customers = customers.Where(a => !string.IsNullOrEmpty(a.CustomerPhone) && a.CustomerPhone == s3);
            }
            if (!String.IsNullOrEmpty(s4))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s4, MyCultureInfo);
                customers = customers.Where(a => a.CreateDate >= s);
            }
            if (!String.IsNullOrEmpty(s5))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s5, MyCultureInfo);
                customers = customers.Where(a => a.CreateDate <= s);
            }
            if (!String.IsNullOrEmpty(s6))
            {
                customers = customers.Where(a => !string.IsNullOrEmpty(a.Code) && a.Code.Contains(s6));
            }
            ls = customers.ToList();
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(customers.OrderByDescending(a => a.CreateDate).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ExportToExcel()
        {
            try
            {
                var gv = new GridView();
                gv.DataSource = ls;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=danhsach product" + DateTime.Now + ".xls");
                Response.ContentType = "application/vnd.ms-excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Charset = "UTF-8";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                objHtmlTextWriter.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();

            }
            catch (Exception e)
            {

            }
            return View("Index");
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.QuayThuong product = db.QuayThuongs.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] TEKA.Models.QuayThuong product)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(product.CustomerPhone))
                    {
                        product.CreateDate = DateTime.Now;
                        db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        logger.Info(String.Format("gửi mã dự thưởng {0} cho số điện thoại {1} thời gian {2}", product.Code, product.CustomerPhone, product.CreateDate));
                        //gui brandname cho sdt
                        //QuayThuong quayThuong = new QuayThuong()
                        //{
                        //    ProductSerial = product.ProductSerial,
                        //    ProductName = product.ProductName,
                        //    CustomerPhone = product.CustomerPhone,
                        //    CustomerAddress = product.CustomerAddress,
                        //};
                        //dong chuong trinh 31/12/2019
                        //string maduthuong = Utils.SendCodeQuayThuong.SendCode(quayThuong, product.Id);//tra lai ma du thuong
                    }
                    return RedirectToAction("Index");
                }
                return View(product);
            }
            catch
            {
                return View(product);
            }
        }
    }
}