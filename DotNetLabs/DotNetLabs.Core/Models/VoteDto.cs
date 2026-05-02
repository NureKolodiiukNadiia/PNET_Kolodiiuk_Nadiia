namespace DotNetLabs.Core.Models;

public sealed class VoteDto
{
    public long Id { get; set; }
    public long TitleId { get; set; }
    public Guid? UserId { get; set; }
    public short Value { get; set; }
    public DateTime UpdatedAt { get; set; }
}
