using System.ComponentModel.DataAnnotations;

namespace DotNetLabs.Web.Pages.SqlTasks;

public sealed class AddVoteInput
{
    [Required]
    public string TitleName { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Range(0, 10)]
    public short VoteValue { get; set; }
}
