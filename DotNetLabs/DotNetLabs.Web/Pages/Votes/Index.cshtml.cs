using Microsoft.AspNetCore.Mvc.RazorPages;
using DotNetLabs.Application.Interfaces;
using DotNetLabs.Core.Models;

namespace DotNetLabs.Web.Pages.Votes;

public class IndexModel : PageModel
{
    private readonly IVoteService _voteService;

    public IndexModel(IVoteService voteService)
    {
        _voteService = voteService;
    }

    public IEnumerable<VoteDto> Votes { get; set; }

    public async Task OnGetAsync()
    {
        var result = await _voteService.GetAllVotesPagedAsync(1, 50, CancellationToken.None);
        if (result.IsSuccess)
        {
            Votes = result.Value;
        }
    }
}
