using DotNetLabs.Application.Interfaces;
using DotNetLabs.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetLabs.Web.Pages.WatchLists.Items
{
    public class IndexModel : PageModel
    {
        private readonly IWatchListService _watchListService;

        public IndexModel(IWatchListService watchListService)
        {
            _watchListService = watchListService;
        }

        public List<WatchListItem> Items { get; set; } = [];
        public long WatchListId { get; set; }

        public async Task<IActionResult> OnGetAsync(long watchListId)
        {
            WatchListId = watchListId;
            var watchList = await _watchListService.GetWatchListAsync(watchListId);
            if (!watchList.IsSuccess)
            {
                return NotFound();
            }

            var result = await _watchListService.GetWatchListItemsAsync(watchListId);
            if (result.IsSuccess)
            {
                Items = result.Value;
            }

            return Page();
        }
    }
}
