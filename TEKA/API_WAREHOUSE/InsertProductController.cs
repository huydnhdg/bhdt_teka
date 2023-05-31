using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using TEKA.API.Model;
using TEKA.Models;

namespace TEKA.API_WAREHOUSE
{
    [RoutePrefix("api/house")]
    public class InsertProductController : ApiController
    {
        TEKAEntities db = new TEKAEntities();
        static Logger logger = LogManager.GetCurrentClassLogger();

        static string PRIVATE_KEY = "tekahouse2021";
        static string CREATE_BY = "Maersk";
        [Route("getproduct")]
        [HttpGet]
        public HttpResponseMessage GetProduct(string privatekey, string serial)
        {
            if (PRIVATE_KEY.Equals(privatekey))
            {
                var product = db.Products.SingleOrDefault(a => a.Serial == serial);
                if (product == null)
                {
                    //không có trong hệ thống
                    var result = new Result()
                    {
                        Status = 0,
                        Message = "Sản phẩm chưa có trong hệ thống"
                    };
                    return ResponseMessage(result);
                }
                else
                {
                    //đã có trong hệ thống
                    var result = new Result()
                    {
                        Status = 1,
                        Message = "Sản phẩm đã tồn tại trong hệ thống"
                    };
                    return ResponseMessage(result);
                }
            }
            else
            {
                //không đúng private key
                var result = new Result()
                {
                    Status = -1,
                    Message = "Private key không đúng."
                };
                return ResponseMessage(result);
            }


        }
        [Route("getusername")]
        [HttpGet]
        public HttpResponseMessage GetUsername(string privatekey, string username)
        {
            if (PRIVATE_KEY.Equals(privatekey))
            {
                var user = db.AspNetUsers.SingleOrDefault(a => a.UserName == username);
                if (user == null)
                {
                    var result = new Result()
                    {
                        Status = 0,
                        Message = "Không tồn tại mã đại lý."
                    };
                    return ResponseMessage(result);
                }
                else
                {
                    var result = new Result()
                    {
                        Status = 1,
                        Message = "Mã đại lý: " + username + "/ tạo ngày: " + user.Createdate + "/ Tên đại lý: " + user.NameAgency
                    };
                    return ResponseMessage(result);
                }
            }
            else
            {
                var result = new Result()
                {
                    Status = -1,
                    Message = "Private key không đúng."
                };
                return ResponseMessage(result);
            }

        }
        [Route("getlistagent")]
        [HttpGet]
        public HttpResponseMessage GetlistAgent(string privatekey)
        {
            if (PRIVATE_KEY.Equals(privatekey))
            {
                var model = from a in db.AspNetUsers
                            from b in a.AspNetUserRoles
                            where b.RoleId == "Agency"
                            select new AgentModel()
                            {
                                Id = a.Id,
                                UserName = a.UserName,
                                AgentName = a.NameAgency
                            };
                var listAgent = model.ToList();
                var result = new ResultData()
                {
                    Status = 0,
                    Message = "Chưa lấy được danh sách đại lý.",
                    Data = listAgent
                };
                return ResponseMessage(result);
            }
            else
            {
                var result = new Result()
                {
                    Status = -1,
                    Message = "Private key không đúng."
                };
                return ResponseMessage(result);
            }


        }
        [Route("insertproduct")]
        [HttpPost]
        public HttpResponseMessage InsertProduct(string privatekey, Product product)
        {
            if (PRIVATE_KEY.Equals(privatekey))
            {
                string json = JsonConvert.SerializeObject(product);
                logger.Info(string.Format("INSERT PRODUCT {0}", json));
                var _product = db.Products.FirstOrDefault(a => a.Serial == product.Serial);
                var _user = db.AspNetUsers.FirstOrDefault(a => a.UserName == product.UserName);

                if (_product != null)
                {
                    var result = new Result()
                    {
                        Status = 1,
                        Message = "Sản phẩm đã tồn tại trong hệ thống"
                    };
                    return ResponseMessage(result);
                }
                else if (_user == null)
                {
                    var result = new Result()
                    {
                        Status = 0,
                        Message = "Không tồn tại mã đại lý."
                    };
                    return ResponseMessage(result);
                }
                else
                {
                    try
                    {
                        product.NameAgency = _user.NameAgency;
                        product.Status = 0;
                        product.CreateBy = CREATE_BY;
                        product.CreateDate = DateTime.Now;
                        product.Quantity = 1;
                        db.Products.Add(product);
                        db.SaveChanges();
                        var result = new Result()
                        {
                            Status = 0,
                            Message = "Đã insert thành công sản phẩm SERIAL: " + product.Serial + "/ Đến đại lý: " + product.NameAgency
                        };
                        return ResponseMessage(result);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        logger.Error(ex.InnerException);
                        var result = new Result()
                        {
                            Status = -1,
                            Message = ex.Message
                        };
                        return ResponseMessage(result);
                    }

                }

            }
            else
            {
                var result = new Result()
                {
                    Status = -1,
                    Message = "Private key không đúng."
                };
                return ResponseMessage(result);
            }


        }
        [Route("deleteproduct")]
        [HttpPost]
        public HttpResponseMessage DeleteProduct(string privatekey, Product product)
        {
            if (PRIVATE_KEY.Equals(privatekey))
            {
                var _product = db.Products.FirstOrDefault(a => a.Serial == product.Serial);
                string json = JsonConvert.SerializeObject(_product);
                logger.Info(string.Format("@DELETE PRODUCT {0}", json));
                if (_product == null)
                {
                    var result = new Result()
                    {
                        Status = 1,
                        Message = "Sản phẩm cần xoá không có trong hệ thống"
                    };
                    return ResponseMessage(result);
                }
                else
                {
                    db.Products.Remove(_product);
                    db.SaveChanges();
                    var result = new Result()
                    {
                        Status = 0,
                        Message = "Đã xoá thành công sản phẩm SERIAL: " + _product.Serial
                    };
                    return ResponseMessage(result);
                }
            }
            else
            {
                var result = new Result()
                {
                    Status = -1,
                    Message = "Private key không đúng."
                };
                return ResponseMessage(result);
            }
        }
        private HttpResponseMessage ResponseMessage(Result result)
        {
            string json = JsonConvert.SerializeObject(result);
            var response = new HttpResponseMessage();
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            logger.Info(json);
            return response;
        }
    }
}
