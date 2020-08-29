namespace Ticket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assignments",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        IsDone = c.Boolean(nullable: false),
                        Admin_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.Admin_ID)
                .ForeignKey("dbo.Tickets", t => t.ID)
                .Index(t => t.ID)
                .Index(t => t.Admin_ID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Surname = c.String(nullable: false),
                        Email = c.String(),
                        Username = c.String(nullable: false, maxLength: 25),
                        Password = c.String(nullable: false),
                        ConfirmGuid = c.Guid(nullable: false),
                        IsConfirmed = c.Boolean(nullable: false),
                        IsAdmin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Replies",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        RepliedTicket_Id = c.Int(),
                        WriterAdmin_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Tickets", t => t.RepliedTicket_Id)
                .ForeignKey("dbo.Users", t => t.WriterAdmin_ID)
                .Index(t => t.RepliedTicket_Id)
                .Index(t => t.WriterAdmin_ID);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Text = c.String(),
                        FilePath = c.String(),
                        IsSolved = c.Boolean(nullable: false),
                        Priority = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Author_ID = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Author_ID)
                .Index(t => t.Author_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Assignments", "ID", "dbo.Tickets");
            DropForeignKey("dbo.Replies", "WriterAdmin_ID", "dbo.Users");
            DropForeignKey("dbo.Replies", "RepliedTicket_Id", "dbo.Tickets");
            DropForeignKey("dbo.Tickets", "Author_ID", "dbo.Users");
            DropForeignKey("dbo.Assignments", "Admin_ID", "dbo.Users");
            DropIndex("dbo.Tickets", new[] { "Author_ID" });
            DropIndex("dbo.Replies", new[] { "WriterAdmin_ID" });
            DropIndex("dbo.Replies", new[] { "RepliedTicket_Id" });
            DropIndex("dbo.Assignments", new[] { "Admin_ID" });
            DropIndex("dbo.Assignments", new[] { "ID" });
            DropTable("dbo.Tickets");
            DropTable("dbo.Replies");
            DropTable("dbo.Users");
            DropTable("dbo.Assignments");
        }
    }
}
