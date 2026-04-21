using DotNetLabs.Application.Interfaces;
using DotNetLabs.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetLabs.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IVoteService, VoteService>();
    }
}
