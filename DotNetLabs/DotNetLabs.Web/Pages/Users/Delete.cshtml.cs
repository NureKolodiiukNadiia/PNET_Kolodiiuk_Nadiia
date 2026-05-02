using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DotNetLabs.Application.Interfaces;

namespace DotNetLabs.Web.Pages.Users;

public class DeleteModel : PageModel
{
    private readonly IUserManagementService _userService;

    public DeleteModel(IUserManagementService userService)
    {
        _userService = userService;
    }

    [BindProperty] public Guid UserId { get; set; }
    public string UserEmail { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var user = await _userService.GetUserAsync(id);
        if (!user.IsSuccess)
        {
            return NotFound();
        }

        UserId = user.Value.Id;
        UserEmail = user.Value.Email;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await _userService.DeleteUserAsync(UserId, CancellationToken.None);

        return result.IsSuccess switch
        {
            true => RedirectToPage("./Index"),
            _ => Page()
        };
    }
}
