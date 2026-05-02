using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Auth;

namespace DotNetLabs.Web.Pages.Users;

public class CreateModel : PageModel
{
    private readonly IUserManagementService _userService;

    public CreateModel(IUserManagementService userService)
    {
        _userService = userService;
    }

    [BindProperty] public UserDetailsRequest Input { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _userService.CreateUserAsync(Input, CancellationToken.None);
        if (result.IsSuccess)
        {
            return RedirectToPage("./Index");
        }

        ModelState.AddModelError(string.Empty, result.Error);

        return Page();
    }
}