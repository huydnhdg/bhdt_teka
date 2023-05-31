using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;

namespace TEKA.Areas.Admin.Controllers
{
    public class CheckProductController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        // GET: Admin/CheckProduct
        public ActionResult Index()
        {
            //danh sach san pham cung seri, chua active thi update thong tin theo seri do // neu lai trung thi xoa di 
            string seri = null;
            var model = db.Products.Where(p => p.Serial == seri).Where(p => p.ActiveDate == null);
            return View();
        }

        public ActionResult UploadFileExcel()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadFileExcel(FormCollection formCollection)
        {
            if (Request != null)
            {
                try
                {
                    HttpPostedFileBase file = Request.Files["UploadedFile"];
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        List<int> line = new List<int>();
                        int count = 0;
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        var products = new List<Product>();
                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;
                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {
                                
                                var productExc = new Product();
                                productExc.Name = workSheet.Cells[rowIterator, 1].Value.ToString();
                                productExc.Code = workSheet.Cells[rowIterator, 2].Value.ToString();
                                productExc.Serial = workSheet.Cells[rowIterator, 3].Value.ToString();
                                productExc.Limited = int.Parse(workSheet.Cells[rowIterator, 4].Value.ToString());
                                productExc.Quantity = int.Parse(workSheet.Cells[rowIterator, 5].Value.ToString());
                                productExc.NameAgency = workSheet.Cells[rowIterator, 6].Value.ToString();
                                productExc.CreateBy = workSheet.Cells[rowIterator, 7].Value.ToString();
                                productExc.UserName = workSheet.Cells[rowIterator, 8].Value.ToString();
                                //tim seri nay trong database
                                var check = db.Products.Where(p => p.Serial == productExc.Serial).OrderByDescending(a => a.CreateDate).FirstOrDefault();//
                                if (check != null)
                                {
                                    if (check.ActiveDate == null)
                                    {
                                        check.Name = productExc.Name;
                                        check.Code = productExc.Code;
                                        check.Serial = productExc.Serial;
                                        check.Limited = productExc.Limited;
                                        check.Quantity = productExc.Quantity;
                                        check.NameAgency = productExc.NameAgency;
                                        check.CreateBy = productExc.CreateBy;
                                        check.UserName = productExc.UserName;
                                        db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        count++;
                                    }
                                    else
                                    {
                                        line.Add(rowIterator);
                                    }
                                }

                            }
                        }

                        ViewBag.mess = "" + count;
                        ViewBag.line = line;
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.mess = ex;
                    return View();
                }
            }
            return View();
        }
    }
}