using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaMeta.Core.Chat;

/// <summary>
/// Represents a message in a chat conversation.
/// </summary>
public class ChatMessageContent
{
    /// <summary>
    /// Initializes a new instance of the ChatMessageContent class.
    /// </summary>
    /// <param name="role">The role of the message sender.</param>
    /// <param name="content">The content of the message.</param>
    public ChatMessageContent(string role, string content)
    {
        Role = role;
        Content = content;
    }

    /// <summary>
    /// Gets or sets the role of the message sender.
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// Gets or sets the content of the message.
    /// </summary>
    public string Content { get; set; }
}

/// <summary>
/// Defines standard roles for chat participants.
/// </summary>
public static class ChatRoles
{
    /// <summary>
    /// The system role, typically used for instructions to the AI.
    /// </summary>
    public const string System = "System";

    /// <summary>
    /// The user role, representing the end user.
    /// </summary>
    public const string User = "User";

    /// <summary>
    /// The assistant role, representing the AI assistant.
    /// </summary>
    public const string Assistant = "Assistant";

    /// <summary>
    /// The function role, representing automated function responses.
    /// </summary>
    public const string Function = "Function";
} 