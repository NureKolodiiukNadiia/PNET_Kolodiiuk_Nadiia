using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DotNetLabs.Application.Interfaces;

namespace DotNetLabs.Web.Pages.Votes;

public class CreateModel : PageModel
{
    private readonly IVoteService _voteService;

    public CreateModel(IVoteService voteService)
    {
        _voteService = voteService;
    }

    [BindProperty] public long TitleId { get; set; }
    [BindProperty] public short Value { get; set; }
    [BindProperty] public Guid UserId { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        var result = await _voteService.VoteAsync(TitleId, Value, UserId, CancellationToken.None);
        if (result.IsSuccess)
        {
            return RedirectToPage("./Index");
        }

        ModelState.AddModelError(string.Empty, result.Error);

        return Page();
    }
}
