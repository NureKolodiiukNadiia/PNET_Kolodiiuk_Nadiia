using System.Data.Common;
using DotNetLabs.Application.Interfaces;
using DotNetLabs.Core.Entities;
using DotNetLabs.Core.Models;
using DotNetLabs.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DotNetLabs.Application.Services;

public sealed class VoteService : IVoteService
{
    private readonly AppDbContext _dbContext;

    public VoteService(AppDbContext context)
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

    public async Task<Result> ChangeVoteAsync(long principalId, short value, Guid? userId, CancellationToken ct)
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
    public async Task<Result<IEnumerable<VoteDto>>> GetAllVotesPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var query = _dbContext.Votes
                .OrderBy(v => v.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new VoteDto
                {
                    Id = v.Id,
                    TitleId = v.TitleId,
                    UserId = v.UserId,
                    Value = v.Value,
                    UpdatedAt = v.UpdatedAt
                });

            var votes = await query.AsNoTracking().ToListAsync(ct);
            return Result.Success<IEnumerable<VoteDto>>(votes);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result.Fail<IEnumerable<VoteDto>>(e.Message);
        }
    }

    public async Task<Result<VoteDto>> GetVoteDetailsAsync(long titleId, Guid? userId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var vote = await _dbContext.Votes
                .Where(v => v.TitleId == titleId && v.UserId == userId)
                .Select(v => new VoteDto
                {
                    Id = v.Id,
                    TitleId = v.TitleId,
                    UserId = v.UserId,
                    Value = v.Value,
                    UpdatedAt = v.UpdatedAt
                })
                .FirstOrDefaultAsync(ct);

            if (vote == null)
            {
                return Result.Fail<VoteDto>("Vote not found");
            }

            return Result.Success(vote);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result.Fail<VoteDto>(e.Message);
        }
    }

    public async Task<Result> DeleteVoteAsync(long titleId, Guid? userId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var vote = await _dbContext.Votes.FirstOrDefaultAsync(v => v.TitleId == titleId && v.UserId == userId, ct);
            if (vote == null)
            {
                return Result.Fail("Vote not found");
            }

            _dbContext.Votes.Remove(vote);
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
            return Result.Fail(e.Message);
        }
    }
}
