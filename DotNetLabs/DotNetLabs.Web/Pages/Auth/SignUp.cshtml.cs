using DotNetLabs.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetLabs.Web.Pages.Auth;

[AllowAnonymous]
public class SignUp : PageModel
{
    private readonly IAuthService _authService;

    public SignUp(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public SignUpInputModel Input { get; set; }

    public IActionResult OnGet()
    {
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _authService.SignUpAsync(Input.Email, Input.Password);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return Page();
        }

        return RedirectToPage("/Auth/SignIn");
    }
}
