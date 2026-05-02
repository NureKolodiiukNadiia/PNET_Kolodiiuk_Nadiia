using DotNetLabs.Application.Interfaces;
using DotNetLabs.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetLabs.Web.Pages.Titles;

public class DetailsModel : PageModel
{
    private readonly ITitleService _titleService;

    public DetailsModel(ITitleService titleService)
    {
        _titleService = titleService;
    }

    public Title Title { get; private set; }

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var result = await _titleService.GetTitleByIdAsync(id, ct);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        Title = result.Value;
        return Page();
    }
}
