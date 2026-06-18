using System.Data.Common;
using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Content;
using DotNetLabs.Core.Entities;
using DotNetLabs.Core.Models;
using DotNetLabs.Infrastructure.DbContexts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DotNetLabs.Application.Services;

public class TitleService : ITitleService
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };

    private readonly AppDbContext _dbContext;
    private readonly IDistributedCache _cache;

    public TitleService(AppDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<Result<IEnumerable<TitleShortInfo>>> GetAllTitlesPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var cacheVersion = await GetCacheVersionAsync(ct);
            var cacheKey = $"titles:list:v{cacheVersion}:p{page}:s{pageSize}";
            var cached = await _cache.GetStringAsync(cacheKey, ct);
            if (!string.IsNullOrWhiteSpace(cached))
            {
                var fromCache = JsonSerializer.Deserialize<List<TitleShortInfo>>(cached);
                if (fromCache != null)
                {
                    return Result.Success<IEnumerable<TitleShortInfo>>(fromCache);
                }
            }

            var query = _dbContext.Titles
                .OrderBy(t => t.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TitleShortInfo(t.Id, t.Name, t.PosterUrl, t.AvgTmdbRating))
                .AsNoTracking();

            var titles = await query.ToListAsync(ct);
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(titles), CacheOptions, ct);

            return Result.Success<IEnumerable<TitleShortInfo>>(titles);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (DbException e)
        {
            return Result.Fail<IEnumerable<TitleShortInfo>>($"DB problems: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail<IEnumerable<TitleShortInfo>>(e.Message);
        }
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
        catch (DbException e)
        {
            return Result.Fail<IEnumerable<TitleShortInfo>>($"DB problems: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail<IEnumerable<TitleShortInfo>>(e.Message);
        }
    }

    public async Task<Result<Title>> GetTitleByIdAsync(int titleId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var cacheVersion = await GetCacheVersionAsync(ct);
            var cacheKey = $"titles:by-id:v{cacheVersion}:id{titleId}";
            var cached = await _cache.GetStringAsync(cacheKey, ct);
            if (!string.IsNullOrWhiteSpace(cached))
            {
                var cachedTitle = JsonSerializer.Deserialize<Title>(cached);
                if (cachedTitle != null)
                {
                    return Result.Success(cachedTitle);
                }
            }

            var title = await _dbContext.Titles
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == titleId, ct);

            if (title != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(title), CacheOptions, ct);
            }

            return title switch
            {
                null => Result.Fail<Title>("Title not found"),
                _ => Result.Success(title)
            };
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (DbException e)
        {
            return Result.Fail<Title>($"DB problems: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail<Title>(e.Message);
        }
    }

    public async Task<Result> CreateTitleAsync(TitleRequest request, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var title = new Title
            {
                Name = request.Name,
                Overview = request.Overview,
                ReleaseDate = request.ReleaseDate,
                Runtime = request.Runtime,
                AvgTmdbRating = request.AvgTmdbRating,
                IsAdult = request.IsAdult,
                HomePage = request.HomePage,
                PosterUrl = request.PosterUrl,
                Tagline = request.Tagline,
                Director = request.Director,
                Actors = request.Actors,
                ProductionCompanies = request.ProductionCompanies,
                Genres = request.Genres,
                Keywords = request.Keywords,
                SpokenLanguages = request.SpokenLanguages,
                UpdatedAt = DateTime.UtcNow
            };
            await _dbContext.Titles.AddAsync(title, ct);
            await _dbContext.SaveChangesAsync(ct);
            await BumpCacheVersionAsync(ct);

            return Result.Success();
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (DbException e)
        {
            return Result.Fail<Title>($"DB problems: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail<Title>(e.Message);
        }
    }

    public async Task<Result> UpdateTitleAsync(int titleId, TitleUpdateRequest request, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var title = await _dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleId, ct);
            if (title == null)
            {
                return Result.Fail("Title not found");
            }

            title.Name = request.Name;
            title.Overview = request.Overview;
            title.ReleaseDate = request.ReleaseDate;
            title.Runtime = request.Runtime;
            title.AvgTmdbRating = request.AvgTmdbRating;
            title.IsAdult = request.IsAdult;
            title.HomePage = request.HomePage;
            title.PosterUrl = request.PosterUrl;
            title.Tagline = request.Tagline;
            title.Director = request.Director;
            title.Actors = request.Actors;
            title.ProductionCompanies = request.ProductionCompanies;
            title.Genres = request.Genres;
            title.Keywords = request.Keywords;
            title.SpokenLanguages = request.SpokenLanguages;
            title.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(ct);
            await BumpCacheVersionAsync(ct);

            return Result.Success();
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (DbException e)
        {
            return Result.Fail($"DB problems: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result> DeleteTitleAsync(int titleId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var title = await _dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleId, ct);
            if (title == null)
            {
                return Result.Fail("Title not found");
            }

            _dbContext.Titles.Remove(title);
            await _dbContext.SaveChangesAsync(ct);
            await BumpCacheVersionAsync(ct);

            return Result.Success();
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (DbException e)
        {
            return Result.Fail($"DB problems: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    private async Task<int> GetCacheVersionAsync(CancellationToken ct)
    {
        var value = await _cache.GetStringAsync("titles:cache-version", ct);
        if (int.TryParse(value, out var version))
        {
            return version;
        }

        await _cache.SetStringAsync("titles:cache-version", "1", CacheOptions, ct);

        return 1;
    }

    private async Task BumpCacheVersionAsync(CancellationToken ct)
    {
        var version = await GetCacheVersionAsync(ct);
        await _cache.SetStringAsync("titles:cache-version", (version + 1).ToString(), CacheOptions, ct);
    }
}
