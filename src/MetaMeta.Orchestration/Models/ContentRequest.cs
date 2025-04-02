using MetaMeta.Core.Models;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents a request to generate content.
/// </summary>
public class ContentRequest : AgentRequest
{
    /// <summary>
    /// Gets or sets the content generation prompt.
    /// </summary>
    public string Prompt { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the type of content to generate.
    /// </summary>
    public ContentType ContentType { get; set; } = ContentType.Text;
    
    /// <summary>
    /// Gets or sets additional specifications for the content.
    /// </summary>
    public string Specifications { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the desired format for the content.
    /// </summary>
    public string Format { get; set; } = "plain";
    
    /// <summary>
    /// Gets or sets the maximum length of the generated content.
    /// </summary>
    public int MaxLength { get; set; } = 1000;
    
    /// <summary>
    /// Gets or sets the desired tone for the content.
    /// </summary>
    public string Tone { get; set; } = "professional";
    
    /// <summary>
    /// Gets or sets additional context for content generation.
    /// </summary>
    public string? AdditionalContext { get; set; }
} 