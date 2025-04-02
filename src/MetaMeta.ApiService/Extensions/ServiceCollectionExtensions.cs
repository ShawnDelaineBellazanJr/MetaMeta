using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace MetaMeta.ApiService.Extensions;

/// <summary>
/// Extension methods for service collection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds OpenAPI (Swagger) support to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "MetaMeta API",
                Version = "v1",
                Description = "API for the MetaMeta AI IDE"
            });
        });

        return services;
    }
}