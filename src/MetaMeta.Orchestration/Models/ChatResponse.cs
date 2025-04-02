using MetaMeta.Core.Models;
using Microsoft.SemanticKernel.Agents;
using System.Collections.Generic;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Response model for chat completion operations.
/// </summary>
public class ChatResponse : AgentResponse
{
    /// <summary>
    /// Gets or sets the session identifier for the conversation.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the response message from the assistant.
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the list of messages in the conversation.
    /// </summary>
    /// <remarks>
    /// Includes context from previous turns and the current response.
    /// </remarks>
    public List<ChatMessageItem> Messages { get; set; } = new();
}

/// <summary>
/// Represents a message in a chat conversation.
/// </summary>
public class ChatMessageItem
{
    /// <summary>
    /// Gets or sets the role of the message sender (system, user, or assistant).
    /// </summary>
    public string Role { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the content of the message.
    /// </summary>
    public string Content { get; set; } = string.Empty;
} 