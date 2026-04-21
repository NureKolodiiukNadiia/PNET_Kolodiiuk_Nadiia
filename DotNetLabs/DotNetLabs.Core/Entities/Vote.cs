namespace DotNetLabs.Core.Entities;

public sealed class Vote
{
    public long Id { get; set; }
    public long TitleId { get; set; }
    public Guid UserId { get; set; }
    public short Value { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Title Title { get; set; }
}
