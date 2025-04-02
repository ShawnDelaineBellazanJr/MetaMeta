using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MetaMeta.Core.Abstractions;
using MetaMeta.Orchestration.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using MetaMeta.Core.Models;

namespace MetaMeta.Orchestration.Agents;

/// <summary>
/// Executive agent that coordinates complex workflows across multiple specialized agents.
/// </summary>
public class ExecutiveAgent : AgentBase<ExecutiveRequest, ExecutiveResponse>
{
    private readonly PlannerAgent _plannerAgent;
    
    /// <summary>
    /// Initializes a new instance of the ExecutiveAgent class.
    /// </summary>
    /// <param name="kernel">The semantic kernel instance.</param>
    /// <param name="logger">The logger for agent operations.</param>
    /// <param name="plannerAgent">The planner agent for creating execution plans.</param>
    public ExecutiveAgent(
        Kernel kernel,
        ILogger<ExecutiveAgent> logger,
        PlannerAgent plannerAgent)
        : base(kernel, logger)
    {
        _plannerAgent = plannerAgent;
    }
    
    /// <summary>
    /// Gets the name of the agent.
    /// </summary>
    protected override string GetAgentName() => "Executive";
    
    /// <summary>
    /// Gets the description of the agent.
    /// </summary>
    protected override string GetAgentDescription() => "Coordinates complex workflows across multiple specialized agents.";
    
    /// <summary>
    /// Gets the instructions for the agent.
    /// </summary>
    protected override string GetAgentInstructions() => 
        "Break down complex goals into executable plans and coordinate the execution of those plans across specialized agents.";
    
    /// <summary>
    /// Executes the agent operation with the specified request.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the agent operation.</returns>
    public override async Task<ExecutiveResponse> ExecuteAsync(
        ExecutiveRequest request, 
        CancellationToken cancellationToken = default)
    {
        LogStep(1, $"Processing executive request for goal: {request.Goal}");
        
        try
        {
            // Step 1: Create a plan for the goal
            LogStep(2, "Creating execution plan");
            
            var planRequest = new PlanRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                Goal = request.Goal,
                Constraints = request.Constraints,
                MaxSteps = 10
            };
            
            var planResult = await _plannerAgent.ExecuteAsync(planRequest, cancellationToken);
            
            var response = new ExecutiveResponse
            {
                Goal = request.Goal,
                ExecutionPlan = planResult.PlanData,
                Success = planResult.Success
            };
            
            if (!planResult.Success)
            {
                response.ErrorMessage = planResult.ErrorMessage;
                return response;
            }
            
            LogStep(3, $"Plan created with {planResult.PlanData.Steps.Count} steps");
            
            // For now, we'll just return the plan without executing it
            response.Result = "Plan created successfully. Execution not implemented yet.";
            response.ExecutionSummary = new List<string>
            {
                "Execution plan was created but not executed in this demo."
            };
            
            return response;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error processing executive request: {Goal}", request.Goal);
            
            return new ExecutiveResponse
            {
                Goal = request.Goal,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
    
    private void ValidateRequest(ExecutiveRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Goal))
        {
            throw new ArgumentException("Goal cannot be empty", nameof(request.Goal));
        }
        
        if (request.Constraints != null && request.Constraints.Length > 0)
        {
            foreach (var constraint in request.Constraints)
            {
                if (string.IsNullOrWhiteSpace(constraint))
                {
                    throw new ArgumentException("Constraints cannot contain empty strings", nameof(request.Constraints));
                }
            }
        }
    }
} 