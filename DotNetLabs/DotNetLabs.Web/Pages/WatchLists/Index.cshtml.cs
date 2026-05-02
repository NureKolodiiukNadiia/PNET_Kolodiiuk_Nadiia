using DotNetLabs.Application.Interfaces;
using DotNetLabs.Core.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetLabs.Web.Pages.WatchLists;

public class IndexModel : PageModel
{
    private readonly IWatchListService _watchListService;

    public IndexModel(IWatchListService watchListService)
    {
        _watchListService = watchListService;
    }

    public IEnumerable<WatchList> WatchLists { get; set; } = [];

    public async Task OnGetAsync()
    {
        var result = await _watchListService.GetAllWatchListsPagedAsync(1, 50, CancellationToken.None);
        if (result.IsSuccess)
        {
            WatchLists = result.Value;
        }
    }
}
