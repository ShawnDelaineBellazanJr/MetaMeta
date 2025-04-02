using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using MetaMeta.Orchestration.Models;

namespace MetaMeta.Orchestration.Agents;

/// <summary>
/// Agent responsible for complex reasoning tasks, including step-by-step problem solving,
/// knowledge synthesis, and logical analysis.
/// </summary>
public class ReasoningAgent : AgentBase<ReasoningRequest, ReasoningResponse>
{
    private readonly MetaMeta.Core.Abstractions.IPromptTemplateFactory _promptFactory;
    private readonly Prompts.PromptLoader _promptLoader;
    private MetaMeta.Core.Abstractions.IPromptTemplate _reasoningPrompt;

    /// <summary>
    /// Initializes a new instance of the ReasoningAgent class.
    /// </summary>
    /// <param name="kernel">The semantic kernel instance.</param>
    /// <param name="logger">The logger for agent operations.</param>
    /// <param name="promptFactory">Factory for creating prompt templates.</param>
    /// <param name="promptLoader">Loader for prompt templates from files.</param>
    public ReasoningAgent(
        Kernel kernel,
        ILogger<ReasoningAgent> logger,
        MetaMeta.Core.Abstractions.IPromptTemplateFactory promptFactory,
        Prompts.PromptLoader promptLoader)
        : base(kernel, logger)
    {
        _promptFactory = promptFactory;
        _promptLoader = promptLoader;
        
        // Initialize prompts asynchronously
        InitializePromptsAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Initializes the prompts used by this agent.
    /// </summary>
    private async Task InitializePromptsAsync()
    {
        try
        {
            // Load the reasoning prompt from the prompty file
            _reasoningPrompt = await _promptLoader.LoadPromptAsync("ReasoningAgent");
            Logger.LogInformation("Successfully loaded ReasoningAgent prompt template");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load ReasoningAgent prompt template");
            
            // Fallback to hardcoded template
            var promptConfig = new MetaMeta.Core.Abstractions.PromptTemplateConfig
            {
                Name = "ProblemAnalysis",
                Template = @"
You are a reasoning expert tasked with breaking down complex problems into clear, logical steps.

PROBLEM:
{{$problem}}

{{#if $context}}
ADDITIONAL CONTEXT:
{{$context}}
{{/if}}

Step through this problem systematically:
1. Identify the key elements and constraints
2. Break down the problem into smaller parts
3. Analyze each part logically
4. Consider different approaches or perspectives
5. Formulate a step-by-step solution process

Your structured analysis:
"
            };
            
            _reasoningPrompt = _promptFactory.Create(promptConfig);
            Logger.LogWarning("Using fallback prompt template for ReasoningAgent");
        }
    }

    /// <summary>
    /// Gets the name of the agent.
    /// </summary>
    protected override string GetAgentName() => "Reasoning";

    /// <summary>
    /// Gets the description of the agent.
    /// </summary>
    protected override string GetAgentDescription() => "Performs complex reasoning, problem-solving, and logical analysis.";

    /// <summary>
    /// Gets the instructions for the agent.
    /// </summary>
    protected override string GetAgentInstructions() => 
        "Analyze problems through logical reasoning, evaluate claims, and provide step-by-step logical analysis.";

    /// <summary>
    /// Executes a reasoning operation based on the provided request.
    /// </summary>
    /// <param name="request">The reasoning request details.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The reasoning response.</returns>
    public override async Task<ReasoningResponse> ExecuteAsync(
        ReasoningRequest request, 
        CancellationToken cancellationToken = default)
    {
        LogStep(1, $"Processing reasoning request for problem: '{request.Problem}'");

        try
        {
            var result = await AnalyzeProblemAsync(
                request.Problem, 
                request.Metadata.TryGetValue("Context", out var context) ? context : null, 
                cancellationToken);
            
            if (!result.Success)
            {
                return new ReasoningResponse
                {
                    RequestId = request.RequestId,
                    SessionId = request.SessionId,
                    Success = false,
                    ErrorMessage = result.ErrorMessage,
                    Style = request.Style
                };
            }
            
            // Parse the reasoning process into steps
            var steps = ParseReasoningSteps(result.ReasoningProcess, request.MaxSteps);
            
            // Extract the conclusion from the final step or from the overall analysis
            string conclusion = ExtractConclusion(result.ReasoningProcess, steps);
            
            // Estimate confidence based on reasoning quality
            int confidenceScore = EstimateConfidence(result.ReasoningProcess);
            
            return new ReasoningResponse
            {
                RequestId = request.RequestId,
                SessionId = request.SessionId,
                Success = true,
                Conclusion = conclusion,
                Steps = steps,
                ConfidenceScore = confidenceScore,
                Style = request.Style
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error performing reasoning for problem: {Problem}", request.Problem);
            
            return new ReasoningResponse
            {
                RequestId = request.RequestId,
                SessionId = request.SessionId,
                Success = false,
                ErrorMessage = ex.Message,
                Style = request.Style
            };
        }
    }

    /// <summary>
    /// Analyzes a problem statement and breaks it down into a step-by-step reasoning process.
    /// </summary>
    /// <param name="problemStatement">The problem statement to analyze.</param>
    /// <param name="context">Additional context information, if available.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A structured breakdown of the reasoning process.</returns>
    public async Task<ReasoningResult> AnalyzeProblemAsync(
        string problemStatement,
        string? context = null,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Analyzing problem: {ProblemStatement}", problemStatement);
        
        try
        {
            var arguments = new KernelArguments
            {
                ["problem"] = problemStatement
            };
            
            if (!string.IsNullOrEmpty(context))
            {
                arguments["context"] = context;
            }

            // Use the loaded prompt template instead of creating one on the fly
            var analysisResult = await Kernel.InvokePromptAsync(_reasoningPrompt.Template, arguments, cancellationToken: cancellationToken);
            var analysis = analysisResult.ToString() ?? string.Empty;

            Logger.LogInformation("Completed problem analysis successfully");
            
            return new ReasoningResult
            {
                OriginalProblem = problemStatement,
                ReasoningProcess = analysis,
                Success = true
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error analyzing problem: {ProblemStatement}", problemStatement);
            
            return new ReasoningResult
            {
                OriginalProblem = problemStatement,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// Evaluates a claim against available evidence to determine its validity.
    /// </summary>
    /// <param name="claim">The claim to evaluate.</param>
    /// <param name="evidence">The available evidence to consider.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result indicating whether the claim is supported, refuted, or uncertain.</returns>
    public async Task<ClaimEvaluationResult> EvaluateClaimAsync(
        string claim,
        string evidence,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Evaluating claim: {Claim}", claim);
        
        try
        {
            // Create the prompt for claim evaluation
            var promptConfig = new MetaMeta.Core.Abstractions.PromptTemplateConfig
            {
                Name = "ClaimEvaluation",
                Template = @"
You are an objective evaluator tasked with assessing claims against evidence.

CLAIM TO EVALUATE:
{{$claim}}

EVIDENCE TO CONSIDER:
{{$evidence}}

Carefully assess whether the evidence supports, refutes, or is insufficient to determine the claim's validity.
Consider:
1. Whether the evidence directly addresses the claim
2. The strength of the connection between evidence and claim
3. Any logical gaps or assumptions required
4. Alternative interpretations of the evidence

Provide your assessment as one of: SUPPORTED, REFUTED, or UNCERTAIN.
Include a brief justification for your assessment.

Your evaluation:
"
            };

            var prompt = _promptFactory.Create(promptConfig);
            
            var arguments = new KernelArguments
            {
                ["claim"] = claim,
                ["evidence"] = evidence
            };

            var evaluationResult = await Kernel.InvokePromptAsync(prompt.Template, arguments, cancellationToken: cancellationToken);
            var evaluation = evaluationResult.ToString() ?? string.Empty;

            // Simple heuristic to determine the verdict from the response
            // In a real implementation, this would be more sophisticated
            string verdict = "INCONCLUSIVE";
            if (evaluation.Contains("SUPPORTED", StringComparison.OrdinalIgnoreCase) || 
                evaluation.Contains("VALID", StringComparison.OrdinalIgnoreCase))
            {
                verdict = "SUPPORTED";
            }
            else if (evaluation.Contains("CONTRADICTED", StringComparison.OrdinalIgnoreCase) || 
                     evaluation.Contains("INVALID", StringComparison.OrdinalIgnoreCase) ||
                     evaluation.Contains("REFUTED", StringComparison.OrdinalIgnoreCase))
            {
                verdict = "CONTRADICTED";
            }
            
            // Estimate confidence level
            string confidence = "LOW";
            if (evaluation.Contains("HIGH CONFIDENCE", StringComparison.OrdinalIgnoreCase))
            {
                confidence = "HIGH";
            }
            else if (evaluation.Contains("MEDIUM CONFIDENCE", StringComparison.OrdinalIgnoreCase))
            {
                confidence = "MEDIUM";
            }

            Logger.LogInformation("Completed claim evaluation with verdict: {Verdict}", verdict);
            
            return new ClaimEvaluationResult
            {
                Claim = claim,
                Evidence = evidence,
                Verdict = verdict,
                Confidence = confidence,
                Reasoning = evaluation,
                Success = true
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error evaluating claim: {Claim}", claim);
            
            return new ClaimEvaluationResult
            {
                Claim = claim,
                Evidence = evidence,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
    
    /// <summary>
    /// Parses the reasoning process into individual steps.
    /// </summary>
    /// <param name="reasoningProcess">The full reasoning process text.</param>
    /// <param name="maxSteps">The maximum number of steps to extract.</param>
    /// <returns>A list of reasoning steps.</returns>
    private List<ReasoningStep> ParseReasoningSteps(string reasoningProcess, int maxSteps)
    {
        var steps = new List<ReasoningStep>();
        
        // Simple parsing to extract steps - in a real implementation this would be more robust
        var lines = reasoningProcess.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        
        int stepNumber = 1;
        StringBuilder currentStep = new StringBuilder();
        string? currentStepTitle = null;
        
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            
            // Check if this line starts a new step
            if (trimmed.StartsWith($"Step {stepNumber}:") || 
                trimmed.StartsWith($"{stepNumber}.") || 
                trimmed.StartsWith($"({stepNumber})"))
            {
                // If we were building a previous step, add it to our list
                if (currentStep.Length > 0 && stepNumber > 1)
                {
                    steps.Add(new ReasoningStep
                    {
                        StepNumber = stepNumber - 1,
                        Description = currentStepTitle ?? $"Step {stepNumber - 1}",
                        Reasoning = currentStep.ToString().Trim()
                    });
                    
                    // Reset for the next step
                    currentStep.Clear();
                }
                
                // Extract the step title
                int colonIndex = trimmed.IndexOf(':');
                currentStepTitle = colonIndex > 0 ? trimmed.Substring(colonIndex + 1).Trim() : trimmed;
                
                stepNumber++;
            }
            else if (stepNumber > 1) // We're inside a step
            {
                currentStep.AppendLine(trimmed);
                
                // Check if this line contains an intermediate conclusion
                if (trimmed.Contains("conclusion", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Contains("therefore", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Contains("thus", StringComparison.OrdinalIgnoreCase))
                {
                    // This is a simplistic heuristic - a real implementation would be more sophisticated
                    if (steps.Count > 0)
                    {
                        steps[steps.Count - 1].IntermediateConclusion = trimmed;
                    }
                }
            }
        }
        
        // Add the final step if needed
        if (currentStep.Length > 0)
        {
            steps.Add(new ReasoningStep
            {
                StepNumber = stepNumber - 1,
                Description = currentStepTitle ?? $"Step {stepNumber - 1}",
                Reasoning = currentStep.ToString().Trim()
            });
        }
        
        // Limit to max steps
        return steps.Count <= maxSteps ? steps : steps.GetRange(0, maxSteps);
    }
    
    /// <summary>
    /// Extracts the final conclusion from the reasoning process.
    /// </summary>
    /// <param name="reasoningProcess">The full reasoning process text.</param>
    /// <param name="steps">The parsed reasoning steps.</param>
    /// <returns>The extracted conclusion.</returns>
    private string ExtractConclusion(string reasoningProcess, List<ReasoningStep> steps)
    {
        // Try to find an explicit conclusion section
        if (reasoningProcess.Contains("Conclusion:", StringComparison.OrdinalIgnoreCase))
        {
            int index = reasoningProcess.IndexOf("Conclusion:", StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                string conclusion = reasoningProcess.Substring(index + "Conclusion:".Length).Trim();
                
                // If there are multiple lines, take only the first paragraph
                int nextLineBreak = conclusion.IndexOf("\n\n");
                if (nextLineBreak > 0)
                {
                    conclusion = conclusion.Substring(0, nextLineBreak).Trim();
                }
                
                return conclusion;
            }
        }
        
        // If no explicit conclusion, use the intermediate conclusion from the last step
        if (steps.Count > 0 && !string.IsNullOrEmpty(steps[steps.Count - 1].IntermediateConclusion))
        {
            return steps[steps.Count - 1].IntermediateConclusion;
        }
        
        // If still nothing, extract the last paragraph as a fallback
        var paragraphs = reasoningProcess.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (paragraphs.Length > 0)
        {
            return paragraphs[paragraphs.Length - 1].Trim();
        }
        
        // Last resort
        return "No clear conclusion could be derived from the analysis.";
    }
    
    /// <summary>
    /// Estimates a confidence score based on the reasoning quality.
    /// </summary>
    /// <param name="reasoningProcess">The reasoning process text.</param>
    /// <returns>A confidence score from 0-100.</returns>
    private int EstimateConfidence(string reasoningProcess)
    {
        // This is a simplified heuristic - in a real implementation this would be more sophisticated
        int baseScore = 50; // Start at medium confidence
        
        // Adjust based on indicators of confident reasoning
        if (reasoningProcess.Contains("certainly", StringComparison.OrdinalIgnoreCase) ||
            reasoningProcess.Contains("definitely", StringComparison.OrdinalIgnoreCase) ||
            reasoningProcess.Contains("clearly", StringComparison.OrdinalIgnoreCase))
        {
            baseScore += 15;
        }
        
        // Reduce for uncertainty markers
        if (reasoningProcess.Contains("perhaps", StringComparison.OrdinalIgnoreCase) ||
            reasoningProcess.Contains("maybe", StringComparison.OrdinalIgnoreCase) ||
            reasoningProcess.Contains("uncertain", StringComparison.OrdinalIgnoreCase) ||
            reasoningProcess.Contains("unclear", StringComparison.OrdinalIgnoreCase))
        {
            baseScore -= 15;
        }
        
        // Adjust for thoroughness
        if (reasoningProcess.Length > 1000)
        {
            baseScore += 10;
        }
        else if (reasoningProcess.Length < 300)
        {
            baseScore -= 10;
        }
        
        // Ensure score is within 0-100 range
        return Math.Clamp(baseScore, 0, 100);
    }
}

/// <summary>
/// Represents a reasoning analysis result.
/// </summary>
public class ReasoningResult
{
    /// <summary>
    /// Gets or sets the original problem statement.
    /// </summary>
    public string OriginalProblem { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the detailed reasoning process.
    /// </summary>
    public string ReasoningProcess { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the reasoning was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the error message if reasoning failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Represents the result of evaluating a claim against evidence.
/// </summary>
public class ClaimEvaluationResult
{
    /// <summary>
    /// Gets or sets the claim that was evaluated.
    /// </summary>
    public string Claim { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the evidence considered for the evaluation.
    /// </summary>
    public string Evidence { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the verdict (SUPPORTED, CONTRADICTED, INCONCLUSIVE).
    /// </summary>
    public string Verdict { get; set; } = "INCONCLUSIVE";
    
    /// <summary>
    /// Gets or sets the confidence level (HIGH, MEDIUM, LOW).
    /// </summary>
    public string Confidence { get; set; } = "LOW";
    
    /// <summary>
    /// Gets or sets the detailed reasoning behind the evaluation.
    /// </summary>
    public string Reasoning { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the evaluation was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the error message if evaluation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
} 