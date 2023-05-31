using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TEKA.Models;
using static TEKA.Utils.PACallAPI;

namespace TEKA.Areas.Admin.Controllers
{
    public class HistoryCallController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        public ActionResult Index(int? page, int? source, int? destination, string status, string from_date, string to_date)
        {
            //call api lấy thông tin lịch sử cuộc gọi
            var pa = new PA();
            pa.limit = 1000;

            if (!string.IsNullOrEmpty(from_date))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(from_date, MyCultureInfo);
                pa.from_date = s.ToString("yyyy-MM-dd");
            }
            else
            {
                DateTime s = DateTime.Now;
                pa.from_date = s.ToString("yyyy-MM-dd");
            }
            if (!string.IsNullOrEmpty(to_date))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(to_date, MyCultureInfo);
                pa.to_date = s.ToString("yyyy-MM-dd");
            }
            else
            {
                DateTime s = DateTime.Now.AddDays(1);
                pa.to_date = s.ToString("yyyy-MM-dd");
            }
            if (source != null)
            {
                pa.source = source;
                ViewBag.source = source;
            }
            if (destination != null)
            {
                pa.destination = destination;
                ViewBag.destination = destination;
            }
            if (!string.IsNullOrEmpty(status))
            {
                pa.status = status;
                ViewBag.status = status;
            }

            string result = Utils.PACallAPI.GetCDRReport(pa);

            var model = new PA_Response();
            List<DataModel> list = new List<DataModel>();
            if (!string.IsNullOrEmpty(result))
            {
                model = JsonConvert.DeserializeObject<PA_Response>(result);
                if (!string.IsNullOrEmpty(model.data))
                {
                    list = JsonConvert.DeserializeObject<List<DataModel>>(model.data);
                }
            }
            return View(list);
        }

        [HttpPost]
        public ActionResult CallNow(string key, string phone)
        {
            string json = Utils.PACallAPI.CallNow(key,phone);
            var result = JsonConvert.DeserializeObject<result>(json);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public class result
        {
            public string code { get; set; }
            public string message { get; set; }
        }

    }
}