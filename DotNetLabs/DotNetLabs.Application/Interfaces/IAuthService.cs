using DotNetLabs.Application.Models.Auth;
using DotNetLabs.Core.Models;

namespace DotNetLabs.Application.Interfaces;

public interface IAuthService
{
    Task<Result> SignUpAsync(string email, string password);

    Task<Result<SignInResponse>> SignInAsync(string email, string password, string ipAddress, CancellationToken ct);

    Task<Result> SignOutAsync(string ipAddress, CancellationToken ct);
}
