namespace ChurchManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAccountChart : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountCharts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DateEntered = c.DateTime(nullable: false),
                        EnteredBy = c.Guid(nullable: false),
                        DateLastEdited = c.DateTime(),
                        EditedBy = c.Guid(),
                        OwnerGroupId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AccountCharts");
        }
    }
}
