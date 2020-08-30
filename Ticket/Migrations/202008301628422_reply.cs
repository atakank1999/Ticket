namespace Ticket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reply : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "Status", c => c.Int(nullable: false));
            DropColumn("dbo.Tickets", "IsSolved");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tickets", "IsSolved", c => c.Boolean(nullable: false));
            DropColumn("dbo.Tickets", "Status");
        }
    }
}
