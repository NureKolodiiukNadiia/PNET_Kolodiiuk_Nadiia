using DotNetLabs.Core.Enums;

namespace DotNetLabs.Core.Entities;

public class Title
{
    public long Id { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public DateTime UpdatedAt { get; set; }
    /// <summary>
    /// In minutes
    /// </summary>
    public int Runtime { get; set; }
    public float? AvgTmdbRating { get; set; }
    public bool IsAdult { get; set; }
    public string Name { get; set; }
    public string HomePage { get; set; }
    public string Overview { get; set; }
    public string PosterUrl { get; set; }
    public string Tagline { get; set; }
    public string Director { get; set; }
    /// <summary>
    /// Separated by ", "
    /// </summary>
    public string Actors { get; set; }
    public string ProductionCompanies { get; set; }
    public string Genres { get; set; }
    public string Keywords { get; set; }
    public string SpokenLanguages { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    public ICollection<WatchListItem> WatchListItems { get; set; } = new List<WatchListItem>();
}
