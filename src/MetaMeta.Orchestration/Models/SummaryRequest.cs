using MetaMeta.Core.Models;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Request model for content summarization operations.
/// </summary>
public class SummaryRequest : AgentRequest
{
    /// <summary>
    /// Gets or sets the content to summarize.
    /// </summary>
    public required string Content { get; set; }
    
    /// <summary>
    /// Gets or sets the type of content being summarized.
    /// </summary>
    /// <remarks>
    /// Examples: "document", "conversation", "code", "meeting", etc.
    /// </remarks>
    public string ContentType { get; set; } = "document";
    
    /// <summary>
    /// Gets or sets the maximum length of the summary in characters.
    /// </summary>
    public int MaxLength { get; set; } = 1000;
    
    /// <summary>
    /// Gets or sets the output format for the summary.
    /// </summary>
    /// <remarks>
    /// Examples: "paragraph", "bullets", "markdown", etc.
    /// </remarks>
    public string Format { get; set; } = "paragraph";
    
    /// <summary>
    /// Gets or sets a specific focus or aspect to emphasize in the summary.
    /// </summary>
    public string? Focus { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to include a list of key points.
    /// </summary>
    public bool IncludeKeyPoints { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to disable chunking for large content.
    /// </summary>
    public bool DisableChunking { get; set; } = false;
} 