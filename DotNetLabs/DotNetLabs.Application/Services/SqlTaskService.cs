using System.Data;
using System.Data.Common;
using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Content;
using DotNetLabs.Core.Models;
using DotNetLabs.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DotNetLabs.Application.Services;

public sealed class SqlTaskService : ISqlTaskService
{
    private readonly AppDbContext _dbContext;

    public SqlTaskService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> AddVoteByProcedureAsync(string titleName, string email, short voteValue, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                "EXEC Add_Vote @TitleName = {0}, @Email = {1}, @VoteValue = {2}",
                [titleName, email, voteValue],
                ct);

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

    public async Task<Result<IEnumerable<TitleGenreInfo>>> GetTitleInfoByGenreAsync(string genre, int i, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var res = await _dbContext
                .GetTitleInfoByGenre(genre, i)
                .AsNoTracking()
                .ToListAsync(ct);

            return Result.Success<IEnumerable<TitleGenreInfo>>(res);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (DbException e)
        {
            return Result.Fail<IEnumerable<TitleGenreInfo>>($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail<IEnumerable<TitleGenreInfo>>($"Error: {e.Message}");
        }
    }

    public async Task<Result<string>> GetTitleWithHighestRatingByDate(DateTime voteDate)
    {
        try
        {
            var res = await _dbContext.Titles
                .Where(title => title.Name == _dbContext.GetHighestRatingTitleByDate(voteDate))
                .FirstOrDefaultAsync();
            if (res is null)
            {
                return Result.Fail<string>("No titles matching criteria");
            }

            return Result.Success(res.Name);
        }
        catch (DbException e)
        {
            return Result.Fail<string>($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail<string>($"Error: {e.Message}");
        }
    }
}
