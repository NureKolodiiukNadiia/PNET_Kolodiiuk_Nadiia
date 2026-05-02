using DotNetLabs.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetLabs.Web.Pages.WatchLists;

public class CreateModel : PageModel
{
    private readonly IWatchListService _watchListService;

    public CreateModel(IWatchListService watchListService)
    {
        _watchListService = watchListService;
    }

    [BindProperty] public Guid UserId { get; set; }

    [BindProperty] public string Name { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _watchListService.CreateWatchListAsync(UserId, Name, CancellationToken.None);
        if (result.IsSuccess)
        {
            return RedirectToPage("./Index");
        }

        ModelState.AddModelError(string.Empty, result.Error);

        return Page();
    }
}
