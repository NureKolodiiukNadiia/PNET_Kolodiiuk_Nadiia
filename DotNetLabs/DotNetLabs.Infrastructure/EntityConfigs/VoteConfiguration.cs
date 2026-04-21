using DotNetLabs.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetLabs.Infrastructure.EntityConfigs;

internal sealed class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id);
        builder.Property(v => v.Value).IsRequired();
        builder.Property(v => v.UpdatedAt).IsRequired();
        builder.Property(v => v.TitleId).IsRequired();
        builder.Property(v => v.UserId).IsRequired();

        builder.HasOne(v => v.Title)
            .WithMany(t => t.Votes)
            .HasForeignKey(v => v.TitleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
