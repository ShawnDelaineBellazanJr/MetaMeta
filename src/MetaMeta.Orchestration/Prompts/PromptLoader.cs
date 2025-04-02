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

    // Define standard prompt folder names
    private static readonly string[] PromptFolders = new[]
    {
        "AgentDesign",
        "ReasoningAgents",
        "ExecutionAgents",
        "MemoryAgents",
        "ContentCreation"
    };

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

            string? filePath = null;

            // Try the organized folder structure first
            foreach (var folder in PromptFolders)
            {
                string folderPath = Path.Combine(promptsDirectory, folder);
                string candidatePath = Path.Combine(folderPath, $"{promptName}.prompty");

                if (File.Exists(candidatePath))
                {
                    filePath = candidatePath;
                    break;
                }
            }

            // If not found in folders, try direct project path
            if (filePath == null)
            {
                filePath = Path.Combine(promptsDirectory, $"{promptName}.prompty");

                // If still not found, try .prompt extension as fallback
                if (!File.Exists(filePath))
                {
                    filePath = Path.Combine(promptsDirectory, $"{promptName}.prompt");
                }
            }

            // If not found locally, try going up to src directory and look in Orchestration/Prompts
            if (filePath == null || !File.Exists(filePath))
            {
                string srcDirectory = Path.GetFullPath(Path.Combine(basePath, "..", "..", ".."));
                string orchestrationPromptsDir = Path.Combine(srcDirectory, "MetaMeta.Orchestration", "Prompts");

                // Try in each subfolder
                foreach (var folder in PromptFolders)
                {
                    string folderPath = Path.Combine(orchestrationPromptsDir, folder);
                    string candidatePath = Path.Combine(folderPath, $"{promptName}.prompty");

                    if (File.Exists(candidatePath))
                    {
                        filePath = candidatePath;
                        break;
                    }
                }

                // Try root prompts folder with both extensions if still not found
                if (filePath == null || !File.Exists(filePath))
                {
                    filePath = Path.Combine(orchestrationPromptsDir, $"{promptName}.prompty");

                    if (!File.Exists(filePath))
                    {
                        filePath = Path.Combine(orchestrationPromptsDir, $"{promptName}.prompt");
                    }
                }
            }

            _logger.LogDebug("Loading prompt from: {FilePath}", filePath);

            if (filePath == null || !File.Exists(filePath))
            {
                _logger.LogWarning("Prompt file not found for agent: {AgentName}", agentName);
                throw new FileNotFoundException($"Prompt file not found for agent: {agentName}", promptName);
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