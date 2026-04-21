using Microsoft.AspNetCore.Identity;

namespace DotNetLabs.Core.Entities;

public sealed class User : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<WatchList> WatchLists { get; set; } = new List<WatchList>();
}
