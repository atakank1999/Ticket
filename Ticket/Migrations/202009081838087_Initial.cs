namespace Ticket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assignments",
                c => new
                {
                    ID = c.Int(nullable: false),
                    IsDone = c.Boolean(nullable: false),
                    Deadline = c.DateTime(nullable: false),
                    Admin_ID = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.Admin_ID, cascadeDelete: true)
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
                "dbo.Logs",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    ObjecType = c.String(),
                    routevalues = c.String(nullable: false),
                    IP = c.String(),
                    Time = c.DateTime(nullable: false),
                    Type = c.String(),
                    Assignment_ID = c.Int(),
                    Reply_ID = c.Int(),
                    Ticket_Id = c.Int(),
                    Users_ID = c.Int(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Assignments", t => t.Assignment_ID)
                .ForeignKey("dbo.Replies", t => t.Reply_ID)
                .ForeignKey("dbo.Tickets", t => t.Ticket_Id)
                .ForeignKey("dbo.Users", t => t.Users_ID)
                .Index(t => t.Assignment_ID)
                .Index(t => t.Reply_ID)
                .Index(t => t.Ticket_Id)
                .Index(t => t.Users_ID);

            CreateTable(
                "dbo.Replies",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Text = c.String(nullable: false),
                    date = c.DateTime(nullable: false),
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
                    Title = c.String(nullable: false),
                    Text = c.String(nullable: false),
                    FilePath = c.String(),
                    Priority = c.Int(nullable: false),
                    Type = c.Int(nullable: false),
                    DateTime = c.DateTime(nullable: false),
                    Status = c.Int(nullable: false),
                    EditedOn = c.DateTime(nullable: false),
                    Author_ID = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Author_ID)
                .Index(t => t.Author_ID);
        }

        public override void Down()
        {
        }
    }
}