using usermanagement.core.Models;
using usermanagement.core.Models.DTO;
using Microsoft.EntityFrameworkCore;
using usermanagement.core.Utilities;
using usermanagement.core;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using Microsoft.Graph;
using Azure.Identity;

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

        public async Task<string> CreateUserAsync(UserCreateDto userDto, string id)
        {
            var user = new User
            {
                AzureAdUserId = id,
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
                if (updatedUserDto.AzureAdUserId != null)
                    user.AzureAdUserId = updatedUserDto.AzureAdUserId;
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
            }
        }
        public async Task DeleteUserAsync(string id)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                user.Status = UserStatus.Deleted;
                await _context.SaveChangesAsync();
            }
        }
        public async Task HardDeleteUserAsync(string id)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                _context.User.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UserCreated(string id)
        {
            var connectionString = Environment.GetEnvironmentVariable("serviceBusConnectionString");
            var sbClient = new ServiceBusClient(connectionString);

            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);

            UserBusCreateDto userCreatedBusDto = new UserBusCreateDto
            {
                Id = user.Id,
                AzureAdUserId = user.AzureAdUserId,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserRole = user.UserRole.Name,
                CreatedDate = user.CreatedDate,
                Status = user.Status.ToString()
            };

            var sender = sbClient.CreateSender("user-created");
            var body = JsonSerializer.Serialize(userCreatedBusDto);
            var sbMessage = new ServiceBusMessage(body);
            await sender.SendMessageAsync(sbMessage);
        }

        public async Task UserUpdated(string id)
        {
            var connectionString = Environment.GetEnvironmentVariable("serviceBusConnectionString");
            var sbClient = new ServiceBusClient(connectionString);

            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);

            UserBusUpdateDto userUpdatedBusDto = new UserBusUpdateDto
            {
                Id = user.Id,
                AzureAdUserId = user.AzureAdUserId,
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

        public async Task UserDeleted(string id, DeleteMode mode)
        {
            var connectionString = Environment.GetEnvironmentVariable("serviceBusConnectionString");
            var sbClient = new ServiceBusClient(connectionString);

            UserBusDeleteDto userSoftDeleteBusDto = new UserBusDeleteDto
            {
                Id = id,
                DeleteMode = mode.ToString()
            };

            var sender = sbClient.CreateSender("user-deleted");
            var body = JsonSerializer.Serialize(userSoftDeleteBusDto);
            var sbMessage = new ServiceBusMessage(body);
            await sender.SendMessageAsync(sbMessage);
        }

        public async Task<Microsoft.Graph.Models.Invitation?> CreateAzureUser(string email) 
        {
            string[] scopes = new[] { "https://graph.microsoft.com/.default" };
            var graphClient = new GraphServiceClient(new ChainedTokenCredential(
                                        new ManagedIdentityCredential(Environment.GetEnvironmentVariable("AZURE_CLIENT_ID")),
                                        new EnvironmentCredential()),scopes);
            
            var body = new Microsoft.Graph.Models.Invitation
            {
                InvitedUserEmailAddress = email,
                InviteRedirectUrl = "https://um-app-prod.azurewebsites.net/",
            };
            var response = await graphClient.Invitations.PostAsync(body);

            return response;
        }

        public async Task DisableAzureUser(string azureId) 
        {
            try 
            {
                string[] scopes = new[] { "https://graph.microsoft.com/.default" };
                var graphClient = new GraphServiceClient(new ChainedTokenCredential(
                                        new ManagedIdentityCredential(Environment.GetEnvironmentVariable("AZURE_CLIENT_ID")),
                                        new EnvironmentCredential()),scopes);
                    
                var requestBody = new Microsoft.Graph.Models.User
                {
                    AccountEnabled = false
                };
                    
                var result = await graphClient.Users[azureId].PatchAsync(requestBody);
            }

            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }   
        }

        public async Task EnableAzureUser(string azureId) 
        {
            try 
            {
                string[] scopes = new[] { "https://graph.microsoft.com/.default" };
                var graphClient = new GraphServiceClient(new ChainedTokenCredential(
                                        new ManagedIdentityCredential(Environment.GetEnvironmentVariable("AZURE_CLIENT_ID")),
                                        new EnvironmentCredential()),scopes);
                    
                var requestBody = new Microsoft.Graph.Models.User
                {
                    AccountEnabled = true
                };
                    
                var result = await graphClient.Users[azureId].PatchAsync(requestBody);
            }

            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }
    }
}
