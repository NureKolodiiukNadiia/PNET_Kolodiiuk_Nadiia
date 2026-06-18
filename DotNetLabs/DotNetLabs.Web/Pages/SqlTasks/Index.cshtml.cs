using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Content;
using DotNetLabs.Web.Pages.SqlTasks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetLabs.Web.Pages.SqlTasks;

public class IndexModel : PageModel
{
    private readonly ISqlTaskService _sqlTaskService;

    public IndexModel(ISqlTaskService sqlTaskService)
    {
        _sqlTaskService = sqlTaskService;
    }

    [BindProperty]
    public AddVoteInput AddVote { get; set; } = new();

    [BindProperty]
    public GenreInput GenreFilter { get; set; } = new();

    [BindProperty]
    public DateInput DateFilter { get; set; } = new();

    public IEnumerable<TitleGenreInfo> GenreResults { get; private set; } = Array.Empty<TitleGenreInfo>();
    public string HighestRatedTitleResult { get; private set; } = string.Empty;
    public string ProcedureResult { get; private set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAddVoteAsync(CancellationToken ct)
    {
        var result = await _sqlTaskService.AddVoteByProcedureAsync(
            AddVote.TitleName, AddVote.Email, AddVote.VoteValue, ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error);

            return Page();
        }

        ProcedureResult = "Stored procedure executed successfully.";

        return Page();
    }

    public async Task<IActionResult> OnPostGenreAsync(CancellationToken ct)
    {
        var result = await _sqlTaskService.GetTitleInfoByGenreAsync(GenreFilter.Genre, GenreFilter.I, ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            
            return Page();
        }

        GenreResults = result.Value;
        
        return Page();
    }

    public async Task<IActionResult> OnPostHighestByDate(CancellationToken ct)
    {
        var result = await _sqlTaskService.GetTitleWithHighestRatingByDate(DateFilter.VoteDate);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            
            return Page();
        }

        HighestRatedTitleResult = result.Value;
        
        return Page();
    }
}
