using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TEKA.Areas.Admin.Controllers
{
    public class ChatController : Controller
    {
        // GET: Admin/Chat
        public ActionResult Index()
        {
            return View();
        }
    }
}