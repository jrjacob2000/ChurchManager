using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {

            //Make sure to call the base method first:
            base.OnModelCreating(modelBuilder);

            //remove the cascade delete for accountChart -> transactionLines
            modelBuilder.Entity<ChurchManager.Models.AccountChart>().HasMany(t => t.TransactionLines).WithRequired().WillCascadeOnDelete(false);

            //modelBuilder.Entity<ApplicationRole>().HasMany(role => role.Operations)
            //    .WithMany(operation => operation.RoleId)
            //    .Map(mc =>
            //    {
            //        mc.ToTable("RoleOperations");
            //        mc.MapLeftKey("RoleId");
            //        mc.MapRightKey("OperationId");

            //    }
            //);



            //modelBuilder.Entity<ApplicationRole>().HasMany<RoleOperation>((ApplicationRole u) => u.RoleOperation);
            //modelBuilder.Entity<Operation>().HasMany<RoleOperation>((Operation u) => u.Roles);

            //modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
            //modelBuilder.Entity<ApplicationRole>().HasKey<string>(r => r.Id).ToTable("AspNetRoles");
            //modelBuilder.Entity<ApplicationUser>().HasMany<ApplicationUserRole>((ApplicationUser u) => u.UserRoles);
            //modelBuilder.Entity<ApplicationUserRole>().HasKey(r => new { UserId = r.UserId, RoleId = r.RoleId }).ToTable("AspNetUserRoles");
        }
        public System.Data.Entity.DbSet<ChurchManager.Models.Family> Families { get; set; }

        public System.Data.Entity.DbSet<ChurchManager.Models.Operation> Operations { get; set; }
        public System.Data.Entity.DbSet<ChurchManager.Models.RoleOperation> RoleOperations { get; set; }

        public System.Data.Entity.DbSet<ChurchManager.Models.ReferenceList> ReferenceList { get; set; }               

        public System.Data.Entity.DbSet<ChurchManager.Models.Church> Churches { get; set; }

        public System.Data.Entity.DbSet<ChurchManager.Models.Person> People { get; set; }

        public System.Data.Entity.DbSet<ChurchManager.Models.Group> Groups { get; set; }

        public System.Data.Entity.DbSet<ChurchManager.Models.AccountChart> AccountCharts { get; set; }

        public System.Data.Entity.DbSet<ChurchManager.Models.Transaction> Transactions { get; set; }

        public System.Data.Entity.DbSet<ChurchManager.Models.TransactionLine> TransactionLines { get; set; }
    }
}