namespace ChurchManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTransactions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionLines",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TransactionId = c.Guid(nullable: false),
                        AccountId = c.Guid(nullable: false),
                        FundId = c.Guid(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountCharts", t => t.AccountId)
                .ForeignKey("dbo.Transactions", t => t.TransactionId, cascadeDelete: true)
                .Index(t => t.TransactionId)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                        Payee = c.String(),
                        Comment = c.String(),
                        AccountRegisterId = c.Guid(nullable: false),
                        DateLastEdited = c.DateTime(),
                        DateEntered = c.DateTime(nullable: false),
                        EnteredBy = c.Guid(nullable: false),
                        EditedBy = c.Guid(),
                        OwnerGroupId = c.Guid(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountCharts", t => t.AccountRegisterId, cascadeDelete: true)
                .Index(t => t.AccountRegisterId);
            
            AddColumn("dbo.AccountCharts", "ShowInRegister", c => c.Boolean(nullable: false));
            DropColumn("dbo.AccountCharts", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccountCharts", "IsActive", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.Transactions", "AccountRegisterId", "dbo.AccountCharts");
            DropForeignKey("dbo.TransactionLines", "TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.TransactionLines", "AccountId", "dbo.AccountCharts");
            DropIndex("dbo.Transactions", new[] { "AccountRegisterId" });
            DropIndex("dbo.TransactionLines", new[] { "AccountId" });
            DropIndex("dbo.TransactionLines", new[] { "TransactionId" });
            DropColumn("dbo.AccountCharts", "ShowInRegister");
            DropTable("dbo.Transactions");
            DropTable("dbo.TransactionLines");
        }
    }
}
