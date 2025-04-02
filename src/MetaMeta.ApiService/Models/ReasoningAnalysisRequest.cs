using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MetaMeta.ApiService.Models;

/// <summary>
/// Request model for the reasoning analysis API endpoint.
/// </summary>
public class ReasoningAnalysisRequest
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
    /// Gets or sets the assistant name to use for reasoning.
    /// </summary>
    public string? Assistant { get; set; }
    
    /// <summary>
    /// Gets or sets the problem to analyze through reasoning.
    /// </summary>
    public required string Problem { get; set; }
    
    /// <summary>
    /// Gets or sets the reasoning style to use (analytical, creative, critical, strategic, scientific).
    /// </summary>
    public string? Style { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum number of reasoning steps.
    /// </summary>
    public int? MaxSteps { get; set; }
    
    /// <summary>
    /// Gets or sets whether to include alternative perspectives.
    /// </summary>
    public bool? IncludeAlternatives { get; set; }
    
    /// <summary>
    /// Gets or sets additional metadata for the reasoning request.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
} 