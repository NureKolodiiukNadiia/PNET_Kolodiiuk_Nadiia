using System.Data.Common;
using DotNetLabs.Application.Interfaces;
using DotNetLabs.Core.Entities;
using DotNetLabs.Core.Models;
using DotNetLabs.Infrastructure.DbContexts;

namespace DotNetLabs.Application.Services;

public sealed class VoteService : IVoteService
{
    private readonly WatchlyDbContext _dbContext;

    public VoteService(WatchlyDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Result> VoteAsync(long titleId, short value, Guid userId, CancellationToken ct)
    {
        try
        {
            ct.ThrowIfCancellationRequested();
            var user = await _dbContext.Users.FindAsync([userId], ct);
            if (user is null)
            {
                return Result.Fail("No user");
            }

            var vote = new Vote
            {
                UpdatedAt = DateTime.UtcNow,
                TitleId = titleId,
                UserId = userId,
                Value = value
            };
            await _dbContext.AddAsync(vote, ct);
            await _dbContext.SaveChangesAsync(ct);

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
            return Result.Fail($"{e.Message}");
        }
    }

    public async Task<Result> ChangeVoteAsync(long principalId, short value, Guid userId, CancellationToken ct)
    {
        try
        {
            ct.ThrowIfCancellationRequested();
            var user = await _dbContext.Users.FindAsync([userId], ct);
            if (user is null)
            {
                return Result.Fail("No user");
            }

            var vote = await _dbContext.Votes.FindAsync([principalId], ct);
            if (vote is null)
            {
                return Result.Fail("No existing vote");
            }

            if (vote.UserId != userId)
            {
                return Result.Fail("Not user's vote");
            }

            vote.Value = value;
            vote.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(ct);

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
            return Result.Fail($"{e.Message}");
        }
    }
}
