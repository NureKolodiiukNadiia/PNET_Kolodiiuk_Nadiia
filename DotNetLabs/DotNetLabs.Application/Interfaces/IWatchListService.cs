using DotNetLabs.Core.Entities;
using DotNetLabs.Core.Models;

namespace DotNetLabs.Application.Interfaces;

public interface IWatchListService
{
    Task<Result<WatchList>> GetWatchListAsync(long watchListId);
    Task<Result<IEnumerable<WatchList>>> GetAllWatchListsPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<Result> CreateWatchListAsync(Guid userId, string name, CancellationToken ct);
    Task<Result> UpdateWatchListAsync(long watchListId, string name, CancellationToken ct);
    Task<Result> DeleteWatchListAsync(long watchListId, CancellationToken ct);
    Task<Result<List<WatchListItem>>> GetWatchListItemsAsync(long watchListId);
    Task<Result<WatchListItem>> GetWatchListItemAsync(long watchListId, long itemId);
    Task<Result> AddItemToWatchListAsync(long watchListId, WatchListItem watchListItem);
    Task<Result> UpdateWatchListItemAsync(long watchListId, long itemId, long titleId);
    Task<Result> RemoveItemFromWatchListAsync(long watchListId, long itemId);
}
