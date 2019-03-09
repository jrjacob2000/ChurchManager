namespace ChurchManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateGroupModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        DateEntered = c.DateTime(nullable: false),
                        EnteredBy = c.Guid(nullable: false),
                        DateLastEdited = c.DateTime(nullable: false),
                        EditedBy = c.Guid(nullable: false),
                        OwnerGroupId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Groups");
        }
    }
}
