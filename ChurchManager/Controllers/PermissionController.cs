using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChurchManager.Models;
using Microsoft.AspNet.Identity;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace ChurchManager.Controllers
{
    [Authorize]
    public class PermissionController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

     
        public PermissionController()
        { }

        public PermissionController(ApplicationUserManager userManager, ApplicationRoleManager roleManager):base(userManager,roleManager)
        {
            
        }
        
        #region User
        [CustomAuthorize(Operations= Constants.Operation.User_View)]
        public ActionResult UserList(string successMessage, string errorMessage)
        {
            // Get Error message if exists.
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ViewBag.ErrorMessage = errorMessage;
            }

            // Get Success message if exists.
            if (!string.IsNullOrEmpty(successMessage))
            {
                ViewBag.SuccessMessage = successMessage;
            }

            var roles = RoleManager.GetRolesByOwner();
            var users = UserManager.GetUsersByOwner().ToList()
                .Select(x => new UserViewModel() {
                    ID = x.Id,
                    Email = x.Email,
                    IsAccountOwner = x.IsAccountOwner,
                    RolesList = (from ur in x.Roles
                                 join r in roles on ur.RoleId equals r.Id
                                 select new SelectListItem() { Value = r.Id, Text = r.Description}).ToList()
                });
            

            var userList = new UserListView() { Items = users };
            userList.CanAdd = UserManager.CanPerform(Constants.Operation.User_Create);
            userList.CanEdit = UserManager.CanPerform(Constants.Operation.User_Edit);
            userList.CanDelete = UserManager.CanPerform(Constants.Operation.User_Delete);

            return View(userList);
        }


        //
        // GET: /Permission/CreateUser
        [CustomAuthorize(Operations = Constants.Operation.User_Create)]
        public ActionResult CreateUser()
        {
            var model = new UserViewModel
            {
                ChangePasswordOnFirstLogin = true,
                RolesList = RoleManager.GetRolesByOwner().ToList().Select(r => new SelectListItem
                {
                    Text = r.Description,
                    Value = r.Name
                }).OrderBy(r => r.Text),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Operations = Constants.Operation.User_Create)]
        public ActionResult CreateUser(UserViewModel user)
        {
            var valid = true;
            if (user.SelectedRoles == null || user.SelectedRoles.Count() == 0)
            {
                valid = false;
                ModelState.AddModelError("", "Please select atleast 1 role");
            }

            if (ModelState.IsValid && valid)
            {
                var currentUser = UserManager.FindById(User.Identity.GetUserId());

                var userId = Guid.NewGuid();
                var userExist = UserManager.FindByName(user.UserName);
                if (userExist == null)
                {
                    var userdb = new ApplicationUser();
                    userdb.UserName = user.UserName;
                    userdb.Email = user.Email;
                    userdb.IsAccountOwner = user.IsAccountOwner;
                    userdb.OwnerGroupId = currentUser.OwnerGroupId;
                    userdb.ChangePasswordOnFirstLogin = user.ChangePasswordOnFirstLogin;

                    var chkUser = UserManager.Create(userdb, user.Password);

                    //Add default User to Role Admin 
                    var succeed = false;
                    if (chkUser.Succeeded)
                    {
                        succeed = true;
                        var updar = UserManager.AddToRoles(userdb.Id.ToString(), user.SelectedRoles.ToArray());
                        if (!updar.Succeeded)
                        {
                            ModelState.AddModelError("", string.Join(", ", updar.Errors));
                            succeed = false;
                        }
                    }
                    else
                    {
                        succeed = false;
                        ModelState.AddModelError("", string.Join(", ", chkUser.Errors));
                    }

                    if (succeed)
                        return RedirectToAction("UserList");
                }
                else
                {
                    ModelState.AddModelError("", "User with the same user name is already exist.");
                }

            }

            user.RolesList = RoleManager.GetRolesByOwner().ToList().Select(r => new SelectListItem
            {
                Selected = user.SelectedRoles != null ? user.SelectedRoles.Contains(r.Name) : false,
                Text = r.Description,
                Value = r.Name
            }).OrderBy(r => r.Text);

            return View(user);
        }

        //
        // GET: /Manage/EditUser
        [CustomAuthorize(Operations = Constants.Operation.User_Edit)]
        public ActionResult EditUser(string Id)
        {
            try
            {
                //ModelState.AddModelError("", "test error");

                var user = UserManager.FindById(Id);
                var userRoles = UserManager.GetRoles(Id);
                var roles = RoleManager.GetRolesByOwner().ToList();

                var selectedRoles = roles.Select(r => new SelectListItem
                {
                    Selected = userRoles.Contains(r.Name),
                    Text = r.Description,
                    Value = r.Name
                }).OrderBy(r => r.Text).ToList();
               
                var model = new UserViewModel
                {
                    ID = user.Id,//Guid.Parse(user.Id),
                    UserName = user.UserName,
                    Email = user.Email,
                    IsAccountOwner = user.IsAccountOwner,
                    RolesList = selectedRoles
                };
                
                return View(model);

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Operations = Constants.Operation.User_Edit)]
        public ActionResult EditUser(UserViewModel user)
        {
            var userId = user.ID.ToString();
            var valid = true;

            if (user.SelectedRoles == null || user.SelectedRoles.Count() == 0)
            {
                valid = false;
                ModelState.AddModelError("", "Please select atleast 1 role");
            }
            else
            {
                var adminRole = RoleManager.GetRolesByOwner().Where(x => x.IsDefaultRole).FirstOrDefault();
                var anyAdminLeft = UserManager.GetUsersByOwner().Where(x => x.Id != userId && x.Roles.Select(s => s.RoleId).Contains(adminRole.Id)).Count() > 0;
                var containAdmin = user.SelectedRoles.Contains(adminRole.Name);

                if (!anyAdminLeft && !containAdmin)
                {
                    valid = false;
                    ModelState.AddModelError("", "You cannot remove Admin role to this user. Please make sure you leave atleast 1 user with an admin role to manage your church account.");
                }

                if(user.IsAccountOwner && !containAdmin)
                {
                    valid = false;
                    ModelState.AddModelError("", "Account owner should be admin.");
                }
            }

            ModelState.Remove("Password");
            if (ModelState.IsValid && valid)
            {
                //dbUser.Email = user.Email;
                var userRoles = UserManager.GetRoles(userId);

                var dbUser = UserManager.FindById(userId);
                var updr = UserManager.RemoveFromRoles(userId, userRoles.ToArray());
                var updar = UserManager.AddToRoles(user.ID.ToString(), user.SelectedRoles.ToArray());

                dbUser.Email = user.Email;
                dbUser.IsAccountOwner = user.IsAccountOwner;
                var upd = UserManager.Update(dbUser);

                if (!updr.Succeeded)
                    ModelState.AddModelError("", string.Join(", ", updr.Errors));

                if (!updar.Succeeded)
                    ModelState.AddModelError("", string.Join(", ", updar.Errors));

                if (!upd.Succeeded)
                    ModelState.AddModelError("", string.Join(", ", upd.Errors));


                if (updr.Succeeded && updar.Succeeded && upd.Succeeded)
                    return RedirectToAction("UserList");
               

            }
            user.RolesList = RoleManager.GetRolesByOwner().ToList().Select(r => new SelectListItem
            {
                Selected = UserManager.GetRoles(userId).Contains(r.Name),
                Text = r.Description,
                Value = r.Name
            }).OrderBy(r => r.Text);
            return View(user);
        }

        [CustomAuthorize(Operations = Constants.Operation.User_Delete)]
        public ActionResult DeleteUser(string Id)
        {
            var user = UserManager.FindById(Id);

            ModalDelete model = new ModalDelete();
            model.Action = "DeleteUser";
            model.Controller = "Permission";
            model.Id = Id.ToString();
            model.Name = user.UserName;

            return PartialView("_ModalDelete", model);
        }

        [CustomAuthorize(Operations = Constants.Operation.User_Delete)]
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDeleteUser(string Id)
        {
            var message = string.Empty;
            var error = string.Empty;
            var user = UserManager.FindById(Id);

            var users = UserManager.GetUsersByOwner().Where(x => x.IsAccountOwner);

            if (user.IsAccountOwner && users.Count() < 2)
            {
                error = "You're not allowed to delete the account owner.If you wish to delete this user please promote another user to be the owner.";
                ModelState.AddModelError("", "You're not allowed to delete the account owner.If you wish to delete this user please promote another user to be the owner.");
            }
            else if (UserManager.GetUsersByOwner().Count() == 1)
            {
                error = "You cannot delete this user, you need atleast 1 user remain.";
                ModelState.AddModelError("", "You cannot delete this user, you need atleast 1 user remain.");
            }
            else
            {
                UserManager.Delete(user);
                message = "Successfully deleted user " + user.UserName;

                if (Id == User.Identity.GetUserId())
                {
                    HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("Login", "Account");
                }
            }

           

            var userList = new UserListView(){ Items= UserManager.GetUsersByOwner().ProjectTo<UserViewModel>().ToList() };

            //return View("UserList", userList);
            return RedirectToAction("UserList", new { successMessage = message, errorMessage = error });

        }


        #endregion

        #region Role
        [CustomAuthorize(Operations = Constants.Operation.Role_View)]
        public ActionResult RoleList(string successMessage, string errorMessage)
        {
            // Get Error message if exists.
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ViewBag.ErrorMessage = errorMessage;
            }

            // Get Success message if exists.
            if (!string.IsNullOrEmpty(successMessage))
            {
                ViewBag.SuccessMessage = successMessage;
            }

            var roles = RoleManager.GetRolesByOwner().ToList()
                .Select(x =>
                new ApplicationRoleView()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        IsDefaultRole = x.IsDefaultRole,
                        OperationList = (from  ro in x.Operations 
                                        join o in db.Operations.ToList() on ro.OperationId equals o.Id
                                        select new SelectListItem() {Value= o.Id, Text= o.Description}).ToList()                        
                    }
                )
                .ToList();

            var rolelistModel = new ApplicationRoleListView() { Items = roles };
            rolelistModel.CanAdd = UserManager.CanPerform(Constants.Operation.Role_Create);
            rolelistModel.CanEdit = UserManager.CanPerform(Constants.Operation.Role_Edit);
            rolelistModel.CanDelete = UserManager.CanPerform(Constants.Operation.Role_Delete);

            return View(rolelistModel);
        }

        [CustomAuthorize(Operations = Constants.Operation.Role_Create)]
        public ActionResult CreateRole()
        {             
            var operations = db.Operations.ToList();
            
            var model = new ApplicationRoleView
            {
                OperationList = operations.Select(s => new SelectListItem
                {
                    Text = s.Description,
                    Value = s.Id
                }).OrderBy(s => s.Text),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Operations = Constants.Operation.Role_Create)]
        public ActionResult CreateRole(ApplicationRoleView roleViewModel)
        {
            var operations = db.Operations.ToList();
            var user = UserManager.FindById(User.Identity.GetUserId());
            var roleName = string.Format("{0}.{1}", user.OwnerGroupId, roleViewModel.Description);

            if (RoleManager.RoleExists(roleName))
            {
                ModelState.AddModelError("", "Role with the same name is already exists.");          
            }

            //Validate Operation is required
            if (roleViewModel.SelectedOperation == null || roleViewModel.SelectedOperation.Count() == 0)
            {
                ModelState.AddModelError("", "Please select atleast 1 operation");                
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var roleId = Guid.NewGuid().ToString();                   

                    var newRole = new ApplicationRole()
                    {
                        Id = roleId,
                        Name = string.Format("{0}.{1}", user.OwnerGroupId, roleViewModel.Description),
                        Description = roleViewModel.Description,
                        OwnerGroupId = user.OwnerGroupId
                    };

                    RoleManager.Create(newRole);

                    var oldOperation = db.RoleOperations.Where(x => x.RoleId == roleId).ToList();
                    db.RoleOperations.RemoveRange(oldOperation);

                    var newOperation = roleViewModel.SelectedOperation.Select(x =>
                        new RoleOperation()
                        {
                            RoleId = roleId,
                            OperationId = x
                        }
                    );

                    db.RoleOperations.AddRange(newOperation);
                    db.SaveChanges();
                    return RedirectToAction("RoleList");
                }
        
                
            }
            catch (Exception ex)
            {
   
                ModelState.AddModelError("", ex.Message);
            }

            //default display in view
            var model = new ApplicationRoleView
            {
                Name = roleViewModel.Name,
                OperationList = operations.Select(s => new SelectListItem
                {
                    Selected = roleViewModel.SelectedOperation == null ? false : roleViewModel.SelectedOperation.Select(x => x).Contains(s.Id),
                    Text = s.Description,
                    Value = s.Id
                }).OrderBy(s => s.Text),
            };

            return View(model);
        }

        [CustomAuthorize(Operations = Constants.Operation.Role_Edit)]
        public ActionResult EditRolePermission(string Id)
        {
            var roleOperation = db.RoleOperations.Where(x => x.RoleId == Id).ToList();
            var role = RoleManager.FindById(Id);
            var operations = db.Operations.ToList();

            var model = new ApplicationRoleView
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                OperationList = operations.Select(s => new SelectListItem
                {
                    Selected = roleOperation.Select(x => x.OperationId).Contains(s.Id),
                    Text = s.Description,
                    Value = s.Id
                }).OrderBy(s => s.Text),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Operations = Constants.Operation.Role_Edit)]
        public ActionResult EditRolePermission(ApplicationRoleView roleViewModel)
        {
            var operations = db.Operations.ToList();
            var role = RoleManager.FindById(roleViewModel.Id);

            
            //Validate Operation is required
            if (roleViewModel.SelectedOperation == null || roleViewModel.SelectedOperation.Count() == 0)
            {
                ModelState.AddModelError("","Please select atleast 1 operation");
                //return View(model);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var user = UserManager.FindById(User.Identity.GetUserId());

                    role.Name = string.Format("{0}.{1}", user.OwnerGroupId, roleViewModel.Description);
                    role.Description = roleViewModel.Description;
                    RoleManager.Update(role);
                    var oldOperation = db.RoleOperations.Where(x => x.RoleId == role.Id).ToList();
                    db.RoleOperations.RemoveRange(oldOperation);

                    var newOperation = roleViewModel.SelectedOperation.Select(x =>
                        new RoleOperation()
                        {
                            RoleId = role.Id,
                            OperationId = x
                        }
                    );

                    db.RoleOperations.AddRange(newOperation);
                    db.SaveChanges();
                    return RedirectToAction("RoleList");
                }
           }
            catch (Exception ex)
            {
                throw ex;
            }

            var model = new ApplicationRoleView
            {
                Id = roleViewModel.Id,
                Name = roleViewModel.Name,
                Description = roleViewModel.Description,
                OperationList = operations.Select(s => new SelectListItem
                {
                    Selected = roleViewModel.SelectedOperation == null ? false : roleViewModel.SelectedOperation.Select(x => x).Contains(s.Id),
                    Text = s.Description,
                    Value = s.Id
                }).OrderBy(s => s.Text),
            };


            return View(model);
        }

        [CustomAuthorize(Operations = Constants.Operation.Role_Delete)]
        public ActionResult DeleteRole(string Id)
        {
            var role = RoleManager.FindById(Id);
           
            ModalDelete model = new ModalDelete();
            model.Action = "DeleteRole";
            model.Controller = "Permission";
            model.Id = Id.ToString();
            model.Name = role.Description;

            return PartialView("_ModalDelete", model);
        }

        [CustomAuthorize(Operations = Constants.Operation.Role_Delete)]
        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDeleteRole(string Id)
        {
            var role = RoleManager.FindById(Id);
            var error = string.Empty;
            var message = string.Empty;

            if (role.IsDefaultRole)
            {
                error = "You're not allowed to delete the default admin role";
                ModelState.AddModelError("", "You're not allowed to delete the default admin role");
            }
            else
            {
                RoleManager.Delete(role);
                message = "Successfully delete role " + role.Description;
                //return RedirectToAction("RoleList");
            }
            
            return RedirectToAction("RoleList", new { successMessage = message, errorMessage = error });
        }


        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
