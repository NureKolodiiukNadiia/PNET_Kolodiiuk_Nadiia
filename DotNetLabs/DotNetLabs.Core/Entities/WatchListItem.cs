namespace DotNetLabs.Core.Entities;

public sealed class WatchListItem
{
    public long Id { get; set; }

    public long WatchListId { get; set; }

    public long TitleId { get; set; }

    public Title Title { get; set; }

    public WatchList WatchList { get; set; }
}
