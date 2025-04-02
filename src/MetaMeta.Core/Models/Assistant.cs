using MetaMeta.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaMeta.Core.Models;

/// <summary>
/// Represents an AI assistant entity within the MetaMeta system.
/// </summary>
/// <remarks>
/// Assistants are the core AI agents that interact with users and perform tasks.
/// Each assistant has text content, vector embeddings for semantic search,
/// and associated metadata for configuration and capabilities.
/// </remarks>
public class Assistant: BaseEntity
{
    /// <summary>
    /// The textual content or description of the assistant.
    /// </summary>
    public required string Text { get; set; }
    
    /// <summary>
    /// Vector embedding representation of the assistant for semantic search operations.
    /// </summary>
    public required float[] Embedding { get; set; }
    
    /// <summary>
    /// Additional metadata associated with the assistant including capabilities,
    /// configuration options, and contextual information.
    /// </summary>
    public required Dictionary<string, object> Metadata { get; set; }
}
