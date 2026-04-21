
using DotNetLabs.Core.Entities;

namespace DotNetLabs.Application.Models.Auth;

public class UserDto
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string UserName { get; set; }

    public IList<string> UserRoles { get; set; }

    public static UserDto MapUser(User user, IList<string> userRoles)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            UserRoles = userRoles
        };
    }
}
