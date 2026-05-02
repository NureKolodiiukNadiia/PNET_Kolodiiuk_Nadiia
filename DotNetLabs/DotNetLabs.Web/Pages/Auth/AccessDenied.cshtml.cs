using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetLabs.Web.Pages.Auth;

[AllowAnonymous]
public class AccessDenied : PageModel
{
    public void OnGet()
    {
    }
}

