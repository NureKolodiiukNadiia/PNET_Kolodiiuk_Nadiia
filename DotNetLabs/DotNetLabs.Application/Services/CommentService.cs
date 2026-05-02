using System.Data.Common;
using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Models.Auth;
using DotNetLabs.Application.Models.Comments;
using DotNetLabs.Core.Entities;
using DotNetLabs.Core.Models;
using DotNetLabs.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DotNetLabs.Application.Services;

public sealed class CommentService : ICommentService
{
    private readonly AppDbContext _dbContext;

    public CommentService(AppDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Result<IEnumerable<CommentDto>>> GetAllCommentsPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var query = _dbContext.Comments
                .Include(c => c.User)
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CommentDto(
                    c.Id,
                    c.TitleId,
                    true,
                    c.UserId,
                    c.UpdatedAt,
                    c.Text,
                    new UserDto
                    {
                        Id = c.User.Id,
                        Email = c.User.Email,
                        UserName = c.User.UserName
                    }));
            var comments = await query.AsNoTracking().ToListAsync(ct);

            return Result.Success<IEnumerable<CommentDto>>(comments);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (DbException e)
        {
            return Result.Fail<IEnumerable<CommentDto>>($"DB problems: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail<IEnumerable<CommentDto>>($"Error: {e.Message}");
        }
    }

    public async Task<Result> LeaveCommentAsync(LeaveCommentRequest request, Guid userId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
            if (user is null)
            {
                return Result.Fail("user is not found");
            }

            var comment = new Comment
            {
                TitleId = request.TitleId,
                Text = request.Text,
                IsDeleted = false,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId,
            };
            await _dbContext.AddAsync(comment, ct);
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

    public async Task<Result> UpdateCommentAsync(long commentId, string text, Guid userId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
            if (user is null)
            {
                return Result.Fail("user is not found");
            }

            var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId, ct);
            if (comment is null)
            {
                return Result.Fail("comment is not found");
            }

            if (comment.UserId != userId)
            {
                return Result.Fail("user doesn't own comment");
            }

            comment.Text = text;
            comment.UpdatedAt = DateTime.UtcNow;
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

    public async Task<Result> DeleteCommentAsync(long commentId, Guid userId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
            if (user is null)
            {
                return Result.Fail("user is not found");
            }

            var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId, ct);
            if (comment is null)
            {
                return Result.Fail("comment is not found");
            }

            if (comment.UserId != userId)
            {
                return Result.Fail("user doesn't own comment");
            }

            comment.IsDeleted = true;
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

    public async Task<Result<CommentDto>> GetCommentByIdAsync(long commentId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var comment = await _dbContext.Comments
                .Include(c => c.User)
                .Where(c => c.Id == commentId)
                .Select(c => new CommentDto(
                    c.Id,
                    c.TitleId,
                    true,
                    c.UserId,
                    c.UpdatedAt,
                    c.Text,
                    new UserDto
                    {
                        Id = c.User.Id,
                        Email = c.User.Email,
                        UserName = c.User.UserName
                    }))
                .FirstOrDefaultAsync(ct);

            if (comment is null)
            {
                return Result.Fail<CommentDto>("Comment not found");
            }

            return Result.Success(comment);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (DbException e)
        {
            return Result.Fail<CommentDto>($"DB problems: {e.Message}");
        }
        catch (Exception e)
        {
            return Result.Fail<CommentDto>($"Error: {e.Message}");
        }
    }
}
