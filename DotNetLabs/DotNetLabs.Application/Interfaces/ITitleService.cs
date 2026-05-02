using DotNetLabs.Application.Models.Content;
using DotNetLabs.Core.Entities;
using DotNetLabs.Core.Models;

namespace DotNetLabs.Application.Interfaces;

public interface ITitleService
{
    Task<Result<Title>> GetTitleByIdAsync(int titleId, CancellationToken ct);
    Task<Result> CreateTitleAsync(TitleRequest request, CancellationToken ct);
    Task<Result> UpdateTitleAsync(int titleId, TitleUpdateRequest request, CancellationToken ct);
    Task<Result> DeleteTitleAsync(int titleId, CancellationToken ct);
    Task<Result<IEnumerable<TitleShortInfo>>> GetAllTitlesPagedAsync(int page, int pageSize, CancellationToken ct);
}
