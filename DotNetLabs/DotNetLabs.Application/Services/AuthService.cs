using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Auth;
using DotNetLabs.Core.Entities;
using DotNetLabs.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace DotNetLabs.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;

    public AuthService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> SignUpAsync(string email, string password)
    {
        var user = new User { Email = email };
        user.UserName = user.Email ?? "";
        try
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return Result.Fail($"Failed to create a user: {string.Join(", ",
                    result.Errors.Select(e => e.Description))}");
            }

            var roleRes = await _userManager.AddToRoleAsync(user, "User");
            if (!roleRes.Succeeded)
            {
                var codes = roleRes.Errors.Select(err => err.Code);
                
                return Result.Fail(
                    $"Failed to add to role: {string.Join(", ", codes)}");
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Fail($"Error registering user: {e.Message}");
        }
    }

    public async Task<Result<SignInResponse>> SignInAsync(
        string email, string password, string ipAddress, CancellationToken ct)
    {
        throw new NotImplementedException();
        // var validationResult = await ValidateUserCredentialsAsync(email, password);
        // if (validationResult.Failure)
        // {
        //     return Result.Fail<SignInResponse>("User credentials validation failure");
        // }
        //
        // var tokensRes = await GenerateTokensAsync(validationResult.Value, ipAddress, ct);
        // if (tokensRes.Failure)
        // {
        //     return Result.Fail<SignInResponse>(tokensRes.Error);
        // }
        //
        // var userRoles = await _userManager.GetRolesAsync(validationResult.Value);
        //
        // var response = CreateSignInResponse(tokensRes.Value, email,
        //     validationResult.Value.UserName, validationResult.Value.Id, userRoles);
        //
        // return Result.Success(response);
    }

    public async Task<Result> SignOutAsync(string ipAddress, CancellationToken ct)
    {
        throw new NotImplementedException();

        return Result.Success();
    }

    private async Task<Result<User>> ValidateUserCredentialsAsync(string email, string password)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _userManager.PasswordHasher.HashPassword(new User(), password);

                return Result.Fail<User>("Invalid email or password");
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);

            return isPasswordCorrect switch
            {
                false => Result.Fail<User>("Invalid email or password"),
                _ => Result.Success(user)
            };
        }
        catch (Exception e)
        {
            return Result.Fail<User>($"Error validating user credentials: {e.Message}");
        }
    }
}
