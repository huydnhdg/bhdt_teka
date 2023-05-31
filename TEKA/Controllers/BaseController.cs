using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEKA.Utils;

namespace TEKA.Controllers
{
    //[Authorize(Roles = "Agency,KTV,PG,Key")]
    public class BaseController : Controller
    {
        
        protected override void ExecuteCore()
        {
            int culture = 0;

            //user changer lang
            if (this.Session == null || this.Session["CurrentCulture"] == null)
            {
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Culture"], out culture);
                this.Session["CurrentCulture"] = culture;
            }
            else
            {
                culture = (int)this.Session["CurrentCulture"];
            }
            //
            SessionManager.CurrentCulture = culture;
            base.ExecuteCore();
        }
        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }


        public ActionResult ThrowException()
        {
            throw new NotImplementedException();
        }

    }
}