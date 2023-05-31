using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;
using PagedList;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using NLog;

namespace TEKA.Areas.Admin.Controllers
{
    [Authorize(Roles = "Partner,Admin,Mod")]
    public class ProductController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        public static List<Product> ls = new List<Product>();
        public static IEnumerable<Product> data = null;
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
            var m = db.Products.OrderByDescending(a => a.CreateDate).Take(10000);
            if (!String.IsNullOrEmpty(s1))
            {
                m = db.Products.Where(s => s.Name.Contains(s1) || s.PXK.Contains(s1));
            }
            if (!String.IsNullOrEmpty(s2))
            {
                m = db.Products.Where(s => s.Serial.Equals(s2.ToUpper()));
            }
            if (!String.IsNullOrEmpty(s3))
            {
                m = m.Where(x => x.Status.ToString() == s3);
            }
            if (!String.IsNullOrEmpty(s4))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s4, MyCultureInfo);
                m = m.Where(a => a.ExportDate >= s);
            }
            if (!String.IsNullOrEmpty(s5))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s5, MyCultureInfo);
                m = m.Where(a => a.ExportDate <= s);
            }
            if (!String.IsNullOrEmpty(s6))
            {
                m = m.Where(a => a.UserName == s6);
            }
            data = m as IEnumerable<Product>;
            ls = m.ToList();
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(m.OrderByDescending(a => a.ExportDate).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Code,Serial,PXK,ExportDate,ImportDate,Limited,EndDate,Quantity,PhoneActive,PhoneSend,NameAgency,Status,CreateDate,CreateBy,ThanhToan,UserName")] TEKA.Models.Product product)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    product.Serial = product.Serial.ToUpper();
                    var agencys = db.AspNetUsers.Where(a => a.UserName == product.UserName);
                    AspNetUser agency = new AspNetUser();
                    if (agencys.Count() > 1)
                    {
                        agency = agencys.FirstOrDefault();
                    }
                    else
                    {
                        agency = agencys.SingleOrDefault();
                    }
                    product.CreateDate = DateTime.Now;//thoi gian hien tai
                    product.CreateBy = User.Identity.Name;//user identity
                    product.NameAgency = agency.NameAgency;
                    db.Products.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(product);
            }
            catch
            {
                return View(product);
            }
        }

        // GET: Admin/Product/UnActive/5
        public ActionResult UnActive(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Product/UnActive/5
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult UnActive(long id)
        {
            TEKA.Models.Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            // TODO: Add update logic here
            if (ModelState.IsValid)
            {
                product.ActiveDate = null;
                product.EndDate = null;
                product.Status = null;
                product.PhoneActive = null;
                product.PhoneSend = null;

                db.Entry(product).State = EntityState.Modified;

                TEKA.Models.Customer customer = db.Customers.Where(s => s.Serial == product.Serial).FirstOrDefault();
                db.Customers.Remove(customer);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult Active(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.Product product = db.Products.Find(id);
            Models.ProductCMSActive productCMSActive = new Models.ProductCMSActive()
            {
                Id = product.Id,
                ProdName = product.Name,
                Serial = product.Serial,
                Code = product.Code,
                Limited = product.Limited,

            };
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(productCMSActive);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Active([Bind(Include = "")] Models.ProductCMSActive productCMSActive)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    productCMSActive.PhoneActive = Utils.ConvertTo.FormatPhoneStartWith84(productCMSActive.PhoneActive);
                    logger.Info(String.Format("cms-Active {0} {1} {2} {3} {4} {5} {6}", productCMSActive.Id, productCMSActive.ActiveDate, productCMSActive.PhoneActive, productCMSActive.Cusname, productCMSActive.Address, productCMSActive.Email, productCMSActive.Birthday));
                    //add thong tin sp
                    TEKA.Models.Product product = db.Products.Find(productCMSActive.Id);
                    product.ActiveDate = productCMSActive.ActiveDate;
                    if (productCMSActive.Limited != null)
                    {
                        product.EndDate = product.ActiveDate.Value.AddMonths(productCMSActive.Limited ?? default(int));//convert int? to int
                    }
                    product.PhoneActive = productCMSActive.PhoneActive;
                    product.PhoneSend = productCMSActive.PhoneActive;
                    product.Limited = productCMSActive.Limited;
                    product.Status = 1;
                    db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    //add thong tin khach hang
                    Customer customer = new Customer();
                    customer.Name = productCMSActive.Cusname;
                    customer.Serial = product.Serial.ToUpper();
                    //customer.Message = message;
                    customer.Phone = productCMSActive.PhoneActive;
                    customer.ActiveDate = productCMSActive.ActiveDate;
                    customer.Address = productCMSActive.Address;
                    customer.Birthday = productCMSActive.Birthday;
                    customer.Email = productCMSActive.Email;
                    customer.CreateBy = User.Identity.Name;
                    customer.CreateDate = DateTime.Now;
                    if (product.Limited != null)
                    {
                        customer.EndDate = product.ActiveDate.Value.AddMonths(product.Limited ?? default(int));//convert int? to int
                    }
                    customer.CodeProduct = product.Code;
                    customer.ActiveWho = 3;
                    customer.Status = 1;
                    customer.Type = 0;
                    db.Customers.Add(customer);//luu thong tin khach hang 

                    db.SaveChanges();
                    //gui brandname
                    string ac = customer.ActiveDate?.ToString("dd/MM/yyy");
                    string en = customer.EndDate?.ToString("dd/MM/yyy");
                    int sendresult = Utils.SMS.SentSMS(productCMSActive.PhoneActive, String.Format("San pham {0} da kich hoat thanh cong tu ngay {1} den ngay {2}. LH 18007227(mien phi) khi can ho tro them. Cam on quy khach.", customer.Serial, ac, en));
                    if (sendresult != -1)
                    {
                        logger.Info(String.Format("cms-Active-sendMT-{0}", customer.Phone));
                    }
                    else
                    {
                        logger.Info(String.Format("cms-Active-sendMT-{0}", "error"));
                    }
                    return RedirectToAction("Index");
                }
                return View(productCMSActive);
            }
            catch
            {
                return View(productCMSActive);
            }
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] TEKA.Models.Product product)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    product.Serial = product.Serial.ToUpper();
                    //var agency = db.AspNetUsers.SingleOrDefault(a => a.UserName == product.UserName);
                    //product.NameAgency = agency.NameAgency;
                    product.CreateDate = product.CreateDate;//thoi gian hien tai
                    product.CreateBy = User.Identity.Name;//user identity
                    if (product.Limited != null && product.ActiveDate != null)
                    {
                        DateTime date = product.ActiveDate ?? DateTime.Now;
                        product.EndDate = date.AddMonths(product.Limited ?? default(int));//convert int? to int
                    }
                    db.Entry(product).State = System.Data.Entity.EntityState.Modified;

                    //dong bo table customer
                    if (product.ActiveDate != null)//=1 da kich hoat =2 het han
                    {

                        Customer customer = db.Customers.Single(s => s.Serial == product.Serial);
                        customer.Phone = product.PhoneActive;
                        customer.Serial = product.Serial;
                        customer.ActiveDate = product.ActiveDate;
                        //customer.NameAgency = agency.NameAgency;
                        if (product.Limited != null && product.ActiveDate != null)
                        {
                            DateTime date = product.ActiveDate ?? DateTime.Now;
                            customer.EndDate = date.AddMonths(product.Limited ?? default(int));//convert int? to int
                        }
                        db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(product);
            }
            catch
            {
                return View(product);
            }
        }

        // GET: Admin/Product/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
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
                TEKA.Models.Product product = db.Products.Find(id);
                db.Products.Remove(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private static List<Product> listproductSaveDB;
        public ActionResult UploadProduct()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadProduct(FormCollection formCollection)//san pham moi, chua duoc kich hoat
        {
            if (Request != null)
            {
                try
                {
                    //var tableDS = db.Products.ToList();//remove old data
                    //db.Products.RemoveRange(tableDS);
                    HttpPostedFileBase file = Request.Files["UploadedFile"];
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        //int count = 0;
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        var products = new List<Product>();
                        int error = 0;
                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;
                            listproductSaveDB = new List<Product>();
                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {
                                ViewBag.count = rowIterator;
                                var product = new Product();
                                product.Name = workSheet.Cells[rowIterator, 1].Value.ToString();
                                product.Code = workSheet.Cells[rowIterator, 2].Value.ToString();
                                product.Serial = workSheet.Cells[rowIterator, 3].Value.ToString().ToUpper();
                                try
                                {
                                    product.PXK = workSheet.Cells[rowIterator, 4].Value.ToString().ToUpper();
                                }
                                catch (Exception) { }
                                try
                                {
                                    product.ExportDate = DateTime.ParseExact(workSheet.Cells[rowIterator, 5].Value.ToString(), "dd/MM/yyyy", null);
                                }
                                catch (Exception) { }
                                product.Limited = int.Parse(workSheet.Cells[rowIterator, 6].Value.ToString());
                                product.Quantity = int.Parse(workSheet.Cells[rowIterator, 7].Value.ToString());
                                product.CreateDate = DateTime.Now;
                                product.CreateBy = User.Identity.Name;
                                product.Status = 0;
                                product.ThanhToan = workSheet.Cells[rowIterator, 8].Value.ToString();
                                product.UserName = workSheet.Cells[rowIterator, 9].Value.ToString();
                                product.Model = workSheet.Cells[rowIterator, 10].Value.ToString();
                                var ag = db.AspNetUsers.Where(a => a.UserName == product.UserName).FirstOrDefault();
                                product.NameAgency = ag.NameAgency;
                                var checktrung = from a in db.Products
                                                 where a.Code == product.Code
                                                 && a.Serial == product.Serial
                                                 && a.Name == product.Name
                                                 && a.UserName == product.UserName
                                                 && a.ExportDate == product.ExportDate
                                                 select a;
                                if (checktrung.Count() == 0)
                                {
                                    products.Add(product);
                                    listproductSaveDB.Add(product);
                                }
                                else
                                {
                                    error++;
                                    product.Name = "SP bị trùng";
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
                    logger.Info(String.Format("Product-Upload-Excel-{0}", ex));
                    ViewBag.text = ex.Message;
                }
            }
            return View();
        }
        public ActionResult SAVEDB()
        {
            db.Products.AddRange(listproductSaveDB);
            try
            {
                logger.Info(String.Format("Product-Upload-Excel-have-{0}-save completed", listproductSaveDB.Count()));
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Info(ex);
                return RedirectToAction("UploadProduct");
            }
            return RedirectToAction("Index");
        }

        public ActionResult UploadOldProduct(FormCollection formCollection)//san pham cu, da duoc kich hoat truoc do bang phieu bao hanh bang giay
        {
            if (Request != null)
            {
                try
                {
                    //var tableDS = db.Products.ToList();//remove old data
                    //db.Products.RemoveRange(tableDS);
                    HttpPostedFileBase file = Request.Files["UploadedFile"];
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
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
                                var product = new Product();
                                product.Name = workSheet.Cells[rowIterator, 1].Value.ToString();
                                product.Code = workSheet.Cells[rowIterator, 2].Value.ToString();
                                product.Serial = workSheet.Cells[rowIterator, 3].Value.ToString().ToUpper();
                                product.PXK = workSheet.Cells[rowIterator, 4].Value.ToString().ToUpper();
                                product.ExportDate = Convert.ToDateTime(workSheet.Cells[rowIterator, 5].Value.ToString());
                                product.Limited = int.Parse(workSheet.Cells[rowIterator, 6].Value.ToString());
                                product.Quantity = int.Parse(workSheet.Cells[rowIterator, 7].Value.ToString());
                                //product.NameAgency = workSheet.Cells[rowIterator, 8].Value.ToString();
                                product.CreateDate = DateTime.Now;
                                product.CreateBy = User.Identity.Name;
                                product.Status = 0;
                                product.ThanhToan = workSheet.Cells[rowIterator, 8].Value.ToString();
                                product.UserName = workSheet.Cells[rowIterator, 9].Value.ToString();
                                product.Model = workSheet.Cells[rowIterator, 10].Value.ToString();
                                products.Add(product);
                            }
                            db.Products.AddRange(products);
                            db.SaveChanges();
                            return RedirectToAction("Index");
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
        public void SanPham_Export()
        {

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "Tên sản phẩm";
            Sheet.Cells["B1"].Value = "Code";
            Sheet.Cells["C1"].Value = "Serial";
            Sheet.Cells["D1"].Value = "Model";
            Sheet.Cells["E1"].Value = "Phiếu XK";
            Sheet.Cells["F1"].Value = "Ngày XK";
            Sheet.Cells["G1"].Value = "Ngày KH";
            Sheet.Cells["H1"].Value = "Thời gian BH";
            Sheet.Cells["I1"].Value = "Hạn BH";
            Sheet.Cells["J1"].Value = "SL";
            Sheet.Cells["K1"].Value = "SĐT BH";
            Sheet.Cells["L1"].Value = "SĐT KH";
            Sheet.Cells["M1"].Value = "Ngày tạo";
            Sheet.Cells["N1"].Value = "Tạo bởi";
            Sheet.Cells["O1"].Value = "Đại lý";
            Sheet.Cells["P1"].Value = "Ngày mua";
            Sheet.Cells["Q1"].Value = "Ngày lắp đặt";
            Sheet.Cells["R1"].Value = "Trạng thái";


            int row = 2;
            foreach (var item in data)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.Name;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Code;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Serial;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Model;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.PXK;
                Sheet.Cells[string.Format("F{0}", row)].Value = (item.ExportDate!=null)?item.ExportDate.GetValueOrDefault().ToString("dd/MM/yyyy"):"";
                Sheet.Cells[string.Format("G{0}", row)].Value = (item.ActiveDate != null) ? item.ActiveDate.GetValueOrDefault().ToString("dd/MM/yyyy"):"";
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Limited;
                Sheet.Cells[string.Format("I{0}", row)].Value = (item.EndDate != null) ? item.EndDate.GetValueOrDefault().ToString("dd/MM/yyyy"):"";
                Sheet.Cells[string.Format("J{0}", row)].Value = item.Quantity;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.PhoneActive;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.PhoneSend;
                Sheet.Cells[string.Format("M{0}", row)].Value = (item.CreateDate != null) ? item.CreateDate.GetValueOrDefault().ToString("dd/MM/yyyy"):"";
                Sheet.Cells[string.Format("N{0}", row)].Value = item.CreateBy;
                Sheet.Cells[string.Format("O{0}", row)].Value = item.UserName;
                Sheet.Cells[string.Format("P{0}", row)].Value = (item.Buydate != null) ? item.Buydate.GetValueOrDefault().ToString("dd/MM/yyyy"):"";
                Sheet.Cells[string.Format("Q{0}", row)].Value = (item.Installdate != null) ? item.Installdate.GetValueOrDefault().ToString("dd/MM/yyyy"):"";
                Sheet.Cells[string.Format("R{0}", row)].Value = (item.Status == 0) ? "NULL" : "Kích hoạt";
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
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
    }
}
