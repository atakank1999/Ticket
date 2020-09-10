using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using System.Web.WebPages;

namespace Ticket.Models.Context
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Log> Logs { get; set; }

        public DatabaseContext() : base("DatabaseContext")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DatabaseContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<DatabaseContext>(new CreateDatabaseIfNotExists<DatabaseContext>());
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            DatabaseContext db = new DatabaseContext();
            string username = string.Empty;
            var ticketList = ChangeTracker.Entries().Where((x => x.State == EntityState.Added && !(x.Entity is Log)));
            Users user = new Users();
            if (HttpContext.Current.Session != null)
            {
                username = HttpContext.Current.Session["Login"] == null ? "Anonymous" : HttpContext.Current.Session["Login"].ToString();
            }

            if (username != "Anonymous" && !username.IsEmpty())
            {
                foreach (Users u in this.Users.ToList())
                {
                    if (u.Username == username && !u.IsDeleted)
                    {
                        user = u;
                    }
                }
            }

            foreach (var entity in ticketList)
            {
                Log log = new Log();

                if (entity.Entity is Ticket)
                {
                    var t = entity.Entity as Ticket;
                    if (t.IsDeleted)
                    {
                        return base.SaveChanges();
                    }
                    log.ObjecType = typeof(Ticket).ToString();
                    log.Ticket = t;
                    log.NexTicket = t;
                    log.Users = user;
                    log.Type = entity.State.ToString();
                    log.routevalues = HttpContext.Current.Request.Url.PathAndQuery;
                    log.Time = DateTime.Now;
                    log.IP = HttpContext.Current.Request.UserHostAddress;
                }
                else if (entity.Entity is Assignment)
                {
                    var a = entity.Entity as Assignment;
                    if (a.IsDeleted)
                    {
                        return base.SaveChanges();
                    }
                    log.ObjecType = typeof(Assignment).ToString();
                    log.NextAssignment = a;
                    log.Assignment = a;
                    log.Users = user;
                    log.Type = entity.State.ToString();
                    log.routevalues = HttpContext.Current.Request.Url.PathAndQuery;
                    log.Time = DateTime.Now;
                    log.IP = HttpContext.Current.Request.UserHostAddress;
                }
                else if (entity.Entity is Reply)
                {
                    var r = entity.Entity as Reply;
                    if (r.IsDeleted)
                    {
                        return base.SaveChanges();
                    }
                    log.ObjecType = typeof(Reply).ToString();
                    log.NextReply = r;
                    log.Reply = r;
                    log.Users = user;
                    log.Type = entity.State.ToString();
                    log.routevalues = HttpContext.Current.Request.Url.PathAndQuery;
                    log.Time = DateTime.Now;
                    log.IP = HttpContext.Current.Request.UserHostAddress;
                }
                else if (entity.Entity is Users)
                {
                    var r = entity.Entity as Users;
                    if (r.IsDeleted)
                    {
                        return base.SaveChanges();
                    }
                    log.ObjecType = typeof(Users).ToString();
                    log.NextUsers = user.Username.IsEmpty() ? log.Users = r : log.Users = user;
                    log.Users = user.Username.IsEmpty() ? log.Users = r : log.Users = user;
                    log.Type = entity.State.ToString();
                    log.routevalues = HttpContext.Current.Request.Url.PathAndQuery;
                    log.Time = DateTime.Now;
                    log.IP = HttpContext.Current.Request.UserHostAddress;
                }
                this.Logs.Add(log);
            }

            return base.SaveChanges();
        }
    }
}