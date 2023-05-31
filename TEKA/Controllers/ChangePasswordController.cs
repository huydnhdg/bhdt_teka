using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NLog;
using TEKA.Areas.Admin;

namespace TEKA.Controllers
{
    [Authorize]
    public class ChangePasswordController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        // GET: ChangePassword
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result =  UserManager.ChangePassword(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user =  UserManager.FindById(User.Identity.GetUserId());
                if (user != null)
                {
                     SignInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
                }
                logger.Info(String.Format("ChangePassword-result-{0}", result));
                return RedirectToAction("Index", "Home");
            }
            AddErrors(result);
            logger.Info(String.Format("ChangePassword-result-{0}", result));
            return View(model);
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
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
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }
    }
}