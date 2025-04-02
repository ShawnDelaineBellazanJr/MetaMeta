using MetaMeta.Core.Models;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents a response from content generation.
/// </summary>
public class ContentResponse : AgentResponse
{
    /// <summary>
    /// Gets or sets the generated content.
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the type of content that was generated.
    /// </summary>
    public ContentType ContentType { get; set; } = ContentType.Text;
    
    /// <summary>
    /// Gets or sets the format of the generated content.
    /// </summary>
    public string Format { get; set; } = "plain";
    
    /// <summary>
    /// Gets or sets the length of the generated content.
    /// </summary>
    public int ContentLength { get; set; }
    
    /// <summary>
    /// Gets or sets additional metadata about the generated content.
    /// </summary>
    public new Dictionary<string, string>? Metadata { get; set; }
} 