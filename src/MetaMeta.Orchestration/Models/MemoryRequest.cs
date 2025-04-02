using System.Collections.Generic;
using MetaMeta.Core.Models;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents a request for a memory operation.
/// </summary>
public class MemoryRequest : AgentRequest
{
    /// <summary>
    /// Gets or sets the memory operation type.
    /// </summary>
    public MemoryOperation Operation { get; set; }

    /// <summary>
    /// Gets or sets the memory collection to operate on.
    /// </summary>
    public string Collection { get; set; } = "default";

    /// <summary>
    /// Gets or sets the key for the memory item.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value for storing or updating.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the search query for search operations.
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the maximum number of results to return for search operations.
    /// </summary>
    public int Limit { get; set; } = 5;
} 