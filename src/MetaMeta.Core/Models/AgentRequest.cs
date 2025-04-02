using System.Collections.Generic;

namespace MetaMeta.Core.Models;

/// <summary>
/// Base class for all agent request models.
/// </summary>
/// <remarks>
/// Provides common properties for tracking and identifying requests to agents.
/// </remarks>
public abstract class AgentRequest
{
    /// <summary>
    /// Gets or sets the unique identifier for the request.
    /// </summary>
    public string RequestId { get; set; } = System.Guid.NewGuid().ToString();
    
    /// <summary>
    /// Gets or sets the session identifier for contextual tracking.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the assistant name or identifier.
    /// </summary>
    public string Assistant { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets additional metadata for the request.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
} 