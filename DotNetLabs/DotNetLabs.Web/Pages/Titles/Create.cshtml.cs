using DotNetLabs.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetLabs.Web.Pages.Titles;

public class CreateModel : PageModel
{
    private readonly ITitleService _titleService;

    public CreateModel(ITitleService titleService)
    {
        _titleService = titleService;
    }

    [BindProperty]
    public TitleCreateInputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var result = await _titleService.CreateTitleAsync(Input.ToRequest(), ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return Page();
        }

        return RedirectToPage("/Index", new { page = Page, pageSize = PageSize });
    }
}
