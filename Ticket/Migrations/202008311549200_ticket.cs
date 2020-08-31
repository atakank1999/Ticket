namespace Ticket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticket : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tickets", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.Tickets", "Text", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tickets", "Text", c => c.String());
            AlterColumn("dbo.Tickets", "Title", c => c.String());
        }
    }
}
