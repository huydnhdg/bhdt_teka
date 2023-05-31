using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TEKA.Models;
using PagedList;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace TEKA.Areas.Admin.Controllers
{
    [OutputCache(Duration = 10, VaryByParam = "none")]
    [Authorize(Roles = "Partner,Admin,Mod")]
    public class DSDailyController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        public static List<ProAgenModel> ls = new List<ProAgenModel>();
        public ActionResult In(string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8, string c1, string c2, string c3, string c4, string c5, string c6, string c7, string c8, int? page)
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
            if (s7 != null)
            {
                page = 1;
            }
            else
            {
                s7 = c7;
            }
            ViewBag.c7 = s7;
            if (s8 != null)
            {
                page = 1;
            }
            else
            {
                s8 = c8;
            }
            ViewBag.c8 = s8;
            var linq = from a in db.Products
                       join b in db.Customers on a.Serial equals b.Serial
                       into temp
                       from t in temp.DefaultIfEmpty()
                       select new ProAgenModel()
                       {
                           Serial = a.Serial,
                           Phone = t.Phone,
                           Name = t.Name,
                           Address = t.Address + " " + t.County + " " + t.City,
                           ActiveDate = t.ActiveDate,
                           EndDate = t.EndDate,
                           ProductCode = a.Code,
                           AgencyName = a.NameAgency,
                           Username = a.UserName,
                           Status = a.Status,
                           TT = t.Status,
                       };
            IEnumerable<ProAgenModel> m = linq.OrderByDescending(x => x.ActiveDate);
            if (!String.IsNullOrEmpty(s1))
            {
                m = m.Where(s => s.Username == s1);
            }
            if (!String.IsNullOrEmpty(s2) || !String.IsNullOrEmpty(s3))
            {
                m = m.Where(s => s.Address != null && s.Address.Contains(s3 + " " + s2));
            }
            if (!String.IsNullOrEmpty(s4))
            {
                m = m.Where(s => s.Serial == s4);
            }
            if (!String.IsNullOrEmpty(s5))
            {
                if (s5 == "0")
                {
                    m = m.Where(x => x.TT == 0 || x.TT == 1 || x.TT == 111);
                }
                else if (s5 == "10" || s5 == "11")
                {
                    m = m.Where(x => x.TT == int.Parse(s5));
                }
            }
            if (!String.IsNullOrEmpty(s6))
            {
                m = m.Where(x => x.Status == int.Parse(s6));
            }
            if (!String.IsNullOrEmpty(s7))
            {
                m = m.Where(x => Convert.ToDateTime(x.ActiveDate) >= Convert.ToDateTime(s7));
            }
            if (!String.IsNullOrEmpty(s8))
            {
                m = m.Where(x => Convert.ToDateTime(x.ActiveDate) <= Convert.ToDateTime(s8));
            }
            ls = m.ToList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var agencys = db.AspNetUsers.Where(d => d.Type == null).ToList();
            ViewBag.agency = agencys;
            var province = db.Provinces.ToList();//danh sach Tinh/ Thanh pho
            ViewBag.province = province;
            return View(ls.ToPagedList(pageNumber, pageSize));
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

        // GET: Admin/ProductAgency/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/ProductAgency/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ProductAgency/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/ProductAgency/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/ProductAgency/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/ProductAgency/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/ProductAgency/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
                Response.AddHeader("content-disposition", "attachment; filename=danhsachdaily " + DateTime.Now + ".xls");
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
