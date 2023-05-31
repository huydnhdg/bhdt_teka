using NLog;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TEKA.Controllers
{
    public class SendBrandnameController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
        // GET: SendBrandname
        public ActionResult Index()
        {
            List<BrandName> lsBrandname = new List<BrandName>();
            foreach (var item in lsBrandname)
            {
                int sendresult = Utils.SMS.SentSMS(item.Phone, String.Format("San pham {0} da kich hoat thanh cong tu ngay {1} den ngay {2}. LH 18007227(mien phi) khi can ho tro them. Cam on quy khach.", item.Serial, item.ActiveDate, item.EndDate));
                if (sendresult != -1)
                {
                    logger.Info(String.Format("send brandname excel success"));
                }
                else
                {
                    logger.Info(String.Format("send brandname excel error {0} {1} {2} {3}", item.Phone, item.Serial, item.ActiveDate, item.EndDate));
                }
            }
            return View();
        }

        public ActionResult UploadPhone(FormCollection formCollection)
        {
            HttpPostedFileBase file = Request.Files["UploadedFile"];
            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
            {
                string fileName = file.FileName;
                string fileContentType = file.ContentType;
                byte[] fileBytes = new byte[file.ContentLength];
                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;

                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        var bn = new BrandName();
                        bn.Phone = workSheet.Cells[rowIterator, 1].Value.ToString();
                        bn.Serial = workSheet.Cells[rowIterator, 2].Value.ToString();
                        bn.ActiveDate = workSheet.Cells[rowIterator, 3].Value.ToString();
                        bn.EndDate = workSheet.Cells[rowIterator, 4].Value.ToString();

                        int sendresult = Utils.SMS.SentSMS(bn.Phone, String.Format("San pham {0} da kich hoat thanh cong tu ngay {1} den ngay {2}. LH 18007227(mien phi) khi can ho tro them. Cam on quy khach.", bn.Serial, bn.ActiveDate, bn.EndDate));
                        if (sendresult != -1)
                        {
                            logger.Info(String.Format("send brandname excel success"));
                        }
                        else
                        {
                            logger.Info(String.Format("send brandname excel error {0} {1} {2} {3}", bn.Phone, bn.Serial, bn.ActiveDate, bn.EndDate));
                        }
                    }
                }
            }
            return View();
        }
        public class BrandName
        {
            public string Phone { get; set; }
            public string Serial { get; set; }
            public string ActiveDate { get; set; }
            public string EndDate { get; set; }
        }
    }
}