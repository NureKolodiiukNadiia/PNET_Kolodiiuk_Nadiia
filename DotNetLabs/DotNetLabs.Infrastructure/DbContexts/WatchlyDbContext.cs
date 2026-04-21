using DotNetLabs.Core;
using DotNetLabs.Core.Entities;
using DotNetLabs.Infrastructure.EntityConfigs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotNetLabs.Infrastructure.DbContexts;

public class WatchlyDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public WatchlyDbContext(DbContextOptions<WatchlyDbContext> options) : base(options)
    {
    }

    public DbSet<Comment> Comments { get; set; }
    public DbSet<Title> Titles { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<WatchList> WatchLists { get; set; }
    public DbSet<WatchListItem> WatchListItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new CommentConfiguration());
        builder.ApplyConfiguration(new TitleConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new VoteConfiguration());
        builder.ApplyConfiguration(new WatchListConfiguration());
        builder.ApplyConfiguration(new WatchListItemConfiguration());
        // builder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(WatchlyDbContext))!);
    }
}
