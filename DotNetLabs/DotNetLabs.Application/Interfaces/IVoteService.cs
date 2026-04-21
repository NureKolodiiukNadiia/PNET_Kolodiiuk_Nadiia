using DotNetLabs.Core.Models;

namespace DotNetLabs.Application.Interfaces;

public interface IVoteService
{
    Task<Result> VoteAsync(long titleId, short value, Guid result, CancellationToken ct);
    Task<Result> ChangeVoteAsync(long titleId, short value, Guid result, CancellationToken ct);
}
