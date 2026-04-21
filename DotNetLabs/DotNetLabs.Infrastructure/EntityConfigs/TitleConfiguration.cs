using DotNetLabs.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetLabs.Infrastructure.EntityConfigs;

internal sealed class TitleConfiguration : IEntityTypeConfiguration<Title>
{
    public void Configure(EntityTypeBuilder<Title> b)
    {
        b.HasKey(t => t.Id);

        b.Property(t => t.ReleaseDate);
        b.Property(t => t.UpdatedAt).IsRequired();
        b.Property(t => t.Runtime).IsRequired();
        b.Property(t => t.Name).HasMaxLength(300).IsRequired();
        b.Property(t => t.AvgTmdbRating);
        b.Property(t => t.HomePage).HasMaxLength(500);
        b.Property(t => t.Overview).HasMaxLength(5000).IsRequired();
        b.Property(t => t.PosterUrl).HasMaxLength(500);
        b.Property(t => t.Actors).HasMaxLength(3000);
        b.Property(t => t.Director).HasMaxLength(200);
        b.Property(t => t.IsAdult).IsRequired();
        b.Property(t => t.Tagline).HasMaxLength(500);
        b.Property(t => t.Genres).HasMaxLength(1000);
        b.Property(t => t.ProductionCompanies).HasMaxLength(1000);
        b.Property(t => t.SpokenLanguages).HasMaxLength(1000);
        b.Property(t => t.Keywords).HasMaxLength(1000);

        b.HasMany(t => t.Votes)
            .WithOne(v => v.Title)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasMany(t => t.Comments)
            .WithOne(c => c.Title)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasMany(t => t.WatchListItems)
            .WithOne(wi => wi.Title)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
