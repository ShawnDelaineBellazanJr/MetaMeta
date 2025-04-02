using System;
using System.IO;
using System.Threading.Tasks;
using MetaMeta.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace MetaMeta.Orchestration.Prompts;

/// <summary>
/// Utility class for loading prompt templates from files.
/// </summary>
public class PromptLoader
{
    private readonly IPromptTemplateFactory _promptFactory;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the PromptLoader class.
    /// </summary>
    /// <param name="promptFactory">The prompt template factory.</param>
    /// <param name="logger">The logger.</param>
    public PromptLoader(IPromptTemplateFactory promptFactory, ILogger logger)
    {
        _promptFactory = promptFactory ?? throw new ArgumentNullException(nameof(promptFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Loads a prompt template from a file.
    /// </summary>
    /// <param name="agentName">The name of the agent to load the prompt for.</param>
    /// <param name="promptName">Optional specific prompt name if different from agent name.</param>
    /// <returns>The loaded prompt template.</returns>
    public async Task<IPromptTemplate> LoadPromptAsync(string agentName, string? promptName = null)
    {
        promptName ??= agentName;
        
        try
        {
            // Look for the prompt file in the Prompts directory
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string promptsDirectory = Path.Combine(basePath, "Prompts");
            
            // First try direct project path
            string filePath = Path.Combine(promptsDirectory, $"{promptName}.prompty");
            
            // If not found, try going up to src directory and look in Orchestration/Prompts
            if (!File.Exists(filePath))
            {
                string srcDirectory = Path.GetFullPath(Path.Combine(basePath, "..", "..", ".."));
                filePath = Path.Combine(srcDirectory, "MetaMeta.Orchestration", "Prompts", $"{promptName}.prompty");
            }
            
            _logger.LogDebug("Loading prompt from: {FilePath}", filePath);
            
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Prompt file not found: {FilePath}", filePath);
                throw new FileNotFoundException($"Prompt file not found for agent: {agentName}", filePath);
            }
            
            string templateContent = await File.ReadAllTextAsync(filePath);
            
            var config = new PromptTemplateConfig
            {
                Name = promptName,
                Template = templateContent,
                Description = $"Prompt template for {agentName}"
            };
            
            return _promptFactory.Create(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load prompt for agent: {AgentName}", agentName);
            throw;
        }
    }
} 