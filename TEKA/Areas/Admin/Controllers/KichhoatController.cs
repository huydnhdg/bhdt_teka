using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using PagedList;
using TEKA.Areas.Admin.Models;
using TEKA.Models;

namespace TEKA.Areas.Admin.Controllers
{
    public class KichhoatController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        static IEnumerable<ActiveSearch> ls = null;
        // GET: Admin/Kichhoat
        public ActionResult Index(string s1, string s2, string s3, string s4, string s5, string s6, string c1, string c2, string c3, string c4, string c5, string c6, int? page)
        {

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

            //var customers = db.Customers.Where(a => a.Status == 1 || a.Status == 11 || a.Status == 111);
            var customers = from a in db.Customers
                            join b in db.Products on a.Serial equals b.Serial where b.Status ==1
                            where a.Status == 1 || a.Status == 11 || a.Status == 111
                            select new ActiveSearch()
                            {
                                Id = a.Id,
                                Name = a.Name,
                                Phone = a.Phone,
                                Address = a.Address,
                                County = a.County,
                                City = a.City,
                                Birthday = a.Birthday,
                                CodeProduct = a.CodeProduct,
                                Serial = a.Serial,
                                Product = b.Name,
                                Model = b.Model,
                                ActiveDate = a.ActiveDate,
                                EndDate =a.EndDate,
                                ActiveWho = a.ActiveWho,
                                NameAgency = a.NameAgency,
                                Status = a.Status
                            };

            if (!String.IsNullOrEmpty(s1))
            {
                customers = customers.Where(a => a.Status.ToString() == s1);
            }
            if (!String.IsNullOrEmpty(s2))
            {
                customers = customers.Where(a => a.Serial==s2|| a.CodeProduct==s2||a.Model==s2||a.Product.Contains(s2));
            }
            if (!String.IsNullOrEmpty(s3))
            {
                customers = customers.Where(a => a.Phone==s3);
            }
            if (!String.IsNullOrEmpty(s4))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s4, MyCultureInfo);
                customers = customers.Where(a => a.ActiveDate >= s);
            }
            if (!String.IsNullOrEmpty(s5))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s5, MyCultureInfo);
                customers = customers.Where(a => a.ActiveDate <= s);
            }
            if (!String.IsNullOrEmpty(s6))
            {
                customers = customers.Where(a => !string.IsNullOrEmpty(a.NameAgency) && a.NameAgency==s6);
            }
            ls = customers as IEnumerable<ActiveSearch>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(ls.OrderByDescending(a => a.CreateDate).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public JsonResult GetListAgent(string text)
        {
            var cate = (from a in db.AspNetUsers
                        where a.NameAgency.Contains(text)|| a.UserName.Contains(text)
                        select new { a.UserName, a.NameAgency }).Take(10);
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ChangeStatus(long id)
        {
            var result = db.Customers.SingleOrDefault(i => i.Id == id);
            if (result.Status == 11)
            {
                result.Status = 10;
            }
            else if (result.Status == 1 || result.Status == 111)
            {
                result.Status = 0;
            }
            else
            {

            }
            db.SaveChanges();

            return Json(new
            {
                status = result.Status
            });
        }
        public ActionResult ExportToExcel()
        {
            try
            {
                var gv = new GridView();
                gv.DataSource = ls.ToList();
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
    }
}