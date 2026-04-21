using DotNetLabs.Application.Models.Content;
using DotNetLabs.Core.Entities;
using DotNetLabs.Core.Models;

namespace DotNetLabs.Application.Interfaces;

public interface IContentService
{
    Task<Result<Title>> GetTitleByIdAsync(int titleId, CancellationToken ct);

    Task<Result<IEnumerable<TitleShortInfo>>> SearchTitlesAsync(string searchTerm, int pageSize, int page, CancellationToken ct);
}
