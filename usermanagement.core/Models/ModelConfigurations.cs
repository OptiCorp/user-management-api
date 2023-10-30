using Microsoft.EntityFrameworkCore;
using usermanagement.core.Models;

namespace usermanagement.core.Configuration
{
    public static class UserConfigurations
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserRole)
                .WithMany()
                .HasForeignKey(u => u.UserRoleId);

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => ur.Id);


        }
    }
}
