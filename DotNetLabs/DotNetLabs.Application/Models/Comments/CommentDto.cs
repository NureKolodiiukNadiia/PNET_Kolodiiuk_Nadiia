using DotNetLabs.Application.Models.Auth;

namespace DotNetLabs.Application.Models.Comments;

public sealed record CommentDto(
    long Id,
    long TitleId,
    bool IsTitle,
    Guid UserId,
    DateTime UpdatedAt,
    string Text,
    UserDto User);
