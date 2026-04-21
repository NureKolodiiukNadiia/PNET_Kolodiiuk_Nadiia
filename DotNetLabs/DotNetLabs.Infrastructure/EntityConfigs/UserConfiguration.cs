using DotNetLabs.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetLabs.Infrastructure.EntityConfigs;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id);

        builder.HasMany(u => u.Comments)
            .WithOne(c => c.User)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.WatchLists)
            .WithOne(wl => wl.User)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
