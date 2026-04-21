using System.Text;
using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Auth;
using DotNetLabs.Core.Entities;
using DotNetLabs.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace DotNetLabs.Application.Services;

public sealed class UserManagementService : IUserManagementService
{
    private readonly UserManager<User> _userManager;

    public UserManagementService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> ChangeUserNameAsync(Guid userId, string userName)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return Result.Fail("User is null");
            }

            var res = await _userManager.SetUserNameAsync(user, userName);
            if (!res.Succeeded)
            {
                return Result.Fail(FormatIdentityError(res));
            }

            var updateRes = await _userManager.UpdateAsync(user);
            if (!updateRes.Succeeded)
            {
                return Result.Fail(FormatIdentityError(res));
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Result<UserInfo>> GetUserAsync(Guid userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user switch
            {
                null => Result.Fail<UserInfo>("User is null"),
                _ => Result.Success(new UserInfo { Id = user.Id, Email = user.Email })
            };
        }
        catch (Exception e)
        {
            return Result.Fail<UserInfo>(e.Message);
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
