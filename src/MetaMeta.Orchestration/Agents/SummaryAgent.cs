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

namespace MetaMeta.Orchestration.Agents;

/// <summary>
/// Agent responsible for summarizing large content into concise, structured formats.
/// </summary>
/// <remarks>
/// Processes documents, conversations, and other large content to extract key information,
/// identify main themes, and produce structured summaries with appropriate detail levels.
/// </remarks>
public class SummaryAgent
{
    private readonly Kernel _kernel;
    private readonly ILogger<SummaryAgent> _logger;
    private readonly MetaMeta.Core.Abstractions.IPromptTemplateFactory _promptFactory;

    /// <summary>
    /// Initializes a new instance of the SummaryAgent class.
    /// </summary>
    /// <param name="kernel">The semantic kernel instance.</param>
    /// <param name="logger">The logger for agent operations.</param>
    /// <param name="promptFactory">Factory for creating prompt templates.</param>
    public SummaryAgent(
        Kernel kernel, 
        ILogger<SummaryAgent> logger, 
        MetaMeta.Core.Abstractions.IPromptTemplateFactory promptFactory)
    {
        _kernel = kernel;
        _logger = logger;
        _promptFactory = promptFactory;
    }

    /// <summary>
    /// Gets the name of the agent.
    /// </summary>
    public string Name => "Summary";

    /// <summary>
    /// Gets the description of the agent.
    /// </summary>
    public string Description => "Summarizes content of various lengths and formats into concise digests.";
    
    /// <summary>
    /// Gets the instructions for the agent.
    /// </summary>
    private string Instructions =>
        "You are a summarization specialist who extracts and condenses key information. " +
        "When provided with content, identify the main points, key details, and overall message. " +
        "Organize summaries in clear formats with appropriate hierarchy based on importance. " +
        "Adapt summary length and detail based on specified requirements.";

    /// <summary>
    /// Executes the summary agent for the specified request.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the agent execution.</returns>
    public async Task<SummaryResponse> ExecuteAsync(SummaryRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Initialize response
        _logger.LogInformation("Step 1: Initializing summary generation");
        var response = new SummaryResponse
        {
            RequestId = request.RequestId,
            ContentType = request.ContentType
        };

        try
        {
            // 2. Create a simplified prompt template
            _logger.LogInformation("Step 2: Creating summary prompt");
            
            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine(Instructions);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("CONTENT TO SUMMARIZE:");
            promptBuilder.AppendLine(request.Content);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine($"Content Type: {request.ContentType}");
            
            if (request.MaxLength > 0)
            {
                promptBuilder.AppendLine($"Maximum Length: {request.MaxLength} characters");
            }
            
            if (!string.IsNullOrEmpty(request.Focus))
            {
                promptBuilder.AppendLine($"Focus on: {request.Focus}");
            }
            
            if (!string.IsNullOrEmpty(request.Format))
            {
                promptBuilder.AppendLine($"Format: {request.Format}");
            }
            
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Include the following in your summary:");
            promptBuilder.AppendLine("1. Main points and key takeaways");
            
            if (request.IncludeKeyPoints)
            {
                promptBuilder.AppendLine("2. A bullet-point list of key points at the end");
            }
            
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("SUMMARY:");
            
            // 3. Generate the summary
            _logger.LogInformation("Step 3: Generating summary");
            var result = await _kernel.InvokePromptAsync(
                promptBuilder.ToString(),
                cancellationToken: cancellationToken);
            
            var summaryText = result.GetValue<string>()?.Trim() ?? string.Empty;
            
            if (string.IsNullOrEmpty(summaryText))
            {
                throw new InvalidOperationException("Summary generation produced no result");
            }
            
            // 4. Extract key points if requested
            if (request.IncludeKeyPoints)
            {
                _logger.LogInformation("Step 4: Extracting key points");
                response.KeyPoints = ExtractKeyPoints(summaryText);
            }
            
            // 5. Prepare response
            response.Summary = summaryText;
            response.Success = true;
            response.Metadata["contentLength"] = request.Content.Length.ToString();
            response.Metadata["summaryLength"] = response.Summary.Length.ToString();
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating summary for content type: {ContentType}", request.ContentType);
            response.Success = false;
            response.ErrorMessage = $"Summary generation failed: {ex.Message}";
            return response;
        }
    }
    
