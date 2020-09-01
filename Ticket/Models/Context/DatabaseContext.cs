using System.Data.Entity;

namespace Ticket.Models.Context
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        public DatabaseContext() : base("DatabaseContext")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DatabaseContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<DatabaseContext>(new CreateDatabaseIfNotExists<DatabaseContext>());
            base.OnModelCreating(modelBuilder);
        }
    }
}