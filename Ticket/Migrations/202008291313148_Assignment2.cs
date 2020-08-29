namespace Ticket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Assignment2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Assignments", "Admin_ID", "dbo.Users");
            DropIndex("dbo.Assignments", new[] { "Admin_ID" });
            AlterColumn("dbo.Assignments", "Admin_ID", c => c.Int(nullable: false));
            CreateIndex("dbo.Assignments", "Admin_ID");
            AddForeignKey("dbo.Assignments", "Admin_ID", "dbo.Users", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Assignments", "Admin_ID", "dbo.Users");
            DropIndex("dbo.Assignments", new[] { "Admin_ID" });
            AlterColumn("dbo.Assignments", "Admin_ID", c => c.Int());
            CreateIndex("dbo.Assignments", "Admin_ID");
            AddForeignKey("dbo.Assignments", "Admin_ID", "dbo.Users", "ID");
        }
    }
}
