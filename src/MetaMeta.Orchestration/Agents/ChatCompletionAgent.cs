using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MetaMeta.Core.Abstractions;
using MetaMeta.Core.Chat;
using MetaMeta.Orchestration.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace MetaMeta.Orchestration.Agents;

/// <summary>
/// Agent responsible for chat-based interactions and conversations.
/// </summary>
public class ChatCompletionAgent
{
    private readonly Kernel _kernel;
    private readonly ILogger<ChatCompletionAgent> _logger;
    private readonly MetaMeta.Core.Abstractions.IPromptTemplateFactory _promptFactory;

    /// <summary>
    /// Initializes a new instance of the ChatCompletionAgent class.
    /// </summary>
    /// <param name="kernel">The semantic kernel instance.</param>
    /// <param name="logger">The logger for agent operations.</param>
    /// <param name="promptFactory">Factory for creating prompt templates.</param>
    public ChatCompletionAgent(
        Kernel kernel,
        ILogger<ChatCompletionAgent> logger,
        MetaMeta.Core.Abstractions.IPromptTemplateFactory promptFactory)
    {
        _kernel = kernel;
        _logger = logger;
        _promptFactory = promptFactory;
    }

    /// <summary>
    /// Gets the name of the agent.
    /// </summary>
    public string Name => "ChatCompletion";

    /// <summary>
    /// Gets the description of the agent.
    /// </summary>
    public string Description => "Handles chat-based interactions and conversations.";

    /// <summary>
    /// Completes a chat conversation based on a prompt and system instructions.
    /// </summary>
    /// <param name="prompt">The user prompt or messages.</param>
    /// <param name="systemInstruction">The system instruction to guide the AI's responses.</param>
    /// <param name="options">Additional options for completion.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The chat completion response.</returns>
    public async Task<ChatCompletionResponse> CompleteAsync(
        object prompt,
        string systemInstruction,
        object? options = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing chat completion");
        
        try
        {
            var promptTemplate = new StringBuilder();
            int messageCount = 1;
            
            // Handle different types of prompts
            if (prompt is string textPrompt)
            {
                // String prompt
                if (!string.IsNullOrEmpty(systemInstruction))
                {
                    promptTemplate.AppendLine($"System: {systemInstruction}");
                    promptTemplate.AppendLine();
                }
                
                promptTemplate.Append($"User: {textPrompt}");
            }
            else if (prompt is IEnumerable<MetaMeta.Core.Chat.ChatMessageContent> chatMessages)
            {
                // Process messages
                var messages = chatMessages.ToList();
                messageCount = messages.Count;
                
                foreach (var message in messages)
                {
                    promptTemplate.AppendLine($"{message.Role}: {message.Content}");
                }
            }
            else
            {
                throw new ArgumentException("Unsupported prompt type", nameof(prompt));
            }

            // Execute prompt through kernel
            var result = await _kernel.InvokePromptAsync(
                promptTemplate.ToString(),
                cancellationToken: cancellationToken);
            
            var content = result.GetValue<string>() ?? string.Empty;
            
            _logger.LogInformation("Completed chat interaction successfully");
            
            return new ChatCompletionResponse
            {
                Content = content,
                MessageCount = messageCount,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat completion");
            
            int messageCount = 1;
            if (prompt is IEnumerable<MetaMeta.Core.Chat.ChatMessageContent> chatMessages)
            {
                messageCount = chatMessages.Count();
            }
            
            return new ChatCompletionResponse
            {
                MessageCount = messageCount,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
    
    /// <summary>
    /// Creates a conversation with multiple turns based on an initial prompt and follow-up questions.
    /// </summary>
    /// <param name="initialPrompt">The initial prompt to start the conversation.</param>
    /// <param name="followUpQuestions">A list of follow-up questions or instructions.</param>
    /// <param name="systemInstruction">The system instruction to guide the AI's responses.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The multi-turn conversation results.</returns>
    public async Task<ConversationResult> CreateConversationAsync(
        string initialPrompt,
        IEnumerable<string> followUpQuestions,
        string systemInstruction,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating multi-turn conversation starting with prompt: {InitialPrompt}", initialPrompt);
        
        try
        {
            var chatHistory = new StringBuilder();
            var turns = new List<ConversationTurn>();
            
            // Add system instruction
            if (!string.IsNullOrEmpty(systemInstruction))
            {
                chatHistory.AppendLine($"System: {systemInstruction}");
                chatHistory.AppendLine();
            }
            
            // Process initial prompt
            chatHistory.AppendLine($"User: {initialPrompt}");
            
            // Get initial response
            var initialResponse = await _kernel.InvokePromptAsync(
                chatHistory.ToString(),
                cancellationToken: cancellationToken);
            
            var initialContent = initialResponse.GetValue<string>() ?? string.Empty;
            chatHistory.AppendLine($"Assistant: {initialContent}");
            chatHistory.AppendLine();
            
            // Add initial turn
            turns.Add(new ConversationTurn
            {
                UserMessage = initialPrompt,
                AssistantMessage = initialContent,
                TurnNumber = 1
            });
            
            // Process follow-up questions
            int turnNumber = 2;
            foreach (var question in followUpQuestions)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                    
                chatHistory.AppendLine($"User: {question}");
                
                var followUpResponse = await _kernel.InvokePromptAsync(
                    chatHistory.ToString(),
                    cancellationToken: cancellationToken);
                
                var followUpContent = followUpResponse.GetValue<string>() ?? string.Empty;
                chatHistory.AppendLine($"Assistant: {followUpContent}");
                chatHistory.AppendLine();
                
                turns.Add(new ConversationTurn
                {
                    UserMessage = question,
                    AssistantMessage = followUpContent,
                    TurnNumber = turnNumber++
                });
            }
            
            _logger.LogInformation("Completed conversation with {TurnCount} turns", turns.Count);
            
            return new ConversationResult
            {
                Turns = turns,
                TurnCount = turns.Count,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating conversation");
            
            return new ConversationResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}

/// <summary>
/// Represents the result of a chat completion request.
/// </summary>
public class ChatCompletionResponse
{
    /// <summary>
    /// Gets or sets the content of the response.
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the number of messages in the conversation.
    /// </summary>
    public int MessageCount { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the completion was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the error message if the completion failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Represents a single turn in a conversation.
/// </summary>
public class ConversationTurn
{
    /// <summary>
    /// Gets or sets the user's message in this turn.
    /// </summary>
    public string UserMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the assistant's response in this turn.
    /// </summary>
    public string AssistantMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the sequential number of this turn in the conversation.
    /// </summary>
    public int TurnNumber { get; set; }
}

/// <summary>
/// Represents the result of a multi-turn conversation.
/// </summary>
public class ConversationResult
{
    /// <summary>
    /// Gets or sets the turns in the conversation.
    /// </summary>
    public List<ConversationTurn> Turns { get; set; } = new List<ConversationTurn>();
    
    /// <summary>
    /// Gets or sets the total number of turns in the conversation.
    /// </summary>
    public int TurnCount { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the conversation was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the error message if the conversation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
} 