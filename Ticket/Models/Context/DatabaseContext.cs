using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ticket.Models.Context
{
    public class DatabaseContext:DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        public DatabaseContext():base("DatabaseContext")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DbContext>());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<DatabaseContext>(new DropCreateDatabaseIfModelChanges<DatabaseContext>());
            base.OnModelCreating(modelBuilder);
        }


    }
}