using DotNetLabs.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetLabs.Web.Pages.Titles;

public class EditModel : PageModel
{
    private readonly ITitleService _titleService;

    public EditModel(ITitleService titleService)
    {
        _titleService = titleService;
    }

    [BindProperty]
    public TitleUpdateInputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var result = await _titleService.GetTitleByIdAsync(id, ct);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        Input = TitleUpdateInputModel.FromTitle(result.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var result = await _titleService.UpdateTitleAsync(Input.TitleId, Input.ToRequest(), ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return Page();
        }

        return RedirectToPage("/Index", new { pageNumber = PageNumber, pageSize = PageSize });
    }
}
