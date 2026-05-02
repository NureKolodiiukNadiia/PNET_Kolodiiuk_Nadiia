using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DotNetLabs.Application.Interfaces;

namespace DotNetLabs.Web.Pages.Votes;

public class EditModel : PageModel
{
    private readonly IVoteService _voteService;

    public EditModel(IVoteService voteService)
    {
        _voteService = voteService;
    }

    [BindProperty] public long TitleId { get; set; }
    [BindProperty] public short Value { get; set; }
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
        Value = vote.Value.Value;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _voteService.ChangeVoteAsync(TitleId, Value, UserId, CancellationToken.None);
        if (result.IsSuccess)
        {
            return RedirectToPage("./Index");
        }

        ModelState.AddModelError(string.Empty, result.Error);

        return Page();
    }
}
