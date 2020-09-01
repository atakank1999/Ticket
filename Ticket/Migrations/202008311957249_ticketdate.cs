namespace Ticket.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ticketdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tickets", "EditedOn", c => c.DateTime(nullable: false));
        }

        public override void Down()
        {
            AlterColumn("dbo.Tickets", "EditedOn", c => c.DateTime());
        }
    }
}