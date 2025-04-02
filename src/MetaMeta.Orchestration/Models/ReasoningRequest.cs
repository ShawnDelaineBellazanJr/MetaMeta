using MetaMeta.Core.Models;
using System.Collections.Generic;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents a request to perform reasoning analysis.
/// </summary>
public class ReasoningRequest : AgentRequest
{
    /// <summary>
    /// Gets or sets the problem statement to analyze.
    /// </summary>
    public string Problem { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the reasoning style to apply.
    /// </summary>
    public ReasoningStyle Style { get; set; } = ReasoningStyle.Analytical;
    
    /// <summary>
    /// Gets or sets the maximum number of reasoning steps.
    /// </summary>
    public int MaxSteps { get; set; } = 5;
    
    /// <summary>
    /// Gets or sets a value indicating whether to include alternative reasoning paths.
    /// </summary>
    public bool IncludeAlternatives { get; set; } = false;
} 