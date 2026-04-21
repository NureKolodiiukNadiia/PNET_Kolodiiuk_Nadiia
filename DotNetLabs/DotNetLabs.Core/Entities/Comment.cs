namespace DotNetLabs.Core.Entities;

public sealed class Comment
{
    public long Id { get; set; }
    public long TitleId { get; set; }
    public bool IsDeleted { get; set; }
    public Guid UserId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Text { get; set; }
    public User User { get; set; }
    public Title Title { get; set; }
}
