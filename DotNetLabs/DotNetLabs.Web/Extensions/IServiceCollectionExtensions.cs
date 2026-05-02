namespace DotNetLabs.Web.Extensions;

internal static class IServiceCollectionExtensions
{
    internal static void RegisterRedisCache(this IServiceCollection services, string connString)
    {
        services.AddStackExchangeRedisCache(opts =>
        {
            opts.Configuration = connString;
            opts.InstanceName = "Labs_";
        });
    }
}
