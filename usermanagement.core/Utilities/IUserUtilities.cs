using usermanagement.core.Models;
using usermanagement.core.Models.DTO;


namespace usermanagement.core.Utilities
{
    public interface IUserUtilities
    {
        bool IsUsernameTaken(IEnumerable<UserDto> users, string username);

        bool IsEmailTaken(IEnumerable<UserDto> users, string userEmail);

        bool IsValidStatus(string value);

        UserDto UserToDto(User user);
    }
}
