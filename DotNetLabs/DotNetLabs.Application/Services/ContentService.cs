using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Content;
using DotNetLabs.Core.Entities;
using DotNetLabs.Core.Models;
using DotNetLabs.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DotNetLabs.Application.Services;

public class ContentService : IContentService
{
    private readonly WatchlyDbContext _dbContext;

    public ContentService(WatchlyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Result<Title>> GetTitleByIdAsync(int titleId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<TitleShortInfo>>> SearchTitlesAsync(
        string searchTerm, 
        int pageSize, 
        int page, 
        CancellationToken ct = default)
    {
        var pattern = $"%{searchTerm}%";
        try
        {
            var query = _dbContext.Titles
                .Where(t => EF.Functions.Like(t.Name, pattern))
                .OrderBy(t => t.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TitleShortInfo(t.Id, t.Name, t.PosterUrl, t.AvgTmdbRating))
                .AsNoTracking();

            return Result.Success<IEnumerable<TitleShortInfo>>(await query.ToListAsync(ct));
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result.Fail<IEnumerable<TitleShortInfo>>(e.Message);
        }
    }
}
