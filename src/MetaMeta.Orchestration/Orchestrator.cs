using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MetaMeta.Orchestration.Agents;
using MetaMeta.Orchestration.Models;
using Microsoft.SemanticKernel;
using MetaMeta.Core.Chat;

namespace MetaMeta.Orchestration;

/// <summary>
/// Orchestrates the interaction between various specialized agents.
/// </summary>
public class Orchestrator
{
    private readonly ILogger<Orchestrator> _logger;
    private readonly Kernel _kernel;
    private readonly PlannerAgent _plannerAgent;
    private readonly MemoryAgent _memoryAgent;
    private readonly ToolAgent _toolAgent;
    private readonly ReasoningAgent _reasoningAgent;
    private readonly ContentAgent _contentAgent;
    private readonly ChatCompletionAgent _chatCompletionAgent;
    
    /// <summary>
    /// Initializes a new instance of the Orchestrator class.
    /// </summary>
    /// <param name="logger">The logger for orchestration operations.</param>
    /// <param name="kernel">The Semantic Kernel instance.</param>
    /// <param name="plannerAgent">The planner agent.</param>
    /// <param name="memoryAgent">The memory agent.</param>
    /// <param name="toolAgent">The tool agent.</param>
    /// <param name="reasoningAgent">The reasoning agent.</param>
    /// <param name="contentAgent">The content agent.</param>
    /// <param name="chatCompletionAgent">The chat completion agent.</param>
    public Orchestrator(
        ILogger<Orchestrator> logger,
        Kernel kernel,
        PlannerAgent plannerAgent,
        MemoryAgent memoryAgent,
        ToolAgent toolAgent,
        ReasoningAgent reasoningAgent,
        ContentAgent contentAgent,
        ChatCompletionAgent chatCompletionAgent)
    {
        _logger = logger;
        _kernel = kernel;
        _plannerAgent = plannerAgent;
        _memoryAgent = memoryAgent;
        _toolAgent = toolAgent;
        _reasoningAgent = reasoningAgent;
        _contentAgent = contentAgent;
        _chatCompletionAgent = chatCompletionAgent;
    }
    
    /// <summary>
    /// Processes a high-level goal by creating and executing a plan using the appropriate agents.
    /// </summary>
    /// <param name="goal">The goal to achieve.</param>
    /// <param name="context">Optional context information to guide the process.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of processing the goal.</returns>
    public async Task<OrchestratorResult> ProcessGoalAsync(
        string goal,
        Dictionary<string, string>? context = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing goal: {Goal}", goal);
        
        try
        {
            // Step 1: Create a plan request
            var planRequest = new PlanRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                Goal = goal,
                Constraints = context != null && context.ContainsKey("constraints") 
                    ? new[] { context["constraints"] } 
                    : null,
                MaxSteps = context != null && context.TryGetValue("maxSteps", out var maxSteps) && int.TryParse(maxSteps, out var maxStepsValue) 
                    ? maxStepsValue : 10
            };
            
            // Step 2: Generate the execution plan
            _logger.LogInformation("Generating execution plan");
            var planResult = await _plannerAgent.ExecuteAsync(planRequest, cancellationToken);
            
            if (!planResult.Success)
            {
                throw new InvalidOperationException($"Failed to generate plan: {planResult.ErrorMessage}");
            }
            
            _logger.LogInformation("Plan generated with {StepCount} steps", planResult.PlanData.Steps.Count);
            
            // Step 3: Execute the plan steps
            var results = new List<StepExecutionResult>();
            var executionErrors = new List<string>();
            
            _logger.LogInformation("Executing plan steps");
            foreach (var step in planResult.PlanData.Steps)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                
                // Check if all dependencies are complete
                if (step.Dependencies.Count > 0)
                {
                    bool allDependenciesMet = true;
                    foreach (var depNum in step.Dependencies)
                    {
                        var depStep = planResult.PlanData.Steps.FirstOrDefault(s => s.StepNumber == depNum);
                        if (depStep == null || !results.Any(r => r.StepNumber == depNum && r.Success))
                        {
                            allDependenciesMet = false;
                            break;
                        }
                    }
                    
                    if (!allDependenciesMet)
                    {
                        executionErrors.Add($"Step {step.StepNumber}: Skipped due to unmet dependencies");
                        continue;
                    }
                }
                
                // Execute the step
                _logger.LogInformation("Executing step {StepNumber}: {Description}", step.StepNumber, step.Description);
                
                try
                {
                    var stepResult = await ExecuteStepAsync(step, results, context, cancellationToken);
                    results.Add(stepResult);
                    
                    if (!stepResult.Success)
                    {
                        executionErrors.Add($"Step {step.StepNumber}: {stepResult.Error}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing step {StepNumber}", step.StepNumber);
                    results.Add(new StepExecutionResult
                    {
                        StepNumber = step.StepNumber,
                        Description = step.Description,
                        Success = false,
                        Error = ex.Message
                    });
                    executionErrors.Add($"Step {step.StepNumber}: {ex.Message}");
                }
            }
            
            // Step 4: Prepare final result
            bool overallSuccess = executionErrors.Count == 0;
            string summary = await GenerateSummaryAsync(goal, planResult.PlanData, results, cancellationToken);
            
            return new OrchestratorResult
            {
                Goal = goal,
                Success = overallSuccess,
                PlanStepCount = planResult.PlanData.Steps.Count,
                CompletedStepCount = results.Count(r => r.Success),
                Summary = summary,
                StepResults = results,
                Errors = executionErrors
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing goal: {Goal}", goal);
            
            return new OrchestratorResult
            {
                Goal = goal,
                Success = false,
                Summary = $"Failed to process goal: {ex.Message}",
                Errors = new List<string> { ex.Message }
            };
        }
    }
    
