using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using usermanagement.core.Configuration;
using usermanagement.core.Models;

namespace usermanagement.core
{
    public class UserManagementDbContext : DbContext
    {
        public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }

        public DbSet<UserRole> UserRole { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Call the Configure method from ModelConfigurations class
            UserConfigurations.Configure(modelBuilder);
        }

    }
}
