using DotNetLabs.Application.Interfaces;
using DotNetLabs.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetLabs.Web.Pages.WatchLists.Items
{
    public class CreateModel : PageModel
    {
        private readonly IWatchListService _watchListService;

        public CreateModel(IWatchListService watchListService)
        {
            _watchListService = watchListService;
        }

        [BindProperty]
        public long WatchListId { get; set; }

        [BindProperty]
        public long TitleId { get; set; }

        public async Task<IActionResult> OnGetAsync(long watchListId)
        {
            var watchList = await _watchListService.GetWatchListAsync(watchListId);
            if (!watchList.IsSuccess)
            {
                return NotFound();
            }

            WatchListId = watchListId;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var item = new WatchListItem
            {
                TitleId = TitleId
            };

            var result = await _watchListService.AddItemToWatchListAsync(WatchListId, item);
            if (result.IsSuccess)
            {
                return RedirectToPage("./Index", new { watchListId = WatchListId });
            }

            ModelState.AddModelError(string.Empty, result.Error);
            return Page();
        }
    }
}
