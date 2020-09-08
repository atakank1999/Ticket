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
            string username = string.Empty;
            var ticketList = ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged);
            Log log = new Log();
            Users user = new Users();
            if (HttpContext.Current.Session != null)
            {
                username = HttpContext.Current.Session["Login"] == null ? "Anonymous" : HttpContext.Current.Session["Login"].ToString();
            }

            if (username != "Anonymous" && !username.IsEmpty())
            {
                foreach (Users u in this.Users.ToList())
                {
                    if (u.Username == username)
                    {
                        user = u;
                    }
                }
            }

            foreach (var entity in ticketList)
            {
                if (entity.Entity is Ticket)
                {
                    var t = entity.Entity as Ticket;
                    log.ObjecType = typeof(Ticket).ToString();
                    log.Ticket = t;
                    log.Users = user;
                    log.Type = entity.State.ToString();
                    log.routevalues = HttpContext.Current.Request.Url.PathAndQuery;
                    log.Time = DateTime.Now;
                    log.IP = HttpContext.Current.Request.UserHostAddress;

                    if (entity.State == EntityState.Modified)
                    {
                        foreach (string prop in entity.OriginalValues.PropertyNames.ToList())
                        {
                            var origin = entity.OriginalValues[prop];
                            if (log.Previous == null)
                            {
                                log.Previous = new Ticket();
                            }
                            Ticket prev = log.Previous as Ticket;
                            prev.GetType().GetProperty(prop).SetValue(prev, origin);
                        }
                    }
                }
                else if (entity.Entity is Assignment)
                {
                    var a = entity.Entity as Assignment;
                    log.ObjecType = typeof(Assignment).ToString();
                    log.Assignment = a;
                    log.Users = user;
                    log.Type = entity.State.ToString();
                    log.routevalues = HttpContext.Current.Request.Url.PathAndQuery;
                    log.Time = DateTime.Now;
                    log.IP = HttpContext.Current.Request.UserHostAddress;

                    if (entity.State == EntityState.Modified)
                    {
                        foreach (string prop in entity.OriginalValues.PropertyNames.ToList())
                        {
                            var origin = entity.OriginalValues[prop];
                            if (log.Previous == null)
                            {
                                log.Previous = new Assignment();
                            }
                            var prev = log.Previous as Assignment;
                            prev.GetType().GetProperty(prop).SetValue(prev, origin);
                        }
                    }
                }
                else if (entity.Entity is Reply)
                {
                    var r = entity.Entity as Reply;
                    log.ObjecType = typeof(Reply).ToString();
                    log.Reply = r;
                    log.Users = user;
                    log.Type = entity.State.ToString();
                    log.routevalues = HttpContext.Current.Request.Url.PathAndQuery;
                    log.Time = DateTime.Now;
                    log.IP = HttpContext.Current.Request.UserHostAddress;

                    if (entity.State == EntityState.Modified)
                    {
                        foreach (string prop in entity.OriginalValues.PropertyNames.ToList())
                        {
                            var origin = entity.OriginalValues[prop];
                            if (log.Previous == null)
                            {
                                log.Previous = new Reply();
                            }
                            var prev = log.Previous as Reply;
                            prev.GetType().GetProperty(prop).SetValue(prev, origin);
                        }
                    }
                }
                else if (entity.Entity is Users)
                {
                    var r = entity.Entity as Users;
                    log.ObjecType = typeof(Users).ToString();
                    log.Users = user.Username.IsEmpty() ? log.Users = r : log.Users = user;
                    log.Type = entity.State.ToString();
                    log.routevalues = HttpContext.Current.Request.Url.PathAndQuery;
                    log.Time = DateTime.Now;
                    log.IP = HttpContext.Current.Request.UserHostAddress;
                    if (entity.State == EntityState.Modified)
                    {
                        foreach (string prop in entity.OriginalValues.PropertyNames.ToList())
                        {
                            var origin = entity.OriginalValues[prop];
                            if (log.Previous == null)
                            {
                                log.Previous = new Users();
                            }
                            var prev = log.Previous as Users;
                            prev.GetType().GetProperty(prop).SetValue(prev, origin);
                        }
                    }
                }
                this.Logs.Add(log);
            }

            return base.SaveChanges();
        }
    }
}