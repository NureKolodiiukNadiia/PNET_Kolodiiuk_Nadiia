using DotNetLabs.Application.Models.Auth;
using DotNetLabs.Core.Models;

namespace DotNetLabs.Application.Interfaces;

public interface IUserManagementService
{
    Task<Result> ChangeUserNameAsync(Guid userId, string userName);
    Task<Result<UserInfo>> GetUserAsync(Guid userId);
}
