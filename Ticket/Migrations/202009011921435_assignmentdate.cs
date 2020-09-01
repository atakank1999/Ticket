namespace Ticket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assignmentdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Assignments", "Deadline", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Assignments", "Deadline", c => c.DateTime());
        }
    }
}
