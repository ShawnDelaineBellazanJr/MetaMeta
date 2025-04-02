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
/// Agent responsible for content generation, transformation, and manipulation.
/// </summary>
public class ContentAgent : AgentBase<ContentRequest, ContentResponse>
{
    private readonly MetaMeta.Core.Abstractions.IPromptTemplateFactory _promptFactory;

    /// <summary>
    /// Initializes a new instance of the ContentAgent class.
    /// </summary>
    /// <param name="kernel">The semantic kernel instance.</param>
    /// <param name="logger">The logger for agent operations.</param>
    /// <param name="promptFactory">Factory for creating prompt templates.</param>
    public ContentAgent(
        Kernel kernel,
        ILogger<ContentAgent> logger,
        MetaMeta.Core.Abstractions.IPromptTemplateFactory promptFactory)
        : base(kernel, logger)
    {
        _promptFactory = promptFactory;
    }

    /// <summary>
    /// Gets the name of the agent.
    /// </summary>
    protected override string GetAgentName() => "Content";

    /// <summary>
    /// Gets the description of the agent.
    /// </summary>
    protected override string GetAgentDescription() => "Generates, transforms, and manipulates content in various formats.";

    /// <summary>
    /// Gets the instructions for the agent.
    /// </summary>
    protected override string GetAgentInstructions() => 
        "Generate high-quality content based on provided specifications, including the desired format, tone, and length.";

