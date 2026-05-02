using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DotNetLabs.Application.Interfaces;

namespace DotNetLabs.Web.Pages.Votes;

public class DeleteModel : PageModel
{
    private readonly IVoteService _voteService;

    public DeleteModel(IVoteService voteService)
    {
        _voteService = voteService;
    }

    [BindProperty] public long TitleId { get; set; }
    [BindProperty] public Guid? UserId { get; set; }

    public async Task<IActionResult> OnGetAsync(long titleId, Guid userId)
    {
        var vote = await _voteService.GetVoteDetailsAsync(titleId, userId, CancellationToken.None);
        if (!vote.IsSuccess)
        {
            return NotFound();
        }

        TitleId = vote.Value.TitleId;
        UserId = vote.Value.UserId;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await _voteService.DeleteVoteAsync(TitleId, UserId, CancellationToken.None);
        if (result.IsSuccess)
        {
            return RedirectToPage("./Index");
        }

        ModelState.AddModelError(string.Empty, result.Error);

        return Page();
    }
}
