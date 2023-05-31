using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEKA.Models;

namespace TEKA.Controllers
{
    [RoutePrefix("bai-viet")]
    public class ActicleController : BaseController
    {
        TEKA.Models.TEKAEntities db = new TEKAEntities();
        [Route("{rewrite}")]
        // GET: Acticle
        public ActionResult Index(String rewrite)
        {
            Article article = db.Articles.Where(n => n.Url == rewrite).SingleOrDefault();
            return View(article);
        }
    }
}