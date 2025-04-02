using MetaMeta.Core.Models;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents a request to the executive agent.
/// </summary>
public class ExecutiveRequest : AgentRequest
{
    /// <summary>
    /// Gets or sets the primary goal or directive.
    /// </summary>
    public string Goal { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets additional context or requirements for achieving the goal.
    /// </summary>
    public string? Context { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum execution time in seconds.
    /// </summary>
    public int MaxExecutionTimeSeconds { get; set; } = 300;
    
    /// <summary>
    /// Gets or sets the constraints to apply during execution.
    /// </summary>
    public string[]? Constraints { get; set; }
    
    /// <summary>
    /// Gets or sets the desired format for the response.
    /// </summary>
    public string ResponseFormat { get; set; } = "text";
} 