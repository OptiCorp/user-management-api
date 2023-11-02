using usermanagement.core.Models;
using usermanagement.core.Models.DTO;

namespace usermanagement.core.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<UserDto>> GetAllUsersAdminAsync();
        Task<UserDto> GetUserByUsernameAsync(string name);
        Task<User> GetUserByAzureAdUserIdAsync(string azureAdUserId);
        Task<UserDto> GetUserByIdAsync(string id);
        Task UpdateUserAsync(UserUpdateDto user);
        Task<string> CreateUserAsync(UserCreateDto user);
        Task DeleteUserAsync(string id);
        Task HardDeleteUserAsync(string id);
        Task<string> GetInspectorRoleIdAsync();
        Task UserCreated(User user);
        Task UserUpdated(User user);
        Task UserSoftDeleted(User user);
        Task UserHardDeleted(User user);
    }
}
