using DotNetLabs.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetLabs.Web.Pages.WatchLists;

public class EditModel : PageModel
{
    private readonly IWatchListService _watchListService;

    public EditModel(IWatchListService watchListService)
    {
        _watchListService = watchListService;
    }

    [BindProperty] public long Id { get; set; }

    [BindProperty] public string Name { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(long id)
    {
        var watchList = await _watchListService.GetWatchListAsync(id);
        if (!watchList.IsSuccess)
        {
            return NotFound();
        }

        Id = watchList.Value.Id;
        Name = watchList.Value.Name;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _watchListService.UpdateWatchListAsync(Id, Name, CancellationToken.None);
        if (result.IsSuccess)
        {
            return RedirectToPage("./Index");
        }

        ModelState.AddModelError(string.Empty, result.Error);
        return Page();
    }
}
