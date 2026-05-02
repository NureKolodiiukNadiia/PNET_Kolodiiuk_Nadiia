using DotNetLabs.Application.Models.Content;
using DotNetLabs.Core.Entities;
using DotNetLabs.Infrastructure.EntityConfigs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotNetLabs.Infrastructure.DbContexts;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Comment> Comments { get; set; }
    public DbSet<Title> Titles { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<WatchList> WatchLists { get; set; }
    public DbSet<WatchListItem> WatchListItems { get; set; }

    [DbFunction("Get_Title_With_Highest_Rating_By_Date", "dbo")]
    public string GetHighestRatingTitleByDate(DateTime voteDate)
    {
        throw new NotSupportedException("This method is for EF Core LINQ translation only.");
    }

    public IQueryable<TitleGenreInfo> GetTitleInfoByGenre(string genre, int i)
        => FromExpression(() => GetTitleInfoByGenre(genre, i));
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new CommentConfiguration());
        builder.ApplyConfiguration(new TitleConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new VoteConfiguration());
        builder.ApplyConfiguration(new WatchListConfiguration());
        builder.ApplyConfiguration(new WatchListItemConfiguration());

        builder.HasDbFunction(typeof(AppDbContext)
                .GetMethod(nameof(GetTitleInfoByGenre),
                    new[] { typeof(string), typeof(int) }))
            .HasName("Get_Title_Info_By_Genre");
        builder.Entity<TitleGenreInfo>().HasNoKey();
    }
}
