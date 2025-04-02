using System.Collections.Generic;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents a single step in a plan.
/// </summary>
public class PlanStep
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
    /// Gets or sets the numeric dependencies (step numbers) for this step.
    /// </summary>
    public List<int> Dependencies { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the step objects that this step depends on.
    /// </summary>
    public List<PlanStep> DependsOn { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the outputs or artifacts produced by this step.
    /// </summary>
    public List<string> Outputs { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the complexity of the step.
    /// </summary>
    public PlanStepComplexity Complexity { get; set; } = PlanStepComplexity.Medium;
}

/// <summary>
/// Represents the complexity level of a plan step.
/// </summary>
public enum PlanStepComplexity
{
    /// <summary>
    /// Low complexity, relatively straightforward to implement.
    /// </summary>
    Low,
    
    /// <summary>
    /// Medium complexity, moderate effort to implement.
    /// </summary>
    Medium,
    
    /// <summary>
    /// High complexity, significant effort to implement.
    /// </summary>
    High
} 