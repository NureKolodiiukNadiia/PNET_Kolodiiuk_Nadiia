using DotNetLabs.Application.Models.Content;
using DotNetLabs.Core.Models;

namespace DotNetLabs.Application.Interfaces;

public interface ISqlTaskService
{
    Task<Result> AddVoteByProcedureAsync(string titleName, string email, short voteValue, CancellationToken ct);
    Task<Result<IEnumerable<TitleGenreInfo>>> GetTitleInfoByGenreAsync(string genre, int i, CancellationToken ct);
    Task<Result<string>> GetTitleWithHighestRatingByDate(DateTime voteDate);
}
