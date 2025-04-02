using System.Collections.Generic;
using MetaMeta.Core.Models;

namespace MetaMeta.Orchestration.Models;

/// <summary>
/// Represents a response from a memory operation.
/// </summary>
public class MemoryResponse : AgentResponse
{
    /// <summary>
    /// Gets or sets the memory operation type that was performed.
    /// </summary>
    public MemoryOperation Operation { get; set; }

    /// <summary>
    /// Gets or sets the memory collection that was operated on.
    /// </summary>
    public string Collection { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the key for the memory item.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value retrieved or stored.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the search query for search operations.
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the results for search operations.
    /// </summary>
    public List<MemoryItem> Results { get; set; } = new List<MemoryItem>();
} 