using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DotNetLabs.Infrastructure.DbContexts;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<WatchlyDbContext>
{
    private static readonly string PathDelimiter = Environment.OSVersion.Platform == PlatformID.Unix ? "/" : "\\";

    public WatchlyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WatchlyDbContext>();
        var basePath = $"{Directory.GetCurrentDirectory()}{PathDelimiter}..{PathDelimiter}..{PathDelimiter}DotNetLabs.Web";
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile($"appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(connectionString);

        return new WatchlyDbContext(optionsBuilder.Options);
    }
}
