namespace DotNetLabs.Application.Models.Content;

public sealed class TitleGenreInfo
{
    public string Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public float? Rating { get; set; }
    public string Director { get; set; }
    public string Genres { get; set; }
}
