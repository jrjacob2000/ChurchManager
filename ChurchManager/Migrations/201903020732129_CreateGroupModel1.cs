namespace ChurchManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateGroupModel1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Groups", "DateLastEdited", c => c.DateTime());
            AlterColumn("dbo.Groups", "EditedBy", c => c.Guid());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Groups", "EditedBy", c => c.Guid(nullable: false));
            AlterColumn("dbo.Groups", "DateLastEdited", c => c.DateTime(nullable: false));
        }
    }
}
