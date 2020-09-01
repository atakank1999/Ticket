namespace Ticket.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Assignment3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Assignments", "Deadline", c => c.DateTime());
        }

        public override void Down()
        {
            AlterColumn("dbo.Assignments", "Deadline", c => c.DateTime(nullable: false));
        }
    }
}