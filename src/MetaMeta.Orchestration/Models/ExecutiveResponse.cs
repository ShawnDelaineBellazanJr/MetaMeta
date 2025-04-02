using System.Collections.Generic;
using MetaMeta.Core.Models;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents a response from the executive agent.
/// </summary>
public class ExecutiveResponse : AgentResponse
{
    /// <summary>
    /// Gets or sets the original goal that was processed.
    /// </summary>
    public string Goal { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the summary of the execution results.
    /// </summary>
    public string Summary { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the detailed narrative of the execution process.
    /// </summary>
    public string Narrative { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the list of execution steps that were performed.
    /// </summary>
    public List<ExecutionStepResult> Steps { get; set; } = new List<ExecutionStepResult>();
    
    /// <summary>
    /// Gets or sets the execution statistics.
    /// </summary>
    public ExecutionStats Stats { get; set; } = new ExecutionStats();
    
    /// <summary>
    /// Gets or sets the execution plan that was generated.
    /// </summary>
    public PlanResponse? ExecutionPlan { get; set; }
    
    /// <summary>
    /// Gets or sets the summary of step executions.
    /// </summary>
    public List<string> ExecutionSummary { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the integrated result from all executed steps.
    /// </summary>
    public string Result { get; set; } = string.Empty;
}

/// <summary>
/// Represents the result of a single execution step.
/// </summary>
public class ExecutionStepResult
{
    /// <summary>
    /// Gets or sets the step number.
    /// </summary>
    public int StepNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the description of the step.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the agent that performed the step.
    /// </summary>
    public string Agent { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the step was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the output of the step.
    /// </summary>
    public string Output { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the error message if the step failed.
    /// </summary>
    public string? Error { get; set; }
}

/// <summary>
/// Represents statistics about the execution.
/// </summary>
public class ExecutionStats
{
    /// <summary>
    /// Gets or sets the total number of steps in the execution plan.
    /// </summary>
    public int TotalSteps { get; set; }
    
    /// <summary>
    /// Gets or sets the number of steps that completed successfully.
    /// </summary>
    public int SuccessfulSteps { get; set; }
    
    /// <summary>
    /// Gets or sets the number of steps that failed.
    /// </summary>
    public int FailedSteps { get; set; }
    
    /// <summary>
    /// Gets or sets the total execution time in milliseconds.
    /// </summary>
    public long ExecutionTimeMs { get; set; }
} 