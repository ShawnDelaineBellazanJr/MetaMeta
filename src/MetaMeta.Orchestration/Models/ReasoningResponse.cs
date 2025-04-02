using MetaMeta.Core.Models;
using System.Collections.Generic;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents a response from a reasoning analysis.
/// </summary>
public class ReasoningResponse : AgentResponse
{
    /// <summary>
    /// Gets or sets the reasoning conclusion.
    /// </summary>
    public string Conclusion { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the step-by-step reasoning process.
    /// </summary>
    public List<ReasoningStep> Steps { get; set; } = new List<ReasoningStep>();
    
    /// <summary>
    /// Gets or sets the confidence level in the conclusion (0-100).
    /// </summary>
    public int ConfidenceScore { get; set; }
    
    /// <summary>
    /// Gets or sets alternative reasoning paths if requested.
    /// </summary>
    public List<AlternativeReasoning>? Alternatives { get; set; }
    
    /// <summary>
    /// Gets or sets the style of reasoning that was applied.
    /// </summary>
    public ReasoningStyle Style { get; set; }
}

/// <summary>
/// Represents a single step in the reasoning process.
/// </summary>
public class ReasoningStep
{
    /// <summary>
    /// Gets or sets the step number.
    /// </summary>
    public int StepNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the description of the reasoning step.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the reasoning applied in this step.
    /// </summary>
    public string Reasoning { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets any intermediate conclusions from this step.
    /// </summary>
    public string? IntermediateConclusion { get; set; }
}

/// <summary>
/// Represents an alternative reasoning path.
/// </summary>
public class AlternativeReasoning
{
    /// <summary>
    /// Gets or sets the alternative conclusion.
    /// </summary>
    public string Conclusion { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a brief summary of the alternative reasoning.
    /// </summary>
    public string Reasoning { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the confidence level in this alternative (0-100).
    /// </summary>
    public int ConfidenceScore { get; set; }
} 