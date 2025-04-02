using System;
using System.Threading;
using System.Threading.Tasks;
using MetaMeta.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace MetaMeta.Orchestration.Agents;

/// <summary>
/// Base class for all agent implementations that provides common functionality.
/// </summary>
/// <typeparam name="TRequest">The request type for the agent.</typeparam>
/// <typeparam name="TResponse">The response type for the agent.</typeparam>
public abstract class AgentBase<TRequest, TResponse>
    where TRequest : AgentRequest
    where TResponse : AgentResponse, new()
{
    /// <summary>
    /// Gets the Semantic Kernel instance.
    /// </summary>
    protected Kernel Kernel { get; }
    
    /// <summary>
    /// Gets the logger for the agent.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Initializes a new instance of the AgentBase class.
    /// </summary>
    /// <param name="kernel">The semantic kernel instance.</param>
    /// <param name="logger">The logger for agent operations.</param>
    protected AgentBase(Kernel kernel, ILogger logger)
    {
        // Step 1: Store dependencies
        Kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Step 2: Initialize the agent
        InitializeAgent();
    }
    
    /// <summary>
    /// Executes the agent operation with the specified request.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the agent operation.</returns>
    public abstract Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Initializes the agent with the kernel.
    /// </summary>
    private void InitializeAgent()
    {
        try
        {
            // Log successful initialization
            Logger.LogInformation("Agent '{AgentName}' initialized successfully", GetAgentName());
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to initialize agent '{AgentName}'", GetAgentName());
            throw new InvalidOperationException($"Failed to initialize agent: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Gets the name of the agent.
    /// </summary>
    /// <returns>The agent name.</returns>
    protected abstract string GetAgentName();
    
    /// <summary>
    /// Gets the instructions for the agent.
    /// </summary>
    /// <returns>The agent instructions.</returns>
    protected abstract string GetAgentInstructions();
    
    /// <summary>
    /// Gets the description of the agent.
    /// </summary>
    /// <returns>The agent description.</returns>
    protected abstract string GetAgentDescription();
    
    /// <summary>
    /// Logs a step in the agent's execution process.
    /// </summary>
    /// <param name="stepNumber">The step number.</param>
    /// <param name="description">The step description.</param>
    protected void LogStep(int stepNumber, string description)
    {
        Logger.LogDebug("Agent '{AgentName}' - Step {StepNumber}: {Description}", 
            GetAgentName(), stepNumber, description);
    }
} 