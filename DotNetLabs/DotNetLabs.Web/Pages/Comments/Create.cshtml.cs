using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Comments;

namespace DotNetLabs.Web.Pages.Comments;

public class CreateModel : PageModel
{
    private readonly ICommentService _commentService;

    public CreateModel(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [BindProperty] public LeaveCommentRequest Input { get; set; }

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

        var result = await _commentService.LeaveCommentAsync(Input, UserId, CancellationToken.None);
        if (result.IsSuccess)
        {
            return RedirectToPage("./Index");
        }

        ModelState.AddModelError(string.Empty, result.Error);

        return Page();
    }
}
