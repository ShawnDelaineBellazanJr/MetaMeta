using System;
using System.Threading.Tasks;
using MetaMeta.ApiService.Models;
using MetaMeta.Orchestration.Agents;
using MetaMeta.Orchestration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MetaMeta.ApiService.Controllers;

/// <summary>
/// API controller for reasoning capabilities.
/// </summary>
/// <remarks>
/// Provides endpoints for logical reasoning and problem-solving, processing problems
/// through step-by-step analytical thinking to reach well-justified conclusions.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class ReasoningController : ControllerBase
{
    private readonly ReasoningAgent _reasoningAgent;
    private readonly ILogger<ReasoningController> _logger;

    /// <summary>
    /// Initializes a new instance of the ReasoningController class.
    /// </summary>
    /// <param name="reasoningAgent">The reasoning agent implementation.</param>
    /// <param name="logger">The logger for controller operations.</param>
    public ReasoningController(ReasoningAgent reasoningAgent, ILogger<ReasoningController> logger)
    {
        // Step 1: Store dependencies
        _reasoningAgent = reasoningAgent;
        _logger = logger;
    }

    /// <summary>
    /// Analyzes a problem using step-by-step reasoning.
    /// </summary>
    /// <param name="request">The reasoning request.</param>
    /// <returns>The reasoning response with steps and conclusion.</returns>
    [HttpPost("analyze")]
    public async Task<ActionResult<ReasoningResponse>> AnalyzeProblem([FromBody] ReasoningAnalysisRequest request)
    {
        try
        {
            // Step 1: Validate request
            if (string.IsNullOrEmpty(request.Problem))
            {
                return BadRequest("Problem statement is required");
            }
            
            // Step 2: Log the incoming request
            _logger.LogInformation("Received reasoning request for problem: {Problem}", request.Problem);
            
            // Step 3: Map API request to domain model
            var agentRequest = new Orchestration.Models.ReasoningRequest
            {
                RequestId = request.RequestId ?? Guid.NewGuid().ToString(),
                SessionId = request.SessionId ?? Guid.NewGuid().ToString(),
                Assistant = request.Assistant ?? "ReasoningAPI",
                Problem = request.Problem,
                Style = MapReasoningStyle(request.Style),
                MaxSteps = request.MaxSteps ?? 5,
                IncludeAlternatives = request.IncludeAlternatives ?? false
            };
            
            // Add any metadata
            if (request.Metadata != null)
            {
                foreach (var entry in request.Metadata)
                {
                    agentRequest.Metadata[entry.Key] = entry.Value;
                }
            }
            
            // Step 4: Execute the reasoning agent
            _logger.LogInformation("Executing reasoning agent");
            var agentResponse = await _reasoningAgent.ExecuteAsync(agentRequest);
            
            // Step 5: Handle failed analysis
            if (!agentResponse.Success)
            {
                _logger.LogError("Reasoning analysis failed: {Error}", agentResponse.ErrorMessage);
                return StatusCode(500, new { 
                    Error = "Reasoning analysis failed", 
                    Details = agentResponse.ErrorMessage 
                });
            }
            
            // Step 6: Return successful response
            return Ok(agentResponse);
        }
        catch (Exception ex)
        {
            // Step 7: Handle and log errors
            _logger.LogError(ex, "Error processing reasoning request: {Message}", ex.Message);
            return StatusCode(500, new {
                Error = "Internal server error",
                Details = ex.Message
            });
        }
    }
    
    /// <summary>
    /// Maps API reasoning style to domain reasoning style.
    /// </summary>
    /// <param name="style">The API reasoning style.</param>
    /// <returns>The domain reasoning style.</returns>
    private Orchestration.Models.ReasoningStyle MapReasoningStyle(string? style)
    {
        return style?.ToLower() switch
        {
            "analytical" => Orchestration.Models.ReasoningStyle.Analytical,
            "creative" => Orchestration.Models.ReasoningStyle.Creative,
            "critical" => Orchestration.Models.ReasoningStyle.Critical,
            "strategic" => Orchestration.Models.ReasoningStyle.Strategic,
            "scientific" => Orchestration.Models.ReasoningStyle.Scientific,
            _ => Orchestration.Models.ReasoningStyle.Analytical
        };
    }
} 