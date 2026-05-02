using DotNetLabs.Application.Models.Comments;
using DotNetLabs.Core.Models;

namespace DotNetLabs.Application.Interfaces;

public interface ICommentService
{
    Task<Result<IEnumerable<CommentDto>>> GetAllCommentsPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<Result> LeaveCommentAsync(LeaveCommentRequest request, Guid userId, CancellationToken ct);
    Task<Result> UpdateCommentAsync(long commentId, string text, Guid userId, CancellationToken ct);
    Task<Result<CommentDto>> GetCommentByIdAsync(long commentId, CancellationToken ct);
    Task<Result> DeleteCommentAsync(long commentId, Guid userId, CancellationToken ct);
}
