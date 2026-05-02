using System.ComponentModel.DataAnnotations;
using DotNetLabs.Application.Models.Content;

namespace DotNetLabs.Web.Pages.Titles;

public class TitleCreateInputModel
{
    [Required]
    public string Name { get; set; }
    public string Overview { get; set; }
    [DataType(DataType.Date)]
    public DateTime? ReleaseDate { get; set; }
    public int Runtime { get; set; }
    public float? AvgTmdbRating { get; set; }
    public bool IsAdult { get; set; }
    public string HomePage { get; set; }
    public string PosterUrl { get; set; }
    public string Tagline { get; set; }
    public string Director { get; set; }
    public string Actors { get; set; }
    public string ProductionCompanies { get; set; }
    public string Genres { get; set; }
    public string Keywords { get; set; }
    public string SpokenLanguages { get; set; }

    public TitleRequest ToRequest()
    {
        return new TitleRequest
        {
            Name = Name,
            Overview = Overview,
            ReleaseDate = ReleaseDate,
            Runtime = Runtime,
            AvgTmdbRating = AvgTmdbRating,
            IsAdult = IsAdult,
            HomePage = HomePage,
            PosterUrl = PosterUrl,
            Tagline = Tagline,
            Director = Director,
            Actors = Actors,
            ProductionCompanies = ProductionCompanies,
            Genres = Genres,
            Keywords = Keywords,
            SpokenLanguages = SpokenLanguages
        };
    }
}
