using System.Data.Common;
using System.Text;
using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Auth;
using DotNetLabs.Core.Entities;
using DotNetLabs.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotNetLabs.Application.Services;

public sealed class UserManagementService : IUserManagementService
{
    private readonly UserManager<User> _userManager;

    public UserManagementService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<User>> GetUserAsync(Guid userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user switch
            {
                null => Result.Fail<User>("User is null"),
                _ => Result.Success(user)
            };
        }
        catch (Exception e)
        {
            return Result.Fail<User>(e.Message);
        }
    }

    public async Task<Result<IEnumerable<UserInfo>>> GetAllUsersPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var users = await _userManager.Users
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserInfo { Id = u.Id, Email = u.Email })
                .ToListAsync(ct);

            return Result.Success<IEnumerable<UserInfo>>(users);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result.Fail<IEnumerable<UserInfo>>(e.Message);
        }
    }

    public async Task<Result> CreateUserAsync(UserDetailsRequest request, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return Result.Fail(FormatIdentityError(result));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                return Result.Fail(FormatIdentityError(roleResult));
            }

            return Result.Success();
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result> UpdateUserProfileAsync(Guid userId, UserDetailsRequest request, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return Result.Fail("User not found");
            }

            user.UserName = request.UserName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Result.Fail(FormatIdentityError(result));
            }

            return Result.Success();
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (DbException e)
        {
            return Result.Fail($"DB problems: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return Result.Fail("User not found");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return Result.Fail(FormatIdentityError(result));
            }

            return Result.Success();
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (DbException e)
        {
            return Result.Fail($"DB problems: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    private string FormatIdentityError(IdentityResult error)
    {
        var sb = new StringBuilder();
        foreach (var identityError in error.Errors)
        {
            sb.Append(identityError.Code)
                .Append('\n')
                .Append(identityError.Description)
                .Append('\n');
        }

        return sb.ToString();
    }
}

