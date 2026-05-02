using System.Security.Claims;
using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Auth;
using DotNetLabs.Core.Entities;
using DotNetLabs.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        string email, string password, CancellationToken ct)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result.Fail<SignInResponse>("User not found");
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordCorrect)
            {
                return Result.Fail<SignInResponse>("Invalid password");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);
            var authProperties = new AuthenticationProperties()
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
            };

            return Result.Success(new SignInResponse
            {
                AuthenticationProperties = authProperties,
                Principal = principal,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    UserRoles = roles
                }
            });
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result.Fail<SignInResponse>("Error signing in");
        }
    }

    public async Task<Result> SignOutAsync(string ipAddress, CancellationToken ct)
    {
        try
        {
            ct.ThrowIfCancellationRequested();
            await Task.CompletedTask;
            return Result.Success();
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result.Fail($"Error signing out: {e.Message}");
        }
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