    /// <summary>
    /// Executes a single step in the plan using the appropriate agent.
    /// </summary>
    /// <param name="step">The step to execute.</param>
    /// <param name="previousResults">Results from previously executed steps.</param>
    /// <param name="context">Additional context for execution.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the step execution.</returns>
    private async Task<StepExecutionResult> ExecuteStepAsync(
        PlanStep step, 
        List<StepExecutionResult> previousResults,
        Dictionary<string, string>? context,
        CancellationToken cancellationToken)
    {
        // Determine which agent should handle this step based on the step's description
        // This is a simplified approach - a more sophisticated approach might involve 
        // semantic matching or more structured step metadata
        string stepText = step.Description.ToLower();
        
        try
        {
            // Handle memory operations
            if (stepText.Contains("remember") || stepText.Contains("retrieve") || stepText.Contains("recall") || 
                stepText.Contains("store") || stepText.Contains("save to memory") || stepText.Contains("search memory"))
            {
                string collection = "default";
                string key = $"step-{step.StepNumber}";
                string content = step.Description;
                
                if (stepText.Contains("store") || stepText.Contains("save"))
                {
                    // Extract or generate content to store
                    var valueToStore = step.Description;
                    if (previousResults.Count > 0)
                    {
                        // Use the output from the most recent successful step
                        var lastResult = previousResults.LastOrDefault(r => r.Success);
                        if (lastResult != null && !string.IsNullOrEmpty(lastResult.Output))
                        {
                            valueToStore = lastResult.Output;
                        }
                    }
                    
                    await _memoryAgent.StoreAsync(collection, key, valueToStore);
                    return new StepExecutionResult
                    {
                        StepNumber = step.StepNumber,
                        Description = step.Description,
                        Agent = "Memory",
                        Operation = "Store",
                        Output = $"Stored information with key '{key}'",
                        Success = true
                    };
                }
                else
                {
                    // Retrieve from memory
                    var retrievedValue = await _memoryAgent.RetrieveAsync(collection, key);
                    return new StepExecutionResult
                    {
                        StepNumber = step.StepNumber,
                        Description = step.Description,
                        Agent = "Memory",
                        Operation = "Retrieve",
                        Output = retrievedValue ?? "No information found",
                        Success = retrievedValue != null
                    };
                }
            }
            // Handle reasoning and analysis
            else if (stepText.Contains("analyze") || stepText.Contains("reason") || stepText.Contains("evaluate") || 
                     stepText.Contains("compare") || stepText.Contains("determine"))
            {
                var reasoningResult = await _reasoningAgent.AnalyzeProblemAsync(step.Description);
                return new StepExecutionResult
                {
                    StepNumber = step.StepNumber,
                    Description = step.Description,
                    Agent = "Reasoning",
                    Operation = "Analyze",
                    Output = reasoningResult.ReasoningProcess,
                    Success = reasoningResult.Success,
                    Error = reasoningResult.ErrorMessage
                };
            }
            // Handle content generation and transformation
            else if (stepText.Contains("generate") || stepText.Contains("create content") || stepText.Contains("write") || 
                     stepText.Contains("draft") || stepText.Contains("summarize") || stepText.Contains("translate"))
            {
                string topic = step.Description;
                string format = "text";
                string tone = "neutral";
                
                // Extract format if specified
                if (context != null && context.TryGetValue("format", out var formatValue))
                {
                    format = formatValue;
                }
                
                // Extract tone if specified
                if (context != null && context.TryGetValue("tone", out var toneValue))
                {
                    tone = toneValue;
                }
                
                // Extract length if specified
                int length = 500; // Default length
                if (context != null && context.TryGetValue("length", out var lengthValue) && 
                    int.TryParse(lengthValue, out var parsedLength))
                {
                    length = parsedLength;
                }
                
                if (stepText.Contains("summarize"))
                {
                    // Get content to summarize from previous steps
                    string contentToSummarize = string.Empty;
                    if (previousResults.Count > 0)
                    {
                        // Use the output from the most recent successful step
                        var lastResult = previousResults.LastOrDefault(r => r.Success);
                        if (lastResult != null && !string.IsNullOrEmpty(lastResult.Output))
                        {
                            contentToSummarize = lastResult.Output;
                        }
                    }
                    
                    var transformParams = new Dictionary<string, string>
                    {
                        ["maxLength"] = length.ToString()
                    };
                    
                    var transformResult = await _contentAgent.TransformContentAsync(
                        contentToSummarize, "summarize", transformParams, cancellationToken);
                    
                    return new StepExecutionResult
                    {
                        StepNumber = step.StepNumber,
                        Description = step.Description,
                        Agent = "Content",
                        Operation = "Summarize",
                        Output = transformResult.TransformedContent,
                        Success = transformResult.Success,
                        Error = transformResult.ErrorMessage
                    };
                }
                else
                {
                    // Generate new content
                    var generationResult = await _contentAgent.GenerateContentAsync(
                        topic, format, tone, length, null, cancellationToken);
                    
                    return new StepExecutionResult
                    {
                        StepNumber = step.StepNumber,
                        Description = step.Description,
                        Agent = "Content",
                        Operation = "Generate",
                        Output = generationResult.Content,
                        Success = generationResult.Success,
                        Error = generationResult.ErrorMessage
                    };
                }
            }
            // Handle tool execution
            else if (stepText.Contains("execute") || stepText.Contains("run") || stepText.Contains("calculate") || 
                     stepText.Contains("lookup") || stepText.Contains("search"))
            {
                // This is a simplified approach - in reality, you'd parse the step description
                // to determine which tool to use and with what parameters
                var availableTools = _toolAgent.GetAvailableTools();
                var suggestedTools = await _toolAgent.SuggestToolsAsync(step.Description, 1);
                
                if (suggestedTools.Count > 0)
                {
                    var tool = suggestedTools[0];
                    var parameters = new Dictionary<string, string>();
                    
                    // Add any parameters from context
                    if (context != null)
                    {
                        foreach (var param in tool.Parameters.Where(p => p.IsRequired))
                        {
                            if (context.TryGetValue(param.Name, out var value))
                            {
                                parameters.Add(param.Name, value);
                            }
                            else
                            {
                                // Generate a placeholder value
                                parameters.Add(param.Name, "auto-generated-value");
                            }
                        }
                    }
                    
                    var toolResult = await _toolAgent.ExecuteToolAsync(
                        tool.PluginName, tool.ToolName, parameters, cancellationToken);
                    
                    return new StepExecutionResult
                    {
                        StepNumber = step.StepNumber,
                        Description = step.Description,
                        Agent = "Tool",
                        Operation = $"Execute {tool.PluginName}.{tool.ToolName}",
                        Output = toolResult.Result,
                        Success = toolResult.Success,
                        Error = toolResult.ErrorMessage
                    };
                }
                else
                {
                    return new StepExecutionResult
                    {
                        StepNumber = step.StepNumber,
                        Description = step.Description,
                        Agent = "Tool",
                        Operation = "NoToolFound",
                        Success = false,
                        Error = "No suitable tool found for this step"
                    };
                }
            }
            // Default to using chat completion for other steps
            else
            {
                // Prepare chat messages
                var messages = new List<MetaMeta.Core.Chat.ChatMessageContent>
                {
                    new MetaMeta.Core.Chat.ChatMessageContent(
                        MetaMeta.Core.Chat.ChatRoles.User, 
                        $"Perform this task: {step.Description}")
                };
                
                // Add context from previous steps
                if (previousResults.Count > 0)
                {
                    var contextBuilder = new System.Text.StringBuilder();
                    contextBuilder.AppendLine("Here are the results from previous steps:");
                    
                    foreach (var prevResult in previousResults.Where(r => r.Success).Take(3))
                    {
                        contextBuilder.AppendLine($"Step {prevResult.StepNumber}: {prevResult.Description}");
                        contextBuilder.AppendLine($"Result: {prevResult.Output}");
                        contextBuilder.AppendLine();
                    }
                    
                    // Add context as a system message
                    messages.Insert(0, new MetaMeta.Core.Chat.ChatMessageContent(
                        MetaMeta.Core.Chat.ChatRoles.System, 
                        contextBuilder.ToString()));
                }
                
                // Execute the chat completion
                var chatResponse = await _chatCompletionAgent.CompleteAsync(
                    messages, 
                    "You are a helpful assistant that completes tasks step by step. Focus on the current task.",
                    null,
                    cancellationToken);
                
                return new StepExecutionResult
                {
                    StepNumber = step.StepNumber,
                    Description = step.Description,
                    Agent = "ChatCompletion",
                    Operation = "Generate",
                    Output = chatResponse.Content,
                    Success = chatResponse.Success,
                    Error = chatResponse.ErrorMessage
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing step {StepNumber}", step.StepNumber);
            return new StepExecutionResult
            {
                StepNumber = step.StepNumber,
                Description = step.Description,
                Agent = "Unknown",
                Operation = "Error",
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    /// <summary>
    /// Generates a summary of the plan execution results.
    /// </summary>
    /// <param name="goal">The original goal.</param>
    /// <param name="plan">The executed plan.</param>
    /// <param name="results">The results of each step execution.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A summary of the execution results.</returns>
    private async Task<string> GenerateSummaryAsync(
        string goal, 
        PlanResponse plan, 
        List<StepExecutionResult> results, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating execution summary");
        
        // Prepare system prompt
        string systemPrompt = @"
You are an AI assistant tasked with summarizing the results of a multi-step plan execution.
Your summary should be concise but informative, focusing on the key outcomes, any issues encountered,
and whether the overall goal was achieved.
";
        
        // Prepare the content for the summary
        var promptBuilder = new System.Text.StringBuilder();
        promptBuilder.AppendLine($"ORIGINAL GOAL: {goal}");
        promptBuilder.AppendLine();
        
        promptBuilder.AppendLine("PLAN STEPS:");
        foreach (var step in plan.Steps)
        {
            promptBuilder.AppendLine($"{step.StepNumber}. {step.Description}");
        }
        promptBuilder.AppendLine();
        
        promptBuilder.AppendLine("EXECUTION RESULTS:");
        foreach (var result in results)
        {
            promptBuilder.AppendLine($"Step {result.StepNumber} ({result.Agent}/{result.Operation}): {(result.Success ? "SUCCESS" : "FAILED")}");
            if (!result.Success && !string.IsNullOrEmpty(result.Error))
            {
                promptBuilder.AppendLine($"  Error: {result.Error}");
            }
            
            // Include a condensed version of the output for successful steps
            if (result.Success && !string.IsNullOrEmpty(result.Output))
            {
                string condensedOutput = result.Output.Length > 100 
                    ? result.Output.Substring(0, 100) + "..." 
                    : result.Output;
                promptBuilder.AppendLine($"  Output: {condensedOutput}");
            }
        }
        
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("Please summarize the results of this plan execution, including:");
        promptBuilder.AppendLine("1. Whether the goal was achieved");
        promptBuilder.AppendLine("2. Key accomplishments");
        promptBuilder.AppendLine("3. Any issues or failures");
        promptBuilder.AppendLine("4. Overall assessment");
        
        // Create chat messages
        var messages = new List<MetaMeta.Core.Chat.ChatMessageContent>
        {
            new MetaMeta.Core.Chat.ChatMessageContent(
                MetaMeta.Core.Chat.ChatRoles.System, systemPrompt),
            new MetaMeta.Core.Chat.ChatMessageContent(
                MetaMeta.Core.Chat.ChatRoles.User, promptBuilder.ToString())
        };
        
        // Get the summary from the chat completion agent
        var summaryResponse = await _chatCompletionAgent.CompleteAsync(
            messages, systemPrompt, null, cancellationToken);
        
        return summaryResponse.Success ? summaryResponse.Content : "Failed to generate summary";
    }
}

/// <summary>
/// Represents the result of a single step execution.
/// </summary>
public class StepExecutionResult
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
    /// Gets or sets the agent that executed the step.
    /// </summary>
    public string Agent { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the operation performed.
    /// </summary>
    public string Operation { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the output of the step execution.
    /// </summary>
    public string Output { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the step execution was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the error message if the step execution failed.
    /// </summary>
    public string? Error { get; set; }
}

/// <summary>
/// Represents the overall result of orchestrating a goal.
/// </summary>
public class OrchestratorResult
{
    /// <summary>
    /// Gets or sets the original goal.
    /// </summary>
    public string Goal { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the goal was successfully achieved.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the number of steps in the plan.
    /// </summary>
    public int PlanStepCount { get; set; }
    
    /// <summary>
    /// Gets or sets the number of steps that were successfully completed.
    /// </summary>
    public int CompletedStepCount { get; set; }
    
    /// <summary>
    /// Gets or sets a summary of the goal execution.
    /// </summary>
    public string Summary { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the results of each step execution.
    /// </summary>
    public List<StepExecutionResult> StepResults { get; set; } = new List<StepExecutionResult>();
    
    /// <summary>
    /// Gets or sets the errors encountered during goal execution.
    /// </summary>
    public List<string> Errors { get; set; } = new List<string>();
} 