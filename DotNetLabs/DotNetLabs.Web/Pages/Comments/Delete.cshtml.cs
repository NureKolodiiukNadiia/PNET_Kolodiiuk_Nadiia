using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DotNetLabs.Application.Interfaces;

namespace DotNetLabs.Web.Pages.Comments;

public class DeleteModel : PageModel
{
    private readonly ICommentService _commentService;

    public DeleteModel(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [BindProperty] public long CommentId { get; set; }
    public string Text { get; set; }

    [BindProperty] public Guid UserId { get; set; }

    public async Task<IActionResult> OnGetAsync(long id)
    {
        var comment = await _commentService.GetCommentByIdAsync(id, CancellationToken.None);
        if (!comment.IsSuccess)
        {
            return NotFound();
        }

        CommentId = comment.Value.Id;
        Text = comment.Value.Text;
        UserId = comment.Value.UserId;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await _commentService.DeleteCommentAsync(CommentId, UserId, CancellationToken.None);
        if (result.IsSuccess)
        {
            return RedirectToPage("./Index");
        }

        ModelState.AddModelError(string.Empty, result.Error);

        return Page();
    }
}
