namespace Ticket.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class tickets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Replies", "date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Tickets", "DateTime", c => c.DateTime(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Tickets", "DateTime");
            DropColumn("dbo.Replies", "date");
        }
    }
}