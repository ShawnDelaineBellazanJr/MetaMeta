using MetaMeta.Core.Models;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Wrapper class to make PlanResponse compatible with the AgentResponse base class for agent operations.
/// </summary>
public class PlanResponseWrapper : AgentResponse
{
    /// <summary>
    /// Gets or sets the underlying plan response data.
    /// </summary>
    public PlanResponse PlanData { get; set; } = new PlanResponse();
} 