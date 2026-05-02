using DotNetLabs.Application.Interfaces;
using DotNetLabs.Core.Entities;
using DotNetLabs.Core.Models;
using DotNetLabs.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace DotNetLabs.Application.Services;

public class WatchListService : IWatchListService
{
    private readonly AppDbContext _dbContext;

    public WatchListService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<WatchList>> GetWatchListAsync(long watchListId)
    {
        try
        {
            var watchList = await _dbContext.WatchLists
                .AsNoTracking()
                .FirstOrDefaultAsync(wl => wl.Id == watchListId);

            if (watchList is null)
            {
                return Result.Fail<WatchList>("Watchlist not found");
            }

            return Result.Success(watchList);
        }
        catch (DbException e)
        {
            return Result.Fail<WatchList>($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail<WatchList>($"Error: {e.Message}");
        }
    }

    public async Task<Result<IEnumerable<WatchList>>> GetAllWatchListsPagedAsync(int page, int pageSize,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var watchLists = await _dbContext.WatchLists
                .OrderBy(wl => wl.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(ct);

            return Result.Success<IEnumerable<WatchList>>(watchLists);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (DbException e)
        {
            return Result.Fail<IEnumerable<WatchList>>($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail<IEnumerable<WatchList>>($"Error: {e.Message}");
        }
    }

    public async Task<Result> CreateWatchListAsync(Guid userId, string name, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var userExists = await _dbContext.Users.AnyAsync(u => u.Id == userId, ct);
            if (!userExists)
            {
                return Result.Fail("User not found");
            }

            var watchList = new WatchList
            {
                UserId = userId,
                Name = name
            };

            await _dbContext.WatchLists.AddAsync(watchList, ct);
            await _dbContext.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (DbException e)
        {
            return Result.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail($"Error: {e.Message}");
        }
    }

    public async Task<Result> UpdateWatchListAsync(long watchListId, string name, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var watchList = await _dbContext.WatchLists.FirstOrDefaultAsync(wl => wl.Id == watchListId, ct);
            if (watchList is null)
            {
                return Result.Fail("Watchlist not found");
            }

            watchList.Name = name;
            await _dbContext.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (DbException e)
        {
            return Result.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail($"Error: {e.Message}");
        }
    }

    public async Task<Result> DeleteWatchListAsync(long watchListId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var watchList = await _dbContext.WatchLists.FirstOrDefaultAsync(wl => wl.Id == watchListId, ct);
            if (watchList is null)
            {
                return Result.Fail("Watchlist not found");
            }

            _dbContext.WatchLists.Remove(watchList);
            await _dbContext.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (DbException e)
        {
            return Result.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail($"Error: {e.Message}");
        }
    }

    public async Task<Result<List<WatchListItem>>> GetWatchListItemsAsync(long watchListId)
    {
        try
        {
            var watchListExists = await _dbContext.WatchLists.AnyAsync(wl => wl.Id == watchListId);
            if (!watchListExists)
            {
                return Result.Fail<List<WatchListItem>>("Watchlist not found");
            }

            var items = await _dbContext.WatchListItems
                .Where(wi => wi.WatchListId == watchListId)
                .AsNoTracking()
                .ToListAsync();

            return Result.Success(items);
        }
        catch (DbException e)
        {
            return Result.Fail<List<WatchListItem>>($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail<List<WatchListItem>>($"Error: {e.Message}");
        }
    }

    public async Task<Result<WatchListItem>> GetWatchListItemAsync(long watchListId, long itemId)
    {
        try
        {
            var item = await _dbContext.WatchListItems
                .AsNoTracking()
                .FirstOrDefaultAsync(wi => wi.WatchListId == watchListId && wi.Id == itemId);

            if (item is null)
            {
                return Result.Fail<WatchListItem>("Watchlist item not found");
            }

            return Result.Success(item);
        }
        catch (DbException e)
        {
            return Result.Fail<WatchListItem>($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail<WatchListItem>($"Error: {e.Message}");
        }
    }

    public async Task<Result> AddItemToWatchListAsync(long watchListId, WatchListItem watchListItem)
    {
        try
        {
            var watchListExists = await _dbContext.WatchLists.AnyAsync(wl => wl.Id == watchListId);
            if (!watchListExists)
            {
                return Result.Fail("Watchlist not found");
            }

            var titleExists = await _dbContext.Titles.AnyAsync(t => t.Id == watchListItem.TitleId);
            if (!titleExists)
            {
                return Result.Fail("Title not found");
            }

            var isTitleInWatchlist = await _dbContext.WatchListItems.AnyAsync(item =>
                item.WatchListId == watchListId && item.TitleId == watchListItem.TitleId);
            if (isTitleInWatchlist)
            {
                return Result.Fail("Duplicated entry");
            }

            watchListItem.WatchListId = watchListId;
            await _dbContext.WatchListItems.AddAsync(watchListItem);
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (DbException e)
        {
            return Result.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail($"Error: {e.Message}");
        }
    }

    public async Task<Result> UpdateWatchListItemAsync(long watchListId, long itemId, long titleId)
    {
        try
        {
            var watchListItem = await _dbContext.WatchListItems
                .FirstOrDefaultAsync(wi => wi.WatchListId == watchListId && wi.Id == itemId);

            if (watchListItem is null)
            {
                return Result.Fail("Watchlist item not found");
            }

            var titleExists = await _dbContext.Titles.AnyAsync(t => t.Id == titleId);
            if (!titleExists)
            {
                return Result.Fail("Title not found");
            }

            watchListItem.TitleId = titleId;
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (DbException e)
        {
            return Result.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail($"Error: {e.Message}");
        }
    }

    public async Task<Result> RemoveItemFromWatchListAsync(long watchListId, long itemId)
    {
        try
        {
            var watchListItem = await _dbContext.WatchListItems
                .FirstOrDefaultAsync(wi => wi.WatchListId == watchListId && wi.Id == itemId);

            if (watchListItem is null)
            {
                return Result.Fail("Watchlist item not found");
            }

            _dbContext.WatchListItems.Remove(watchListItem);
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (DbException e)
        {
            return Result.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail($"Error: {e.Message}");
        }
    }
}
