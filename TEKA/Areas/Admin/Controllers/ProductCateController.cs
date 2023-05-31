using DocumentFormat.OpenXml.Drawing.Charts;
using NLog;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.UI;
using TEKA.Models;

namespace TEKA.Areas.Admin.Controllers
{
    [Authorize(Roles = "Partner,Admin")]
    public class ProductCateController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        public static IEnumerable<Model> data = null;
        public ActionResult Index(string s1, string s2, string s3, string s4, string s5, string s6, string c1, string c2,
            string c3, string c4, string c5, string c6, int? page)
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
            var model = from a in db.Models                        
                        select a;
            
            if (!String.IsNullOrEmpty(s1))
            {
                model = model.Where(a => a.ProductCate.Contains(s1) || a.Code.Contains(s1) || a.Model1.Contains(s1));
            }
            data = model as IEnumerable<Model>;

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(model.OrderBy(a => a.ProductCate).ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Product/Edit/5
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "")] TEKA.Models.Model model)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {

                    db.Models.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch
            {
                return View(model);
            }
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.Model model = db.Models.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] TEKA.Models.Model model)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch
            {
                return View(model);
            }
        }

        // GET: Admin/Product/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.Model model = db.Models.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                // TODO: Add delete logic here
                TEKA.Models.Model model = db.Models.Find(id);
                db.Models.Remove(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private static List<Model> listproductSaveDB;
        public ActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Upload(FormCollection formCollection)//san pham moi, chua duoc kich hoat
        {
            if (Request != null)
            {
                try
                {
                    HttpPostedFileBase file = Request.Files["UploadedFile"];
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        //int count = 0;
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        var products = new List<Model>();
                        int error = 0;
                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;
                            listproductSaveDB = new List<Model>();
                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {
                                ViewBag.count = rowIterator;
                                var product = new Model();
                                product.Model1 = workSheet.Cells[rowIterator, 1].Value.ToString();
                                product.Code = workSheet.Cells[rowIterator, 2].Value.ToString();                                
                                product.ProductCate = workSheet.Cells[rowIterator, 3].Value.ToString();
                                var checktrung = db.Models.Where(a => a.Code == product.Code);
                                if (checktrung.Count() == 0)
                                {
                                    products.Add(product);
                                    listproductSaveDB.Add(product);
                                }
                                else
                                {
                                    error++;
                                    product.Code = "Mã bị trùng";
                                    products.Add(product);
                                }
                            }
                            ViewBag.error = error;
                            return View(products);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.text = ex.Message;
                }
            }
            return View();
        }
        public ActionResult SAVEDB()
        {
            db.Models.AddRange(listproductSaveDB);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Upload");
            }
            return RedirectToAction("Index");
        }

        public void Model_Export()
        {

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "Model";
            Sheet.Cells["B1"].Value = "Code";
            Sheet.Cells["C1"].Value = "Phân loại";

            int row = 2;
            foreach (var item in data)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.Model1;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Code;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.ProductCate;
                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        
    }
}