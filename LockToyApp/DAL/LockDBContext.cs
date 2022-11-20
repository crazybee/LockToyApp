
using LockToyApp.DBEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ToyContracts;

namespace LockToyApp.DAL
{
    public class LockDBContext : DbContext
    {
        public ConnectionStrings ConnectionStrings { get; }

        public LockDBContext(IOptionsSnapshot<ConnectionStrings> connectionStrings)
        {
            this.ConnectionStrings = connectionStrings.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server with connection string from app settings
            options.UseSqlServer(this.ConnectionStrings.SqlConnectioniString);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Door> Doors { get; set; }

        public DbSet<UserRegistration> UserRegistrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Door>().ToTable("Doors");
        }

    }
}
