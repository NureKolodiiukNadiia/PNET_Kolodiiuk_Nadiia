using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Content;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetLabs.Web.Pages;

public class IndexModel : PageModel
{
    private const int DefaultPageSize = 10;
    private readonly ITitleService _titleService;

    public IndexModel(ITitleService titleService)
    {
        _titleService = titleService;
    }

    public IEnumerable<TitleShortInfo> Titles { get; private set; } = Array.Empty<TitleShortInfo>();

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = DefaultPageSize;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var result = await _titleService.GetAllTitlesPagedAsync(Page, PageSize, ct);
        if (result.IsSuccess)
        {
            Titles = result.Value;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
    {
        var result = await _titleService.DeleteTitleAsync(id, ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            var listResult = await _titleService.GetAllTitlesPagedAsync(Page, PageSize, ct);
            if (listResult.IsSuccess)
            {
                Titles = listResult.Value;
            }

            return Page();
        }

        return RedirectToPage("/Index", new { page = Page, pageSize = PageSize });
    }
}
