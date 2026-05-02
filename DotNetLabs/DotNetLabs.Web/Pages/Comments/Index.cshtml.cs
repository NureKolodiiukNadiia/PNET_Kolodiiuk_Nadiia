using Microsoft.AspNetCore.Mvc.RazorPages;
using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Comments;

namespace DotNetLabs.Web.Pages.Comments;

public class IndexModel : PageModel
{
    private readonly ICommentService _commentService;

    public IndexModel(ICommentService commentService)
    {
        _commentService = commentService;
    }

    public IEnumerable<CommentDto> Comments { get; set; }

    public async Task OnGetAsync()
    {
        var result = await _commentService.GetAllCommentsPagedAsync(1, 50, CancellationToken.None);
        if (result.IsSuccess)
        {
            Comments = result.Value;
        }
    }
}
