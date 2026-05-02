using Microsoft.AspNetCore.Mvc.RazorPages;
using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Auth;

namespace DotNetLabs.Web.Pages.Users;

public class IndexModel : PageModel
{
    private readonly IUserManagementService _userService;

    public IndexModel(IUserManagementService userService)
    {
        _userService = userService;
    }

    public IEnumerable<UserInfo> Users { get; set; }

    public async Task OnGetAsync()
    {
        var result = await _userService.GetAllUsersPagedAsync(1, 50, CancellationToken.None);
        if (result.IsSuccess)
        {
            Users = result.Value;
        }
    }
}
