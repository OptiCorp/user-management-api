using usermanagement.core.Models;


namespace usermanagement.core.Utilities
{
    public interface IUserRoleUtilities
    {
        bool IsUserRoleNameTaken(IEnumerable<UserRole> userRoles, string userRoleName);
        bool IsValidUserRole(IEnumerable<UserRole> userRoles, string userRoleId);
    }
}
