using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace ChurchManager.Controllers
{
    public class BaseController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

    
        public BaseController()
        {
        }

        public BaseController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = _roleManager;
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

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return this._roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set { this._roleManager = value; }
        }

        public Models.ApplicationUser Operator()
        {
            if (string.IsNullOrEmpty(User.Identity.GetUserId()))
                throw new Exception("Please re-login");
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user == null)
                throw new Exception("unable find current user");
            return user;
        }
    }
}
