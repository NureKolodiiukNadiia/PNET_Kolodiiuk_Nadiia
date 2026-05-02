using DotNetLabs.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetLabs.Web.Pages.WatchLists.Items
{
    public class DeleteModel : PageModel
    {
        private readonly IWatchListService _watchListService;

        public DeleteModel(IWatchListService watchListService)
        {
            _watchListService = watchListService;
        }

        [BindProperty]
        public long WatchListId { get; set; }

        [BindProperty]
        public long ItemId { get; set; }

        public long TitleId { get; set; }

        public async Task<IActionResult> OnGetAsync(long watchListId, long itemId)
        {
            var item = await _watchListService.GetWatchListItemAsync(watchListId, itemId);
            if (!item.IsSuccess)
            {
                return NotFound();
            }

            WatchListId = watchListId;
            ItemId = itemId;
            TitleId = item.Value.TitleId;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _watchListService.RemoveItemFromWatchListAsync(WatchListId, ItemId);
            if (result.IsSuccess)
            {
                return RedirectToPage("./Index", new { watchListId = WatchListId });
            }

            ModelState.AddModelError(string.Empty, result.Error);
            return Page();
        }
    }
}
