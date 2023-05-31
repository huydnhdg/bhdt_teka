using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;

namespace TEKA.Areas.Admin.Controllers
{
    public class Test11Controller : Controller
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        // GET: Admin/Test
        public ActionResult Index()
        {
            //var code = db.Warrantis.Where(a => a.Outdate == true && a.Status == 9);
            //foreach(var item in code)
            //{
            //    var warr = db.Warrantis.Find(item.Id);
            //    warr.Outdate = false;
            //    db.Entry(warr).State = System.Data.Entity.EntityState.Modified;
            //}
            //db.SaveChanges();
            return View();
        }
        public ActionResult Remove84()
        {
            //var customer = db.Customers.Where(a => a.Id >= 23646);
            //foreach(var item in customer)
            //{
            //    var cus = db.Customers.Find(item.Id);
            //    cus.Phone = Utils.FormatString.formatUserId(cus.Phone, 2);
            //    db.Entry(cus).State = System.Data.Entity.EntityState.Modified;
            //}
            //db.SaveChanges();
            var warranti = db.Warrantis.Where(a => a.Id >= 13454);
            foreach (var item in warranti)
            {
                var cus = db.Warrantis.Find(item.Id);
                cus.Phone = Utils.FormatString.formatUserId(cus.Phone, 2);
                db.Entry(cus).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
            return View();
        }
        class modelwarr
        {
            public string Id { get; set; }
            public string Code { get; set; }
            public string Createdate { get; set; }
        }

        private static List<Model> listproductSaveDB;
        private static List<Warranti> listWarr;
        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            try
            {
                //var tableDS = db.Products.ToList();//remove old data
                //db.Products.RemoveRange(tableDS);
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    listproductSaveDB = new List<Model>();
                    listWarr = new List<Warranti>();
                    //int count = 0;
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
                        int index = 1;
                        try
                        {
                            for (int rowIterator = 39704; rowIterator <= 40000; rowIterator++)
                            {
                                index++;
                                ViewBag.count = rowIterator;
                                //update file phiếu bảo hành cũ
                                string no = "";
                                string createdate = "";
                                string cusname = "";
                                string address = "";
                                string province = "";
                                string phone = "";
                                string phone_war = "";
                                string prodcate = "";
                                string model = "";
                                string serial = "";
                                string code = "";
                                string buydate = "";
                                string agent = "";
                                string note = "";
                                string cate = "";
                                string startdate = "";
                                string station = "";
                                string ktv = "";
                                string complate = "";
                                string status = "";
                                try { no = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception ex) { }
                                try { createdate = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception ex) { }
                                try { cusname = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception ex) { }
                                try { address = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception ex) { }
                                try { province = workSheet.Cells[rowIterator, 6].Value.ToString(); } catch (Exception ex) { }
                                try { phone = workSheet.Cells[rowIterator, 7].Value.ToString(); } catch (Exception ex) { }
                                try { prodcate = workSheet.Cells[rowIterator, 8].Value.ToString(); } catch (Exception ex) { }
                                try { model = workSheet.Cells[rowIterator, 9].Value.ToString(); } catch (Exception ex) { }
                                try { serial = workSheet.Cells[rowIterator, 10].Value.ToString(); } catch (Exception ex) { }
                                try { code = workSheet.Cells[rowIterator, 11].Value.ToString(); } catch (Exception ex) { }
                                try { buydate = workSheet.Cells[rowIterator, 12].Value.ToString(); } catch (Exception ex) { }
                                try { agent = workSheet.Cells[rowIterator, 13].Value.ToString(); } catch (Exception ex) { }
                                try { note = workSheet.Cells[rowIterator, 14].Value.ToString(); } catch (Exception ex) { }
                                try { cate = workSheet.Cells[rowIterator, 15].Value.ToString(); } catch (Exception ex) { }
                                try { startdate = workSheet.Cells[rowIterator, 16].Value.ToString(); } catch (Exception ex) { }
                                try { station = workSheet.Cells[rowIterator, 17].Value.ToString(); } catch (Exception ex) { }
                                try { ktv = workSheet.Cells[rowIterator, 18].Value.ToString(); } catch (Exception ex) { }
                                try { complate = workSheet.Cells[rowIterator, 19].Value.ToString(); } catch (Exception ex) { }
                                try { status = workSheet.Cells[rowIterator, 22].Value.ToString(); } catch (Exception ex) { }

                                DateTime? buy = null;
                                if (!string.IsNullOrEmpty(buydate))
                                {
                                    try
                                    {
                                        try { buy = DateTime.ParseExact(buydate, "dd/MM/yyyy", CultureInfo.InvariantCulture); }
                                        catch (Exception ex) { }
                                    }
                                    catch (Exception ex)
                                    {
                                        buy = null;
                                    }

                                }
                                else
                                {
                                    buy = null;
                                }
                                DateTime? create = null;
                                if (!string.IsNullOrEmpty(createdate))
                                {
                                    try
                                    {
                                        try { create = DateTime.ParseExact(createdate, "dd/MM/yyyy", CultureInfo.InvariantCulture); }
                                        catch (Exception ex) { }
                                    }
                                    catch (Exception ex)
                                    {
                                        create = null;
                                    }

                                }
                                else
                                {
                                    create = null;
                                }
                                string extra = "";
                                if (!string.IsNullOrEmpty(code))
                                {
                                    extra = "CODE: " + code;
                                }
                                if (!string.IsNullOrEmpty(agent))
                                {
                                    extra = extra + " DL: " + agent;
                                }
                                if (!string.IsNullOrEmpty(startdate))
                                {
                                    extra = extra + " YC: " + startdate;
                                }
                                if (!string.IsNullOrEmpty(createdate))
                                {
                                    extra = extra + " TP: " + createdate;
                                }
                                if (!string.IsNullOrEmpty(buydate))
                                {
                                    extra = extra + " MH: " + buydate;
                                }
                                if (!string.IsNullOrEmpty(station))
                                {
                                    extra = extra + "TBH: " + station;
                                }
                                if (!string.IsNullOrEmpty(ktv))
                                {
                                    extra = extra + "KTV: " + ktv;
                                }
                                if (!string.IsNullOrEmpty(note))
                                {
                                    extra = extra + " Note: " + note;
                                }
                                if (!string.IsNullOrEmpty(complate))
                                {
                                    extra = extra + " KQ: " + note;
                                }
                                var warr = new Warranti()
                                {
                                    CodeWarr = no.Replace("\\s\\s+", " ").Trim(),
                                    Phone = getPhone(phone),

                                    Name = cusname.Replace("\\s\\s+", " ").Trim(),
                                    Address = address.Replace("\\s\\s+", " ").Trim(),
                                    Province = province.Replace("\\s\\s+", " ").Trim(),
                                    PhoneWarr = phone_war.Replace("\\s\\s+", " ").Trim(),
                                    ProductCate = prodcate.Replace("\\s\\s+", " ").Trim(),
                                    Serial = serial.Replace("\\s\\s+", " ").Trim(),
                                    Model = model.Replace("\\s\\s+", " ").Trim(),
                                    Extra = extra.Replace("\\s\\s+", " ").Trim(),
                                    Buydate = buy,
                                    Note = note.Replace("\\s\\s+", " ").Trim(),
                                    Status = getStatus(status),
                                    Createdate = create,
                                    Category = getCate(cate)
                                };
                                var check = db.Warrantis.FirstOrDefault(a => a.CodeWarr == warr.CodeWarr);
                                if (check != null)
                                {
                                    continue;
                                }
                                db.Warrantis.Add(warr);
                                db.SaveChanges();
                                string idkey = getIdKey(station);
                                //string idktv = getIdKey(ktv);
                                if (!string.IsNullOrEmpty(idkey))
                                {
                                    string notekey = "";
                                    if (!string.IsNullOrEmpty(ktv))
                                    {
                                        notekey = "KTV: " + ktv;
                                    }
                                    if (!string.IsNullOrEmpty(note))
                                    {
                                        notekey = notekey + " Note: " + note;
                                    }
                                    var addkey = new AddKey()
                                    {
                                        IdWarranti = warr.Id,
                                        IdKey = idkey,
                                        Createdate = warr.Createdate,
                                        Comeback = complate,
                                        //IdKTV = idktv,
                                        Note = notekey
                                    };
                                    db.AddKeys.Add(addkey);
                                    db.SaveChanges();
                                }

                            }
                        }
                        catch (Exception ex) {
                            ViewBag.text = ex.Message;
                            return View();
                        }

                        //db.Models.AddRange(listproductSaveDB);
                        //db.SaveChanges();
                        ViewBag.text = "Bạn đã upload thành công " + index + "sản phẩm";
                        ViewBag.product = products;

                        
                    }
                }
                return View();
            }
            catch(Exception ex)
            {
                ViewBag.text = ex.Message;
                return View();
            }           
            
        }
        string getCate(string txt)
        {
            if (txt == "Tính phí")
            {
                return "Thanh toán";
            }
            else
            {
                return txt;
            }
        }
        int getStatus(string txt)
        {
            if (txt == "đang xử lý")
            {
                return 5;
            }
            else if (txt == "Đợi linh kiện")
            {
                return 7;

            }
            else if (txt == "Hoàn thành")
            {
                return 1;
            }
            else if (txt == "Huỷ bỏ")
            {
                return 9;
            }
            else
            {
                return 1;
            }
        }
        string getPhone(string txt)
        {
            try
            {
                return txt.Substring(0, 10);
            }
            catch (Exception ex)
            {
                return txt;
            }

        }
        string getIdKey(string txt)
        {
            var key = db.AspNetUsers.FirstOrDefault(a => a.UserName == txt);
            if (key != null)
            {
                return key.Id;
            }
            else
            {
                return "";
            }
        }
    }
}