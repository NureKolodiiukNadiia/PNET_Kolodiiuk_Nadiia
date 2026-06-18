using System.ComponentModel.DataAnnotations;

namespace DotNetLabs.Web.Pages.SqlTasks.Models;

public sealed class GenreInput
{
    [Required]
    public string Genre { get; set; } = string.Empty;

    [Range(0, 10)]
    public int I { get; set; } = 2;
}
