using System;
using System.Collections.Generic;

namespace MetaMeta.Core.Models;

/// <summary>
/// Base class for all agent response models.
/// </summary>
/// <remarks>
/// Provides common properties for tracking, identifying, and reporting the status of agent responses.
/// </remarks>
public abstract class AgentResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for the response.
    /// </summary>
    public string ResponseId { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Gets or sets the identifier of the request that triggered this response.
    /// </summary>
    public string RequestId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the session identifier for contextual tracking.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the error message if the operation failed.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the timestamp of when the response was created.
    /// </summary>
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
    
    /// <summary>
    /// Gets or sets additional metadata for the response.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
} 