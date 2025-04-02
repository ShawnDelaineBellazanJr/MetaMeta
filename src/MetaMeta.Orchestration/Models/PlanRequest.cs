using MetaMeta.Core.Models;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Request model for plan generation operations.
/// </summary>
public class PlanRequest : AgentRequest
{
    /// <summary>
    /// Gets or sets the goal to plan for.
    /// </summary>
    public required string Goal { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum number of steps to include in the plan.
    /// </summary>
    public int MaxSteps { get; set; } = 10;
    
    /// <summary>
    /// Gets or sets a value indicating whether parallel execution of steps is allowed.
    /// </summary>
    public bool AllowParallelExecution { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the constraints to consider when generating the plan.
    /// </summary>
    public string[]? Constraints { get; set; }
} 