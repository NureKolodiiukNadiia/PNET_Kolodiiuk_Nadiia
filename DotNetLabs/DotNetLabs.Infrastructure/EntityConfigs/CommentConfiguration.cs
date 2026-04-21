using DotNetLabs.Core;
using DotNetLabs.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetLabs.Infrastructure.EntityConfigs;

internal sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.Property(c => c.Id).IsRequired();
        builder.Property(c => c.TitleId).IsRequired();
        builder.Property(c => c.UserId);
        builder.Property(c => c.UpdatedAt).IsRequired();
        builder.Property(c => c.Text).HasMaxLength(5000).IsRequired();
        builder.Property(c => c.IsDeleted).IsRequired();

        builder.HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(c => c.Title)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TitleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

