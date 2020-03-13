namespace ChurchManager.Migrations
{
    using ChurchManager.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ChurchManager.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "ChurchManager.Models.ApplicationDbContext";
        }

        protected override void Seed(ChurchManager.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            CreateOperation(context);
            CreateRolesandUsers(context);
            CreateReferenceList(context);

        }

        private void CreateRolesandUsers(ChurchManager.Models.ApplicationDbContext context)
        {
           

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("SuperAdmin"))
            {

                // first we create Admin rool   
                var role = new ApplicationRole();
                role.Name = "SuperAdmin";
                role.Description = "SuperAdmin";
                roleManager.Create(role);

               
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new ApplicationRole();
                role.Name = "Admin";
                role.Description = "Admin";
                role.IsDefaultRole = true;
                role.Operations = context.Operations.ToList().Select(x => new RoleOperation() { RoleId = role.Id, OperationId = x.Id }).ToList();
                roleManager.Create(role);

               
            }

            // creating Creating Employee role    
            if (!roleManager.RoleExists("User"))
            {
                var role = new ApplicationRole();
                role.Name = "User";
                role.Description = "User";
                roleManager.Create(role);

            }

            //Here we create a Admin super user who will maintain the website
            var user = new ApplicationUser();
            user.UserName = "jrjacob2000@gmail.com";
            user.Email = "jrjacob2000@gmail.com";

            string userPWD = "password1";

            var chkUser = UserManager.Create(user, userPWD);

            //Add default User to Role Admin   
            if (chkUser.Succeeded)
            {
                UserManager.AddToRole(user.Id, "SuperAdmin");
                UserManager.AddToRole(user.Id, "Admin");

            }
        }

        private void CreateOperation(ChurchManager.Models.ApplicationDbContext db)
        {
            var currentOperations = db.Operations.ToList();

            #region Family Operation
            if (!currentOperations.Exists(x => x.Id == ChurchManager.Constants.Operation.Family_View))
                db.Operations.Add(new Operation()
                {
                    Id = "Family_View",
                    Description = "Can view family"
                });

            if (!currentOperations.Exists(x => x.Id == ChurchManager.Constants.Operation.Family_Edit))
                db.Operations.Add(new Operation()
                {
                    Id = "Family_Edit",
                    Description = "Can edit family"
                });

            if (!currentOperations.Exists(x => x.Id == ChurchManager.Constants.Operation.Family_Create))
                db.Operations.Add(new Operation()
                {
                    Id = "Family_Create",
                    Description = "Can create family"
                });

            if (!currentOperations.Exists(x => x.Id == ChurchManager.Constants.Operation.Family_Delete))
                db.Operations.Add(new Operation()
                {
                    Id = "Family_Delete",
                    Description = "Can delete family"
                });

            #endregion

            #region Manage User Operation
            if (!currentOperations.Exists(x => x.Id == "User_View"))
                db.Operations.Add(new Operation()
                {
                    Id = "User_View",
                    Description = "Can view user"
                });

            if (!currentOperations.Exists(x => x.Id == "User_Edit"))
                db.Operations.Add(new Operation()
                {
                    Id = "User_Edit",
                    Description = "Can edit user"
                });

            if (!currentOperations.Exists(x => x.Id == "User_Create"))
                db.Operations.Add(new Operation()
                {
                    Id = "User_Create",
                    Description = "Can create user"
                });

            if (!currentOperations.Exists(x => x.Id == "User_Delete"))
                db.Operations.Add(new Operation()
                {
                    Id = "User_Delete",
                    Description = "Can delete user"
                });

            #endregion

            #region Roles
            if (!currentOperations.Exists(x => x.Id == "Role_Create"))
                db.Operations.Add(new Operation()
                {
                    Id = "Role_Create",
                    Description = "Can create role"
                });

            if (!currentOperations.Exists(x => x.Id == "Role_Edit"))
                db.Operations.Add(new Operation()
                {
                    Id = "Role_Edit",
                    Description = "Can edit role"
                });

            if (!currentOperations.Exists(x => x.Id == "Role_View"))
                db.Operations.Add(new Operation()
                {
                    Id = "Role_View",
                    Description = "Can view role"
                });

            if (!currentOperations.Exists(x => x.Id == "Role_Delete"))
                db.Operations.Add(new Operation()
                {
                    Id = "Role_Delete",
                    Description = "Can delete role"
                });

            if (!currentOperations.Exists(x => x.Id == "Church_Edit"))
                db.Operations.Add(new Operation()
                {
                    Id = "Church_Edit",
                    Description = "Can edit church profile"
                });


            #endregion

            #region Persons Operation
            if (!currentOperations.Exists(x => x.Id == ChurchManager.Constants.Operation.Person_Create))
                db.Operations.Add(new Operation()
                {
                    Id = ChurchManager.Constants.Operation.Person_Create,
                    Description = "Can create person"
                });

            if (!currentOperations.Exists(x => x.Id == ChurchManager.Constants.Operation.Person_Delete))
                db.Operations.Add(new Operation()
                {
                    Id = ChurchManager.Constants.Operation.Person_Delete,
                    Description = "Can delete person"
                });

            if (!currentOperations.Exists(x => x.Id == ChurchManager.Constants.Operation.Person_Edit))
                db.Operations.Add(new Operation()
                {
                    Id = ChurchManager.Constants.Operation.Person_Edit,
                    Description = "Can edit person"
                });

            if (!currentOperations.Exists(x => x.Id == ChurchManager.Constants.Operation.Person_View))
                db.Operations.Add(new Operation()
                {
                    Id = ChurchManager.Constants.Operation.Person_View,
                    Description = "Can view person"
                });

            #endregion


            #region Group Operation
            if (!currentOperations.Exists(x => x.Id == ChurchManager.Constants.Operation.Group_View))
                db.Operations.Add(new Operation()
                {
                    Id = ChurchManager.Constants.Operation.Group_View,
                    Description = "Can view group"
                });

            if (!currentOperations.Exists(x => x.Id == ChurchManager.Constants.Operation.Group_Edit))
                db.Operations.Add(new Operation()
                {
                    Id = ChurchManager.Constants.Operation.Group_Edit,
                    Description = "Can edit group"
                });

            if (!currentOperations.Exists(x => x.Id == ChurchManager.Constants.Operation.Group_Create))
                db.Operations.Add(new Operation()
                {
                    Id = ChurchManager.Constants.Operation.Group_Create,
                    Description = "Can create group"
                });

            if (!currentOperations.Exists(x => x.Id == ChurchManager.Constants.Operation.Group_Delete))
                db.Operations.Add(new Operation()
                {
                    Id = ChurchManager.Constants.Operation.Group_Delete,
                    Description = "Can delete group"
                });

            #endregion

            db.SaveChanges();
        }

        private void CreateReferenceList(ApplicationDbContext db)
        {
            var currentReferenceList = db.ReferenceList.ToList();

            
            if (!currentReferenceList.Exists(x => x.Id == "FR_HH" && x.ReferenceType == ReferenceType.FamilyRole.ToString()))
                db.ReferenceList.Add(new ReferenceList()
                {
                    Id = "FR_HH",
                    Description = "Head of Household",
                    ReferenceType = ReferenceType.FamilyRole.ToString()
                });

            if (!currentReferenceList.Exists(x => x.Id == "FR_S" && x.ReferenceType == ReferenceType.FamilyRole.ToString()))
                db.ReferenceList.Add(new ReferenceList()
                {
                    Id = "FR_S",                    
                    Description = "Spouse",
                    ReferenceType = ReferenceType.FamilyRole.ToString()
                });

            if (!currentReferenceList.Exists(x => x.Id == "FR_HH" && x.ReferenceType == ReferenceType.FamilyRole.ToString()))
                db.ReferenceList.Add(new ReferenceList()
                {
                    Id = "FR_C",
                    Description = "Child",
                    ReferenceType = ReferenceType.FamilyRole.ToString()
                });

            if (!currentReferenceList.Exists(x => x.Id == "FR_OR" && x.ReferenceType == ReferenceType.FamilyRole.ToString()))
                db.ReferenceList.Add(new ReferenceList()
                {
                    Id = "FR_OR",
                    Description = "Other Relatives",
                    ReferenceType = ReferenceType.FamilyRole.ToString()
                });

            if (!currentReferenceList.Exists(x => x.Id == "FR_NR" && x.ReferenceType == ReferenceType.FamilyRole.ToString()))
                db.ReferenceList.Add(new ReferenceList()
                {
                    Id = "FR_NR",
                    Description = "Non Relatives",
                    ReferenceType = ReferenceType.FamilyRole.ToString()
                });

            if (!currentReferenceList.Exists(x => x.Id == "0" && x.ReferenceType == "AccountChartType"))
                db.ReferenceList.Add(new ReferenceList()
                {
                    Id = "0",
                    Description = AccountChartTypeEnum.Asset.ToString(),
                    ReferenceType = "AccountChartType"
                });

            if (!currentReferenceList.Exists(x => x.Id == "1" && x.ReferenceType == "AccountChartType"))
                db.ReferenceList.Add(new ReferenceList()
                {
                    Id = "1",
                    Description = AccountChartTypeEnum.Liability.ToString(),
                    ReferenceType = "AccountChartType"
                });

            if (!currentReferenceList.Exists(x => x.Id == "2" && x.ReferenceType == "AccountChartType"))
                db.ReferenceList.Add(new ReferenceList()
                {
                    Id = "2",
                    Description = AccountChartTypeEnum.Liability.ToString(),
                    ReferenceType = "AccountChartType"
                });

            if (!currentReferenceList.Exists(x => x.Id == "3" && x.ReferenceType == "AccountChartType"))
                db.ReferenceList.Add(new ReferenceList()
                {
                    Id = "3",
                    Description = AccountChartTypeEnum.Income.ToString(),
                    ReferenceType = "AccountChartType"
                });

            if (!currentReferenceList.Exists(x => x.Id == "4" && x.ReferenceType == "AccountChartType"))
                db.ReferenceList.Add(new ReferenceList()
                {
                    Id = "4",
                    Description = AccountChartTypeEnum.Expenses.ToString(),
                    ReferenceType = "AccountChartType"
                });
                       

        }
    }
}
