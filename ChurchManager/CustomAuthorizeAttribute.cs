using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using ChurchManager.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;


namespace ChurchManager
{
    public class CustomAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public string Operations = null;

        //public new string[] Roles { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("HttpContext");
            }
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }
            //if (Roles == null)
            //{
            //    return true;
            //}
            //if (Roles.Length == 0)
            //{
            //    return true;
            //}

            var userManager = httpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var currentUser = userManager.FindByName(httpContext.User.Identity.Name);
            var currenUserRole = userManager.GetRoles(currentUser.Id);
            var permittedRoles = Roles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            if (permittedRoles.Any(x => currenUserRole.Contains(x)))
            {
                return true;
            }
            return false;
        }

        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            ////string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            ////string actionName = filterContext.ActionDescriptor.ActionName;

            ////implement to retrieve role base on Activity

            //string roles = "User";//GetRoles.GetActionRoles(actionName, controllerName);
            //if (!string.IsNullOrWhiteSpace(roles))
            //{
            //    this.Roles = roles;//roles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //}

            if (!string.IsNullOrEmpty(Operations))
            {
                ApplicationDbContext db = new ApplicationDbContext();

                var operationList = this.Operations.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                var roles = (from rp in db.RoleOperations
                             join r in db.Roles on rp.RoleId equals r.Id
                             where operationList.Contains(rp.OperationId)
                             select r.Name).ToList();

                Roles = string.Join(",", roles);
            }
            //Roles = Roles + (string.IsNullOrEmpty(Roles)?"":",") + "SuperAdmin";
            base.OnAuthorization(filterContext);
        }

        
 


    }
}