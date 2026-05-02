using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace DotNetLabs.Application.Models.Auth;

public class SignInResponse
{
    public required AuthenticationProperties AuthenticationProperties { get; set; }
    public required ClaimsPrincipal Principal { get; set; }
    public required UserDto User { get; set; }
}
