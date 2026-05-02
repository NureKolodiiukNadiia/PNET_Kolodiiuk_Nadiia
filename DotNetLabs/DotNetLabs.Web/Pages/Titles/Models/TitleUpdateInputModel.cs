using System.ComponentModel.DataAnnotations;
using DotNetLabs.Application.Models.Content;
using DotNetLabs.Core.Entities;

namespace DotNetLabs.Web.Pages.Titles;

public class TitleUpdateInputModel
{
    [Range(1, int.MaxValue)]
    public int TitleId { get; set; }
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

    public static TitleUpdateInputModel FromTitle(Title title)
    {
        return new TitleUpdateInputModel
        {
            TitleId = (int)title.Id,
            Name = title.Name,
            Overview = title.Overview,
            ReleaseDate = title.ReleaseDate,
            Runtime = title.Runtime,
            AvgTmdbRating = title.AvgTmdbRating,
            IsAdult = title.IsAdult,
            HomePage = title.HomePage,
            PosterUrl = title.PosterUrl,
            Tagline = title.Tagline,
            Director = title.Director,
            Actors = title.Actors,
            ProductionCompanies = title.ProductionCompanies,
            Genres = title.Genres,
            Keywords = title.Keywords,
            SpokenLanguages = title.SpokenLanguages
        };
    }

    public TitleUpdateRequest ToRequest()
    {
        return new TitleUpdateRequest
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
