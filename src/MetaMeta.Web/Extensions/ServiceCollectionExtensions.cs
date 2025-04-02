using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MetaMeta.Web.Extensions;

/// <summary>
/// Extension methods for service collection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Redis output cache to the service collection
    /// </summary>
    /// <param name="builder">The web application builder</param>
    /// <param name="connectionName">The connection name for Redis</param>
    /// <returns>The web application builder</returns>
    public static WebApplicationBuilder AddRedisOutputCache(this WebApplicationBuilder builder, string connectionName)
    {
        builder.Services.AddOutputCache(options =>
        {
            options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(5)));
        });

        // Add Redis caching for production environments
        if (builder.Environment.IsProduction())
        {
            // In production, we'd set up Redis
            // For development, just use default memory cache
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString(connectionName) 
                    ?? "localhost:6379";
            });
        }

        return builder;
    }
} 