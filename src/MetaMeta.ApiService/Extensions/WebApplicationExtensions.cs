using Microsoft.AspNetCore.Builder;

namespace MetaMeta.ApiService.Extensions;

/// <summary>
/// Extension methods for WebApplication
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Maps OpenAPI (Swagger) endpoints for the web application
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
    public static WebApplication MapOpenApi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "MetaMeta API v1");
        });

        return app;
    }

    /// <summary>
    /// Configures Swagger documentation for the web application
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "MetaMeta API v1");
            c.RoutePrefix = "swagger";
        });

        return app;
    }
}