using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using NLog;
using System;
using System.Web;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

using System.IO;
using TEKA.Models;
using TEKA;
using TEKA.API.Model;
using TEKA.Areas.Admin;
using TEKA.Utils;
using System.Web.WebPages.Html;
using TEKA.FCM;
using ImageResizer;

namespace TEKA.API
{
    [RoutePrefix("api/app")]
    public class DefaultController : ApiController
    {
        TEKAEntities db = new TEKAEntities();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Route("getreportactive")]
        [HttpGet]
        public HttpResponseMessage GetReportActive(string UserId = "", int year = 0)
        {
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }
            var us = db.AspNetUsers.Find(UserId);
            var model = from a in db.Products
                        where a.UserName == us.UserName && a.Status == 1
                        select a;
            int countProd = model.Count();
            int countAmount = 0;

            //check theo nam
            if (year > 2000)
            {
                model = model.Where(a => a.ActiveDate.Value.Year == year);
            }
            List<int> month = new List<int>();
            for (int i = 1; i <= 12; i++)
            {
                int item = model.Where(a => a.ActiveDate.Value.Month == i).Count();
                month.Add(item);
            }
            var report = new ReportImport()
            {
                Amount = countAmount,
                CountProduct = countProd,
                Month = month
            };
            var result = new ReportImportRes()
            {
                Status = 1,
                Message = "OK",
                Data = new List<ReportImport>() { report }
            };
            return ResponseMessage(result);

        }
        [Route("province")]
        [HttpGet]
        public HttpResponseMessage GetProvince(string textSearch = "")
        {
            var model = from a in db.Provinces
                        select new Adr()
                        {
                            Id = a.Id,
                            Name = a.Name
                        };
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.Name.Contains(textSearch));
            }
            var result = new ProvinceRes()
            {
                Status = 1,
                Message = "OK",
                Data = model.ToList()
            };
            return ResponseMessage(result);
        }
        [Route("district")]
        [HttpGet]
        public HttpResponseMessage GetDistrict(long Id, string textSearch = "")
        {
            var model = from a in db.Districts
                        where a.ProvinceId == Id
                        select new Adr()
                        {
                            Id = a.Id,
                            Name = a.Name
                        };
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.Name.Contains(textSearch));
            }
            var result = new DistrictRes()
            {
                Status = 1,
                Message = "OK",
                Data = model.ToList()
            };
            return ResponseMessage(result);
        }
        [Route("ward")]
        [HttpGet]
        public HttpResponseMessage GetWard(long Id, string textSearch = "")
        {
            var model = from a in db.Wards
                        where a.DistrictID == Id
                        select new Adr()
                        {
                            Id = a.Id,
                            Name = a.Name
                        };
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.Name.Contains(textSearch));
            }
            var result = new WardRes()
            {
                Status = 1,
                Message = "OK",
                Data = model.ToList()
            };
            return ResponseMessage(result);
        }
        // Tra cứu thông tin sản phẩm
        [Route("getproduct")]
        [HttpGet]
        public HttpResponseMessage GetProduct(string serial)
        {
            var result = new ProductRes();
            var model = db.Products.Where(a => a.Serial == serial).SingleOrDefault();
            if (model == null)
            {
                // Sản phẩm không tồn tại
                result = new ProductRes()
                {
                    Status = -1,
                    Message = "Sản phẩm không tồn tại.",
                    Data = new List<ProductActiveModel>()
                };
            }
            else
            {
                // 
                if (model.Status == 1)
                {
                    // Sản phẩm đã được kích hoạt
                    var product = new ProductActiveModel()
                    {
                        Activedate = model.ActiveDate,
                        //Buydate = model.Buydate,
                        BuyAdr = model.NameAgency, // Tên Đại lý
                        Id = model.Id,
                        Name = model.Name,
                        Code = model.Code,
                        Model = model.Model,
                        Serial = model.Serial,
                        Limited = model.Limited,

                    };
                    result = new ProductRes()
                    {
                        Status = 1,
                        Message = "Sản phẩm đã được kích hoạt.",
                        Data = new List<ProductActiveModel>() { product },
                    };
                }
                else
                {
                    //chua kich hoat
                    var product = new ProductActiveModel()
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Code = model.Code,
                        Model = model.Model,
                        Serial = model.Serial,
                        Limited = model.Limited,

                    };
                    result = new ProductRes()
                    {
                        Status = 0,
                        Message = "Sản phẩm chưa được kích hoạt.",
                        Data = new List<ProductActiveModel>() { product },
                    };

                }
            }
            return ResponseMessage(result);
        }
        //tra cuu thong tin khach hang
        [Route("getcustomer")]
        [HttpGet]
        public HttpResponseMessage GetCustomer(string phone)
        {
            try
            {

                var customer = db.Customers.Where(p => p.Phone == phone).OrderByDescending(p => p.ActiveDate).FirstOrDefault();
                var model = new CustomerModel()
                {
                    Id = customer.Id,
                    Phone = customer.Phone,
                    Name = customer.Name,
                    Address = customer.Address,
                    Ward = customer.Ward,
                    District = customer.County,
                    City = customer.City
                };
                var result = new CustomerRes()
                {
                    Status = 1,
                    Message = "OK",
                    Data = new List<CustomerModel>() { model },
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                var result = new CustomerRes()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<CustomerModel>()
                };
                return ResponseMessage(result);
            }

        }
        //tra cuu bao hanh kich hoat
        [Route("getactive")]
        [HttpGet]
        public HttpResponseMessage GetActive(string serial)
        {
            try
            {
                var customer = db.Customers.Where(a => a.Serial == serial).FirstOrDefault();
                var product = db.Products.Where(a => a.Serial == serial).FirstOrDefault();
                var model = new ActiveModel()
                {
                    CusName = customer.Name,
                    Phone = customer.Phone,
                    Address = customer.Address + " " + customer.Ward + " " + customer.County + " " + customer.City,
                    Serial = customer.Serial,
                    Limited = product.Limited,
                    Activedate = customer.ActiveDate.Value.ToString("dd/MM/yyyy"),
                    Enddate = customer.EndDate.Value.ToString("dd/MM/yyyy"),
                };
                var res = new ActiveRes()
                {
                    Status = 1,
                    Message = "OK",
                    Data = new List<ActiveModel>() { model }
                };
                return ResponseMessage(res);
            }
            catch (Exception ex)
            {
                var res = new ActiveRes()
                {
                    Status = -1,
                    Message = "Sản phẩm không tồn tại.",
                    Data = new List<ActiveModel>()
                };
                return ResponseMessage(res);
            }

        }
        //danh sach kich hoat
        [Route("getlistactive")]
        [HttpGet]
        public HttpResponseMessage GetListActive(string UserId = "", string str_date = "", string end_date = "", string serial = "", string status = "")
        {
            try
            {
                var model = from a in db.Customers
                            join b in db.AspNetUsers on a.NameAgency equals b.NameAgency
                            select new ActiveListModel()
                            {
                                CusName = a.Name,
                                Phone = a.Phone,
                                Address = a.Address + " " + a.Ward + " " + a.County + " " + a.City,
                                Serial = a.Serial,
                                Activedate = a.ActiveDate,
                                AgentId = b.Id,
                                Amount = 0
                            };

                model = model.Where(a => a.AgentId == UserId);
                if (!string.IsNullOrEmpty(str_date))
                {
                    var from_date_str = DateTime.Parse(str_date, new System.Globalization.CultureInfo("pt-BR"));
                    from_date_str = from_date_str.AddDays(-1);
                    model = model.Where(da => da.Activedate > from_date_str);
                }
                if (!string.IsNullOrEmpty(end_date))
                {
                    var to_date_str = DateTime.Parse(end_date, new System.Globalization.CultureInfo("pt-BR"));
                    to_date_str = to_date_str.AddDays(1);
                    model = model.Where(da => da.Activedate < to_date_str);
                }
                if (!string.IsNullOrEmpty(serial))
                {
                    model = model.Where(a => a.Serial == serial);
                }
                if (!string.IsNullOrEmpty(status))
                {
                    if (status == "0")
                    {
                        model = model.Where(a => a.Thanhtoan == null);
                    }
                    else if (status == "1")
                    {
                        model = model.Where(a => a.Thanhtoan != null);
                    }
                }
                var result = new ActiveListRes()
                {
                    Status = 1,
                    Message = "OK",
                    Data = model.ToList()
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                var result = new ActiveListRes()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<ActiveListModel>()
                };
                return ResponseMessage(result);
            }


        }
        //tra cuu thong tin phieu bao hanh
        [Route("getwarranti")]
        [HttpGet]
        public HttpResponseMessage GetWarranti(string serial = "", string phone = "")
        {
            try
            {

                var model = from a in db.Warrantis
                            join b in db.AddKeys on a.Id equals b.IdWarranti into temp
                            from t in temp.DefaultIfEmpty()

                            join k in db.AspNetUsers on t.IdKey equals k.Id into temp2
                            from t2 in temp2.DefaultIfEmpty()

                            join kt in db.AspNetUsers on t.IdKTV equals kt.Id into temp3
                            from t3 in temp3.DefaultIfEmpty()
                            select new WarrantiModel()
                            {
                                Id = a.Id,
                                Serial = a.Serial,
                                Phone = a.Phone,
                                Phone2 = a.PhoneWarr,
                                CusName = a.Name,
                                Address = a.Address + " " + a.Ward + " " + a.District + " " + a.Province,
                                Note = a.Note,
                                Status = a.Status,
                                Createdate = a.Createdate,
                                Createby = a.Createby,
                                KeyWarranti = t2.UserName,
                                KTV = t3.UserName,
                                Comment = t.Comeback,
                                ProductCate = (a.ProductCate != null) ? a.ProductCate : "Bổ sung sau",
                                Model = (a.Model != null) ? a.Model : "Bổ sung sau",
                                CodeWarr = a.CodeWarr,
                                LogWarrantis = db.LogWarantis.Where(a => a.IdWarranti == a.Id).ToList()
                            };

                if (!string.IsNullOrEmpty(serial))
                {
                    model = model.Where(a => a.Serial == serial).OrderByDescending(a => a.Createdate);
                }
                else if (!string.IsNullOrEmpty(phone))
                {

                    model = model.Where(a => a.Phone == phone || a.Phone2 == phone).OrderByDescending(a => a.Createdate);
                }
                else
                {
                    model = model.Where(a => a.Id == 0);//khong tim thay thong tin
                }
                var result = new SearchWarrantiRes()
                {
                    Status = 1,
                    Message = "OK",
                    Data = new List<WarrantiModel>() { model.FirstOrDefault() }
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                var result = new SearchWarrantiRes()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<WarrantiModel>()
                };
                return ResponseMessage(result);
            }

        }
        //danh sach phieu bao hanh theo tai khoan
        [Route("getlistwarranti")]
        [HttpGet]
        public HttpResponseMessage GetListWarranti(string UserId = "", string str_date = "", string end_date = "", int status = -1, string textSearch = "")
        {
            try
            {
                var model = from a in db.Warrantis
                            join b in db.AddKeys on a.Id equals b.IdWarranti into temp
                            from t in temp.DefaultIfEmpty()

                            join k in db.AspNetUsers on t.IdKey equals k.Id into temp2
                            from t2 in temp2.DefaultIfEmpty()

                            join kt in db.AspNetUsers on t.IdKTV equals kt.Id into temp3
                            from t3 in temp3.DefaultIfEmpty()
                            select new WarrantiModel()
                            {
                                Id = a.Id,
                                Serial = a.Serial,
                                Phone = a.Phone,
                                Phone2 = a.PhoneWarr,
                                CusName = a.Name,
                                Address = a.Address + " " + a.Ward + " " + a.District + " " + a.Province,
                                Note = a.Note,
                                Status = a.Status,
                                Createdate = a.Createdate,
                                Createby = a.Createby,
                                IdKey = t2.Id,
                                KeyWarranti = t2.UserName,
                                IdKTV = t3.Id,
                                KTV = t3.UserName,
                                ProductCate = (a.ProductCate != null) ? a.ProductCate : "Bổ sung sau",
                                Model = (a.Model != null) ? a.Model : "Bổ sung sau",
                                CodeWarr = a.CodeWarr,
                                LogWarrantis = db.LogWarantis.Where(l => l.IdWarranti == a.Id).ToList()
                            };
                if (!string.IsNullOrEmpty(textSearch))
                {
                    model = model.Where(a => a.ProductCate.Contains(textSearch) || a.Model.Contains(textSearch)
                    || a.CodeWarr.Contains(textSearch) || a.Serial.Contains(textSearch) || a.Phone.Contains(textSearch) || a.Phone2.Contains(textSearch));
                }
                if (!string.IsNullOrEmpty(UserId))
                {
                    var user = db.AspNetUsers.Find(UserId);
                    if (user.AspNetUserRoles.FirstOrDefault().RoleId == "Key")
                    {
                        model = model.Where(a => a.IdKey == user.Id);
                    }
                    else if (user.AspNetUserRoles.FirstOrDefault().RoleId == "KTV")
                    {
                        model = model.Where(a => a.IdKTV == user.Id);
                    }
                }
                if (status != -1)
                {
                    model = model.Where(a => a.Status == status);
                }
                if (!string.IsNullOrEmpty(str_date))
                {
                    var from_date_str = DateTime.Parse(str_date, new System.Globalization.CultureInfo("pt-BR"));
                    from_date_str = from_date_str.AddDays(-1);
                    model = model.Where(da => da.Createdate > from_date_str);
                }
                if (!string.IsNullOrEmpty(end_date))
                {
                    var to_date_str = DateTime.Parse(end_date, new System.Globalization.CultureInfo("pt-BR"));
                    to_date_str = to_date_str.AddDays(1);
                    model = model.Where(da => da.Createdate < to_date_str);
                }

                var result = new SearchWarrantiRes()
                {
                    Status = 1,
                    Message = "OK",
                    Data = model.OrderByDescending(a => a.Createdate).ToList()
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                var result = new SearchWarrantiRes()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<WarrantiModel>()
                };
                return ResponseMessage(result);
            }
        }
        //danh sach tram bao hanh
        [Route("getlistkeywarranti")]
        [HttpGet]
        public HttpResponseMessage GetListKeyWarranti(string textSearch = "")
        {
            var model = from a in db.AspNetUsers
                        from b in a.AspNetUserRoles
                        where b.RoleId == "Key"
                        select new UserModel()
                        {
                            Id = a.Id,
                            UserName = a.UserName,
                        };
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.UserName.Contains(textSearch));
            }
            var result = new ListKeyRes()
            {
                Status = 1,
                Message = "Ok",
                Data = model.OrderBy(x => x.UserName).ToList(),
            };
            return ResponseMessage(result);
        }
        //danh sach nhan vien bao hanh
        [Route("getlistktv")]
        [HttpGet]
        public HttpResponseMessage GetListKTV(string UserId, string textSearch = "")
        {
            var model = from a in db.AspNetUsers
                        from b in a.AspNetUserRoles
                        where a.Createby == UserId
                        select new UserModel()
                        {
                            Id = a.Id,
                            UserName = a.UserName,
                        };
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.UserName.Contains(textSearch));
            }
            var result = new ListKeyRes()
            {
                Status = 1,
                Message = "Ok",
                Data = model.OrderBy(x => x.UserName).ToList(),
            };
            return ResponseMessage(result);
        }

        [Route("getlistagent")]
        [HttpGet]
        public HttpResponseMessage GetListAgent(string UserId, string textSearch = "")
        {
            var model = from a in db.AspNetUsers
                        from b in a.AspNetUserRoles
                        where b.RoleId == "Agency"
                        select new UserModel()
                        {
                            Id = a.Id,
                            UserName = a.UserName,
                        };
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.UserName.Contains(textSearch));
            }
            var result = new ListKeyRes()
            {
                Status = 1,
                Message = "Ok",
                Data = model.OrderBy(x => x.UserName).ToList(),
            };
            return ResponseMessage(result);
        }
        //danh sach log trang thai
        [Route("getlogwarr")]
        [HttpGet]
        public HttpResponseMessage GetLogWarr(long id)
        {
            var log = db.LogWarantis.Where(x => x.IdWarranti == id).OrderByDescending(x => x.Createdate);
            var result = new LogWarrRes()
            {
                Status = 1,
                Message = "OK",
                Data = log.ToList(),
            };
            return ResponseMessage(result);
        }

        [Route("getbanners")]
        [HttpGet]
        public HttpResponseMessage GetBanner()
        {
            var model = db.Banners.OrderByDescending(a => a.Createdate);
            var result = new BannerRes()
            {
                Status = 1,
                Message = "OK",
                Data = model.ToList()
            };
            return ResponseMessage(result);

        }
        [Route("getarticles")]
        [HttpGet]
        public HttpResponseMessage GetArticle(string url = "")
        {

            var model = db.Articles.OrderByDescending(a => a.CreateDate);
            Article artice = new Article();
            if (!string.IsNullOrEmpty(url))
            {
                artice = model.Where(a => a.Url == url).SingleOrDefault();
                var res = new ArticleRes()
                {
                    Status = 1,
                    Message = "Danh sách tin tức",
                    Data = new List<Article>() { artice }
                };
                return ResponseMessage(res);
            }
            var result = new ArticleRes()
            {
                Status = 1,
                Message = "Danh sách tin tức",
                Data = model.ToList()
            };
            return ResponseMessage(result);

        }

        //kich hoat bao hanh
        [Route("active")]
        [HttpPost]
        public HttpResponseMessage Active(ActiveReq model)
        {

            try
            {
                string json = JsonConvert.SerializeObject(model);
                logger.Info(json);
                var product = db.Products.Find(model.IDProduct);
                var customer = model.Customer;
                string activeby = "";
                if (!string.IsNullOrEmpty(model.UserId))
                {
                    var user = db.AspNetUsers.Find(model.UserId);
                    if (user != null)
                    {
                        activeby = user.UserName;
                    }

                }
                if (product != null)
                {
                    if (!string.IsNullOrEmpty(activeby) && product.UserName != activeby)
                    {
                        var result = new Result()
                        {
                            Status = -1,
                            Message = "Đại lý không có quyền kích hoạt sản phẩm.",
                        };
                        return ResponseMessage(result);
                    }

                    if (product.Status == 1)//kich hoat roi
                    {
                        var result = new Result()
                        {
                            Status = -1,
                            Message = "Sản phẩm đã được kích hoạt trước đó.",
                        };
                        return ResponseMessage(result);
                    }
                    else if (product.Status == -2)
                    {
                        var result = new Result()
                        {
                            Status = -1,
                            Message = "Sản phẩm không được kích hoạt.",
                        };
                        return ResponseMessage(result);
                    }
                    else
                    {
                        //edit thong tin san pham
                        customer.Phone = Utils.ConvertTo.FormatPhoneStartWith84(customer.Phone);
                        product.ActiveDate = DateTime.Now;
                        if (product.Limited != null)
                        {
                            product.EndDate = DateTime.Now.AddMonths(product.Limited ?? default(int));//convert int? to int
                        }
                        product.PhoneActive = customer.Phone;
                        product.Status = 1;//1: active 0: chua active 2: het han
                        product.Buydate = ConvertTo.TryParse(model.Buydate);
                        //product.Installdate = ConvertTo.TryParse(installdate);
                        db.Entry(product).State = System.Data.Entity.EntityState.Modified;//active to product

                        //add thogn tin kich hoat
                        Customer active = new Customer();
                        active.Name = customer.Name;
                        active.Serial = product.Serial;
                        active.Phone = customer.Phone;
                        active.ActiveDate = DateTime.Now;
                        active.Address = customer.Address;
                        active.Ward = customer.Ward;

                        active.City = customer.City;
                        active.County = customer.District;
                        //active.Birthday = ConvertTo.TryParse(birthday);
                        active.Buydate = ConvertTo.TryParse(model.Buydate);
                        //active.Installdate = ConvertTo.TryParse(installdate);
                        active.Email = customer.Email;
                        active.CreateBy = activeby;//dai ly kich hoat cho khach hang
                        active.CreateDate = DateTime.Now;
                        if (product.Limited != null)
                        {
                            active.EndDate = DateTime.Now.AddMonths(product.Limited ?? default(int));//convert int? to int
                        }
                        active.CodeProduct = product.Code;
                        active.NameAgency = product.NameAgency;
                        active.ActiveWho = 1;
                        active.Type = 1;
                        active.Status = 1;
                        db.Customers.Add(active);

                        db.SaveChanges();//<-----------------
                        logger.Info("active success");

                        var result = new ProductRes()
                        {
                            Status = 1,
                            Message = string.Format(TempMessage.active_success,
                            product.Serial, active.ActiveDate, active.ActiveDate.Value.AddMonths(product.Limited ?? default(int))),
                            Data = new List<ProductActiveModel>()
                        };
                        return ResponseMessage(result);
                    }
                }
                else//khong co thong tin
                {
                    var result = new Result()
                    {
                        Status = -1,
                        Message = "Sản phẩm không tồn tại trong hệ thống.",
                    };
                    return ResponseMessage(result);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                var result = new ProductRes()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<ProductActiveModel>()
                };
                return ResponseMessage(result);
            }

        }
        //yeu cau bao hanh
        [Route("warranti")]
        [HttpPost]
        public HttpResponseMessage Warranti(WarrantiReq model)
        {
            try
            {
                string json = JsonConvert.SerializeObject(model);
                logger.Info(json);
                if (string.IsNullOrEmpty(model.Phone) && string.IsNullOrEmpty(model.Phone2))
                {
                    var resultErr = new ProductRes()
                    {
                        Status = -1,
                        Message = "Yêu cầu nhập SĐT để liên hệ bảo hành.",
                        Data = new List<ProductActiveModel>()
                    };
                    return ResponseMessage(resultErr);
                }
                if (!string.IsNullOrEmpty(model.Phone))
                {
                    model.Phone = model.Phone;
                }
                string createby = "";
                bool key = false;
                if (!string.IsNullOrEmpty(model.UserId))
                {
                    var user = db.AspNetUsers.Find(model.UserId);
                    if (user != null)
                    {
                        createby = user.UserName;
                        if (user.AspNetUserRoles.FirstOrDefault().RoleId == "Key")
                        {
                            key = true;
                        }

                    }
                }
                var warranti = new Warranti();
                warranti.Createby = createby;
                warranti.Phone = model.Phone;
                warranti.PhoneWarr = model.Phone2;
                warranti.Serial = model.Serial;
                warranti.Model = model.Model;
                warranti.ProductCate = model.ProductCate;
                warranti.Agent = model.Agent;
                warranti.Category = model.Category;
                warranti.Extra = model.Extra;
                warranti.IdProduct = model.IDProduct;
                warranti.IdCustomer = model.IdCustomer;
                warranti.Address = model.Address;
                warranti.Ward = model.Ward;
                warranti.District = model.District;
                warranti.Province = model.Province;
                warranti.Note = model.Note;
                warranti.Name = model.CusName;
                warranti.Chanel = model.Chanel;
                warranti.Status = 0;
                warranti.Createdate = DateTime.Now;
                warranti.Outdate = false;

                db.Warrantis.Add(warranti);
                db.SaveChanges();
                //tao ma cho phieu
                warranti.CodeWarr = Utils.Robot.Get_Code_Warranti(warranti.Id.ToString());//tao code
                db.Entry(warranti).State = EntityState.Modified;
                var log = new LogWaranti()
                {
                    Createdate = DateTime.Now,
                    Detail = createby + " Tạo mới thông tin bảo hành từ App",
                    IdWarranti = warranti.Id
                };
                db.LogWarantis.Add(log);
                db.SaveChanges();
                //chuyen luon phieu nay cho tram
                if (key)
                {
                    //string id = User.Identity.GetUserId();
                    var addkey = new AddKey();
                    addkey.IdWarranti = warranti.Id;
                    addkey.IdKey = model.UserId;
                    addkey.Createdate = DateTime.Now;
                    addkey.Deadline = DateTime.Now.AddDays(5);
                    db.AddKeys.Add(addkey);
                    //doi trang thai thanh chuyen tram
                    warranti.Status = 2;
                    db.Entry(warranti).State = EntityState.Modified;
                    var log2 = new LogWaranti()
                    {
                        Createdate = DateTime.Now,
                        Detail = "Chuyển phiếu đến trạm bảo hành",
                        IdWarranti = warranti.Id
                    };
                    db.LogWarantis.Add(log2);
                    db.SaveChanges();
                }

                var result = new ProductRes()
                {
                    Status = 1,
                    Message = TempMessage.warranti_success,
                    Data = new List<ProductActiveModel>()
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                var result = new ProductRes()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<ProductActiveModel>()
                };
                return ResponseMessage(result);
            }

        }
        //danh sach phan loai
        [Route("category")]
        [HttpGet]
        public HttpResponseMessage GetCategory()
        {
            List<string> model = new List<string>() { "Bảo hành", "Tính phí", "Lắp đặt" };
            var result = new ListStringRes()
            {
                Status = 1,
                Message = "OK",
                Data = model
            };
            return ResponseMessage(result);
        }
        //danh sach loai san pham
        [Route("productcate")]
        [HttpGet]
        public HttpResponseMessage GetProductCate(string textSearch = "")
        {
            var cate = db.Models.Select(a => a.ProductCate);
            if (!string.IsNullOrEmpty(textSearch))
            {
                cate = cate.Where(a => a.Contains(textSearch));
            }
            var model = cate.GroupBy(a => a).Where(b => b.Count() > 0).Select(g => g.Key);
            var list = new List<String>();
            list.Add("Bổ sung sau");
            list.AddRange(model.ToList());
            var result = new ListStringRes()
            {
                Status = 1,
                Message = "OK " + model.Count(),
                Data = list
            };
            return ResponseMessage(result);
        }

        [Route("model")]
        [HttpGet]
        public HttpResponseMessage GetModel(string cate, string textSearch = "")
        {
            var model = db.Models.Where(a => a.ProductCate.Contains(cate)).Select(a => a.Model1);
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.Contains(textSearch));
            }
            var list = new List<String>();
            list.Add("Bổ sung sau");
            list.AddRange(model.ToList());
            var result = new ListStringRes()
            {
                Status = 1,
                Message = "OK",
                Data = list
            };
            return ResponseMessage(result);
        }
        //chuyen  phieu cho tram bao hanh
        [Route("addkeywarr")]
        [HttpPost]
        public HttpResponseMessage AddKeyWarr(AddKeyWarrReq model)
        {
            string json = JsonConvert.SerializeObject(model);
            logger.Info(json);
            var warranti = db.Warrantis.Find(model.IDWarranti);
            var user = db.AspNetUsers.Find(model.KeyWarranti);
            if (model.Status == 2)
            {

                var log = new LogWaranti()
                {
                    Createdate = DateTime.Now,
                    Detail = "Chuyển cho trạm bảo hành: " + user.UserName,
                    IdWarranti = warranti.Id
                };
                db.LogWarantis.Add(log);
                var addkey = new AddKey();
                addkey.Deadline = model.Deadline;
                addkey.IdWarranti = warranti.Id;
                addkey.IdKey = user.Id;
                addkey.Createdate = DateTime.Now;
                db.AddKeys.Add(addkey);
                //luu thông tin vao db
                db.SaveChanges();//<--------------
                warranti.Status = 2;
                db.Entry(warranti).State = EntityState.Modified;
                db.SaveChanges();//<----------------

                var result = new ProductRes()
                {
                    Status = 1,
                    Message = "OK",
                    Data = new List<ProductActiveModel>()

                };
                return ResponseMessage(result);
            }
            else
            {
                var key = db.AddKeys.Where(a => a.IdWarranti == warranti.Id).SingleOrDefault();
                db.AddKeys.Remove(key);

                var log = new LogWaranti()
                {
                    Createdate = DateTime.Now,
                    Detail = "Thu hồi ",
                    IdWarranti = warranti.Id
                };
                db.LogWarantis.Add(log);

                warranti.Status = 0;
                db.Entry(warranti).State = EntityState.Modified;
                db.SaveChanges();//<----------------
                var result = new ProductRes()
                {
                    Status = 1,
                    Message = "Đã thu hồi phiếu bảo hành.",
                    Data = new List<ProductActiveModel>()
                };
                return ResponseMessage(result);
            }

        }
        //chuyen phieu cho tho bao hanh
        [Route("addktv")]
        [HttpPost]
        public HttpResponseMessage AddKTV(AddKTVReq model)
        {
            try
            {
                string json = JsonConvert.SerializeObject(model);
                logger.Info(json);
                var check = db.AddKeys.Where(a => a.IdWarranti == model.IDWarranti).SingleOrDefault();

                if (check.IdKTV != null)
                {
                    var user = db.AspNetUsers.Find(check.IdKTV);
                    var resultEr = new ProductRes()
                    {
                        Status = -1,
                        Message = "Phiếu đã được tiếp nhận bởi " + user.UserName,
                        Data = new List<ProductActiveModel>()
                    };
                    return ResponseMessage(resultEr);
                }
                var ktv = db.AspNetUsers.Find(model.IDKTV);
                //add ktv
                check.IdKTV = model.IDKTV;
                db.Entry(check).State = EntityState.Modified;
                //update trang thai phieu bao hanh
                var warranti = db.Warrantis.Find(model.IDWarranti);
                warranti.Status = 5;//dang xu ly
                db.Entry(warranti).State = EntityState.Modified;
                //luu log trang thai
                var log = new LogWaranti()
                {
                    Createdate = DateTime.Now,
                    Detail = "Chuyển phiếu cho KTV: " + ktv.UserName,
                    IdWarranti = model.IDWarranti
                };
                db.LogWarantis.Add(log);
                //gửi notify cho thợ khi nhận được phiếu
                var sent = new SentNotify();
                sent.Sent(ktv.UserName, string.Format("Bạn nhận được 1 yêu cầu xử lý cho phiếu có mã {0}", warranti.CodeWarr));
                db.SaveChanges();
                var result = new ProductRes()
                {
                    Status = 1,
                    Message = "Đã chuyển phiếu thành công cho KTV.",
                    Data = new List<ProductActiveModel>()
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                var result = new ProductRes()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<ProductActiveModel>()
                };
                return ResponseMessage(result);
            }

        }
        //xem chi tiet phieu bao hanh
        [Route("getwarrantidetail")]
        [HttpGet]
        public HttpResponseMessage GetDetail(long Id)
        {
            var warranti = db.Warrantis.Find(Id);
            var addkey = db.AddKeys.Where(a => a.IdWarranti == warranti.Id).SingleOrDefault();
            var listcheckout = from a in db.ImageWarrs where a.IDKey == addkey.Id && a.Type == 2 select a.Image;
            var listcheckin = from a in db.ImageWarrs where a.IDKey == addkey.Id && a.Type == 1 select a.Image;
            var listdevice = from a in db.Phutung_Warranti.Where(a => a.IdKey == Id)
                             select new PhuTung()
                             {
                                 ID = a.Id,
                                 IDWarranti = a.IdKey,
                                 IDPhutung = a.IdDevice,
                                 Price = a.Price,
                                 Name = a.Name,
                                 Amount = a.Amount
                             };

            var key = db.AspNetUsers.Find(addkey.IdKey);
            var ktv = db.AspNetUsers.Find(addkey.IdKTV);

            var model = new UpdateDetailReq()
            {
                Id = addkey.Id,
                Serial = warranti.Serial,
                Phone = warranti.Phone + " " + warranti.PhoneWarr,
                Address = warranti.Address + " " + warranti.Ward + " " + warranti.District + " " + warranti.Province,
                CusName = warranti.Name,
                Details = warranti.Note,
                Createdate = warranti.Createdate,

                KeyWarranti = key.UserName,
                PhoneKTV = ktv.PhoneNumber,
                KTV = ktv.UserName,
                ProductCate = (warranti.ProductCate != null) ? warranti.ProductCate : "Bổ sung sau",
                Model = (warranti.Model != null) ? warranti.Model : "Bổ sung sau",
                //SerialHot = warranti.SerialHot,
                //SerialCool = warranti.SerialCool,
                FeeSuggest = addkey.Cost,
                CheckIns = listcheckin.ToList(),
                Long = addkey.Long,
                Lat = addkey.Lat,
                Note = addkey.Comeback,
                Devices = listdevice.ToList(),
                CheckOuts = listcheckout.ToList(),
                Price = addkey.Price,
                KM = addkey.KM,
                MoveFee = addkey.MoveFee ?? 0,
                Amount = addkey.Amount,
                Sign = addkey.Sign

            };
            var result = new DetailWarrantiRes()
            {
                Status = 1,
                Message = "Ok",
                Data = new List<UpdateDetailReq>() { model }
            };
            return ResponseMessage(result);
        }
        //hoanh thanh phieu 
        [Route("updatedetails")]
        [HttpPost]
        public HttpResponseMessage Update(UpdateDetailReq detailReq)
        {
            try
            {
                string json = JsonConvert.SerializeObject(detailReq);
                logger.Info("update req = " + json);
                string msg = User.Identity.Name + "Cập nhật trạng thái ";

                var addkey = db.AddKeys.Where(a => a.Id == detailReq.Id).SingleOrDefault();

                var warranti = db.Warrantis.Find(addkey.IdWarranti);

                warranti.ProductCate = detailReq.ProductCate;
                warranti.Model = detailReq.Model;
                warranti.Serial = detailReq.Serial;

                int suggest = 0;
                if (detailReq.Devices.Count() > 0)
                {
                    msg = msg + "Hoàn thành";
                    addkey.Successdate = DateTime.Now;
                    if(addkey.Successdate != null && warranti.Createdate !=null)
                    {
                        warranti.CountDay = (addkey.Successdate.Value.Date - warranti.Createdate.Value.Date).Days;
                    }
                    warranti.Status = 1;
                    warranti.Outdate = false;
                    //add thong tin phu tung
                    var _phutung = db.Phutung_Warranti.Where(a => a.IdKey == addkey.Id);
                    if (_phutung.Count() > 0)
                    {
                        db.Phutung_Warranti.RemoveRange(_phutung);
                        db.SaveChanges();
                    }
                    foreach (var item in detailReq.Devices)
                    {
                        suggest = suggest + (int)item.Price;
                        var phutung = new Phutung_Warranti()
                        {
                            IdKey = addkey.Id,
                            IdDevice = item.IDPhutung,
                            Price = (int)item.Price,
                            Quantity = 1,
                            Amount = (int)item.Price,
                            Name = db.Devices.Find(item.IDPhutung).Name
                        };
                        db.Phutung_Warranti.Add(phutung);
                        db.SaveChanges();
                    }

                }
                else
                {
                    msg = msg + "Chờ linh kiện";
                    warranti.Status = 7;
                }
                addkey.Cost = suggest;
                addkey.Lat = detailReq.Lat;
                addkey.Long = detailReq.Long;
                addkey.Comeback = detailReq.Note;
                addkey.Price = (int?)detailReq.Price;
                addkey.Amount = (int?)detailReq.Amount;
                addkey.KM = detailReq.KM;
                addkey.MoveFee = (int?)detailReq.MoveFee;
                addkey.Sign = detailReq.Sign;

                db.Entry(warranti).State = EntityState.Modified;
                db.Entry(addkey).State = EntityState.Modified;
                var log = new LogWaranti()
                {
                    IdWarranti = warranti.Id,
                    Createdate = DateTime.Now,
                    Detail = msg
                };
                db.LogWarantis.Add(log);
                db.SaveChanges();
                var iol = db.ImageWarrs.Where(a => a.IDKey == addkey.Id);
                if (iol.Count() > 0)
                {
                    db.ImageWarrs.RemoveRange(iol);
                    db.SaveChanges();
                }
                //add check in
                foreach (var item in detailReq.CheckIns)
                {
                    var image = new ImageWarr()
                    {
                        IDKey = addkey.Id,
                        Image = item,
                        Type = 1

                    };
                    db.ImageWarrs.Add(image);
                }
                db.SaveChanges();
                //add check out
                foreach (var item in detailReq.CheckOuts)
                {
                    var image = new ImageWarr()
                    {
                        IDKey = addkey.Id,
                        Image = item,
                        Type = 2

                    };
                    db.ImageWarrs.Add(image);
                }
                db.SaveChanges();
                var result = new ProductRes()
                {
                    Status = 1,
                    Message = msg,
                    Data = new List<ProductActiveModel>()
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                var result = new ProductRes()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<ProductActiveModel>()
                };
                return ResponseMessage(result);
            }

        }
        //gia phu tung
        [Route("getdeviceprice")]
        [HttpGet]
        public HttpResponseMessage GetDevicePrice(string textSearch = "")
        {
            var model = from a in db.Devices
                        select new PhutungPriceModel()
                        {
                            ID = a.Id,
                            Name = a.Name,
                            Code = a.Code,
                            Price = a.Price,
                            Createdate = a.Createdate
                        };
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.Name.Contains(textSearch) || a.Code.Contains(textSearch));
            }
            var result = new DevicePriceRes()
            {
                Status = 1,
                Message = "OK",
                Data = model.OrderByDescending(a=>a.Createdate).Take(100).ToList()
            };
            return ResponseMessage(result);
        }

        private HttpResponseMessage ResponseMessage(Result result)
        {
            string json = JsonConvert.SerializeObject(result);
            var response = new HttpResponseMessage();
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return response;
        }
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? Request.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<HttpResponseMessage> Login(LoginViewModel model)
        {
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password,
                model.RememberMe, shouldLockout: false);
            var res = new UserModelRes()
            {
                Status = 1,
                Message = "Đăng nhập thành công.",
                Data = new List<UserModel>()
            };
            switch (result)
            {
                case SignInStatus.Success:
                    try
                    {
                        var user = from a in db.AspNetUsers
                                   from b in a.AspNetUserRoles
                                   select new UserModel()
                                   {
                                       Id = a.Id,
                                       UserName = a.UserName,
                                       PhoneNumber = a.PhoneNumber,
                                       //Createdate = a.Createdate,
                                       Email = a.Email,
                                       Address = a.Address,
                                       //BankAccount = a.BankAccount,
                                       //BankAccountHolder = a.BankAccountHolder,
                                       //BankName = a.BankName,
                                       //Nguoidaidien = a.Nguoidaidien,
                                       //AgentName = a.AgentName,
                                       Taxcode = a.TaxCode,
                                       Role = b.RoleId
                                   };
                        res.Data = new List<UserModel>() { user.FirstOrDefault(a => a.UserName == model.UserName) };
                    }
                    catch (Exception ex)
                    {
                        logger.Info(ex);
                    }

                    return ResponseMessage(res);
                case SignInStatus.LockedOut:
                    res.Status = -1;
                    res.Message = "Tài khoản đã bị khóa.";
                    return ResponseMessage(res);
                case SignInStatus.Failure:
                default:
                    res.Status = -1;
                    res.Message = "Đăng nhập không thành công";
                    return ResponseMessage(res);
            }
        }
        [Route("logout")]
        [HttpPost]
        public HttpResponseMessage Logout()
        {
            IAuthenticationManager AuthenticationManager = Request.GetOwinContext().Authentication;
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            var res = new UserModelRes()
            {
                Status = 1,
                Message = "Đăng xuất thành công.",
                Data = new List<UserModel>()
            };
            return ResponseMessage(res);
        }
        [Route("changepassword")]
        [HttpPost]
        public async Task<HttpResponseMessage> ChangePassword(ChangePasswordViewModel model)
        {
            var user = db.AspNetUsers.Where(a => a.UserName == model.UserName).SingleOrDefault();
            if (user != null)
            {
                var result = await UserManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var usermanage = await UserManager.FindByIdAsync(user.Id);
                    if (usermanage != null)
                    {
                        await SignInManager.SignInAsync(usermanage, isPersistent: false, rememberBrowser: false);
                    }
                    var res = new UserModelRes()
                    {
                        Status = 1,
                        Message = "Đổi mật khẩu thành công.",
                        Data = new List<UserModel>()
                    };
                    return ResponseMessage(res);
                }
            }

            var resul = new UserModelRes()
            {
                Status = -1,
                Message = "Đổi mật khẩu không thành công.",
                Data = new List<UserModel>()
            };
            return ResponseMessage(resul);
        }

        [Route("postdevice")]
        [HttpPost]
        public HttpResponseMessage PostDevice(User_Device user)
        {
            try
            {
                var userdevice = db.User_Device.Where(a => a.DeviceId == user.DeviceId).SingleOrDefault();
                if (userdevice == null)
                {
                    user.Createdate = DateTime.Now;
                    db.User_Device.Add(user);
                    db.SaveChanges();
                    var result = new ProductRes()
                    {
                        Status = 1,
                        Message = "OK",
                        Data = new List<ProductActiveModel>()
                    };
                    return ResponseMessage(result);
                }
                else
                {
                    var result = new ProductRes()
                    {
                        Status = 1,
                        Message = "Đã được cập nhật trước đó.",
                        Data = new List<ProductActiveModel>()
                    };
                    return ResponseMessage(result);
                }
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                var result = new ProductRes()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<ProductActiveModel>()
                };
                return ResponseMessage(result);
            }
        }
        [Route("uploadimage")]
        [HttpPost]
        public HttpResponseMessage PostUserImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {

                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
                    string link = "";
                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 10; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 1 mb.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                            string strrand = Guid.NewGuid().ToString();


                            var path = HttpContext.Current.Server.MapPath("~/ImageWarr/" + strrand + "_" + postedFile.FileName);

                            postedFile.SaveAs(path);
                            ResizeSettings resizeSetting = new ResizeSettings
                            {
                                MaxWidth = 800,
                                MaxHeight = 800,
                            };
                            ImageBuilder.Current.Build(path, path, resizeSetting);
                            link = "/ImageWarr/" + strrand + "_" + postedFile.FileName;
                        }
                    }

                    var ssresult = new UploadImageRes()
                    {
                        Status = 1,
                        Message = "Tải ảnh lên thành công.",
                        Data = new List<string>() { link }
                    };
                    return ResponseMessage(ssresult);
                }
                var result = new UploadImageRes()
                {
                    Status = -1,
                    Message = "Please upload a image",
                    Data = new List<string>() { }
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                var result = new UploadImageRes()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<string>() { }
                };
                return ResponseMessage(result);
            }
        }

        [Route("getmovefee")]
        [HttpGet]
        public HttpResponseMessage GetMoveFee()
        {
            var model = db.MoveFees.Take(100);
            var result = new MoveFeeRes()
            {
                Status = 1,
                Message = "OK",
                Data = model.ToList()
            };
            return ResponseMessage(result);
        }
    }
}
