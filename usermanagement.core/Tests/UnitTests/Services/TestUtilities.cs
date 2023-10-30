using Microsoft.EntityFrameworkCore;
using usermanagement.core;
using usermanagement.core.Models;

namespace usermanagement.core.Tests.Services
{
    public class TestUtilities
    {
        public async Task<UserManagementDbContext> GetDbContext(string testType)
        {
            var options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var databaseContext = new UserManagementDbContext(options);
            databaseContext.Database.EnsureCreated();

            if (testType == "UserRole")
            {
                if (await databaseContext.UserRole.CountAsync() <= 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        await databaseContext.UserRole.AddAsync(
                            new UserRole
                            {
                                Id = string.Format("UserRole {0}", i),
                                Name = string.Format("UserRole {0}", i)
                            }
                        );
                    }
                    await databaseContext.User.AddAsync(
                        new User
                        {
                            Id = "User 1",
                            AzureAdUserId = "Some email",
                            UserRoleId = "UserRole 1",
                            FirstName = "name",
                            LastName = "nameson",
                            Email = "some email",
                            Username = "username1",
                            Status = UserStatus.Active,
                            CreatedDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"))
                        }
                    );
                    await databaseContext.SaveChangesAsync();
                }
                return databaseContext;
            }

            await databaseContext.UserRole.AddRangeAsync(
                new UserRole
                {
                    Id = "Inspector",
                    Name = "Inspector"
                },
                new UserRole
                {
                    Id = "Leader",
                    Name = "Leader"
                }
            );

            if (testType == "User")
            {
                if (await databaseContext.User.CountAsync() <= 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        await databaseContext.User.AddAsync(
                            new User
                            {
                                Id = string.Format("User {0}", i),
                                AzureAdUserId = string.Format("AzureAD{0}@bouvet.no", i),
                                UserRoleId = i % 2 == 0 ? "Inspector" : "Leader",
                                FirstName = "name",
                                LastName = "nameson",
                                Email = "some email",
                                Username = string.Format("Username {0}", i),
                                Status = i % 5 == 0 ? UserStatus.Deleted : UserStatus.Active,
                                CreatedDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"))
                            }
                        );
                    }
                    await databaseContext.SaveChangesAsync();
                }
                return databaseContext;
            }

            await databaseContext.User.AddRangeAsync(
                new User
                {
                    Id = "User 1",
                    AzureAdUserId = "Some email",
                    UserRoleId = "Inspector",
                    FirstName = "name",
                    LastName = "nameson",
                    Email = "some email",
                    Username = "username1",
                    Status = UserStatus.Active,
                    CreatedDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"))
                },
                new User
                {
                    Id = "User 2",
                    AzureAdUserId = "Some email",
                    UserRoleId = "Leader",
                    FirstName = "name",
                    LastName = "nameson",
                    Email = "some email",
                    Username = "username2",
                    Status = UserStatus.Active,
                    CreatedDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"))
                },
                new User
                {
                    Id = "User 3",
                    AzureAdUserId = "Some email",
                    UserRoleId = "Inspector",
                    FirstName = "name",
                    LastName = "nameson",
                    Email = "some email",
                    Username = "username3",
                    Status = UserStatus.Active,
                    CreatedDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"))
                }
            );

            await databaseContext.SaveChangesAsync();

            return databaseContext;
        }
    }
}
