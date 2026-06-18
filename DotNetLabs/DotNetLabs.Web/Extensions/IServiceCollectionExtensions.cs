namespace DotNetLabs.Web.Extensions;

internal static class IServiceCollectionExtensions
{
    internal static void RegisterRedisCache(this IServiceCollection services, string connString)
    {
        if (string.IsNullOrWhiteSpace(connString))
        {
            services.AddDistributedMemoryCache();
            return;
        }

        services.AddStackExchangeRedisCache(opts =>
        {
            opts.Configuration = connString;
            opts.InstanceName = "Labs_";
        });
    }
}
