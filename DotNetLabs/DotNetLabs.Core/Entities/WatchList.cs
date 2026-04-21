namespace DotNetLabs.Core.Entities;

public sealed class WatchList
{
    public long Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; }

    public User User { get; set; }

    public ICollection<WatchListItem> WatchListItems { get; set; } = new List<WatchListItem>();
}
