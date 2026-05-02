using DotNetLabs.Core.Models;

namespace DotNetLabs.Application.Interfaces;

public interface IVoteService
{
    Task<Result<IEnumerable<VoteDto>>> GetAllVotesPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<Result<VoteDto>> GetVoteDetailsAsync(long titleId, Guid? userId, CancellationToken ct);
    Task<Result> VoteAsync(long titleId, short value, Guid userId, CancellationToken ct);
    Task<Result> ChangeVoteAsync(long titleId, short value, Guid? userId, CancellationToken ct);
    Task<Result> DeleteVoteAsync(long titleId, Guid? userId, CancellationToken ct);
}
