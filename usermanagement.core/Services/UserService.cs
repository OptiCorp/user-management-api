﻿using usermanagement.core.Models;
using usermanagement.core.Models.DTO;
using Microsoft.EntityFrameworkCore;
using usermanagement.core.Utilities;
using usermanagement.core;
using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace usermanagement.core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManagementDbContext _context;
        private readonly IUserUtilities _userUtilities;

        public UserService(UserManagementDbContext context, IUserUtilities userUtilities)
        {
            _context = context;
            _userUtilities = userUtilities;
        }

        public async Task<string> GetInspectorRoleIdAsync()
        {
            var inspectorRole = await _context.UserRole
                                        .FirstOrDefaultAsync(role => role.Name == "Inspector");
            return inspectorRole?.Id;
        }
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.User
                            .Include(u => u.UserRole)
                            .Where(s => s.Status == UserStatus.Active)
                            .Select(u => _userUtilities.UserToDto(u))
                            .ToListAsync();
        }
        public async Task<IEnumerable<UserDto>> GetAllUsersAdminAsync()
        {
            return await _context.User
                            .Include(u => u.UserRole)
                            .Select(u => _userUtilities.UserToDto(u))
                            .ToListAsync();
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            var user = await _context.User
                            .Include(u => u.UserRole)
                            .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            return _userUtilities.UserToDto(user);
        }

        public async Task<User> GetUserByAzureAdUserIdAsync(string azureAdUserId)
        {
            return await _context.User
                            .Include(u => u.UserRole)
                            .FirstOrDefaultAsync(u => u.AzureAdUserId == azureAdUserId);
        }
        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _context.User
                            .Include(u => u.UserRole)
                            .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return null;

            return _userUtilities.UserToDto(user);
        }

        public async Task<string> CreateUserAsync(UserCreateDto userDto)
        {
            var user = new User
            {
                AzureAdUserId = userDto.AzureAdUserId,
                Username = userDto.Username,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                UserRoleId = userDto.UserRoleId,
                CreatedDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time")),
                Status = UserStatus.Active
            };
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            await UserCreated(user);

            return user.Id;
        }
        public async Task UpdateUserAsync(UserUpdateDto updatedUserDto)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == updatedUserDto.Id);
            if (user != null)
            {
                if (updatedUserDto.Username != null)
                    user.Username = updatedUserDto.Username;
                if (updatedUserDto.FirstName != null)
                    user.FirstName = updatedUserDto.FirstName;
                if (updatedUserDto.LastName != null)
                    user.LastName = updatedUserDto.LastName;
                if (updatedUserDto.Email != null)
                    user.Email = updatedUserDto.Email;
                if (updatedUserDto.UserRoleId != null)
                    user.UserRoleId = updatedUserDto.UserRoleId;
                if (updatedUserDto.Status != null)
                {
                    string status = updatedUserDto.Status.ToLower();
                    switch (status)
                    {
                        case "active":
                            user.Status = UserStatus.Active;
                            break;
                        case "disabled":
                            user.Status = UserStatus.Disabled;
                            break;
                        case "deleted":
                            user.Status = UserStatus.Deleted;
                            break;
                        default:
                            break;
                    }
                }
                user.UpdatedDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
                await _context.SaveChangesAsync();
                await UserUpdated(user);
            }
        }
        public async Task DeleteUserAsync(string id)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                user.Status = UserStatus.Deleted;
                await _context.SaveChangesAsync();
                await UserSoftDeleted(user);
            }
        }
        public async Task HardDeleteUserAsync(string id)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                _context.User.Remove(user);
                await _context.SaveChangesAsync();
                await UserHardDeleted(user);
            }
        }

        public async Task UserCreated(User user)
        {
            var connectionString = "Endpoint=sb://servicebus-turbinsikker-prod.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=jsxc2wM5vV4rhtevLn921gUZCcs7eLEsg+ASbHwJEng=";
            var sbClient = new ServiceBusClient(connectionString);

            UserBusDto userCreatedBusDto = new UserBusDto
            {
                Id = user.Id,
                AzureAdUserId = user.AzureAdUserId,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserRole = user.UserRole.Id,
                CreatedDate = user.CreatedDate
            };

            var sender = sbClient.CreateSender("user-created");
            var body = JsonSerializer.Serialize(userCreatedBusDto);
            var sbMessage = new ServiceBusMessage(body);
            await sender.SendMessageAsync(sbMessage);
        }

        public async Task UserUpdated(User user)
        {
            var connectionString = "Endpoint=sb://servicebus-turbinsikker-prod.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=jsxc2wM5vV4rhtevLn921gUZCcs7eLEsg+ASbHwJEng=";
            var sbClient = new ServiceBusClient(connectionString);

            UserBusDto userUpdatedBusDto = new UserBusDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Email = user.Email,
                UserRole = user.UserRole.Name,
                Status = user.Status.ToString(),
                UpdatedDate = user.UpdatedDate
            };

            var sender = sbClient.CreateSender("user-updated");
            var body = JsonSerializer.Serialize(userUpdatedBusDto);
            var sbMessage = new ServiceBusMessage(body);
            await sender.SendMessageAsync(sbMessage);
        }

        public async Task UserSoftDeleted(User user)
        {
            var connectionString = "Endpoint=sb://servicebus-turbinsikker-prod.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=jsxc2wM5vV4rhtevLn921gUZCcs7eLEsg+ASbHwJEng=";
            var sbClient = new ServiceBusClient(connectionString);

            UserBusDto userSoftDeleteBusDto = new UserBusDto
            {
                Id = user.Id,
            };

            var sender = sbClient.CreateSender("user-soft-deleted");
            var body = JsonSerializer.Serialize(userSoftDeleteBusDto);
            var sbMessage = new ServiceBusMessage(body);
            await sender.SendMessageAsync(sbMessage);
        }

        public async Task UserHardDeleted(User user)
        {
            var connectionString = "Endpoint=sb://servicebus-turbinsikker-prod.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=jsxc2wM5vV4rhtevLn921gUZCcs7eLEsg+ASbHwJEng=";
            var sbClient = new ServiceBusClient(connectionString);

            UserBusDto userHardDeleteBusDto = new UserBusDto
            {
                Id = user.Id,
            };

            var sender = sbClient.CreateSender("user-hard-deleted");
            var body = JsonSerializer.Serialize(userHardDeleteBusDto);
            var sbMessage = new ServiceBusMessage(body);
            await sender.SendMessageAsync(sbMessage);
        }
    }
}