    /// <summary>
    /// Extracts key points from the summary text.
    /// </summary>
    /// <param name="summaryText">The summary text.</param>
    /// <returns>A list of key points.</returns>
    private List<string> ExtractKeyPoints(string summaryText)
    {
        var keyPoints = new List<string>();
        
        // Look for sections that might contain key points
        var keyPointsMarkers = new[]
        {
            "key points:", "key takeaways:", "key insights:", "main points:", 
            "key findings:", "highlights:", "important points:"
        };
        
        // Find the start of the key points section
        int keyPointsStart = -1;
        foreach (var marker in keyPointsMarkers)
        {
            keyPointsStart = summaryText.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (keyPointsStart >= 0)
            {
                keyPointsStart += marker.Length;
                break;
            }
        }
        
        if (keyPointsStart < 0)
        {
            // No explicit key points section found
            return keyPoints;
        }
        
        // Extract the key points section
        string keyPointsSection = summaryText.Substring(keyPointsStart).Trim();
        
        // Split by bullet points or numbered items
        string[] lines = keyPointsSection.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            string trimmedLine = line.Trim();
            
            // Check if the line starts with a bullet point or number
            if (trimmedLine.StartsWith("-") || 
                trimmedLine.StartsWith("•") || 
                trimmedLine.StartsWith("*") || 
                IsNumberedItem(trimmedLine))
            {
                // Extract the content after the bullet or number
                string pointContent = ExtractPointContent(trimmedLine);
                if (!string.IsNullOrWhiteSpace(pointContent))
                {
                    keyPoints.Add(pointContent);
                }
            }
            else if (keyPoints.Count > 0)
            {
                // If this line doesn't start with a bullet but we already have some key points,
                // it might be a continuation of the previous point
                keyPoints[keyPoints.Count - 1] += " " + trimmedLine;
            }
        }
        
        return keyPoints;
    }
    
    /// <summary>
    /// Determines if a line is a numbered item.
    /// </summary>
    /// <param name="line">The line to check.</param>
    /// <returns>True if the line is a numbered item, otherwise false.</returns>
    private bool IsNumberedItem(string line)
    {
        // Check for patterns like "1.", "1)", "(1)", etc.
        if (string.IsNullOrWhiteSpace(line) || line.Length < 2)
        {
            return false;
        }
        
        // Check for digit followed by period or parenthesis
        if (char.IsDigit(line[0]) && (line[1] == '.' || line[1] == ')'))
        {
            return true;
        }
        
        // Check for parenthesized number like "(1)"
        if (line[0] == '(' && line.Length > 2 && char.IsDigit(line[1]) && line.Contains(')'))
        {
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Extracts the content of a point after the bullet or number.
    /// </summary>
    /// <param name="line">The line containing a bullet point or numbered item.</param>
    /// <returns>The content of the point.</returns>
    private string ExtractPointContent(string line)
    {
        string content = line.Trim();
        
        // Remove leading bullet point or numbered item
        if (content.StartsWith("-") || content.StartsWith("•") || content.StartsWith("*"))
        {
            content = content.Substring(1).Trim();
        }
        else if (IsNumberedItem(content))
        {
            // Find the position after the number and its delimiter
            int delimiterPos = content.IndexOfAny(new[] { '.', ')', '(' });
            if (delimiterPos >= 0)
            {
                // If it's a pattern like "(1)" find the closing parenthesis
                if (content[delimiterPos] == '(')
                {
                    int closingPos = content.IndexOf(')', delimiterPos);
                    if (closingPos >= 0)
                    {
                        content = content.Substring(closingPos + 1).Trim();
                    }
                }
                else
                {
                    content = content.Substring(delimiterPos + 1).Trim();
                }
            }
        }
        
        return content;
    }
} 