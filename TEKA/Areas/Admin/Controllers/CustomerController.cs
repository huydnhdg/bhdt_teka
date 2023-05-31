using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;
using TEKA.Utils;
using PagedList;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using NLog;

namespace TEKA.Areas.Admin.Controllers
{
    [Authorize(Roles = "Partner,Admin,Mod")]
    public class CustomerController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        public static List<Customer> ls = new List<Customer>();
        // GET: Admin/Customer
        public ActionResult Index(string s1, string s2, string s3, string s4, string s5, string s6, string c1, string c2, string c3, string c4, string c5, string c6, int? page)
        {
            s6 = Utils.ConvertTo.FormatPhoneStartWith84(s6);
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
            //IEnumerable<Customer> m = db.Customers.OrderByDescending(a => a.CreateDate);
            var m = from a in db.Customers.OrderByDescending(a => a.CreateDate)
                    select a;
            if (!String.IsNullOrEmpty(s1))
            {
                m = m.Where(s => s.Name.Contains(s1));
            }
            if (!String.IsNullOrEmpty(s2))
            {
                m = m.Where(s => s.Serial.Contains(s2));
            }
            if (!String.IsNullOrEmpty(s3))
            {
                if (s3 == "0")
                {
                    m = m.Where(x => x.Status == 0 || x.Status == 1 || x.Status == 111);
                }
                else if (s3 == "10" || s3 == "11")
                {
                    m = m.Where(x => x.Status == int.Parse(s3));
                }
            }
            if (!String.IsNullOrEmpty(s4))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s4, MyCultureInfo);
                m = m.Where(a => a.CreateDate >= s);
            }
            if (!String.IsNullOrEmpty(s5))
            {
                CultureInfo MyCultureInfo = new CultureInfo("de-DE");
                DateTime s = DateTime.Parse(s5, MyCultureInfo);
                m = m.Where(a => a.CreateDate <= s);
            }
            if (!String.IsNullOrEmpty(s6))
            {
                m = m.Where(a => a.Phone.Contains(s6));
            }
            IEnumerable<Customer> data = m as IEnumerable<Customer>;
            ls = m.ToList();
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(data.ToPagedList(pageNumber, pageSize));

        }
        //public ActionResult Index([Bind(Include = "id,StartPage,name,serial,status,phone,tungay,denngay")] Khachhang khachhang)
        //{
        //    //int id = 0, int StartPage = 0, string name = "", string serial = "", string status = "", string phone = "", string tungay = "", string denngay = ""
        //    IEnumerable<TEKA.Models.Customer> Customers = db.Customers.OrderByDescending(p => p.ActiveDate);
        //    if (!string.IsNullOrEmpty(khachhang.name))
        //    {
        //        Customers = Customers.Where(n => Utils.TiengViet.Convert(n.Name).Contains(khachhang.name));
        //    }
        //    if (!string.IsNullOrEmpty(khachhang.tungay))
        //    {
        //        CultureInfo MyCultureInfo = new CultureInfo("de-DE");
        //        DateTime s = DateTime.Parse(khachhang.tungay, MyCultureInfo);
        //        Customers = Customers.Where(a => a.ActiveDate >= s);
        //    }
        //    if (!string.IsNullOrEmpty(khachhang.denngay))
        //    {
        //        CultureInfo MyCultureInfo = new CultureInfo("de-DE");
        //        DateTime s = DateTime.Parse(khachhang.denngay, MyCultureInfo);
        //        Customers = Customers.Where(a => a.ActiveDate <= s);
        //    }
        //    if (!string.IsNullOrEmpty(khachhang.serial))
        //    {
        //        Customers = Customers.Where(s => s.Serial.ToUpper() == khachhang.serial.ToUpper());
        //    }
        //    if (!string.IsNullOrEmpty(khachhang.status))
        //    {

        //        int sta = int.Parse(khachhang.status);
        //        if (sta == 1)//khach hang tu kich hoat
        //        {
        //            Customers = Customers.Where(s => s.Status == 0 || s.Status == 1 || s.Status == 111);
        //        }
        //        else if (sta == 0)//da thanh toan
        //        {
        //            Customers = Customers.Where(s => s.Status == 10);
        //        }
        //        else if (sta == 2)//chua thanh toan
        //        {
        //            Customers = Customers.Where(s => s.Status == 11);
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(khachhang.phone))
        //    {
        //        khachhang.phone = Utils.ConvertTo.FormatPhoneStartWith84(khachhang.phone);
        //        Customers = Customers.Where(s => s.Phone == khachhang.phone);
        //    }
        //    int Page = khachhang.id;
        //    int Perpage = 10;
        //    int Row = Page * Perpage;
        //    int count = Customers.Count();
        //    int DisplayPage = 10;
        //    ViewBag.Row = Row;
        //    ViewBag.SectionTitle = @"<a href=""/Home"">Home</a>";
        //    /**
        //         * hien thi 1 luc 5 page
        //         */
        //    int TotalPage = count / Perpage + (count % Perpage == 0 ? 0 : 1);
        //    String PagingHtml = String.Empty;
        //    if (Page == khachhang.StartPage + DisplayPage - 1)
        //    {
        //        //tang giai page vd: 0-4 --> 4-8
        //        khachhang.StartPage += DisplayPage - 1;
        //    }
        //    else if (Page == khachhang.StartPage)
        //    {
        //        if (khachhang.StartPage > DisplayPage - 1)
        //        {
        //            khachhang.StartPage -= DisplayPage - 1;
        //        }
        //        else
        //        {
        //            khachhang.StartPage = 0;
        //        }
        //    }

        //    for (int i = khachhang.StartPage; i < DisplayPage + khachhang.StartPage; i++)
        //    {
        //        if (TotalPage <= i)
        //            break;
        //        String url = Url.Action("Index", new { Id = i, StartPage = khachhang.StartPage });
        //        if (i == Page)
        //        {
        //            PagingHtml += String.Format(@"<li><a href=""{0}""><strong>{1}</strong></a></li>", url, (i + 1));
        //        }
        //        else
        //        {
        //            PagingHtml += String.Format(@"<li><a href=""{0}"">{1}</a></li>", url, (i + 1));
        //        }
        //    }

        //    ViewBag.PagingHtml = PagingHtml;
        //    ViewBag.Page = khachhang.id;
        //    khachhang.Customers = Customers.Skip(Row).Take(10).ToList();
        //    return View(khachhang);
        //}

        // GET: Admin/Customer/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.Customer model = db.Customers.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: Admin/Customer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Customer/Create
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

        // GET: Admin/Customer/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.Customer model = db.Customers.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Admin/Customer/Edit/5
        //[HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Name,Phone,Address,Email,Birthday,CreateDate,CreateBy,Status,CodeProduct,Serial,ActiveDate,NameAgency,CodeAgency,Endate,ErrorNote,Type,Message,City,County,ActiveWho,ErrorDate,StatusError")] TEKA.Models.Customer model)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here
        //        if (ModelState.IsValid)
        //        {

        //            model.Serial = model.Serial.ToUpper();
        //            //dong bo table product
        //            Product product = db.Products.SingleOrDefault(s => s.Serial.ToUpper() == model.Serial);
        //            product.ActiveDate = model.ActiveDate;
        //            if (product.Limited != null && product.ActiveDate != null)
        //            {
        //                product.EndDate = DateTime.Now.AddMonths(product.Limited ?? default(int));//convert int? to int
        //            }
        //            product.PhoneActive = model.Phone;
        //            db.Entry(product).State = System.Data.Entity.EntityState.Modified;

        //            model.CreateDate = DateTime.Now;//thoi gian hien tai
        //            model.CreateBy = User.Identity.Name;//user identity
        //            if (product.Limited != null && product.ActiveDate != null)
        //            {
        //                model.EndDate = DateTime.Now.AddMonths(product.Limited ?? default(int));//convert int? to int
        //            }
        //            db.Entry(model).State = System.Data.Entity.EntityState.Modified;

        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //        return View(model);
        //    }
        //    catch
        //    {
        //        return View(model);
        //    }
        //}
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] TEKA.Models.Customer customer)
        {
            try
            {
                var _customer = db.Customers.Find(customer.Id);
                _customer.Name = customer.Name;
                _customer.Phone = customer.Phone;
                _customer.Address = customer.Address;
                _customer.County = customer.County;
                _customer.City = customer.City;
                _customer.Email = customer.Email;
                _customer.Birthday = customer.Birthday;
                db.Entry(_customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View(customer);
            }
        }
        // GET: Admin/Agency/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEKA.Models.Customer user = db.Customers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Agency/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long? id)
        {
            try
            {
                // TODO: Add delete logic here
                TEKA.Models.Customer user = db.Customers.Find(id);
                db.Customers.Remove(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult ActiveDS(FormCollection formCollection)
        {
            if (Request != null)
            {
                try
                {
                    HttpPostedFileBase file = Request.Files["UploadedFile"];
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        //var products = new List<Product>();
                        //var customers = new List<Customer>();
                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;

                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {
                                ViewBag.item = rowIterator;
                                var product = new Product();
                                var customer = new Customer();
                                string seri = workSheet.Cells[rowIterator, 1].Value.ToString();
                                string name = workSheet.Cells[rowIterator, 2].Value.ToString();
                                string phonesend = workSheet.Cells[rowIterator, 3].Value.ToString();
                                string namekh = null;
                                string add = null;
                                string cou = null;
                                string city = null;
                                string br = null;
                                string email = null;
                                try { namekh = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception e) { };
                                try { add = workSheet.Cells[rowIterator, 5].Value.ToString(); } catch (Exception e) { };
                                try { cou = workSheet.Cells[rowIterator, 6].Value.ToString(); } catch (Exception e) { };
                                try { city = workSheet.Cells[rowIterator, 7].Value.ToString(); } catch (Exception e) { };
                                try
                                {
                                    DateTime birthday;
                                    br = workSheet.Cells[rowIterator, 8].Value.ToString();
                                    birthday = Convert.ToDateTime(br);
                                    customer.Birthday = birthday;
                                }
                                catch (Exception e) { };
                                try { email = workSheet.Cells[rowIterator, 9].Value.ToString(); } catch (Exception e) { };



                                var check = db.Products.Where(s => s.Serial.ToUpper() == seri.ToUpper() && s.Name == name).OrderByDescending(d => d.ExportDate);
                                if (check.Count() > 1)
                                {
                                    product = check.FirstOrDefault();
                                }
                                else
                                {
                                    product = check.SingleOrDefault();
                                }

                                if (product != null)
                                {
                                    if (product.ActiveDate != null)
                                    {
                                        ViewBag.text = String.Format("Sản phẩm {0} đã được kích hoạt trước đó", product.Serial);
                                        return View();
                                    }
                                    //update thong tin bảng san pham

                                    phonesend = Utils.ConvertTo.FormatPhoneStartWith84(phonesend);
                                    product.ActiveDate = DateTime.Now;
                                    if (product.Limited != null)
                                    {
                                        product.EndDate = DateTime.Now.AddMonths(product.Limited ?? default(int));//convert int? to int
                                    }
                                    product.PhoneSend = phonesend;
                                    product.Status = 1;//1: active 0: chua active 2: het han
                                    db.Entry(product).State = System.Data.Entity.EntityState.Modified;//active to product

                                    //them thong tin vao bang khach hang

                                    customer.Name = namekh;
                                    customer.Serial = seri.ToUpper();
                                    customer.Phone = phonesend;
                                    customer.ActiveDate = DateTime.Now;
                                    customer.Address = add;
                                    customer.City = city;
                                    customer.County = cou;
                                    if (email != "#")
                                    {
                                        customer.Email = email;
                                    }
                                    customer.CreateBy = User.Identity.Name;//dai ly kich hoat cho khach hang
                                    customer.CreateDate = DateTime.Now;
                                    if (product.Limited != null)
                                    {
                                        customer.EndDate = DateTime.Now.AddMonths(product.Limited ?? default(int));//convert int? to int
                                    }
                                    customer.CodeProduct = product.Code;
                                    customer.NameAgency = product.NameAgency;
                                    customer.CodeAgency = product.UserName;
                                    customer.ActiveWho = 1;
                                    customer.Type = 0;
                                    customer.Status = 11;
                                    db.Customers.Add(customer);//luu thong tin khach hang

                                    //gui tin nhan den so dien thoai da kich hoat
                                    int sendresult = Utils.SMS.SentSMS(customer.Phone, String.Format("San pham {0} da kich hoat thanh cong tu ngay {1} den ngay {2}. LH 18007227(mien phi) khi can ho tro them. Cam on quy khách.", customer.Serial, customer.ActiveDate, customer.EndDate));
                                    if (sendresult != -1)
                                    {
                                        logger.Info(String.Format("customer-Active-sendMT-{0}", customer.Phone));
                                    }
                                    else
                                    {
                                        logger.Info(String.Format("customer-Active-sendMT-{0}", "error"));
                                    }

                                }
                                else
                                {
                                    ViewBag.text = String.Format("Sản phẩm {0} không tồn tại", seri);
                                    return View();
                                }
                            }
                            db.SaveChanges();//duyet xong ds moi luu db
                            ViewBag.text = "Bạn đã active thành công danh sách.";

                        }

                    }
                }
                catch (Exception ex)
                {
                    ViewBag.text = String.Format("Đã có lỗi xảy ra: {0}", ex);
                    logger.Info(String.Format("Customer-Active-Excel- {0}", ex));
                }
            }
            return View();
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
                Response.AddHeader("content-disposition", "attachment; filename=danhsachkhachhang" + DateTime.Now + ".xls");
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
