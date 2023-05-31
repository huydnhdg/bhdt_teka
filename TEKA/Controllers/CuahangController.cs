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
using TEKA.Models;

namespace TEKA.Controllers
{
    [RoutePrefix("cua-hang")]
    [Authorize(Roles = "Agency")]
    public class CuahangController : BaseController
    {
        public static List<ProAgenModel> data = new List<ProAgenModel>();
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
        // GET: Cuahang
        [Route]
        public ActionResult Index(string s1, string s2, string s3, string s4, string s5, string s6, string s7, string c1, string c2, string c3, string c4, string c5, string c6, string c7, int? page)
        {
            s1 = Utils.ConvertTo.FormatPhoneStartWith84(s1);
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
            if (s7 != null)
            {
                page = 1;
            }
            else
            {
                s7 = c7;
            }
            ViewBag.c7 = s7;
            var linq = from a in db.Products
                       where a.UserName == User.Identity.Name
                       join b in db.Customers on a.Serial equals b.Serial
                       into temp
                       from t in temp.DefaultIfEmpty()
                       select new ProAgenModel()
                       {
                           ProductName = a.Name,
                           Serial = a.Serial,
                           Phone = t.Phone,
                           Name = t.Name,
                           Address = t.Address + " " + t.County + " " + t.City,
                           ActiveDate = a.ActiveDate,
                           EndDate = t.EndDate,
                           ExportDate = a.ExportDate,
                           ProductCode = a.Code,
                           Status = a.Status,
                           ThanhToan = "",
                           PXK = a.PXK
                       };
            IEnumerable<ProAgenModel> m = linq.OrderByDescending(x => x.ActiveDate) as IEnumerable<ProAgenModel>;
            if (!String.IsNullOrEmpty(s1))
            {
                m = m.Where(s => s.Phone == s1);
            }
            if (!String.IsNullOrEmpty(s2))
            {
                m = m.Where(s => s.Serial.ToUpper() == s2.ToUpper());
            }
            if (!String.IsNullOrEmpty(s3))
            {
                m = m.Where(x => x.ProductCode.Contains(s3) || x.PXK == s3.ToUpper());
            }
            if (!String.IsNullOrEmpty(s4))
            {
                m = m.Where(x => x.Status == Int32.Parse(s4));
            }
            if (!String.IsNullOrEmpty(s5))
            {
                if (s5 == "0")
                {
                    m = m.Where(x => x.Status == 0 || x.Status == 1 || x.Status == 111);
                }
                else if (s5 == "10" || s5 == "11")
                {
                    m = m.Where(x => x.Status == int.Parse(s5));
                }
            }
            if (!String.IsNullOrEmpty(s6))
            {
                DateTime d = DateTime.Parse(s6);
                m = m.Where(x => x.ActiveDate >= d);
            }
            if (!String.IsNullOrEmpty(s7))
            {
                DateTime d = DateTime.Parse(s7);
                m = m.Where(x => x.ActiveDate <= d);
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            data = m.ToList();
            return View(m.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ExportToExcel()
        {
            try
            {
                var gv = new GridView();
                gv.DataSource = data;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=danhsach" + DateTime.Now + ".xls");
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