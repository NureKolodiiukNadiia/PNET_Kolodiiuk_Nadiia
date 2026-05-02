using DotNetLabs.Application.Models.Auth;
using DotNetLabs.Core.Entities;
using DotNetLabs.Core.Models;

namespace DotNetLabs.Application.Interfaces;

public interface IUserManagementService
{
    Task<Result<User>> GetUserAsync(Guid userId);
    Task<Result<IEnumerable<UserInfo>>> GetAllUsersPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<Result> CreateUserAsync(UserDetailsRequest request, CancellationToken ct);
    Task<Result> UpdateUserProfileAsync(Guid userId, UserDetailsRequest request, CancellationToken ct);
    Task<Result> DeleteUserAsync(Guid userId, CancellationToken ct);
}
