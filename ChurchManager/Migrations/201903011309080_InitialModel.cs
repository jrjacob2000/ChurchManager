namespace ChurchManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Churches",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Address = c.String(nullable: false),
                        City = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        ZipCode = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        FbPage = c.String(),
                        OwnerGroupId = c.Guid(nullable: false),
                        DateEntered = c.DateTime(nullable: false),
                        DateLastEdited = c.DateTime(),
                        EnteredBy = c.Guid(nullable: false),
                        EditedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Families",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Address1 = c.String(nullable: false),
                        Address2 = c.String(),
                        City = c.String(nullable: false),
                        State = c.String(),
                        Zip = c.String(),
                        Country = c.String(nullable: false),
                        HomePhone = c.String(),
                        WorkPhone = c.String(),
                        CellPhone = c.String(),
                        Email = c.String(),
                        WeddingDate = c.DateTime(),
                        DateEntered = c.DateTime(nullable: false),
                        DateLastEdited = c.DateTime(),
                        EnteredBy = c.Guid(nullable: false),
                        EditedBy = c.Guid(),
                        SendNewsLetter = c.Boolean(nullable: false),
                        DateDeactivated = c.Boolean(nullable: false),
                        Latitude = c.Decimal(precision: 18, scale: 2),
                        Longitude = c.Decimal(precision: 18, scale: 2),
                        Envelope = c.Int(nullable: false),
                        OwnerGroupId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Operations",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RoleOperations",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        OperationId = c.String(nullable: false, maxLength: 128),
                        OwnerGroupId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.OperationId })
                .ForeignKey("dbo.Operations", t => t.OperationId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.OperationId);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        FirstName = c.String(nullable: false),
                        MiddleName = c.String(),
                        LastName = c.String(nullable: false),
                        Suffix = c.String(),
                        Address1 = c.String(nullable: false),
                        Address2 = c.String(),
                        City = c.String(nullable: false),
                        State = c.String(),
                        Zip = c.String(),
                        Country = c.String(nullable: false),
                        HomePhone = c.String(),
                        WorkPhone = c.String(),
                        CellPhone = c.String(),
                        Email = c.String(),
                        WorkEmail = c.String(),
                        BirthDay = c.DateTime(nullable: false),
                        MembershipDate = c.DateTime(),
                        Gender = c.String(nullable: false),
                        Envelope = c.Int(nullable: false),
                        Classification = c.String(),
                        DateLastEdited = c.DateTime(),
                        DateEntered = c.DateTime(nullable: false),
                        EnteredBy = c.Guid(nullable: false),
                        EditedBy = c.Guid(),
                        OwnerGroupId = c.Guid(nullable: false),
                        Family_ID = c.Guid(),
                        FamilyRole_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Families", t => t.Family_ID)
                .ForeignKey("dbo.ReferenceLists", t => t.FamilyRole_Id)
                .Index(t => t.Family_ID)
                .Index(t => t.FamilyRole_Id);
            
            CreateTable(
                "dbo.ReferenceLists",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                        ReferenceType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Description = c.String(),
                        OwnerGroupId = c.Guid(),
                        IsDefaultRole = c.Boolean(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        OwnerGroupId = c.Guid(nullable: false),
                        IsAccountOwner = c.Boolean(nullable: false),
                        ChangePasswordOnFirstLogin = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.RoleOperations", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.People", "FamilyRole_Id", "dbo.ReferenceLists");
            DropForeignKey("dbo.People", "Family_ID", "dbo.Families");
            DropForeignKey("dbo.RoleOperations", "OperationId", "dbo.Operations");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.People", new[] { "FamilyRole_Id" });
            DropIndex("dbo.People", new[] { "Family_ID" });
            DropIndex("dbo.RoleOperations", new[] { "OperationId" });
            DropIndex("dbo.RoleOperations", new[] { "RoleId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ReferenceLists");
            DropTable("dbo.People");
            DropTable("dbo.RoleOperations");
            DropTable("dbo.Operations");
            DropTable("dbo.Families");
            DropTable("dbo.Churches");
        }
    }
}
