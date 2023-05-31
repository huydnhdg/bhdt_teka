using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;

namespace TEKA.Controllers
{
    [RoutePrefix("tra-cuu-kich-hoat")]
    public class SearchActiveController : BaseController
    {
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
        [Route]
        public ActionResult Index(string phone)
        {
            if (!string.IsNullOrEmpty(phone))
            {
                ViewBag.phone = phone;
                string username = User.Identity.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    var customers = from a in db.Products
                                    join b in db.Customers on a.Serial equals b.Serial into temp
                                    from t in temp.DefaultIfEmpty()
                                    select new ActiveSearch()
                                    {
                                        Id = a.Id,
                                        Name = a.Name,                                        
                                        NameAgency = a.NameAgency,
                                        Status = a.Status,
                                        Buydate = a.Buydate,
                                        Exportdate = a.ExportDate,
                                        Serial = a.Serial,
                                        Product = a.Name,
                                        Model = a.Model,
                                        ActiveDate = a.ActiveDate,
                                        EndDate = a.EndDate,

                                        Phone = t.Phone,
                                        Address = t.Address,
                                        County = t.County,
                                        City = t.City,
                                        Birthday = t.Birthday,
                                        CodeProduct = t.CodeProduct,
                                        ActiveWho = t.ActiveWho,
                                    };
                    customers = customers.Where(a => a.Serial == phone || a.Phone == phone);
                    return View(customers);
                }
                else
                {
                    var customers = from a in db.Customers
                                    join b in db.Products on a.Serial equals b.Serial
                                    where b.Status == 1
                                    where a.Status == 1 || a.Status == 11 || a.Status == 111
                                    where a.Phone == phone || a.Serial == phone
                                    select new ActiveSearch()
                                    {
                                        Id = a.Id,
                                        Name = a.Name,
                                        Phone = a.Phone,
                                        Address = a.Address,
                                        County = a.County,
                                        City = a.City,
                                        Birthday = a.Birthday,
                                        CodeProduct = a.CodeProduct,
                                        Serial = a.Serial,
                                        Product = b.Name,
                                        Model = b.Model,
                                        ActiveDate = a.ActiveDate,
                                        EndDate = a.EndDate,
                                        ActiveWho = a.ActiveWho,
                                        NameAgency = b.NameAgency,
                                        Status = a.Status,
                                        Buydate = b.Buydate,
                                        Exportdate = b.ExportDate
                                    };
                    if (customers.Count() > 0)
                    {
                        var customer = customers.OrderBy(a => a.Name).FirstOrDefault();
                        ViewBag.customer = customer;
                        return View(customers);
                    }
                }
                ViewBag.message = "NOT";
            }

            return View();
        }
    }
}