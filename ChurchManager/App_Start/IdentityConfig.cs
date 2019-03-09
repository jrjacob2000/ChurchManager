using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using ChurchManager.Models;
using System.Net.Mail;
using System.Net;

namespace ChurchManager
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = true;
            client.EnableSsl = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.Credentials = new NetworkCredential("jrjacob2000@gmail.com", "Ilovelord");

            return client.SendMailAsync("jrjacob2000@gmail.com",//ConfigurationManager.AppSettings["SupportEmailAddr"],
                                        message.Destination,
                                        message.Subject,
                                        message.Body);
            // Plug in your email service here to send an email.
            //return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            //var manager = new ApplicationUserManager(new ApplicationUserStore(context.Get<ApplicationDbContext>()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 2,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public IQueryable<ApplicationUser> GetUsersByOwner()
        {
            using (var db = new ApplicationDbContext())
            {
                var userId = HttpContext.Current.User.Identity.GetUserId();
                var user = db.Users.Where(x => x.Id == userId).FirstOrDefault();

                var users = Users.AsNoTracking().Where(x => x.OwnerGroupId == user.OwnerGroupId);

                return (users);
            }
        }

        public bool CanPerform(string operations)
        {
            using (var db = new ApplicationDbContext())
            {
                var userId = HttpContext.Current.User.Identity.GetUserId();
                var userRoles = this.GetRoles(userId);
                var operationList = operations.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                var canperform = (from r in db.Roles
                                  join rp in db.RoleOperations on r.Id equals rp.RoleId
                                  where userRoles.Contains(r.Name) && operationList.Contains(rp.OperationId)
                                  select r).Count() > 0;

                return canperform;
            }


        }
        public static bool CanPerform(string operations, HttpContextBase httpContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return false;

            var userManager = httpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            using (var db = new ApplicationDbContext())
            {
                var userId = HttpContext.Current.User.Identity.GetUserId();
                var userRoles = userManager.GetRoles(userId);
                var operationList = operations.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                var canperform = (from r in db.Roles
                         join rp in db.RoleOperations on r.Id equals rp.RoleId
                         where userRoles.Contains(r.Name) && operationList.Contains(rp.OperationId)
                         select r).Count() > 0;

                return canperform;
            }

          
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> store) : base(store)
        {
        }
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var roleStore = new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>());
            return new ApplicationRoleManager(roleStore);
        }


        public IQueryable<ApplicationRole> GetRolesByOwner()
        {
            using (var db = new ApplicationDbContext())
            {
                var userId = HttpContext.Current.User.Identity.GetUserId();
                var user = db.Users.Where(x => x.Id == userId).FirstOrDefault();
                var roles = Roles.AsNoTracking().Where(x => x.OwnerGroupId == user.OwnerGroupId);
                return (roles);
            }
        }
    }
}
