using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Auth;

namespace DotNetLabs.Web.Pages.Users;

public class EditModel : PageModel
{
    private readonly IUserManagementService _userService;

    public EditModel(IUserManagementService userService)
    {
        _userService = userService;
    }

    [BindProperty] public Guid UserId { get; set; }
    [BindProperty] public UserDetailsRequest Input { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var user = await _userService.GetUserAsync(id);
        if (!user.IsSuccess)
        {
            return NotFound();
        }

        UserId = user.Value.Id;
        Input = new UserDetailsRequest
        {
            Email = user.Value.Email,
            PhoneNumber = user.Value.PhoneNumber,
            UserName = user.Value.UserName
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _userService.UpdateUserProfileAsync(UserId, Input, CancellationToken.None);
        if (result.IsSuccess)
        {
            return RedirectToPage("./Index");
        }

        ModelState.AddModelError(string.Empty, result.Error);

        return Page();
    }
}
