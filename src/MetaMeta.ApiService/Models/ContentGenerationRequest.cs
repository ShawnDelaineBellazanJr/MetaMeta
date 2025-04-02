using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MetaMeta.ApiService.Models;

/// <summary>
/// Request model for the content generation API endpoint.
/// </summary>
public class ContentGenerationRequest
{
    /// <summary>
    /// Gets or sets the unique identifier for tracking the request.
    /// </summary>
    public string? RequestId { get; set; }
    
    /// <summary>
    /// Gets or sets the context session identifier.
    /// </summary>
    public string? SessionId { get; set; }
    
    /// <summary>
    /// Gets or sets the assistant name to use for generation.
    /// </summary>
    public string? Assistant { get; set; }
    
    /// <summary>
    /// Gets or sets the primary prompt for content generation.
    /// </summary>
    public required string Prompt { get; set; }
    
    /// <summary>
    /// Gets or sets the type of content to generate (text, code, document, email, mixed).
    /// </summary>
    public string ContentType { get; set; } = "text";
    
    /// <summary>
    /// Gets or sets detailed specifications for the content.
    /// </summary>
    public string? Specifications { get; set; }
    
    /// <summary>
    /// Gets or sets the target format for the content.
    /// </summary>
    public string? Format { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum length of the generated content.
    /// </summary>
    public int? MaxLength { get; set; }
    
    /// <summary>
    /// Gets or sets the tone for the generated content.
    /// </summary>
    public string? Tone { get; set; }
    
    /// <summary>
    /// Gets or sets additional context for content generation.
    /// </summary>
    public Dictionary<string, string>? AdditionalContext { get; set; }
} 