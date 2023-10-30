using usermanagement.core.Models;
using usermanagement.core.Models.DTO;


namespace usermanagement.core.Services
{
    public interface IUserRoleService
    {
        Task<IEnumerable<UserRole>> GetUserRolesAsync();
        Task<UserRole> GetUserRoleByIdAsync(string id);
        Task<UserRole> GetUserRoleByUserRoleNameAsync(string userRoleName);
        Task UpdateUserRoleAsync(UserRoleUpdateDto userRole);
        Task<string> CreateUserRoleAsync(UserRoleCreateDto userRole);
        Task DeleteUserRoleAsync(string id);
        Task<bool> IsUserRoleInUseAsync(UserRole userRole);
    }
}
