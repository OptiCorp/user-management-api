using usermanagement.core.Models;

namespace usermanagement.core.Utilities
{

    public class UserRoleUtilities : IUserRoleUtilities
    {
        public bool IsUserRoleNameTaken(IEnumerable<UserRole> userRoles, string userRoleName)
        {
            return userRoles.Any(role => role.Name == userRoleName);
        }

        public bool IsValidUserRole(IEnumerable<UserRole> userRoles, string userRoleId)
        {
            return userRoles.Any(role => role.Id == userRoleId);
        }

    }
}
