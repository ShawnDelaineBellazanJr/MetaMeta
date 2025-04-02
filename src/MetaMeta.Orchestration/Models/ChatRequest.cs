using MetaMeta.Core.Models;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Request model for chat completion operations.
/// </summary>
public class ChatRequest : AgentRequest
{
    /// <summary>
    /// Gets or sets the user's message to process.
    /// </summary>
    public required string Message { get; set; }
    
    /// <summary>
    /// Gets or sets the persona to use for the chat response.
    /// </summary>
    /// <remarks>
    /// Supported values include: "technical", "creative", "professional", "concise".
    /// Default generic persona used if not specified.
    /// </remarks>
    public string Persona { get; set; } = "default";
    
    /// <summary>
    /// Gets or sets the custom system prompt to override the default.
    /// </summary>
    public string? SystemPrompt { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum number of previous messages to include in context.
    /// </summary>
    /// <remarks>
    /// Controls context window utilization. Default is 10 if not specified.
    /// </remarks>
    public int MaxHistoryLength { get; set; } = 10;
} 