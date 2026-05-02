using DotNetLabs.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetLabs.Web.Pages.Auth;

[AllowAnonymous]
public class SignIn : PageModel
{
    private readonly IAuthService _authService;

    public SignIn(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public SignInInputModel Input { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    public IActionResult OnGet(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Page();

        ct.ThrowIfCancellationRequested();
        var result = await _authService.SignInAsync(Input.Email, Input.Password, ct);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error);

            return Page();
        }

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            result.Value.Principal,
            result.Value.AuthenticationProperties);

        if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
        {
            return LocalRedirect(ReturnUrl);
        }

        return RedirectToPage("/Index");
    }
}
