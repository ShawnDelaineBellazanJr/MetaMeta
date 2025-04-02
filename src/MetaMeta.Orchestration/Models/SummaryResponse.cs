using MetaMeta.Core.Models;
using System.Collections.Generic;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Response model for content summarization operations.
/// </summary>
public class SummaryResponse : AgentResponse
{
    /// <summary>
    /// Gets or sets the type of content that was summarized.
    /// </summary>
    public string ContentType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the generated summary.
    /// </summary>
    public string Summary { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the list of key points extracted from the content.
    /// </summary>
    public List<string> KeyPoints { get; set; } = new();
} 