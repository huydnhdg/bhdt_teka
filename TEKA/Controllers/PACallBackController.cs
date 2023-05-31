using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEKA.Hubs;
using static TEKA.Utils.PACallAPI;
using NLog;
using Microsoft.AspNet.SignalR;

namespace TEKA.Controllers
{
    public class PACallBackController : Controller
    {
        TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
        static Logger logger = LogManager.GetCurrentClassLogger();
        [HttpPost]
        public void Index()
        {
            var destination = Request["destination"];
            var source = Request["source"];
            logger.Info("destination " + destination);
            logger.Info("source " + source);

            var customer = db.Customers.Where(a => a.Phone == destination || a.Phone == source).FirstOrDefault(a => a.Name != null);
            var warranti = db.Warrantis.Where(a => a.Phone == destination || a.Phone == source || a.PhoneWarr == destination || a.PhoneWarr == source).FirstOrDefault(a => a.Name != null);
            string name = "Khách mới";
            if (customer != null)
            {
                name = customer.Name;
            }
            if (warranti != null)
            {
                name = warranti.Name;
            }
            logger.Info("name " + name);
            //var model = new PAModel()
            //{
            //    source = source,
            //    destination = destination,
            //    cusname = name
            //};
            var context = GlobalHost.ConnectionManager.GetHubContext<ClientCall>();
            context.Clients.All.message(source, destination, name);
            //
        }
    }
}