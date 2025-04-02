using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetaMeta.ApiService.Models;
using MetaMeta.Orchestration.Agents;
using MetaMeta.Orchestration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MetaMeta.ApiService.Controllers;

/// <summary>
/// API controller for content generation capabilities.
/// </summary>
/// <remarks>
/// Provides endpoints for generating various types of content including
/// text, code, and structured documents based on specifications.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class ContentController : ControllerBase
{
    private readonly ContentAgent _contentAgent;
    private readonly ILogger<ContentController> _logger;

    /// <summary>
    /// Initializes a new instance of the ContentController class.
    /// </summary>
    /// <param name="contentAgent">The content generation agent implementation.</param>
    /// <param name="logger">The logger for controller operations.</param>
    public ContentController(ContentAgent contentAgent, ILogger<ContentController> logger)
    {
        // Step 1: Store dependencies
        _contentAgent = contentAgent;
        _logger = logger;
    }

    /// <summary>
    /// Generates content based on the provided request.
    /// </summary>
    /// <param name="request">The content generation request.</param>
    /// <returns>The generated content response.</returns>
    [HttpPost("generate")]
    public async Task<ActionResult<ContentResponse>> GenerateContent([FromBody] ContentGenerationRequest request)
    {
        try
        {
            // Step 1: Validate request
            if (string.IsNullOrEmpty(request.Prompt))
            {
                return BadRequest("Prompt is required");
            }
            
            // Step 2: Log the incoming request
            _logger.LogInformation("Received content generation request: {Prompt}, Type: {Type}", 
                request.Prompt, request.ContentType);
            
            // Step 3: Map API request to domain model
            var agentRequest = new ContentRequest
            {
                RequestId = request.RequestId ?? Guid.NewGuid().ToString(),
                SessionId = request.SessionId ?? Guid.NewGuid().ToString(),
                Assistant = request.Assistant ?? "ContentAPI",
                Prompt = request.Prompt,
                ContentType = MapContentType(request.ContentType),
                Specifications = request.Specifications ?? string.Empty,
                Format = request.Format ?? "plain",
                MaxLength = request.MaxLength ?? 1000,
                Tone = request.Tone ?? "professional"
            };
            
            // Set additional context in metadata if provided
            if (request.AdditionalContext != null)
            {
                foreach (var item in request.AdditionalContext)
                {
                    agentRequest.Metadata[item.Key] = item.Value;
                }
            }
            
            // Step 4: Execute the content agent
            _logger.LogInformation("Executing content agent");
            var agentResponse = await _contentAgent.ExecuteAsync(agentRequest);
            
            // Step 5: Handle failed generation
            if (!agentResponse.Success)
            {
                _logger.LogError("Content generation failed: {Error}", agentResponse.ErrorMessage);
                return StatusCode(500, new { 
                    Error = "Content generation failed", 
                    Details = agentResponse.ErrorMessage 
                });
            }
            
            // Step 6: Return successful response
            return Ok(agentResponse);
        }
        catch (Exception ex)
        {
            // Step 7: Handle and log errors
            _logger.LogError(ex, "Error processing content generation request: {Message}", ex.Message);
            return StatusCode(500, new {
                Error = "Internal server error",
                Details = ex.Message
            });
        }
    }
    
    /// <summary>
    /// Maps API content type to domain content type.
    /// </summary>
    /// <param name="contentType">The API content type.</param>
    /// <returns>The domain content type.</returns>
    private ContentType MapContentType(string contentType)
    {
        return contentType?.ToLower() switch
        {
            "text" => ContentType.Text,
            "code" => ContentType.Code,
            "document" => ContentType.Document,
            "email" => ContentType.Email,
            "mixed" => ContentType.Mixed,
            _ => ContentType.Text
        };
    }
} 