using System;
using System.IO;
using System.Threading.Tasks;
using MetaMeta.Core.Abstractions;

namespace MetaMeta.Orchestration.Prompts;

/// <summary>
/// Simple console logger for the PromptLoader.
/// </summary>
public interface ISimpleLogger
{
    void LogDebug(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
}

/// <summary>
/// Default implementation of ISimpleLogger that writes to console.
/// </summary>
public class ConsoleLogger : ISimpleLogger
{
    public void LogDebug(string message, params object[] args)
    {
        Console.WriteLine($"DEBUG: {string.Format(message, args)}");
    }

    public void LogWarning(string message, params object[] args)
    {
        Console.WriteLine($"WARNING: {string.Format(message, args)}");
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        Console.WriteLine($"ERROR: {string.Format(message, args)}");
        Console.WriteLine($"EXCEPTION: {exception}");
    }
}

/// <summary>
/// Utility class for loading prompt templates from files.
/// </summary>
public class PromptLoader
{
    private readonly IPromptTemplateFactory _promptFactory;
    private readonly ISimpleLogger _logger;

    /// <summary>
    /// Initializes a new instance of the PromptLoader class.
    /// </summary>
    /// <param name="promptFactory">The prompt template factory.</param>
    /// <param name="logger">The logger.</param>
    public PromptLoader(IPromptTemplateFactory promptFactory, ISimpleLogger logger = null)
    {
        _promptFactory = promptFactory ?? throw new ArgumentNullException(nameof(promptFactory));
        _logger = logger ?? new ConsoleLogger();
    }

    /// <summary>
    /// Loads a prompt template from a file.
    /// </summary>
    /// <param name="agentName">The name of the agent to load the prompt for.</param>
    /// <param name="promptName">Optional specific prompt name if different from agent name.</param>
    /// <param name="category">Optional category for Library prompts.</param>
    /// <returns>The loaded prompt template.</returns>
    public async Task<IPromptTemplate> LoadPromptAsync(string agentName, string? promptName = null, string? category = null)
    {
        promptName ??= agentName;

        try
        {
            // Look for the prompt file in the Prompts directory
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string promptsDirectory = Path.Combine(basePath, "Prompts");
            string filePath = null;

            // First check if it's a Library prompt
            if (!string.IsNullOrEmpty(category))
            {
                // Check in Library structure
                filePath = Path.Combine(promptsDirectory, "Library", category, $"{promptName}.prompty");

                // If not found in bin directory, try source directory
                if (!File.Exists(filePath))
                {
                    string srcDirectory = Path.GetFullPath(Path.Combine(basePath, "..", "..", ".."));
                    filePath = Path.Combine(srcDirectory, "MetaMeta.Orchestration", "Prompts", "Library", category, $"{promptName}.prompty");
                }
            }
            else
            {
                // Try direct project path first
                filePath = Path.Combine(promptsDirectory, $"{promptName}.prompty");

                // If not found, try .prompt extension
                if (!File.Exists(filePath))
                {
                    filePath = Path.Combine(promptsDirectory, $"{promptName}.prompt");
                }

                // If not found in bin directory, try source directory
                if (!File.Exists(filePath))
                {
                    string srcDirectory = Path.GetFullPath(Path.Combine(basePath, "..", "..", ".."));
                    filePath = Path.Combine(srcDirectory, "MetaMeta.Orchestration", "Prompts", $"{promptName}.prompty");

                    // If still not found, try .prompt extension
                    if (!File.Exists(filePath))
                    {
                        filePath = Path.Combine(srcDirectory, "MetaMeta.Orchestration", "Prompts", $"{promptName}.prompt");
                    }

                    // As a last resort, search in the Library structure
                    if (!File.Exists(filePath))
                    {
                        string[] categories = new[] { "AgentDesign", "ContentCreation", "FeedbackAnalytics", "Marketing", "Monetization", "ProductDevelopment", "Sales", "TechnicalDocs" };
                        foreach (var cat in categories)
                        {
                            var libraryPath = Path.Combine(srcDirectory, "MetaMeta.Orchestration", "Prompts", "Library", cat, $"{promptName}.prompty");
                            if (File.Exists(libraryPath))
                            {
                                filePath = libraryPath;
                                break;
                            }
                        }
                    }
                }
            }

            _logger.LogDebug("Loading prompt from: {FilePath}", filePath);

            if (filePath == null || !File.Exists(filePath))
            {
                _logger.LogWarning("Prompt file not found: {FilePath}", filePath ?? "(null)");
                throw new FileNotFoundException($"Prompt file not found for agent: {agentName}", filePath ?? promptName);
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