    /// <summary>
    /// Executes the content generation operation based on the provided request.
    /// </summary>
    /// <param name="request">The content request details.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The content response.</returns>
    public override async Task<ContentResponse> ExecuteAsync(
        ContentRequest request, 
        CancellationToken cancellationToken = default)
    {
        LogStep(1, $"Processing content request for '{request.Prompt}'");

        try
        {
            string topic = request.Prompt;
            string format = request.Format;
            string tone = request.Tone;
            int length = request.MaxLength;
            
            // Generate the content
            var result = await GenerateContentAsync(
                topic, 
                format, 
                tone, 
                length, 
                request.Specifications, 
                cancellationToken);
            
            if (!result.Success)
            {
                return new ContentResponse
                {
                    RequestId = request.RequestId,
                    SessionId = request.SessionId,
                    Success = false,
                    ErrorMessage = result.ErrorMessage,
                    ContentType = request.ContentType
                };
            }
            
            return new ContentResponse
            {
                RequestId = request.RequestId,
                SessionId = request.SessionId,
                Success = true,
                Content = result.Content,
                ContentType = request.ContentType,
                Format = format,
                ContentLength = result.WordCount,
                Metadata = new Dictionary<string, string>
                {
                    ["Topic"] = topic,
                    ["Tone"] = tone
                }
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error performing content generation for prompt: {Prompt}", request.Prompt);
            
            return new ContentResponse
            {
                RequestId = request.RequestId,
                SessionId = request.SessionId,
                Success = false,
                ErrorMessage = ex.Message,
                ContentType = request.ContentType
            };
        }
    }

    /// <summary>
    /// Generates content based on a given topic and requirements.
    /// </summary>
    /// <param name="topic">The topic to generate content about.</param>
    /// <param name="format">The desired format (e.g., blog post, email, report).</param>
    /// <param name="tone">The desired tone (e.g., formal, casual, technical).</param>
    /// <param name="length">The approximate desired length in words.</param>
    /// <param name="guidelines">Additional guidelines or requirements.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The generated content.</returns>
    public async Task<ContentGenerationResult> GenerateContentAsync(
        string topic,
        string format,
        string tone,
        int length,
        string? guidelines = null,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Generating {Format} content on topic: {Topic}", format, topic);
        
        try
        {
            var promptConfig = new MetaMeta.Core.Abstractions.PromptTemplateConfig
            {
                Name = "ContentGeneration",
                Template = @"
You are a professional content creator tasked with generating high-quality content.

TOPIC:
{{$topic}}

FORMAT:
{{$format}}

TONE:
{{$tone}}

APPROXIMATE LENGTH:
{{$length}} words

{{#if $guidelines}}
ADDITIONAL GUIDELINES:
{{$guidelines}}
{{/if}}

Create well-structured, engaging content that meets these requirements. Focus on providing value to the reader while maintaining the specified tone and format.

Generated content:
"
            };

            var prompt = _promptFactory.Create(promptConfig);
            var arguments = new KernelArguments
            {
                ["topic"] = topic,
                ["format"] = format,
                ["tone"] = tone,
                ["length"] = length.ToString()
            };
            
            if (!string.IsNullOrEmpty(guidelines))
            {
                arguments["guidelines"] = guidelines;
            }

            var contentResult = await Kernel.InvokePromptAsync(prompt.Template, arguments, cancellationToken: cancellationToken);
            var content = contentResult.ToString() ?? string.Empty;

            Logger.LogInformation("Successfully generated {Format} content on topic: {Topic}", format, topic);
            
            return new ContentGenerationResult
            {
                Topic = topic,
                Format = format,
                Content = content,
                WordCount = CountWords(content),
                Success = true
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating content for topic: {Topic}", topic);
            
            return new ContentGenerationResult
            {
                Topic = topic,
                Format = format,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// Transforms existing content according to the specified operation.
    /// </summary>
    /// <param name="content">The original content to transform.</param>
    /// <param name="operation">The transformation operation (e.g., summarize, expand, translate).</param>
    /// <param name="parameters">Additional parameters for the transformation.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The transformed content.</returns>
    public async Task<ContentTransformationResult> TransformContentAsync(
        string content,
        string operation,
        Dictionary<string, string>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Transforming content with operation: {Operation}", operation);
        
        try
        {
            // Build a description of the transformation
            string operationDescription;
            switch (operation.ToLower())
            {
                case "summarize":
                    operationDescription = "Create a concise summary capturing the key points";
                    break;
                case "expand":
                    operationDescription = "Expand on the content with additional details and examples";
                    break;
                case "simplify":
                    operationDescription = "Rewrite the content in simpler language, suitable for a general audience";
                    break;
                case "formalize":
                    operationDescription = "Rewrite the content in a more formal, professional tone";
                    break;
                case "translate":
                    string language = parameters != null && parameters.TryGetValue("language", out var lang) ? lang : "Spanish";
                    operationDescription = $"Translate the content into {language}";
                    break;
                default:
                    operationDescription = $"Apply the {operation} transformation";
                    break;
            }
            
            // Format any additional parameters as instructions
            var parameterInstructions = new StringBuilder();
            if (parameters != null && parameters.Count > 0)
            {
                parameterInstructions.AppendLine("ADDITIONAL REQUIREMENTS:");
                foreach (var param in parameters)
                {
                    if (param.Key != "language") // Skip language as it's already handled
                    {
                        parameterInstructions.AppendLine($"- {param.Key}: {param.Value}");
                    }
                }
            }

            var promptConfig = new MetaMeta.Core.Abstractions.PromptTemplateConfig
            {
                Name = "ContentTransformation",
                Template = $@"
You are a professional content editor tasked with transforming content according to specific requirements.

TRANSFORMATION:
{operationDescription}

ORIGINAL CONTENT:
{{$content}}

{parameterInstructions}

Apply the specified transformation while preserving the core meaning and important information from the original content.

Transformed content:
"
            };

            var prompt = _promptFactory.Create(promptConfig);
            var arguments = new KernelArguments
            {
                ["content"] = content
            };

            var transformResult = await Kernel.InvokePromptAsync(prompt.Template, arguments, cancellationToken: cancellationToken);
            var transformedContent = transformResult.ToString() ?? string.Empty;

            Logger.LogInformation("Successfully transformed content with operation: {Operation}", operation);
            
            return new ContentTransformationResult
            {
                OriginalContent = content,
                TransformedContent = transformedContent,
                Operation = operation,
                OriginalWordCount = CountWords(content),
                TransformedWordCount = CountWords(transformedContent),
                Success = true
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error transforming content with operation: {Operation}", operation);
            
            return new ContentTransformationResult
            {
                OriginalContent = content,
                Operation = operation,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// Creates an outline for content on a given topic.
    /// </summary>
    /// <param name="topic">The topic to create an outline for.</param>
    /// <param name="depth">The depth level of the outline (1-3).</param>
    /// <param name="format">The desired format (e.g., article, report, book).</param>
    /// <param name="guidelines">Additional guidelines or requirements.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The content outline.</returns>
    public async Task<ContentOutlineResult> CreateOutlineAsync(
        string topic,
        int depth,
        string format,
        string? guidelines = null,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Creating outline for {Format} on topic: {Topic} with depth: {Depth}", format, topic, depth);
        
        try
        {
            // Ensure depth is between 1 and 3
            depth = Math.Clamp(depth, 1, 3);
            
            var promptConfig = new MetaMeta.Core.Abstractions.PromptTemplateConfig
            {
                Name = "ContentOutline",
                Template = @"
You are a professional content strategist tasked with creating detailed outlines.

TOPIC:
{{$topic}}

FORMAT:
{{$format}}

OUTLINE DEPTH:
{{$depth}} (1 = high-level sections only, 2 = sections with subsections, 3 = detailed with sub-subsections)

{{#if $guidelines}}
ADDITIONAL GUIDELINES:
{{$guidelines}}
{{/if}}

Create a well-structured, comprehensive outline that would serve as a strong foundation for creating content on this topic.
Use a clear hierarchical structure with appropriate numbering/bullet points for each level.

Outline:
"
            };

            var prompt = _promptFactory.Create(promptConfig);
            var arguments = new KernelArguments
            {
                ["topic"] = topic,
                ["format"] = format,
                ["depth"] = depth.ToString()
            };
            
            if (!string.IsNullOrEmpty(guidelines))
            {
                arguments["guidelines"] = guidelines;
            }

            var outlineResult = await Kernel.InvokePromptAsync(prompt.Template, arguments, cancellationToken: cancellationToken);
            var outline = outlineResult.ToString() ?? string.Empty;

            Logger.LogInformation("Successfully created outline for {Format} on topic: {Topic}", format, topic);
            
            return new ContentOutlineResult
            {
                Topic = topic,
                Format = format,
                Depth = depth,
                Outline = outline,
                SectionCount = CountSections(outline, depth),
                Success = true
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating outline for topic: {Topic}", topic);
            
            return new ContentOutlineResult
            {
                Topic = topic,
                Format = format,
                Depth = depth,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// Counts the number of words in a text.
    /// </summary>
    /// <param name="text">The text to count words in.</param>
    /// <returns>The word count.</returns>
    private int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }
        
        // Split by whitespace and count non-empty entries
        var words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        return words.Length;
    }

    /// <summary>
    /// Counts the number of sections in an outline.
    /// </summary>
    /// <param name="outline">The outline text.</param>
    /// <param name="depth">The depth level of the outline.</param>
    /// <returns>The section count.</returns>
    private int CountSections(string outline, int depth)
    {
        if (string.IsNullOrWhiteSpace(outline))
        {
            return 0;
        }
        
        // This is a very simple approximation
        var lines = outline.Split('\n');
        int count = 0;
        
        foreach (var line in lines)
        {
            // Count lines that look like sections (start with numbers, Roman numerals, or common bullet points)
            var trimmed = line.Trim();
            if (trimmed.Length > 0 && (char.IsDigit(trimmed[0]) || 
                                       trimmed.StartsWith("I.") || 
                                       trimmed.StartsWith("V.") || 
                                       trimmed.StartsWith("X.") ||
                                       trimmed.StartsWith("-") || 
                                       trimmed.StartsWith("•") ||
                                       trimmed.StartsWith("*")))
            {
                count++;
            }
        }
        
        return count;
    }
}

/// <summary>
/// Represents a content generation result.
/// </summary>
public class ContentGenerationResult
{
    /// <summary>
    /// Gets or sets the topic of the generated content.
    /// </summary>
    public string Topic { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the format of the generated content.
    /// </summary>
    public string Format { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the generated content.
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the word count of the generated content.
    /// </summary>
    public int WordCount { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the generation was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the error message if generation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Represents a content transformation result.
/// </summary>
public class ContentTransformationResult
{
    /// <summary>
    /// Gets or sets the original content before transformation.
    /// </summary>
    public string OriginalContent { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the transformed content.
    /// </summary>
    public string TransformedContent { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the transformation operation that was applied.
    /// </summary>
    public string Operation { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the word count of the original content.
    /// </summary>
    public int OriginalWordCount { get; set; }
    
    /// <summary>
    /// Gets or sets the word count of the transformed content.
    /// </summary>
    public int TransformedWordCount { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the transformation was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the error message if transformation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Represents a content outline creation result.
/// </summary>
public class ContentOutlineResult
{
    /// <summary>
    /// Gets or sets the topic of the outline.
    /// </summary>
    public string Topic { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the format the outline is designed for.
    /// </summary>
    public string Format { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the depth level of the outline.
    /// </summary>
    public int Depth { get; set; }
    
    /// <summary>
    /// Gets or sets the outline content.
    /// </summary>
    public string Outline { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the number of sections in the outline.
    /// </summary>
    public int SectionCount { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the outline creation was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the error message if outline creation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
} 