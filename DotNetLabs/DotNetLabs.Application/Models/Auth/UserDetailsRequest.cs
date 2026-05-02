namespace DotNetLabs.Application.Models.Auth;

public sealed class UserDetailsRequest
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
