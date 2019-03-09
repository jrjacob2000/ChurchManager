namespace ChurchManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ManyToManyGroupPerson : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonGroups",
                c => new
                    {
                        Person_Id = c.Guid(nullable: false),
                        Group_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Person_Id, t.Group_Id })
                .ForeignKey("dbo.People", t => t.Person_Id, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.Group_Id, cascadeDelete: true)
                .Index(t => t.Person_Id)
                .Index(t => t.Group_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonGroups", "Group_Id", "dbo.Groups");
            DropForeignKey("dbo.PersonGroups", "Person_Id", "dbo.People");
            DropIndex("dbo.PersonGroups", new[] { "Group_Id" });
            DropIndex("dbo.PersonGroups", new[] { "Person_Id" });
            DropTable("dbo.PersonGroups");
        }
    }
}
