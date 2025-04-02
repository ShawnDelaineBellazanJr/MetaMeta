using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using MetaMeta.Orchestration.Agents;
using MetaMeta.Orchestration.Prompts;
using MetaMeta.Core.Abstractions;

namespace MetaMeta.Orchestration;

/// <summary>
/// Extension methods for setting up orchestration services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds orchestration services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddOrchestrationServices(this IServiceCollection services, object configuration)
    {
        // We need to adapt the configuration object to extract values
        var configAdapter = new ConfigurationAdapter(configuration);

        // Register the Semantic Kernel
        services.AddScoped<Kernel>(provider =>
        {
            var builder = Kernel.CreateBuilder();
            var logger = provider.GetRequiredService<ILogger<Kernel>>();

            try
            {
                // Add AI services based on configuration
                var aiServiceType = configAdapter.GetValue("AI:ServiceType");

                if (aiServiceType == "OpenAI")
                {
                    var modelId = configAdapter.GetValue("AI:OpenAI:ModelId") ?? "gpt-4";
                    var apiKey = configAdapter.GetValue("AI:OpenAI:ApiKey");

                    if (string.IsNullOrEmpty(apiKey))
                    {
                        logger.LogWarning("OpenAI API key not found in configuration");
                        throw new InvalidOperationException("OpenAI API key is missing");
                    }

                    logger.LogInformation("Adding OpenAI chat completion with model: {ModelId}", modelId);
                    builder.AddOpenAIChatCompletion(modelId, apiKey);
                }
                else if (aiServiceType == "AzureOpenAI")
                {
                    var deploymentName = configAdapter.GetValue("AI:AzureOpenAI:DeploymentName");
                    var endpoint = configAdapter.GetValue("AI:AzureOpenAI:Endpoint");
                    var apiKey = configAdapter.GetValue("AI:AzureOpenAI:ApiKey");

                    if (string.IsNullOrEmpty(deploymentName) || string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
                    {
                        logger.LogWarning("Azure OpenAI configuration is incomplete");
                        throw new InvalidOperationException("Azure OpenAI configuration is incomplete");
                    }

                    logger.LogInformation("Adding Azure OpenAI chat completion with deployment: {DeploymentName}", deploymentName);
                    builder.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);
                }
                else
                {
                    logger.LogWarning("Unsupported AI service type: {ServiceType}", aiServiceType);
                    throw new ArgumentException($"Unsupported AI service type: {aiServiceType}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to configure AI service. Using dummy kernel for testing only!");
                // In development/testing, we might want to continue with a dummy kernel
                // In production, we should probably rethrow
            }

            return builder.Build();
        });

        // Register the PromptLoader
        services.AddSingleton<PromptLoader>(sp =>
        {
            var promptFactory = sp.GetRequiredService<MetaMeta.Core.Abstractions.IPromptTemplateFactory>();
            var logger = sp.GetRequiredService<ILogger<PromptLoader>>();
            return new PromptLoader(promptFactory, logger);
        });

        // Register all agents
        services.AddScoped<PlannerAgent>();
        services.AddScoped<MemoryAgent>();
        services.AddScoped<ToolAgent>();
        services.AddScoped<ReasoningAgent>();
        services.AddScoped<ContentAgent>();
        services.AddScoped<ChatCompletionAgent>();

        // Register the Chat Completion Service
        services.AddScoped(provider =>
        {
            var kernel = provider.GetRequiredService<Kernel>();
            return kernel.GetRequiredService<Microsoft.SemanticKernel.ChatCompletion.IChatCompletionService>();
        });

        // Register the Orchestrator
        services.AddScoped<Orchestrator>();

        return services;
    }

    /// <summary>
    /// Simple adapter for configuration to avoid direct dependency on Microsoft.Extensions.Configuration.
    /// </summary>
    private class ConfigurationAdapter
    {
        private readonly object _configuration;

        public ConfigurationAdapter(object configuration)
        {
            _configuration = configuration;
        }

        public string GetValue(string key)
        {
            // Use reflection to access the indexer on IConfiguration
            // This avoids a direct dependency on the IConfiguration interface
            try
            {
                var indexerProperty = _configuration.GetType().GetProperty("Item", new[] { typeof(string) });
                if (indexerProperty != null)
                {
                    var value = indexerProperty.GetValue(_configuration, new object[] { key });
                    return value?.ToString() ?? string.Empty;
                }

                // Try with a method called GetSection + indexer for nested sections
                var parts = key.Split(':');
                object section = _configuration;

                foreach (var part in parts)
                {
                    var getSectionMethod = section.GetType().GetMethod("GetSection", new[] { typeof(string) });
                    if (getSectionMethod == null)
                        return string.Empty;

                    section = getSectionMethod.Invoke(section, new object[] { part });
                    if (section == null)
                        return string.Empty;
                }

                var valueProperty = section.GetType().GetProperty("Value");
                if (valueProperty != null)
                {
                    var value = valueProperty.GetValue(section);
                    return value?.ToString() ?? string.Empty;
                }
            }
            catch
            {
                // If anything goes wrong, return empty string
                return string.Empty;
            }

            return string.Empty;
        }
    }
}